/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: VersionTests.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DictionaryTests
{
    [TestClass]
    public class VersionTests
    {
        [TestMethod]
        public void TestVersionsNumericIndex()
        {
            Assert.AreEqual(9, Fix.Dictionary.Versions.Count);
            Assert.AreEqual("FIX.4.0", Fix.Dictionary.Versions[0].BeginString);
            Assert.AreEqual("FIX.4.1", Fix.Dictionary.Versions[1].BeginString);
            Assert.AreEqual("FIX.4.2", Fix.Dictionary.Versions[2].BeginString);
            Assert.AreEqual("FIX.4.3", Fix.Dictionary.Versions[3].BeginString);
            Assert.AreEqual("FIX.4.4", Fix.Dictionary.Versions[4].BeginString);
            Assert.AreEqual("FIX.5.0", Fix.Dictionary.Versions[5].BeginString);
            Assert.AreEqual("FIX.5.0SP1", Fix.Dictionary.Versions[6].BeginString);
            Assert.AreEqual("FIX.5.0SP2", Fix.Dictionary.Versions[7].BeginString);
            Assert.AreEqual("FIXT.1.1", Fix.Dictionary.Versions[8].BeginString);
        }

        [TestMethod]
        public void TestNonExistentVersion()
        {
            Assert.IsNull(Fix.Dictionary.Versions["Fix.23.45"]);
        }

        [TestMethod]
        public void TestVersionsBeginStringIndex()
        {
            Assert.AreEqual("FIX.4.0", Fix.Dictionary.Versions["FIX.4.0"].BeginString);
            Assert.AreEqual("FIX.4.1", Fix.Dictionary.Versions["FIX.4.1"].BeginString);
            Assert.AreEqual("FIX.4.2", Fix.Dictionary.Versions["FIX.4.2"].BeginString);
            Assert.AreEqual("FIX.4.3", Fix.Dictionary.Versions["FIX.4.3"].BeginString);
            Assert.AreEqual("FIX.4.4", Fix.Dictionary.Versions["FIX.4.4"].BeginString);
            Assert.AreEqual("FIX.5.0", Fix.Dictionary.Versions["FIX.5.0"].BeginString);
            Assert.AreEqual("FIX.5.0SP1", Fix.Dictionary.Versions["FIX.5.0SP1"].BeginString);
            Assert.AreEqual("FIX.5.0SP2", Fix.Dictionary.Versions["FIX.5.0SP2"].BeginString);
            Assert.AreEqual("FIXT.1.1", Fix.Dictionary.Versions["FIXT.1.1"].BeginString);
        }

        [TestMethod]
        public void TestVersionMessages()
        {
            Assert.AreEqual(32, Fix.Dictionary.Versions[0].Messages.Count);
            Assert.AreEqual(32, Fix.Dictionary.Versions["FIX.4.0"].Messages.Count);
        }

        [TestMethod]
        public void TestIsEqual()
        {
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_4_0, Fix.Dictionary.Versions[0]);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_4_1, Fix.Dictionary.Versions[1]);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_4_2, Fix.Dictionary.Versions[2]);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_4_3, Fix.Dictionary.Versions[3]);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_4_4, Fix.Dictionary.Versions[4]);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_5_0, Fix.Dictionary.Versions[5]);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_5_0SP1, Fix.Dictionary.Versions[6]);
            Assert.AreEqual(Fix.Dictionary.Versions.FIX_5_0SP2, Fix.Dictionary.Versions[7]);
            Assert.AreEqual(Fix.Dictionary.Versions.FIXT_1_1, Fix.Dictionary.Versions[8]);  
        }

        [TestMethod]
        public void TestEqualsOperator()
        {
            Assert.IsTrue(Fix.Dictionary.Versions.FIX_4_0 == Fix.Dictionary.Versions[0]);
            Assert.IsTrue(Fix.Dictionary.Versions.FIX_4_1 == Fix.Dictionary.Versions[1]);
            Assert.IsTrue(Fix.Dictionary.Versions.FIX_4_2 == Fix.Dictionary.Versions[2]);
            Assert.IsTrue(Fix.Dictionary.Versions.FIX_4_3 == Fix.Dictionary.Versions[3]);
            Assert.IsTrue(Fix.Dictionary.Versions.FIX_4_4 == Fix.Dictionary.Versions[4]);
            Assert.IsTrue(Fix.Dictionary.Versions.FIX_5_0 == Fix.Dictionary.Versions[5]);
            Assert.IsTrue(Fix.Dictionary.Versions.FIX_5_0SP1 == Fix.Dictionary.Versions[6]);
            Assert.IsTrue(Fix.Dictionary.Versions.FIX_5_0SP2 == Fix.Dictionary.Versions[7]);
            Assert.IsTrue(Fix.Dictionary.Versions.FIXT_1_1 == Fix.Dictionary.Versions[8]);
        }

        [TestMethod]
        public void TestNotEqualsOperator()
        {
            Assert.IsFalse(Fix.Dictionary.Versions.FIX_4_0 != Fix.Dictionary.Versions[0]);
            Assert.IsFalse(Fix.Dictionary.Versions.FIX_4_1 != Fix.Dictionary.Versions[1]);
            Assert.IsFalse(Fix.Dictionary.Versions.FIX_4_2 != Fix.Dictionary.Versions[2]);
            Assert.IsFalse(Fix.Dictionary.Versions.FIX_4_3 != Fix.Dictionary.Versions[3]);
            Assert.IsFalse(Fix.Dictionary.Versions.FIX_4_4 != Fix.Dictionary.Versions[4]);
            Assert.IsFalse(Fix.Dictionary.Versions.FIX_5_0 != Fix.Dictionary.Versions[5]);
            Assert.IsFalse(Fix.Dictionary.Versions.FIX_5_0SP1 != Fix.Dictionary.Versions[6]);
            Assert.IsFalse(Fix.Dictionary.Versions.FIX_5_0SP2 != Fix.Dictionary.Versions[7]);
            Assert.IsFalse(Fix.Dictionary.Versions.FIXT_1_1 != Fix.Dictionary.Versions[8]);
        }
    }
}
