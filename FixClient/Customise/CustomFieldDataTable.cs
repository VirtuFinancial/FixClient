/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CustomFieldDataTable.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Data;

namespace FixClient
{
    class CustomFieldDataTable : DataTable
    {
        public const string ColumnNameName = "Name";
        public const string ColumnNameTag = "Tag";

        public CustomFieldDataTable(string name)
        : base(name)
        {
            DataColumn columnTag = Columns.Add(ColumnNameTag);
            Columns.Add(ColumnNameName);
            PrimaryKey = new[] { columnTag };
        }

        protected override System.Type GetRowType()
        {
            return typeof(CustomFieldDataRow);
        }

        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new CustomFieldDataRow(builder);
        }
    }
}
