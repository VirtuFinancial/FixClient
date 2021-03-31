/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Program.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Timers;
using static System.Console;

namespace FixPersistentSessionPerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("FIX session connecting");
            Connect();
            WriteLine("FIX session connected");
            WriteLine("FIX session logging on");
            Logon();
            WriteLine("FIX session logged on");
            StartTimer();
            StartMessageGenerator();
            Thread.Sleep(30000);
        }

        static void StartMessageGenerator()
        {
            Task.Factory.StartNew(() => SendTestRequests(), TaskCreationOptions.LongRunning);
        }

        static void SendTestRequests()
        {
            for(long testRequestId = 1; testRequestId <= long.MaxValue; ++testRequestId)
            {
                var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TestRequest.MsgType };
                message.Fields.Set(Fix.Dictionary.Fields.TestReqID.Tag, testRequestId);
                Initiator.Send(message);
            }
        }

        static void StartTimer()
        {
            _timer = new System.Timers.Timer
            {
                Interval = IntervalInMilliseconds,
            };
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            long currentAcceptorOutgoingMsgSeqNum = Acceptor.OutgoingSeqNum;
            long currentAcceptorIncomingMsgSeqNum = Acceptor.IncomingSeqNum;
            long outAcceptorMessages = currentAcceptorOutgoingMsgSeqNum - _lastAcceptorOutgoingMsgSeqNum;
            long inAcceptorMessages = currentAcceptorIncomingMsgSeqNum - _lastAcceptorIncomingMsgSeqNum;

            _lastAcceptorOutgoingMsgSeqNum = currentAcceptorOutgoingMsgSeqNum;
            _lastAcceptorIncomingMsgSeqNum = currentAcceptorIncomingMsgSeqNum;

            WriteLine($"Acceptor IN {inAcceptorMessages} OUT {outAcceptorMessages}");
        }

        const long IntervalInMilliseconds = 1000;

        static long _lastAcceptorOutgoingMsgSeqNum = 0;
        static long _lastAcceptorIncomingMsgSeqNum = 0;


        const string Host = "127.0.0.1";
        static int Port = 20101;
        static TcpListener _listener;
        static TcpClient _client;
        static Socket _socket;
        static ManualResetEventSlim _connectionEstablished;
        static ManualResetEventSlim _loggedOn;
        static Fix.PersistentSession Initiator;
        static Fix.PersistentSession Acceptor;
        static System.Timers.Timer _timer;

        static void Acceptor_Error(object sender, Fix.Session.LogEvent ev)
        {
            WriteLine("Acceptor error: " + ev.Message);
        }

        static void Acceptor_Information(object sender, Fix.Session.LogEvent ev)
        {
            WriteLine("Acceptor info: " + ev.Message);
        }

        static void Acceptor_Warning(object sender, Fix.Session.LogEvent ev)
        {
            WriteLine("Acceptor warn: " + ev.Message);
        }

        static void Initiator_Error(object sender, Fix.Session.LogEvent ev)
        {
            WriteLine("Initiator error: " + ev.Message);
        }

        static void Initiator_Information(object sender, Fix.Session.LogEvent ev)
        {
            WriteLine("Initiator info: " + ev.Message);
        }

        static void Initiator_Warning(object sender, Fix.Session.LogEvent ev)
        {
            WriteLine("Initiator warn: " + ev.Message);
        }

        static void AcceptTcpClientCallback(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;
            _socket = listener.EndAcceptSocket(ar);
            _socket.NoDelay = true;
            Acceptor = new Fix.PersistentSession
            {
                Stream = new Fix.NetworkStream(_socket, true),
                LogonBehaviour = Fix.Behaviour.Acceptor,
                PersistMessages = false
            };
            Acceptor.Error += Acceptor_Error;
            Acceptor.Warning += Acceptor_Warning;
            Acceptor.Information += Acceptor_Information;
         
            _connectionEstablished.Set();
        }

        static void Connect()
        {
            var random = new Random(Environment.TickCount);
            Port = random.Next(1024, UInt16.MaxValue);
            _connectionEstablished = new ManualResetEventSlim(false);

            _listener = new TcpListener(Fix.Network.GetLocalAddress(Host), Port);
            _listener.Start();
            _listener.BeginAcceptSocket(AcceptTcpClientCallback, _listener);
            _client = new TcpClient {NoDelay = true};
            _client.Connect(Fix.Network.GetAddress(Host), Port);
            Initiator = new Fix.PersistentSession
            {
                Stream = _client.GetStream(),
                LogonBehaviour = Fix.Behaviour.Initiator,
                PersistMessages = false
            };
            Initiator.Error += Initiator_Error;
            Initiator.Warning += Initiator_Warning;
            Initiator.Information += Initiator_Information;

            _connectionEstablished.Wait();
        }

        static void Logon()
        {
            Acceptor.SenderCompId = "ACCEPTOR";
            Acceptor.TargetCompId = "INITIATOR";

            Initiator.SenderCompId = "INITIATOR";
            Initiator.TargetCompId = "ACCEPTOR";

            Acceptor.FileName = $"C:\\TEMP\\{Acceptor.SenderCompId}-{Acceptor.TargetCompId}.session";
            Initiator.FileName = $"C:\\TEMP\\{Initiator.SenderCompId}-{Initiator.TargetCompId}.session";

            if (File.Exists(Acceptor.FileName))
                File.Delete(Acceptor.FileName);

            if (File.Exists(Initiator.FileName))
                File.Delete(Initiator.FileName);

            _loggedOn = new ManualResetEventSlim(false);
            Acceptor.Open();
            Initiator.StateChanged += Initiator_StateChanged;
            Initiator.Open();
            _loggedOn.Wait();
        }

        static void Initiator_StateChanged(object sender, Fix.Session.StateEvent ev)
        {
            _loggedOn.Set();
        }
    }
}
