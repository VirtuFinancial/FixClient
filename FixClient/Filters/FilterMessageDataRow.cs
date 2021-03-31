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

﻿using System.Data;
using Fix;

namespace FixClient
{
    class FilterMessageDataRow : DataRow
    {
        public FilterMessageDataRow(DataRowBuilder builder)
        :   base(builder)
        {
        }

        public Dictionary.Message Message { get; set; }
    }
}
