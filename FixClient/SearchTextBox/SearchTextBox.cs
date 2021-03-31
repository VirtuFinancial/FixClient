/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: SearchTextBox.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FixClient
{
    class SearchTextBox : CueTextBox
    {
        public SearchTextBox()
        {
            Cue = "Search...";
            BackColor = LookAndFeel.Color.GridCellBackground;
            Button.FlatStyle = FlatStyle.Flat;
            Button.FlatAppearance.MouseOverBackColor = BackColor;
            Button.FlatAppearance.MouseDownBackColor = BackColor;
            Button.FlatAppearance.BorderSize = 0;
            Button.ForeColor = System.Drawing.Color.Gray;
            Button.Text = "X";
            Button.Size = new System.Drawing.Size(16, 16);
            Button.Click += (sender, ev) =>
            {
                if (string.IsNullOrEmpty(Text))
                {
                    OnTextChanged(null);
                }
                else
                {
                    Text = null;
                }
            };
        }
    }
}
