/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Field.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Fix.Repository
{
    //
    //<Field added="FIX.2.7">
    //    <Tag>1</Tag>
    //    <Name>Account</Name>
    //    <Type>char</Type>
    //    <NotReqXML>1</NotReqXML>
    //    <Description>Account mnemonic as agreed between broker and institution.</Description>
    //</Field>
    //
    //
    // TODO - make this enumerable etc or even better replace with a List<>.
    //
    [XmlRoot("Fields")]
    public class Fields
    {
        [XmlElement("Field")]
        public List<Field> Items { get; } = new List<Field>();
    }

    public class Field : ICloneable
    {
        public int Tag { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string NotReqXML { get; set; }
        public string Description { get; set; }

        [XmlAttribute("added")]
        public string Added { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
