/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: LookAndFeel.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
namespace FixClient;

static class LookAndFeel
{
    public static class Color
    {
        public static readonly System.Drawing.Color ToolStrip = System.Drawing.Color.FromArgb(255, 232, 232, 236);

        public static readonly System.Drawing.Color FormBackLight = System.Drawing.Color.FromArgb(255, 218, 224, 231);
        public static readonly System.Drawing.Color FormBackDark = System.Drawing.Color.FromArgb(255, 193, 210, 231);

        public static readonly System.Drawing.Color Bid = System.Drawing.Color.FromArgb(255, 0, 70, 204);
        public static readonly System.Drawing.Color Ask = System.Drawing.Color.FromArgb(255, 250, 50, 50);
        public static readonly System.Drawing.Color Unknown = System.Drawing.Color.SaddleBrown;

        public static readonly System.Drawing.Color New = System.Drawing.Color.FromArgb(255, 0, 128, 0);
        public static readonly System.Drawing.Color Rejected = System.Drawing.Color.FromArgb(255, 250, 50, 50);
        public static readonly System.Drawing.Color Pending = System.Drawing.Color.FromArgb(255, 254, 90, 27);

        public static readonly System.Drawing.Color GridColumnHeader = System.Drawing.Color.FromArgb(255, 0, 122, 204);
        public static readonly System.Drawing.Color GridCellBackground = System.Drawing.Color.FromArgb(255, 250, 250, 250);
        public static readonly System.Drawing.Color GridCellForeground = System.Drawing.Color.FromArgb(255, 37, 37, 37);
        public static readonly System.Drawing.Color Grid = System.Drawing.Color.FromArgb(255, 239, 239, 242);
        public static readonly System.Drawing.Color GridCellSelectedBackground = System.Drawing.Color.FromArgb(255, 0, 122, 204);
        public static readonly System.Drawing.Color GridCellSelectedForeground = System.Drawing.Color.WhiteSmoke;

        public static readonly System.Drawing.Color Custom = System.Drawing.Color.Brown;
        public static readonly System.Drawing.Color Incoming = System.Drawing.Color.FromArgb(255, 0, 128, 0);
        public static readonly System.Drawing.Color Outgoing = System.Drawing.Color.FromArgb(255, 0, 70, 204);

        public static readonly System.Drawing.Color LogWarningBackColor = System.Drawing.Color.FromArgb(255, 254, 90, 27);
        public static readonly System.Drawing.Color LogWarningForeColor = System.Drawing.Color.White;

        public static readonly System.Drawing.Color LogErrorBackColor = System.Drawing.Color.Red;
        public static readonly System.Drawing.Color LogErrorForeColor = System.Drawing.Color.White;
    }
}

