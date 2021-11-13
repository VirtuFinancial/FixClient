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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Fix.Dictionary;

namespace FixTests;

[TestClass]
public class OrderTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWrongMsgType()
    {
        var message = new Fix.Message { MsgType = FIX_5_0SP2.Messages.ExecutionReport.MsgType };
        var order = new Fix.Order(message);
        Assert.IsNotNull(order);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorNoSenderCompId()
    {
        var message = new Fix.Message { MsgType = FIX_5_0SP2.Messages.NewOrderSingle.MsgType };
        var order = new Fix.Order(message);
        Assert.IsNotNull(order);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorNoTargetCompId()
    {
        var message = new Fix.Message { MsgType = FIX_5_0SP2.Messages.NewOrderSingle.MsgType };
        message.Fields.Set(FIX_5_0SP2.Fields.SenderCompID, "SENDER");
        var order = new Fix.Order(message);
        Assert.IsNotNull(order);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorNoSymbol()
    {
        var message = new Fix.Message { MsgType = FIX_5_0SP2.Messages.NewOrderSingle.MsgType };
        message.Fields.Set(FIX_5_0SP2.Fields.SenderCompID, "SENDER");
        message.Fields.Set(FIX_5_0SP2.Fields.TargetCompID, "TARGET");
        var order = new Fix.Order(message);
        Assert.IsNotNull(order);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorNoClOrdId()
    {
        var message = new Fix.Message { MsgType = FIX_5_0SP2.Messages.NewOrderSingle.MsgType };
        message.Fields.Set(FIX_5_0SP2.Fields.SenderCompID, "SENDER");
        message.Fields.Set(FIX_5_0SP2.Fields.TargetCompID, "TARGET");
        message.Fields.Set(FIX_5_0SP2.Fields.Symbol, "BHP");
        var order = new Fix.Order(message);
        Assert.IsNotNull(order);
    }

    [TestMethod]
    public void TestConstructorAllMinimumRequirementsMet()
    {
        var message = new Fix.Message { MsgType = FIX_5_0SP2.Messages.NewOrderSingle.MsgType };
        message.Fields.Set(FIX_5_0SP2.Fields.SenderCompID, "SENDER");
        message.Fields.Set(FIX_5_0SP2.Fields.TargetCompID, "TARGET");
        message.Fields.Set(FIX_5_0SP2.Fields.Symbol, "BHP");
        message.Fields.Set(FIX_5_0SP2.Fields.ClOrdID, "1.2.3");
        message.Fields.Set(FIX_5_0SP2.Fields.OrderQty, 5000);
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

