/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: MessageTypeDataRow.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System.Data;

namespace FixClient;

class MessageTypeDataRow : DataRow
{
    public MessageTypeDataRow(DataRowBuilder builder)
    : base(builder)
    {
    }

    public Fix.Dictionary.Message? Message { get; set; }
}

