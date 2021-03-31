/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: ResetForm.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FixClient
{
    public partial class ResetForm : Form
    {
        public ResetForm()
        {
            InitializeComponent();
            iconPictureBox.Image = SystemIcons.Question.ToBitmap();
        }

        public Fix.OrderBook.Retain Retain
        {
            get
            {
                Fix.OrderBook.Retain retain = Fix.OrderBook.Retain.None;

                if(retainActiveGtcOrdersCheckBox.Checked)
                {
                    retain |= Fix.OrderBook.Retain.ActiveGTC;
                }

                if (retainActiveGtdOrdersCheckBox.Checked)
                {
                    retain |= Fix.OrderBook.Retain.ActiveGTD;
                }

                return retain;
            }
        }

        public bool ResetGeneratedIds
        {
            get { return resetGeneratedIdsCheckBox.Checked; }
        }

    }
}
