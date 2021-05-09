/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FilterMessageDataRow.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using Fix;
using System.Data;

namespace FixClient
{
    class FilterMessageDataRow : DataRow
    {
        public FilterMessageDataRow(DataRowBuilder builder)
        : base(builder)
        {
        }

        public Dictionary.Message? Message { get; set; }
    }
}
