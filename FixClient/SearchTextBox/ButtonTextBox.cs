/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: ButtonTextBox.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FixClient;

public class ButtonTextBox : TextBox
{
    readonly Button _button;

    public event EventHandler ButtonClick { add { _button.Click += value; } remove { _button.Click -= value; } }

    public ButtonTextBox()
    {
        _button = new Button
        {
            Cursor = Cursors.Default
        };
        _button.SizeChanged += (o, e) => OnResize(e);
        Controls.Add(_button);
    }

    public Button Button
    {
        get { return _button; }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        _button.Size = new Size(_button.Width, ClientSize.Height + 2);
        _button.Location = new Point(ClientSize.Width - _button.Width, -1);
        // Send EM_SETMARGINS to prevent text from disappearing underneath the button
        SendMessage(Handle, 0xd3, (IntPtr)2, (IntPtr)(_button.Width << 16));
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
}

