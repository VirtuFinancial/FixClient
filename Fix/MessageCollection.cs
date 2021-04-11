/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: MessageCollection.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;

namespace Fix
{
    public class MessageCollection : IEnumerable<Message>, ICloneable
    {
        readonly List<Message> _messages = new();

        #region Events

        public class MessageEvent : EventArgs
        {
            public MessageEvent(Message message)
            {
                Message = message;
            }

            public Message Message { get; }
        }

        public delegate void MessageDelegate(object sender, MessageEvent e);

        public event MessageDelegate MessageAdded;
        public event MessageDelegate Reset;

        protected void OnMessageAdded(Message message)
        {
            MessageAdded?.Invoke(this, new MessageEvent(message));
        }

        protected void OnReset()
        {
            Reset?.Invoke(this, null);
        }

        #endregion

        public void Add(Message message)
        {
            _messages.Add(message);
            OnMessageAdded(message);
        }

        public int Count => _messages.Count;

        public Message this[int index] => _messages[index];

        public void Clear()
        {
            _messages.Clear();
            OnReset();
        }

        public static MessageCollection Parse(string path)
        {
            var parser = new Parser();
            return parser.Parse(new Uri($"file://{Path.GetFullPath(path)}"));
        }

        #region IEnumerable<Message>

        public IEnumerator<Message> GetEnumerator()
        {
            return _messages.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public object Clone()
        {
            var clone = new MessageCollection();
            foreach (Message message in _messages)
                clone.Add(message);
            return clone;
        }
    }
}
