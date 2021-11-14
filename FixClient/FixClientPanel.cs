/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FixClientPanel.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
namespace FixClient;

public partial class FixClientPanel : ToolStripContainer
{
    ToolStripMenuItem? _menuStrip;

    public FixClientPanel()
    {
        InitializeComponent();
        TopToolStripPanel.BackColor = LookAndFeel.Color.ToolStrip;
    }

    public ToolStripMenuItem? ToolStripMenuItem
    {
        get
        {
            return _menuStrip;
        }
    }

    /*
    protected void SetToolStrip(ToolStrip value)
    {
        TopToolStripPanel.Join(value);            
    }
    */

    protected void SetMenuStrip(ToolStripMenuItem value)
    {
        _menuStrip = value;
    }

}

