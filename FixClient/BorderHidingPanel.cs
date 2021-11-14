/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: BorderHidingPanel.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System.Drawing;

namespace FixClient;

public partial class BorderHidingPanel : Panel
{
    readonly Control _control;

    public BorderHidingPanel(Control control)
    {
        InitializeComponent();
        _control = control;
        _control.Dock = DockStyle.None;
        Controls.Add(control);
        BorderStyle = BorderStyle.None;
        SizeChanged += BorderHidingPanelSizeChanged;
    }

    void BorderHidingPanelSizeChanged(object? sender, EventArgs e)
    {
        if (_control != null)
        {
            _control.Location = new Point(-1, -1);
            _control.Size = new Size(Size.Width + 2, Size.Height + 2);
        }
    }
}

