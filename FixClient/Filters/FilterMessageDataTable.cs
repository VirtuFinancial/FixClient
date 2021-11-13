/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FilterMessageDataTable.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System.Data;

namespace FixClient;

class FilterMessageDataTable : DataTable
{
    public const string ColumnVisible = "Visible";
    public const string ColumnMsgType = "Type";
    public const string ColumnName = "Name";

    public FilterMessageDataTable(string name)
    : base(name)
    {
        Columns.Add(ColumnVisible, typeof(bool));
        Columns.Add(ColumnMsgType);
        Columns.Add(ColumnName);
    }

    protected override System.Type GetRowType()
    {
        return typeof(FilterMessageDataRow);
    }

    protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
    {
        return new FilterMessageDataRow(builder);
    }
}

