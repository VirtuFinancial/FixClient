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

using Fix;
using System.Data;

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
