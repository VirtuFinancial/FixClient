/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FieldTests.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DictionaryTests
{
    [TestClass]
    public class FieldTests
    {
        [TestMethod]
        public void TestFieldLookupByIndex()
        {
            Fix.Dictionary.Field field = Fix.Dictionary.Fields[0];
            Assert.IsNotNull(field);
            Assert.AreEqual(1, field.Tag);
            Assert.AreEqual("Account", field.Name);
        }

        [TestMethod]
        public void TestFieldLookupByTag()
        {
            Fix.Dictionary.Field field = Fix.Dictionary.Fields["1"];
            Assert.IsNotNull(field);
            Assert.AreEqual(1, field.Tag);
            Assert.AreEqual("Account", field.Name);
        }

        [TestMethod]
        public void TestFieldLookupByName()
        {
            Fix.Dictionary.Field field = Fix.Dictionary.Fields["OrderQty"];
            Assert.IsNotNull(field);
            Assert.AreEqual(38, field.Tag);
            Assert.AreEqual("OrderQty", field.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestFieldLookupByNameNonFieldPropertyIsNotReflected()
        {
            _ = Fix.Dictionary.Fields["Count"];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestFieldLookupByInvalidName()
        {
            _ = Fix.Dictionary.Fields["VybongBysanton"];
        }

        [TestMethod]
        public void TestLowerBound()
        {
            Assert.IsNotNull(Fix.Dictionary.Fields["1"]);
        }

        [TestMethod]
        public void TestUpperBound()
        {
            Assert.IsNotNull(Fix.Dictionary.Fields["140"]);
        }

        [TestMethod]
        public void TestUpperBoundByIndex()
        {
            Assert.IsNull(Fix.Dictionary.Fields[6505]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestOutOfRangeMin()
        {
            _ = Fix.Dictionary.Fields["0"];
        }

        [TestMethod]
        public void TestGap()
        {
            Assert.IsNull(Fix.Dictionary.Fields["101"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestOutOfRangeMax()
        {
            _ = Fix.Dictionary.FIX_4_0.Fields["141"];
        }

        [TestMethod]
        public void TestExplicitFieldReference()
        {
            Fix.Dictionary.Field IDSource = Fix.Dictionary.FIX_4_0.Fields.IDSource;
            Assert.IsNotNull(IDSource);
        }
    }
}
