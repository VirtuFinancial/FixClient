/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: MessageDataTable.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.Data;
using System.Drawing;

namespace FixClient;

class MessageDataTable : DataTable
{
    public const string ColumnSendingTime = "Sent";
    public const string ColumnMsgType = "Type";
    public const string ColumnStatus = "Status";
    public const string ColumnStatusImage = "StatusImage";
    public const string ColumnStatusMessage = "StatusMessage";
    public const string ColumnMsgTypeDescription = "Message";
    public const string ColumnMsgSeqNum = "Seq";
    public const string ColumnAdministrative = "Administrative";

    public MessageDataTable(string name)
        : base(name)
    {
        // In the history view we use virtual mode so it is important to position the visible columns first because
        // they are the only ones the grid knows about and will ask for them with 0 based indexes.
        Columns.Add(ColumnSendingTime);
        Columns.Add(ColumnMsgSeqNum);
        Columns.Add(ColumnStatusImage, typeof(Image));
        Columns.Add(ColumnMsgTypeDescription);
        Columns.Add(ColumnMsgType).ColumnMapping = MappingType.Hidden;
        Columns.Add(ColumnStatus, typeof(Fix.MessageStatus)).ColumnMapping = MappingType.Hidden;
        Columns.Add(ColumnStatusMessage).ColumnMapping = MappingType.Hidden;
        Columns.Add(ColumnAdministrative, typeof(bool)).ColumnMapping = MappingType.Hidden;
    }

    protected override Type GetRowType()
    {
        return typeof(MessageDataRow);
    }

    protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
    {
        return new MessageDataRow(builder);
    }
}

