/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: MessageTypeDataTable.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.Data;

namespace FixClient;

class MessageTypeDataTable : DataTable
{
    public const string ColumnMsgType = "Type";
    public const string ColumnMsgTypeDescription = "Message";
    public const string ColumnSearchMsgType = "SearchType";
    public const string ColumnSearchMsgTypeDescription = "SearchMessage";

    public MessageTypeDataTable(string name)
        : base(name)
    {
        //
        // We can have both j and J as valid messages types.
        //
        CaseSensitive = true;

        DataColumn column = Columns.Add(ColumnMsgType);
        Columns.Add(ColumnMsgTypeDescription);
        Columns.Add(ColumnSearchMsgType).ColumnMapping = MappingType.Hidden;
        Columns.Add(ColumnSearchMsgTypeDescription).ColumnMapping = MappingType.Hidden;
        PrimaryKey = new[] { column };
    }

    protected override Type GetRowType()
    {
        return typeof(MessageTypeDataRow);
    }

    protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
    {
        return new MessageTypeDataRow(builder);
    }
}

