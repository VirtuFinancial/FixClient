/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Order.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using static Fix.Dictionary;

namespace Fix
{
    public class Order : ICloneable
    {
        public Order(Message message)
        {
            if (message.MsgType != FIX_5_0SP2.Messages.NewOrderSingle.MsgType)
            {
                throw new ArgumentException("Message is not an NewOrderSingle");
            }

            Field field = message.Fields.Find(FIX_5_0SP2.Fields.SenderCompID);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a SenderCompID");
            }

            SenderCompID = field.Value;

            field = message.Fields.Find(FIX_5_0SP2.Fields.TargetCompID);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a TargetCompID");
            }

            TargetCompID = field.Value;

            field = message.Fields.Find(FIX_5_0SP2.Fields.Symbol);

            if (string.IsNullOrEmpty(field?.Value))
            {
                field = message.Fields.Find(FIX_5_0SP2.Fields.SecurityID);

                if (string.IsNullOrEmpty(field?.Value))
                {
                    throw new ArgumentException("Message does not contain a Symbol or SecurityID");
                }
            }

            Symbol = field.Value;

            field = message.Fields.Find(FIX_5_0SP2.Fields.ClOrdID);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a ClOrdID");
            }

            ClOrdID = field.Value;

            field = message.Fields.Find(FIX_5_0SP2.Fields.OrderQty);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain an OrderQty");
            }

            OrderQty = (long)field;

            field = message.Fields.Find(FIX_5_0SP2.Fields.ExDestination);

            if (field != null)
            {
                ExDestination = field.Value;
            }

            Price = (decimal?)message.Fields.Find(FIX_5_0SP2.Fields.Price);
            Side = message.Fields.Find(FIX_5_0SP2.Fields.Side);
            TimeInForce = message.Fields.Find(FIX_5_0SP2.Fields.TimeInForce);
            Text = message.Fields.Find(FIX_5_0SP2.Fields.Text)?.Value;

            field = message.Fields.Find(FIX_5_0SP2.Fields.ListID);

            if (field != null)
            {
                ListID = field.Value;
            }
            /*
	        mOrdType = order_single->find_field(fix::field::OrdType);
	        mPrice = order_single->find_field(fix::field::Price);
	        mListID = order_single->find_field(fix::field::ListID);
	        mOrdStatus = order_single->find_field(fix::field::OrdStatus);
	        mAvgPx = order_single->find_field(fix::field::AvgPx);
	        mCumQty = order_single->find_field(fix::field::CumQty);
	        mLeavesQty = order_single->find_field(fix::field::LeavesQty);
             */

            var sendingTimeField = message.Fields.Find(FIX_5_0SP2.Fields.SendingTime);
            
            if (sendingTimeField != null)
            {
                SendingTime = (DateTime)sendingTimeField;
            }

            Messages = new List<Message>
            {
                message
            };

            Key = CreateKey(SenderCompID, TargetCompID, ClOrdID);
        }

        public List<Message> Messages { get; private set; }

        public string SenderCompID { get; set; }
        public string TargetCompID { get; set; }
        public string ClOrdID { get; set; }
        public string NewClOrdID { get; set; } // This is for replaced orders, it is the reverse of OrigClOrdID
        public string Symbol { get; set; }
        public long OrderQty { get; set; }

        public long? CumQty { get; set; }
        public long? LeavesQty { get; set; }
        public string ListID { get; set; }
        public string OrigClOrdID { get; set; }
        public decimal? Price { get; set; }
        public decimal? AvgPx { get; set; }
        public Field? Side { get; set; }
        public Field? TimeInForce { get; set; }
        public Field? OrdStatus { get; set; }
        public Field? PreviousOrdStatus { get; set; }
        public string OrderID { get; set; }
        public string Text { get; set; }
        public string ExDestination { get; set; }

        public Message PendingMessage { get; set; }
        public long? PendingOrderQty { get; set; }
        public decimal? PendingPrice { get; set; }
        public DateTime SendingTime { get; private set; }

        public bool Active
        {
            get
            {
                if (OrdStatus == null)
                {
                    return true;
                }

                if (OrdStatus.Value == FIX_5_0SP2.OrdStatus.New.Value ||
                    OrdStatus.Value == FIX_5_0SP2.OrdStatus.PartiallyFilled.Value ||
                    OrdStatus.Value == FIX_5_0SP2.OrdStatus.Filled.Value ||
                    OrdStatus.Value == FIX_5_0SP2.OrdStatus.PendingCancel.Value ||
                    OrdStatus.Value == FIX_5_0SP2.OrdStatus.PendingNew.Value ||
                    OrdStatus.Value == FIX_5_0SP2.OrdStatus.Calculated.Value ||
                    OrdStatus.Value == FIX_5_0SP2.OrdStatus.AcceptedForBidding.Value ||
                    OrdStatus.Value == FIX_5_0SP2.OrdStatus.PendingReplace.Value)
                {
                    return true;
                }

                /*
                    case Fix.OrdStatus.DoneForDay:
                    case Fix.OrdStatus.Canceled:
                    case Fix.OrdStatus.Replaced:
                    case Fix.OrdStatus.Stopped:
                    case Fix.OrdStatus.Rejected:
                    case Fix.OrdStatus.Suspended:
                    case Fix.OrdStatus.Expired:
                        break;
                */
              
                return false;
            }
        }

        public string Key { get; private set; }

        static string CreateKey(string SenderCompID, string TargetCompID, string ClOrdID)
        {
            return $"{SenderCompID}-{TargetCompID}-{ClOrdID}";
        }

        public object Clone()
        {
            var clone = (Order)MemberwiseClone();
            clone.Messages = new List<Message>();
            foreach (var message in Messages)
            {
                clone.Messages.Add((Message)message.Clone());
            }
            return clone;
        }
    }
}
