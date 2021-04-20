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
using static Fix.Dictionary;

namespace Fix
{
    public class MissingFieldException : Exception
    {
        public MissingFieldException(Dictionary.VersionField field)
        : base($"Message does not contain a {field.Name} field")
        {
        }
    }

    public partial class Message
    {
        public string MsgType
        {
            get
            {
                if (Definition != null)
                    return Definition.MsgType;
                Field field = Fields.Find(FIX_5_0SP2.Fields.MsgType);
                if (field == null)
                    throw new MissingFieldException(FIX_5_0SP2.Fields.MsgType);
                return field.Value;
            }
            set
            {
                Fields.Set(FIX_5_0SP2.Fields.MsgType.Tag, value);
                Definition = null;
            }
        }

        public string BeginString
        {
            get
            {
                Field field = Fields.Find(FIX_5_0SP2.Fields.BeginString);
                if (field == null)
                    throw new MissingFieldException(FIX_5_0SP2.Fields.BeginString);
                return field.Value;
            }
        }

        public int BodyLength
        {
            get
            {
                Field field = Fields.Find(FIX_5_0SP2.Fields.BodyLength);
                if (field == null)
                    throw new MissingFieldException(FIX_5_0SP2.Fields.BodyLength);
                return Convert.ToInt32(field.Value);
            }
        }

        public int MsgSeqNum
        {
            get
            {
                Field field = Fields.Find(FIX_5_0SP2.Fields.MsgSeqNum);
                if (field == null)
                    throw new MissingFieldException(FIX_5_0SP2.Fields.MsgSeqNum);
                return Convert.ToInt32(field.Value);
            }
        }

        public string CheckSum => Fields.Find(FIX_5_0SP2.Fields.CheckSum)?.Value;

        public string SenderCompID
        {
            get
            {
                return Fields.Find(FIX_5_0SP2.Fields.SenderCompID)?.Value;
            }
            set
            {
                Fields.Set(FIX_5_0SP2.Fields.SenderCompID, value);
            }
        }

        public string TargetCompID
        {
            get
            {
                return Fields.Find(FIX_5_0SP2.Fields.TargetCompID)?.Value;
            }
            set
            {
                Fields.Set(FIX_5_0SP2.Fields.TargetCompID, value);
            }
        }

        public string SendingTime => Fields.Find(FIX_5_0SP2.Fields.SendingTime)?.Value;

        public int BeginSeqNo
        {
            get
            {
                Field field = Fields.Find(FIX_5_0SP2.Fields.BeginSeqNo);
                if (field == null)
                    throw new MissingFieldException(FIX_5_0SP2.Fields.BeginSeqNo);
                if (!int.TryParse(field.Value, out int value))
                    throw new Exception($"Message contains an invalid {field}");
                return value;
            }
        }

        public int EndSeqNo
        {
            get
            {
                Field field = Fields.Find(FIX_5_0SP2.Fields.EndSeqNo);
                if (field == null)
                    throw new MissingFieldException(FIX_5_0SP2.Fields.EndSeqNo);
                if (!int.TryParse(field.Value, out int value))
                    throw new Exception($"Message contains an invalid {field}");
                return value;
            }
        }

        public bool GapFillFlag
        {
            get
            {
                Field field = Fields.Find(FIX_5_0SP2.Fields.GapFillFlag);
                if (field == null)
                    return false;
                return (bool)field;
            }
        }

        public bool ResetSeqNumFlag
        {
            get
            {
                Field field = Fields.Find(FIX_5_0SP2.Fields.ResetSeqNumFlag);
                if (field == null)
                    return false;
                return (bool)field;
            }
        }

        public int NewSeqNo
        {
            get
            {
                Field field = Fields.Find(FIX_5_0SP2.Fields.NewSeqNo);
                if (field == null)
                    throw new MissingFieldException(FIX_5_0SP2.Fields.NewSeqNo);
                if (!int.TryParse(field.Value, out int value))
                    throw new Exception($"Message contains an invalid {field}");
                return value;
            }
        }

        public bool PossDupFlag
        {
            get
            {
                Field field = Fields.Find(FIX_5_0SP2.Fields.PossDupFlag);
                if (field == null)
                    return false;
                return (bool)field;
            }
        }

        public Field? OrdStatus => Fields.Find(FIX_5_0SP2.Fields.OrdStatus);

        public Field? SessionStatus => Fields.Find(FIX_5_0SP2.Fields.SessionStatus);

        public string ClOrdID => Fields.Find(FIX_5_0SP2.Fields.ClOrdID)?.Value;

        public string OrigClOrdID => Fields.Find(FIX_5_0SP2.Fields.OrigClOrdID)?.Value;

    }
}
