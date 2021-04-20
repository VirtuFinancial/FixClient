/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Message.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Fix.Dictionary;

namespace Fix
{
    public enum MessageStatus
    {
        None,
        Info,
        Warn,
        Error
    }

    public partial class Message : ICloneable
    {
        public Message()
        {
            //
            // Insert place holders for the standard header fields. Order is important so insert them
            // now and they can be overridden as required later on.
            //
            Fields = new FieldCollection
            {
                new Field(FIX_5_0SP2.Fields.BeginString),
                new Field(FIX_5_0SP2.Fields.BodyLength),
                new Field(FIX_5_0SP2.Fields.MsgType),
                new Field(FIX_5_0SP2.Fields.SenderCompID),
                new Field(FIX_5_0SP2.Fields.TargetCompID),
                new Field(FIX_5_0SP2.Fields.MsgSeqNum),
                new Field(FIX_5_0SP2.Fields.SendingTime)
            };
        }

        public Message(Dictionary.Message definition)
        {
            Fields = new FieldCollection(definition);
            Definition = definition;
        }

        public Message(string data)
        : this(Encoding.ASCII.GetBytes(data))
        {
        }

        public Message(byte[] data)
        {
            Fields = new FieldCollection();
            using MemoryStream stream = new(data);
            using Reader reader = new(stream);
            reader.Read(this);
        }

        public Message(string[,] data)
        {
            Fields = new FieldCollection(data);
        }

        public Message(IEnumerable<Field> source)
        : this()
        {
            foreach (Field field in source)
            {
                Fields.Set(field);
            }
        }

        public Message Trim()
        {
            var message = new Message();
            message.Fields.Clear();

            foreach (Field field in Fields)
            {
                if (field.Value != null && field.Value.Trim().Length > 0)
                    message.Fields.Add(field);
            }

            return message;
        }

        public static Message Parse(string text)
        {
            var parser = new Parser { Strict = false };
            using (MemoryStream stream = new(Encoding.ASCII.GetBytes(text)))
            {
                MessageCollection messages = parser.Parse(stream);
                if (messages != null && messages.Count > 0)
                    return messages[0];
            }
            return null;
        }

        public MessageStatus Status { get; set; }
        public string StatusMessage { get; set; }

        public Dictionary.Message Definition { get; set; }

        public FieldCollection Fields { get; }

        // TODO - This should probably be in the definition and it should come from the repo data, is it actually in the repo?
        public bool Administrative
        {
            get
            {
                if (MsgType == FIX_5_0SP2.Messages.Heartbeat.MsgType ||
                    MsgType == FIX_5_0SP2.Messages.Logon.MsgType ||
                    MsgType == FIX_5_0SP2.Messages.Logout.MsgType ||
                    MsgType == FIX_5_0SP2.Messages.Reject.MsgType ||
                    MsgType == FIX_5_0SP2.Messages.ResendRequest.MsgType ||
                    MsgType == FIX_5_0SP2.Messages.SequenceReset.MsgType ||
                    MsgType == FIX_5_0SP2.Messages.TestRequest.MsgType)
                {
                    return true;
                }

                return false;
            }
        }

        public string ComputeCheckSum()
        {
            return ComputeCheckSum(this);
        }

        public static string ComputeCheckSum(Message message)
        {
            //
            // The checksum is computed by summing all bytes with the message (except those of the
            // checksum field '10=nnn\001' itself) modulo 256.
            //
            int value = message.Fields.Where(field => field.Tag != FIX_5_0SP2.Fields.CheckSum.Tag).Sum(field => field.ComputeCheckSum()) % 256;
            return value.ToString("D3");
        }

        public string ComputeBodyLength()
        {
            return ComputeBodyLength(this);
        }

        public static string ComputeBodyLength(Message message)
        {
            int bodyLength = 0;
            bool passedBodyLength = false;

            foreach (Field field in message.Fields)
            {
                if (field.Tag == FIX_5_0SP2.Fields.CheckSum.Tag)
                    continue;

                if (field.Tag == FIX_5_0SP2.Fields.BodyLength.Tag)
                {
                    passedBodyLength = true;
                    continue;
                }

                if (!passedBodyLength)
                    continue;

                bodyLength += field.ComputeBodyLength();
            }

            return bodyLength.ToString();
        }

        public bool Incoming { get; set; }

        #region Object

        public override string ToString()
        {
            var builder = new StringBuilder(Incoming ? "Incoming" : "Outgoing");

            builder.Append("\r\n{\r\n");

            //
            // Determine the width of the widest field name so we can format the log message nicely.
            //
            int widestName = 0;

            foreach (Field field in Fields)
            {
                if (field.Definition == null)
                {
                    field.Definition = FIX_5_0SP2.Fields[field.Tag - 1];
                    if (field.Definition == null)
                    {
                        // Dictionary.Fields is the latest version so try an older version to catch deprecated fields. 
                        // TODO - Walk the versions in reverse order until we find a match
                        field.Definition = Dictionary.FIX_4_2.Fields[field.Tag - 1];
                        if (field.Definition == null)
                        {
                            continue;
                        }
                    }
                }
                int length = field.Definition.Name.Length;
                if (length > widestName)
                    widestName = length;
            }

            foreach (Field field in Fields)
            {
                string name = field.Definition == null ? string.Empty : field.Definition.Name;
                string description = field.Tag == FIX_5_0SP2.Fields.MsgType.Tag ? Definition?.Name : field.ValueDescription;
                builder.AppendFormat("    {0} {1} - {2}{3}\r\n",
                                     name.PadLeft(widestName),
                                     string.Format("({0})", field.Tag).PadLeft(6),
                                     field.Value,
                                     string.IsNullOrEmpty(description) ? string.Empty : " - " + description);
            }

            builder.Append('}');

            return builder.ToString();
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            var clone = new Message();
            clone.Fields.Clear();
            foreach (Field field in Fields)
            {
                clone.Fields.Add((Field)field.Clone());
            }
            clone.Definition = Definition;
            clone.Incoming = Incoming;
            clone.Status = Status;
            clone.StatusMessage = StatusMessage;
            return clone;
        }

        #endregion
    }
}
