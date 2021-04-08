/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: HistoryPanel.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FixClient
{
    partial class HistoryPanel : FixClientPanel
    {
        readonly DataTable _messageTable;
        readonly DataView _messageView;

        readonly FieldDataTable _fieldTable;
        readonly DataView _fieldView;

        readonly HistoryMessageDataGridView _messageGrid;
        readonly FieldDataGridView _fieldGrid;

        readonly SearchTextBox _messageSearchTextBox;
        readonly SearchTextBox _fieldSearchTextBox;

        readonly TextBox _rawMessage;
        readonly TextBox _statusMessage;

        readonly ToolStripButton _clearButton;
        readonly ToolStripButton _resendButton;
        readonly ToolStripButton _exportButton;
        readonly ToolStripCheckBox _tailMessagesCheckBox;

        readonly ToolStripMenuItem _clearMenuItem;
        readonly ToolStripMenuItem _resendMenuItem;
        readonly ToolStripMenuItem _exportMenuItem;

        Session _session;
        //
        // This event is used by the mainform to updated the context of the dictionary view
        //
        public delegate void MessageDelegate(Fix.Message message);
        public event MessageDelegate MessageSelected;

        public HistoryPanel()
        {
            InitializeComponent();

            #region ToolStrip
            var toolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = LookAndFeel.Color.ToolStrip,
                Renderer = new ToolStripRenderer()
            };


            _clearButton = new ToolStripButton
            {
                Image = Properties.Resources.Clear,
                ImageTransparentColor = Color.Magenta,
                ToolTipText = "Clear all session messages"
            };
            _clearButton.Click += ClearButtonClick;

            _resendButton = new ToolStripButton
            {
                Image = Properties.Resources.Send,
                ImageTransparentColor = Color.White,
                ToolTipText = "Resend the selected message"
            };
            _resendButton.Click += ResendButtonClick;

            _exportButton = new ToolStripButton
            {
                Image = Properties.Resources.Export,
                ImageTransparentColor = Color.Magenta,
                ToolTipText = "Export the message history to a text file"
            };
            _exportButton.Click += ExportButtonClick;

            _tailMessagesCheckBox = new ToolStripCheckBox();
            _tailMessagesCheckBox.CheckChanged += TailMessagesCheckBoxCheckChanged;

            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                   _clearButton,
                   _resendButton,
                   _exportButton,
                   new ToolStripLabel("Tail Messages"),
                   _tailMessagesCheckBox
            });

            TopToolStripPanel.Join(toolStrip);

            #endregion

            #region MenuStrip
            var menu = new ToolStripMenuItem("Action");
            SetMenuStrip(menu);

            _clearMenuItem = new ToolStripMenuItem("Clear", _clearButton.Image);
            _clearMenuItem.Click += ClearButtonClick;
            menu.DropDownItems.Add(_clearMenuItem);

            _resendMenuItem = new ToolStripMenuItem("Resend", _resendButton.Image);
            _resendMenuItem.Click += ResendButtonClick;
            menu.DropDownItems.Add(_resendMenuItem);

            _exportMenuItem = new ToolStripMenuItem("Export", _exportButton.Image);
            _exportMenuItem.Click += ExportButtonClick;
            menu.DropDownItems.Add(_exportMenuItem);
            #endregion

            _messageSearchTextBox = new SearchTextBox
            {
                Dock = DockStyle.Top,
            };
            _messageSearchTextBox.TextChanged += MessageSearchTextBoxTextChanged;
            _fieldSearchTextBox = new SearchTextBox
            {
                Dock = DockStyle.Top,
            };
            _fieldSearchTextBox.TextChanged += FieldSearchTextBoxTextChanged;

            var container = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };

            _messageGrid = new HistoryMessageDataGridView
            {
                Dock = DockStyle.Fill,
                VirtualMode = true
            };
            _messageGrid.SelectionChanged += MessageGridSelectionChanged;
            _messageGrid.CellValueNeeded += MessageGridCellValueNeeded;
            _messageGrid.CellFormatting += MessageGridCellFormatting;

            _messageTable = new MessageDataTable("Messages");
            _messageView = new DataView(_messageTable);
            _messageView.ListChanged += ViewListChanged;

            _fieldTable = new FieldDataTable("Fields");
            _fieldView = new DataView(_fieldTable);

            _fieldGrid = new FieldDataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = _fieldView
            };

            var rightSplitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = ClientRectangle.Height
            };

            _rawMessage = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = LookAndFeel.Color.GridCellBackground,
                ForeColor = LookAndFeel.Color.GridCellForeground,
                BorderStyle = BorderStyle.None
            };

            _statusMessage = new TextBox
            {
                Dock = DockStyle.Bottom,
                ReadOnly = true
            };

            rightSplitter.Panel1.Controls.Add(_fieldGrid);
            rightSplitter.Panel1.Controls.Add(_fieldSearchTextBox);
            rightSplitter.Panel2.Controls.Add(_rawMessage);
            rightSplitter.Panel2.Controls.Add(_statusMessage);

            container.Panel1.Controls.Add(_messageGrid);
            container.Panel1.Controls.Add(_messageSearchTextBox);
            container.Panel2.Controls.Add(rightSplitter);

            ContentPanel.Controls.Add(container);

            IntPtr h = Handle;

            UpdateUiState();
        }

        void MessageGridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataRowView view = _messageView[e.RowIndex];

            if (view == null)
                return;

            var dataRow = view.Row as MessageDataRow;

            if (dataRow == null)
                return;

            e.CellStyle.ForeColor = dataRow.Message.Incoming ? LookAndFeel.Color.Incoming : LookAndFeel.Color.Outgoing;
            e.FormattingApplied = true;
        }

        void MessageGridCellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = _messageView[e.RowIndex][e.ColumnIndex];
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                ScrollToBottom();
            }
        }

        void TailMessagesCheckBoxCheckChanged(object sender, EventArgs e)
        {
            _session.AutoScrollMessages = _tailMessagesCheckBox.Checked;
            if (!string.IsNullOrEmpty(_session.FileName))
                _session.Write();
            ScrollToBottom();
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
            if (_fieldView == null || SelectedMessage == null)
                return;
            ApplyFieldSearch();
            _fieldSearchTextBox.Focus();
        }

        void ApplyMessageSearch()
        {
            string search = null;
            if (string.IsNullOrEmpty(_messageSearchTextBox.Text))
            {
                _messageView.Sort = string.Empty;
            }
            else
            {
                search = string.Format("{1} LIKE '%{0}%' OR {2} LIKE '%{0}%' OR {3} LIKE '%{0}%'",
                                       _messageSearchTextBox.Text,
                                       MessageDataTable.ColumnSendingTime,
                                       MessageDataTable.ColumnMsgTypeDescription,
                                       MessageDataTable.ColumnMsgSeqNum);
            }
            _messageGrid.RowCount = 0;
            _messageView.RowFilter = _session.MessageRowFilter(search);
            _messageGrid.RowCount = _messageView.Count;
        }

        void MessageSearchTextBoxTextChanged(object sender, EventArgs e)
        {
            ApplyMessageSearch();
            if (_messageView.Sort == string.Empty)
            {
                ScrollToBottom();
            }
            _messageSearchTextBox.Focus();
        }

        Fix.Message SelectedMessage
        {
            get
            {
                if (_messageGrid.SelectedRows.Count == 0)
                    return null;
                DataRowView view = _messageView[_messageGrid.SelectedRows[0].Index];
                var dataRow = view.Row as MessageDataRow;
                if (dataRow == null)
                    return null;
                return dataRow.Message;
            }
        }

        void ExportButtonClick(object sender, EventArgs e)
        {
            string filename = Session.SenderCompId + "-" + Session.TargetCompId + ".txt";

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                dlg.FilterIndex = 2;
                dlg.RestoreDirectory = true;
                dlg.FileName = filename;
                dlg.Title = "Export";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Cursor original = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;

                    try
                    {
                        using (FileStream stream = new FileStream(dlg.FileName, FileMode.Create))
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                foreach (Fix.Message message in Session.Messages)
                                {
                                    string timestamp = "Unknown";
                                    Fix.Field field = message.Fields.Find(Fix.Dictionary.Fields.SendingTime.Tag);
                                    if (field != null)
                                    {
                                        timestamp = field.Value;
                                    }
                                    string direction = message.Incoming ? "received" : "sent";

                                    Fix.Dictionary.Message definition = Session.Version.Messages[message.MsgType];

                                    writer.WriteLine("{0} ({1}) ({2})",
                                                     timestamp,
                                                     direction,
                                                     definition.Name);

                                    writer.WriteLine("{");
                                    writer.WriteLine(message.ToString());
                                    writer.WriteLine("}");
                                }
                            }
                        }
                    }
                    finally
                    {
                        Cursor.Current = original;
                    }
                }
            }
        }

        void UpdateUiState()
        {
            _resendButton.Enabled = false;
            _resendMenuItem.Enabled = false;
            _clearButton.Enabled = false;
            _clearMenuItem.Enabled = false;
            _exportButton.Enabled = false;
            _exportMenuItem.Enabled = false;
            _tailMessagesCheckBox.Enabled = false;
            _messageSearchTextBox.Enabled = false;
            _fieldSearchTextBox.Enabled = false;

            if (Session == null || Session.Messages == null || Session.Messages.Count < 1)
                return;

            _clearButton.Enabled = true;
            _clearMenuItem.Enabled = true;
            _exportButton.Enabled = true;
            _exportMenuItem.Enabled = true;
            _tailMessagesCheckBox.Enabled = true;
            _messageSearchTextBox.Enabled = true;

            Fix.Message message = SelectedMessage;

            if (message != null)
            {
                _fieldSearchTextBox.Enabled = true;

                if (Session.Connected && !message.Incoming)
                {
                    _resendButton.Enabled = true;
                    _resendMenuItem.Enabled = true;
                }
            }
        }

        void ResendButtonClick(object sender, EventArgs e)
        {
            Fix.Message source = SelectedMessage;

            if (source == null)
                return;
            //
            // Create a new copy of the message.
            //
            var message = new Fix.Message();

            message.Fields.Clear();

            foreach (Fix.Field field in source.Fields)
            {
                if (field.Tag == Fix.Dictionary.Fields.ClOrdID.Tag)
                {
                    message.Fields.Add(new Fix.Field(field.Tag, Session.FormatClOrdId(Session.NextClOrdId++)));
                }
                else if (field.Tag == Fix.Dictionary.Fields.OrderID.Tag)
                {
                    message.Fields.Add(new Fix.Field(field.Tag, Session.NextOrderId++));
                }
                else if (field.Tag == Fix.Dictionary.Fields.ExecID.Tag)
                {
                    message.Fields.Add(new Fix.Field(field.Tag, Session.NextExecId++));
                }
                else
                {
                    message.Fields.Add(new Fix.Field(field.Tag, field.Value));
                }
            }

            Session.Send(message);
        }

        void ClearButtonClick(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(this,
                                                  "This will clear the order history as well, are you sure",
                                                  Application.ProductName,
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            if (Session != null)
            {
                Session.ResetMessages();
            }
        }

        void MessageGridSelectionChanged(object sender, EventArgs e)
        {
            try
            {
                _fieldTable.BeginLoadData();
                _fieldTable.Rows.Clear();

                Fix.Message message = SelectedMessage;

                if (message == null)
                    return;

                foreach (Fix.Field field in message.Fields)
                {
                    var dataRow = _fieldTable.NewRow() as FieldDataRow;

                    if (dataRow == null)
                        continue;

                    if (field.Definition == null)
                    {
                        field.Definition = Session.FieldDefinition(message, field);
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

                string text;
                using (MemoryStream stream = new MemoryStream())
                using (Fix.Writer writer = new Fix.Writer(stream, true))
                {
                    writer.Write(message);
                    writer.Close();
                    text = Encoding.UTF8.GetString(stream.GetBuffer());
                }

                _rawMessage.Text = text;

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

                if (MessageSelected != null)
                {
                    MessageSelected(message);
                }

                ApplyFieldSearch();
            }
            finally
            {
                _fieldTable.EndLoadData();
                UpdateUiState();
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
                    _session.Messages.MessageAdded -= MessageAdded;
                    _session.MessageFilterChanged -= SessionMessageFilterChanged;
                }

                _session = value;

                if (_session != null)
                {
                    _session.Messages.MessageAdded += MessageAdded;
                    _session.MessageFilterChanged += SessionMessageFilterChanged;
                    _session.SessionReset += (sender, ev) => Reload();
                    _session.MessagesReset += (sender, ev) => Reload();

                    _tailMessagesCheckBox.Checked = _session.AutoScrollMessages;
                }

                Reload();
            }
        }

        void MessageAdded(object sender, Fix.MessageCollection.MessageEvent ev)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => MessageAdded(sender, ev)));
                return;
            }

            try
            {
                _messageTable.BeginLoadData();
                AddMesage(ev.Message);
                _messageGrid.RowCount = _messageView.Count;
            }
            finally
            {
                _messageTable.EndLoadData();
            }
            ScrollToBottom();
        }

        void ScrollToBottom()
        {
            if (_messageGrid.Rows.Count > 0 && _session.AutoScrollMessages)
            {
                _messageGrid.FirstDisplayedScrollingRowIndex = _messageGrid.Rows.Count - 1;
            }
        }

        void SessionMessageFilterChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SessionMessageFilterChanged(sender, e)));
                return;
            }

            ApplyMessageSearch();
        }

        void ViewListChanged(object sender, ListChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => ViewListChanged(sender, e)));
                return;
            }

            ScrollToBottom();
        }

        Image ImageForMessageStatus(Fix.MessageStatus status)
        {
            switch (status)
            {
                case Fix.MessageStatus.Error:
                    return Properties.Resources.MessageStatusError;
                case Fix.MessageStatus.Info:
                    return Properties.Resources.MessageStatusInfo;
                case Fix.MessageStatus.Warn:
                    return Properties.Resources.MessageStatusWarn;
            }

            return Properties.Resources.MessageStatusNone;
        }

        void AddMesage(Fix.Message message)
        {
            Fix.Dictionary.Message definition = message.Definition;

            if (definition == null)
            {
                definition = Fix.Dictionary.Messages[message.MsgType];

                if (definition == null)
                {
                    definition = Fix.Dictionary.FIX_4_0.Messages[message.MsgType];
                }
            }

            var row = _messageTable.NewRow() as MessageDataRow;
            row.Message = message;
            row[MessageDataTable.ColumnSendingTime] = message.Fields.Find(Fix.Dictionary.Fields.SendingTime).Value;
            row[MessageDataTable.ColumnMsgType] = message.MsgType;
            row[MessageDataTable.ColumnStatus] = message.Status;
            row[MessageDataTable.ColumnStatusImage] = ImageForMessageStatus(message.Status);
            row[MessageDataTable.ColumnStatusMessage] = message.StatusMessage;
            row[MessageDataTable.ColumnMsgTypeDescription] = definition == null ? null : definition.Name;
            row[MessageDataTable.ColumnMsgSeqNum] = message.Fields.Find(Fix.Dictionary.Fields.MsgSeqNum).Value;
            _messageTable.Rows.Add(row);
        }

        void Reload()
        {
            try
            {
                _messageTable.BeginLoadData();
                _rawMessage.Text = string.Empty;
                _messageGrid.RowCount = 0;
                _messageTable.Clear();
                foreach (Fix.Message message in Session.Messages)
                {
                    AddMesage(message);
                }
                _messageGrid.RowCount = _messageView.Count;
            }
            finally
            {
                _messageTable.EndLoadData();
                ApplyMessageSearch();
            }

            UpdateUiState();
        }
    }
}