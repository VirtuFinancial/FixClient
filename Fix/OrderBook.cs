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
using System;
using System.Linq;
using static Fix.Dictionary;

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

        public event OrderDelegate? OrderInserted;
        public event OrderDelegate? OrderUpdated;
        public event OrderDelegate? OrderDeleted;

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

        public OrderCollection Orders { get; } = new();

        public bool DeleteInactiveOrders { get; set; }
        public int MaximumOrders { get; set; }

        public MessageCollection Messages { get; } = new MessageCollection();

        [Flags]
        public enum Retain
        {
            None = 0,
            ActiveGTC = 1 << 1,
            ActiveGTD = 1 << 2
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
                    if (item.Active)
                    {
                        if ((options & Retain.ActiveGTC) != 0 && item.TimeInForce == FIX_5_0SP2.TimeInForce.GoodTillCancel)
                            return false;

                        if ((options & Retain.ActiveGTD) != 0 && item.TimeInForce == FIX_5_0SP2.TimeInForce.GoodTillDate)
                            return false;
                    }

                    return true;
                }).ToArray();

                foreach (var item in ordersToRemove)
                {
                    Orders.Remove(item.Key);
                }
            }

            Messages.Clear();
        }

        public void Delete(Order order)
        {
            if (Orders.Remove(KeyForOrder(order)))
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
                if (message.Fields.Find(FIX_5_0SP2.Fields.MsgType) is not Field)
                {
                    message.Status = MessageStatus.Error;
                    message.StatusMessage = " because it does not contain a MsgType";
                    return false;
                }

                if (message.Administrative)
                    return true;

                if (message.Fields.Find(FIX_5_0SP2.Fields.PossDupFlag) is Field field && (bool)field)
                {
                    message.Status = MessageStatus.Warn;
                    message.StatusMessage = StatusMessageHeader + " because it is a possible duplicate";
                    return false;
                }

                if (message.MsgType == FIX_5_0SP2.Messages.NewOrderSingle.MsgType)
                {
                    result = ProcessNewOrderSingle(message);
                }
                else if (message.MsgType == FIX_5_0SP2.Messages.ExecutionReport.MsgType)
                {
                    result = ProcessExecutionReport(message);
                }
                else if (message.MsgType == FIX_5_0SP2.Messages.OrderCancelReject.MsgType)
                {
                    result = ProcessOrderCancelReject(message);
                }
                else if (message.MsgType == FIX_5_0SP2.Messages.NewOrderList.MsgType ||
                         message.MsgType == FIX_4_0.Messages.KodiakWaveOrder.MsgType)
                {
                    result = ProcessOrderList(message);
                }
                else if (message.MsgType == FIX_5_0SP2.Messages.OrderCancelReplaceRequest.MsgType ||
                         message.MsgType == FIX_4_0.Messages.KodiakWaveOrderCorrectionRequest.MsgType)
                {
                    result = ProcessOrderCancelReplaceRequest(message);
                }
                else if (message.MsgType == FIX_5_0SP2.Messages.OrderCancelRequest.MsgType ||
                         message.MsgType == FIX_4_0.Messages.KodiakWaveOrderCancelRequest.MsgType)
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
                        message.StatusMessage = StatusMessageHeader + " - please create an issue here https://github.com/GaryHughes/FixClient/issues and attach your session files";
                    }
                }

                Messages.Add(message);
            }

            return result;
        }

        bool ProcessNewOrderSingle(Message message)
        {
            if (message.Fields.Find(FIX_5_0SP2.Fields.ClOrdID) is null)
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

            if (message.Fields.Find(FIX_5_0SP2.Fields.OrdStatus) is not Field ordStatusField)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the OrdStatus field is missing";
                return false;
            }

            var ordStatus = (FieldValue)ordStatusField;

            if (message.Fields.Find(FIX_5_0SP2.Fields.ClOrdID) is not Field ClOrdID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the ClOrdID field is missing";
                return false;
            }

            var OrigClOrdID = message.Fields.Find(FIX_5_0SP2.Fields.OrigClOrdID);

            if (ordStatus == FIX_5_0SP2.OrdStatus.Canceled  && OrigClOrdID is not Field)
            {
                if (ProcessOrdStatusUpdate(message, ClOrdID.Value, ordStatus))
                {
                    return true;
                }
            }

            Field? ExecType = message.Fields.Find(FIX_5_0SP2.Fields.ExecType);
            //
            // Use hardcoded values for ExecType because values were removed in later releases and we don't want the
            // conversion to explode.
            //
            if (ordStatus == FIX_5_0SP2.OrdStatus.Replaced ||
                ordStatus == FIX_5_0SP2.OrdStatus.PendingCancel ||
                ordStatus == FIX_5_0SP2.OrdStatus.PendingReplace ||
                ordStatus == FIX_5_0SP2.OrdStatus.Canceled ||
                ExecType?.Value == "5")
            {
                var SenderCompID = message.SenderCompID;
                var TargetCompID = message.TargetCompID;

                if (OrigClOrdID is not Field || OrigClOrdID.Value == ClOrdID.Value)
                {
                    for (int index = Orders.Count - 1; index >= 0; --index)
                    {
                        Order order = Orders[index];

                        if (order.NewClOrdID == null)
                        {
                            continue;
                        }

                        if (order.SenderCompID != TargetCompID || order.TargetCompID != SenderCompID)
                        {
                            continue;
                        }

                        if (order.ClOrdID == ClOrdID.Value)
                        {
                            OrigClOrdID = ClOrdID;
                            ClOrdID = new Field(FIX_5_0SP2.Fields.ClOrdID, order.NewClOrdID);
                            break;
                        }

                        if (order.NewClOrdID == ClOrdID.Value)
                        {
                            OrigClOrdID = new Field(FIX_5_0SP2.Fields.ClOrdID, order.ClOrdID);
                            ClOrdID = new Field(FIX_5_0SP2.Fields.ClOrdID, order.NewClOrdID);
                            break;
                        }
                    }
                }

                if (OrigClOrdID is Field)
                {
                    //
                    // When we first store the order we set the comp id's relative to the order source so we
                    // need to flip them when searching for orders to match messages coming from the destination.
                    //
                    Order? order = FindOrder(TargetCompID,
                                             SenderCompID,
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
                    if (ordStatus == FIX_5_0SP2.OrdStatus.Replaced ||
                       ordStatus == FIX_5_0SP2.OrdStatus.PendingReplace ||
                       ordStatus == FIX_5_0SP2.OrdStatus.PendingCancel ||
                       ordStatus == FIX_5_0SP2.OrdStatus.Canceled)
                    {
                        ProcessOrdStatusUpdate(message, order, ordStatus);
                    }
                    else if (ExecType?.Value == "5")
                    {
                        ProcessOrdStatusUpdate(message, order, FIX_5_0SP2.OrdStatus.Replaced);
                    }

                    if (ordStatus == FIX_5_0SP2.OrdStatus.Replaced || ExecType?.Value == "5")
                    {
                        Message? pending = order.PendingMessage;

                        if (pending == null)
                        {
                            return false;
                        }

                        var replacement = (Order)order.Clone();
                        replacement.Messages.Add(message);

                        replacement.ClOrdID = ClOrdID.Value;
                        replacement.OrigClOrdID = OrigClOrdID.Value;
                        

                        if (ordStatus != FIX_5_0SP2.OrdStatus.Replaced)
                        {
                            replacement.OrdStatus = ordStatus;
                        }
                        else if (order.PreviousOrdStatus != null)
                        {
                            replacement.OrdStatus = order.PreviousOrdStatus;
                        }
                        else
                        {
                            replacement.OrdStatus = FIX_5_0SP2.OrdStatus.New;
                        }

                        if (message.Fields.Find(FIX_5_0SP2.Fields.OrderQty) is Field OrderQtyField && (long?)OrderQtyField is long OrderQty)
                        {
                            replacement.OrderQty = OrderQty;
                        }

                        UpdateOrder(replacement, message, true);
                        AddOrder(replacement);
                    }

                    return true;
                }
            }

            return ProcessOrdStatusUpdate(message, ClOrdID.Value, ordStatus);
        }

        bool ProcessOrderCancelReject(Message message)
        {
            if (message.Fields.Find(FIX_5_0SP2.Fields.OrigClOrdID) is not Field OrigClOrdID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the OrigClOrdID field is missing";
                return false;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.SenderCompID) is not Field SenderCompID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the SenderCompID field is missing";
                return false;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.TargetCompID) is not Field TargetCompID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the TargetCompID field is missing";
                return false;
            }
            //
            // When we first store the order we set the comp id's relative to the order source so we
            // need to flip them when searching for orders to match messages coming from the destination.
            //
            Order? order = FindOrder(TargetCompID.Value,
                                     SenderCompID.Value,
                                     OrigClOrdID.Value);

            if (order == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + $" because a matching order with ClOrdID = {OrigClOrdID.Value} could not be found";
                return false;
            }
            order.OrdStatus = order.PreviousOrdStatus ?? FIX_5_0SP2.OrdStatus.New;

            if (message.Fields.Find(FIX_5_0SP2.Fields.Text) is Field Text)
            {
                order.Text = Text.Value;
            }

            order.Messages.Add(message);
            OnOrderUpdated(order);

            if (DeleteInactiveOrders && !order.Active)
            {
                DeleteOrder(order);
                OnOrderDeleted(order);
            }

            return true;
        }

        bool ProcessOrderList(Message message)
        {
            Field? SenderCompID = message.Fields.Find(FIX_5_0SP2.Fields.SenderCompID);
            Field? TargetCompID = message.Fields.Find(FIX_5_0SP2.Fields.TargetCompID);
            Field? ListID = message.Fields.Find(FIX_5_0SP2.Fields.ListID);

            if (ListID is null)
            {
                return false;
            }

            Message? orderSingle = null;

            foreach (Field field in message.Fields)
            {
                if (field.Tag == FIX_5_0SP2.Fields.ClOrdID.Tag)
                {
                    if (orderSingle != null)
                    {
                        ProcessNewOrderSingle(orderSingle);
                    }

                    orderSingle = new Message
                    {
                        MsgType = FIX_5_0SP2.Messages.NewOrderSingle.MsgType
                    };

                    if (SenderCompID is Field sender)
                    {
                        orderSingle.Fields.Set(sender);
                    }

                    if (TargetCompID is Field target)
                    {
                        orderSingle.Fields.Set(target);
                    }

                    orderSingle.Fields.Set(ListID);
                    orderSingle.Fields.Set(field);

                    continue;
                }

                if (orderSingle != null)
                {
                    orderSingle.Fields.Set(field);
                }
            }

            if (orderSingle != null)
            {
                ProcessNewOrderSingle(orderSingle);
            }

            return true;
        }

        bool ProcessOrderCancelReplaceRequest(Message message)
        {
            if (message.Fields.Find(FIX_5_0SP2.Fields.ClOrdID) is not Field ClOrdID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the ClOrdID field is missing";
                return false;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.OrigClOrdID) is not Field OrigClOrdID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the OrigClOrdID field is missing";
                return false;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.SenderCompID) is not Field SenderCompID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the SenderCompID field is missing";
                return false;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.TargetCompID) is not Field TargetCompID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the TargetCompID field is missing";
                return false;
            }

            Order? order = FindOrder(SenderCompID.Value,
                                     TargetCompID.Value,
                                     OrigClOrdID.Value);

            if (order == null)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + $" because a matching order with ClOrdID = {OrigClOrdID.Value} could not be found";
                return false;
            }

            order.PreviousOrdStatus = order.OrdStatus;
            order.OrdStatus = FIX_5_0SP2.OrdStatus.PendingReplace;
            order.PendingMessage = message;
            order.NewClOrdID = ClOrdID.Value;

            if (message.Fields.Find(FIX_5_0SP2.Fields.OrderQty) is Field OrderQtyField && 
                (long?)OrderQtyField is long OrderQty && 
                OrderQty != order.OrderQty)
            {
                order.PendingOrderQty = (long)OrderQty;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.Price) is Field PriceField &&
                (decimal?)PriceField is decimal Price &&
                Price != order.Price)
            {
                order.PendingPrice = Price;
            }

            order.Messages.Add(message);

            OnOrderUpdated(order);

            return true;
        }

        bool ProcessOrderCancelRequest(Message message)
        {
            if (message.Fields.Find(FIX_5_0SP2.Fields.ClOrdID) is not Field ClOrdID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the ClOrdID field is missing";
                return false;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.OrigClOrdID) is not Field OrigClOrdID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the OrigClOrdID field is missing";
                return false;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.SenderCompID) is not Field SenderCompID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the SenderCompID field is missing";
                return false;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.TargetCompID) is not Field TargetCompID)
            {
                message.Status = MessageStatus.Error;
                message.StatusMessage = StatusMessageHeader + " because the TargetCompID field is missing";
                return false;
            }

            Order? order = FindOrder(SenderCompID.Value,
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
            order.OrdStatus = FIX_5_0SP2.OrdStatus.PendingCancel;
            order.NewClOrdID = ClOrdID.Value;

            if (message.MsgType == FIX_4_0.Messages.KodiakWaveOrderCancelRequest.MsgType)
            {
                Orders.ReplaceKey(order.ClOrdID, ClOrdID.Value);
                order.ClOrdID = ClOrdID.Value;
            }

            OnOrderUpdated(order);

            return true;
        }


        bool ProcessOrdStatusUpdate(Message message, string ClOrdID, FieldValue status)
        {
            //
            // When we first store the order we set the comp id's relative to the order source so we
            // need to flip them when searching for orders to match messages coming from the destination.
            //
            Order? order = FindOrder(message.TargetCompID,
                                     message.SenderCompID,
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

        void ProcessOrdStatusUpdate(Message message, Order order, FieldValue status)
        {
            if (order.OrdStatus != FIX_5_0SP2.OrdStatus.PendingReplace || (order.OrdStatus == FIX_5_0SP2.OrdStatus.PendingReplace && status != FIX_5_0SP2.OrdStatus.PendingCancel))
            {
                order.OrdStatus = status;
            }

            if (order.OrdStatus != FIX_5_0SP2.OrdStatus.PendingCancel &&
               order.OrdStatus != FIX_5_0SP2.OrdStatus.PendingReplace &&
               order.OrdStatus != FIX_5_0SP2.OrdStatus.Replaced)
            {
                UpdateOrder(order, message);
            }
            else
            {
                Field? ExecType = message.Fields.Find(FIX_5_0SP2.Fields.ExecType);

                if (ExecType is not null)
                {
                    //
                    // Use hardcoded values for ExecType because values were removed in later releases and we don't want the
                    // conversion to explode.
                    //
                    if (ExecType.Value == "1" /* Partial */|| ExecType.Value == "2" /* Fill */ )
                    {
                        UpdateOrder(order, message);
                    }
                }
            }

            if (order.OrdStatus == FIX_5_0SP2.OrdStatus.Replaced)
            {
                order.LeavesQty = 0;
                order.PendingOrderQty = null;
                order.PendingPrice = null;
            }

            order.Messages.Add(message);
            OnOrderUpdated(order);

            if (DeleteInactiveOrders && !order.Active)
            {
                DeleteOrder(order);
                OnOrderDeleted(order);
            }
        }

        public static string KeyForOrder(Order order)
        {
            return KeyForOrder(order.SenderCompID, order.TargetCompID, order.ClOrdID);
        }

        public static string KeyForOrder(string SenderCompID, string TargetCompID, string ClOrdID)
        {
            return $"{SenderCompID}-{TargetCompID}-{ClOrdID}";
        }

        Order? FindOrder(string SenderCompID, string TargetCompID, string ClOrdID)
        {
            if (Orders.TryGetValue(KeyForOrder(SenderCompID, TargetCompID, ClOrdID), out var order))
            {
                return order;
            }

            return null;
        }

        void DeleteOrder(Order order)
        {
            Orders.Remove(KeyForOrder(order.SenderCompID, order.TargetCompID, order.ClOrdID));
        }

        bool AddOrder(Order order)
        {
            Order? existing = FindOrder(order.SenderCompID, order.TargetCompID, order.ClOrdID);

            if (existing != null)
            {
                order.Messages[0].Status = MessageStatus.Error;
                order.Messages[0].StatusMessage = StatusMessageHeader + $" because an order with ClOrdID = {order.ClOrdID} already exists";
                return false;
            }

            if (MaximumOrders > 0 && Orders.Count >= MaximumOrders)
            {
                Order? inactive = Orders.FirstOrDefault(o => !o.Active);
                if (inactive != null)
                {
                    DeleteOrder(inactive);
                    OnOrderDeleted(inactive);
                }
            }

            order.UpdateKey();
            Orders.Add(order);

            OnOrderInserted(order);

            return true;
        }

        static void UpdateOrder(Order order, Message message, bool replacement = false)
        {
            if (message.Fields.Find(FIX_5_0SP2.Fields.Price) is Field PriceField && (decimal?)PriceField is decimal Price && Price > 0)
            {
                order.Price = Price;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.AvgPx) is Field AvgPxField && (decimal?)AvgPxField is decimal AvgPx)
            {
                order.AvgPx = AvgPx;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.CumQty) is Field CumQtyField && (long?)CumQtyField is long CumQty)
            {
                order.CumQty = CumQty;
            }

            if (!replacement && message.Fields.Find(FIX_5_0SP2.Fields.LeavesQty) is Field LeavesQtyField && (long?)LeavesQtyField is long LeavesQty)
            {
                order.LeavesQty = LeavesQty;
            }
            else
            {
                if (order.Active)
                {
                    if (order.CumQty.HasValue)
                    {
                        long done = order.CumQty.Value;
                        long value = 0;

                        if (done <= order.OrderQty)
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

            if (order.LeavesQty.HasValue)
            {
                if (order.LeavesQty > 0 && order.OrdStatus?.Value == FIX_5_0SP2.OrdStatus.Filled.Value ||
                    order.LeavesQty < order.OrderQty && order.OrdStatus?.Value == FIX_5_0SP2.OrdStatus.New.Value)
                {
                    order.OrdStatus = FIX_5_0SP2.OrdStatus.PartiallyFilled;
                }
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.Text) is Field Text)
            {
                order.Text = Text.Value;
            }

            if (message.Fields.Find(FIX_5_0SP2.Fields.OrderID) is Field OrderID)
            {
                order.OrderID = OrderID.Value;
            }
        }
    }
}
