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
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FixClient
{
    static class StringExtensions
    {
        public static string SplitInParts(this string stringToSplit, int maximumLineLength)
        {
            return Regex.Replace(stringToSplit, @"(.{1," + maximumLineLength + @"})(?:\s|$)", "$1\n");
        }
    }
}
