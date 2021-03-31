/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CustomFieldCollection.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml;

namespace FixClient
{
    class CustomFieldCollection
    {
        readonly Dictionary<string, CustomFieldCategory> _fields = new Dictionary<string, CustomFieldCategory>();

        public CustomFieldCollection(string filename)
        {
            Load(filename);
        }

        public void Load(string filename)
        {
            using (XmlTextReader reader = new XmlTextReader(filename))
            {
                while (!reader.EOF)
                {
                    reader.MoveToContent();

                    if (reader.Name != "Field")
                    {
                        reader.Read();
                        continue;
                    }

                    string name = reader.GetAttribute("name");
                    int id = Convert.ToInt32(reader.GetAttribute("id"));
                    string[] categories = reader.GetAttribute("categories").Split(',');

                    foreach (string c in categories)
                    {
                        string category = c.Trim();

                        if(category == String.Empty)
                            continue;

                        if (!_fields.ContainsKey(category))
                        {
                            _fields[category] = new CustomFieldCategory(category);
                        }

                        _fields[category].Add(new CustomField { Tag = id, Name = name });
                    }

                    reader.Read();
                }
            }
        }

        public Dictionary<string, CustomFieldCategory> Fields
        {
            get 
            { 
                return _fields; 
            }
        }
        
    }
}
