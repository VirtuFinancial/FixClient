/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: OrderDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace FixClient
{
	public sealed partial class OrderDataGridView : DataGridView
	{
		public OrderDataGridView()
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
			MultiSelect = false;
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

        public bool DisplayExDestination { get; set; }

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

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);

            DataGridViewColumn column = e.Column;

            switch (column.Name)
            {
                case OrderDataTable.ColumnSymbol:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;

                case OrderDataTable.ColumnExDestination:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.Visible = DisplayExDestination;
                    break;

                case OrderDataTable.ColumnTimeInForce:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;

                case OrderDataTable.ColumnOrdStatus:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;

                case OrderDataTable.ColumnSide:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;

                case OrderDataTable.ColumnQuantity:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;

                case OrderDataTable.ColumnLimit:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;

                case OrderDataTable.ColumnAvgPrice:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    column.HeaderText = "Avg Price";
                    break;

                case OrderDataTable.ColumnDone:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;

                case OrderDataTable.ColumnLeaves:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;

                case OrderDataTable.ColumnClOrdId:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;

                case OrderDataTable.ColumnOrigClOrdId:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;

                case OrderDataTable.ColumnListId:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;

                case OrderDataTable.ColumnText:
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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

	        if (column.Name == OrderDataTable.ColumnQuantity)
	        {
                object quantityValue = row[OrderDataTable.ColumnQuantity];
                object pendingQuantityValue = row[OrderDataTable.ColumnPendingQuantity];

                if (quantityValue != DBNull.Value && pendingQuantityValue != DBNull.Value)
                {
                    int quantity = Convert.ToInt32(quantityValue);
                    int pendingQuantity = Convert.ToInt32(pendingQuantityValue);
                    e.Value = string.Format("{0} ({1})", quantity, pendingQuantity);
                    e.FormattingApplied = true;
                }

                return;
	        }

            if (column.Name == OrderDataTable.ColumnLimit)
            {
                object limitValue = row[OrderDataTable.ColumnLimit];
                object pendingLimitValue = row[OrderDataTable.ColumnPendingLimit];

                if (limitValue != DBNull.Value && pendingLimitValue != DBNull.Value)
                {
                    decimal limit = Convert.ToDecimal(limitValue);
                    decimal pendingLimit = Convert.ToDecimal(pendingLimitValue);
                    e.Value = string.Format("{0} ({1})", limit, pendingLimit);
                    e.FormattingApplied = true;
                }

                return;
            }

            if (column.Name == OrderDataTable.ColumnSide)
            {
                Color color = LookAndFeel.Color.GridCellForeground;

                if (e.Value == null || e.Value is DBNull)
                {
                    e.Value = "Unknown";
                    color = LookAndFeel.Color.Unknown;
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
            
            if (column.Name == OrderDataTable.ColumnOrdStatus)
            {
                Color color = LookAndFeel.Color.GridCellForeground;

                if (e.Value == null || e.Value is DBNull)
                {
                    //
                    // This is not an actual Fix state
                    //
                    e.Value = "Pending";
                    color = LookAndFeel.Color.Pending;
                }
                else
                {
                    var status = (Fix.OrdStatus)e.Value;

                    switch (status)
                    {
                        case Fix.OrdStatus.Calculated:
                        case Fix.OrdStatus.Canceled:
                        case Fix.OrdStatus.DoneForDay:
                        case Fix.OrdStatus.Filled:
                        case Fix.OrdStatus.Replaced:
                            break;

                        case Fix.OrdStatus.New:
                        case Fix.OrdStatus.PartiallyFilled:
                            color = LookAndFeel.Color.New;
                            break;

                        case Fix.OrdStatus.PendingNew:
                        case Fix.OrdStatus.PendingCancel:
                        case Fix.OrdStatus.PendingReplace:
                            color = LookAndFeel.Color.Pending;
                            break;

                        case Fix.OrdStatus.Expired:
                        case Fix.OrdStatus.Rejected:
                        case Fix.OrdStatus.Stopped:
                        case Fix.OrdStatus.Suspended:
                            color = LookAndFeel.Color.Rejected;
                            break;
                    }

                    e.Value = status.ToString();
                }
                
                e.CellStyle.ForeColor = color;
                e.FormattingApplied = true;
                return;
            }

            if (column.Name == OrderDataTable.ColumnTimeInForce)
            {
                if (e.Value != null && e.Value != DBNull.Value)
                {
                    var timeInForce = (Fix.TimeInForce)e.Value;
                    e.Value = ShortTimeInForceDescription(timeInForce);
                    e.FormattingApplied = true;
                    return;
                }
            }

            base.OnCellFormatting(e);
        }

        public static string ShortTimeInForceDescription(Fix.TimeInForce timeInForce)
        {
            switch (timeInForce)
            {
                case Fix.TimeInForce.AtTheOpening:
                    return "ATO";

                case Fix.TimeInForce.Day:
                    return "DAY";

                case Fix.TimeInForce.FillOrKill:
                    return "FOK";

                case Fix.TimeInForce.GoodTillCancel:
                    return "GTC";

                case Fix.TimeInForce.GoodTillCrossing:
                    return "GTX";

                case Fix.TimeInForce.GoodTillDate:
                    return "GTD";

                case Fix.TimeInForce.ImmediateOrCancel:
                    return "IOC";

                case Fix.TimeInForce.AtTheClose:
                    return "ATC";
            }

            return timeInForce.ToString();
        }
    }
}