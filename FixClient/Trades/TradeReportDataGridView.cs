/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: TradeReportDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace FixClient
{
	public sealed partial class TradeReportDataGridView : DataGridView
	{
		public TradeReportDataGridView()
		{
			InitializeComponent();

            EnableHeadersVisualStyles = false;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            ColumnHeadersDefaultCellStyle.BackColor = LookAndFeel.Color.GridColumnHeader;
            ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ColumnHeadersHeight -= 3;
			MultiSelect = true;
			SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			AllowUserToAddRows = false;
			RowHeadersVisible = false;
			BorderStyle = BorderStyle.None;
			CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
			AllowUserToDeleteRows = false;
			RowTemplate.Resizable = DataGridViewTriState.False;
	        DefaultCellStyle.SelectionBackColor = LookAndFeel.Color.GridCellSelectedBackground;
            DefaultCellStyle.SelectionForeColor = LookAndFeel.Color.GridCellSelectedForeground;
            DefaultCellStyle.BackColor = LookAndFeel.Color.GridCellBackground;
            DefaultCellStyle.ForeColor = LookAndFeel.Color.GridCellForeground;
            BackgroundColor = LookAndFeel.Color.GridCellBackground;
            GridColor = LookAndFeel.Color.Grid;
            DefaultCellStyle.WrapMode = DataGridViewTriState.False;
			DefaultCellStyle.Font = new Font("Arial", 8);
			RowTemplate.Height -= 3;
			DoubleBuffered = true;
            ReadOnly = true;
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);

            DataGridViewColumn column = e.Column;

            switch (column.Name)
            {
                case TradeReportDataTable.ColumnSymbol:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;

                case OrderDataTable.ColumnSide:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;
                    
                case TradeReportDataTable.ColumnLastQty:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;

                case TradeReportDataTable.ColumnLastPx:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;

                case TradeReportDataTable.ColumnTrdType:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;
                  
                case TradeReportDataTable.ColumnTradeReportId:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;

                case TradeReportDataTable.ColumnPartyId:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;

                case TradeReportDataTable.ColumnText:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    break;

            }
        }

	    protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
	    {
	        DataGridViewColumn column = Columns[e.ColumnIndex];
            var rowView = Rows[e.RowIndex].DataBoundItem as DataRowView;

            if (rowView == null)
                return;

            DataRow row = rowView.Row;
          
            if (column.Name == OrderDataTable.ColumnSide)
            {
                Color color = LookAndFeel.Color.GridCellForeground;

                if (e.Value == null || e.Value is DBNull)
                {
                    e.Value = "Unknown";
                    color = Color.Orange;
                }
                else
                {
                    var side = (Fix.Side)e.Value;

                    switch (side)
                    {
                        case Fix.Side.Buy:
                        case Fix.Side.Cross:
                            color = LookAndFeel.Color.Bid;
                            break;

                        case Fix.Side.Sell:
                            color = LookAndFeel.Color.Ask;
                            break;

                        case Fix.Side.Undisclosed:
                            break;
                    }

                    e.Value = side.ToString();
                }

                e.CellStyle.ForeColor = color;
                e.FormattingApplied = true;
                return;
            }
      
            base.OnCellFormatting(e);
        }
 	}
}