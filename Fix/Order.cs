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

namespace Fix
{
    public class Order : ICloneable
    {
        public Order(Message message)
        {
            if (message.MsgType != Dictionary.Messages.NewOrderSingle.MsgType)
            {
                throw new ArgumentException("Message is not an NewOrderSingle");
            }

            Field field = message.Fields.Find(Dictionary.Fields.SenderCompID);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a SenderCompID");
            }

            SenderCompID = field.Value;

            field = message.Fields.Find(Dictionary.Fields.TargetCompID);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a TargetCompID");
            }

            TargetCompID = field.Value;

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

            field = message.Fields.Find(Dictionary.Fields.ClOrdID);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain a ClOrdID");
            }

            ClOrdID = field.Value;

            field = message.Fields.Find(Dictionary.Fields.OrderQty);

            if (string.IsNullOrEmpty(field?.Value))
            {
                throw new ArgumentException("Message does not contain an OrderQty");
            }

            OrderQty = (long)field;

            field = message.Fields.Find(Dictionary.Fields.ExDestination);

            if (field != null)
            {
                ExDestination = field.Value;
            }

            Price = (decimal?)message.Fields.Find(Dictionary.Fields.Price);
            Side = (Side?)message.Fields.Find(Dictionary.Fields.Side);
            TimeInForce = (TimeInForce?)message.Fields.Find(Dictionary.Fields.TimeInForce);
            Text = (string)message.Fields.Find(Dictionary.Fields.Text);

            field = message.Fields.Find(Dictionary.Fields.ListID);

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

            Messages = new List<Message>
            {
                message
            };
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
        public Side? Side { get; set; }
        public TimeInForce? TimeInForce { get; set; }
        public OrdStatus? OrdStatus { get; set; }
        public OrdStatus? PreviousOrdStatus { get; set; }
        public string OrderID { get; set; }
        public string Text { get; set; }
        public string ExDestination { get; set; }

        public Message PendingMessage { get; set; }
        public long? PendingOrderQty { get; set; }
        public decimal? PendingPrice { get; set; }

        public bool Active
        {
            get
            {
                if (OrdStatus == null)
                    return true;

                switch (OrdStatus.Value)
                {
                    case Fix.OrdStatus.New:
                    case Fix.OrdStatus.PartiallyFilled:
                    case Fix.OrdStatus.Filled:
                    case Fix.OrdStatus.PendingCancel:
                    case Fix.OrdStatus.PendingNew:
                    case Fix.OrdStatus.Calculated:
                    case Fix.OrdStatus.AcceptedForBidding:
                    case Fix.OrdStatus.PendingReplace:
                        return true;

                    case Fix.OrdStatus.DoneForDay:
                    case Fix.OrdStatus.Canceled:
                    case Fix.OrdStatus.Replaced:
                    case Fix.OrdStatus.Stopped:
                    case Fix.OrdStatus.Rejected:
                    case Fix.OrdStatus.Suspended:
                    case Fix.OrdStatus.Expired:
                        break;
                }

                return false;
            }
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
