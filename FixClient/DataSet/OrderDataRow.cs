/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: OrderDataRow.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System.Data;
using Fix;

namespace FixClient
{
    class OrderDataRow : DataRow
    {
        public OrderDataRow(DataRowBuilder builder)
            : base(builder)
        {
        }

        public Order Order { get; set; }
    }
}
