/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: EditableMessageFieldDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Fix.Dictionary;

namespace FixClient
{
    public partial class EditableMessageFieldDataGridView : MessageFieldDataGridView
    {
        readonly ToolTip _toolTip = new();

        public EditableMessageFieldDataGridView()
        {
            InitializeComponent();
        }

        public Fix.Message Message { get; set; }

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            //
            // This fires if we enter an invalid value for an enumerated field. Suppress it so we don't get an
            // error dialog.
            //
        }

        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control as ComboBox == null)
                return;
            (e.Control as ComboBox).DrawMode = DrawMode.OwnerDrawFixed;
            (e.Control as ComboBox).DrawItem -= ComboBoxDrawItem;
            (e.Control as ComboBox).DrawItem += ComboBoxDrawItem;
            (e.Control as ComboBox).DropDownClosed -= ComboBoxDropDownClosed;
            (e.Control as ComboBox).DropDownClosed += ComboBoxDropDownClosed;
        }

        void ComboBoxDropDownClosed(object? sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                _toolTip.Hide(comboBox);
            }
        }

        void ComboBoxDrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            if (sender is not ComboBox comboBox)
                return;

            string text = comboBox.GetItemText(comboBox.Items[e.Index]);

            e.DrawBackground();

            using (SolidBrush br = new(e.ForeColor))
            {
                e.Graphics.DrawString(text, e.Font, br, e.Bounds);
            }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                if (comboBox.Items[e.Index] is not EnumDescription enumDescription)
                {
                    return;
                }

                var tooltipText = $"{enumDescription.Value} - {enumDescription.Description}".SplitInParts(100);

                Point position = comboBox.PointToClient(Cursor.Position);
                position.Y += 40;
                _toolTip.Show(tooltipText, comboBox, position);
            }
            else
            {
                _toolTip.Hide(comboBox);
            }

            e.DrawFocusRectangle();
        }

        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            //
            // Prevent the combobox from showing in the description column if this field
            // is not an enumeration.
            //
            if (Columns[e.ColumnIndex].Name != FieldDataTable.ColumnDescription)
                return;

            Fix.Field field = FieldAtIndex(e.RowIndex);

            var definition = FIX_5_0SP2.Fields[field.Tag];

            if (definition == null || definition.Values.Count == 0)
            {
                e.Cancel = true;
            }
        }

        protected override void OnUserDeletingRow(DataGridViewRowCancelEventArgs e)
        {
            //
            // We can never actually delete rows, just use this as a shortcut to clear the value.
            //
            e.Cancel = true;
            e.Row.Cells[FieldDataTable.ColumnValue].Value = string.Empty;
        }

        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            for (int index = e.RowIndex; index < e.RowIndex + e.RowCount; ++index)
            {
                Fix.Field field = FieldAtIndex(index);

                if (field is null)
                    continue;

                var definition = FIX_5_0SP2.Fields[field.Tag];

                if (definition == null)
                    continue;

                if (definition.Values.Count == 0)
                    continue;

                DataGridViewRow row = Rows[index];

                if (row.Cells[FieldDataTable.ColumnDescription] is DataGridViewComboBoxCell cell)
                {
                    var collection = new EnumDescriptionCollection(definition);
                    cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
                    cell.ReadOnly = false;
                    cell.DataSource = collection;

                    if (field.Value != null && definition.Values.TryGetValue(field.Value, out var valueDefinition))
                    {
                        InternalChange = true;
                        cell.Value = new EnumDescription(valueDefinition.Name, field.Value, valueDefinition.Description);
                        InternalChange = false;
                    }
                }
            }
        }

        bool InternalChange { get; set; }

        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            if (InternalChange)
                return;

            if (Message == null)
                return;

            DataGridViewColumn column = Columns[e.ColumnIndex];
            DataGridViewRow row = Rows[e.RowIndex];

            if (row.DataBoundItem is not DataRowView rowView)
                return;

            if (rowView.Row is not FieldDataRow dataRow)
                return;
            //
            // We need to get the index of the row in the underlying table not the view as the view
            // may have been filtered.
            //
            var view = DataSource as DataView;
            DataTable table = view.Table;
            int index = table.Rows.IndexOf(dataRow);

            Fix.Field field = dataRow.Field;

            if (column.Name == FieldDataTable.ColumnDescription)
            {
                //
                // The user has selected an item in a combo box for an field with an enumerated value so
                // update the source field as well.
                //
                dataRow[FieldDataTable.ColumnValue] = (string)CurrentCell.Value;
                Message.Fields[index].Value = (string)CurrentCell.Value;
                return;
            }

            if (column.Name != FieldDataTable.ColumnValue)
                return;

            DataGridViewCell cell = row.Cells[FieldDataTable.ColumnValue];

            string value = cell.Value == null ? "" : cell.Value.ToString().Trim();

            var definition = FIX_5_0SP2.Fields[field.Tag];

            if (!string.IsNullOrEmpty(value) && definition != null)
            {
                if (definition.DataType == FIX_5_0SP2.DataTypes.Int.Name ||
                    definition.DataType == FIX_5_0SP2.DataTypes.Length.Name ||
                    definition.DataType == FIX_5_0SP2.DataTypes.SeqNum.Name)
                {
                    if (!int.TryParse(value, out var _))
                    {
                        MessageBox.Show(this,
                                        string.Format("{0} must be an integer", definition.Name),
                                        Application.ProductName,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        cell.Value = "";
                        return;
                    }
                }
            }

            Message.Fields[index].Value = value;
            //
            // The bodylength changes whenever the message fields change so update it.
            //
            if (field.Tag == FIX_5_0SP2.Fields.BodyLength.Tag)
                return;

            foreach (DataGridViewRow r in Rows)
            {
                var rv = r.DataBoundItem as DataRowView;
                dataRow = rv.Row as FieldDataRow;

                if (dataRow.Field.Tag == FIX_5_0SP2.Fields.BodyLength.Tag)
                {
                    dataRow[FieldDataTable.ColumnValue] = Message.ComputeBodyLength();
                    break;
                }
            }
            //
            // Enumerated values such as 'OrderSingle.Side' have a description.
            //
            if (definition == null)
                return;

            if (row.Cells[FieldDataTable.ColumnDescription] is not DataGridViewComboBoxCell comboCell)
                return;

            if (CurrentCell != null && CurrentCell.Value != DBNull.Value && definition.Values.Count > 0)
            {
                comboCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
                comboCell.FlatStyle = FlatStyle.Flat;
                comboCell.ReadOnly = false;
                comboCell.DataSource = new EnumDescriptionCollection(definition);
                comboCell.ValueMember = FieldDataTable.ColumnValue;
                comboCell.DisplayMember = FieldDataTable.ColumnName;
                comboCell.Value = CurrentCell.Value;
            }
            else
            {
                comboCell.Value = null;
            }
        }
    }
}
