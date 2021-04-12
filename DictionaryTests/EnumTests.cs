/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: EnumTests.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace DictionaryTests
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            Attribute attribute = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute));
            return attribute is not System.ComponentModel.DescriptionAttribute descriptionAttribute ? value.ToString() : descriptionAttribute.Description;
        }
    }

    [TestClass]
    public class EnumTests
    {
        [TestMethod]
        public void TestStringValue()
        {
            Assert.AreEqual("Buy", Enum.GetName(typeof(Fix.Dictionary.FIX_4_0.Side), Fix.Dictionary.FIX_4_0.Side.Buy));
        }

        [TestMethod]
        public void TestValue()
        {
            Assert.AreEqual('1', Convert.ToChar(Fix.Dictionary.FIX_4_0.Side.Buy));
            Assert.AreEqual('2', Convert.ToChar(Fix.Dictionary.FIX_4_0.Side.Sell));
        }

        [TestMethod]
        public void TestDescription()
        {
            Assert.AreEqual("Stay on offerside", Fix.Dictionary.FIX_4_0.ExecInst.StayOnOfferSide.GetDescription());
        }

    }
}
