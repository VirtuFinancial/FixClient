/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: MessageFieldDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;

namespace FixClient
{
    public partial class MessageFieldDataGridView : DataGridView
    {
        public MessageFieldDataGridView()
        {
            InitializeComponent();

            EnableHeadersVisualStyles = false;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            ColumnHeadersDefaultCellStyle.BackColor = LookAndFeel.Color.GridColumnHeader;
            ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersHeight -= 3;
            Dock = DockStyle.Fill;
            BackgroundColor = LookAndFeel.Color.GridCellBackground;
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MultiSelect = true;
            RowHeadersVisible = false;
            DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            RowTemplate.Resizable = DataGridViewTriState.False;
            GridColor = LookAndFeel.Color.Grid;
            AllowUserToAddRows = false;
            DefaultCellStyle.Font = new Font("Arial", 8);
            EnableHeadersVisualStyles = false;
            RowTemplate.Height -= 3;
            DefaultCellStyle.BackColor = LookAndFeel.Color.GridCellBackground;
            DefaultCellStyle.Padding = new Padding(3, 0, 3, 0);
            DefaultCellStyle.SelectionBackColor = LookAndFeel.Color.GridCellSelectedBackground;
            DefaultCellStyle.SelectionForeColor = LookAndFeel.Color.GridCellSelectedForeground;
            EditMode = DataGridViewEditMode.EditOnEnter;
            AutoGenerateColumns = false;
            //
            // We need to manually define the columns so we can configure the ComboBox column.
            //
            DataGridViewColumn column;

            column = new DataGridViewIndentColumn
            {
                Name = FieldDataTable.ColumnIndent,
                DataPropertyName = FieldDataTable.ColumnIndent,
                ReadOnly = true,
                HeaderText = string.Empty,
                Width = 15
            };
            Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                Name = FieldDataTable.ColumnTag,
                DataPropertyName = FieldDataTable.ColumnTag,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                ReadOnly = true,
                DefaultCellStyle = {Alignment = DataGridViewContentAlignment.MiddleLeft}
            };
            Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                Name = FieldDataTable.ColumnName,
                DataPropertyName = FieldDataTable.ColumnName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                ReadOnly = true
            };
            Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                Name = FieldDataTable.ColumnValue,
                DataPropertyName = FieldDataTable.ColumnValue,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            };
            Columns.Add(column);

            var comboColumn = new DataGridViewComboBoxColumn
            {
                Name = FieldDataTable.ColumnDescription,
                DisplayMember = "Description",
                ValueMember = "Value",
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };

            Columns.Add(comboColumn);
        }

        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                var view = DataSource as DataView;
                if (view != null)
                {
                    view.Sort = string.Empty;
                    Refresh();
                    return;
                }
            }

            base.OnColumnHeaderMouseClick(e);
        }

        public Fix.Field FieldAtIndex(int rowIndex)
        {
            DataGridViewRow row = Rows[rowIndex];
            var rowView = row.DataBoundItem as DataRowView;

            if (rowView == null)
            {
                return null;
                // HMM - this was thrown when changing from the messages to the history view
                //throw new Exception(string.Format("MessageFieldDataGridView Row.DataBoundItem at index {0} is not a DataRowView ", rowIndex));
            }

            var dataRow = rowView.Row as FieldDataRow;

            if (dataRow == null)
            {
                throw new Exception(string.Format("MessageFieldDataGridView RowView.Row at index {0} is not a FieldDataRow", rowIndex));
            }

            return dataRow.Field;
        }

        protected string ToolTipForCell(int rowIndex, int columnIndex, char value)
        {
            DataGridViewRow row = Rows[rowIndex];
            var rowView = row.DataBoundItem as DataRowView;

            if (rowView == null || rowView.IsNew)
                return null;

            var dataRow = (FieldDataRow)rowView.Row;

            Fix.Field field = dataRow.Field;

            if (field.Definition == null)
                return null;

            Type type = field.Definition.EnumeratedType;

            if (type == null)
                return null;

            string name = type.GetEnumName(value);

            if (string.IsNullOrEmpty(name))
                return null;

            MemberInfo[] info = type.GetMember(name);

            if (info.Length < 1)
                return null;

            var attributes = info[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            string description = ((DescriptionAttribute)attributes[0]).Description;

            return string.Join("\n", description.SplitInParts(80));
        }

        protected override void OnCellToolTipTextNeeded(DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewColumn column = Columns[e.ColumnIndex];

            if (column.Name != FieldDataTable.ColumnDescription && column.Name != FieldDataTable.ColumnValue)
                return;

            DataGridViewCell cell = Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.Value == null || cell.Value == DBNull.Value)
                return;

            char enumValue;
            if (!Char.TryParse(cell.Value.ToString(), out enumValue))
            {
                if (!(cell.Value is int))
                    return;
                enumValue = Convert.ToChar(cell.Value);
            }

            e.ToolTipText = ToolTipForCell(e.RowIndex, e.ColumnIndex, enumValue);
        }
    
        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridViewRow row = Rows[e.RowIndex];

                var rowView = row.DataBoundItem as DataRowView;

                if (rowView == null || rowView.IsNew)
                    return;

                var dataRow = (FieldDataRow)rowView.Row;

                DataGridViewColumn column = Columns[e.ColumnIndex];

                if (column.Name != FieldDataTable.ColumnIndent)
                {
                    object customValue = dataRow[FieldDataTable.ColumnCustom];
                    bool custom = false;

                    if (customValue != null && customValue != DBNull.Value)
                    {
                        custom = (bool) customValue;
                    }

                    if (custom)
                    {
                        e.CellStyle.ForeColor = Color.Brown;
                    }
                    else
                    {
                        object requiredValue = dataRow[FieldDataTable.ColumnRequired];
                        var required = (bool) requiredValue;
                        e.CellStyle.ForeColor = required ? LookAndFeel.Color.Incoming : LookAndFeel.Color.Outgoing;
                    }
                }
            }
            finally
            {
                base.OnCellFormatting(e);
            }
        }
    }
}
