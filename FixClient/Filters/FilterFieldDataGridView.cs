/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FilterFieldDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System.Drawing;
using System.Windows.Forms;

namespace FixClient;

public partial class FilterFieldDataGridView : DataGridView
{
    public FilterFieldDataGridView()
    {
        InitializeComponent();

        EnableHeadersVisualStyles = false;
        ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
        ColumnHeadersDefaultCellStyle.BackColor = LookAndFeel.Color.GridColumnHeader;
        ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        ColumnHeadersHeight -= 3;
        BackgroundColor = LookAndFeel.Color.GridCellBackground;
        BorderStyle = BorderStyle.None;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        MultiSelect = false;
        RowHeadersVisible = false;
        DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        RowTemplate.Resizable = DataGridViewTriState.False;
        AutoGenerateColumns = false;
        CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        DefaultCellStyle.Padding = new Padding(3, 0, 3, 0);
        DefaultCellStyle.Font = new Font("Arial", 8);
        RowTemplate.Height -= 3;
        BackgroundColor = LookAndFeel.Color.GridCellBackground;
        GridColor = LookAndFeel.Color.Grid;
        DefaultCellStyle.BackColor = LookAndFeel.Color.GridCellBackground;
        DefaultCellStyle.ForeColor = LookAndFeel.Color.GridCellForeground;
        DefaultCellStyle.SelectionBackColor = LookAndFeel.Color.GridCellSelectedBackground;
        DefaultCellStyle.SelectionForeColor = LookAndFeel.Color.GridCellSelectedForeground;
        DoubleBuffered = true;

        DataGridViewColumn column = new DataGridViewCheckBoxColumn
        {
            Name = FilterFieldDataTable.ColumnVisible,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        };
        Columns.Add(column);

        column = new DataGridViewTextBoxColumn
        {
            Name = FilterFieldDataTable.ColumnTag,
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        };
        Columns.Add(column);

        column = new DataGridViewTextBoxColumn
        {
            Name = FilterFieldDataTable.ColumnName,
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        };
        Columns.Add(column);
    }
}
