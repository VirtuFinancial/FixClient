/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: MsgContent.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Fix.Repository
{
    //
    //<MsgContent added="FIX.2.7">
    //    <ComponentID>1</ComponentID>
    //    <TagText>StandardHeader</TagText>
    //    <Indent>0</Indent>
    //    <Position>1</Position>
    //    <Reqd>1</Reqd>
    //    <Description>MsgType = 0</Description>
    //</MsgContent>
    //
    //
    // TODO - make this enumerable etc or even better replace with a List<>.
    //
    [XmlRoot("MsgContents")]
    public class MsgContents
    {
        [XmlElement("MsgContent")]
        public List<MsgContent> Items { get; } = new List<MsgContent>();
    }

    public class MsgContent : ICloneable
    {
        public string ComponentID { get; set; }
        public string TagText { get; set; }
        public string Indent { get; set; }
        public string Position { get; set; }
        public string Reqd { get; set; }
        public string Description { get; set; }

        [XmlAttribute("added")]
        public string Added { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
