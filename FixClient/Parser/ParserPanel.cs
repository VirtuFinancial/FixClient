using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FixClient
{
    public partial class ParserPanel : FixClientPanel
    {
        readonly ToolStripCheckBox _showAdminMessageCheckBox;

        readonly ToolStripDropDownButton _statusButton;
        readonly ToolStripMenuItem _statusNoneMenuItem;
        readonly ToolStripMenuItem _statusInfoMenuItem;
        readonly ToolStripMenuItem _statusWarnMenuItem;
        readonly ToolStripMenuItem _statusErrorMenuItem;

        readonly RichTextBox _textbox;

        public ParserPanel()
        {
            InitializeComponent();

            #region ToolStrip
            var loadButton = new ToolStripButton(Properties.Resources.Open)
            {
                ImageTransparentColor = Color.Magenta,
                ToolTipText = "Load a Glue/Gate/FIX Router/Desk Server log file"
            };
            loadButton.Click += async (sender, ev) => await LoadClientMessagesButtonClick(sender, ev);

            _showAdminMessageCheckBox = new ToolStripCheckBox();
            _showAdminMessageCheckBox.CheckChanged += ShowAdminMessageCheckBoxCheckChanged;

            _statusNoneMenuItem = new ToolStripMenuItem { Text = "None", Checked = true, CheckOnClick = true };
            //_statusNoneMenuItem.Click += StatusMenuItemClick;

            _statusInfoMenuItem = new ToolStripMenuItem { Text = "Info", Checked = true, CheckOnClick = true };
            //_statusInfoMenuItem.Click += StatusMenuItemClick;

            _statusWarnMenuItem = new ToolStripMenuItem { Text = "Warn", Checked = true, CheckOnClick = true };
            //_statusWarnMenuItem.Click += StatusMenuItemClick;

            _statusErrorMenuItem = new ToolStripMenuItem { Text = "Error", Checked = true, CheckOnClick = true };
            //_statusErrorMenuItem.Click += StatusMenuItemClick;

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

            _textbox = new()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font(FontFamily.GenericMonospace, Font.Size)
            };

            TopToolStripPanel.Join(toolStrip);
            ContentPanel.Controls.Add(_textbox);

            ShowAdminMessageCheckBoxCheckChanged(this, EventArgs.Empty);
        }

        void ShowAdminMessageCheckBoxCheckChanged(object? sender, EventArgs e)
        {
            //UpdateMessageFilter();
        }

        async Task LoadClientMessagesButtonClick(object? sender, EventArgs e)
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
                _textbox.Text = string.Empty;

                var orderBook = new Fix.OrderBook();

                var url = new Uri($"file://{Path.GetFullPath(dlg.FileName)}");

                await foreach (var message in Fix.Parser.Parse(url))
                {
                    //if (message.Administrative)
                    //{
                    //    continue;
                    //}

                    _textbox.AppendText(message.PrettyPrint() + "\r\n");

                    if (orderBook.Process(message) == Fix.OrderBookMessageEffect.Modified)
                    {
                        var report = new Fix.OrderReport(orderBook);
                        using var stream = new MemoryStream();
                        report.Print(stream);
                        stream.Flush();
                        _textbox.AppendText("\r\n" + Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position));
                    }
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
                Cursor.Current = original;
            }
        }
    }
}