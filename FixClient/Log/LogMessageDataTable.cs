/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: LogMessageDataTable.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.Data;

namespace FixClient;

enum LogLevel
{
    Info,
    Warn,
    Error
}

class LogMessageDataTable : DataTable
{
    public const string ColumnTimestamp = "Timestamp";
    public const string ColumnMessage = "Message";
    public const string ColumnLevel = "Level";

    public LogMessageDataTable(string name)
    : base(name)
    {
        Columns.Add(ColumnTimestamp, typeof(DateTime));
        Columns.Add(ColumnMessage);
        Columns.Add(ColumnLevel, typeof(LogLevel)).ColumnMapping = MappingType.Hidden;
    }
}

