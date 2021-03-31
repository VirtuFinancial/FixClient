/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: OrderTests.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FixTests
{
    [TestClass]
    public class OrderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorWrongMsgType()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.ExecutionReport.MsgType };
            var order = new Fix.Order(message);
            Assert.IsNotNull(order);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestConstructorNoSenderCompId()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.NewOrderSingle.MsgType };
            var order = new Fix.Order(message);
            Assert.IsNotNull(order);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestConstructorNoTargetCompId()
        {
            var message = new Fix.Message {MsgType = Fix.Dictionary.Messages.NewOrderSingle.MsgType};
            message.Fields.Set(Fix.Dictionary.Fields.SenderCompID, "SENDER");
            var order = new Fix.Order(message);
            Assert.IsNotNull(order);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestConstructorNoSymbol()
        {
            var message = new Fix.Message {MsgType = Fix.Dictionary.Messages.NewOrderSingle.MsgType};
            message.Fields.Set(Fix.Dictionary.Fields.SenderCompID, "SENDER");
            message.Fields.Set(Fix.Dictionary.Fields.TargetCompID, "TARGET");
            var order = new Fix.Order(message);
            Assert.IsNotNull(order);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestConstructorNoClOrdId()
        {
            var message = new Fix.Message {MsgType = Fix.Dictionary.Messages.NewOrderSingle.MsgType};
            message.Fields.Set(Fix.Dictionary.Fields.SenderCompID, "SENDER");
            message.Fields.Set(Fix.Dictionary.Fields.TargetCompID, "TARGET");
            message.Fields.Set(Fix.Dictionary.Fields.Symbol, "BHP");
            var order = new Fix.Order(message);
            Assert.IsNotNull(order);
        }

        [TestMethod]
        public void TestConstructorAllMinimumRequirementsMet()
        {
            var message = new Fix.Message {MsgType = Fix.Dictionary.Messages.NewOrderSingle.MsgType};
            message.Fields.Set(Fix.Dictionary.Fields.SenderCompID, "SENDER");
            message.Fields.Set(Fix.Dictionary.Fields.TargetCompID, "TARGET");
            message.Fields.Set(Fix.Dictionary.Fields.Symbol, "BHP");
            message.Fields.Set(Fix.Dictionary.Fields.ClOrdID, "1.2.3");
            message.Fields.Set(Fix.Dictionary.Fields.OrderQty, 5000);
            var order = new Fix.Order(message);
            Assert.IsNotNull(order);
            Assert.AreEqual("SENDER", order.SenderCompID);
            Assert.AreEqual("TARGET", order.TargetCompID);
            Assert.AreEqual("BHP", order.Symbol);
            Assert.AreEqual("1.2.3", order.ClOrdID);
            Assert.AreEqual(5000, order.OrderQty);
            Assert.AreEqual(1, order.Messages.Count);
        }
        

    }
}
