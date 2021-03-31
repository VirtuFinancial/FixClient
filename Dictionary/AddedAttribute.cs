/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: AddedAttribute.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fix
{
    public class AddedAttribute : Attribute
    {
        public AddedAttribute(string added)
        {
            Added = added;
        }
        public string Added { get; }
    }
}
