/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Version.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Fix.Repository
{
    public class Version
    {
        public Version(string path, string beginString)
        {
            Root = path;
            BeginString = beginString;
            Scan();
        }

        public string Root { get; }
        public string BeginString { get; }
        public List<Message> Messages { get; } = new List<Message>();
        public SortedList<int, Field> Fields { get; } = new SortedList<int, Field>();
        public Dictionary<string, List<MsgContent>> MsgContents { get; } = new Dictionary<string, List<MsgContent>>();
        public Dictionary<string, Component> Components { get; } = new Dictionary<string, Component>();
        public Dictionary<int, List<Enum>> Enums { get; } = new Dictionary<int, List<Enum>>();
        public List<string> DataTypes { get; } = new List<string>();

        class DirectoryComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int left = Convert.ToInt32(x.Substring(2));
                int right = Convert.ToInt32(y.Substring(2));
                return left.CompareTo(right);
            }
        }

        public void Scan()
        {
            string directory = Root;
            string extension = "Base";

            string mostRecent = (from entry in Directory.EnumerateDirectories(Root)
                                 where Path.GetFileName(entry).ToUpper().StartsWith("EP")
                                 select Path.GetFileName(entry)).OrderByDescending(entry => entry, new DirectoryComparer()).FirstOrDefault();

            if(!string.IsNullOrEmpty(mostRecent))
            {
                extension = mostRecent;
            }
            
            directory += "/" + extension;
            
            var customMessages = new List<Message>();

            using (FileStream fs = new FileStream(directory + "/Messages.xml", FileMode.Open))
            {
                var ser = new XmlSerializer(typeof(Messages));
                var m = (Messages)ser.Deserialize(fs);
                foreach(Message message in m.Items)
                {
                    Messages.Add((Message)message.Clone());

                    // Add ITG extensions
                    if (BeginString == "FIX.4.0")
                    {
                        switch (message.MsgType)
                        {
                            case "E":
                                var kodiakWaveOrder = (Message)message.Clone();
                                kodiakWaveOrder.MsgType = "UWO";
                                kodiakWaveOrder.Name = "KodiakWaveOrder";
                                customMessages.Add(kodiakWaveOrder);
                                break;
                            case "G":
                                var kodiakWaveOrderCorrectionRequest = (Message)message.Clone();
                                kodiakWaveOrderCorrectionRequest.MsgType = "UWOCorrR";
                                kodiakWaveOrderCorrectionRequest.Name = "KodiakWaveOrderCorrectionRequest";
                                customMessages.Add(kodiakWaveOrderCorrectionRequest);
                                break;
                            case "F":
                                var kodiakWaveOrderCancelRequest = (Message)message.Clone();
                                kodiakWaveOrderCancelRequest.MsgType = "UWOCanR";
                                kodiakWaveOrderCancelRequest.Name = "KodiakWaveOrderCancelRequest";
                                customMessages.Add(kodiakWaveOrderCancelRequest);
                                break;
                            case "H":
                                var kodiakWaveOrderStatusRequest = (Message)message.Clone();
                                kodiakWaveOrderStatusRequest.MsgType = "UWOSR";
                                kodiakWaveOrderStatusRequest.Name = "KodiakWaveOrderStatusRequest";
                                customMessages.Add(kodiakWaveOrderStatusRequest);
                                break;
                            case "J":
                                var kodiakWaveAllocation = (Message)message.Clone();
                                kodiakWaveAllocation.MsgType = "UWALLOC";
                                kodiakWaveAllocation.Name = "KodiakWaveAllocation";
                                customMessages.Add(kodiakWaveAllocation);
                                break;
                        }
                    }
                }
            }

            Messages.AddRange(customMessages);

            using (FileStream fs = new FileStream(directory + "/Enums.xml", FileMode.Open))
            {
                var ser = new XmlSerializer(typeof(Enums));
                var m = (Enums)ser.Deserialize(fs);
                InjectHKEXExchangeTradeTypeEnum(m);
                foreach (Enum en in m.Items)
                {
                    var clone = (Enum)en.Clone();
                    List<Enum> values;
                    if (!Enums.TryGetValue(clone.Tag, out values))
                    {
                        values = new List<Enum>();
                        Enums[clone.Tag] = values;
                    }
                    values.Add(clone);
                }
            }

            using (FileStream fs = new FileStream(directory + "/Fields.xml", FileMode.Open))
            {
                var ser = new XmlSerializer(typeof(Fields));
                var m = (Fields)ser.Deserialize(fs);
                foreach (Field field in m.Items)
                {
                    var clone = (Field)field.Clone();

                    if (clone.Type.ToUpper() == "STIRNG")
                        clone.Type = "string";

                    Fields.Add(clone.Tag, clone);

                    if (!DataTypes.Contains(clone.Type, StringComparer.OrdinalIgnoreCase))
                    {
                        DataTypes.Add(clone.Type);
                    }
                }

                InjectHKEXExchangeTradeTypeField();
            }

            using (FileStream fs = new FileStream(directory + "/MsgContents.xml", FileMode.Open))
            {
                var ser = new XmlSerializer(typeof(MsgContents));
                var m = (MsgContents)ser.Deserialize(fs);
                foreach (MsgContent content in m.Items)
                {
                    var clone = (MsgContent)content.Clone();
                    List<MsgContent> contents;
                    if(!MsgContents.TryGetValue(clone.ComponentID, out contents))
                    {
                        contents = new List<MsgContent>();
                        MsgContents[clone.ComponentID] = contents;
                    }
                    contents.Add(clone);
                }
                InjectHKEXExchangeTradeTypeIntoMsgContents();
            }

            using (FileStream fs = new FileStream(directory + "/Components.xml", FileMode.Open))
            {
                var ser = new XmlSerializer(typeof(Components));
                var m = (Components)ser.Deserialize(fs);
                foreach (Component component in m.Items)
                {
                    var clone = (Component)component.Clone();
                    Components.Add(clone.Name, clone);
                }
            }
        }

        void InjectHKEXExchangeTradeTypeIntoMsgContents()
        {
            // Find the trade capture report
            Message tradeCaptureReport = Messages.FirstOrDefault(m => m.MsgType == "AE");

            if (tradeCaptureReport == null)
                return;

            // Find it's fields
            List<MsgContent> content;
            if (!MsgContents.TryGetValue(tradeCaptureReport.ComponentID, out content))
                return;

            var field = new MsgContent
            {
                Added = "HKEX",
                ComponentID = tradeCaptureReport.ComponentID,
                Description = "Exchange assigned trade type for a reported trade",
                Indent = "0",
                Position = content.Count.ToString(),
                Reqd = "0",
                TagText = "5681"
            };

            content.Insert(content.Count, field);
        }

        void InjectHKEXExchangeTradeTypeField()
        {
            if (BeginString?.StartsWith("FIX.5") ?? false)
            {
                // Inject custom HKEX field type
                var exchangeTradeType = new Field
                {
                    Tag = 5681,
                    Name = "ExchangeTradeType",
                    Added = "HKEX",
                    Type = "char",
                    Description = "Exchange assigned trade type for a reported trade"
                };

                if (Fields.Count >= exchangeTradeType.Tag)
                {
                    Fields[exchangeTradeType.Tag] = exchangeTradeType;
                }
                else
                {
                    Fields.Add(exchangeTradeType.Tag, exchangeTradeType);
                }
            }
        }

        void InjectHKEXExchangeTradeTypeEnum(Enums enums)
        {
            /*
                Inject this custom type used by the HKEX

                5681 ExchangeTradeType Char Exchange assigned trade type for a reported trade:
            
                M = Manual Trade
                S = Manual Non Standard price
                Q = Special Lot
                P = Odd Lot
                R = Previous Day
                V = Overseas
                E = Special Lot – Semi-Automatic
                O = Odd Lot – Semi-Automatic
            */
            var definitions = new Dictionary<string, string>
            {
                ["M"] = "Manual Trade",
                ["S"] = "Manual Non Standard Price",
                ["Q"] = "Special Lot",
                ["P"] = "Odd Lot",
                ["R"] = "Previous Day",
                ["V"] = "Overseas",
                ["E"] = "Special Lot - Semi - Automatic",
                ["O"] = "Odd Lot - Semi - Automatic"
            };

           // int tag = enums.Items.Max(e => e.Tag) + 1;

            foreach(var entry in definitions)
            {
                string symbolicName = entry.Value.Replace(" ", "").Replace("-", "");
                var value = new Enum
                {
                    Tag = 5681,
                    Added = "HKEX",
                    Value = entry.Key,
                    SymbolicName = symbolicName,
                    Description = entry.Value
                };
                enums.Items.Add(value);
            }

        }
    }
}
