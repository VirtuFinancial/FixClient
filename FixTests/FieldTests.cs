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
using static Fix.Dictionary;

namespace FixTests
{
    [TestClass]
    public class FieldTests
    {
        [TestMethod]
        public void TestFieldValueDescriptionWithNumericEnumDefinition()
        {
            var field = new Fix.Field(FIX_5_0SP2.SessionStatus.SessionActive);
            Assert.AreEqual("SessionActive", field.ValueDescription);
        }

        /*
        [TestMethod]
        public void TestDescriptionOfExecInstWithMultipleValues()
        {
            var field = new Fix.Field(FIX_5_0SP2.Fields.ExecInst, "c x");
            Assert.AreEqual("Ignore price validity checks, Ignore notional value checks", field.ValueDescription);
        }
        */

        /*
        [TestMethod]
        public void TestExecInstWithMultipleValues()
        {
            var field = new Fix.Field(FIX_5_0SP2.Fields.ExecInst, "c x");
            var values = (Fix.ExecInst[])field;
            Assert.AreEqual(2, values.Length);
            Assert.AreEqual(FIX_5_0SP2.ExecInst.IgnorePriceValidityChecks, values[0]);
            Assert.AreEqual(FIX_5_0SP2.ExecInst.IgnoreNotionalValueChecks, values[1]);
        }
        */

        [TestMethod]
        public void TestIntTagConstructor()
        {
            var field = new Fix.Field(35, "D");
            Assert.AreEqual(35, field.Tag);
            Assert.AreEqual("D", field.Value);
        }

        [TestMethod]
        public void TestStringTagConstructor()
        {
            var field = new Fix.Field("35", "D");
            Assert.AreEqual(35, field.Tag);
            Assert.AreEqual("D", field.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidStringTagConstructor()
        {
            _ = new Fix.Field("blah", "D");
        }

        [TestMethod]
        public void TestClone()
        {
            var one = new Fix.Field("35", "D");
            var two = (Fix.Field)one.Clone();
            Assert.AreEqual(one.Tag, two.Tag);
            Assert.AreEqual(one.Value, two.Value);
            two.Value = "E";
            Assert.AreEqual(one.Tag, two.Tag);
            Assert.AreNotEqual(one.Value, two.Value);
            Assert.AreEqual("D", one.Value);
            Assert.AreEqual("E", two.Value);
        }

        [TestMethod]
        public void TestShortTimestampDateTimeConversion()
        {
            var field = new Fix.Field(FIX_5_0SP2.Fields.ExpireTime, "20140306-06:01:32");
            Assert.IsNotNull((DateTime?)field);
        }

        [TestMethod]
        public void TestLongTimestampDateTimeConversion()
        {
            var field = new Fix.Field(FIX_5_0SP2.Fields.ExpireTime, "20140306-06:01:32.345");
            Assert.IsNotNull((DateTime?)field);
        }

        [TestMethod]
        public void TestDateDateTimeConversion()
        {
            var field = new Fix.Field(FIX_5_0SP2.Fields.ExpireTime, "20140306");
            Assert.IsNotNull((DateTime?)field);
        }

        [TestMethod]
        public void TestInvalidDateTimeConversion()
        {
            var field = new Fix.Field(FIX_5_0SP2.Fields.ExpireTime, "blah");
            Assert.IsNull((DateTime?)field);
        }
    }
}
