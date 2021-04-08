/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: SessionTests.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace FixTests
{
    [TestClass]
    public class SessionTests : SessionTestsBase<Fix.Session>
    {
        #region Setup

        const string SenderCompId = "INITIATOR";
        const string TargetCompId = "ACCEPTOR";

        readonly object _serialiser = new object();

        [TestInitialize]
        public void TestInitialize()
        {
            Monitor.Enter(_serialiser);
            Initialize();
            Initiator.SenderCompId = SenderCompId;
            Initiator.TargetCompId = TargetCompId;
            Initiator.TestRequestDelay = 0;
            Acceptor.SenderCompId = TargetCompId;
            Acceptor.TargetCompId = SenderCompId;
            Acceptor.TestRequestDelay = 0;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Cleanup();
            Monitor.Exit(_serialiser);
        }
        #endregion

        [TestMethod]
        [DeploymentItem("Logs/t_orion_resend.history")]
        public void TestIgnoreSequenceResetDuringUserInitatedResend()
        {
            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            SendFromInitiator(Fix.Dictionary.Messages.NewOrderSingle);
            ReceiveAtAcceptor(Fix.Dictionary.Messages.NewOrderSingle);
            SendFromAcceptor(Fix.Dictionary.Messages.ExecutionReport);
            ReceiveAtInitiator(Fix.Dictionary.Messages.ExecutionReport);

            SendFromAcceptor(Fix.Dictionary.Messages.Heartbeat);
            ReceiveAtInitiator(Fix.Dictionary.Messages.Heartbeat);

            SendFromInitiator(Fix.Dictionary.Messages.NewOrderSingle);
            ReceiveAtAcceptor(Fix.Dictionary.Messages.NewOrderSingle);
            SendFromAcceptor(Fix.Dictionary.Messages.ExecutionReport);
            ReceiveAtInitiator(Fix.Dictionary.Messages.ExecutionReport);

            Assert.AreEqual(6, Initiator.OutgoingSeqNum);
            Assert.AreEqual(7, Initiator.IncomingSeqNum);

            for (int i = 0; i < Initiator.Messages.Count; ++i)
            {
                Console.WriteLine("{0} - {1} - {2}", i, Initiator.Messages[i].MsgSeqNum, Initiator.Messages[i].MsgType);
            }

            Assert.AreEqual(false, Initiator.Messages[7].Administrative);   // ExecutionReport
            Assert.AreEqual(true, Initiator.Messages[8].Administrative);    // Heartbeat
            Assert.AreEqual(false, Initiator.Messages[9].Administrative);   // NewOrderSingle
            Assert.AreEqual(false, Initiator.Messages[10].Administrative);  // ExecutionReport

            SendFromInitiator(Fix.Dictionary.Messages.ResendRequest, new[]
            {
                 new Fix.Field(Fix.Dictionary.Fields.BeginSeqNo, 4),
                 new Fix.Field(Fix.Dictionary.Fields.EndSeqNo, 6)
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.ExecutionReport);
            ReceiveAtInitiator(Fix.Dictionary.Messages.SequenceReset);
            ReceiveAtInitiator(Fix.Dictionary.Messages.ExecutionReport);

            Assert.AreEqual(7, Initiator.OutgoingSeqNum);
            Assert.AreEqual(7, Initiator.IncomingSeqNum);
        }

        [TestMethod]
        public void TestUserInitiatedIntraSessionResend()
        {
            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            // TODO - generate a heap of orders

            // TODO - initate a resend

            // TODO - make sure our seq numbers are not broken
        }

        [TestMethod]
        public void TestSessionStatus()
        {
            Initiator.SessionStatus = Fix.SessionStatus.SessionActive;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            Assert.AreEqual(Fix.SessionStatus.SessionActive, Acceptor.SessionStatus);
        }

        [TestMethod]
        public void TestSessionNoStatus()
        {
            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            Assert.IsNull(Acceptor.SessionStatus);
        }

        [TestMethod]
        public void TestSessionStatusWithNextExpectedMsgSeqNum()
        {
            Acceptor.NextExpectedMsgSeqNum = true;
            Initiator.NextExpectedMsgSeqNum = true;
            Initiator.SessionStatus = Fix.SessionStatus.SessionActive;

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

            Assert.AreEqual(Fix.SessionStatus.SessionActive, Acceptor.SessionStatus);
        }


        [TestMethod]
        public void TestCorrectNextExpectedMsgSeqNumAtInitiatorAndAcceptor()
        {
            Acceptor.NextExpectedMsgSeqNum = true;
            Initiator.NextExpectedMsgSeqNum = true;

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
        public void TestNextExpectedMsgSeqNumMissingAtAcceptor()
        {
            Acceptor.NextExpectedMsgSeqNum = true;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logout, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.Text, string.Format("Logon does not contain NextExpectedMsgSeqNum"))
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);
        }

        [TestMethod]
        public void TestNextExpectedMsgSeqNumMissingAtInitiator()
        {
            Initiator.NextExpectedMsgSeqNum = false;
            Acceptor.NextExpectedMsgSeqNum = true;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon);

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logout, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.Text, string.Format("Logon does not contain NextExpectedMsgSeqNum"))
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);
        }

        [TestMethod]
        public void TestNextExpectedMsgSeqNumHigherThanExpected()
        {
            Acceptor.NextExpectedMsgSeqNum = true;
            Initiator.NextExpectedMsgSeqNum = true;
            Initiator.IncomingSeqNum = 50;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logout, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.Text, string.Format("NextExpectedMsgSeqNum too high, expecting 1 but received 50"))
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);
        }

        [TestMethod]
        public void TestIncomingNextExpectedMsgSeqNumTooLow()
        {
            Acceptor.NextExpectedMsgSeqNum = true;
            Acceptor.OutgoingSeqNum = 389;
            Acceptor.IncomingSeqNum = 393;
            Acceptor.ValidateMessages = false;

            Initiator.NextExpectedMsgSeqNum = true;
            Initiator.OutgoingSeqNum = 396;
            Initiator.IncomingSeqNum = 389;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 396),
                new Fix.Field(Fix.Dictionary.Fields.NextExpectedMsgSeqNum, 389)
            });

            Acceptor.Send(new Fix.Message
            {
                MsgType = Fix.Dictionary.Messages.Logon.MsgType,
                Fields = {
                    new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 389),
                    new Fix.Field(Fix.Dictionary.Fields.HeartBtInt.Tag, 30),
                    new Fix.Field(Fix.Dictionary.Fields.NextExpectedMsgSeqNum, 393)
                }
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 389),
                new Fix.Field(Fix.Dictionary.Fields.NextExpectedMsgSeqNum, 393)
            });

            InitiatorStateChange(Fix.State.Resending);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.SequenceReset, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 393),
                new Fix.Field(Fix.Dictionary.Fields.GapFillFlag, true),
                new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 397)
            });

            Acceptor.Send(new Fix.Message
            {
                MsgType = Fix.Dictionary.Messages.Heartbeat.MsgType,
                Fields = {
                    new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 390)
                }
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 390)
            });

            InitiatorStateChange(Fix.State.LoggedOn);
        }

        [TestMethod]
        public void TestIncomingMsgSeqNumNextExpectedMsgSeqNumDoesNotInitiateResendGapFillMissingMessages()
        {
            Acceptor.NextExpectedMsgSeqNum = true;
            Acceptor.OutgoingSeqNum = 1872;
            Acceptor.IncomingSeqNum = 548;

            Initiator.NextExpectedMsgSeqNum = true;
            Initiator.OutgoingSeqNum = 550;
            Initiator.IncomingSeqNum = 1;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 550),
                new Fix.Field(Fix.Dictionary.Fields.NextExpectedMsgSeqNum, 1)
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 1872),
                new Fix.Field(Fix.Dictionary.Fields.NextExpectedMsgSeqNum, 549)
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.SequenceReset, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 1873)
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.SequenceReset, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 549),
                new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 551)
            });

            InitiatorStateChange(Fix.State.Resending);
            InitiatorStateChange(Fix.State.LoggedOn);

            AcceptorStateChange(Fix.State.Resending);
            AcceptorStateChange(Fix.State.LoggedOn);

            ReceiveAtInitiator(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 1873),
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "0")
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 551),
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "0")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 1874),
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "0")
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 552),
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "0")
            });

            Assert.AreEqual(Fix.State.LoggedOn, Acceptor.State);
            Assert.AreEqual(Fix.State.LoggedOn, Initiator.State);
        }

        [TestMethod]
        public void TestIncomingMsgSeqNumNextExpectedMsgSeqNumDoesNotInitiateResendResendMissingMessages()
        {
            var messageToResend = new Fix.Message
            {
                MsgType = Fix.Dictionary.Messages.ExecutionReport.MsgType
            };
            messageToResend.Fields.Set(Fix.Dictionary.Fields.MsgSeqNum, 6);

            Acceptor.NextExpectedMsgSeqNum = true;
            Acceptor.OutgoingSeqNum = 12;
            Acceptor.IncomingSeqNum = 6;

            Acceptor.Messages.Add(messageToResend);

            Initiator.NextExpectedMsgSeqNum = true;
            Initiator.OutgoingSeqNum = 6;
            Initiator.IncomingSeqNum = 6;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6),
                new Fix.Field(Fix.Dictionary.Fields.NextExpectedMsgSeqNum, 6)
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 12),
                new Fix.Field(Fix.Dictionary.Fields.NextExpectedMsgSeqNum, 7)
            });

            InitiatorStateChange(Fix.State.Resending);
            AcceptorStateChange(Fix.State.Resending);

            ReceiveAtInitiator(Fix.Dictionary.Messages.ExecutionReport, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6),
                new Fix.Field(Fix.Dictionary.Fields.PossDupFlag, true)
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.SequenceReset, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 12),
                new Fix.Field(Fix.Dictionary.Fields.GapFillFlag, true),
                new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 13)
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 7),
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "0")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 13),
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "0")
            });

            /*
            ReceiveAtInitiator(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 14),
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "0")
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 8),
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "0")
            });
            */

            InitiatorStateChange(Fix.State.LoggedOn);
            //AcceptorStateChange(Fix.State.LoggedOn);
        }

        [TestMethod]
        public void TestLogonWithResetSeqNumFlagFromInitiator()
        {
            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            SendFromInitiator(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            SendFromInitiator(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.EncryptMethod.Tag, Fix.EncryptMethod.None),
                new Fix.Field(Fix.Dictionary.Fields.HeartBtInt.Tag, 30),
                new Fix.Field(Fix.Dictionary.Fields.ResetSeqNumFlag, "Y")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.ResetSeqNumFlag, "Y")
            });

            SendFromInitiator(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });
        }

        [TestMethod]
        public void TestLogonWithResetSeqNumFlagFromAcceptor()
        {
            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            SendFromAcceptor(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            SendFromAcceptor(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.EncryptMethod.Tag, Fix.EncryptMethod.None),
                new Fix.Field(Fix.Dictionary.Fields.HeartBtInt.Tag, 30),
                new Fix.Field(Fix.Dictionary.Fields.ResetSeqNumFlag, "Y")
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.ResetSeqNumFlag, "Y")
            });

            SendFromAcceptor(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });
        }

        [TestMethod]
        public void TestNewOrderSingleRejectedFollowedByResend()
        {
            Acceptor.HeartBtInt = 1000;
            Initiator.HeartBtInt = 1000;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            int beginSeqNum = Initiator.OutgoingSeqNum - 1;

            SendFromInitiator(Fix.Dictionary.Messages.NewOrderSingle, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.NoAllocs, 1),
                new Fix.Field(Fix.Dictionary.Fields.AllocAccount, "ROM"),
                new Fix.Field(Fix.Dictionary.Fields.AllocAccount, "FOO")
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.NewOrderSingle, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.NoAllocs, 1),
                new Fix.Field(Fix.Dictionary.Fields.AllocAccount, "ROM"),
                new Fix.Field(Fix.Dictionary.Fields.AllocAccount, "FOO")
            });

            SendFromAcceptor(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, beginSeqNum)
            });

            SentFromAcceptor(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, beginSeqNum)
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, beginSeqNum)
            });

            // Pretend the acceptor didn't increment the MsgSeqNum for the rejected message.
            Acceptor.IncomingSeqNum = beginSeqNum;

            int endSeqNo = Initiator.OutgoingSeqNum;

            SendFromInitiator(Fix.Dictionary.Messages.Heartbeat);
            ReceiveAtAcceptor(Fix.Dictionary.Messages.Heartbeat);

            SentFromAcceptor(Fix.Dictionary.Messages.ResendRequest, new[]
            {
                 new Fix.Field(Fix.Dictionary.Fields.BeginSeqNo, beginSeqNum),
                 new Fix.Field(Fix.Dictionary.Fields.EndSeqNo, endSeqNo)
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.SequenceReset, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.GapFillFlag, true),
                new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, beginSeqNum + 1)
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.NewOrderSingle, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.NoAllocs, 1),
                new Fix.Field(Fix.Dictionary.Fields.AllocAccount, "ROM"),
                new Fix.Field(Fix.Dictionary.Fields.AllocAccount, "FOO"),
                new Fix.Field(Fix.Dictionary.Fields.PossDupFlag, true)
            });

        }

        [TestMethod]
        public void TestLogonSpecifyCompIdsAtBothEndsCorrectly()
        {
            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);
        }

        [TestMethod]
        public void TestLogonOnlySpecifyCompIdsAtInitiator()
        {
            Acceptor.SenderCompId = null;
            Acceptor.TargetCompId = null;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            Assert.AreEqual(Acceptor.SenderCompId, TargetCompId);
            Assert.AreEqual(Acceptor.TargetCompId, SenderCompId);
        }

        [TestMethod]
        public void TestLogonWrongSenderCompId()
        {
            Initiator.SenderCompId = "WRONG";

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            SentFromAcceptor(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.Text, "Received SenderCompID 'WRONG' when expecting 'INITIATOR'")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.Text, "Received SenderCompID 'WRONG' when expecting 'INITIATOR'")
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);

            Assert.IsFalse(Acceptor.Connected);
            Assert.IsFalse(Initiator.Connected);
        }

        [TestMethod]
        public void TestLogonWrongTargetCompId()
        {
            Initiator.TargetCompId = "WRONG";

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            SendFromInitiator(Fix.Dictionary.Messages.Logon);
            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon);

            SentFromAcceptor(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.Text, "Received TargetCompID 'WRONG' when expecting 'ACCEPTOR'")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.Text, "Received TargetCompID 'WRONG' when expecting 'ACCEPTOR'")
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);

            Assert.IsFalse(Acceptor.Connected);
            Assert.IsFalse(Initiator.Connected);
        }

        [TestMethod]
        public void TestHearbeatIntervalExtractedFromLogon()
        {
            Acceptor.HeartBtInt = 30;
            Initiator.HeartBtInt = 10;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            Assert.AreEqual(10, Acceptor.HeartBtInt);
        }

        [TestMethod]
        public void TestTestRequest()
        {
            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);

            SendFromInitiator(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });
        }

        [TestMethod]
        public void TestMsgSeqNumTooHigh()
        {
            Initiator.OutgoingSeqNum = 5;
            Initiator.TestRequestDelay = 1;
            Acceptor.TestRequestDelay = 1;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 5)
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 1)
            });

            AcceptorStateChange(Fix.State.Resending);

            ReceiveAtInitiator(Fix.Dictionary.Messages.ResendRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.BeginSeqNo, 1),
                new Fix.Field(Fix.Dictionary.Fields.EndSeqNo, 5)
            });

            ReceiveAtAcceptor(Fix.Dictionary.Messages.SequenceReset, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.GapFillFlag, "Y"),
                new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 6)
            });

            AcceptorStateChange(Fix.State.LoggedOn);
            InitiatorStateChange(Fix.State.LoggedOn);
        }

        [TestMethod]
        public void TestMsgSeqNumTooLow()
        {
            Initiator.OutgoingSeqNum = 1;
            Acceptor.IncomingSeqNum = 5;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 1)
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logout, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.Text, string.Format("MsgSeqNum too low, expecting 5 but received 1"))
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);
        }

        [TestMethod]
        public void TestIncomingMsgSeqNumAtIniatorWayTooHigh()
        {
            Initiator.OutgoingSeqNum = 2;
            Initiator.TestRequestDelay = 1;
            Acceptor.ValidateMessages = false;
            Acceptor.IncomingSeqNum = 2;
            Acceptor.OutgoingSeqNum = 5002;
            Acceptor.TestRequestDelay = 1;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 2)
            });

            Acceptor.Send(new Fix.Message
            {
                MsgType = Fix.Dictionary.Messages.Logon.MsgType,
                Fields = {
                    new Fix.Field(Fix.Dictionary.Fields.HeartBtInt, 30),
                    new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 5002)
                }
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 5002)
            });

            InitiatorStateChange(Fix.State.Resending);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.ResendRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.BeginSeqNo, 1),
                new Fix.Field(Fix.Dictionary.Fields.EndSeqNo, 5002)
            });

            Acceptor.ValidateMessages = true;

            Acceptor.Send(new Fix.Message
            {
                MsgType = Fix.Dictionary.Messages.SequenceReset.MsgType,
                Fields =
                {
                    new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 5003)
                }
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.SequenceReset, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 5003)
            });

            // We can't test Acceptor state because we had validation turned off so it isn't set at all
            InitiatorStateChange(Fix.State.LoggedOn);
        }

        [TestMethod]
        public void TestSequenceResetAfterLoggedOn()
        {
            Initiator.OutgoingSeqNum = 6246;
            Initiator.IncomingSeqNum = 6247;
            Initiator.ValidateMessages = false;
            Acceptor.IncomingSeqNum = 6243;
            Acceptor.OutgoingSeqNum = 6239;
            Acceptor.TestRequestDelay = 1;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            AcceptorStateChange(Fix.State.LoggingOn);
            InitiatorStateChange(Fix.State.LoggingOn);
            /*
            2014-09-01 13:57:40.1225|DEBUG|AwesomO.Execution.FixClient|Incoming
            {
                  BeginString    (8) - FIX.4.2
                   BodyLength    (9) - 66
                      MsgType   (35) - A - Logon
                    MsgSeqNum   (34) - 6246
                 SenderCompID   (49) - GATE
                 TargetCompID   (56) - GEH_DESK
                  SendingTime   (52) - 20140901-03:57:33
                EncryptMethod   (98) - 0 - None / Other
                   HeartBtInt  (108) - 30
                     CheckSum   (10) - 084
            }
            */
            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6246)
            });
            /*
            2014-09-01 13:57:40.1385|DEBUG|AwesomO.Execution.FixClient|Outgoing
            {
                  BeginString    (8) - FIX.4.2
                   BodyLength    (9) - 
                      MsgType   (35) - A - Logon
                 SenderCompID   (49) - GEH_DESK
                 TargetCompID   (56) - GATE
                    MsgSeqNum   (34) - 6239
                  SendingTime   (52) - 20140901-03:57:40.147
                EncryptMethod   (98) - 0 - None / Other
                   HeartBtInt  (108) - 30
            }
            */
            ReceiveAtInitiator(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6239)
            });
            /*
            2014-09-01 13:57:40.1545|INFO|AwesomO.Execution.FixClient|[GEH_DESK,GATE] 10.132.100.2:65371 Recoverable message sequence error, expected 6243 received 6246 - initiating recovery
            2014-09-01 13:57:40.1545|INFO|AwesomO.Execution.FixClient|[GEH_DESK,GATE] 10.132.100.2:65371 Transiton from state LoggingOn to state Resending
            2014-09-01 13:57:40.1545|INFO|AwesomO.Execution.FixClient|[GEH_DESK,GATE] 10.132.100.2:65371 Requesting resend, BeginSeqNo 6243 EndSeqNo 6246
            */
            AcceptorStateChange(Fix.State.Resending);
            /*
            2014-09-01 13:57:40.1545|DEBUG|AwesomO.Execution.FixClient|Outgoing
            {
                 BeginString    (8) - FIX.4.2
                  BodyLength    (9) - 
                     MsgType   (35) - 2 - ResendRequest
                SenderCompID   (49) - GEH_DESK
                TargetCompID   (56) - GATE
                   MsgSeqNum   (34) - 6240
                 SendingTime   (52) - 20140901-03:57:40.156
                  BeginSeqNo    (7) - 6243
                    EndSeqNo   (16) - 6246
            }
            */
            ReceiveAtInitiator(Fix.Dictionary.Messages.ResendRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6240),
                new Fix.Field(Fix.Dictionary.Fields.BeginSeqNo, 6243),
                new Fix.Field(Fix.Dictionary.Fields.EndSeqNo, 6246)
            });
            /*
            2014-09-01 13:57:40.1545|DEBUG|AwesomO.Execution.FixClient|Incoming
            {
                    BeginString    (8) - FIX.4.2
                     BodyLength    (9) - 95
                        MsgType   (35) - 4 - SequenceReset
                      MsgSeqNum   (34) - 6243
                    PossDupFlag   (43) - Y - Possible duplicate
                   SenderCompID   (49) - GATE
                   TargetCompID   (56) - GEH_DESK
                OrigSendingTime  (122) - 20140901-03:57:40
                    SendingTime   (52) - 20140901-03:57:40
                       NewSeqNo   (36) - 6247
                    GapFillFlag  (123) - Y - Gap Fill Message, Msg Seq Num Field Valid
                       CheckSum   (10) - 008
            }
            */
            var message = new Fix.Message
            {
                MsgType = Fix.Dictionary.Messages.SequenceReset.MsgType,
                Fields =
                {
                    new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 6247),
                    new Fix.Field(Fix.Dictionary.Fields.PossDupFlag, true),
                    new Fix.Field(Fix.Dictionary.Fields.GapFillFlag, true)
                }
            };
            message.Fields.Set(new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6243));

            Initiator.Send(message, false);
            /*
            2014-09-01 13:57:40.1545|INFO|AwesomO.Execution.FixClient|[GEH_DESK,GATE] 10.132.100.2:65371 SeqenceReset GapFill received, NewSeqNo = 6247
            2014-09-01 13:57:40.1545|INFO|AwesomO.Execution.FixClient|[GEH_DESK,GATE] 10.132.100.2:65371 Incoming sequence number reset to 6247
            2014-09-01 13:57:40.1545|INFO|AwesomO.Execution.FixClient|[GEH_DESK,GATE] 10.132.100.2:65371 Resend complete
            2014-09-01 13:57:40.1545|INFO|AwesomO.Execution.FixClient|[GEH_DESK,GATE] 10.132.100.2:65371 Transiton from state Resending to state LoggedOn
            */
            ReceiveAtAcceptor(Fix.Dictionary.Messages.SequenceReset, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6243),
                new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 6247),
                new Fix.Field(Fix.Dictionary.Fields.PossDupFlag, true),
                new Fix.Field(Fix.Dictionary.Fields.GapFillFlag, true)
            });

            AcceptorStateChange(Fix.State.LoggedOn);
            /*
            2014-09-01 13:57:40.1545|DEBUG|AwesomO.Execution.FixClient|Outgoing
            {
                 BeginString    (8) - FIX.4.2
                  BodyLength    (9) - 
                     MsgType   (35) - 1 - TestRequest
                SenderCompID   (49) - GEH_DESK
                TargetCompID   (56) - GATE
                   MsgSeqNum   (34) - 6241
                 SendingTime   (52) - 20140901-03:57:40.161
                   TestReqID  (112) - 3118
            }
            */
            message = new Fix.Message
            {
                MsgType = Fix.Dictionary.Messages.TestRequest.MsgType,
                Fields =
                {
                    new Fix.Field(Fix.Dictionary.Fields.TestReqID, 3118)
                }
            };
            message.Fields.Set(new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6241));

            Acceptor.Send(message);
            /*
            2014-09-01 13:57:40.1545|DEBUG|AwesomO.Execution.FixClient|Incoming
            {
                    BeginString    (8) - FIX.4.2
                     BodyLength    (9) - 89
                        MsgType   (35) - 4 - SequenceReset
                      MsgSeqNum   (34) - 6247
                    PossDupFlag   (43) - Y - Possible duplicate
                   SenderCompID   (49) - GATE
                   TargetCompID   (56) - GEH_DESK
                OrigSendingTime  (122) - 20140901-03:57:40
                    SendingTime   (52) - 20140901-03:57:40
                       NewSeqNo   (36) - 6248
                       CheckSum   (10) - 227
            }
            */
            message = new Fix.Message
            {
                MsgType = Fix.Dictionary.Messages.SequenceReset.MsgType,
                Fields =
                {
                    new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 6248),
                    new Fix.Field(Fix.Dictionary.Fields.PossDupFlag, true)
                }
            };
            message.Fields.Set(new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6247));

            Initiator.Send(message, false);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.SequenceReset, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6247),
                new Fix.Field(Fix.Dictionary.Fields.NewSeqNo, 6248),
                new Fix.Field(Fix.Dictionary.Fields.PossDupFlag, true)
            });
            /*
            2014-09-01 13:57:40.1545|DEBUG|AwesomO.Execution.FixClient|Incoming
            {
                 BeginString    (8) - FIX.4.2
                  BodyLength    (9) - 63
                     MsgType   (35) - 0 - Heartbeat
                   MsgSeqNum   (34) - 6248
                SenderCompID   (49) - GATE
                TargetCompID   (56) - GEH_DESK
                 SendingTime   (52) - 20140901-03:57:40
                   TestReqID  (112) - 3118
                    CheckSum   (10) - 198
            }
            */
            message = new Fix.Message
            {
                MsgType = Fix.Dictionary.Messages.Heartbeat.MsgType,
                Fields =
                {
                    new Fix.Field(Fix.Dictionary.Fields.TestReqID, 3118)
                }
            };
            message.Fields.Set(new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6248));

            Initiator.Send(message, false);

            ReceiveAtAcceptor(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.MsgSeqNum, 6248),
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, 3118)
            });

            Assert.AreEqual(Fix.State.LoggedOn, Acceptor.State);
        }

        [TestMethod]
        public void TestFirstMessageNotLogon()
        {
            Initiator.LogonBehaviour = Fix.Behaviour.Acceptor;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            InitiatorStateChange(Fix.State.LoggingOn);
            AcceptorStateChange(Fix.State.LoggingOn);

            SendFromInitiator(Fix.Dictionary.Messages.NewOrderSingle);
            ReceiveAtAcceptor(Fix.Dictionary.Messages.NewOrderSingle);

            ReceiveAtInitiator(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.Text, "First message is not a Logon")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logout, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.Text, "First message is not a Logon")
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);

            Assert.IsFalse(Acceptor.Connected);
            Assert.IsFalse(Initiator.Connected);
        }

        [TestMethod]
        public void TestLogonWithNoHeartBtInt()
        {
            Initiator.LogonBehaviour = Fix.Behaviour.Acceptor;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            InitiatorStateChange(Fix.State.LoggingOn);
            AcceptorStateChange(Fix.State.LoggingOn);

            SendFromInitiator(Fix.Dictionary.Messages.Logon);
            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon);

            ReceiveAtInitiator(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.Text, "Logon message does not contain a HeartBtInt")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logout, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.Text, "Logon message does not contain a HeartBtInt")
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);

            Assert.IsFalse(Acceptor.Connected);
            Assert.IsFalse(Initiator.Connected);
        }

        [TestMethod]
        public void TestLogonWithInvalidHeartBtInt()
        {
            Initiator.LogonBehaviour = Fix.Behaviour.Acceptor;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            InitiatorStateChange(Fix.State.LoggingOn);
            AcceptorStateChange(Fix.State.LoggingOn);

            SendFromInitiator(Fix.Dictionary.Messages.Logon, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.HeartBtInt, "DODGY")
            });
            ReceiveAtAcceptor(Fix.Dictionary.Messages.Logon);

            ReceiveAtInitiator(Fix.Dictionary.Messages.Reject, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.RefSeqNum, 1),
                new Fix.Field(Fix.Dictionary.Fields.Text, "DODGY is not a valid numeric HeartBtInt")
            });

            ReceiveAtInitiator(Fix.Dictionary.Messages.Logout, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.Text, "DODGY is not a valid numeric HeartBtInt")
            });

            AcceptorStateChange(Fix.State.Disconnected);
            InitiatorStateChange(Fix.State.Disconnected);

            Assert.IsFalse(Acceptor.Connected);
            Assert.IsFalse(Initiator.Connected);
        }

        [TestMethod]
        public void TestMillisecondTimestampsEnabled()
        {
            Acceptor.MillisecondTimestamps = true;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            SendFromInitiator(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            Fix.Message heartbeat = ReceiveAtInitiator(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            Fix.Field sendingTime = heartbeat.Fields.Find(Fix.Dictionary.Fields.SendingTime);
            Assert.IsNotNull(sendingTime);
            Assert.AreEqual(21, sendingTime.Value.Length);
        }

        [TestMethod]
        public void TestMillisecondTimestampsDisabled()
        {
            Acceptor.MillisecondTimestamps = false;

            Assert.AreEqual(Fix.State.Connected, Acceptor.State);
            Assert.AreEqual(Fix.State.Connected, Initiator.State);

            Acceptor.Open();
            Initiator.Open();

            StandardLogonSequence();

            SendFromInitiator(Fix.Dictionary.Messages.TestRequest, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            Fix.Message heartbeat = ReceiveAtInitiator(Fix.Dictionary.Messages.Heartbeat, new[]
            {
                new Fix.Field(Fix.Dictionary.Fields.TestReqID, "TEST")
            });

            Fix.Field sendingTime = heartbeat.Fields.Find(Fix.Dictionary.Fields.SendingTime);
            Assert.IsNotNull(sendingTime);
            Assert.AreEqual(17, sendingTime.Value.Length);
        }

        [TestMethod]
        public void TestCloneSession()
        {
            var original = new Fix.Session
            {
                OrderBehaviour = Fix.Behaviour.Initiator,
                BeginString = Fix.Dictionary.Versions.FIXT_1_1,
                DefaultApplVerId = Fix.Dictionary.Versions.FIX_5_0,
                LogonBehaviour = Fix.Behaviour.Initiator,
                SenderCompId = "SENDER",
                TargetCompId = "TARGET",
                HeartBtInt = 30,
                MillisecondTimestamps = true,
                IncomingSeqNum = 1,
                OutgoingSeqNum = 1,
                BrokenNewSeqNo = true,
                TestRequestId = 1,
                FragmentMessages = true,
                AutoSendingTime = true
            };

            var clone = (Fix.Session)original.Clone();

            clone.OrderBehaviour = Fix.Behaviour.Acceptor;
            clone.BeginString = Fix.Dictionary.Versions.FIX_4_2;
            clone.DefaultApplVerId = Fix.Dictionary.Versions.FIX_4_2;
            clone.LogonBehaviour = Fix.Behaviour.Acceptor;
            clone.SenderCompId = "INITIATOR";
            clone.TargetCompId = "ACCEPTOR";
            clone.HeartBtInt = 60;
            clone.MillisecondTimestamps = false;
            clone.IncomingSeqNum = 2;
            clone.OutgoingSeqNum = 2;
            clone.BrokenNewSeqNo = false;
            clone.TestRequestId = 2;
            clone.FragmentMessages = false;
            clone.AutoSendingTime = false;

            Assert.AreEqual(Fix.Behaviour.Initiator, original.OrderBehaviour);
            Assert.AreEqual(Fix.Dictionary.Versions.FIXT_1_1, original.BeginString);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_5_0, original.DefaultApplVerId);
            Assert.AreEqual(Fix.Behaviour.Initiator, original.LogonBehaviour);
            Assert.AreEqual("SENDER", original.SenderCompId);
            Assert.AreEqual("TARGET", original.TargetCompId);
            Assert.AreEqual(30, original.HeartBtInt);
            Assert.AreEqual(true, original.MillisecondTimestamps);
            Assert.AreEqual(1, original.IncomingSeqNum);
            Assert.AreEqual(1, original.OutgoingSeqNum);
            Assert.AreEqual(true, original.BrokenNewSeqNo);
            Assert.AreEqual(1, original.TestRequestId);
            Assert.AreEqual(true, original.FragmentMessages);
            Assert.AreEqual(true, original.AutoSendingTime);

            Assert.AreEqual(Fix.Behaviour.Acceptor, clone.OrderBehaviour);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_4_2, clone.BeginString);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_4_2, clone.DefaultApplVerId);
            Assert.AreEqual(Fix.Behaviour.Acceptor, clone.LogonBehaviour);
            Assert.AreEqual("INITIATOR", clone.SenderCompId);
            Assert.AreEqual("ACCEPTOR", clone.TargetCompId);
            Assert.AreEqual(60, clone.HeartBtInt);
            Assert.AreEqual(false, clone.MillisecondTimestamps);
            Assert.AreEqual(2, clone.IncomingSeqNum);
            Assert.AreEqual(2, clone.OutgoingSeqNum);
            Assert.AreEqual(false, clone.BrokenNewSeqNo);
            Assert.AreEqual(2, clone.TestRequestId);
            Assert.AreEqual(false, clone.FragmentMessages);
            Assert.AreEqual(false, clone.AutoSendingTime);
        }

        [TestMethod]
        public void TestClonePersistentSession()
        {
            var original = new Fix.PersistentSession
            {
                OrderBehaviour = Fix.Behaviour.Initiator,
                BeginString = Fix.Dictionary.Versions.FIXT_1_1,
                DefaultApplVerId = Fix.Dictionary.Versions.FIX_5_0,
                LogonBehaviour = Fix.Behaviour.Initiator,
                SenderCompId = "SENDER",
                TargetCompId = "TARGET",
                HeartBtInt = 30,
                MillisecondTimestamps = true,
                IncomingSeqNum = 1,
                OutgoingSeqNum = 1,
                BrokenNewSeqNo = true,
                TestRequestId = 1,
                FragmentMessages = true,
                AutoSendingTime = true,
                FileName = @"C:\some\path\file.session"
            };

            var clone = (Fix.PersistentSession)original.Clone();

            clone.OrderBehaviour = Fix.Behaviour.Acceptor;
            clone.BeginString = Fix.Dictionary.Versions.FIX_4_2;
            clone.DefaultApplVerId = Fix.Dictionary.Versions.FIX_4_2;
            clone.LogonBehaviour = Fix.Behaviour.Acceptor;
            clone.SenderCompId = "INITIATOR";
            clone.TargetCompId = "ACCEPTOR";
            clone.HeartBtInt = 60;
            clone.MillisecondTimestamps = false;
            clone.IncomingSeqNum = 2;
            clone.OutgoingSeqNum = 2;
            clone.BrokenNewSeqNo = false;
            clone.TestRequestId = 2;
            clone.FragmentMessages = false;
            clone.AutoSendingTime = false;
            clone.FileName = @"D:\other\path\file.session";

            Assert.AreEqual(Fix.Behaviour.Initiator, original.OrderBehaviour);
            Assert.AreEqual(Fix.Dictionary.Versions.FIXT_1_1, original.BeginString);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_5_0, original.DefaultApplVerId);
            Assert.AreEqual(Fix.Behaviour.Initiator, original.LogonBehaviour);
            Assert.AreEqual("SENDER", original.SenderCompId);
            Assert.AreEqual("TARGET", original.TargetCompId);
            Assert.AreEqual(30, original.HeartBtInt);
            Assert.AreEqual(true, original.MillisecondTimestamps);
            Assert.AreEqual(1, original.IncomingSeqNum);
            Assert.AreEqual(1, original.OutgoingSeqNum);
            Assert.AreEqual(true, original.BrokenNewSeqNo);
            Assert.AreEqual(1, original.TestRequestId);
            Assert.AreEqual(true, original.FragmentMessages);
            Assert.AreEqual(true, original.AutoSendingTime);
            Assert.AreEqual(@"C:\some\path\file.session", original.FileName);

            Assert.AreEqual(Fix.Behaviour.Acceptor, clone.OrderBehaviour);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_4_2, clone.BeginString);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_4_2, clone.DefaultApplVerId);
            Assert.AreEqual(Fix.Behaviour.Acceptor, clone.LogonBehaviour);
            Assert.AreEqual("INITIATOR", clone.SenderCompId);
            Assert.AreEqual("ACCEPTOR", clone.TargetCompId);
            Assert.AreEqual(60, clone.HeartBtInt);
            Assert.AreEqual(false, clone.MillisecondTimestamps);
            Assert.AreEqual(2, clone.IncomingSeqNum);
            Assert.AreEqual(2, clone.OutgoingSeqNum);
            Assert.AreEqual(false, clone.BrokenNewSeqNo);
            Assert.AreEqual(2, clone.TestRequestId);
            Assert.AreEqual(false, clone.FragmentMessages);
            Assert.AreEqual(false, clone.AutoSendingTime);
            Assert.AreEqual(@"D:\other\path\file.session", clone.FileName);
        }

        /*
        [TestMethod]
        public void TestPossDupFlagTrueAndOrigSendingTimeNotSpecified()
        {
        }

        [TestMethod]
        public void TestInvalidSendingTime()
        {
        }
        */
    }
}
