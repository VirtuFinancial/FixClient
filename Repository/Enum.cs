/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Enum.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Fix.Repository
{
    //
    //  <Enum added="FIX.2.7">
    //      <Tag>4</Tag>
    //      <Value>B</Value>
    //      <SymbolicName>Buy</SymbolicName>
    //      <Description>Buy</Description>
    //  </Enum>
    //
    // TODO - make this enumerable etc or even better replace with a List<>.
    //
    [XmlRoot("Enums")]
    public class Enums
    {
        [XmlElement("Enum")]
        public List<Enum> Items { get; } = new List<Enum>();
    }

    public class Enum : ICloneable
    {
        public int Tag { get; set; }
        public string Value { get; set; }
        public string SymbolicName { get; set; }
        public string Description { get; set; }

        [XmlAttribute("added")]
        public string Added { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
