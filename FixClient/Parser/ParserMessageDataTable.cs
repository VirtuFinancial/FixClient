/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: ParserMessageDataTable.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.Data;
using System.Drawing;

namespace FixClient;

class ParserMessageDataTable : DataTable
{
    public const string ColumnSendingTime = "Sent";
    public const string ColumnMsgType = "MsgType";
    public const string ColumnStatus = "Status";
    public const string ColumnStatusImage = "StatusImage";
    public const string ColumnStatusMessage = "StatusMessage";
    public const string ColumnMsgTypeDescription = "Message";
    public const string ColumnMsgSeqNum = "Seq";
    public const string ColumnAdministrative = "Administrative";

    public ParserMessageDataTable(string name)
        : base(name)
    {
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

