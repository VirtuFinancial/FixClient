/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Component.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Fix.Repository
{
    //
    //<Component added="FIX.4.0">
    //    <ComponentID>1001</ComponentID>
    //    <ComponentType>Block</ComponentType>
    //    <CategoryID>Session</CategoryID>
    //    <Name>StandardHeader</Name>
    //    <NotReqXML>1</NotReqXML>		
    //    <Description>The standard FIX message header</Description>
    //</Component>
    //
    //
    // TODO - make this enumerable etc or even better replace with a List<>.
    //
    [XmlRoot("Components")]
    public class Components
    {
        [XmlElement("Component")]
        public List<Component> Items { get; } = new List<Component>();
    }

    public class Component : ICloneable
    {
        public string ComponentID { get; set; }
        public string ComponentType { get; set; }
        public string CategoryID { get; set; }
        public string Name { get; set; }
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
