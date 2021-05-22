/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: ParserPanel.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static Fix.Dictionary;

namespace FixClient
{
    public partial class ParserPanel : FixClientPanel
    {
        readonly ParserMessageDataGridView _messageGridView;

        readonly ParserMessageDataTable _messageTable;
        readonly DataView _messageView;

        readonly SearchTextBox _messageSearchTextBox;
        readonly SearchTextBox _fieldSearchTextBox;

        readonly FieldDataTable _fieldTable;
        readonly DataView _fieldView;
        readonly TextBox _statusMessage;

        readonly ToolStripCheckBox _showAdminMessageCheckBox;

        readonly ToolStripDropDownButton _statusButton;
        readonly ToolStripMenuItem _statusNoneMenuItem;
        readonly ToolStripMenuItem _statusInfoMenuItem;
        readonly ToolStripMenuItem _statusWarnMenuItem;
        readonly ToolStripMenuItem _statusErrorMenuItem;


        readonly OrderDataTable _orderTable;
        readonly DataView _orderView;
        readonly Fix.OrderBook _orderBook;
        readonly OrderDataGridView orderGrid;
        readonly SearchTextBox _orderSearchTextBox;

        readonly Image messageStatusErrorImage;
        readonly Image messageStatusWarnImage;
        readonly Image messageStatusInfoImage;
        readonly Image messageStatusNoneImage;

        public ParserPanel()
        {
            InitializeComponent();

            _messageTable = new ParserMessageDataTable("Messages");
            _messageView = new DataView(_messageTable);

            _fieldTable = new FieldDataTable("Fields");
            _fieldView = new DataView(_fieldTable);

            _messageSearchTextBox = new SearchTextBox
            {
                Dock = DockStyle.Top
            };
            _messageSearchTextBox.TextChanged += MessageSearchTextBoxTextChanged;
            _fieldSearchTextBox = new SearchTextBox
            {
                Dock = DockStyle.Top
            };
            _fieldSearchTextBox.TextChanged += FieldSearchTextBoxTextChanged;

            _statusMessage = new TextBox
            {
                Dock = DockStyle.Bottom,
                ReadOnly = true
            };

            messageStatusErrorImage = Properties.Resources.MessageStatusError;
            messageStatusInfoImage = Properties.Resources.MessageStatusInfo;
            messageStatusWarnImage = Properties.Resources.MessageStatusWarn;
            messageStatusNoneImage = Properties.Resources.MessageStatusNone;

            #region ToolStrip
            var loadButton = new ToolStripButton(Properties.Resources.Open)
            {
                ImageTransparentColor = Color.Magenta,
                ToolTipText = "Load a Glue/Gate/FIX Router/Desk Server log file"
            };
            loadButton.Click += LoadClientMessagesButtonClick;

            _showAdminMessageCheckBox = new ToolStripCheckBox();
            _showAdminMessageCheckBox.CheckChanged += ShowAdminMessageCheckBoxCheckChanged;

            _statusNoneMenuItem = new ToolStripMenuItem { Text = "None", Checked = true, CheckOnClick = true };
            _statusNoneMenuItem.Click += StatusMenuItemClick;

            _statusInfoMenuItem = new ToolStripMenuItem { Text = "Info", Checked = true, CheckOnClick = true };
            _statusInfoMenuItem.Click += StatusMenuItemClick;

            _statusWarnMenuItem = new ToolStripMenuItem { Text = "Warn", Checked = true, CheckOnClick = true };
            _statusWarnMenuItem.Click += StatusMenuItemClick;

            _statusErrorMenuItem = new ToolStripMenuItem { Text = "Error", Checked = true, CheckOnClick = true };
            _statusErrorMenuItem.Click += StatusMenuItemClick;

            _statusButton = new ToolStripDropDownButton("Message Status");

            _statusButton.DropDownItems.Add(_statusNoneMenuItem);
            _statusButton.DropDownItems.Add(_statusInfoMenuItem);
            _statusButton.DropDownItems.Add(_statusWarnMenuItem);
            _statusButton.DropDownItems.Add(_statusErrorMenuItem);

            var toolStrip = new ToolStrip(new ToolStripItem[]
            {
                loadButton,
                new ToolStripLabel("Show Administrative Messages"),
                _showAdminMessageCheckBox,
                _statusButton
            })
            {
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = LookAndFeel.Color.ToolStrip,
                Renderer = new ToolStripRenderer()
            };

            #endregion

            var splitter = new SplitContainer
            {
                Orientation = Orientation.Horizontal,
                Dock = DockStyle.Fill,
                SplitterDistance = 200
            };

            var messageSplitter = new SplitContainer
            {
                Orientation = Orientation.Vertical,
                Dock = DockStyle.Fill
            };

            var container = new ToolStripContainer { Dock = DockStyle.Fill };
            container.ContentPanel.Controls.Add(messageSplitter);
            container.TopToolStripPanel.Join(toolStrip);
            container.TopToolStripPanel.BackColor = LookAndFeel.Color.ToolStrip;

            _messageGridView = new ParserMessageDataGridView
            {
                Dock = DockStyle.Fill,
                VirtualMode = true
            };
            _messageGridView.SelectionChanged += ClientMessageGridGridSelectionChanged;
            _messageGridView.CellValueNeeded += MessageGridViewCellValueNeeded;
            _messageGridView.CellFormatting += MessageGridViewCellFormatting;

            foreach (DataColumn source in _messageTable.Columns)
            {
                if (source.ColumnMapping == MappingType.Hidden)
                    continue;

                DataGridViewColumn column;

                if (source.DataType == typeof(Image))
                {
                    column = new DataGridViewImageColumn
                    {
                        Name = source.ColumnName,
                        ValueType = source.DataType
                    };
                }
                else
                {
                    column = new DataGridViewTextBoxColumn
                    {
                        Name = source.ColumnName,
                        ValueType = source.DataType
                    };
                }
                _messageGridView.Columns.Add(column);
            }

            var fieldGridView = new FieldDataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = _fieldView
            };

            messageSplitter.Panel1.Controls.Add(_messageGridView);
            messageSplitter.Panel1.Controls.Add(_messageSearchTextBox);
            messageSplitter.Panel2.Controls.Add(fieldGridView);
            messageSplitter.Panel2.Controls.Add(_fieldSearchTextBox);
            messageSplitter.Panel2.Controls.Add(_statusMessage);

            _orderBook = new Fix.OrderBook();
            _orderBook.Messages.MessageAdded += MessagesMessageAdded;
            _orderBook.OrderInserted += OrderBookOrderInserted;
            _orderBook.OrderUpdated += OrderBookOrderUpdated;

            _orderTable = new OrderDataTable("Orders");
            _orderView = new DataView(_orderTable);
            orderGrid = new OrderDataGridView
            {
                DataSource = _orderView,
                Dock = DockStyle.Fill,
                DisplayExDestination = true
            };

            _orderSearchTextBox = new SearchTextBox
            {
                Dock = DockStyle.Top
            };
            _orderSearchTextBox.TextChanged += OrderSearchTextBoxTextChanged;

            splitter.Panel1.Controls.Add(container);
            splitter.Panel2.Controls.Add(orderGrid);
            splitter.Panel2.Controls.Add(_orderSearchTextBox);

            ContentPanel.Controls.Add(splitter);

            ShowAdminMessageCheckBoxCheckChanged(this, EventArgs.Empty);
        }

        void MessageGridViewCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            DataRowView view = _messageView[e.RowIndex];
        
            if (view.Row is MessageDataRow dataRow)
            {
                e.CellStyle.ForeColor = (dataRow.Message?.Incoming ?? false)
                    ? LookAndFeel.Color.Incoming
                    : LookAndFeel.Color.Outgoing;
                e.FormattingApplied = true;
            }
        }

        void MessageGridViewCellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = _messageView[e.RowIndex][e.ColumnIndex];
        }

        void StatusMenuItemClick(object? sender, EventArgs e)
        {
            UpdateMessageFilter();
        }

        void OrderSearchTextBoxTextChanged(object? sender, EventArgs e)
        {
            if (sender is not TextBox textBox)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox.Text))
            {
                _orderView.RowFilter = null;
                _orderView.Sort = string.Empty;
            }
            else
            {
                var buffer = new StringBuilder();

                if (_orderView.Table is DataTable table)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        if (column.DataType.IsEnum)
                        {
                            buffer.AppendFormat("{0} LIKE '%{1}%' OR ", column.ColumnName + "String", textBox.Text);
                        }
                        else
                        {
                            buffer.AppendFormat("CONVERT({0}, System.String) LIKE '%{1}%' OR ", column.ColumnName, textBox.Text);
                        }
                    }
                }

                buffer.Remove(buffer.Length - 3, 3);
                _orderView.RowFilter = buffer.ToString();
            }
            textBox.Focus();
        }

        void FieldSearchTextBoxTextChanged(object? sender, EventArgs e)
        {
            if (_fieldView == null)
            {
                return;
            }
            
            if (sender is not TextBox textBox)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox.Text))
            {
                _fieldView.RowFilter = null;
                _fieldView.Sort = string.Empty;
            }
            else
            {
                _fieldView.RowFilter = string.Format("CONVERT({0}, System.String) LIKE '%{3}%' OR {1} LIKE '%{3}%' OR {2} LIKE '%{3}%'",
                                                        FieldDataTable.ColumnTag,
                                                        FieldDataTable.ColumnName,
                                                        FieldDataTable.ColumnValue,
                                                        textBox.Text);
            }
            textBox.Focus();
        }

        void MessageSearchTextBoxTextChanged(object? sender, EventArgs e)
        {
            UpdateMessageFilter();
        }

        void UpdateMessageFilter()
        {
            string filter = string.Empty;

            if (!_showAdminMessageCheckBox.Checked)
            {
                filter = "NOT Administrative";
            }

            if (!_statusNoneMenuItem.Checked)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("{0} <> {1}", ParserMessageDataTable.ColumnStatus, (int)Fix.MessageStatus.None);
            }

            if (!_statusInfoMenuItem.Checked)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("{0} <> {1}", ParserMessageDataTable.ColumnStatus, (int)Fix.MessageStatus.Info);
            }

            if (!_statusWarnMenuItem.Checked)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("{0} <> {1}", ParserMessageDataTable.ColumnStatus, (int)Fix.MessageStatus.Warn);
            }

            if (!_statusErrorMenuItem.Checked)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("{0} <> {1}", ParserMessageDataTable.ColumnStatus, (int)Fix.MessageStatus.Error);
            }

            if (string.IsNullOrEmpty(_messageSearchTextBox.Text))
            {
                _messageView.Sort = string.Empty;
            }
            else
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("({0} LIKE '%{3}%' OR {1} LIKE '%{3}%' OR {2} LIKE '%{3}%')",
                                        ParserMessageDataTable.ColumnSendingTime,
                                        ParserMessageDataTable.ColumnMsgSeqNum,
                                        ParserMessageDataTable.ColumnMsgTypeDescription,
                                        _messageSearchTextBox.Text);
            }

            _messageGridView.RowCount = 0;
            _messageView.RowFilter = filter;
            _messageGridView.RowCount = _messageView.Count;
            _messageSearchTextBox.Focus();
        }

        void OrderBookOrderUpdated(object? sender, Fix.OrderBookEventArgs e)
        {
            Fix.Order order = e.Order;

            if (_orderTable.Rows.Find(order.ClOrdID) is not OrderDataRow row)
            {
                return;
            }

            row.Order = order;
            UpdateRow(row);
        }

        void OrderBookOrderInserted(object? sender, Fix.OrderBookEventArgs e)
        {
            Fix.Order order = e.Order;
            if (_orderTable.Rows.Find(order.ClOrdID) is OrderDataRow)
            {
                return;
            }

            OrderDataRow row = (OrderDataRow)_orderTable.NewRow();
            row.Order = order;
            row[OrderDataTable.ColumnClOrdId] = order.ClOrdID;
            //
            // Prime these with 0's so the grid looks pretty.
            //
            row[OrderDataTable.ColumnDone] = 0;
            row[OrderDataTable.ColumnLeaves] = 0;
            row[OrderDataTable.ColumnAvgPrice] = 0;
            UpdateRow(row);
            _orderTable.Rows.Add(row);
        }

        static void UpdateRow(OrderDataRow row)
        {
            if (row.Order is not Fix.Order order)
            {
                return;
            }

            row[OrderDataTable.ColumnQuantity] = order.OrderQty;
            row[OrderDataTable.ColumnSymbol] = order.Symbol;
            row[OrderDataTable.ColumnExDestination] = order.ExDestination;

            if (order.TimeInForce != null)
            {
                row[OrderDataTable.ColumnTimeInForce] = order.TimeInForce;
                row[OrderDataTable.ColumnTimeInForceString] = OrderDataGridView.ShortTimeInForceDescription(order.TimeInForce);
            }

            if (order.OrdStatus != null)
            {
                row[OrderDataTable.ColumnOrdStatus] = order.OrdStatus;
                row[OrderDataTable.ColumnOrdStatusString] = order.OrdStatus.Name;
            }
            else
            {
                row[OrderDataTable.ColumnOrdStatusString] = FIX_5_0SP2.OrdStatus.PendingNew.Name;
            }

            if (order.OrigClOrdID != null)
                row[OrderDataTable.ColumnOrigClOrdId] = order.OrigClOrdID;

            if (order.Side != null)
            {
                row[OrderDataTable.ColumnSide] = order.Side;
                row[OrderDataTable.ColumnSideString] = order.Side.Name;
            }

            long cumQty = order.CumQty ?? 0;
            long leavesQty = order.LeavesQty ?? 0;

            if (!order.LeavesQty.HasValue)
            {
                if (order.Active)
                {
                    leavesQty = order.OrderQty - cumQty;
                }
            }

            row[OrderDataTable.ColumnLeaves] = leavesQty;
            row[OrderDataTable.ColumnDone] = cumQty;

            if (order.AvgPx != null)
                row[OrderDataTable.ColumnAvgPrice] = order.AvgPx.Value;

            if (order.Price != null)
                row[OrderDataTable.ColumnLimit] = order.Price.Value;

            if (order.ListID != null)
                row[OrderDataTable.ColumnListId] = order.ListID;

            if (order.Text != null)
                row[OrderDataTable.ColumnText] = order.Text;
        }

        void ShowAdminMessageCheckBoxCheckChanged(object? sender, EventArgs e)
        {
            UpdateMessageFilter();
        }

        void LoadClientMessagesButtonClick(object? sender, EventArgs e)
        {
            using OpenFileDialog dlg = new();

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Cursor? original = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                Fix.MessageCollection messages = Fix.MessageCollection.Parse(dlg.FileName);

                if (messages == null)
                {
                    MessageBox.Show(this,
                                    "The file could not be parsed",
                                    Application.ProductName,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                Messages = messages;
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
                Cursor.Current = original;
            }
        }

        Fix.Message? SelectedMessage
        {
            get
            {
                if (_messageGridView.SelectedRows.Count == 0)
                {
                    return null;
                }

                DataRowView view = _messageView[_messageGridView.SelectedRows[0].Index];
         
                if (view.Row is not MessageDataRow dataRow)
                {
                    return null;
                }

                return dataRow.Message;
            }
        }

        public static Fix.Dictionary.Message? MessageDefinition(Fix.Message message)
        {
            Fix.Field? beginString = message.Fields.Find(FIX_5_0SP2.Fields.BeginString);
            Fix.Dictionary.Version? version = null;

            if (beginString is not null && !beginString.Value.StartsWith("FIXT."))
            {
                version = Versions[beginString.Value];
            }

            if (version is null)
            {
                version = Versions.Default;
            }

            Fix.Dictionary.Message? exemplar = version.Messages[message.MsgType];

            if (exemplar == null)
            {
                return FIX_5_0SP2.Messages[message.MsgType];
            }

            return exemplar;
        }

        public static MessageField? FieldDefinition(Fix.Dictionary.Message message, Fix.Field field)
        {
            message.Fields.TryGetValue(field.Tag, out var definition);
            return definition;
        }

        void ClientMessageGridGridSelectionChanged(object? sender, EventArgs e)
        {
            try
            {
                _fieldTable.BeginLoadData();
                _fieldTable.Clear();

                if (SelectedMessage is not Fix.Message message)
                {
                    return;
                }

                switch (message.Status)
                {
                    case Fix.MessageStatus.Error:
                        _statusMessage.ForeColor = Color.White;
                        _statusMessage.BackColor = Color.Red;
                        break;
                    case Fix.MessageStatus.Warn:
                        _statusMessage.ForeColor = Color.Black;
                        _statusMessage.BackColor = Color.Yellow;
                        break;
                    case Fix.MessageStatus.Info:
                        _statusMessage.ForeColor = Color.White;
                        _statusMessage.BackColor = Color.DodgerBlue;
                        break;

                    case Fix.MessageStatus.None:
                        _statusMessage.ForeColor = SystemColors.ControlText;
                        _statusMessage.BackColor = SystemColors.Control;
                        break;
                }

                _statusMessage.Text = message.StatusMessage;

                var messageDefinition = FIX_5_0SP2.Messages[message.MsgType];

                foreach (Fix.Field field in message.Fields)
                {
                    if (_fieldTable.NewRow() is not FieldDataRow dataRow)
                    {
                        continue;
                    }

                    var description = field.Describe(messageDefinition);

                    dataRow.Field = field;

                    if (description != null)
                    {
                        dataRow[FieldDataTable.ColumnIndent] = description.Depth;
                        dataRow[FieldDataTable.ColumnName] = description.Name;
                        dataRow[FieldDataTable.ColumnCustom] = false;
                        dataRow[FieldDataTable.ColumnRequired] = description.Required;
                        dataRow[FieldDataTable.ColumnDescription] = description.Description;
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
            }
            finally
            {
                _fieldTable.EndLoadData();
            }
        }

        Image ImageForMessageStatus(Fix.MessageStatus status)
        {
            return status switch
            {
                Fix.MessageStatus.Error => messageStatusErrorImage,
                Fix.MessageStatus.Info => messageStatusInfoImage,
                Fix.MessageStatus.Warn => messageStatusWarnImage,
                _ => messageStatusNoneImage,
            };
        }

        void MessagesMessageAdded(object? sender, Fix.MessageCollection.MessageEvent ev)
        {
            Fix.Message message = ev.Message;

            if (message.Fields.Find(FIX_5_0SP2.Fields.MsgType) is not Fix.Field)
            {
                return;
            }

            if (message.Definition == null)
            {
                message.Definition = MessageDefinition(message);
            }

            var row = (MessageDataRow)_messageTable.NewRow();
            row.Message = message;

            row[ParserMessageDataTable.ColumnSendingTime] = message.SendingTime;
            row[ParserMessageDataTable.ColumnMsgType] = message.MsgType;
            row[MessageDataTable.ColumnStatus] = message.Status;
            row[MessageDataTable.ColumnStatusImage] = ImageForMessageStatus(message.Status);
            row[MessageDataTable.ColumnStatusMessage] = message.StatusMessage;
            row[ParserMessageDataTable.ColumnMsgTypeDescription] = message.Definition == null ? string.Empty : message.Definition.Name;
            row[ParserMessageDataTable.ColumnAdministrative] = message.Administrative;

            var field = message.Fields.Find(FIX_5_0SP2.Fields.MsgSeqNum);

            row[ParserMessageDataTable.ColumnMsgSeqNum] = field?.Value;

            _messageTable.Rows.Add(row);
        }

        public Fix.MessageCollection Messages
        {
            set
            {
                try
                {
                    _messageTable.BeginLoadData();
                    _orderTable.BeginLoadData();

                    _messageTable.Clear();
                    _orderTable.Clear();

                    _orderBook.Clear();

                    foreach (Fix.Message message in value)
                    {
                        _orderBook.Process(message);
                    }

                    _messageGridView.RowCount = _messageView.Count;
                }
                finally
                {
                    _orderTable.EndLoadData();
                    _messageTable.EndLoadData();
                }
            }
        }
    }
}