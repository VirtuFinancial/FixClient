/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: HistoryMessageDataGridView.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System.Drawing;
using System.Windows.Forms;

namespace FixClient;

public sealed partial class HistoryMessageDataGridView : DataGridView
{
    public HistoryMessageDataGridView()
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
        GridColor = LookAndFeel.Color.Grid;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        DefaultCellStyle.Font = new Font("Arial", 8);
        ReadOnly = true;
        CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        DefaultCellStyle.BackColor = LookAndFeel.Color.GridCellBackground;
        DefaultCellStyle.Padding = new Padding(3, 0, 3, 0);
        DefaultCellStyle.SelectionBackColor = LookAndFeel.Color.GridCellSelectedBackground;
        DefaultCellStyle.SelectionForeColor = LookAndFeel.Color.GridCellSelectedForeground;
        DoubleBuffered = true;
        RowTemplate.Height -= 3;
        ShowCellToolTips = false;
        AutoGenerateColumns = false;

        DataGridViewColumn column = new DataGridViewTextBoxColumn
        {
            Name = MessageDataTable.ColumnSendingTime,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        };

        Columns.Add(column);

        column = new DataGridViewTextBoxColumn
        {
            Name = MessageDataTable.ColumnMsgSeqNum,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
        };

        Columns.Add(column);

        column = new DataGridViewImageColumn
        {
            Name = MessageDataTable.ColumnStatusImage,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            DefaultCellStyle = { NullValue = null },
            HeaderCell = new DataGridViewImageColumnHeaderCell
            {
                Image = Properties.Resources.MessageStatusInfo,
                Value = null
            },
            //DefaultHeaderCellType = { HeaderCell.GetType() }
        };

        Columns.Add(column);

        column = new DataGridViewTextBoxColumn
        {
            Name = MessageDataTable.ColumnMsgTypeDescription,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        };

        Columns.Add(column);
    }
}
