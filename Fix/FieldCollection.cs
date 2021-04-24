/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FieldCollection.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using static Fix.Dictionary;

namespace Fix
{
    public class FieldCollection : IEnumerable<Field>
    {
        //
        // We need a much smarter data structure here but this will do to get things working and
        // define the interface
        //
        readonly List<Field> _fields = new();
        readonly Dictionary.Message _messageDefinition;

        public FieldCollection(Dictionary.Message messageDefinition = null)
        {
            _messageDefinition = messageDefinition;
        }

        public FieldCollection(string[,] data)
        {
            for (int index = 0; index < data.Length / 2; ++index)
            {
                if (!int.TryParse(data[index, 0], out int tag))
                {
                    throw new ArgumentException($"Non numeric tag '{data[index, 0]}={data[index, 1]}'");
                }

                Add(new Field(tag, data[index, 1]));
            }
        }

        void UpdateDefinition(Field field)
        {
            //throw new NotImplementedException();
            /*
            if (field.Definition == null && _messageDefinition != null)
            {
                if (_messageDefinition.Fields.TryGetValue(field.Tag, out var definition))
                {
                    field.Definition = definition;
                }
            }
            */
        }

        public void Set(Field value)
        {
            foreach (Field field in _fields)
            {
                if (field.Tag == value.Tag)
                {
                    field.Value = value.Value;
                    UpdateDefinition(field);
                    return;
                }
            }
            Add(value);
        }

        /*
        public void Set(Dictionary.Field field, string value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, int value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, long value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, decimal value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, bool value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, Side value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, SecurityIDSource value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, EncryptMethod value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, ExecType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, SessionStatus value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, TrdType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, TrdRptStatus value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, TradeHandlingInstr value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, TradeReportTransType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, TradeReportType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, TradeReportRejectReason value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, NoSides value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, PartyIDSource value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, PartyRole value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, ClearingInstruction value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, OrderCategory value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, Dictionary.FIX_4_2.ExecType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, Dictionary.FIX_5_0.ExecType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, Dictionary.FIX_4_0.ExecTransType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, Dictionary.FIX_4_0.CxlType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, OrdStatus value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, EmailType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, CxlRejResponseTo value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, OrdType value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, TimeInForce value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, HandlInst value) => Set(field.Tag, value);
        public void Set(Dictionary.Field field, Dictionary.Version value) => Set(field.Tag, value.BeginString);
        public void Set(Dictionary.Field field, ExchangeTradeType value) => Set(field.Tag, value);
        */

        public void Set(int tag, string value) => Set(new Field(tag, value));
        public void Set(int tag, int value) => Set(new Field(tag, value));
        public void Set(int tag, long value) => Set(new Field(tag, value));
        public void Set(int tag, decimal value) => Set(new Field(tag, value));
        public void Set(int tag, bool value) => Set(new Field(tag, value));

        public void Set(FieldValue value) => Set(new Field(value.Tag, value.Value));
        public void Set(VersionField field, string value) => Set(new Field(field, value));
        public void Set(VersionField field, int value) => Set(new Field(field, value));
        public void Set(VersionField field, long value) => Set(new Field(field, value));
        public void Set(VersionField field, decimal value) => Set(new Field(field, value));
        public void Set(VersionField field, bool value) => Set(new Field(field, value));
        public void Set(VersionField field, Field value) => Set(new Field(field.Tag, value.Value));

        /*
        public void Set(int tag, EncryptMethod value) => Set(new Field(tag, value));
        public void Set(int tag, ExecType value) => Set(new Field(tag, value));
        public void Set(int tag, SessionStatus value) => Set(new Field(tag, value));
        public void Set(int tag, TrdType value) => Set(new Field(tag, value));
        public void Set(int tag, TrdRptStatus value) => Set(new Field(tag, value));
        public void Set(int tag, TradeHandlingInstr value) => Set(new Field(tag, value));
        public void Set(int tag, TradeReportTransType value) => Set(new Field(tag, value));
        public void Set(int tag, TradeReportType value) => Set(new Field(tag, value));
        public void Set(int tag, TradeReportRejectReason value) => Set(new Field(tag, value));
        public void Set(int tag, NoSides value) => Set(new Field(tag, value));
        public void Set(int tag, PartyIDSource value) => Set(new Field(tag, value));
        public void Set(int tag, PartyRole value) => Set(new Field(tag, value));
        public void Set(int tag, ClearingInstruction value) => Set(new Field(tag, value));
        public void Set(int tag, OrderCategory value) => Set(new Field(tag, value));
        public void Set(int tag, Dictionary.FIX_4_2.ExecType value) => Set(new Field(tag, value));
        public void Set(int tag, Dictionary.FIX_5_0.ExecType value) => Set(new Field(tag, value));
        public void Set(int tag, Dictionary.FIX_4_0.ExecTransType value) => Set(new Field(tag, value));
        public void Set(int tag, Dictionary.FIX_4_0.CxlType value) => Set(new Field(tag, value));
        public void Set(int tag, OrdStatus value) => Set(new Field(tag, value));
        public void Set(int tag, Side value) => Set(new Field(tag, value));
        public void Set(int tag, SecurityIDSource value) => Set(new Field(tag, value));
        public void Set(int tag, EmailType value) => Set(new Field(tag, value));
        public void Set(int tag, CxlRejResponseTo value) => Set(new Field(tag, value));
        public void Set(int tag, OrdType value) => Set(new Field(tag, value));
        public void Set(int tag, TimeInForce value) => Set(new Field(tag, value));
        public void Set(int tag, HandlInst value) => Set(new Field(tag, value));
        public void Set(int tag, ExchangeTradeType value) => Set(new Field(tag, value));


        public void Add(Dictionary.Field definition, string value) => Add(new Field(definition.Tag, value));
        public void Add(Dictionary.Field field, int value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, long value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, decimal value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, bool value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, Side value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, SecurityIDSource value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, EncryptMethod value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, ExecType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, SessionStatus value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, TrdType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, TrdRptStatus value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, TradeHandlingInstr value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, TradeReportTransType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, TradeReportType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, TradeReportRejectReason value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, NoSides value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, PartyIDSource value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, PartyRole value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, ClearingInstruction value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, OrderCategory value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, Dictionary.FIX_4_2.ExecType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, Dictionary.FIX_5_0.ExecType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, Dictionary.FIX_4_0.ExecTransType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, Dictionary.FIX_4_0.CxlType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, OrdStatus value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, EmailType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, CxlRejResponseTo value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, OrdType value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, TimeInForce value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, HandlInst value) => Add(field.Tag, value);
        public void Add(Dictionary.Field field, Dictionary.Version value) => Add(field.Tag, value.BeginString);
        public void Add(Dictionary.Field field, ExchangeTradeType value) => Add(field.Tag, value);
        */


        public void Add(int tag, string value) => Add(new Field(tag, value));
        public void Add(int tag, int value) => Add(new Field(tag, value));
        public void Add(int tag, long value) => Add(new Field(tag, value));
        public void Add(int tag, decimal value) => Add(new Field(tag, value));
        public void Add(int tag, bool value) => Add(new Field(tag, value));

        /*
        public void Add(int tag, EncryptMethod value) => Add(new Field(tag, value));
        public void Add(int tag, ExecType value) => Add(new Field(tag, value));
        public void Add(int tag, SessionStatus value) => Add(new Field(tag, value));
        public void Add(int tag, TrdType value) => Add(new Field(tag, value));
        public void Add(int tag, TrdRptStatus value) => Add(new Field(tag, value));
        public void Add(int tag, TradeHandlingInstr value) => Add(new Field(tag, value));
        public void Add(int tag, TradeReportTransType value) => Add(new Field(tag, value));
        public void Add(int tag, TradeReportType value) => Add(new Field(tag, value));
        public void Add(int tag, TradeReportRejectReason value) => Add(new Field(tag, value));
        public void Add(int tag, NoSides value) => Add(new Field(tag, value));
        public void Add(int tag, PartyIDSource value) => Add(new Field(tag, value));
        public void Add(int tag, PartyRole value) => Add(new Field(tag, value));
        public void Add(int tag, ClearingInstruction value) => Add(new Field(tag, value));
        public void Add(int tag, OrderCategory value) => Add(new Field(tag, value));
        public void Add(int tag, Dictionary.FIX_4_2.ExecType value) => Add(new Field(tag, value));
        public void Add(int tag, Dictionary.FIX_5_0.ExecType value) => Add(new Field(tag, value));
        public void Add(int tag, Dictionary.FIX_4_0.ExecTransType value) => Add(new Field(tag, value));
        public void Add(int tag, Dictionary.FIX_4_0.CxlType value) => Add(new Field(tag, value));
        public void Add(int tag, OrdStatus value) => Add(new Field(tag, value));
        public void Add(int tag, Side value) => Add(new Field(tag, value));
        public void Add(int tag, SecurityIDSource value) => Add(new Field(tag, value));
        public void Add(int tag, EmailType value) => Add(new Field(tag, value));
        public void Add(int tag, CxlRejResponseTo value) => Add(new Field(tag, value));
        public void Add(int tag, OrdType value) => Add(new Field(tag, value));
        public void Add(int tag, TimeInForce value) => Add(new Field(tag, value));
        public void Add(int tag, HandlInst value) => Add(new Field(tag, value));
        public void Add(int tag, ExchangeTradeType value) => Add(new Field(tag, value));
        */


        public void Add(Field field)
        {
            UpdateDefinition(field);
            _fields.Add(field);
        }

        public void Insert(int index, Field value)
        {
            _fields.Insert(index, value);
        }

        public int Count => _fields.Count;

        public bool TryGetValue(Dictionary.VersionField definition, out Field result)
        {
            return TryGetValue(definition.Tag, out result);
        }

        public bool TryGetValue(int tag, out Field result)
        {
            if (_fields is null)
            {
                result = Field.Invalid;
                return false;
            }

            var found = (from field in _fields
                         where field.Tag == tag
                         select field).FirstOrDefault();

            if (found is null)
            {
                result = Field.Invalid;
                return false;
            }

            result = found;

            return true;
        }
        public Field? TryGetValue(Dictionary.VersionField definition)

        {
            return TryGetValue(definition.Tag);
        }

        public Field? TryGetValue(int tag)
        {
            if (TryGetValue(tag, out var field))
            {
                return field;
            }

            return null;
        }


        public bool Contains(VersionField field)
        {
            return Find(field) != null;
        }

        public Field Find(int tag)
        {
            return (from Field field in _fields
                    where field.Tag == tag
                    select field).FirstOrDefault();
        }

        public Field Find(VersionField definition) => Find(definition.Tag);
        
        public Field FindFrom(int tag, int index)
        {
            int temp = index;
            return FindFrom(tag, ref temp);
        }

        public Field FindFrom(int tag, ref int index)
        {
            if (index < 0 || index >= _fields.Count)
                throw new IndexOutOfRangeException();

            for (; index < _fields.Count; ++index)
            {
                Field field = _fields[index];
                if (field.Tag == tag)
                    return field;
            }
            index = -1;
            return null;
        }

        public Field FindFrom(Dictionary.VersionField definition, int index)
        {
            int temp = index;
            return FindFrom(definition, ref temp);
        }

        public Field FindFrom(Dictionary.VersionField definition, ref int index) => FindFrom(definition.Tag, ref index);

        public Field this[int index]
        {
            get { return _fields[index]; }
        }

        public void Clear()
        {
            _fields.Clear();
        }

        public void Repeat(int index, int count)
        {
            _fields.InsertRange(index + count, CloneRange(index, count));
        }

        public void Remove(int index, int count)
        {
            _fields.RemoveRange(index, count);
        }

        public void Remove(Dictionary.VersionField definition)
        {
            Remove(definition.Tag);
        }

        public void Remove(int tag)
        {
            Field existing = _fields.Find(f => f.Tag == tag);
            if (existing != null)
            {
                _fields.Remove(existing);
            }
        }

        IEnumerable<Field> CloneRange(int index, int count)
        {
            return from Field field in _fields.GetRange(index, count) select (Field)field.Clone();
        }

        #region IEnumerable<Field>

        public IEnumerator<Field> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
