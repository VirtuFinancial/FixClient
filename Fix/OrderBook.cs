/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: OrderBook.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Fix
{
    public class OrderBookEventArgs : EventArgs
    {
        public OrderBookEventArgs(Order order)
        {
            Order = order;
        }

        public Order Order { get; }
    }

    public class OrderBook
    {
        #region Events
        
        public delegate void OrderDelegate(object sender, OrderBookEventArgs e);

        public event OrderDelegate OrderInserted;
        public event OrderDelegate OrderUpdated;
        public event OrderDelegate OrderDeleted;

        protected void OnOrderInserted(Order order)
        {
            OrderInserted?.Invoke(this, new OrderBookEventArgs(order));
        }

        protected void OnOrderUpdated(Order order)
        {
            OrderUpdated?.Invoke(this, new OrderBookEventArgs(order));
        }

        protected void OnOrderDeleted(Order order)
        {
            OrderDeleted?.Invoke(this, new OrderBookEventArgs(order));
        }

        #endregion

        const string StatusMessageHeader = "This message was not processed by the order book";

        public Dictionary.OrderedDictionary<string, Order> Orders { get; } = new Dictionary.OrderedDictionary<string, Order>();

        public bool DeleteInactiveOrders { get; set; }
        public int MaximumOrders { get; set; }

        public MessageCollection Messages { get; } = new MessageCollection();

        [Flags]
        public enum Retain
        {
            None        = 0,
            ActiveGTC   = 1 << 1,
            ActiveGTD   = 1 << 2
        }

        public void Clear(Retain options = Retain.None)
        {
            if (options == Retain.None)
            {
                Orders.Clear();
            }
            else
            {
                var ordersToRemove = Orders.Where(item =>
                {
                    if (item.Value.Active)
                    {
                        if ((options & Retain.ActiveGTC) != 0 && item.Value.TimeInForce == TimeInForce.GoodTillCancel)
                            return false;

                        if ((options & Retain.ActiveGTD) != 0 && item.Value.TimeInForce == TimeInForce.GoodTillDate)
                            return false;
                    }

                    return true;
                }).ToArray();

                foreach(var item in ordersToRemove)
                {
                    Orders.Remove(item.Key);
                }
            }

            Messages.Clear();
        }

        public void Delete(Order order)
        {
            if(Orders.Remove(KeyForOrder(order)))
            {
                OnOrderDeleted(order);
            }
        }

        public bool Process(Message message)
        {
            if (message == null)
                return false;

            bool result = true;

            try
            {
                if (message.Fields.Find(Dictionary.Fields.MsgType) == null)
                {
                    message.Status = MessageStatus.Error;
                    message.StatusMessage = " because it does not contain a MsgType";
                    return false;
                }

                if (message.Administrative)
                    return true;

                Field possDupFlag = message.Fields.Find(Dictionary.Fields.PossDupFlag);

                if (possDupFlag != null && (bool) possDupFlag)
                {
                    message.Status = MessageStatus.Warn;
                    message.StatusMessage = StatusMessageHeader + " because it is a possible duplicate";
                    return false;
                }

                if (message.MsgType == Dictionary.Messages.NewOrderSingle.MsgType)
                {
                    result = ProcessNewOrderSingle(message);
                }
                else if (message.MsgType == Dictionary.Messages.ExecutionReport.MsgType)
                {
                    result = ProcessExecutionReport(message);
                }
                else if (message.MsgType == Dictionary.Messages.OrderCancelReject.MsgType)
                {
                    result = ProcessOrderCancelReject(message);
                }
                else if (message.MsgType == Dictionary.Messages.NewOrderList.MsgType ||
                         message.MsgType == Dictionary.FIX_4_0.Messages.KodiakWaveOrder.MsgType)
                {
                    result = ProcessOrderList(message);
                }
                else if (message.MsgType == Dictionary.Messages.OrderCancelReplaceRequest.MsgType ||
                         message.MsgType == Dictionary.FIX_4_0.Messages.KodiakWaveOrderCorrectionRequest.MsgType)
                {
                    result = ProcessOrderCancelReplaceRequest(message);
                }
                else if (message.MsgType == Dictionary.Messages.OrderCancelRequest.MsgType ||
                         message.MsgType == Dictionary.FIX_4_0.Messages.KodiakWaveOrderCancelRequest.MsgType)
                {
                    result = ProcessOrderCancelRequest(message);
                }
            }
            catch (Exception ex)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = ex.Message;
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
                        message.StatusMessage = StatusMessageHeader + " - please send your session to gary.hughes@itg.com so he can improve the error message";
                    }
                }

                Messages.Add(message);
            }

            return result;
        }

        bool ProcessNewOrderSingle(Message message)
        {
            Field clOrdID = message.Fields.Find(Dictionary.Fields.ClOrdID);

            if (clOrdID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the ClOrdID field is missing";
                return false;
            }

            bool result;

            try
            {
                result = AddOrder(new Order(message));
            }
            catch (Exception ex)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = ex.Message;
                result = false;
            }

            return result;
        }

        bool ProcessExecutionReport(Message message)
        {
            /*
             This was removed in 4.3
              
            Fix.Dictionary.FIX_4_2.ExecTransType? execTransType = (Fix.ExecTransType?)message.Fields.Find(Dictionary.Fields.ExecTransType);

            if(execTransType == null)
                return false;

            if(execTransType != Fix.ExecTransType.New &&
               execTransType != Fix.ExecTransType.Cancel &&
               execTransType != Fix.ExecTransType.Correct)
            {
                return false;
            }
            */

            Field ordStatusField = message.Fields.Find(Dictionary.Fields.OrdStatus);

            if (ordStatusField == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the OrdStatus field is missing";
                return false;
            }

            var ordStatus = (OrdStatus) ordStatusField;

            Field ClOrdID = message.Fields.Find(Dictionary.Fields.ClOrdID);

            if (ClOrdID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the ClOrdID field is missing";
                return false;
            }

            Field OrigClOrdID = message.Fields.Find(Dictionary.Fields.OrigClOrdID);

            if(ordStatus == Fix.OrdStatus.Canceled && ClOrdID != null && OrigClOrdID == null)
            {
                if (ProcessOrdStatusUpdate(message, ClOrdID.Value, ordStatus))
                    return true;
            }

            Field ExecType = message.Fields.Find(Dictionary.Fields.ExecType);
            //
            // Use hardcoded values for ExecType because values were removed in later releases and we don't want the
            // conversion to explode.
            //
            if( ordStatus == Fix.OrdStatus.Replaced || 
                ordStatus == Fix.OrdStatus.PendingCancel ||
                ordStatus == Fix.OrdStatus.PendingReplace ||
                ordStatus == Fix.OrdStatus.Canceled ||
                (ExecType != null && ExecType.Value == "5"))
            {
                Field SenderCompID = message.Fields.Find(Dictionary.Fields.SenderCompID);
                Field TargetCompID = message.Fields.Find(Dictionary.Fields.TargetCompID);
                
                if(OrigClOrdID == null || OrigClOrdID.Value == ClOrdID.Value)
                {
                    for(int index = Orders.Count - 1; index >= 0; --index)
                    {
                        Order order = Orders[index];

                        if(order.NewClOrdID == null)
                            continue;
                        
                        if(order.SenderCompID != TargetCompID.Value || order.TargetCompID != SenderCompID.Value)
                           continue;

                        if(order.ClOrdID == ClOrdID.Value)
                        {
                            OrigClOrdID = ClOrdID;
                            ClOrdID = new Field(Dictionary.Fields.ClOrdID, order.NewClOrdID);
                            break;
                        }
                
                        if(order.NewClOrdID == ClOrdID.Value)
                        {
                            OrigClOrdID = new Field(Dictionary.Fields.ClOrdID, order.ClOrdID);
                            ClOrdID = new Field(Dictionary.Fields.ClOrdID, order.NewClOrdID);
                            break;
                        }
                    }
                }

                if(OrigClOrdID != null)
                {
                    //
                    // When we first store the order we set the comp id's relative to the order source so we
                    // need to flip them when searching for orders to match messages coming from the destination.
                    //
                    Order order = FindOrder(TargetCompID.Value,
                                            SenderCompID.Value,
                                            OrigClOrdID.Value);

                    if (order == null)
                    {
                        message.Status = MessageStatus.Error;
                        message.StatusMessage = StatusMessageHeader + string.Format(" because a matching order with ClOrdID = {0} could not be found", OrigClOrdID.Value);
                        return false;
                    }
                    //
                    // Use hardcoded values for ExecType because values were removed in later releases and we don't want the
                    // conversion to explode.
                    //
                    if(ordStatus == Fix.OrdStatus.Replaced ||
                       ordStatus == Fix.OrdStatus.PendingReplace ||
                       ordStatus == Fix.OrdStatus.PendingCancel ||
                       ordStatus == Fix.OrdStatus.Canceled)
                    {
                        ProcessOrdStatusUpdate(message, order, ordStatus);
                    }
                    else if (ExecType != null && ExecType.Value == "5")
                    {
                        ProcessOrdStatusUpdate(message, order, Fix.OrdStatus.Replaced);
                    }
                    
                    if(ordStatus == Fix.OrdStatus.Replaced || (ExecType != null && ExecType.Value == "5"))
                    {
                        Message pending = order.PendingMessage;

                        if(pending == null)
                            return false;

                        var replacement = (Order)order.Clone();
                        replacement.Messages.Add(message);

                        replacement.ClOrdID = ClOrdID.Value;
                        replacement.OrigClOrdID = OrigClOrdID.Value;
                
                        if(ordStatus != Fix.OrdStatus.Replaced)
                        {
					        replacement.OrdStatus = ordStatus;
                        }
                        else if(order.PreviousOrdStatus != null)
                        {
					        replacement.OrdStatus = order.PreviousOrdStatus;
                        }
                        else
                        {
                            replacement.OrdStatus = Fix.OrdStatus.New;
                        }

                        Field OrderQty = message.Fields.Find(Dictionary.Fields.OrderQty);

                        if(OrderQty != null)
				        {
					        replacement.OrderQty = (long)OrderQty;
				        }

                        UpdateOrder(replacement, message, true);
				        AddOrder(replacement);
                    }
        
                    return true;
                }
            }

            return ProcessOrdStatusUpdate(message, ClOrdID.Value, (OrdStatus)ordStatus);
        }

        bool ProcessOrderCancelReject(Message message)
        {
            Field OrigClOrdID = message.Fields.Find(Dictionary.Fields.OrigClOrdID);

            if (OrigClOrdID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the OrigClOrdID field is missing";
                return false;
            }

            Field SenderCompID = message.Fields.Find(Dictionary.Fields.SenderCompID);

            if (SenderCompID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the SenderCompID field is missing";
                return false;
            }

            Field TargetCompID = message.Fields.Find(Dictionary.Fields.TargetCompID);

            if (TargetCompID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the TargetCompID field is missing";
                return false;
            }
            //
            // When we first store the order we set the comp id's relative to the order source so we
            // need to flip them when searching for orders to match messages coming from the destination.
            //
            Order order = FindOrder(TargetCompID.Value,
                                    SenderCompID.Value,
                                    OrigClOrdID.Value);

            if (order == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + $" because a matching order with ClOrdID = {OrigClOrdID.Value} could not be found";
                return false;
            }
            order.OrdStatus = order.PreviousOrdStatus ?? OrdStatus.New;
            Field Text = message.Fields.Find(Dictionary.Fields.Text);

            if(Text != null)
            {
                order.Text = Text.Value;
            }

            order.Messages.Add(message);
            OnOrderUpdated(order);

            if(DeleteInactiveOrders && !order.Active)
            {
                DeleteOrder(order);
                OnOrderDeleted(order);
            }

            return true;
        }

        bool ProcessOrderList(Message message)
        {
            Field SenderCompID = message.Fields.Find(Dictionary.Fields.SenderCompID);
            Field TargetCompID = message.Fields.Find(Dictionary.Fields.TargetCompID);
            Field ListID = message.Fields.Find(Dictionary.Fields.ListID);
            Field ClOrdID = message.Fields.Find(Dictionary.Fields.ClOrdID);

            if (ListID == null)
                return false;

            Message orderSingle = null;
            
            foreach(Field field in message.Fields)
            { 
                if(field.Tag == Dictionary.Fields.ClOrdID.Tag)
                {
                    if(orderSingle != null)
                    {
                        ProcessNewOrderSingle(orderSingle);
                    }

                    orderSingle = new Message
                    {
                        MsgType = Dictionary.Messages.NewOrderSingle.MsgType
                    };
                    orderSingle.Fields.Set(SenderCompID);
                    orderSingle.Fields.Set(TargetCompID);
                    orderSingle.Fields.Set(ListID);
                    orderSingle.Fields.Set(field);

                    continue;
                }

                if(orderSingle != null)
                {
                    orderSingle.Fields.Set(field);
                }
            }
            
            if(orderSingle != null)
            {
                ProcessNewOrderSingle(orderSingle);
            }
            
            return true;
        }

        bool ProcessOrderCancelReplaceRequest(Message message)
        {
            Field ClOrdID = message.Fields.Find(Dictionary.Fields.ClOrdID);

            if (ClOrdID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the ClOrdID field is missing";
                return false;
            }

            Field OrigClOrdID = message.Fields.Find(Dictionary.Fields.OrigClOrdID);

            if (OrigClOrdID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the OrigClOrdID field is missing";
                return false;
            }

            Field SenderCompID = message.Fields.Find(Dictionary.Fields.SenderCompID);

            if (SenderCompID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the SenderCompID field is missing";
                return false;
            }

            Field TargetCompID = message.Fields.Find(Dictionary.Fields.TargetCompID);

            if (TargetCompID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the TargetCompID field is missing";
                return false;
            }

            Order order = FindOrder(SenderCompID.Value,
                                    TargetCompID.Value,
                                    OrigClOrdID.Value);

            if (order == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + $" because a matching order with ClOrdID = {OrigClOrdID.Value} could not be found";
                return false;
            }

            order.PreviousOrdStatus = order.OrdStatus;
            order.OrdStatus = Fix.OrdStatus.PendingReplace;
            order.PendingMessage = message;
            order.NewClOrdID = ClOrdID.Value;

            Field OrderQty = message.Fields.Find(Dictionary.Fields.OrderQty);
            if (OrderQty != null && (long)OrderQty != order.OrderQty)
                order.PendingOrderQty = (long)OrderQty;

            Field Price = message.Fields.Find(Dictionary.Fields.Price);
            if (Price != null && (decimal)Price != order.Price)
                order.PendingPrice = (decimal) Price;

            order.Messages.Add(message);

            OnOrderUpdated(order);

            return true;
        }

        bool ProcessOrderCancelRequest(Message message)
        {
            Field ClOrdID = message.Fields.Find(Dictionary.Fields.ClOrdID);

            if (ClOrdID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the ClOrdID field is missing";
                return false;
            }

            Field OrigClOrdID = message.Fields.Find(Dictionary.Fields.OrigClOrdID);

            if (OrigClOrdID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the OrigClOrdID field is missing";
                return false;
            }

            Field SenderCompID = message.Fields.Find(Dictionary.Fields.SenderCompID);

            if (SenderCompID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the SenderCompID field is missing";
                return false;
            }

            Field TargetCompID = message.Fields.Find(Dictionary.Fields.TargetCompID);

            if (TargetCompID == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the TargetCompID field is missing";
                return false;
            }

            Order order = FindOrder(SenderCompID.Value,
                                    TargetCompID.Value,
                                    OrigClOrdID.Value);

            if (order == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + $" because a matching order with ClOrdID = {OrigClOrdID.Value} could not be found";
                return false;
            }

            order.Messages.Add(message);	
            order.PreviousOrdStatus = order.OrdStatus;
            order.OrdStatus = Fix.OrdStatus.PendingCancel;
            order.NewClOrdID = ClOrdID.Value;

            if (message.MsgType == Dictionary.FIX_4_0.Messages.KodiakWaveOrderCancelRequest.MsgType)
            {
                Orders.ReplaceKey(order.ClOrdID, ClOrdID.Value);
                order.ClOrdID = ClOrdID.Value;
            }

            OnOrderUpdated(order);

            return true;
        }


        bool ProcessOrdStatusUpdate(Message message, string ClOrdID, OrdStatus status)
        {
            //
            // When we first store the order we set the comp id's relative to the order source so we
            // need to flip them when searching for orders to match messages coming from the destination.
            //
            Order order = FindOrder(message.Fields.Find(Dictionary.Fields.TargetCompID).Value,
                                    message.Fields.Find(Dictionary.Fields.SenderCompID).Value,
                                   ClOrdID);
            if (order == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + $" because a matching order with ClOrdID = {ClOrdID} could not be found";
                return false;
            }

            ProcessOrdStatusUpdate(message, order, status);
            
            return true;
        }

        void ProcessOrdStatusUpdate(Message message, Order order, OrdStatus status)
        {
            if (order.OrdStatus != OrdStatus.PendingReplace || (order.OrdStatus == OrdStatus.PendingReplace && status != OrdStatus.PendingCancel))
            {
                order.OrdStatus = status;
            }

            if(order.OrdStatus != Fix.OrdStatus.PendingCancel &&
               order.OrdStatus != Fix.OrdStatus.PendingReplace &&
               order.OrdStatus != Fix.OrdStatus.Replaced)
            {
                UpdateOrder(order, message);
            }
	        else
	        {	
	            Field ExecType = message.Fields.Find(Dictionary.Fields.ExecType);

		        if(ExecType != null)
		        {
                    //
                    // Use hardcoded values for ExecType because values were removed in later releases and we don't want the
                    // conversion to explode.
                    //
			        if(ExecType.Value == "1" /* Partial */|| ExecType.Value == "2" /* Fill */ )
			        {
				        UpdateOrder(order, message);
			        }
		        }
	        }

            if(order.OrdStatus == Fix.OrdStatus.Replaced)
            {
                order.LeavesQty = 0;
                order.PendingOrderQty = null;
                order.PendingPrice = null;
            }
            
            order.Messages.Add(message);
            OnOrderUpdated(order);
            
            if(DeleteInactiveOrders && !order.Active)
            {
                DeleteOrder(order);
                OnOrderDeleted(order);
            }
        }

        public string KeyForOrder(Order order)
        {
            return KeyForOrder(order.SenderCompID, order.TargetCompID, order.ClOrdID);
        }

        public string KeyForOrder(string SenderCompID, string TargetCompID, string ClOrdID)
        {
            return $"{SenderCompID}-{TargetCompID}-{ClOrdID}";
        }

        Order FindOrder(string SenderCompID, string TargetCompID, string ClOrdID)
        {
            Order order;
            if (Orders.TryGetValue(KeyForOrder(SenderCompID, TargetCompID, ClOrdID), out order))
                return order;
            return null;
        }

        void DeleteOrder(Order order)
        {
            Orders.Remove(KeyForOrder(order.SenderCompID, order.TargetCompID, order.ClOrdID));
        }

        bool AddOrder(Order order)
        {
            Order existing = FindOrder(order.SenderCompID, order.TargetCompID, order.ClOrdID);

            if (existing != null)
            {
                order.Messages[0].Status = MessageStatus.Error;
                order.Messages[0].StatusMessage = StatusMessageHeader + $" because an order with ClOrdID = {order.ClOrdID} already exists";
                return false;
            }

            if(MaximumOrders > 0 && Orders.Count >= MaximumOrders)
	        {
                Order inactive = Orders.FirstOrDefault(o => !o.Value.Active).Value;
                if (inactive != null)
                {
                    DeleteOrder(inactive);
                    OnOrderDeleted(inactive);
                }
	        }

            Orders.Add(KeyForOrder(order), order);

            OnOrderInserted(order);
            
            return true;
        }

        void UpdateOrder(Order order, Message message, bool replacement = false)
        {
            Field Price = message.Fields.Find(Dictionary.Fields.Price);
            Field AvgPx = message.Fields.Find(Dictionary.Fields.AvgPx);
            Field CumQty = message.Fields.Find(Dictionary.Fields.CumQty);
            Field LeavesQty = message.Fields.Find(Dictionary.Fields.LeavesQty);
            Field Text = message.Fields.Find(Dictionary.Fields.Text);
            Field OrderID = message.Fields.Find(Dictionary.Fields.OrderID);
            
            if(Price != null && (decimal)Price > 0)
                order.Price = (decimal)Price;

            if(AvgPx != null)
                order.AvgPx = (decimal)AvgPx;

            if(CumQty != null)
                order.CumQty = (long)CumQty;

            if(LeavesQty != null && !replacement)
            {
                order.LeavesQty = (long)LeavesQty;
            }
            else
            {
                if(order.Active)
                {
                    if(order.CumQty.HasValue)
                    {
                        long done = order.CumQty.Value;
                        long value = 0;
                
                        if(done <= order.OrderQty)
                        {
                            value = order.OrderQty - done;
                        }

                        order.LeavesQty = value;
                    }
                }
                else
                {
                    order.LeavesQty = 0;
                }
            }

	        if(order.LeavesQty.HasValue)
	        {
		        if(order.LeavesQty > 0 && order.OrdStatus == Fix.OrdStatus.Filled ||
		           order.LeavesQty < order.OrderQty && order.OrdStatus == Fix.OrdStatus.New)
		        {
			        order.OrdStatus = Fix.OrdStatus.PartiallyFilled;
		        }
	        }
            
            if(Text != null)
                order.Text = Text.Value;
            
            if(OrderID != null)
                order.OrderID = OrderID.Value;
        }
    }
}
