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

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace FixClient
{
    public partial class EditableMessageFieldDataGridView : MessageFieldDataGridView
    {
        readonly ToolTip _toolTip = new ToolTip();

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
            var comboBox = e.Control as ComboBox;
            if (comboBox == null)
                return;
            comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox.DrawItem -= ComboBoxDrawItem;
            comboBox.DrawItem += ComboBoxDrawItem;
            comboBox.DropDownClosed -= ComboBoxDropDownClosed;
            comboBox.DropDownClosed += ComboBoxDropDownClosed;
        }

        void ComboBoxDropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                _toolTip.Hide(comboBox);
            }
        }
       
        void ComboBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            
            var comboBox = sender as ComboBox;
            
            if (comboBox == null)
                return;
            
            string text = comboBox.GetItemText(comboBox.Items[e.Index]);
            
            e.DrawBackground();

            using (SolidBrush br = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(text, e.Font, br, e.Bounds);
            }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                var enumDescription = comboBox.Items[e.Index] as EnumDescription;
                if (enumDescription == null)
                    return;
                string description = ToolTipForCell(CurrentCell.RowIndex, CurrentCell.ColumnIndex, Convert.ToChar(enumDescription.Value));
                Point position = comboBox.PointToClient(Cursor.Position);
                position.Y += 40;
                _toolTip.Show(description, comboBox, position);
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
            if(Columns[e.ColumnIndex].Name != FieldDataTable.ColumnDescription) 
                return;

            Fix.Field field = FieldAtIndex(e.RowIndex);

            if (field.Definition == null || field.Definition.EnumeratedType == null)
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

                if (field == null || field.Definition == null)
                    continue;

                Type enumType = field.Definition.EnumeratedType;

                if (enumType == null)
                    continue;

                DataGridViewRow row = Rows[index];

                var cell = row.Cells[FieldDataTable.ColumnDescription] as DataGridViewComboBoxCell;

                if (cell != null)
                {
                    var collection = new EnumDescriptionCollection(enumType);
                    cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
                    cell.ReadOnly = false;
                    cell.DataSource = collection;
                    
                    if (EnumTypeHasNumericValues(enumType))
                    {
                        int value;
                        if (int.TryParse(field.Value, out value))
                        {
                            InternalChange = true;
                            cell.Value = new EnumDescription(Enum.GetName(enumType, value), value);
                            InternalChange = false;
                        }
                    }
                    else
                    {
                        char value;
                        if (char.TryParse(field.Value, out value))
                        {
                            InternalChange = true;
                            cell.Value = new EnumDescription(Enum.GetName(enumType, value), value);
                            InternalChange = false;
                        }
                    }
                }
            }
        }

        bool InternalChange { get; set; }

        bool EnumTypeHasNumericValues(Type enumType)
        {
            // TODO - we need a better way of doing type equality for version specific enums against the global definitions
            return enumType.Name == typeof(Fix.TrdType).Name || 
                   enumType.Name == typeof(Fix.SessionStatus).Name;
        }

        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            if (InternalChange)
                return;

            if (Message == null)
                return;

            DataGridViewColumn column = Columns[e.ColumnIndex];
            DataGridViewRow row = Rows[e.RowIndex];
            var rowView = row.DataBoundItem as DataRowView;
            
            if (rowView == null)
                return;
                
            var dataRow = rowView.Row as FieldDataRow;

            if (dataRow == null)
                return;
            //
            // We need to get the index of the row in the underlying table not the view as the view
            // may have been filtered.
            //
            var view = DataSource as DataView;
            DataTable table = view.Table;
            int index = table.Rows.IndexOf(dataRow);

            Fix.Field field = dataRow.Field;
            Type enumType = field.Definition?.EnumeratedType;

            if (column.Name == FieldDataTable.ColumnDescription)
            {
                //
                // The user has selected an item in a combo box for an field with an enumerated value so
                // update the source field as well.
                //
                object raw = CurrentCell.Value;
                string converted = CurrentCell.Value.ToString();

                if (EnumTypeHasNumericValues(enumType))
                {
                    converted = Convert.ToInt32(CurrentCell.Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    if (raw is int)
                    {
                        converted = (Convert.ToChar(Convert.ToInt32(CurrentCell.Value))).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                
                dataRow[FieldDataTable.ColumnValue] = converted;
                Message.Fields[index].Value = converted;
                return;
            }
                
            if (column.Name != FieldDataTable.ColumnValue)
                return;
            
            DataGridViewCell cell = row.Cells[FieldDataTable.ColumnValue];

            string value = cell.Value == null ? "" : cell.Value.ToString().Trim();

            if (!string.IsNullOrEmpty(value) && field.Definition != null)
            {
                if (field.Definition.DataType == Fix.Dictionary.DataTypes.Int ||
                    field.Definition.DataType == Fix.Dictionary.DataTypes.Length ||
                    field.Definition.DataType == Fix.Dictionary.DataTypes.SeqNum)
                {
                    int i;
                    if (!int.TryParse(value, out i))
                    {
                        MessageBox.Show(this,
                                        string.Format("{0} must be an integer", field.Definition.Name),
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
            if (field.Tag == Fix.Dictionary.Fields.BodyLength.Tag)
                return;

            foreach (DataGridViewRow r in Rows)
            {
                var rv = r.DataBoundItem as DataRowView;
                dataRow = rv.Row as FieldDataRow;

                if (dataRow.Field.Tag == Fix.Dictionary.Fields.BodyLength.Tag)
                {
                    dataRow[FieldDataTable.ColumnValue] = Message.ComputeBodyLength();
                    break;
                }
            }
            //
            // Enumerated values such as 'OrderSingle.Side' have a description.
            //
            if (field.Definition == null)
                return;

            var comboCell = row.Cells[FieldDataTable.ColumnDescription] as DataGridViewComboBoxCell;

            if (comboCell == null)
                return;

            if (CurrentCell != null && CurrentCell.Value != DBNull.Value && enumType != null)
            {
                comboCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
                comboCell.FlatStyle = FlatStyle.Flat;
                comboCell.ReadOnly = false;
                comboCell.DataSource = new EnumDescriptionCollection(enumType);
                comboCell.ValueMember = FieldDataTable.ColumnValue;
                comboCell.DisplayMember = FieldDataTable.ColumnDescription;
                
                if (EnumTypeHasNumericValues(enumType))
                {
                    int i;
                    if(int.TryParse(CurrentCell.Value.ToString(), out i))
                    {
                        comboCell.Value = i;
                    }
                    else
                    {
                        comboCell.Value = null;
                    }
                }
                else
                {
                    char c;
                    if (char.TryParse(CurrentCell.Value.ToString(), out c))
                    {
                        comboCell.Value = Convert.ToInt32(c);
                    }
                    else
                    {
                        comboCell.Value = null;
                    }
                }
            }
            else
            {
                comboCell.Value = null;
            }
        }
    }
}
