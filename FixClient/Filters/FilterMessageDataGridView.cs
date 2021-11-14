/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FilterMessageDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System.Drawing;

namespace FixClient;

public sealed partial class FilterMessageDataGridView : DataGridView
{
    public FilterMessageDataGridView()
    {
        EnableHeadersVisualStyles = false;
        ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
        ColumnHeadersDefaultCellStyle.BackColor = LookAndFeel.Color.GridColumnHeader;
        ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        ColumnHeadersHeight -= 3;
        Dock = DockStyle.Fill;
        BackgroundColor = Color.WhiteSmoke;
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
        DefaultCellStyle.Font = new Font("Arial", 8);
        DefaultCellStyle.Padding = new Padding(3, 0, 3, 0);
        RowTemplate.Height -= 3;
        GridColor = LookAndFeel.Color.Grid;
        DefaultCellStyle.BackColor = LookAndFeel.Color.GridCellBackground;
        DefaultCellStyle.ForeColor = LookAndFeel.Color.GridCellForeground;
        DefaultCellStyle.SelectionBackColor = LookAndFeel.Color.GridCellSelectedBackground;
        DefaultCellStyle.SelectionForeColor = LookAndFeel.Color.GridCellSelectedForeground;
        DoubleBuffered = true;

        DataGridViewColumn column = new DataGridViewCheckBoxColumn
        {
            AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
            Name = "Visible"
        };
        Columns.Add(column);

        column = new DataGridViewTextBoxColumn
        {
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
            Name = "Type",
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        Columns.Add(column);

        column = new DataGridViewTextBoxColumn
        {
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            Name = "Name"
        };
        Columns.Add(column);

        InitializeComponent();
    }
}
