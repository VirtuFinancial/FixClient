/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: NativeTabControl.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FixClient
{
    class NativeTabControl : NativeWindow
    {
        public NativeTabControl(Padding adjustPadding)
        {
            AdjustPadding = adjustPadding;
        }

        protected override void WndProc(ref Message m)
        {
            if ((m.Msg == TCM_ADJUSTRECT))
            {
                var rc = (RECT)m.GetLParam(typeof(RECT));
                //Adjust these values to suit, dependant upon Appearance 
                rc.Left += AdjustPadding.Left;
                rc.Right += AdjustPadding.Right;
                rc.Top += AdjustPadding.Top;
                rc.Bottom += AdjustPadding.Bottom;
                Marshal.StructureToPtr(rc, m.LParam, true);
            }
            base.WndProc(ref m);
        }

        Padding AdjustPadding { get; set; }
        const int TCM_FIRST = 0x1300;
        const int TCM_ADJUSTRECT = (TCM_FIRST + 40);
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

    }

}
