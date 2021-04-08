/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: MessagesPanel.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FixClient
{
    partial class MessagesPanel : FixClientPanel
    {
        readonly MessageTypeDataGridView _messageGrid;
        readonly MessageTypeDataTable _messageTable;
        readonly DataView _messageView;
        readonly BindingSource _messageBindingSource;
        string _selectedMsgType;
        bool _suppressMsgTypeUpdate;

        readonly EditableMessageFieldDataGridView _fieldGrid;
        FieldDataTable _fieldTable;
        DataView _fieldView;

        readonly TabControl _tabControl;
        readonly NativeTabControl _nativeTabControl = new NativeTabControl(new Padding(-4, -2, 4, 4));
        readonly InspectorPanel _inspectorPanel;
        readonly MessageOptionsPanel _messageOptionsPanel;

        readonly SearchTextBox _messageSearchTextBox;
        readonly SearchTextBox _fieldSearchTextBox;

        Session _session;

        readonly Timer _timer;

        readonly ToolStripButton _sendButton;
        readonly ToolStripButton _filterButton;
        readonly ToolStripButton _removeFilterButton;
        readonly ToolStripButton _repeatButton;
        readonly ToolStripButton _removeButton;
        readonly ToolStripButton _resetButton;
        readonly ToolStripButton _pasteButton;
        readonly ToolStripButton _editGoaButton;

        readonly ToolStripMenuItem _sendMenuItem;
        readonly ToolStripMenuItem _filterMenuItem;
        readonly ToolStripMenuItem _removeFilterMenuItem;
        readonly ToolStripMenuItem _repeatMenuItem;
        readonly ToolStripMenuItem _removeMenuItem;
        readonly ToolStripMenuItem _resetMenuItem;
        readonly ToolStripMenuItem _pasteMenuItem;
        readonly ToolStripMenuItem _editGoaMenuItem;

        readonly ToolStripMenuItem _repeatContextMenuItem;
        readonly ToolStripMenuItem _removeContextMenuItem;
        readonly ToolStripMenuItem _resetContextMenuItem;
        readonly ToolStripMenuItem _insertContextMenuItem;

        readonly ContextMenuStrip _contextMenu;
        //
        // This event is used by the mainform to updated the context of the dictionary view
        //
        public delegate void MessageDelegate(Fix.Message message);
        public event MessageDelegate MessageSelected;

        protected void OnMessageSelected(Fix.Message message)
        {
            if (!_suppressMsgTypeUpdate)
            {
                _selectedMsgType = message.MsgType;
            }

            if (MessageSelected != null)
            {
                MessageSelected(message);
            }
        }

        public MessagesPanel()
        {
            var leftSplitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };

            var rightSplitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 1000
            };

            _messageTable = new MessageTypeDataTable("MessageTypes");
            _messageView = new DataView(_messageTable);
            _messageBindingSource = new BindingSource { DataSource = _messageView };

            _messageGrid = new MessageTypeDataGridView { Dock = DockStyle.Fill };
            _messageGrid.SelectionChanged += MessageGridSelectionChanged;
            _messageGrid.DataSource = _messageBindingSource;

            _fieldGrid = new EditableMessageFieldDataGridView();
            _fieldGrid.SelectionChanged += FieldGridSelectionChanged;
            _fieldGrid.CellContextMenuStripNeeded += FieldGridCellContextMenuStripNeeded;

            _inspectorPanel = new InspectorPanel { Dock = DockStyle.Fill };
            _messageOptionsPanel = new MessageOptionsPanel { Dock = DockStyle.Fill };

            var inspectorPage = new TabPage("Dictionary");
            inspectorPage.Controls.Add(_inspectorPanel);

            var optionsPage = new TabPage("Options");
            optionsPage.Controls.Add(_messageOptionsPanel);

            _tabControl = new TabControl { Dock = DockStyle.Fill };
            _nativeTabControl.AssignHandle(_tabControl.Handle);

            _tabControl.TabPages.Add(inspectorPage);
            _tabControl.TabPages.Add(optionsPage);


            #region Message ToolStrip

            var messageToolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = LookAndFeel.Color.ToolStrip,
                Renderer = new ToolStripRenderer()
            };

            _sendButton = new ToolStripButton
            {
                ToolTipText = "Send the selected message to the remote server",
                Image = Properties.Resources.Send,
                ImageTransparentColor = Color.White
            };
            _sendButton.Click += SendButtonClick;
            messageToolStrip.Items.Add(_sendButton);

            #endregion

            #region Field ToolStrip

            var fieldToolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = LookAndFeel.Color.ToolStrip,
                Renderer = new ToolStripRenderer()
            };

            _filterButton = new ToolStripButton
            {
                ToolTipText = "Filter all optional fields which do not have assigned values",
                Image = Properties.Resources.Filter,
                ImageTransparentColor = Color.White
            };
            _filterButton.Click += FilterButtonClick;
            fieldToolStrip.Items.Add(_filterButton);

            _removeFilterButton = new ToolStripButton
            {
                ToolTipText = "Remove all filters from the optional fields",
                Image = Properties.Resources.RemoveFilter,
                ImageTransparentColor = Color.White
            };
            _removeFilterButton.Click += RemoveFilterButtonClick;
            fieldToolStrip.Items.Add(_removeFilterButton);

            _repeatButton = new ToolStripButton
            {
                ToolTipText = "Repeat the selected fields",
                Image = Properties.Resources.RepeatFields,
                ImageTransparentColor = Color.Magenta
            };
            _repeatButton.Click += RepeatButtonClick;
            fieldToolStrip.Items.Add(_repeatButton);

            _removeButton = new ToolStripButton
            {
                ToolTipText = "Remove the selected fields",
                Image = Properties.Resources.RemoveFields,
                ImageTransparentColor = Color.Magenta
            };
            _removeButton.Click += RemoveButtonClick;
            fieldToolStrip.Items.Add(_removeButton);

            _resetButton = new ToolStripButton
            {
                ToolTipText = "Reset the fields of this message to their defaults",
                Image = Properties.Resources.ResetFields,
                ImageTransparentColor = Color.Magenta
            };
            _resetButton.Click += ResetButtonClick;
            fieldToolStrip.Items.Add(_resetButton);

            _pasteButton = new ToolStripButton
            {
                ToolTipText = "Paste a formatted log message or raw FIX message",
                Image = Properties.Resources.Paste,
                ImageTransparentColor = Color.White
            };
            _pasteButton.Click += PasteButtonClick;
            fieldToolStrip.Items.Add(_pasteButton);

            _editGoaButton = new ToolStripButton
            {
                ToolTipText = "Edit GATE Generic Order Attributes",
                Image = Properties.Resources.GOA,
                ImageTransparentColor = Color.Magenta
            };
            _editGoaButton.Click += EditGoaButtonClick;
            fieldToolStrip.Items.Add(_editGoaButton);
            #endregion

            #region MenuStrip
            var menu = new ToolStripMenuItem("Action");

            _sendMenuItem = new ToolStripMenuItem("Send",
                                                 _sendButton.Image,
                                                 SendButtonClick);
            menu.DropDownItems.Add(_sendMenuItem);

            menu.DropDownItems.Add(new ToolStripSeparator());

            _filterMenuItem = new ToolStripMenuItem("Filter",
                                                   _filterButton.Image,
                                                   FilterButtonClick);
            menu.DropDownItems.Add(_filterMenuItem);

            _removeFilterMenuItem = new ToolStripMenuItem("Remove Filters",
                                                         _removeFilterButton.Image,
                                                         RemoveFilterButtonClick);
            menu.DropDownItems.Add(_removeFilterMenuItem);


            _repeatMenuItem = new ToolStripMenuItem("Repeat Fields",
                                                   _repeatButton.Image,
                                                   RepeatButtonClick);
            menu.DropDownItems.Add(_repeatMenuItem);

            _removeMenuItem = new ToolStripMenuItem("Remove Fields",
                                                    _removeButton.Image,
                                                    RemoveButtonClick);

            menu.DropDownItems.Add(_removeMenuItem);

            _resetMenuItem = new ToolStripMenuItem("Reset Fields",
                                                  _resetButton.Image,
                                                  ResetButtonClick);

            menu.DropDownItems.Add(_resetMenuItem);

            _pasteMenuItem = new ToolStripMenuItem("Paste",
                                                  _pasteButton.Image,
                                                  PasteButtonClick,
                                                  Keys.Control | Keys.V);
            menu.DropDownItems.Add(_pasteMenuItem);

            _editGoaMenuItem = new ToolStripMenuItem("Edit GOA",
                                                    _editGoaButton.Image,
                                                    EditGoaButtonClick,
                                                    Keys.Control | Keys.G);
            menu.DropDownItems.Add(_editGoaMenuItem);

            SetMenuStrip(menu);
            #endregion

            #region ContextMenu
            _contextMenu = new ContextMenuStrip();

            _repeatContextMenuItem = new ToolStripMenuItem("Repeat Fields",
                                                          _repeatButton.Image,
                                                          RepeatButtonClick);

            _removeContextMenuItem = new ToolStripMenuItem("Remove Fields",
                                                          _removeButton.Image,
                                                          RemoveButtonClick);

            _insertContextMenuItem = new ToolStripMenuItem("Insert Custom Field",
                                                          Properties.Resources.Customise)
            {
                ImageTransparentColor = Color.Magenta
            };

            _resetContextMenuItem = new ToolStripMenuItem("Reset Fields",
                                                         _resetButton.Image,
                                                         ResetButtonClick);

            _contextMenu.Items.Add(_repeatContextMenuItem);
            _contextMenu.Items.Add(_removeContextMenuItem);
            _contextMenu.Items.Add(_insertContextMenuItem);
            _contextMenu.Items.Add(_resetContextMenuItem);
            #endregion

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


            var messageContainer = new ToolStripContainer
            {
                Dock = DockStyle.Fill,
                BackColor = LookAndFeel.Color.ToolStrip
            };
            messageContainer.TopToolStripPanel.BackColor = LookAndFeel.Color.ToolStrip;
            messageContainer.ContentPanel.Controls.Add(_messageGrid);
            messageContainer.ContentPanel.Controls.Add(_messageSearchTextBox);
            messageContainer.TopToolStripPanel.Controls.Add(messageToolStrip);

            var fieldContainer = new ToolStripContainer
            {
                Dock = DockStyle.Fill,
                BackColor = LookAndFeel.Color.ToolStrip
            };

            fieldContainer.TopToolStripPanel.BackColor = LookAndFeel.Color.ToolStrip;
            fieldContainer.ContentPanel.Controls.Add(_fieldGrid);
            fieldContainer.ContentPanel.Controls.Add(_fieldSearchTextBox);
            fieldContainer.TopToolStripPanel.Controls.Add(fieldToolStrip);

            leftSplitter.Panel1.Controls.Add(messageContainer);
            leftSplitter.Panel2.Controls.Add(fieldContainer);

            rightSplitter.Panel1.Controls.Add(leftSplitter);
            rightSplitter.Panel2.Controls.Add(_tabControl);

            ContentPanel.Controls.Add(rightSplitter);

            _timer = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            _timer.Tick += TimerTick;

            InitializeComponent();

            UpdateUiState();
        }

        void ApplyFieldSearch()
        {
            if (_fieldView == null)
                return;
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
            _fieldView.RowFilter = _session.FieldRowFilter(SelectedMessage.MsgType, search);
        }

        void FieldSearchTextBoxTextChanged(object sender, EventArgs e)
        {
            if (_fieldView == null || SelectedMessage == null)
                return;
            ApplyFieldSearch();
            _fieldSearchTextBox.Focus();
        }

        void MessageSearchTextBoxTextChanged(object sender, EventArgs e)
        {
            if (_messageView == null)
                return;
            string search = null;
            if (string.IsNullOrEmpty(_messageSearchTextBox.Text))
            {
                _messageView.Sort = string.Empty;
            }
            else
            {
                search = string.Format("{0} LIKE '%{2}%' OR {1} LIKE '%{2}%'",
                                       MessageTypeDataTable.ColumnSearchMsgType,
                                       MessageTypeDataTable.ColumnSearchMsgTypeDescription,
                                       _messageSearchTextBox.Text.ToUpper());
            }
            _messageView.RowFilter = _session.MessageRowFilter(search);
            _messageSearchTextBox.Focus();
        }

        Fix.Message SelectedMessage
        {
            get
            {
                if (_messageGrid.SelectedRows.Count == 0)
                    return null;

                var rowView = _messageGrid.SelectedRows[0].DataBoundItem as DataRowView;

                if (rowView == null)
                    return null;

                var messageRow = rowView.Row as MessageTypeDataRow;

                if (messageRow == null)
                    return null;

                return _session.MessageForTemplate(messageRow.Message);
            }
        }

        Fix.Field SelectedField
        {
            get
            {
                if (_fieldGrid.SelectedRows.Count == 0)
                    return null;

                var rowView = _fieldGrid.SelectedRows[0].DataBoundItem as DataRowView;

                if (rowView == null)
                    return null;

                var fieldRow = rowView.Row as FieldDataRow;

                if (fieldRow == null)
                    return null;

                return fieldRow.Field;
            }
        }

        void EditGoaButtonClick(object sender, EventArgs e)
        {
            Fix.Message message = SelectedMessage;

            if (message == null || _fieldGrid.SelectedRows.Count != 1)
            {
                MessageBox.Show(this,
                                "You must select a single field to edit GOA",
                                Application.ProductName,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow row = _fieldGrid.SelectedRows[0];
            var rowView = row.DataBoundItem as DataRowView;

            if (rowView == null)
            {
                throw new Exception(string.Format("MessageFieldDataGridView Row.DataBoundItem at index {0} is not a DataRowView ", row.Index));
            }

            var dataRow = rowView.Row as FieldDataRow;
            if (dataRow == null)
                return;

            var fieldTag = (int)dataRow[FieldDataTable.ColumnTag];
            var fieldName = (string)dataRow[FieldDataTable.ColumnName];

            using (GoaEditor editor = new GoaEditor())
            {
                editor.Text = string.Format("{0} - {1}", fieldTag, fieldName);
                editor.Goa = row.Cells[FieldDataTable.ColumnValue].Value.ToString();
                if (editor.ShowDialog() != DialogResult.OK)
                    return;
                row.Cells[FieldDataTable.ColumnValue].Value = editor.Goa;
            }
        }

        void FieldGridCellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow gridRow = _fieldGrid.Rows[e.RowIndex];
            var rowView = gridRow.DataBoundItem as DataRowView;
            if (rowView == null)
                return;
            DataRow row = rowView.Row;

            ContextMenuRowIndex = _fieldTable.Rows.IndexOf(row);

            if (Session.CustomFields.Any())
            {
                _insertContextMenuItem.DropDownItems.Clear();

                ToolStripMenuItem[] items = new ToolStripMenuItem[Session.CustomFields.Count()];

                int index = 0;

                foreach (CustomField field in Session.CustomFields.Values)
                {
                    items[index] = new ToolStripMenuItem(string.Format("{0} - {1}", field.Tag, field.Name));
                    items[index].Click += InsertCustomFieldClick;
                    items[index++].Tag = field;
                }

                _insertContextMenuItem.DropDownItems.AddRange(items);
            }

            _repeatContextMenuItem.Enabled = _fieldGrid.SelectedRows.Count > 0;
            _removeContextMenuItem.Enabled = _fieldGrid.SelectedRows.Count > 0;
            _insertContextMenuItem.Enabled = Session.CustomFields.Any();
            _resetContextMenuItem.Enabled = _messageGrid.SelectedRows.Count > 0;

            e.ContextMenuStrip = _contextMenu;
        }

        int ContextMenuRowIndex { get; set; }

        void InsertCustomFieldClick(object sender, EventArgs e)
        {
            Fix.Message message = SelectedMessage;

            if (message == null)
                return;

            var item = (ToolStripMenuItem)sender;
            var field = (CustomField)item.Tag;

            message.Fields.Insert(ContextMenuRowIndex, new Fix.Field(field.Tag.ToString(), string.Empty));

            MessageGridSelectionChanged(this, null);
        }

        void ResetButtonClick(object sender, EventArgs e)
        {
            try
            {
                Fix.Message message = SelectedMessage;

                if (message == null)
                    return;

                _session.ResetTemplateMessage(message.MsgType);

                RemoveFilterButtonClick(this, null);
                MessageGridSelectionChanged(this, null);
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

        void RemoveButtonClick(object sender, EventArgs e)
        {
            if (_fieldGrid.SelectedRows.Count == 0)
                return;

            Fix.Message message = SelectedMessage;

            if (message == null)
                return;
            //
            // We need to get the indexes of the rows in the underlying table not the view as the view
            // may have been filtered.
            //
            int end = _fieldTable.Rows.IndexOf((_fieldGrid.SelectedRows[0].DataBoundItem as DataRowView).Row);
            int begin = _fieldTable.Rows.IndexOf((_fieldGrid.SelectedRows[_fieldGrid.SelectedRows.Count - 1].DataBoundItem as DataRowView).Row);

            if (end < begin)
            {
                //
                // If the user selects the rows bottom up thats the order the rows will be in SelectedRows so we need to 
                // reverse the order here.
                //
                int tmp = end;
                end = begin;
                begin = tmp;
            }
            //
            // Update some fields automatically to make life easier for the user.
            //
            if (message.MsgType == Fix.Dictionary.Messages.NewOrderList.MsgType || message.MsgType == Fix.Dictionary.FIX_4_0.Messages.KodiakWaveOrder.MsgType)
            {
                //
                // In the OrderList message the ClOrdID is the delimiter for a repeating group
                // so decrement the ListNoOrders and NoOrders fields by this count.
                //
                int clOrdIdCount = (from FieldDataRow row
                                    in _fieldTable.Rows
                                    where row.Field.Tag == Fix.Dictionary.Fields.ClOrdID.Tag
                                    select row).Count();

                Fix.Field totNoOrders = message.Fields.Find(Fix.Dictionary.Fields.TotNoOrders);

                if (totNoOrders != null)
                {
                    if (Session.AutoTotNoOrders)
                    {
                        message.Fields.Set(Fix.Dictionary.Fields.TotNoOrders, clOrdIdCount);
                    }

                    if (Session.AutoNoOrders)
                    {
                        message.Fields.Set(Fix.Dictionary.Fields.NoOrders, clOrdIdCount);
                    }
                }
            }

            message.Fields.Remove(begin, end - begin + 1);
            //
            // Force the field grid to be updated so the fields disappear.
            //
            MessageGridSelectionChanged(this, null);
        }

        void FieldGridSelectionChanged(object sender, EventArgs e)
        {
            Fix.Field field = SelectedField;

            _inspectorPanel.Field = field;

            _repeatButton.Enabled = field != null;
            _removeButton.Enabled = field != null;
            _repeatMenuItem.Enabled = field != null;
            _removeMenuItem.Enabled = field != null;
            _repeatContextMenuItem.Enabled = field != null;
            _removeContextMenuItem.Enabled = field != null;
            _insertContextMenuItem.Enabled = field != null;
        }

        void RepeatButtonClick(object sender, EventArgs e)
        {
            if (_fieldGrid.SelectedRows.Count == 0)
                return;
            //
            // We need to get the indexes of the rows in the underlying table not the view as the view
            // may have been filtered.
            //
            int end = _fieldTable.Rows.IndexOf((_fieldGrid.SelectedRows[0].DataBoundItem as DataRowView).Row);
            int begin = _fieldTable.Rows.IndexOf((_fieldGrid.SelectedRows[_fieldGrid.SelectedRows.Count - 1].DataBoundItem as DataRowView).Row);

            if (end < begin)
            {
                //
                // If the user selects the rows bottom up thats the order the rows will be in SelectedRows so we need to 
                // reverse the order here.
                //
                int tmp = end;
                end = begin;
                begin = tmp;
            }

            Fix.Message message = SelectedMessage;

            if (message == null)
                return;

            message.Fields.Repeat(begin, end - begin + 1);
            //
            // Update some fields automatically to make life easier for the user.
            //
            if (message.MsgType == Fix.Dictionary.Messages.NewOrderList.MsgType || message.MsgType == Fix.Dictionary.FIX_4_0.Messages.KodiakWaveOrder.MsgType)
            {
                //
                // In the OrderList message the ClOrdID is the delimiter for a repeating group
                // so increment the ListNoOrders and NoOrders fields by this count.
                //
                int clOrdIdCount = (from DataGridViewRow row in _fieldGrid.SelectedRows
                                    select row.DataBoundItem as DataRowView
                                    into rowView
                                    select rowView.Row
                                    into dataRow
                                    select (int)dataRow[FieldDataTable.ColumnTag]).Count(tag => tag == Fix.Dictionary.Fields.ClOrdID.Tag);

                Fix.Field totNoOrders = message.Fields.Find(Fix.Dictionary.Fields.TotNoOrders);

                if (totNoOrders != null && totNoOrders.Value != null && totNoOrders.Value.Trim() != string.Empty)
                {
                    clOrdIdCount += Convert.ToInt32(totNoOrders.Value);
                }
                else
                {
                    //
                    // If the TotNoOrders was not set at all then we have to scan the entire message
                    // to get it right.
                    //
                    clOrdIdCount = message.Fields.Count(field => field.Tag == Fix.Dictionary.Fields.ClOrdID.Tag);
                }

                if (Session.AutoTotNoOrders)
                {
                    message.Fields.Set(Fix.Dictionary.Fields.TotNoOrders, clOrdIdCount);
                }

                if (Session.AutoNoOrders)
                {
                    message.Fields.Set(Fix.Dictionary.Fields.NoOrders, clOrdIdCount);
                }
            }
            //
            // Force the field grid to be updated so we get our new fields.
            //
            MessageGridSelectionChanged(this, null);
        }

        void PasteButtonClick(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText(TextDataFormat.Text))
                return;

            Fix.Message parsedMessage = Fix.Message.Parse(Clipboard.GetText(TextDataFormat.Text));

            if (parsedMessage == null || parsedMessage.Fields.Count == 0)
            {
                MessageBox.Show(this,
                    "Unable to parse the contents of the clipboard",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Fix.Message message = FindMessage(parsedMessage.MsgType);

            if (message == null)
            {
                // TODO
                return;
            }

            using (var form = new PasteMessageForm())
            {
                form.DefineUnknownAsCustom = Session.PasteDefineCustomFields;
                form.FilterEmptyFields = Session.PasteFilterEmptyFields;
                form.SmartPaste = Session.PasteSmart;
                form.ResetExistingMessage = Session.PasteResetExisting;
                form.ProcessRepeatingGroups = Session.PasteProcessRepeatingGroups;

                if (form.ShowDialog() != DialogResult.OK)
                    return;

                Session.PasteDefineCustomFields = form.DefineUnknownAsCustom;
                Session.PasteFilterEmptyFields = form.FilterEmptyFields;
                Session.PasteSmart = form.SmartPaste;
                Session.PasteResetExisting = form.ResetExistingMessage;
                Session.PasteProcessRepeatingGroups = form.ProcessRepeatingGroups;

                Session.Write();

                if (form.ResetExistingMessage)
                {
                    _session.ResetTemplateMessage(message.MsgType);
                }

                if (form.SmartPaste)
                {
                    if (form.ProcessRepeatingGroups)
                    {
                        SmartPasteWithGroups(parsedMessage, message, form.DefineUnknownAsCustom);
                    }
                    else
                    {
                        SmartPasteWithoutGroups(parsedMessage, message, form.DefineUnknownAsCustom);
                    }
                }
                else
                {
                    SimplePaste(parsedMessage, message, form.DefineUnknownAsCustom);
                }

                SelectMessage(message.MsgType);
                MessageGridSelectionChanged(null, null);
                RemoveFilterButtonClick(this, null);

                if (form.FilterEmptyFields)
                {
                    FilterButtonClick(this, null);
                }

                if (form.DefineUnknownAsCustom)
                {
                    Session.WriteCustomFields();
                }
            }
        }

        struct IndentIndex
        {
            public IndentIndex(int indent, int index)
            {
                Indent = indent;
                Index = index;
            }
            public readonly int Indent;
            public readonly int Index;
        }

        void SmartPasteWithGroups(Fix.Message parsedMessage, Fix.Message message, bool defineUnknownAsCustom)
        {
            message.Fields.Clear();

            Fix.Dictionary.Message exemplar = Session.Version.Messages[message.MsgType];

            var indents = new Stack<IndentIndex>();
            var indexes = new Dictionary<int, int>();
            int? groupIndent = null;
            int exemplarIndex = 0;

            foreach (var field in parsedMessage.Fields)
            {
                Fix.Dictionary.Field definition;

                if (!Session.Version.Fields.TryGetValue(field.Tag, out definition) || definition == null)
                {
                    CustomField custom;
                    if (!Session.CustomFields.TryGetValue(field.Tag, out custom) && defineUnknownAsCustom)
                    {
                        custom = new CustomField { Tag = field.Tag, Name = field.Tag.ToString() };
                        Session.AddCustomField(custom);
                    }
                    message.Fields.Add(field.Tag, field.Value);
                    continue;
                }

                for (; exemplarIndex < exemplar.FieldCount; ++exemplarIndex)
                {
                    definition = exemplar.Fields[exemplarIndex];

                    if (indents.Count == 0 || indents.Peek().Indent < definition.Indent)
                    {
                        indents.Push(new IndentIndex(definition.Indent, exemplarIndex));
                    }
                    else if (indents.Peek().Indent > definition.Indent)
                    {
                        if (groupIndent != null && indents.Peek().Indent == groupIndent)
                        {
                            groupIndent = null;
                        }
                        indents.Pop();
                    }

                    if (groupIndent == null)
                    {
                        int index;
                        if (indexes.TryGetValue(field.Tag, out index))
                        {
                            // We've seen this tag already so this means we have a repeating group. Move back to
                            // the start of the group in the definition and continue on.
                            exemplarIndex = index;
                            definition = exemplar.Fields[exemplarIndex];
                            groupIndent = definition.Indent;
                            message.Fields.Add(definition, field.Value);
                            ++exemplarIndex;
                            break;
                        }
                    }

                    if (definition.Tag == field.Tag)
                    {
                        message.Fields.Add(definition, field.Value);
                        indexes[field.Tag] = exemplarIndex;
                        ++exemplarIndex;
                        break;
                    }

                    message.Fields.Add(definition, string.Empty);
                }
            }

            /*
            while (exemplarIndex < exemplar.FieldCount)
            {
                Fix.Dictionary.Field definition = exemplar.Fields[exemplarIndex];
                message.Fields.Add(definition, string.Empty);
                ++exemplarIndex;
            }
            */

        }

        void SmartPasteWithoutGroups(Fix.Message parsedMessage, Fix.Message message, bool defineUnknownAsCustom)
        {
            foreach (Fix.Field field in parsedMessage.Fields)
            {
                Fix.Dictionary.Field definition;
                if (!Session.Version.Fields.TryGetValue(field.Tag, out definition))
                {
                    CustomField custom;
                    if (!Session.CustomFields.TryGetValue(field.Tag, out custom) && defineUnknownAsCustom)
                    {
                        custom = new CustomField { Tag = field.Tag, Name = field.Tag.ToString() };
                        Session.AddCustomField(custom);
                    }
                }

                if (field.Tag == Fix.Dictionary.Fields.ClOrdID.Tag)
                {
                    message.Fields.Set(field.Tag, Session.AutoClOrdId ? Session.FormatClOrdId(Session.NextClOrdId) : field.Value);
                }
                else if (field.Tag == Fix.Dictionary.Fields.OrderID.Tag)
                {
                    message.Fields.Set(field.Tag, Session.NextOrderId);
                }
                else if (field.Tag == Fix.Dictionary.Fields.ExecID.Tag)
                {
                    message.Fields.Set(field.Tag, Session.NextExecId);
                }
                else if (field.Tag == Fix.Dictionary.Fields.ListID.Tag)
                {
                    message.Fields.Set(field.Tag, Session.NextListId);
                }
                else if (field.Tag == Fix.Dictionary.Fields.AllocID.Tag)
                {
                    message.Fields.Set(field.Tag, Session.NextAllocId);
                }
                else if (field.Tag == Fix.Dictionary.Fields.NextExpectedMsgSeqNum.Tag)
                {
                    message.Fields.Set(field.Tag, Session.IncomingSeqNum);
                }
                else
                {
                    message.Fields.Set(field.Tag, field.Value);
                }
            }

            Session.WriteCustomFields();
        }

        void SimplePaste(Fix.Message parsedMessage, Fix.Message message, bool defineUnknownAsCustom)
        {
            message.Fields.Clear();

            foreach (Fix.Field field in parsedMessage.Fields)
            {
                Fix.Dictionary.Field definition;
                if (!Session.Version.Fields.TryGetValue(field.Tag, out definition))
                {
                    CustomField custom;
                    if (!Session.CustomFields.TryGetValue(field.Tag, out custom) && defineUnknownAsCustom)
                    {
                        custom = new CustomField { Tag = field.Tag, Name = field.Tag.ToString() };
                        Session.AddCustomField(custom);
                    }
                }

                if (field.Tag == Fix.Dictionary.Fields.ClOrdID.Tag)
                {
                    message.Fields.Add(field.Tag, Session.AutoClOrdId ? Session.FormatClOrdId(Session.NextClOrdId) : field.Value);
                }
                else if (field.Tag == Fix.Dictionary.Fields.OrderID.Tag)
                {
                    message.Fields.Add(field.Tag, Session.NextOrderId);
                }
                else if (field.Tag == Fix.Dictionary.Fields.ExecID.Tag)
                {
                    message.Fields.Add(field.Tag, Session.NextExecId);
                }
                else if (field.Tag == Fix.Dictionary.Fields.ListID.Tag)
                {
                    message.Fields.Add(field.Tag, Session.NextListId);
                }
                else if (field.Tag == Fix.Dictionary.Fields.AllocID.Tag)
                {
                    message.Fields.Add(field.Tag, Session.NextAllocId);
                }
                else if (field.Tag == Fix.Dictionary.Fields.NextExpectedMsgSeqNum.Tag)
                {
                    message.Fields.Add(field.Tag, Session.IncomingSeqNum);
                }
                else
                {
                    message.Fields.Add(field.Tag, field.Value);
                }
            }

            Session.WriteCustomFields();
        }

        void RemoveFilterButtonClick(object sender, EventArgs e)
        {
            Fix.Message message = SelectedMessage;

            if (message == null)
                return;

            try
            {
                _session.AutoWriteFilters = false;

                foreach (Fix.Field field in message.Fields)
                {
                    Session.FieldVisible(message.MsgType, field.Tag, true);
                }
            }
            finally
            {
                Session.AutoWriteFilters = true;
                Session.WriteFilters();
            }

            ApplyFieldSearch();
        }

        void FilterButtonClick(object sender, EventArgs e)
        {
            Fix.Message message = SelectedMessage;

            if (message == null)
                return;

            try
            {
                Session.AutoWriteFilters = false;

                foreach (Fix.Field field in message.Fields)
                {
                    //if (message.CustomFields.Contains(field.Id))
                    //    continue;

                    if (field.Definition == null)
                        continue;

                    if (field.Definition.Required)
                        continue;

                    if (field.Value != null && !String.IsNullOrEmpty(field.Value.Trim()))
                        continue;

                    Session.FieldVisible(message.MsgType, field.Tag, false);
                }
            }
            finally
            {
                Session.AutoWriteFilters = true;
                Session.WriteFilters();
            }
            //
            // Construct the basic filter that hides deselected fields from the Filters view.
            //
            ApplyFieldSearch();
        }

        void SendButtonClick(object sender, EventArgs e)
        {
            Fix.Message defaults = SelectedMessage;

            if (defaults == null)
                return;

            if (Session == null || !Session.Connected)
                return;

            var message = new Fix.Message();

            message.Fields.Clear();
            //
            // Count the number of ClOrdId's in the message so we can allocate that many in the session.
            //
            int clOrdIdCount = 0;

            foreach (Fix.Field field in defaults.Fields)
            {
                if (string.IsNullOrEmpty(field.Value))
                    continue;

                if (!Session.FieldVisible(defaults.MsgType, field.Tag))
                    continue;
                //
                // Only allocate a ClOrdId if we are actually sending that field.
                //
                if (field.Tag == Fix.Dictionary.Fields.ClOrdID.Tag)
                    ++clOrdIdCount;

                message.Fields.Add(new Fix.Field(field.Tag, field.Value));
            }

            /*
            if (message.MsgType == Fix.Message.Logout)
            {
                //
                // Set this so we don't bounce back another logout when we receive the reply logout.
                //
                Session.ExpectingLogout = true;
            }
            */

            Session.Send(message);
            //
            // This message requires us to change our next outgoing MsgSeqNo
            //
            // When the remote end receives a SequenceReset it is required to ignore the
            // MsgSeqNum and then expect the NewSeqNo as the next incoming MsgSeqNum. We
            // therefore set our next outgoing MsgSeqNo AFTER sending the SequenceReset
            // because the engine sets the MsgSeqNum for us.
            //
            /*
            if (message.MsgType.CompareTo(Fix.Message.SequenceReset) == 0)
            {
                foreach (DataGridViewRow row in _fieldGrid.Rows)
                {
                    DataRowView rowView = row.DataBoundItem as DataRowView;
                    DataRow dataRow = rowView.Row;
                    Fix.Field.FieldId fieldId = (Fix.Field.FieldId) dataRow[FieldDataTable.ColumnId];

                    if (fieldId == Fix.Field.FieldId.NewSeqNo)
                    {
                        Session.NextSeqNoOut = Convert.ToInt32(row.Cells[FieldDataTable.ColumnValue].Value);
                        break;
                    }
                }
            }
            */

            var updatedFields = new List<KeyValuePair<int, string>>();

            if (message.MsgType == Fix.Dictionary.Messages.NewOrderSingle.MsgType ||
                message.MsgType == Fix.Dictionary.Messages.NewOrderList.MsgType ||
                message.MsgType == Fix.Dictionary.Messages.OrderCancelRequest.MsgType ||
                message.MsgType == Fix.Dictionary.Messages.OrderCancelReplaceRequest.MsgType ||
                message.MsgType == Fix.Dictionary.FIX_4_0.Messages.KodiakWaveOrder.MsgType ||
                message.MsgType == Fix.Dictionary.FIX_4_0.Messages.KodiakWaveOrderCancelRequest.MsgType ||
                message.MsgType == Fix.Dictionary.FIX_4_0.Messages.KodiakWaveOrderCorrectionRequest.MsgType ||
                message.MsgType == Fix.Dictionary.Messages.AllocationInstruction.MsgType)
            {
                //
                // Allocate any ClOrdIds that were used.
                //
                if (Session.AutoClOrdId)
                {
                    Session.NextClOrdId += clOrdIdCount;
                }
                //
                // We may have multiple ClOrdIds in one message such as order list but we don't want to
                // allocate them until the message is actually sent so we just increment the counter
                // locally.
                //
                int nextClOrdId = Session.NextClOrdId;

                for (int index = 0; index < defaults.Fields.Count; ++index)
                {
                    Fix.Field field = defaults.Fields[index];

                    if (field.Tag == Fix.Dictionary.Fields.MsgSeqNum.Tag)
                    {
                        defaults.Fields[index].Value = Session.OutgoingSeqNum.ToString();
                        updatedFields.Add(new KeyValuePair<int, string>(index, Session.OutgoingSeqNum.ToString()));
                    }
                    else if (field.Tag == Fix.Dictionary.Fields.ClOrdID.Tag && Session.AutoClOrdId)
                    {
                        string value = Session.FormatClOrdId(nextClOrdId);
                        defaults.Fields[index].Value = value;
                        updatedFields.Add(new KeyValuePair<int, string>(index, value));
                        ++nextClOrdId;
                    }
                    else if (field.Tag == Fix.Dictionary.Fields.ListID.Tag && Session.AutoListId)
                    {
                        if (message.MsgType != Fix.Dictionary.Messages.OrderCancelRequest.MsgType &&
                            message.MsgType != Fix.Dictionary.Messages.OrderCancelReplaceRequest.MsgType)
                        {
                            int listId = Session.NextListId++;
                            defaults.Fields[index].Value = listId.ToString();
                            updatedFields.Add(new KeyValuePair<int, string>(index, listId.ToString()));
                        }
                    }
                    else if (field.Tag == Fix.Dictionary.Fields.AllocID.Tag && Session.AutoAllocId)
                    {
                        int allocId = Session.NextAllocId++;
                        defaults.Fields[index].Value = allocId.ToString();
                        updatedFields.Add(new KeyValuePair<int, string>(index, allocId.ToString()));
                    }
                }
            }
            else if (message.MsgType == Fix.Dictionary.Messages.ExecutionReport.MsgType)
            {
                for (int index = 0; index < defaults.Fields.Count; ++index)
                {
                    Fix.Field field = defaults.Fields[index];

                    if (field.Tag == Fix.Dictionary.Fields.MsgSeqNum.Tag)
                    {
                        defaults.Fields[index].Value = Session.OutgoingSeqNum.ToString();
                        updatedFields.Add(new KeyValuePair<int, string>(index, Session.OutgoingSeqNum.ToString()));
                    }
                    else if (field.Tag == Fix.Dictionary.Fields.OrderID.Tag)
                    {
                        int orderId = Session.NextOrderId++;
                        defaults.Fields[index].Value = orderId.ToString();
                        updatedFields.Add(new KeyValuePair<int, string>(index, orderId.ToString()));
                    }
                    else if (field.Tag == Fix.Dictionary.Fields.ExecID.Tag)
                    {
                        int execId = Session.NextExecId++;
                        defaults.Fields[index].Value = execId.ToString();
                        updatedFields.Add(new KeyValuePair<int, string>(index, execId.ToString()));
                    }
                }
            }
            else if (message.MsgType == Fix.Dictionary.Messages.TradeCaptureReport.MsgType &&
                     Session.AutoTradeReportId &&
                     Session.OrderBehaviour == Fix.Behaviour.Initiator)
            {
                for (int index = 0; index < defaults.Fields.Count; ++index)
                {
                    Fix.Field field = defaults.Fields[index];

                    if (field.Tag == Fix.Dictionary.Fields.TradeReportID.Tag)
                    {
                        int tradeReportId = Session.NextTradeReportId++;
                        defaults.Fields[index].Value = tradeReportId.ToString();
                        updatedFields.Add(new KeyValuePair<int, string>(index, tradeReportId.ToString()));
                    }
                }
            }
            else if (message.MsgType == Fix.Dictionary.Messages.TradeCaptureReport.MsgType && Session.AutoTradeId)
            {
                for (int index = 0; index < defaults.Fields.Count; ++index)
                {
                    Fix.Field field = defaults.Fields[index];

                    if (field.Tag == Fix.Dictionary.Fields.TradeID.Tag)
                    {
                        int tradeId = Session.NextTradeId++;
                        defaults.Fields[index].Value = tradeId.ToString();
                        updatedFields.Add(new KeyValuePair<int, string>(index, tradeId.ToString()));
                    }
                }
            }

            foreach (KeyValuePair<int, string> field in updatedFields)
            {
                _fieldTable.Rows[field.Key][FieldDataTable.ColumnValue] = field.Value;
            }

            _fieldGrid.RefreshEdit();
        }

        public void UpdateUiState()
        {
            if (Session == null || !Session.Connected)
            {
                _sendButton.Enabled = false;
                _sendMenuItem.Enabled = false;
            }
            else
            {
                _sendButton.Enabled = true;
                _sendMenuItem.Enabled = true;
            }

            _filterButton.Enabled = Session != null;
            _removeFilterButton.Enabled = Session != null;
            _pasteButton.Enabled = Session != null;
            _editGoaButton.Enabled = Session != null;

            _filterMenuItem.Enabled = Session != null;
            _removeFilterMenuItem.Enabled = Session != null;
            _pasteMenuItem.Enabled = Session != null;
            _editGoaMenuItem.Enabled = Session != null;

            _repeatButton.Enabled = _fieldGrid.SelectedRows.Count > 0;
            _removeButton.Enabled = _fieldGrid.SelectedRows.Count > 0;
            _resetButton.Enabled = _messageGrid.SelectedRows.Count > 0;

            _repeatMenuItem.Enabled = _fieldGrid.SelectedRows.Count > 0;
            _removeMenuItem.Enabled = _fieldGrid.SelectedRows.Count > 0;
            _resetMenuItem.Enabled = _messageGrid.SelectedRows.Count > 0;

            _messageSearchTextBox.Enabled = Session != null;
            _fieldSearchTextBox.Enabled = Session != null;
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
                    _session.MessageFilterChanged -= SessionMessageFilterChanged;
                    _session.FieldFilterChanged -= SessionFieldFilterChanged;
                    _session.SessionReset -= SessionSessionReset;
                }

                _session = value;
                Reload();

                if (_session != null)
                {
                    _session.MessageFilterChanged += SessionMessageFilterChanged;
                    _session.FieldFilterChanged += SessionFieldFilterChanged;
                    _session.SessionReset += SessionSessionReset;
                }
            }
        }

        void SessionSessionReset(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SessionSessionReset(sender, e)));
                return;
            }

            MessageGridSelectionChanged(this, null);
        }

        void SessionMessageFilterChanged(object sender, EventArgs e)
        {
            _messageView.RowFilter = Session.MessageRowFilter();
        }

        void SessionFieldFilterChanged(object sender, EventArgs e)
        {
            if (Session == null)
                return;
            Fix.Message message = SelectedMessage;
            if (message == null)
                return;
            ApplyFieldSearch();
        }

        public void Reload()
        {
            try
            {
                _suppressMsgTypeUpdate = true;
                _messageTable.BeginLoadData();
                _messageTable.Clear();

                foreach (Fix.Dictionary.Message message in _session.Version.Messages)
                {
                    var row = _messageTable.NewRow() as MessageTypeDataRow;

                    if (row != null)
                    {
                        row.Message = message;
                        row[MessageTypeDataTable.ColumnMsgType] = message.MsgType;
                        row[MessageTypeDataTable.ColumnMsgTypeDescription] = message.Name;
                        // The row filter does not give us a means to do case insensitive search or ToUpper() etc so
                        // stuff an uppercase copy of the MsgType in here just for searching.
                        row[MessageTypeDataTable.ColumnSearchMsgType] = message.MsgType.ToUpper();
                        row[MessageTypeDataTable.ColumnSearchMsgTypeDescription] = message.Name.ToUpper();
                        _messageTable.Rows.Add(row);
                    }
                }

                _messageView.RowFilter = _session.MessageRowFilter();
            }
            finally
            {
                _messageTable.EndLoadData();
                _suppressMsgTypeUpdate = false;
            }
            //
            // Preserve the user's selection across disconnects.
            //
            if (!string.IsNullOrEmpty(_selectedMsgType))
            {
                SelectMessage(_selectedMsgType);
            }

            _messageOptionsPanel.Session = Session;
        }

        void MessageGridSelectionChanged(object sender, EventArgs e)
        {
            Fix.Message message = SelectedMessage;

            _inspectorPanel.Message = message == null ? null : _session.Version.Messages[message.MsgType];

            try
            {
                _fieldGrid.DataSource = null;
                _fieldTable = new FieldDataTable("Fields");
                _fieldGrid.Message = message;

                if (message == null)
                    return;

                message.Fields.Set(Fix.Dictionary.Fields.BeginString, _session.BeginString);

                if (_session.BeginString.BeginString == Fix.Dictionary.Versions.FIXT_1_1.BeginString &&
                    message.MsgType == Fix.Dictionary.Messages.Logon.MsgType)
                {
                    message.Fields.Set(Fix.Dictionary.Fields.DefaultApplVerID, _session.DefaultApplVerId.ApplVerID);
                    message.Fields.Set(Fix.Dictionary.Fields.EncryptMethod, Fix.EncryptMethod.None);
                    message.Fields.Set(Fix.Dictionary.Fields.HeartBtInt, _session.HeartBtInt);
                }

                message.Fields.Set(Fix.Dictionary.Fields.SenderCompID, _session.SenderCompId);
                message.Fields.Set(Fix.Dictionary.Fields.TargetCompID, _session.TargetCompId);

                if (_session.AutoSetMsgSeqNum)
                {
                    message.Fields.Set(Fix.Dictionary.Fields.MsgSeqNum, Session.OutgoingSeqNum);
                }
                //
                // We may have multiple ClOrdIds in one message such as order list but we don't want to
                // allocate them until the message is actually sent so we just increment the counter
                // locally.
                //
                int nextClOrdId = Session.NextClOrdId;
                int nextAllocId = Session.NextAllocId;
                int nextListSeqNo = 1;
                int index = 0;
                string time = Fix.Field.TimeString(_session.MillisecondTimestamps);

                foreach (Fix.Field field in message.Fields)
                {
                    if (field.Value != null && !string.IsNullOrEmpty(field.Value.Trim()))
                    {
                        _session.FieldVisible(message.MsgType, field.Tag, true);
                    }

                    if (field.Definition == null)
                    {
                        field.Definition = Session.FieldDefinition(message, field);
                    }
                    //
                    // Update any time fields as required.
                    //
                    if ((field.Tag == Fix.Dictionary.Fields.SendingTime.Tag && Session.AutoSendingTime) ||
                        (field.Tag == Fix.Dictionary.Fields.TransactTime.Tag && Session.AutoTransactTime))
                    {
                        message.Fields[index].Value = time;
                    }

                    if (field.Tag == Fix.Dictionary.Fields.NextExpectedMsgSeqNum.Tag &&
                        Session.NextExpectedMsgSeqNum)
                    {
                        message.Fields[index].Value = Session.IncomingSeqNum.ToString();
                    }

                    //
                    // Update any of the various ID's as required
                    //
                    if (message.MsgType == Fix.Dictionary.Messages.NewOrderSingle.MsgType ||
                        message.MsgType == Fix.Dictionary.Messages.NewOrderList.MsgType ||
                        message.MsgType == Fix.Dictionary.FIX_4_0.Messages.KodiakWaveOrder.MsgType)
                    {

                        if (field.Tag == Fix.Dictionary.Fields.ClOrdID.Tag)
                        {
                            message.Fields[index].Value = Session.FormatClOrdId(nextClOrdId);
                            ++nextClOrdId;
                        }
                        else if (field.Tag == Fix.Dictionary.Fields.OrderID.Tag)
                        {
                            message.Fields[index].Value = Session.NextOrderId.ToString();
                        }
                        else if (field.Tag == Fix.Dictionary.Fields.ExecID.Tag)
                        {
                            message.Fields[index].Value = Session.NextExecId.ToString();
                        }
                        else if (field.Tag == Fix.Dictionary.Fields.ListID.Tag)
                        {
                            message.Fields[index].Value = Session.NextListId.ToString();
                        }
                        else if (field.Tag == Fix.Dictionary.Fields.ListSeqNo.Tag)
                        {
                            if (Session.AutoListSeqNo && message.MsgType != Fix.Dictionary.FIX_4_0.Messages.KodiakWaveOrder.MsgType)
                            {
                                message.Fields[index].Value = nextListSeqNo.ToString();
                                ++nextListSeqNo;
                            }
                        }
                        else if (field.Tag == Fix.Dictionary.Fields.AllocID.Tag)
                        {
                            message.Fields[index].Value = nextAllocId.ToString();
                        }
                    }
                    else if (message.MsgType == Fix.Dictionary.Messages.AllocationInstruction.MsgType)
                    {
                        if (field.Tag == Fix.Dictionary.Fields.AllocID.Tag)
                        {
                            message.Fields[index].Value = nextAllocId.ToString();
                        }
                    }
                    else if (message.MsgType == Fix.Dictionary.Messages.TradeCaptureReport.MsgType &&
                             Session.OrderBehaviour == Fix.Behaviour.Initiator)
                    {
                        if (field.Tag == Fix.Dictionary.Fields.TradeReportID.Tag)
                        {
                            message.Fields[index].Value = Session.NextTradeReportId.ToString();
                        }
                    }

                    ++index;
                }

                message.Fields.Set(Fix.Dictionary.Fields.BodyLength, message.ComputeBodyLength());

                //
                // Now the message is ready populate the field view
                //
                int previousIndent = 0;

                foreach (Fix.Field field in message.Fields)
                {
                    if (field.Tag == Fix.Dictionary.Fields.CheckSum.Tag)
                    {
                        //
                        // This gets set automatically by the library when sending a message across
                        // the transport so there is no need to make it editable.
                        //
                        continue;
                    }

                    var dataRow = _fieldTable.NewRow() as FieldDataRow;

                    if (dataRow == null)
                        continue;

                    dataRow.Field = field;

                    if (field.Definition != null)
                    {
                        dataRow[FieldDataTable.ColumnIndent] = field.Definition.Indent;
                        dataRow[FieldDataTable.ColumnName] = field.Definition.Name;
                        dataRow[FieldDataTable.ColumnRequired] = field.Definition.Required;
                        dataRow[FieldDataTable.ColumnCustom] = false;
                        previousIndent = field.Definition.Indent;
                    }
                    else
                    {
                        CustomField custom;

                        if (Session.CustomFields.TryGetValue(field.Tag, out custom))
                        {
                            dataRow[FieldDataTable.ColumnName] = custom.Name;
                            dataRow[FieldDataTable.ColumnCustom] = true;
                        }
                        else
                        {
                            dataRow[FieldDataTable.ColumnName] = string.Empty;
                            dataRow[FieldDataTable.ColumnCustom] = false;
                        }

                        dataRow[FieldDataTable.ColumnIndent] = previousIndent;
                        dataRow[FieldDataTable.ColumnRequired] = false;
                    }

                    dataRow[FieldDataTable.ColumnTag] = field.Tag;
                    dataRow[FieldDataTable.ColumnValue] = field.Value;

                    _fieldTable.Rows.Add(dataRow);
                }
            }
            finally
            {
                _fieldView = new DataView(_fieldTable);

                FieldSearchTextBoxTextChanged(_fieldSearchTextBox, null);
                _fieldGrid.DataSource = _fieldView;
                if (message != null)
                {
                    ApplyFieldSearch();
                }
                _fieldGrid.Focus();
            }

            OnMessageSelected(message);
        }

        void TimerTick(object sender, EventArgs e)
        {
            if (_session == null || string.IsNullOrEmpty(_session.FileName))
                return;

            if (_fieldTable == null)
                return;

            string time = Fix.Field.TimeString(_session.MillisecondTimestamps);

            foreach (FieldDataRow row in _fieldTable.Rows)
            {
                Fix.Field field = row.Field;
                if ((field.Tag == Fix.Dictionary.Fields.SendingTime.Tag && Session.AutoSendingTime) ||
                    (field.Tag == Fix.Dictionary.Fields.TransactTime.Tag && Session.AutoTransactTime))
                {
                    row[FieldDataTable.ColumnValue] = time;
                }
            }

            Session.WriteTemplates();
        }

        public void UpdateMessage(Fix.Message message, Fix.Order order)
        {
            //
            // Order.Messages are stored in arrival order so we just iterate through and get
            // the most recent version of each field.
            //
            foreach (Fix.Field field in message.Fields)
            {
                if (field.Tag == Fix.Dictionary.Fields.MsgType.Tag)
                    continue;

                foreach (Fix.Message source in order.Messages)
                {
                    Fix.Field sourceField = source.Fields.Find(field.Tag);

                    if (sourceField == null)
                        continue;

                    message.Fields.Set(field.Tag, sourceField.Value);
                }
            }
        }

        public void AcknowledgeAmend(Fix.Order order)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.ExecutionReport.MsgType);

            if (message == null)
                return;

            string ClOrdID = order.PendingMessage.ClOrdID;
            string OrigClOrdID = order.PendingMessage.OrigClOrdID;

            Fix.Message lastMessage = order.Messages.LastOrDefault();

            Fix.OrdStatus ordStatus = Fix.OrdStatus.Replaced;
            Fix.ExecType execType = Fix.ExecType.Replaced;

            if (lastMessage != null && lastMessage.MsgType == Fix.Dictionary.Messages.OrderCancelReplaceRequest.MsgType)
            {
                ordStatus = Fix.OrdStatus.PendingReplace;
                execType = Fix.ExecType.PendingReplace;
                message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, ClOrdID);
                message.Fields.Set(Fix.Dictionary.Fields.OrigClOrdID, OrigClOrdID);
            }
            else
            {
                message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, ClOrdID);
                message.Fields.Set(Fix.Dictionary.Fields.OrigClOrdID, OrigClOrdID);
            }

            if (order.Side.HasValue)
            {
                message.Fields.Set(Fix.Dictionary.Fields.Side, (Fix.Side)order.Side);
            }

            message.Fields.Set(Fix.Dictionary.Fields.Symbol, order.Symbol);
            message.Fields.Set(Fix.Dictionary.Fields.OrderQty, order.OrderQty);
            message.Fields.Set(Fix.Dictionary.Fields.OrdStatus, ordStatus);
            if (order.OrderID != null)
            {
                message.Fields.Set(Fix.Dictionary.Fields.OrderID, order.OrderID);
            }
            message.Fields.Set(Fix.Dictionary.Fields.ExecID, Session.NextExecId.ToString());
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, 0);
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, 0);
            message.Fields.Set(Fix.Dictionary.Fields.CumQty, order.CumQty ?? 0);
            message.Fields.Set(Fix.Dictionary.Fields.AvgPx, order.AvgPx ?? 0);

            if (Session.Version.BeginString != Fix.Dictionary.Versions.FIX_4_0.BeginString)
            {
                message.Fields.Set(Fix.Dictionary.Fields.ExecType, execType);
                message.Fields.Set(Fix.Dictionary.Fields.LeavesQty, 0);
            }

            if (Session.Version.BeginString.StartsWith("FIX.4."))
            {
                message.Fields.Set(Fix.Dictionary.FIX_4_0.Fields.ExecTransType, Fix.Dictionary.FIX_4_0.ExecTransType.New);
            }

            SelectMessage(message.MsgType);
        }

        public void AcknowledgeCancel(Fix.Order order)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.ExecutionReport.MsgType);

            if (message == null)
                return;

            message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, order.ClOrdID);

            if (order.Side.HasValue)
            {
                message.Fields.Set(Fix.Dictionary.Fields.Side, (Fix.Side)order.Side);
            }

            message.Fields.Set(Fix.Dictionary.Fields.Symbol, order.Symbol);
            message.Fields.Set(Fix.Dictionary.Fields.OrderQty, order.OrderQty);
            message.Fields.Set(Fix.Dictionary.Fields.OrdStatus, Fix.OrdStatus.Canceled);
            if (order.OrderID != null)
            {
                message.Fields.Set(Fix.Dictionary.Fields.OrderID, order.OrderID);
            }
            message.Fields.Set(Fix.Dictionary.Fields.ExecID, Session.NextExecId.ToString());
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, 0);
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, 0);
            message.Fields.Set(Fix.Dictionary.Fields.CumQty, order.CumQty ?? 0);
            message.Fields.Set(Fix.Dictionary.Fields.AvgPx, order.AvgPx ?? 0);

            if (Session.Version.BeginString != Fix.Dictionary.Versions.FIX_4_0.BeginString)
            {
                message.Fields.Set(Fix.Dictionary.Fields.ExecType, Fix.ExecType.Canceled);
                message.Fields.Set(Fix.Dictionary.Fields.LeavesQty, 0);
            }

            if (Session.Version.BeginString.StartsWith("FIX.4."))
            {
                message.Fields.Set(Fix.Dictionary.FIX_4_0.Fields.ExecTransType, Fix.Dictionary.FIX_4_0.ExecTransType.New);
            }

            SelectMessage(message.MsgType);
        }

        public void ReplyTradeReport(Fix.TradeReport.ReportSide buySide, Fix.TradeReport.ReportSide sellSide)
        {
            Fix.Message target = FindMessage(Fix.Dictionary.Messages.TradeCaptureReport.MsgType);

            if (target == null)
                return;
            //
            // This message has repeating groups so we can't just set each field we need. Construct a message with the
            // fields we want and then just do a smart paste into the target message.
            //
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReport.MsgType };

            Fix.TradeReport trade = buySide?.TradeReport ?? sellSide?.TradeReport;

            if (trade == null)
                return;

            message.Fields.Add(Fix.Dictionary.Fields.LastQty, trade.LastQty);
            message.Fields.Add(Fix.Dictionary.Fields.LastPx, trade.LastPx);

            message.Fields.Add(Fix.Dictionary.Fields.ExchangeTradeType, Fix.ExchangeTradeType.ManualTrade);
            message.Fields.Add(Fix.Dictionary.Fields.ExecType, Fix.ExecType.Trade);

            string securityExchange = trade.SecurityExchange;
            if (!string.IsNullOrEmpty(securityExchange))
                message.Fields.Add(Fix.Dictionary.Fields.SecurityExchange, securityExchange);

            string securityID = trade.SecurityID;
            if (!string.IsNullOrEmpty(securityID))
                message.Fields.Add(Fix.Dictionary.Fields.SecurityID, securityID);

            Fix.SecurityIDSource? securityIDSource = trade.SecurityIDSource;
            if (securityIDSource != null)
                message.Fields.Add(Fix.Dictionary.Fields.SecurityIDSource, securityIDSource.Value);

            message.Fields.Add(Fix.Dictionary.Fields.TradeHandlingInstr, Fix.TradeHandlingInstr.TradeConfirmation);

            message.Fields.Add(Fix.Dictionary.Fields.TradeID, Session.NextTradeId);
            message.Fields.Add(Fix.Dictionary.Fields.TradeReportID, trade.TradeReportID);

            message.Fields.Add(Fix.Dictionary.Fields.TradeReportTransType, Fix.TradeReportTransType.Replace);
            message.Fields.Add(Fix.Dictionary.Fields.TradeReportType, Fix.TradeReportType.Submit);
            message.Fields.Add(Fix.Dictionary.Fields.TrdRptStatus, Fix.TrdRptStatus.Accepted);
            message.Fields.Add(Fix.Dictionary.Fields.TrdType, Fix.TrdType.PrivatelyNegotiatedTrades);


            if (buySide != null && sellSide != null)
            {
                message.Fields.Add(Fix.Dictionary.Fields.NoSides, Fix.NoSides.BothSides);
            }
            else
            {
                message.Fields.Add(Fix.Dictionary.Fields.NoSides, Fix.NoSides.OneSide);
            }

            if (buySide != null)
            {
                message.Fields.Add(Fix.Dictionary.Fields.Side, buySide.Side);
                message.Fields.Add(Fix.Dictionary.Fields.NoPartyIDs, 1);
                message.Fields.Add(Fix.Dictionary.Fields.PartyID, buySide.PartyID);
                message.Fields.Add(Fix.Dictionary.Fields.PartyIDSource, Fix.PartyIDSource.Proprietary);
                message.Fields.Add(Fix.Dictionary.Fields.PartyRole, Fix.PartyRole.ExecutingFirm);

                message.Fields.Add(Fix.Dictionary.Fields.NoClearingInstructions, 1);
                message.Fields.Add(Fix.Dictionary.Fields.ClearingInstruction, Fix.ClearingInstruction.ProcessNormally);
                message.Fields.Add(Fix.Dictionary.Fields.OrderCategory, Fix.OrderCategory.InternalCrossOrder);
            }

            if (sellSide != null)
            {
                // We need to repeat the side group before doing these fields - effectively smart paste (maybe construct a message and paste it?).
                message.Fields.Add(Fix.Dictionary.Fields.Side, sellSide.Side);
                message.Fields.Add(Fix.Dictionary.Fields.NoPartyIDs, 1);
                message.Fields.Add(Fix.Dictionary.Fields.PartyID, sellSide.PartyID);
                message.Fields.Add(Fix.Dictionary.Fields.PartyIDSource, Fix.PartyIDSource.Proprietary);
                message.Fields.Add(Fix.Dictionary.Fields.PartyRole, Fix.PartyRole.ExecutingFirm);
            }

            SimplePaste(message, target, true);

            SelectMessage(message.MsgType);
        }

        public void AcknowledgeTradeReport(Fix.TradeReport.ReportSide buySide, Fix.TradeReport.ReportSide sellSide)
        {
            Fix.Message target = FindMessage(Fix.Dictionary.Messages.TradeCaptureReportAck.MsgType);

            if (target == null)
                return;
            //
            // This message has repeating groups so we can't just set each field we need. Construct a message with the
            // fields we want and then just do a smart paste into the target message.
            //
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReportAck.MsgType };

            Fix.TradeReport trade = buySide?.TradeReport ?? sellSide?.TradeReport;

            if (trade == null)
                return;



            /*
                                      Custom(5681): M
                                    ExecType( 150): F
                                      LastPx(  31): 60.05
                                     LastQty(  32): 100
                                     SecurityExchange( 207): XHKG
                                  SecurityID(  48): 3699
                                    IDSource(  22): 8
                          TradeHandlingInstr(1123): 0
                                     TradeID(1003): 3699000000146
                        TradeReportTransType( 487): 0
                             TradeReportType( 856): 0
                                TransactTime(  60): 20150714-01:32:58.000
                                TrdRptStatus( 939): 0 - Accepted
                                     TrdType( 828): 22
                                     NoSides( 552): 2

                                        Side(  54): 1
                                  NoPartyIDs( 453): 1
                                     PartyID( 448): 7268
                               PartyIDSource( 447): D
                                   PartyRole( 452): 1

                      NoClearingInstructions( 576): 1
                         ClearingInstruction( 577): 0
                               OrderCategory(1115): A
                                  
                                        Side(  54): 2
                                  NoPartyIDs( 453): 1
                                     PartyID( 448): 7268
                               PartyIDSource( 447): D
                                   PartyRole( 452): 17
                            
                                    CheckSum(  10): 130

            */

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

            SimplePaste(message, target, true);

            SelectMessage(message.MsgType);
        }

        public void RejectTradeReport(Fix.TradeReport.ReportSide buySide, Fix.TradeReport.ReportSide sellSide)
        {
            Fix.Message target = FindMessage(Fix.Dictionary.Messages.TradeCaptureReportAck.MsgType);

            if (target == null)
                return;
            //
            // This message has repeating groups so we can't just set each field we need. Construct a message with the
            // fields we want and then just do a smart paste into the target message.
            //
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReportAck.MsgType };

            Fix.TradeReport trade = buySide?.TradeReport ?? sellSide?.TradeReport;

            if (trade == null)
                return;

            message.Fields.Set(Fix.Dictionary.Fields.TradeReportID, trade.TradeReportID);

            message.Fields.Set(Fix.Dictionary.Fields.ExecType, Fix.ExecType.New);
            message.Fields.Add(Fix.Dictionary.Fields.TradeReportType, Fix.TradeReportType.Submit);
            message.Fields.Set(Fix.Dictionary.Fields.TrdRptStatus, Fix.TrdRptStatus.Rejected);
            message.Fields.Set(Fix.Dictionary.Fields.TradeReportRejectReason, Fix.TradeReportRejectReason.InvalidTradeType);

            message.Fields.Set(Fix.Dictionary.Fields.Symbol, trade.Symbol);
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, trade.LastQty);
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, trade.LastPx);
            message.Fields.Set(Fix.Dictionary.Fields.TrdType, trade.TrdType);
            //message.Fields.Set(Fix.Dictionary.Fields.TradeReportTransType, Fix.TradeReportTransType.New);

            if (buySide != null && sellSide != null)
            {
                message.Fields.Add(Fix.Dictionary.Fields.NoSides, Fix.NoSides.BothSides);
            }
            else
            {
                message.Fields.Add(Fix.Dictionary.Fields.NoSides, Fix.NoSides.OneSide);
            }

            if (buySide != null)
            {
                message.Fields.Add(Fix.Dictionary.Fields.Side, buySide.Side);
                message.Fields.Add(Fix.Dictionary.Fields.NoPartyIDs, 1);
                message.Fields.Add(Fix.Dictionary.Fields.PartyID, buySide.PartyID);
                message.Fields.Add(Fix.Dictionary.Fields.PartyIDSource, Fix.PartyIDSource.Proprietary);
                message.Fields.Add(Fix.Dictionary.Fields.PartyRole, Fix.PartyRole.ExecutingFirm);

                message.Fields.Add(Fix.Dictionary.Fields.NoClearingInstructions, 1);
                message.Fields.Add(Fix.Dictionary.Fields.ClearingInstruction, Fix.ClearingInstruction.ProcessNormally);
                message.Fields.Add(Fix.Dictionary.Fields.OrderCategory, Fix.OrderCategory.InternalCrossOrder);
            }

            if (sellSide != null)
            {
                // We need to repeat the side group before doing these fields - effectively smart paste (maybe construct a message and paste it?).
                message.Fields.Add(Fix.Dictionary.Fields.Side, sellSide.Side);
                message.Fields.Add(Fix.Dictionary.Fields.NoPartyIDs, 1);
                message.Fields.Add(Fix.Dictionary.Fields.PartyID, sellSide.PartyID);
                message.Fields.Add(Fix.Dictionary.Fields.PartyIDSource, Fix.PartyIDSource.Proprietary);
                message.Fields.Add(Fix.Dictionary.Fields.PartyRole, Fix.PartyRole.ExecutingFirm);
            }


            SimplePaste(message, target, true);

            SelectMessage(message.MsgType);
        }

        public void AcknowledgeOrder(Fix.Order order)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.ExecutionReport.MsgType);

            if (message == null)
                return;

            message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, order.ClOrdID);

            if (order.Side.HasValue)
            {
                message.Fields.Set(Fix.Dictionary.Fields.Side, (Fix.Side)order.Side);
            }

            message.Fields.Set(Fix.Dictionary.Fields.Symbol, order.Symbol);
            message.Fields.Set(Fix.Dictionary.Fields.OrderQty, order.OrderQty);
            message.Fields.Set(Fix.Dictionary.Fields.OrdStatus, Fix.OrdStatus.New);

            if (order.OrderID == null)
            {
                order.OrderID = Session.NextOrderId.ToString();
            }

            message.Fields.Set(Fix.Dictionary.Fields.OrderID, order.OrderID);
            message.Fields.Set(Fix.Dictionary.Fields.ExecID, Session.NextExecId.ToString());
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, 0);
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, 0);
            message.Fields.Set(Fix.Dictionary.Fields.CumQty, 0);
            message.Fields.Set(Fix.Dictionary.Fields.AvgPx, 0);

            if (Session.Version.BeginString != Fix.Dictionary.Versions.FIX_4_0.BeginString)
            {
                message.Fields.Set(Fix.Dictionary.Fields.ExecType, Fix.ExecType.New);
                message.Fields.Set(Fix.Dictionary.Fields.LeavesQty, order.OrderQty);
            }

            if (Session.Version.BeginString.StartsWith("FIX.4."))
            {
                message.Fields.Set(Fix.Dictionary.FIX_4_0.Fields.ExecTransType, Fix.Dictionary.FIX_4_0.ExecTransType.New);
            }

            SelectMessage(message.MsgType);
        }

        public void RejectOrder(Fix.Order order)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.ExecutionReport.MsgType);

            if (message == null)
                return;

            message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, order.ClOrdID);

            if (order.Side.HasValue)
            {
                message.Fields.Set(Fix.Dictionary.Fields.Side, (Fix.Side)order.Side);
            }

            message.Fields.Set(Fix.Dictionary.Fields.Symbol, order.Symbol);
            message.Fields.Set(Fix.Dictionary.Fields.OrderQty, order.OrderQty);
            message.Fields.Set(Fix.Dictionary.Fields.OrdStatus, Fix.OrdStatus.Rejected);

            if (order.OrderID == null)
            {
                order.OrderID = Session.NextOrderId.ToString();
            }

            message.Fields.Set(Fix.Dictionary.Fields.OrderID, order.OrderID);
            message.Fields.Set(Fix.Dictionary.Fields.ExecID, Session.NextExecId.ToString());
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, 0);
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, 0);
            message.Fields.Set(Fix.Dictionary.Fields.CumQty, 0);
            message.Fields.Set(Fix.Dictionary.Fields.AvgPx, 0);

            if (Session.Version.BeginString != Fix.Dictionary.Versions.FIX_4_0.BeginString)
            {
                message.Fields.Set(Fix.Dictionary.Fields.ExecType, Fix.ExecType.Rejected);
                message.Fields.Set(Fix.Dictionary.Fields.LeavesQty, 0);
            }

            if (Session.Version.BeginString.StartsWith("FIX.4."))
            {
                message.Fields.Set(Fix.Dictionary.FIX_4_0.Fields.ExecTransType, Fix.Dictionary.FIX_4_0.ExecTransType.New);
            }

            SelectMessage(message.MsgType);
        }

        public void ReportOrder(Fix.Order order)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.ExecutionReport.MsgType);

            if (message == null)
                return;

            message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, order.ClOrdID);

            if (order.Side.HasValue)
            {
                message.Fields.Set(Fix.Dictionary.Fields.Side, (Fix.Side)order.Side);
            }

            message.Fields.Set(Fix.Dictionary.Fields.Symbol, order.Symbol);
            message.Fields.Set(Fix.Dictionary.Fields.OrderQty, order.OrderQty);
            message.Fields.Set(Fix.Dictionary.Fields.OrdStatus, Fix.OrdStatus.Filled);

            if (order.OrderID == null)
            {
                order.OrderID = Session.NextOrderId.ToString();
            }

            message.Fields.Set(Fix.Dictionary.Fields.OrderID, order.OrderID);
            message.Fields.Set(Fix.Dictionary.Fields.ExecID, Session.NextExecId.ToString());
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, order.LeavesQty ?? order.OrderQty);
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, order.Price ?? 0);
            message.Fields.Set(Fix.Dictionary.Fields.CumQty, order.OrderQty);
            message.Fields.Set(Fix.Dictionary.Fields.AvgPx, order.Price ?? 0);

            if (Session.Version.BeginString != Fix.Dictionary.Versions.FIX_4_0.BeginString)
            {
                if (Session.Version.BeginString == Fix.Dictionary.Versions.FIX_4_1.BeginString ||
                    Session.Version.BeginString == Fix.Dictionary.Versions.FIX_4_2.BeginString)
                {
                    message.Fields.Set(Fix.Dictionary.Fields.ExecType, Fix.Dictionary.FIX_4_2.ExecType.Fill);
                }
                else
                {
                    message.Fields.Set(Fix.Dictionary.Fields.ExecType, Fix.Dictionary.FIX_5_0.ExecType.Trade);
                }
                message.Fields.Set(Fix.Dictionary.Fields.LeavesQty, 0);
            }

            if (Session.Version.BeginString.StartsWith("FIX.4."))
            {
                message.Fields.Set(Fix.Dictionary.FIX_4_0.Fields.ExecTransType, Fix.Dictionary.FIX_4_0.ExecTransType.New);
            }

            SelectMessage(message.MsgType);
        }

        Fix.Message FindMessage(string msgType)
        {
            var row = _messageTable.Rows.Find(msgType) as MessageTypeDataRow;

            if (row == null)
            {
                MessageBox.Show(this,
                                string.Format("Unable to find the {0} message", msgType),
                                Application.ProductName,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return null;
            }

            return _session.MessageForTemplate(row.Message);
        }

        void SelectMessage(string msgType)
        {
            int previous = _messageBindingSource.Position;

            _messageBindingSource.Position = _messageBindingSource.Find(MessageTypeDataTable.ColumnMsgType, msgType);

            if (_messageBindingSource.Position == previous)
            {
                MessageGridSelectionChanged(this, null);
            }
        }

        public void UnsolicitedCancelOrder(Fix.Order order)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.ExecutionReport.MsgType);

            if (message == null)
                return;

            message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, order.ClOrdID);

            if (order.Side.HasValue)
            {
                message.Fields.Set(Fix.Dictionary.Fields.Side, (Fix.Side)order.Side);
            }

            message.Fields.Set(Fix.Dictionary.Fields.Symbol, order.Symbol);
            message.Fields.Set(Fix.Dictionary.Fields.OrderQty, order.OrderQty);
            message.Fields.Set(Fix.Dictionary.Fields.OrdStatus, Fix.OrdStatus.Canceled);

            if (order.OrderID == null)
            {
                order.OrderID = Session.NextOrderId.ToString();
            }

            message.Fields.Set(Fix.Dictionary.Fields.OrderID, order.OrderID);
            message.Fields.Set(Fix.Dictionary.Fields.ExecID, Session.NextExecId.ToString());
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, 0);
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, 0);
            message.Fields.Set(Fix.Dictionary.Fields.CumQty, order.OrderQty);
            message.Fields.Set(Fix.Dictionary.Fields.AvgPx, order.Price ?? 0);

            if (Session.Version.BeginString != Fix.Dictionary.Versions.FIX_4_0.BeginString)
            {
                message.Fields.Set(Fix.Dictionary.Fields.ExecType, Fix.Dictionary.FIX_4_2.ExecType.Canceled);
                message.Fields.Set(Fix.Dictionary.Fields.LeavesQty, 0);
            }

            if (Session.Version.BeginString.StartsWith("FIX.4."))
            {
                message.Fields.Set(Fix.Dictionary.FIX_4_0.Fields.ExecTransType, Fix.Dictionary.FIX_4_0.ExecTransType.New);
            }

            SelectMessage(message.MsgType);
        }

        public void CancelOrder(Fix.Order order)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.OrderCancelRequest.MsgType);

            if (message == null)
                return;

            UpdateMessage(message, order);

            message.Fields.Set(Fix.Dictionary.Fields.OrigClOrdID, order.ClOrdID);
            message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, Session.FormatClOrdId(Session.NextClOrdId));
            //
            // This field was removed from later versions.
            //
            Fix.Field beginString = message.Fields.Find(Fix.Dictionary.Fields.BeginString);

            if (beginString != null && beginString.Value == Fix.Dictionary.Versions.FIX_4_0.BeginString)
            {
                message.Fields.Set(Fix.Dictionary.FIX_4_0.Fields.CxlType, "F");
            }
            //
            // If we've previously cancelled an order from an OrderList in this session the
            // CancelRequest will have ListID set, we need to unset this if we are cancelling
            // an OrderSingle.
            //
            if (order.ListID == null)
            {
                message.Fields.Set(Fix.Dictionary.Fields.ListID, "");
            }

            SelectMessage(message.MsgType);
        }

        public void AmendOrder(Fix.Order order)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.OrderCancelReplaceRequest.MsgType);

            if (message == null)
                return;

            UpdateMessage(message, order);

            message.Fields.Set(Fix.Dictionary.Fields.OrigClOrdID, order.ClOrdID);
            message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, Session.FormatClOrdId(Session.NextClOrdId));
            //
            // This field is not required in OrderList but it is in OrderCancelReplaceRequest so set it
            // to a default if it was not set.
            //
            // TODO

            //if (orderSingle.FindField(Fix.Field.FieldId.HandlInst) == null)
            //{
            //    message.SetField(Fix.Field.FieldId.HandlInst, "1");
            //}

            //
            // If we've previously amended an order from an OrderList in this session the
            // CancelReplaceRequest will have ListID set, we need to unset this if we are 
            // amending an OrderSingle.
            //
            if (order.ListID == null)
            {
                message.Fields.Set(Fix.Dictionary.Fields.ListID, "");
            }

            SelectMessage(message.MsgType);
        }

        public void OrderStatus(Fix.Order order)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.OrderStatusRequest.MsgType);

            if (message == null)
                return;

            UpdateMessage(message, order);

            SelectMessage(message.MsgType);
        }

        public void ListExecute(string listId)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.ListExecute.MsgType);

            if (message == null)
                return;

            message.Fields.Set(Fix.Dictionary.Fields.ListID, listId);

            SelectMessage(message.MsgType);
        }

        public void ListStatus(string listId)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.ListStatusRequest.MsgType);

            if (message == null)
                return;

            message.Fields.Set(Fix.Dictionary.Fields.ListID, listId);

            SelectMessage(message.MsgType);
        }

        public void ListCancel(string listId)
        {
            Fix.Message message = FindMessage(Fix.Dictionary.Messages.ListCancelRequest.MsgType);

            if (message == null)
                return;

            message.Fields.Set(Fix.Dictionary.Fields.ListID, listId);

            SelectMessage(message.MsgType);
        }
    }
}