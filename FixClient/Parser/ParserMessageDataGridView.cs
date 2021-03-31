/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: ParserMessageDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace FixClient
{
	public sealed partial class ParserMessageDataGridView : DataGridView
	{
		public ParserMessageDataGridView()
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
			SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			MultiSelect = false;
			RowHeadersVisible = false;
			RowTemplate.Resizable = DataGridViewTriState.False;
	        GridColor = LookAndFeel.Color.Grid;
			AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
			ReadOnly = true;
			CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            DefaultCellStyle.Font = new Font("Arial", 8);
            DefaultCellStyle.BackColor = LookAndFeel.Color.GridCellBackground;
			DefaultCellStyle.Padding = new Padding(3, 0, 3, 0);
			DefaultCellStyle.SelectionBackColor = LookAndFeel.Color.GridCellSelectedBackground;
			DefaultCellStyle.SelectionForeColor = LookAndFeel.Color.GridCellSelectedForeground;
			DoubleBuffered = true;
            RowTemplate.Height -= 3;
            ShowCellToolTips = false;
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);

            DataGridViewColumn column = e.Column;

            switch(column.Name)
            {
                case ParserMessageDataTable.ColumnSendingTime:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    break;

                case ParserMessageDataTable.ColumnMsgSeqNum:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;

                case MessageDataTable.ColumnStatusImage:
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        column.DefaultCellStyle.NullValue = null;
                        var headerCell = new DataGridViewImageColumnHeaderCell();
                        column.DefaultHeaderCellType = headerCell.GetType();
                        column.HeaderCell = headerCell;
                        headerCell.Image = Properties.Resources.MessageStatusInfo;
                        headerCell.Value = null;
                    }
                    break;

                case ParserMessageDataTable.ColumnMsgTypeDescription:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    break;
            }

            base.OnColumnAdded(e);
        }
	}
}