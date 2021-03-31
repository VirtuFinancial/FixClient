/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: TradeReport.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fix
{
    public class TradeReport : ICloneable
    {
        public class ReportSide
        {
            public ReportSide(TradeReport tradeReport)
            {
                TradeReport = tradeReport;
            }

            public TradeReport TradeReport { get; }
            public FieldCollection Fields { get; } = new FieldCollection();
            public Side Side => (Side)Fields.Find(Dictionary.Fields.Side);
            public string PartyID => Fields.Find(Dictionary.Fields.PartyID)?.Value;
            public string Text => Fields.Find(Dictionary.Fields.Text)?.Value;
            public string Key => $"{TradeReport.TradeReportID}-{Side}";
        }

        public TradeReport(Message message)
        {
            if (message.MsgType != Dictionary.Messages.TradeCaptureReport.MsgType)
            {
                throw new ArgumentException("Message is not a TradeCaptureReport");
            }

            Field field = message.Fields.Find(Dictionary.Fields.TradeReportID);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a TradeReportID");
            }

            TradeReportID = field.Value;

            field = message.Fields.Find(Dictionary.Fields.Symbol);

            if (string.IsNullOrEmpty(field?.Value))
            {
                field = message.Fields.Find(Dictionary.Fields.SecurityID);

                if (string.IsNullOrEmpty(field?.Value))
                {
                    throw new ArgumentException("Message does not contain a Symbol or SecurityID");
                }
            }

            Symbol = field.Value;

            field = message.Fields.Find(Dictionary.Fields.LastPx);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a LastPx");
            }

            LastPx = (decimal)field;

            field = message.Fields.Find(Dictionary.Fields.LastQty);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a LastQty");
            }

            LastQty = (long)field;

            field = message.Fields.Find(Dictionary.Fields.TrdType);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a TrdType");
            }

            TrdType = (TrdType)field;

            field = message.Fields.Find(Dictionary.Fields.Side);

            if (field == null)
            {
                throw new ArgumentException("Message does not contain a Side");
            }

            Messages = new List<Message>
            {
                message
            };
        }

        public string TradeReportID { get; }
        public string Symbol { get; }
        public decimal LastPx { get; }
        public long LastQty { get; }
        public TrdType TrdType { get; }

        // 207     SecurityExchange            XHKG
        // 48      SecurityID                  3699
        // 22      SecurityIDSource            8       ExchangeSymbol
        public string SecurityExchange => Messages.FirstOrDefault()?.Fields.Find(Dictionary.Fields.SecurityExchange)?.Value;
        public string SecurityID => Messages.FirstOrDefault()?.Fields.Find(Dictionary.Fields.SecurityID)?.Value;
        public SecurityIDSource? SecurityIDSource
        {
            get
            {
                Field field = Messages.FirstOrDefault()?.Fields.Find(Dictionary.Fields.SecurityIDSource);
                if (field != null)
                    return (SecurityIDSource)field;
                return null;
            }
        }

        public ReportSide BuySide { get; set; }
        public ReportSide SellSide { get; set; }
        public List<Message> Messages { get; }

        public void SetSide(ReportSide side)
        {
            if (side.Side == Fix.Side.Buy)
            {
                BuySide = side;
            }
            else
            {
                SellSide = side;
            }
        }

        #region ICloneable

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}
