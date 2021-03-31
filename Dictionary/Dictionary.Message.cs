/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Dictionary.Message.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Fix
{
    public partial class Dictionary
    {
        public abstract class Message
        {
            const string Category = "Message";

            protected Message(string msgType, string name, string description, string added)
            {
                MsgType = msgType;
                Name = name;
                Description = description;
                Added = added;
            }

            protected abstract MessageFieldCollection GetFields();

            [Category(Category)]
            public string MsgType { get; }

            [Category(Category)]
            public string Name { get; }

            [Category(Category)]
            public string Added { get; }

            [Browsable(false)]
            public string Description { get; }
            [Browsable(false)]
            public MessageFieldCollection Fields => GetFields();

            [Category(Category)]
            [Description("Fields")]
            [Browsable(false)]
            public abstract int FieldCount { get; }

            #region Object Methods
            public override string ToString()
            {
                return Name;
            }
            #endregion
        }

        public class MessageCollection : IEnumerable<Message>
        {
            public virtual Message this[int index]
            {
                get
                {
                    return _messages[index];
                }
            }

            public virtual Message this[string msgType]
            {
                get
                {
                    return (from message in _messages where message.MsgType == msgType select message).FirstOrDefault();
                }
            }

            public virtual int Count => _messages.Length;

            public virtual IEnumerator<Message> GetEnumerator()
            {
                foreach(Message message in _messages)
                    yield return message;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            protected Message[] _messages;
        }
    }
}
