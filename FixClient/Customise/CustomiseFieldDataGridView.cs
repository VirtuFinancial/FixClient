/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CustomiseFieldDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Drawing;
using System.Windows.Forms;

namespace FixClient
{
    public partial class CustomiseFieldDataGridView : DataGridView
    {
        public const string ColumnFieldId = "Id";
        public const string ColumnFieldName = "Name";

        public CustomiseFieldDataGridView()
        {
            InitializeComponent();

            BackgroundColor = LookAndFeel.Color.GridCellBackground;
            BorderStyle = BorderStyle.None;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MultiSelect = false;
            RowHeadersVisible = false;
            ColumnHeadersVisible = false;
            DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            RowTemplate.Resizable = DataGridViewTriState.False;
            AutoGenerateColumns = false;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            AllowUserToAddRows = false;
            BackgroundColor = LookAndFeel.Color.GridCellBackground;
            DefaultCellStyle.Padding = new Padding(3, 0, 3, 0);
            DefaultCellStyle.Font = new Font("Arial", 8);
            RowTemplate.Height -= 3;
            BackgroundColor = LookAndFeel.Color.GridCellBackground;
            GridColor = LookAndFeel.Color.Grid;
            DefaultCellStyle.BackColor = LookAndFeel.Color.GridCellBackground;
            DefaultCellStyle.ForeColor = LookAndFeel.Color.GridCellForeground;
            DefaultCellStyle.SelectionBackColor = LookAndFeel.Color.GridCellSelectedBackground;
            DefaultCellStyle.SelectionForeColor = LookAndFeel.Color.GridCellSelectedForeground;
            DoubleBuffered = true;
            ReadOnly = true;
            AutoGenerateColumns = true;
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            DataGridViewColumn column = e.Column;

            switch (column.Name)
            {
                case CustomFieldDataTable.ColumnNameTag:
                    e.Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    break;

                case CustomFieldDataTable.ColumnNameName:
                    e.Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    break;
            }

            base.OnColumnAdded(e);
        }
    }
}