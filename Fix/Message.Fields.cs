/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Message.Fields.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;

namespace Fix
{
    public class MissingFieldException : Exception
    {
        public MissingFieldException(Dictionary.Field field)
        : base($"Message does not contain a {field.Name} field")
        {
            Field = field;
        }
        Dictionary.Field Field { get; }
    }

    public partial class Message
    {
        public string MsgType
        {
            get
            {
                if (Definition != null)
                    return Definition.MsgType;
                Field field = Fields.Find(Dictionary.Fields.MsgType);
                if (field == null)
                    throw new MissingFieldException(Dictionary.Fields.MsgType);
                return field.Value;
            }
            set
            {
                Fields.Set(Dictionary.Fields.MsgType.Tag, value);
                Definition = null;
            }
        }

        public string BeginString
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.BeginString);
                if (field == null)
                    throw new MissingFieldException(Dictionary.Fields.BeginString);
                return field.Value;
            }
        }

        public int BodyLength
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.BodyLength);
                if (field == null)
                    throw new MissingFieldException(Dictionary.Fields.BodyLength);
                return Convert.ToInt32(field.Value);
            }
        }

        public int MsgSeqNum
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.MsgSeqNum);
                if (field == null)
                    throw new MissingFieldException(Dictionary.Fields.MsgSeqNum);
                return Convert.ToInt32(field.Value);
            }
        }

        public string CheckSum => Fields.Find(Dictionary.Fields.CheckSum)?.Value;

        public string SenderCompID
        {
            get
            {
                return Fields.Find(Dictionary.Fields.SenderCompID)?.Value;
            }
            set
            {
                Fields.Set(Dictionary.Fields.SenderCompID, value);
            }
        }

        public string TargetCompID
        {
            get
            {
                return Fields.Find(Dictionary.Fields.TargetCompID)?.Value;
            }
            set
            {
                Fields.Set(Dictionary.Fields.TargetCompID, value);
            }
        }

        public string SendingTime => Fields.Find(Dictionary.Fields.SendingTime)?.Value;

        public int BeginSeqNo
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.BeginSeqNo);
                if (field == null)
                    throw new MissingFieldException(Dictionary.Fields.BeginSeqNo);
                int value;
                if (!int.TryParse(field.Value, out value))
                    throw new Exception($"Message contains an invalid {field}");
                return value;
            }
        }

        public int EndSeqNo
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.EndSeqNo);
                if (field == null)
                    throw new MissingFieldException(Dictionary.Fields.EndSeqNo);
                int value;
                if (!int.TryParse(field.Value, out value))
                    throw new Exception($"Message contains an invalid {field}");
                return value;
            }
        }

        public bool GapFillFlag
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.GapFillFlag);
                if (field == null)
                    return false;
                return (bool)field;
            }
        }

        public bool ResetSeqNumFlag
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.ResetSeqNumFlag);
                if (field == null)
                    return false;
                return (bool)field;
            }
        }

        public int NewSeqNo
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.NewSeqNo);
                if (field == null)
                    throw new MissingFieldException(Dictionary.Fields.NewSeqNo);
                int value;
                if (!int.TryParse(field.Value, out value))
                    throw new Exception($"Message contains an invalid {field}");
                return value;
            }
        }

        public bool PossDupFlag
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.PossDupFlag);
                if (field == null)
                    return false;
                return (bool)field;
            }
        }

        public OrdStatus? OrdStatus
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.OrdStatus);
                if (field == null)
                    return null;
                return (OrdStatus)field;
            }
        }

        public SessionStatus? SessionStatus
        {
            get
            {
                Field field = Fields.Find(Dictionary.Fields.SessionStatus);
                if (field == null)
                    return null;
                return (SessionStatus)field;
            }
        }

        public string ClOrdID => Fields.Find(Dictionary.Fields.ClOrdID)?.Value;

        public string OrigClOrdID => Fields.Find(Dictionary.Fields.OrigClOrdID)?.Value;

    }
}
