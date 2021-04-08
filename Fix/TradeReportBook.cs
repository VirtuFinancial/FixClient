/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: TradeReportBook.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace Fix
{
    public class TradeReportBookEventArgs : EventArgs
    {
        public TradeReportBookEventArgs(TradeReport trade)
        {
            Trade = trade;
        }

        public TradeReport Trade { get; }
    }

    public class TradeReportBook
    {
        #region Events

        public delegate void TradeReportDelegate(object sender, TradeReportBookEventArgs e);

        public event TradeReportDelegate TradeReportInserted;
        public event TradeReportDelegate TradeReportUpdated;
        public event TradeReportDelegate TradeReportDeleted;

        protected void OnTradeReportInserted(TradeReport trade)
        {
            TradeReportInserted?.Invoke(this, new TradeReportBookEventArgs(trade));
        }

        protected void OnTradeReportUpdated(TradeReport trade)
        {
            TradeReportUpdated?.Invoke(this, new TradeReportBookEventArgs(trade));
        }

        protected void OnTradeReportDeleted(TradeReport trade)
        {
            TradeReportDeleted?.Invoke(this, new TradeReportBookEventArgs(trade));
        }

        #endregion

        const string StatusMessageHeader = "This message was not processed by the trade report book";

        public TradeReportBook()
        {
            Trades = new List<TradeReport>();
            Messages = new MessageCollection();
        }

        public List<TradeReport> Trades { get; }

        public MessageCollection Messages { get; }

        public void Clear()
        {
            Trades.Clear();
            Messages.Clear();
        }

        public bool Process(Message message)
        {
            if (message == null)
                return false;

            bool result = true;

            try
            {
                if (message.Administrative)
                    return true;

                Field possDupFlag = message.Fields.Find(Dictionary.Fields.PossDupFlag);

                if (possDupFlag != null && (bool)possDupFlag)
                {
                    message.Status = MessageStatus.Warn;
                    message.StatusMessage = StatusMessageHeader + " because it is a possible duplicate";
                    return false;
                }

                if (message.MsgType == Dictionary.Messages.TradeCaptureReportRequest.MsgType)
                {
                    result = ProcessTradeCaptureReportRequest(message);
                }
                else if (message.MsgType == Dictionary.Messages.TradeCaptureReportRequestAck.MsgType)
                {
                    result = ProcessTradeCaptureReportRequestAck(message);
                }
                else if (message.MsgType == Dictionary.Messages.TradeCaptureReport.MsgType)
                {
                    result = ProcessTradeCaptureReport(message);
                }
                else if (message.MsgType == Dictionary.Messages.TradeCaptureReportAck.MsgType)
                {
                    result = ProcessTradeCaptureReportAck(message);
                }
            }
            finally
            {
                if (!result)
                {
                    if (message.Status == MessageStatus.None)
                    {
                        message.Status = MessageStatus.Warn;
                    }

                    if (string.IsNullOrEmpty(message.StatusMessage))
                    {
                        message.StatusMessage = StatusMessageHeader +
                                                " - please send your session to gary.hughes@itg.com so he can improve the error message";
                    }
                }

                Messages.Add(message);
            }

            return result;
        }

        bool ProcessTradeCaptureReportRequest(Message message)
        {
            return false;
        }

        bool ProcessTradeCaptureReportRequestAck(Message message)
        {
            return false;
        }

        bool ProcessTradeCaptureReport(Message message)
        {
            bool result = false;

            try
            {
                Field tradeReportID = message.Fields.Find(Dictionary.Fields.TradeReportID);
                Field symbol = message.Fields.Find(Dictionary.Fields.Symbol);
                if (symbol == null)
                {
                    symbol = message.Fields.Find(Dictionary.Fields.SecurityID);
                }
                Field lastQty = message.Fields.Find(Dictionary.Fields.LastQty);
                Field lastPx = message.Fields.Find(Dictionary.Fields.LastPx);
                Field trdType = message.Fields.Find(Dictionary.Fields.TrdType);

                if (tradeReportID == null)
                {
                    message.Status = MessageStatus.Error;
                    message.StatusMessage = StatusMessageHeader + " because the TradeReportID field is missing";
                    return false;
                }

                if (symbol == null)
                {
                    message.Status = MessageStatus.Error;
                    message.StatusMessage = StatusMessageHeader + " because there is no Symbol or SecurityID field";
                    return false;
                }

                if (lastQty == null)
                {
                    message.Status = MessageStatus.Error;
                    message.StatusMessage = StatusMessageHeader + " because the LastQty field is missing";
                    return false;
                }

                if (lastPx == null)
                {
                    message.Status = MessageStatus.Error;
                    message.StatusMessage = StatusMessageHeader + " because the LastPx field is missing";
                    return false;
                }

                if (trdType == null)
                {
                    message.Status = MessageStatus.Error;
                    message.StatusMessage = StatusMessageHeader + " because the TrdType field is missing";
                    return false;
                }

                var report = new TradeReport(message);
                TradeReport.ReportSide side = null;

                foreach (Field field in message.Fields)
                {
                    if (field.Tag == Dictionary.Fields.Side.Tag)
                    {
                        if (side != null)
                        {
                            report.SetSide(side);
                        }

                        side = new TradeReport.ReportSide(report);
                    }

                    if (field?.Tag == Dictionary.Fields.CheckSum.Tag)
                        continue;

                    side?.Fields.Add(field);
                }

                if (side != null)
                {
                    report.SetSide(side);
                }

                AddTradeReport(report);

                result = true;
            }
            catch (Exception ex)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = ex.Message;
                result = false;
            }

            return result;
        }

        bool ProcessTradeCaptureReportAck(Message message)
        {
            return false;
        }

        TradeReport FindTrade(string tradeReportID)
        {
            return Trades.Find(current => current.TradeReportID == tradeReportID);
        }

        bool AddTradeReport(TradeReport trade)
        {
            TradeReport existing = FindTrade(trade.TradeReportID);

            if (existing != null)
            {
                trade.Messages[0].Status = MessageStatus.Error;
                trade.Messages[0].StatusMessage = StatusMessageHeader + $" because an order with TradeReportID {trade.TradeReportID} already exists";
                return false;
            }

            Trades.Add(trade);
            OnTradeReportInserted(trade);

            return true;
        }

    }
}
