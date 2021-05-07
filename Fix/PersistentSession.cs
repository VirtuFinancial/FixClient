/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: PersistentSession.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Fix
{
    public class PersistentSession : Session, IDisposable
    {
        readonly object _syncObject = new();

        Timer _writeTimer;
        JsonSerializer _serialiser;

        int _writeRequired;
        const int WRITE_REQUIRED = 1;
        const int WRITE_NON_REQUIRED = 0;

        Writer _historyWriter;
        string _fileName;

        bool _closing;

        public PersistentSession()
        {
            PersistMessages = true;
        }

        // This constructor is only used for cloning
        public PersistentSession(PersistentSession session)
        : base(session)
        {
            FileName = session.FileName;
            PersistMessages = session.PersistMessages;
        }

        [Browsable(false)]
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    lock (_syncObject)
                    {
                        _fileName = GetFileNamePrefix(value) + ".session";
                    }
                }
            }
        }

        [Browsable(false)]
        public bool PersistMessages { get; set; }

        protected static string GetFileNamePrefix(string filename)
        {
            return Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar +
                   Path.GetFileNameWithoutExtension(filename);
        }

        string MessagesFileName
        {
            get
            {
                if (FileName == null)
                    return null;
                return GetFileNamePrefix(FileName) + ".history";
            }
        }

        public override void Open()
        {
            StartSessionWriter();
            StartHistoryWriter();
            base.Open();
        }

        public override void Close()
        {
            _closing = true;
            StopHistoryWriter();
            StopSessionWriter();
            base.Close();
            _closing = false;
        }

        protected override void OnMessageReceived(Message message)
        {
            base.OnMessageReceived(message);
            if (PersistMessages)
            {
                Messages.Add((Message)message.Clone());
            }
            Interlocked.CompareExchange(ref _writeRequired, WRITE_REQUIRED, WRITE_NON_REQUIRED);
        }

        protected override void OnMessageSending(Message message)
        {
            base.OnMessageSending(message);
            if (PersistMessages)
            {
                Messages.Add((Message)message.Clone());
            }
            Interlocked.CompareExchange(ref _writeRequired, WRITE_REQUIRED, WRITE_NON_REQUIRED);
        }

        void StartHistoryWriter()
        {
            lock (_syncObject)
            {
                if (_historyWriter == null)
                {
                    if (string.IsNullOrEmpty(MessagesFileName))
                        return;

                    _historyWriter = new Writer(new FileStream(MessagesFileName, FileMode.Append, FileAccess.Write, FileShare.Read), false);
                    Messages.MessageAdded += MessagesMessageAdded;
                }
            }
        }

        bool StopHistoryWriter()
        {
            bool wasRunning = false;

            lock (_syncObject)
            {
                Messages.MessageAdded -= MessagesMessageAdded;
                _historyWriter?.Dispose();
                _historyWriter = null;
                wasRunning = true;
            }

            return wasRunning;
        }

        void StartSessionWriter()
        {
            if (_writeTimer == null)
            {
                _writeTimer = new Timer(WriteTimerFired, null, 1000, 1000);
            }
        }

        void StopSessionWriter()
        {
            _writeTimer?.Dispose();
            _writeTimer = null;
            WriteSession();
        }

        protected bool Reading;

        public virtual void Read()
        {
            lock (_syncObject)
            {
                if (FileName == null)
                {
                    throw new Exception("FileName has not been set");
                }

                var serializer = CreateSerializer();

                using (var reader = new JsonTextReader(new StreamReader(new FileStream(FileName, FileMode.Open))))
                {
                    serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    serializer.Populate(reader, this);
                }

                UpdateReadonlyAttributes();

                Reading = true;
                ReadMessages();
                Reading = false;
            }
        }

        public virtual void Write()
        {
            WriteSession();
        }

        void WriteTimerFired(object context)
        {
            if (Interlocked.CompareExchange(ref _writeRequired, WRITE_NON_REQUIRED, WRITE_REQUIRED) == WRITE_REQUIRED)
            {
                WriteSession();
            }
        }

        static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            serializer.Converters.Add(new StringEnumConverter());
            serializer.Converters.Add(new Json.FixVersionConverter());

            return serializer;
        }

        void WriteSession()
        {
            try
            {
                if (_serialiser == null)
                {
                    _serialiser = CreateSerializer();
                }

                PersistentSession session;

                lock (_syncObject)
                {
                    session = (PersistentSession)Clone();

                    if (session.FileName == null)
                    {
                        OnError("Ignoring Write request because FileName has not been set");
                        return;
                    }

                    using FileStream stream = new(FileName, FileMode.Create);
                    using JsonWriter writer = new JsonTextWriter(new StreamWriter(stream));
                    writer.Formatting = Formatting.Indented;
                    JObject environment = JObject.FromObject(session, _serialiser);
                    environment.WriteTo(writer);
                }
            }
            catch (Exception ex)
            {
                OnError($"Unable to write session file '{ex.Message}'");
                if (!_closing)
                {
                    Close();
                }
            }
        }

        void ReadMessages()
        {
            if (!PersistMessages)
                return;

            int errors = 0;

            try
            {
                using FileStream stream = new(MessagesFileName, FileMode.OpenOrCreate);
                using Reader reader = new(stream);
                for (; ; )
                {
                    try
                    {
                        Message message = reader.ReadLine();
                        if (message == null)
                            break;
                        Messages.Add(message);
                    }
                    catch (Exception ex)
                    {
                        ++errors;
                        OnWarning(ex.Message);
                        reader.DiscardLine();
                    }
                }
            }
            catch (Exception ex)
            {
                ++errors;
                OnWarning(ex.Message);
            }

            if (errors > 0)
            {
                OnWarning("{0} error{1} occurred reading the message history - messages may be missing or incorrect",
                          errors, errors == 1 ? "" : "s");
            }

            StartHistoryWriter();
        }

        public override void ResetMessages()
        {
            StopSessionWriter();
            bool historyWriterWasRunning = StopHistoryWriter();
            base.ResetMessages();
            File.Delete(MessagesFileName);
            StartSessionWriter();
            if (historyWriterWasRunning)
            {
                StartHistoryWriter();
            }
        }

        void MessagesMessageAdded(object sender, MessageCollection.MessageEvent ev)
        {
            if (Reading)
                return;

            try
            {
                _historyWriter.WriteLine(ev.Message);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                StopSessionWriter();
                StopHistoryWriter();
                _disposed = true;
            }
        }

        bool _disposed;

        #endregion

        #region ICloneable

        public override object Clone()
        {
            return new PersistentSession(this);
        }

        #endregion
    }
}
