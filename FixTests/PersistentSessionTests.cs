/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: PersistentSessionTests.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;

namespace FixTests
{
    [TestClass]
    public class PersistentSessionTests : SessionTestsBase<Fix.PersistentSession>
    {
        #region Setup

        const string SenderCompId = "INITIATOR";
        const string TargetCompId = "ACCEPTOR";

        readonly object _serialiser = new();

        [TestInitialize]
        public void TestInitialize()
        {
            Monitor.Enter(_serialiser);
            Initialize();
            InitialiseInitiator();
            InitialiseAcceptor();
        }

        void InitialiseInitiator()
        {
            Initiator.SenderCompId = SenderCompId;
            Initiator.TargetCompId = TargetCompId;
            Initiator.TestRequestDelay = 0;
            Initiator.NextExpectedMsgSeqNum = true;
            Initiator.FileName = string.Format("C:\\TEMP\\{0}-{1}.session", Initiator.SenderCompId,
                Initiator.TargetCompId);
            if (File.Exists(Initiator.FileName))
            {
                File.Delete(Initiator.FileName);
            }
        }

        void InitialiseAcceptor()
        {
            Acceptor.SenderCompId = TargetCompId;
            Acceptor.TargetCompId = SenderCompId;
            Acceptor.TestRequestDelay = 0;
            Acceptor.NextExpectedMsgSeqNum = true;
            Acceptor.FileName = string.Format("C:\\TEMP\\{0}-{1}.session", Acceptor.SenderCompId, Acceptor.TargetCompId);
            if (File.Exists(Acceptor.FileName))
            {
                File.Delete(Acceptor.FileName);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Cleanup();
            Monitor.Exit(_serialiser);
        }
        #endregion

        [TestMethod]
        public void TestCorrectNextExpectedMsgSeqNumAtInitiatorAndAcceptor()
        {
            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon);
            ReceiveAtInitiator(Fix.Dictionary.Messages.Logon);

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);
        }

        [TestMethod]
        public void TestNextExpectedMsgSeqNumHigherThanExpected()
        {
            Initiator.OutgoingSeqNum = 3640;
            Initiator.IncomingSeqNum = 1086;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logout, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.Text, string.Format("NextExpectedMsgSeqNum too high, expecting 1 but received 1086"))
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);

            Reconnect();
            InitialiseAcceptor();

            InitiatorStateChange(Fix.State.Connected);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logout, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.Text, string.Format("NextExpectedMsgSeqNum too high, expecting 1 but received 1086"))
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);
        }
    }
}
