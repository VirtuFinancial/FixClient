/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: TradeReportTests.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FixTests
{
    [TestClass]
    public class TradeReportTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorWrongMsgType()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.ExecutionReport.MsgType };
            var trade = new Fix.TradeReport(message);
        }

        [TestMethod]
        public void TestConstructorWithSymbol()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReport.MsgType };

            message.Fields.Set(Fix.Dictionary.Fields.TradeReportID, "1");
            message.Fields.Set(Fix.Dictionary.Fields.Symbol, "BHP");
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, "1000");
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, "25");
            message.Fields.Set(Fix.Dictionary.Fields.TrdType, Fix.TrdType.RegularTrade);
            message.Fields.Set(Fix.Dictionary.Fields.Side, Fix.Side.Buy);

            var trade = new Fix.TradeReport(message);
        }

        [TestMethod]
        public void TestConstructorWithSecurityID()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReport.MsgType };

            message.Fields.Set(Fix.Dictionary.Fields.TradeReportID, "1");
            message.Fields.Set(Fix.Dictionary.Fields.SecurityID, "BHP");
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, "1000");
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, "25");
            message.Fields.Set(Fix.Dictionary.Fields.TrdType, Fix.TrdType.RegularTrade);
            message.Fields.Set(Fix.Dictionary.Fields.Side, Fix.Side.Buy);

            var trade = new Fix.TradeReport(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorNoSymbolOrSecurityID()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReport.MsgType };
            
            message.Fields.Set(Fix.Dictionary.Fields.TradeReportID, "1");
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, "1000");
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, "25");
            message.Fields.Set(Fix.Dictionary.Fields.TrdType, Fix.TrdType.RegularTrade);
            
            var trade = new Fix.TradeReport(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorNoTradeReportID()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReport.MsgType };

            message.Fields.Set(Fix.Dictionary.Fields.SecurityID, "BHP");
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, "1000");
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, "25");
            message.Fields.Set(Fix.Dictionary.Fields.TrdType, Fix.TrdType.RegularTrade);

            var trade = new Fix.TradeReport(message);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorNoLastQty()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReport.MsgType };

            message.Fields.Set(Fix.Dictionary.Fields.TradeReportID, "1");
            message.Fields.Set(Fix.Dictionary.Fields.SecurityID, "BHP");
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, "25");
            message.Fields.Set(Fix.Dictionary.Fields.TrdType, Fix.TrdType.RegularTrade);

            var trade = new Fix.TradeReport(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorNoLastPx()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReport.MsgType };

            message.Fields.Set(Fix.Dictionary.Fields.TradeReportID, "1");
            message.Fields.Set(Fix.Dictionary.Fields.SecurityID, "BHP");
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, "1000");
            message.Fields.Set(Fix.Dictionary.Fields.TrdType, Fix.TrdType.RegularTrade);

            var trade = new Fix.TradeReport(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorNoTrdType()
        {
            var message = new Fix.Message { MsgType = Fix.Dictionary.Messages.TradeCaptureReport.MsgType };

            message.Fields.Set(Fix.Dictionary.Fields.TradeReportID, "1");
            message.Fields.Set(Fix.Dictionary.Fields.SecurityID, "BHP");
            message.Fields.Set(Fix.Dictionary.Fields.LastQty, "1000");
            message.Fields.Set(Fix.Dictionary.Fields.LastPx, "25");

            var trade = new Fix.TradeReport(message);
        }
    }
}
