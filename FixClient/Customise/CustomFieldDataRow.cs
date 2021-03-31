/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CustomFieldDataRow.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FixClient
{
    class CustomFieldDataRow : DataRow
    {
        public CustomFieldDataRow(DataRowBuilder builder)
        :   base(builder)
        {
        }

        public CustomField Field { get; set; }
    }
}
