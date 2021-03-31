/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: TradeReportDataTable.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Data;

namespace FixClient
{
    class TradeReportDataTable : DataTable
    {
        public const string ColumnKey = "Key";
        public const string ColumnSide = "Side";
        public const string ColumnSideString = "SideString";
        public const string ColumnSymbol = "Symbol";
        public const string ColumnLastQty = "Quantity";
        public const string ColumnLastPx = "Price";
        public const string ColumnTrdType = "TrdType";
        public const string ColumnTradeReportId = "TradeReportID";
        public const string ColumnPartyId = "PartyID";
        public const string ColumnText = "Text";

        public TradeReportDataTable(string name)
        :   base(name)
        {
            Columns.Add(ColumnSide, typeof(Fix.Side));
            Columns.Add(ColumnSideString).ColumnMapping = MappingType.Hidden;
            Columns.Add(ColumnSymbol);
            Columns.Add(ColumnLastQty);
            Columns.Add(ColumnLastPx);
            Columns.Add(ColumnTradeReportId);
            Columns.Add(ColumnPartyId);
            Columns.Add(ColumnText);
            Columns.Add(ColumnTrdType).ColumnMapping = MappingType.Hidden;
            DataColumn key = Columns.Add(ColumnKey);
            key.ColumnMapping = MappingType.Hidden;
            PrimaryKey = new[] {key};
        }

        protected override Type GetRowType()
        {
            return typeof(TradeReportDataRow);
        }

        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new TradeReportDataRow(builder);
        }
    }
}
