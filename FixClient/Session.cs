/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Session.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FixClient
{
    public partial class Session : Fix.PersistentSession
    {
        const string CategoryInitiator = "Initiator Message Generation";
        const string CategoryAcceptor = "Acceptor Message Generation";

        #region Events

        public class CustomFieldEventArgs : EventArgs
        {
            public CustomFieldEventArgs(CustomField field)
            {
                Field = field;
            }
            public CustomField Field { get; }
        }

        public delegate void ResetDelegate(object sender, EventArgs e);
        public delegate void CustomFieldDelegate(object sender, CustomFieldEventArgs e);

        public event ResetDelegate SessionReset;
        public event ResetDelegate MessagesReset;
        public event CustomFieldDelegate CustomFieldAdded;

        protected void OnMessagesReset()
        {
            MessagesReset?.Invoke(this, null);
        }

        protected void OnSessionReset()
        {
            SessionReset?.Invoke(this, null);
        }

        protected void OnCustomFieldAdded(CustomField field)
        {
            CustomFieldAdded?.Invoke(this, new CustomFieldEventArgs(field));
        }

        #endregion

        public Session()
        {
            BindHost = string.Empty;
            Host = "127.0.0.1";
            Port = 9810;
            UpdateReadonlyAttributes();

            _messageFilters = new Dictionary<string, bool>();
            Reading = true;
            AutoWriteFilters = true;
            Reading = false;

            Messages.MessageAdded += (sender, ev) =>
            {
                OrderBook.Process(ev.Message);
            };

            Messages.Reset += (sender, ev) =>
            {
                OrderBook.Clear(_retain);
            };
        }

        public Session(Session session)
        : base(session)
        {
            Behaviour = session.Behaviour;
            BindHost = session.BindHost;
            BindPort = session.BindPort;
            Host = session.Host;
            Port = session.Port;
            NextClOrdId = session.NextClOrdId;
            AppendDateToClOrdID = session.AppendDateToClOrdID;
            NextListId = session.NextListId;
            NextAllocId = session.NextAllocId;
            NextOrderId = session.NextOrderId;
            NextExecId = session.NextExecId;
            AutoSetMsgSeqNum = session.AutoSetMsgSeqNum;
            AutoTotNoOrders = session.AutoTotNoOrders;
            AutoNoOrders = session.AutoNoOrders;
            AutoListId = session.AutoListId;
            AutoClOrdId = session.AutoClOrdId;
            AutoListSeqNo = session.AutoListSeqNo;
            AutoTransactTime = session.AutoTransactTime;
            AutoAllocId = session.AutoAllocId;
            AutoScrollMessages = session.AutoScrollMessages;
            OrderBook = new Fix.OrderBook();
            Reading = true; // Disable the filter write if AutoWrite == true
            AutoWriteFilters = session.AutoWriteFilters;
            Reading = false;
            PasteDefineCustomFields = session.PasteDefineCustomFields;
            PasteFilterEmptyFields = session.PasteFilterEmptyFields;
            PasteSmart = session.PasteSmart;
            PasteResetExisting = session.PasteResetExisting;
            PasteProcessRepeatingGroups = session.PasteProcessRepeatingGroups;
        }

        [Category(CategoryNetwork)]
        [JsonProperty]
        public Fix.Behaviour Behaviour { get; set; }

        [Category(CategoryNetwork)]
        [DisplayName("Bind Host")]
        [ReadOnly(false)]
        [JsonProperty]
        public string BindHost { get; set; }

        [Category(CategoryNetwork)]
        [DisplayName("Bind Port")]
        [ReadOnly(false)]
        [JsonProperty]
        public int BindPort { get; set; }

        [Category(CategoryNetwork)]
        [JsonProperty]
        public string Host { get; set; }

        [Category(CategoryNetwork)]
        [JsonProperty]
        public int Port { get; set; }

        [Category(CategoryInitiator)]
        [DisplayName("Next ClOrdID")]
        [ReadOnly(false)]
        [JsonProperty]
        public int NextClOrdId { get; set; } = 1;

        [Category(CategoryInitiator)]
        [DisplayName("Append Date to ClOrdID")]
        [ReadOnly(false)]
        [JsonProperty]
        public bool AppendDateToClOrdID { get; set; }

        public string FormatClOrdId(int clOrdId)
        {
            if (AppendDateToClOrdID)
            {
                return $"{clOrdId}-" + DateTime.Today.Date.ToString("yyyyMMdd");
            }

            return $"{clOrdId}";
        }

        [Category(CategoryInitiator)]
        [DisplayName("Next ListID")]
        [ReadOnly(false)]
        [JsonProperty]
        public int NextListId { get; set; } = 1;

        [Category(CategoryInitiator)]
        [DisplayName("Next AllocID")]
        [ReadOnly(false)]
        [JsonProperty]
        public int NextAllocId { get; set; } = 1;

        [Category(CategoryAcceptor)]
        [DisplayName("Next OrderID")]
        [ReadOnly(false)]
        [JsonProperty]
        public int NextOrderId { get; set; } = 1;

        [Category(CategoryAcceptor)]
        [DisplayName("Next ExecID")]
        [ReadOnly(false)]
        [JsonProperty]
        public int NextExecId { get; set; } = 1;

        [Browsable(false)]
        [JsonProperty]
        public bool AutoSetMsgSeqNum { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool AutoTotNoOrders { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool AutoNoOrders { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool AutoListId { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool AutoClOrdId { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool AutoListSeqNo { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool AutoTransactTime { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool AutoAllocId { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool AutoScrollMessages { get; set; } = true;

        [Browsable(false)]
        public Fix.OrderBook OrderBook { get; } = new Fix.OrderBook();

        #region Options for the paste message dialog

        [Browsable(false)]
        [JsonProperty]
        public bool PasteDefineCustomFields { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool PasteFilterEmptyFields { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool PasteSmart { get; set; } = true;

        [Browsable(false)]
        [JsonProperty]
        public bool PasteProcessRepeatingGroups { get; set; } = false;

        [Browsable(false)]
        [JsonProperty]
        public bool PasteResetExisting { get; set; } = true;


        #endregion

        public override void ResetMessages()
        {
            base.ResetMessages();
            OnMessagesReset();
        }

        Fix.OrderBook.Retain _retain;

        public void Reset(bool resetGeneratedIds, Fix.OrderBook.Retain retain)
        {
            _retain = retain;

            if (resetGeneratedIds)
            {
                NextAllocId = 1;
                NextClOrdId = 1;
                NextExecId = 1;
                NextListId = 1;
                NextOrderId = 1;
            }

            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            ResetMessages();
            OnSessionReset();
        }

        public override void UpdateReadonlyAttributes()
        {
            SetReadOnly("DefaultApplVerId", BeginString.BeginString != Fix.Dictionary.Versions.FIXT_1_1.BeginString);
            SetReadOnly("BindHost", Behaviour == Fix.Behaviour.Acceptor);
            SetReadOnly("BindPort", Behaviour == Fix.Behaviour.Acceptor);
            SetReadOnly("NextClOrdId", OrderBehaviour == Fix.Behaviour.Acceptor);
            SetReadOnly("NextListId", OrderBehaviour == Fix.Behaviour.Acceptor);
            SetReadOnly("NextAllocId", OrderBehaviour == Fix.Behaviour.Acceptor);
            SetReadOnly("NextOrderId", OrderBehaviour == Fix.Behaviour.Initiator);
            SetReadOnly("NextExecId", OrderBehaviour == Fix.Behaviour.Initiator);
        }

        #region Logging

        public void LogInformation(string format, params object[] args)
        {
            OnInformation(string.Format(format, args));
        }

        public void LogWarning(string message)
        {
            OnWarning(message);
        }

        public void LogError(string message)
        {
            OnError(message);
        }

        #endregion

        #region Custom Fields

        readonly Dictionary<int, CustomField> _customFields = new();

        [Browsable(false)]
        public Dictionary<int, CustomField> CustomFields
        {
            get { return _customFields; }
        }

        public void AddCustomField(CustomField field)
        {
            if (_customFields.ContainsKey(field.Tag))
            {
                throw new ArgumentException(string.Format("Session already contains a custom field with Tag = {0}", field.Tag));
            }
            _customFields[field.Tag] = field;
            OnCustomFieldAdded(field);
        }

        public void RemoveCustomField(CustomField field)
        {
            if (!_customFields.ContainsKey(field.Tag))
            {
                throw new ArgumentException(string.Format("Session does not contain a custom field with Tag = {0}", field.Tag));
            }
            _customFields.Remove(field.Tag);
        }

        #endregion

        #region Templates

        readonly Dictionary<string, Fix.Message> _messageTemplates = new();

        public override Fix.Message MessageForTemplate(Fix.Dictionary.Message templateMessage)
        {
            if (!_messageTemplates.TryGetValue(templateMessage.MsgType, out Fix.Message message))
            {
                message = base.MessageForTemplate(templateMessage);

                foreach (var templateField in templateMessage.Fields)
                {
                    message.Fields.Set(new Fix.Field(templateField));
                }

                message.MsgType = templateMessage.MsgType;
                message.Definition = templateMessage;
                _messageTemplates[message.MsgType] = message;
            }

            return message;
        }

        public void ResetTemplateMessage(string msgType)
        {
            if (_messageTemplates.TryGetValue(msgType, out Fix.Message template))
            {
                Fix.Dictionary.Message definition = template.Definition;

                template.Fields.Clear();

                foreach (var templateField in definition.Fields)
                {
                    template.Fields.Set(new Fix.Field(templateField));
                }

                Fix.Message exemplar = base.MessageForTemplate(definition);

                foreach (Fix.Field field in exemplar.Fields)
                {
                    template.Fields.Set(new Fix.Field(field.Tag, field.Value));
                }

                template.MsgType = msgType;
                template.Definition = definition;
            }
        }

        public void ResetMessageTemplates()
        {
            Dictionary<string, Fix.Message> existing = _messageTemplates.ToDictionary(item => item.Key, item => item.Value);
            _messageTemplates.Clear();
            foreach (var item in existing)
            {
                Fix.Message previous = item.Value;
                if (previous.Definition == null)
                    continue;
                Fix.Message template = MessageForTemplate(previous.Definition);
                foreach (Fix.Field field in previous.Fields)
                {
                    if (!string.IsNullOrEmpty(field.Value))
                    {
                        template.Fields.Set(field);
                    }
                }
            }
        }

        #endregion

        #region Filters

        readonly Dictionary<string, bool> _messageFilters = new();
        readonly Dictionary<string, Dictionary<int, bool>> _fieldFilters = new();

        public delegate void MessageFilterDelegate(object sender, EventArgs e);
        public delegate void FieldFilterDelegate(object sender, EventArgs e);

        public event MessageFilterDelegate MessageFilterChanged;
        public event FieldFilterDelegate FieldFilterChanged;

        protected void OnMessageFilterChanged()
        {
            MessageFilterChanged?.Invoke(this, null);
            WriteFilters();
        }

        protected void OnFieldFilterChanged()
        {
            if (!AutoWriteFilters)
                return;

            FieldFilterChanged?.Invoke(this, null);

            WriteFilters();
        }

        public void MessageVisible(string msgType, bool visible, bool raiseEvent = true)
        {
            _messageFilters[msgType] = visible;
            if (raiseEvent)
            {
                OnMessageFilterChanged();
            }
        }

        public bool MessageVisible(string msgType)
        {
            if (_messageFilters.TryGetValue(msgType, out bool visible))
            {
                return visible;
            }
            _messageFilters[msgType] = true;
            return true;
        }

        public Dictionary<int, bool> FieldFilters(string msgType)
        {
            if (!_fieldFilters.TryGetValue(msgType, out Dictionary<int, bool> filters))
            {
                filters = new Dictionary<int, bool>();
                _fieldFilters[msgType] = filters;
            }
            return filters;
        }

        public void FieldVisible(string msgType, int tag, bool visible)
        {
            Dictionary<int, bool> filters;
            if (!_fieldFilters.TryGetValue(msgType, out _))
            {
                filters = new Dictionary<int, bool>();
                _fieldFilters[msgType] = filters;
            }
            _fieldFilters[msgType][tag] = visible;
            OnFieldFilterChanged();
        }

        public bool FieldVisible(string msgType, int tag)
        {
            if (!_fieldFilters.TryGetValue(msgType, out Dictionary<int, bool> filters))
            {
                return true;
            }

            if (!filters.TryGetValue(tag, out bool visible))
            {
                return true;
            }

            return visible;
        }

        public string FieldRowFilter(string msgType, string searchString = null)
        {
            var expression = new StringBuilder(string.Format("{0} NOT IN (", FieldDataTable.ColumnTag));

            bool items = false;
            foreach (KeyValuePair<int, bool> filter in FieldFilters(msgType))
            {
                if (filter.Value)
                    continue;
                if (items)
                    expression.Append(',');
                items = true;
                expression.Append(string.Format("{0}", Convert.ToInt32(filter.Key)));
            }

            if (!items)
                return searchString;

            expression.Append(')');

            string result = searchString ?? expression.ToString();

            if (!string.IsNullOrEmpty(searchString))
            {
                result = string.Format("({0}) AND ({1})", searchString, expression);
            }

            return result;
        }

        public string MessageRowFilter(string searchString = null)
        {
            var expression = new StringBuilder();

            bool items = false;
            foreach (KeyValuePair<string, bool> filter in _messageFilters)
            {
                if (filter.Value)
                    continue;
                if (expression.Length > 0)
                    expression.Append(" AND ");
                items = true;
                expression.Append(string.Format("{0} <> '{1}'", MessageTypeDataTable.ColumnMsgType, filter.Key));
            }

            if (!items)
                return searchString;

            string result = searchString ?? expression.ToString();

            if (!string.IsNullOrEmpty(searchString))
            {
                result = string.Format("({0}) AND ({1})", searchString, expression);
            }

            return result;
        }

        #endregion

        #region PersistentSession

        string TemplatesFileName => GetFileNamePrefix(FileName) + ".templates";

        string FiltersFileName => GetFileNamePrefix(FileName) + ".filters";

        string CustomFieldsFileName => GetFileNamePrefix(FileName) + ".custom";

        public override void Read()
        {
            base.Read();
            Reading = true;
            ReadTemplates();
            ReadFilters();
            ReadCustomFields();
            Reading = false;
        }

        void ReadCustomFields()
        {
            int errors = 0;
            try
            {
                if (!File.Exists(CustomFieldsFileName))
                    return;

                using var stream = new FileStream(CustomFieldsFileName, FileMode.Open);
                using var sr = new StreamReader(stream);
                using var reader = new JsonTextReader(sr);
                JObject filters = JObject.Load(reader);
                JToken fields = filters["Fields"];
                foreach (var field in fields)
                {
                    try
                    {
                        AddCustomField(new CustomField
                        {

                            Tag = Convert.ToInt32(field["Tag"]),
                            Name = field["Name"].ToString()
                        });
                    }
                    catch (Exception ex)
                    {
                        ++errors;
                        OnError(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ++errors;
                OnError(ex.Message);
            }

            if (errors > 0)
            {
                OnError("{0} error{1} occurred reading the custom field definitions - custom fields may be missing or incorrect",
                        errors, errors == 1 ? "" : "s");
            }
        }

        public void WriteCustomFields()
        {
            if (Reading)
                return;

            using FileStream stream = new(CustomFieldsFileName, FileMode.Create);
            using JsonWriter writer = new JsonTextWriter(new StreamWriter(stream));
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("Fields");
            writer.WriteStartArray();
            foreach (var field in CustomFields.Values)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Tag");
                writer.WriteValue(field.Tag);
                writer.WritePropertyName("Name");
                writer.WriteValue(field.Name);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        void ReadFilters()
        {
            try
            {
                if (!File.Exists(FiltersFileName))
                    return;

                using var stream = new FileStream(FiltersFileName, FileMode.Open);
                using var sr = new StreamReader(stream);
                using var reader = new JsonTextReader(sr);
                JObject filters = JObject.Load(reader);
                JToken messages = filters["Messages"];
                foreach (var item in messages)
                {
                    foreach (Fix.Dictionary.Message message in Version.Messages)
                    {
                        if (message.Name == item.ToString())
                        {
                            MessageVisible(message.MsgType, false);
                            break;
                        }
                    }
                }
                JToken fields = filters["Fields"];
                foreach (var entry in fields)
                {
                    var property = (JProperty)entry.First;
                    Fix.Dictionary.Message message = Version.Messages.FirstOrDefault(item => item.Name == property.Name);
                    if (message == null)
                        continue;
                    foreach (string fieldEntry in (JArray)property.Value)
                    {
                        Fix.Dictionary.Field field = message.Fields.FirstOrDefault(item => item.Name == fieldEntry);
                        if (field == null)
                            continue;
                        FieldVisible(message.MsgType, field.Tag, false);
                    }
                }
            }
            catch (Exception ex)
            {
                OnError("An error occurred reading the filter definitions - filters may be missing or incorrect: {0}", ex.Message);
            }
        }

        bool _autoWriteFilters;

        [Browsable(false)]
        public bool AutoWriteFilters
        {
            get { return _autoWriteFilters; }
            set
            {
                _autoWriteFilters = value;
                if (!Reading)
                {
                    OnFieldFilterChanged();
                }
            }
        }

        public void WriteFilters()
        {
            if (Reading || !AutoWriteFilters)
                return;

            using FileStream stream = new(FiltersFileName, FileMode.Create);
            using JsonWriter writer = new JsonTextWriter(new StreamWriter(stream));
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("Messages");
            writer.WriteStartArray();
            foreach (var filter in _messageFilters)
            {
                if (!filter.Value)
                {
                    writer.WriteValue(Version.Messages[filter.Key].Name);
                }
            }
            writer.WriteEndArray();
            writer.WritePropertyName("Fields");
            writer.WriteStartArray();
            foreach (var filter in _fieldFilters)
            {
                if (filter.Value.Count == 0 || filter.Value.All(field => field.Value))
                    continue;
                writer.WriteStartObject();
                writer.WritePropertyName(Version.Messages[filter.Key].Name);
                writer.WriteStartArray();
                foreach (var field in filter.Value)
                {
                    if (field.Value)
                        continue;
                    if (Version.Fields.TryGetValue(field.Key.ToString(), out var fieldDefinition))
                    {
                        writer.WriteValue(Version.Fields[field.Key.ToString()].Name);
                    }
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        void ReadTemplates()
        {
            int errors = 0;
            try
            {
                _messageTemplates.Clear();
                using Stream stream = new FileStream(TemplatesFileName, FileMode.OpenOrCreate, FileAccess.Read);
                using Fix.Reader reader = new(stream) { ValidateDataFields = false };
                for (; ; )
                {
                    try
                    {
                        Fix.Message template = reader.ReadLine();
                        if (template == null)
                            break;
                        template.Definition = Version.Messages[template.MsgType];
                        _messageTemplates[template.MsgType] = template;
                    }
                    catch (Exception ex)
                    {
                        ++errors;
                        OnError(ex.Message);
                        reader.DiscardLine();
                    }
                }
            }
            catch (Exception ex)
            {
                ++errors;
                OnError(ex.Message);
            }

            if (errors > 0)
            {
                OnError("{0} error{1} occurred reading the message templates - messages may be missing or incorrect",
                        errors, errors == 1 ? "" : "s");
            }
        }

        public void WriteTemplates()
        {
            using Fix.Writer writer = new(new FileStream(TemplatesFileName, FileMode.Create), leaveOpen: false);
            foreach (Fix.Message message in _messageTemplates.Values)
            {
                writer.WriteLine(message);
            }
        }

        #endregion

        #region ICloneable

        public override object Clone()
        {
            return new Session(this);
        }

        public void CopyPropertiesFrom(Session source)
        {
            Type type = GetType();

            foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!info.CanWrite || !info.CanRead)
                    continue;

                if (info.GetCustomAttributes(typeof(JsonPropertyAttribute), false).Length == 0)
                    continue;

                info.SetValue(this, info.GetValue(source));
            }
        }

        #endregion
    }
}
