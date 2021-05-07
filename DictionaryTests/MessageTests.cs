/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: MessageTests.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DictionaryTests
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void TestMessages()
        {
            Assert.IsNotNull(Fix.Dictionary.Versions["FIX.4.0"]);
            Assert.IsNotNull(Fix.Dictionary.Versions["FIX.4.1"]);
            Assert.IsNotNull(Fix.Dictionary.Versions["FIX.4.2"]);
            Assert.IsNotNull(Fix.Dictionary.Versions["FIX.4.3"]);
            Assert.IsNotNull(Fix.Dictionary.Versions["FIX.4.4"]);
            Assert.IsNotNull(Fix.Dictionary.Versions["FIX.5.0"]);
            Assert.IsNotNull(Fix.Dictionary.Versions["FIX.5.0SP1"]);
            Assert.IsNotNull(Fix.Dictionary.Versions["FIX.5.0SP2"]);
            Assert.IsNull(Fix.Dictionary.Versions["FIX.6.0"]);
        }

        [TestMethod]
        public void TestMessageDefinitions()
        {
            Assert.AreEqual("0", Fix.Dictionary.Messages.Heartbeat.MsgType);
            Assert.AreEqual("Heartbeat", Fix.Dictionary.Messages.Heartbeat.Name);
            Assert.AreEqual("The Heartbeat monitors the status of the communication link and identifies when the last of a string of messages was not received.", Fix.Dictionary.Messages.Heartbeat.Description);
        }

        [TestMethod]
        public void TestToString()
        {
            Assert.AreEqual("Heartbeat", Fix.Dictionary.Messages.Heartbeat.ToString());
        }

        [TestMethod]
        public void TestMessagesCount()
        {
            Assert.AreEqual(32, Fix.Dictionary.FIX_4_0.Messages.Count);
            Assert.AreEqual(28, Fix.Dictionary.FIX_4_1.Messages.Count);
            Assert.AreEqual(46, Fix.Dictionary.FIX_4_2.Messages.Count);
            Assert.AreEqual(68, Fix.Dictionary.FIX_4_3.Messages.Count);
            Assert.AreEqual(93, Fix.Dictionary.FIX_4_4.Messages.Count);
            Assert.AreEqual(101, Fix.Dictionary.FIX_5_0.Messages.Count);
            Assert.AreEqual(113, Fix.Dictionary.FIX_5_0SP1.Messages.Count);
            Assert.AreEqual(145, Fix.Dictionary.FIX_5_0SP2.Messages.Count);
        }

        [TestMethod]
        public void TestMessageIndexLookup()
        {
            Fix.Dictionary.Message message = Fix.Dictionary.Messages[0];
            Assert.AreEqual("0", message.MsgType);
            Assert.AreEqual("Heartbeat", message.Name);

            message = Fix.Dictionary.Messages[10];
            Assert.AreEqual("A", message.MsgType);
            Assert.AreEqual("Logon", message.Name);
        }

        [TestMethod]
        public void TestMessageMsgTypeLookup()
        {
            Fix.Dictionary.Message message = Fix.Dictionary.Messages["0"];
            Assert.AreEqual("0", message.MsgType);
            Assert.AreEqual("Heartbeat", message.Name);

            message = Fix.Dictionary.Messages["A"];
            Assert.AreEqual("A", message.MsgType);
            Assert.AreEqual("Logon", message.Name);
        }

        [TestMethod]
        public void TestStaticMessageLookup()
        {
            Fix.Dictionary.Message message = Fix.Dictionary.Messages.Logon;
            Assert.AreEqual("A", message.MsgType);
            Assert.AreEqual("Logon", message.Name);
        }

        [TestMethod]
        public void TestDynamicFieldLookup()
        {
            Fix.Dictionary.Message message = Fix.Dictionary.FIX_4_0.Messages.Logon;
            Assert.IsTrue(message.Fields.TryGetValue(10, out Fix.Dictionary.Field field));
            Assert.AreEqual(10, field.Tag);
            Assert.AreEqual("CheckSum", field.Name);
        }
    }
}
