/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: MessageTypeDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Drawing;
using System.Windows.Forms;

namespace FixClient
{
    public partial class MessageTypeDataGridView : DataGridView
    {
        public MessageTypeDataGridView()
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
            BackgroundColor = LookAndFeel.Color.GridCellBackground;
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MultiSelect = false;
            RowHeadersVisible = false;
            DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            GridColor = LookAndFeel.Color.Grid;
            RowTemplate.Resizable = DataGridViewTriState.False;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            RowTemplate.Height -= 3;
            DefaultCellStyle.BackColor = LookAndFeel.Color.GridCellBackground;
            DefaultCellStyle.ForeColor = LookAndFeel.Color.GridCellForeground;
            DefaultCellStyle.Padding = new Padding(3, 0, 3, 0);
            DefaultCellStyle.SelectionBackColor = LookAndFeel.Color.GridCellSelectedBackground;
            DefaultCellStyle.SelectionForeColor = LookAndFeel.Color.GridCellSelectedForeground;
            ReadOnly = true;
            ShowCellToolTips = false;
        }

        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                var source = DataSource as BindingSource;
                if (source != null)
                {
                    source.Sort = string.Empty;
                    Refresh();
                    return;
                }
            }

            base.OnColumnHeaderMouseClick(e);
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);

            DataGridViewColumn column = e.Column;

            switch (column.Name)
            {
                case MessageTypeDataTable.ColumnMsgType:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    break;

                case MessageTypeDataTable.ColumnMsgTypeDescription:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    break;
            }
        }
    }
}
