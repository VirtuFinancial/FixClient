/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Session.Transport.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Fix
{
    public partial class Session
    {
        [Browsable(false)]
        public bool Connected
        {
            get { return _stream != null && _stream.CanRead && _stream.CanWrite; }
        }

        [Browsable(false)]
        public Stream Stream
        {
            get { return _stream; }
            set
            {
                _stream = value;
                State = State.Connected;
                _reader = new Reader(_stream)
                {
                    ValidateDataFields = ValidateDataFields
                };
                if (_writer != null)
                {
                    _writer.MessageWriting -= WriterMessageWriting;
                }
                _writer = new Writer(_stream, true);
                _writer.MessageWriting += WriterMessageWriting;
            }
        }

        void WriterMessageWriting(object sender, Writer.MessageEvent ev)
        {
            OnMessageSending(ev.Message);
        }

        public virtual void Open()
        {
            Logon();
            Task.Run(() =>
            {
                try
                {
                    while (_reader != null)
                    {
                        Message message = _reader.Read();
                        if (message == null)
                            break;
                        Receive(message);
                    }
                }
                catch (EndOfStreamException ex)
                {
                    OnError(ex.Message);
                    Close();
                }
                catch (Exception ex)
                {
                    OnError(ex.Message);
                    Close();
                }
            });
        }

        readonly object _syncRoot = new();

        public virtual void Close()
        {
            lock (_syncRoot)
            {
                if (State == State.Disconnected)
                    return;

                _testRequestTimer?.Dispose();
                _testRequestTimer = null;

                StopDefibrillator();

                _reader?.Close();
                _reader = null;

                _writer?.Close();
                _writer = null;

                var networkStream = _stream as NetworkStream;
                networkStream?.Socket.Shutdown(SocketShutdown.Send);
                _stream?.Close();
                _stream = null;

                State = State.Disconnected;
            }
        }

        public void Send(Message message, bool setSeqNum = true)
        {
            lock (_syncRoot)
            {
                PerformSend(message, setSeqNum);
            }
        }

        void PerformSend(Message message, bool setSeqNum = true)
        {
            try
            {
                if (message.MsgType == Dictionary.Messages.TestRequest.MsgType)
                {
                    Field testReqId = message.Fields.Find(Dictionary.Fields.TestReqID);
                    if (testReqId != null)
                    {
                        ExpectedTestRequestId = testReqId.Value;
                    }
                }
                else if (message.MsgType == Dictionary.Messages.Logon.MsgType && message.ResetSeqNumFlag)
                {
                    State = State.Resetting;
                    OutgoingSeqNum = 1;
                    IncomingSeqNum = 1;
                }

                message.Incoming = false;
                message.Fields.Set(Dictionary.FIXT_1_1.Fields.BeginString, BeginString.BeginString);

                if (BeginString.BeginString == Dictionary.Versions.FIXT_1_1.BeginString &&
                    message.MsgType != Dictionary.Messages.Logon.MsgType)
                {
                    // Remove unpopulated optional header fields.
                    Field field = message.Fields.Find(Dictionary.Fields.ApplVerID);
                    if (field != null && string.IsNullOrEmpty(field.Value))
                        message.Fields.Remove(field.Tag);

                    field = message.Fields.Find(Dictionary.Fields.CstmApplVerID);
                    if (field != null && string.IsNullOrEmpty(field.Value))
                        message.Fields.Remove(field.Tag);

                    field = message.Fields.Find(Dictionary.Fields.ApplExtID);
                    if (field != null && string.IsNullOrEmpty(field.Value))
                        message.Fields.Remove(field.Tag);
                }

                message.Fields.Set(Dictionary.Fields.SenderCompID, SenderCompId);
                message.Fields.Set(Dictionary.Fields.TargetCompID, TargetCompId);

                if (setSeqNum)
                {
                    message.Fields.Set(Dictionary.Fields.MsgSeqNum, AllocateOutgoingSeqNum());
                }

                message.Fields.Set(Dictionary.Fields.SendingTime, Field.TimeString(MillisecondTimestamps));

                if (_writer != null)
                {
                    _writer.FragmentMessages = FragmentMessages;
                    _writer.Write(message);
                }

                OnMessageSent(message);
            }
            catch (EndOfStreamException ex)
            {
                OnError(ex.Message);
                Close();
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                Close();
            }
        }

        Stream _stream;
        Reader _reader;
        Writer _writer;
    }
}
