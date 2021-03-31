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

﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace DictionaryTests
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
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
