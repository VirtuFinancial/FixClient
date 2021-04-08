/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Message.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Fix.Repository
{
    //
    //<Message added="FIX.2.7">
    //    <ComponentID>1</ComponentID>
    //    <MsgType>0</MsgType>
    //    <Name>Heartbeat</Name>
    //    <CategoryID>Session</CategoryID>
    //    <SectionID>Session</SectionID>
    //    <NotReqXML>1</NotReqXML>
    //    <Description>The Heartbeat monitors the status of the communication link and identifies when the last of a string of messages was not received.</Description>
    //</Message>
    //
    // TODO - make this enumerable etc or even better replace with a List<>.
    //
    [XmlRoot("Messages")]
    public class Messages
    {
        [XmlElement("Message")]
        public List<Message> Items { get; } = new List<Message>();
    }

    public class Message : ICloneable
    {
        public string ComponentID { get; set; }
        public string MsgType { get; set; }
        public string Name { get; set; }
        public string CategoryID { get; set; }
        public string SectionID { get; set; }
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
