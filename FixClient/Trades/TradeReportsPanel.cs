/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: TradeReportsPanel.cs
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FixClient
{
    partial class TradeReportsPanel : FixClientPanel
    {
        readonly TradeReportDataGridView _tradeReportGrid;
        readonly TradeReportDataTable _tradeReportTable;
        readonly DataView _tradeReportView;

        readonly FieldDataTable _fieldTable;
        readonly DataView _fieldView;
        readonly FieldDataGridView _fieldGrid;

        readonly SearchTextBox _tradeReportSearchTextBox;
        readonly SearchTextBox _fieldSearchTextBox;

        readonly SplitContainer _splitter;
        bool _splitterSet;

        readonly MessagesPanel _messageDefaults;
        readonly ToolStripButton _defaultsButton;

        Session _session;

        readonly ToolStrip _acceptorToolStrip;

        readonly ToolStripButton _acceptorRejectButton;
        readonly ToolStripButton _acceptorAckButton;

        readonly ToolStripButton _acceptorReplyButton;

        readonly ToolStripMenuItem _acceptorAckMenuItem;
        readonly ToolStripMenuItem _acceptorRejectMenuItem;
        readonly ToolStripMenuItem _acceptorReplyMenuItem;

        readonly ToolStripMenuItem _acceptorMenuStrip;

        public TradeReportsPanel(MessagesPanel messageDefaults, ToolStripButton defaultsButton)
        {
            _messageDefaults = messageDefaults;
            _defaultsButton = defaultsButton;

            InitializeComponent();

            #region ToolStrip
         
            _acceptorToolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = LookAndFeel.Color.ToolStrip,
                Renderer = new ToolStripRenderer()
            };

            _acceptorAckButton = new ToolStripButton
            {
                Text = "Ack",
                ToolTipText = "Acknowledge the selected trade report side(s)"
            };
            _acceptorAckButton.Click += AcceptorAckButtonClick;

            _acceptorRejectButton = new ToolStripButton
            {
                Text = "Reject",
                ToolTipText = "Reject the selected trade report side(s)"
            };
            _acceptorRejectButton.Click += AcceptorRejectButtonClick;

            _acceptorReplyButton = new ToolStripButton
            {
                Text = "Reply",
                ToolTipText = "Reply to the selected trade report sides(s) with another TradeCaptureReport"
            };
            _acceptorReplyButton.Click += AcceptorReplyButtonClick;

            _acceptorToolStrip.Items.AddRange(new ToolStripItem[] 
            {    
                //_acceptorAckButton,
                _acceptorRejectButton,
                _acceptorReplyButton
            });

            #endregion

            #region MenuStrip
           
            _acceptorMenuStrip = new ToolStripMenuItem("Action");

            _acceptorAckMenuItem = new ToolStripMenuItem("Acknowledge", _acceptorAckButton.Image);
            _acceptorAckMenuItem.Click += AcceptorAckButtonClick;
            _acceptorMenuStrip.DropDownItems.Add(_acceptorAckMenuItem);

            _acceptorRejectMenuItem = new ToolStripMenuItem("Reject", _acceptorRejectButton.Image);
            _acceptorRejectMenuItem.Click += AcceptorRejectButtonClick;
            _acceptorMenuStrip.DropDownItems.Add(_acceptorRejectMenuItem);

            _acceptorReplyMenuItem = new ToolStripMenuItem("Reply", _acceptorReplyButton.Image);
            _acceptorReplyMenuItem.Click += AcceptorReplyButtonClick;
            _acceptorMenuStrip.DropDownItems.Add(_acceptorReplyMenuItem);

            #endregion

            _tradeReportSearchTextBox = new SearchTextBox
            {
                Dock = DockStyle.Top,
            };
            _tradeReportSearchTextBox.TextChanged += TradeReportSearchTextBoxTextChanged;
            _fieldSearchTextBox = new SearchTextBox
            {
                Dock = DockStyle.Top,
            };
            _fieldSearchTextBox.TextChanged += FieldSearchTextBoxTextChanged;

            _tradeReportTable = new TradeReportDataTable("Trades");
            _tradeReportView = new DataView(_tradeReportTable);

            _tradeReportGrid = new TradeReportDataGridView
            {
                Dock = DockStyle.Fill,
                //VirtualMode = true,
                DataSource = _tradeReportView
            };
            _tradeReportGrid.SelectionChanged += TradeReportViewSelectionChanged;

            _fieldTable = new FieldDataTable("Fields");
            _fieldView = new DataView(_fieldTable);

            _fieldGrid = new FieldDataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = _fieldView
            };

            _splitter = new SplitContainer
            {
                Dock = DockStyle.Fill
            };

            _splitter.Panel1.Controls.Add(_tradeReportGrid);
            _splitter.Panel1.Controls.Add(_tradeReportSearchTextBox);
            _splitter.Panel2.Controls.Add(_fieldGrid);
            _splitter.Panel2.Controls.Add(_fieldSearchTextBox);

            ContentPanel.Controls.Add(_splitter);

            UpdateUiState();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!_splitterSet)
            {
                _splitter.SplitterDistance = 800;
                _splitterSet = true;
            }
        }

        void ApplyFieldSearch()
        {
            string search = null;
            if (string.IsNullOrEmpty(_fieldSearchTextBox.Text))
            {
                _fieldView.Sort = string.Empty;
            }
            else
            {
                search = string.Format("CONVERT({0}, System.String) LIKE '%{3}%' OR {1} LIKE '%{3}%' OR {2} LIKE '%{3}%'",
                                           FieldDataTable.ColumnTag,
                                           FieldDataTable.ColumnName,
                                           FieldDataTable.ColumnValue,
                                           _fieldSearchTextBox.Text);
            }
            _fieldView.RowFilter = search;
        }

        void FieldSearchTextBoxTextChanged(object sender, EventArgs e)
        {
            if (_fieldView == null || SelectedReportSide == null)
                return;
            ApplyFieldSearch();
            _fieldSearchTextBox.Focus();
        }

        void ApplyTradeReportSearch()
        {
            string search = null;

            string text = _tradeReportSearchTextBox.Text;

            if (string.IsNullOrEmpty(text))
            {
                _tradeReportView.Sort = string.Empty;
            }
            else
            {
                var buffer = new StringBuilder();
                foreach (DataColumn column in _tradeReportView.Table.Columns)
                {
                    if (column.ColumnMapping == MappingType.Hidden)
                        continue;

                    if (column.DataType.IsEnum)
                    {
                        buffer.AppendFormat("{0} LIKE '%{1}%' OR ", column.ColumnName + "String", text);
                    }
                    else
                    {
                        buffer.AppendFormat("CONVERT({0}, System.String) LIKE '%{1}%' OR ", column.ColumnName, text);
                    }
                }
                buffer.Remove(buffer.Length - 3, 3);
                search = buffer.ToString();
            }

            //_tradeReportGrid.RowCount = 0;
            _tradeReportView.RowFilter = search;
            //_tradeReportGrid.RowCount = _tradeReportView.Count;
        }

        void TradeReportSearchTextBoxTextChanged(object sender, EventArgs e)
        {
            ApplyTradeReportSearch();
            if (_tradeReportView.Sort == string.Empty)
            {
                ScrollToBottom();
            }
            _tradeReportSearchTextBox.Focus();
        }

        void ScrollToBottom()
        {
            if (_tradeReportGrid.Rows.Count > 0 /*&& _session.AutoScrollMessages*/)
            {
                _tradeReportGrid.FirstDisplayedScrollingRowIndex = _tradeReportGrid.Rows.Count - 1;
            }
        }

        Fix.TradeReport.ReportSide SelectedReportSide
        {
            get
            {
                if (_tradeReportGrid.SelectedRows.Count == 0)
                    return null;
                DataRowView view = _tradeReportView[_tradeReportGrid.SelectedRows[0].Index];
                var dataRow = view.Row as TradeReportDataRow;
                if (dataRow == null)
                    return null;
                return dataRow.ReportSide;
            }
        }

        void TradeReportViewSelectionChanged(object sender, EventArgs e)
        {
            try
            {
                _fieldTable.BeginLoadData();
                _fieldTable.Rows.Clear();

                if (_tradeReportGrid.SelectedRows.Count > 1)
                    return;

                Fix.TradeReport.ReportSide side = SelectedReportSide;

                if (side == null)
                    return;

                foreach (Fix.Field field in side.Fields)
                {
                    var dataRow = _fieldTable.NewRow() as FieldDataRow;

                    if (dataRow == null)
                        continue;

                    if (field.Definition == null)
                    {
                        field.Definition = Session.FieldDefinition(side.TradeReport.Messages[0], field);
                    }

                    dataRow.Field = field;

                    if (field.Definition != null)
                    {
                        dataRow[FieldDataTable.ColumnIndent] = field.Definition.Indent;
                        dataRow[FieldDataTable.ColumnName] = field.Definition.Name;
                        dataRow[FieldDataTable.ColumnCustom] = false;
                        dataRow[FieldDataTable.ColumnRequired] = field.Definition.Required;
                        dataRow[FieldDataTable.ColumnDescription] = field.ValueDescription;
                    }
                    else
                    {
                        dataRow[FieldDataTable.ColumnCustom] = true;
                        dataRow[FieldDataTable.ColumnRequired] = false;
                    }

                    dataRow[FieldDataTable.ColumnTag] = field.Tag;
                    dataRow[FieldDataTable.ColumnValue] = field.Value;

                    _fieldTable.Rows.Add(dataRow);
                }

                ApplyFieldSearch();
            }
            finally
            {
                _fieldTable.EndLoadData();
                UpdateUiState();
            }
        }

        List<Fix.TradeReport.ReportSide> SelectedReportSides()
        {
            var sides = new List<Fix.TradeReport.ReportSide>();

            foreach (DataGridViewRow gridRow in _tradeReportGrid.SelectedRows)
            {
                DataRowView view = _tradeReportView[gridRow.Index];
                var dataRow = view.Row as TradeReportDataRow;
                if (dataRow == null)
                    continue;
                sides.Add(dataRow.ReportSide);
            }

            return sides;
        }

        void AcceptorRejectButtonClick(object sender, EventArgs e)
        {
            Fix.TradeReport.ReportSide buySide = null;
            Fix.TradeReport.ReportSide sellSide = null;

            foreach (var side in SelectedReportSides())
            {
                if (side.Side == Fix.Side.Buy)
                {
                    buySide = side;
                }
                else
                {
                    sellSide = side;
                }
            }

            if (buySide == null && sellSide == null)
                return;
            _defaultsButton.PerformClick();
            _messageDefaults.RejectTradeReport(buySide, sellSide);
        }

        void AcceptorAckButtonClick(object sender, EventArgs e)
        {
            Fix.TradeReport.ReportSide buySide = null;
            Fix.TradeReport.ReportSide sellSide = null;

            foreach (var side in SelectedReportSides())
            {
                if (side.Side == Fix.Side.Buy)
                {
                    buySide = side;
                }
                else
                {
                    sellSide = side;
                }
            }

            if (buySide == null && sellSide == null)
                return;
            _defaultsButton.PerformClick();
            _messageDefaults.AcknowledgeTradeReport(buySide, sellSide);
        }

        void AcceptorReplyButtonClick(object sender, EventArgs e)
        {
            Fix.TradeReport.ReportSide buySide = null;
            Fix.TradeReport.ReportSide sellSide = null;

            foreach (var side in SelectedReportSides())
            {
                if(side.Side == Fix.Side.Buy)
                {
                    buySide = side;
                }
                else
                {
                    sellSide = side;
                }
            }

            if (buySide == null && sellSide == null)
                return;

            _defaultsButton.PerformClick();
            _messageDefaults.ReplyTradeReport(buySide, sellSide);
        }

        void AcknowledgeAllButtonClick(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(this,
                                                  "This will acknowledge all pending trades, are you sure?",
                                                  Application.ProductName,
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Information);

            if (result != DialogResult.Yes)
                return;

            AcknowledgeAllPendingTrades();
        }

        void RejectAllButtonClick(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(this,
                                                  "This will reject all pending trades, are you sure?",
                                                  Application.ProductName,
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Information);

            if (result != DialogResult.Yes)
                return;

            RejectAllPendingTrades();
        }

        void AcknowledgeAllPendingTrades()
        {
            foreach (DataGridViewRow row in _tradeReportGrid.Rows)
            {
                try
                {
                    var view = row.DataBoundItem as DataRowView;
                    var tradeRow = view.Row as TradeReportDataRow;
                    Fix.TradeReport trade = tradeRow.ReportSide.TradeReport;

                    var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReportAck.MsgType };

                    if (message == null)
                        return;

                    /*
                    message.Fields.Set(Fix.Dictionary.Fields.TradeReportID, trade.TradeReportID);

                    message.Fields.Set(Fix.Dictionary.Fields.ExecType, Fix.ExecType.New);
                    message.Fields.Set(Fix.Dictionary.Fields.TrdRptStatus, Fix.TrdRptStatus.Accepted);

                    message.Fields.Set(Fix.Dictionary.Fields.Symbol, trade.Symbol);
                    message.Fields.Set(Fix.Dictionary.Fields.LastQty, trade.LastQty);
                    message.Fields.Set(Fix.Dictionary.Fields.LastPx, trade.LastPx);
                    message.Fields.Set(Fix.Dictionary.Fields.TrdType, trade.TrdType);
                    //message.Fields.Set(Fix.Dictionary.Fields.TradeReportTransType, Fix.TradeReportTransType.New);

                    message.Fields.Set(Fix.Dictionary.Fields.NoSides, 1);
                    message.Fields.Set(Fix.Dictionary.Fields.Side, trade.Side);
                    message.Fields.Set(Fix.Dictionary.Fields.PartyID, trade.PartyID);
                    */

                    Session.Send(message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                                    ex.Message,
                                    Application.ProductName,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }

        void RejectAllPendingTrades()
        {
            foreach (DataGridViewRow row in _tradeReportGrid.Rows)
            {
                try
                {
                    var view = row.DataBoundItem as DataRowView;
                    var tradeRow = view.Row as TradeReportDataRow;
                    Fix.TradeReport trade = tradeRow.ReportSide.TradeReport;

                    var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReportAck.MsgType };

                    if (message == null)
                        return;

                    /*
                    message.Fields.Set(Fix.Dictionary.Fields.TradeReportID, trade.TradeReportID);

                    message.Fields.Set(Fix.Dictionary.Fields.ExecType, Fix.ExecType.New);
                    message.Fields.Set(Fix.Dictionary.Fields.TrdRptStatus, Fix.TrdRptStatus.Rejected);

                    message.Fields.Set(Fix.Dictionary.Fields.Symbol, trade.Symbol);
                    message.Fields.Set(Fix.Dictionary.Fields.LastQty, trade.LastQty);
                    message.Fields.Set(Fix.Dictionary.Fields.LastPx, trade.LastPx);
                    message.Fields.Set(Fix.Dictionary.Fields.TrdType, trade.TrdType);
                    //message.Fields.Set(Fix.Dictionary.Fields.TradeReportTransType, Fix.TradeReportTransType.New);

                    message.Fields.Set(Fix.Dictionary.Fields.NoSides, 1);
                    message.Fields.Set(Fix.Dictionary.Fields.Side, trade.Side);
                    message.Fields.Set(Fix.Dictionary.Fields.PartyID, trade.PartyID);
                    */

                    Session.Send(message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                                    ex.Message,
                                    Application.ProductName,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }

        public Session Session
        {
            get
            {
                return _session;
            }
            set
            {
                if (_session != null)
                {
                    _session.TradeBook.Messages.Reset -= MessagesReset;
                    _session.TradeBook.TradeReportInserted -= TradeBookTradeInserted;
                    _session.TradeBook.TradeReportUpdated -= TradeBookTradeUpdated;
                    _session.SessionReset -= SessionSessionReset;
                    _session.StateChanged -= SessionStateChanged;
                }

                _session = value;
                Reload();

                if (_session != null)
                {
                    _session.TradeBook.Messages.Reset += MessagesReset;
                    _session.TradeBook.TradeReportInserted += TradeBookTradeInserted;
                    _session.TradeBook.TradeReportUpdated += TradeBookTradeUpdated;
                    _session.SessionReset += SessionSessionReset;
                    _session.StateChanged += SessionStateChanged;
                }

                if (value.OrderBehaviour == Fix.Behaviour.Initiator)
                {
                    //SetToolStrip(_clientToolStrip);
                    //SetMenuStrip(_clientMenuStrip);
                }
                else
                {
                    TopToolStripPanel.Join(_acceptorToolStrip);
                    SetMenuStrip(_acceptorMenuStrip);
                }
            }
        }

        void SessionSessionReset(object sender, EventArgs ev)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SessionSessionReset(sender, ev)));
                return;
            }

            Reload();
        }

        void MessagesReset(object sender, Fix.MessageCollection.MessageEvent ev)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => MessagesReset(sender, ev)));
                return;
            }

            Reload();
        }

        void SessionStateChanged(object sender, Fix.Session.StateEvent ev)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SessionStateChanged(sender, ev)));
                return;
            }

            UpdateUiState();
        }

        void TradeBookTradeUpdated(object sender, Fix.TradeReportBookEventArgs ev)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => TradeBookTradeUpdated(sender, ev)));
                return;
            }

            /*
            Fix.TradeReport trade = ev.Trade;
            
            TradeReportDataRow row = _tradeReportTable.Rows.Find(trade.TradeReportID) as TradeReportDataRow;

            if (row == null)
                return;

            row.Report= trade;
            UpdateRow(row);
            _tradeReportView.RefreshEdit();
            */
        }

        void TradeBookTradeInserted(object sender, Fix.TradeReportBookEventArgs ev)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => TradeBookTradeInserted(sender, ev)));
                return;
            }

            AddTradeReport(ev.Trade);
        }

        void AddTradeReport(Fix.TradeReport trade)
        {
            if (trade.BuySide != null)
            {
                AddTradeReportSide(trade.BuySide);
            }

            if(trade.SellSide != null)
            {
                AddTradeReportSide(trade.SellSide);
            }
        }

        void AddTradeReportSide(Fix.TradeReport.ReportSide side)
        { 
            string key = side.Key;

            var row = _tradeReportTable.Rows.Find(key) as TradeReportDataRow;

            if (row != null)
                return;

            row = (TradeReportDataRow)_tradeReportTable.NewRow();
            row.ReportSide = side;
            row[TradeReportDataTable.ColumnKey] = key;

            UpdateRow(row);
            _tradeReportTable.Rows.Add(row);
        }

        void UpdateRow(TradeReportDataRow row)
        {
            Fix.TradeReport trade = row.ReportSide.TradeReport;
            Fix.TradeReport.ReportSide side = row.ReportSide;

            row[TradeReportDataTable.ColumnSide] = side.Side;
            row[TradeReportDataTable.ColumnSideString] = side.Side.ToString();
            row[TradeReportDataTable.ColumnSymbol] = trade.Symbol;
            row[TradeReportDataTable.ColumnLastQty] = trade.LastQty;
            row[TradeReportDataTable.ColumnLastPx] = trade.LastPx;
            row[TradeReportDataTable.ColumnTrdType] = trade.TrdType;
            row[TradeReportDataTable.ColumnTradeReportId] = trade.TradeReportID;
            row[TradeReportDataTable.ColumnPartyId] = side.PartyID;
            row[TradeReportDataTable.ColumnText] = side.Text;
        }

        void Reload()
        {
            try
            {
                _tradeReportTable.BeginLoadData();
                _tradeReportTable.Clear();

                foreach (Fix.TradeReport trade in Session.TradeBook.Trades)
                {
                    AddTradeReport(trade);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                                ex.Message,
                                Application.ProductName,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            finally
            {
                _tradeReportTable.EndLoadData();
                UpdateUiState();
            }
        }

        void UpdateUiState()
        {
            _tradeReportSearchTextBox.Enabled = _tradeReportTable.Rows.Count > 0;
            _fieldSearchTextBox.Enabled = SelectedReportSide != null;

            bool enabled = true;
            string tradeReportID = null;

            foreach(DataGridViewRow gridRow in _tradeReportGrid.SelectedRows)
            {
                DataRowView view = _tradeReportView[gridRow.Index];
                var dataRow = view.Row as TradeReportDataRow;

                if (dataRow == null)
                    continue;

                if (string.IsNullOrEmpty(tradeReportID))
                {
                    tradeReportID = dataRow.ReportSide.TradeReport.TradeReportID;
                    continue;
                }
                
                if (dataRow.ReportSide.TradeReport.TradeReportID != tradeReportID)
                {
                    enabled = false;
                    break;
                }
            }

            _acceptorAckButton.Enabled = enabled;
            _acceptorRejectButton.Enabled = enabled;
            _acceptorReplyButton.Enabled = enabled;

            _acceptorAckMenuItem.Enabled = enabled;
            _acceptorRejectMenuItem.Enabled = enabled;
            _acceptorReplyMenuItem.Enabled = enabled;
        }
    }
}
