/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: SessionTransportTests.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FixTests
{
    [TestClass]
    public class SessionTransportTests : SessionTestsBase<Fix.Session>
    {
        #region Setup
        [TestInitialize]
        public void TestInitialize()
        {
            Initialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Cleanup();
        }
        #endregion

       
    }
}
