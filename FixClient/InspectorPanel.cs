/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: InspectorPanel.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace FixClient
{
    class MessageProperties
    {
        Fix.Dictionary.Message _message;

        public MessageProperties(Fix.Dictionary.Message message)
        {
            _message = message;
        }

        const string Category = "Message";

        [Category(Category)]
        public string Name => _message.Name;

        [Category(Category)]
        public string MsgType => _message.MsgType;

        [Category(Category)]
        public string Pedigree => _message.Pedigree.ToString();
    }

    class FieldProperties
    {
        Fix.FieldDescription _description;

        public FieldProperties(Fix.FieldDescription description)
        {
            _description = description;
        }

        const string Category = "Field";

        [Category(Category)]
        public string Name => _description.Name;

        [Category(Category)]
        public int Tag => _description.Tag;

        [Category(Category)]
        public string DataType => _description.DataType;

        [Category(Category)]
        public int Depth => _description.Depth;

        [Category(Category)]
        public bool Required => _description.Required;

        [Category(Category)]
        public Fix.Dictionary.Pedigree Pedigree => _description.Pedigree;


    }

    class ValueProperties
    {
        const string Category = "Value";

        [Category(Category)]
        [ReadOnly(true)]
        public string? Type { get; set; }

        [Category(Category)]
        [ReadOnly(true)]
        public string? Added { get; set; }

        [Category(Category)]
        [ReadOnly(true)]
        public string? Value { get; set; }

        [Category(Category)]
        [ReadOnly(true)]
        public string? Name { get; set; }
    }

    public partial class InspectorPanel : Panel
    {
        readonly PropertyGrid _messagePropertyGrid;
        readonly TextBox _messageDescription;
        readonly PropertyGrid _fieldPropertyGrid;
        readonly TextBox _fieldDescription;
        readonly PropertyGrid _valuePropertyGrid;
        readonly TextBox _valueDescription;

        Fix.Dictionary.Message? _messageDefinition;
        Fix.FieldDescription? _fieldDefinition;

        public InspectorPanel()
        {
            InitializeComponent();

            _messagePropertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                HelpVisible = false,
                ToolbarVisible = false
            };

            _messageDescription = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true
            };

            _fieldPropertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                HelpVisible = false,
                ToolbarVisible = false
            };

            _fieldDescription = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true
            };

            _valuePropertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                HelpVisible = false,
                ToolbarVisible = false
            };

            _valueDescription = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true
            };

            var splitterOne = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal
            };

            var splitterTwo = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 500
            };

            var messageSplitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal
            };

            var fieldSplitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal
            };

            var valueSplitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal
            };

            var panel1 = new BorderHidingPanel(_messagePropertyGrid) { Dock = DockStyle.Fill };
            var panel2 = new BorderHidingPanel(_messageDescription) { Dock = DockStyle.Fill };

            var panel3 = new BorderHidingPanel(_fieldPropertyGrid) { Dock = DockStyle.Fill };
            var panel4 = new BorderHidingPanel(_fieldDescription) { Dock = DockStyle.Fill };

            var panel5 = new BorderHidingPanel(_valuePropertyGrid) { Dock = DockStyle.Fill };
            var panel6 = new BorderHidingPanel(_valueDescription) { Dock = DockStyle.Fill };

            messageSplitter.Panel1.Controls.Add(panel1);
            messageSplitter.Panel2.Controls.Add(panel2);

            fieldSplitter.Panel1.Controls.Add(panel3);
            fieldSplitter.Panel2.Controls.Add(panel4);

            valueSplitter.Panel1.Controls.Add(panel5);
            valueSplitter.Panel2.Controls.Add(panel6);

            splitterOne.Panel1.Controls.Add(messageSplitter);
            splitterOne.Panel2.Controls.Add(fieldSplitter);

            splitterTwo.Panel1.Controls.Add(splitterOne);
            splitterTwo.Panel2.Controls.Add(valueSplitter);

            Controls.Add(splitterTwo);
        }

        public Fix.Dictionary.Message? Message
        {
            set
            {
                _messageDefinition = value;
                _messagePropertyGrid.SelectedObject = value is null ? null : new MessageProperties(value);
                _messageDescription.Text = value?.Description;
            }
        }

        

        public Fix.Field? Field
        {
            set
            {
                _fieldDefinition = value?.Describe(_messageDefinition);

                _fieldPropertyGrid.SelectedObject = _fieldDefinition is null ? null : new FieldProperties(_fieldDefinition);
                _fieldDescription.Text = _fieldDefinition?.Description;

                /*
                if (definition?.EnumeratedType == null)
                {
                    _valuePropertyGrid.SelectedObject = null;
                    _valueDescription.Text = null;
                    return;
                }

                var valueProperties = new ValueProperties
                {
                    Type = definition.EnumeratedType.Name
                };

                try
                {
                    if (!string.IsNullOrEmpty(value.Value) && value.Value.Length == 1)
                    {
                        string name = definition.EnumeratedType.GetEnumName(Convert.ToChar(value.Value));

                        if (string.IsNullOrEmpty(name))
                            return;

                        MemberInfo[] info = definition.EnumeratedType.GetMember(name);

                        if (info.Length < 1)
                            return;

                        var attributes = info[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        _valueDescription.Text = ((DescriptionAttribute)attributes[0]).Description;

                        string added = null;
                        attributes = info[0].GetCustomAttributes(typeof(Fix.AddedAttribute), false);
                        if (attributes.Length > 0)
                        {
                            added = ((Fix.AddedAttribute)attributes[0]).Added;
                        }

                        valueProperties.Value = value.Value;
                        valueProperties.Name = name;
                        valueProperties.Added = added;
                    }
                }
                finally
                {
                    _valuePropertyGrid.SelectedObject = valueProperties;
                }
                */
            }
        }
    }
}
