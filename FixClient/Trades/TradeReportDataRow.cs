/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: TradeReportDataRow.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System.Data;
using Fix;

namespace FixClient
{
    class TradeReportDataRow : DataRow
    {
        public TradeReportDataRow(DataRowBuilder builder)
            : base(builder)
        {
        }

        public TradeReport.ReportSide ReportSide { get; set; }
    }
}
