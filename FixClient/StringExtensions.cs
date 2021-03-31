/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: StringExtensions.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;

namespace FixClient
{
    static class StringExtensions
    {
        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            int length;
            for (var i = 0; i < s.Length; i += length)
            {
                int next = i + Math.Min(partLength, s.Length - i);
                while (next < s.Length && s[next++] != ' ') { }
                length = next - i;
                yield return s.Substring(i, length);
            }
        }
    }
}
