/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FilterFieldDataTable.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System.Data;

namespace FixClient
{
    class FilterFieldDataTable : DataTable
    {
        public const string ColumnVisible = "Visible";
        public const string ColumnTag = "Tag";
        public const string ColumnName = "Name";

        public FilterFieldDataTable(string name)
            : base(name)
        {
            Columns.Add(ColumnVisible, typeof(bool));
            Columns.Add(ColumnTag, typeof(int));
            Columns.Add(ColumnName);
        }
    }
}
