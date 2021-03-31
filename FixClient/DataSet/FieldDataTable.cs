/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FieldDataTable.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Data;

namespace FixClient
{
    class FieldDataTable : DataTable
    {
        public const string ColumnIndent = "Indent";
        public const string ColumnTag = "Tag";
        public const string ColumnName = "Name";
        public const string ColumnValue = "Value";
        public const string ColumnDescription = "Description";
        public const string ColumnRequired = "Required";
        public const string ColumnCustom = "Custom";

        public FieldDataTable(string name)
            : base(name)
        {
            Columns.Add(ColumnIndent, typeof (Int32)).ColumnMapping = MappingType.Hidden;
            Columns.Add(ColumnTag, typeof(Int32));
            Columns.Add(ColumnName);
            Columns.Add(ColumnValue);
            Columns.Add(ColumnDescription);
            Columns.Add(ColumnRequired, typeof(bool)).ColumnMapping = MappingType.Hidden;
            Columns.Add(ColumnCustom, typeof(bool)).ColumnMapping = MappingType.Hidden;
        }

        protected override Type GetRowType()
        {
            return typeof(FieldDataRow);
        }

        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new FieldDataRow(builder);
        }
    }
}
