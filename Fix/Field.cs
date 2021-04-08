/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Field.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fix
{
    public class Field : ICloneable
    {
        public static readonly byte Separator = Convert.ToByte('\x01');
        public static readonly byte ValueSeparator = Convert.ToByte('=');

        public const string DateFormat = "yyyyMMdd";
        public const string TimestampFormatShort = "yyyyMMdd-HH:mm:ss";
        public const string TimestampFormatLong = "yyyyMMdd-HH:mm:ss.fff";

        public static string TimeString(bool fractionalSeconds = false)
        {
            return DateTime.UtcNow.ToString(fractionalSeconds ? TimestampFormatLong : TimestampFormatShort);
        }

        public Field(int tag, string value)
        {
            Tag = tag;
            Value = value;
        }

        public Field(int tag, int value)
        {
            Tag = tag;
            Value = value.ToString();
        }

        public Field(int tag, long value)
        {
            Tag = tag;
            Value = value.ToString();
        }

        public Field(int tag, decimal value)
        {
            Tag = tag;
            Value = value.ToString();
        }

        public Field(int tag, bool value)
        {
            Tag = tag;
            Value = value ? "Y" : "N";
        }

        public Field(string tag, string value)
        {
            int intTag;

            if (!int.TryParse(tag, out intTag))
            {
                throw new ArgumentException($"Invalid FIX tag '{tag}' is not an integer");
            }

            Tag = intTag;
            Value = value;
        }

        public Field(Dictionary.Field definition)
        {
            Definition = definition;
            Tag = definition.Tag;
        }

        public Field(Dictionary.Field definition, string value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, bool value)
            : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, int value)
        : this(definition.Tag, value.ToString())
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, decimal value)
        : this(definition.Tag, value.ToString())
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, Side value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, OrdType value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, SecurityIDSource value)
            : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, TimeInForce value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, OrdStatus value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, ExecType value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, HandlInst value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, TradeHandlingInstr value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, TradeReportTransType value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, TradeReportType value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, TradeReportRejectReason value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, NoSides value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, PartyIDSource value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, PartyRole value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, ClearingInstruction value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, OrderCategory value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, ExchangeTradeType value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, CxlRejResponseTo value)
            : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, SessionStatus value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, Dictionary.FIX_4_2.ExecType value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, Dictionary.FIX_5_0.ExecType value)
            : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(Dictionary.Field definition, Dictionary.FIX_4_2.ExecTransType value)
        : this(definition.Tag, value)
        {
            Definition = definition;
        }

        public Field(int tag, EncryptMethod value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, ExecType value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, SessionStatus value)
        : this(tag, ((int)value).ToString())
        {
        }

        public Field(int tag, TrdType value)
        : this(tag, ((int)value).ToString())
        {
        }

        public Field(int tag, TrdRptStatus value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, TradeHandlingInstr value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, TradeReportTransType value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, TradeReportType value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, TradeReportRejectReason value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, NoSides value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, PartyIDSource value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, PartyRole value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, ClearingInstruction value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, OrderCategory value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, ExchangeTradeType value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, Dictionary.FIX_4_2.ExecType value)
            : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, Dictionary.FIX_5_0.ExecType value)
            : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, Dictionary.FIX_4_0.ExecTransType value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, Dictionary.FIX_4_0.CxlType value)
            : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, Side value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, SecurityIDSource value)
            : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, OrdStatus value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, EmailType value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, CxlRejResponseTo value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, OrdType value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, TimeInForce value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, HandlInst value)
        : this(tag, ((char)value).ToString())
        {
        }

        public Field(int tag, Dictionary.FIX_4_2.ExecTransType value)
            : this(tag, ((char)value).ToString())
        {
        }

        public int Tag { get; }
        public string Value { get; set; }
        public Dictionary.Field Definition { get; set; }
        public bool Data { get; set; }

        public static explicit operator bool(Field field)
        {
            if (field.Value == "N" || field.Value == "n" || field.Value == "false" || field.Value == "0")
                return false;
            if (field.Value == "Y" || field.Value == "y" || field.Value == "true" || field.Value == "1")
                return true;
            throw new Exception($"Field does not contain a valid boolean value {field}");
        }

        public static explicit operator long?(Field field)
        {
            long result;
            if (field == null || !long.TryParse(field.Value, out result))
                return null;
            return result;
        }

        public static explicit operator string(Field field)
        {
            return field?.Value;
        }

        public static explicit operator decimal?(Field field)
        {
            decimal result;
            if (field == null || !decimal.TryParse(field.Value, out result))
                return null;
            return result;
        }

        public static explicit operator DateTime?(Field field)
        {
            DateTime result;

            string[] formats = { TimestampFormatLong, TimestampFormatShort, DateFormat };

            if (!DateTime.TryParseExact(field.Value,
                                        formats,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out result))
            {
                return null;
            }

            return result;
        }

        public static explicit operator Side?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(Convert.ToChar(field.Value));
            if (!Enum.IsDefined(typeof(Fix.Side), value))
                throw new ApplicationException($"{value} is not a valid value for Side");
            return (Fix.Side)Enum.ToObject(typeof(Fix.Side), value);
        }

        public static explicit operator OrdStatus?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(Convert.ToChar(field.Value));
            if (!Enum.IsDefined(typeof(Fix.OrdStatus), value))
                throw new ApplicationException($"{value} is not a valid value for OrdStatus");
            return (Fix.OrdStatus)Enum.ToObject(typeof(Fix.OrdStatus), value);
        }

        public static explicit operator TimeInForce?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(Convert.ToChar(field.Value));
            if (!Enum.IsDefined(typeof(Fix.TimeInForce), value))
                throw new ApplicationException($"{value} is not a valid value for TimeInForce");
            return (Fix.TimeInForce)Enum.ToObject(typeof(Fix.TimeInForce), value);
        }

        public static explicit operator ExecType?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(Convert.ToChar(field.Value));
            if (!Enum.IsDefined(typeof(Fix.ExecType), value))
                throw new ApplicationException($"{value} is not a valid value for ExecType");
            return (Fix.ExecType)Enum.ToObject(typeof(Fix.ExecType), value);
        }

        public static explicit operator OrdType?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(Convert.ToChar(field.Value));
            if (!Enum.IsDefined(typeof(Fix.OrdType), value))
                throw new ApplicationException($"{value} is not a valid value for OrdType");
            return (Fix.OrdType)Enum.ToObject(typeof(Fix.OrdType), value);
        }

        public static explicit operator SecurityIDSource?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(Convert.ToChar(field.Value));
            if (!Enum.IsDefined(typeof(Fix.SecurityIDSource), value))
                throw new ApplicationException($"{value} is not a valid value for SecurityIDSource");
            return (Fix.SecurityIDSource)Enum.ToObject(typeof(Fix.SecurityIDSource), value);
        }

        public static explicit operator ExecInst[](Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;

            var values = new List<ExecInst>();

            foreach (string value in from item in field.Value.Split(' ') select item.Trim())
            {
                if (string.IsNullOrEmpty(value))
                    continue;

                try
                {
                    int intValue = Convert.ToInt32(Convert.ToChar(value));
                    if (!Enum.IsDefined(typeof(Fix.ExecInst), intValue))
                        throw new ApplicationException();
                    values.Add((Fix.ExecInst)Enum.ToObject(typeof(Fix.ExecInst), intValue));
                }
                catch (Exception)
                {
                    throw new ApplicationException($"{value} is not a valid value for ExecInst");
                }
            }

            if (values.Count == 0)
                return null;

            return values.ToArray();
        }

        public static explicit operator Dictionary.FIX_4_2.ExecType?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(Convert.ToChar(field.Value));
            if (!Enum.IsDefined(typeof(Dictionary.FIX_4_2.ExecType), value))
                throw new ApplicationException($"{value} is not a valid value for ExecType");
            return (Dictionary.FIX_4_2.ExecType)Enum.ToObject(typeof(Dictionary.FIX_4_2.ExecType), value);
        }

        public static explicit operator Dictionary.FIX_4_0.ExecTransType?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(Convert.ToChar(field.Value));
            if (!Enum.IsDefined(typeof(Dictionary.FIX_4_0.ExecTransType), value))
                throw new ApplicationException($"{value} is not a valid value for ExecTransType");
            return (Dictionary.FIX_4_0.ExecTransType)Enum.ToObject(typeof(Dictionary.FIX_4_0.ExecTransType), value);
        }

        public static explicit operator CxlRejResponseTo?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(Convert.ToChar(field.Value));
            if (!Enum.IsDefined(typeof(Fix.CxlRejResponseTo), value))
                throw new ApplicationException($"{value} is not a valid value for CxlRejResponseTo");
            return (Fix.CxlRejResponseTo)Enum.ToObject(typeof(Fix.CxlRejResponseTo), value);
        }

        public static explicit operator TrdType?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(field.Value);
            if (!Enum.IsDefined(typeof(Fix.TrdType), value))
                throw new ApplicationException($"{value} is not a valid value for TrdType");
            return (Fix.TrdType)Enum.ToObject(typeof(Fix.TrdType), value);
        }

        public static explicit operator SessionStatus?(Field field)
        {
            if (string.IsNullOrEmpty(field?.Value))
                return null;
            int value = Convert.ToInt32(field.Value);
            if (!Enum.IsDefined(typeof(Fix.SessionStatus), value))
                throw new ApplicationException($"{value} is not a valid value for SessionStatus");
            return (Fix.SessionStatus)Enum.ToObject(typeof(Fix.SessionStatus), value);
        }

        string EnumDescription(Type enumeratedType, object value)
        {
            string name = Enum.GetName(Definition.EnumeratedType, value);
            if (string.IsNullOrEmpty(name))
                return null;
            MemberInfo[] info = Definition.EnumeratedType.GetMember(name);
            if (info.Length == 0)
                return null;
            var attributes = info[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((DescriptionAttribute)attributes[0]).Description;
        }

        public string ValueDescription
        {
            get
            {
                if (Definition?.EnumeratedType == null)
                    return null;
                var buffer = new StringBuilder();
                bool first = true;
                foreach (string value in from item in Value.Split(' ') select item.Trim())
                {
                    if (string.IsNullOrEmpty(value))
                        continue;

                    char charValue;
                    if (!char.TryParse(value, out charValue))
                        return null;
                    string description = EnumDescription(Definition.EnumeratedType, charValue);
                    if (string.IsNullOrEmpty(description))
                    {
                        int intValue;
                        if (!int.TryParse(value, out intValue))
                            return null;
                        description = EnumDescription(Definition.EnumeratedType, intValue);
                        if (string.IsNullOrEmpty(description))
                            break;
                    }

                    if (first) first = false;
                    else buffer.Append(", ");

                    buffer.Append(description);
                }
                return buffer.ToString();
            }
        }

        public int ComputeCheckSum()
        {
            int checksum = Encoding.ASCII.GetBytes(Tag + "=").Select(b => (int)b).Sum();

            if (!string.IsNullOrEmpty(Value))
            {
                if (Data)
                {
                    byte[] bytes = Convert.FromBase64String(Value);
                    checksum += bytes.Select(b => (sbyte)b).Select(i => (int)i).Sum();
                }
                else
                {
                    checksum += Encoding.ASCII.GetBytes(Value).Select(b => (int)b).Sum();
                }
            }

            checksum += Encoding.ASCII.GetBytes("\x01").Select(b => (int)b).Sum();

            return checksum;
        }

        public int ComputeBodyLength()
        {
            int bodyLength = Encoding.ASCII.GetBytes(string.Format("{0}=", Tag)).Length;

            if (!string.IsNullOrEmpty(Value))
            {
                if (Data)
                {
                    byte[] bytes = Convert.FromBase64String(Value);
                    bodyLength += bytes.Length;
                }
                else
                {
                    bodyLength += Value.Length;
                }
            }

            bodyLength += 1;

            return bodyLength;
        }

        #region Object

        public override string ToString() => $"{Tag}={Value}";

        #endregion

        public object Clone()
        {
            return new Field(Tag, Value) { Definition = Definition };
        }
    }
}
