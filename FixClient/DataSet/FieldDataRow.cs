/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FieldDataRow.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Data;

namespace FixClient
{
    public class FieldDataRow : DataRow
    {
        public FieldDataRow(DataRowBuilder builder)
        : base(builder)
        {
        }

        public Fix.Field Field { get; set; }
    }
}
