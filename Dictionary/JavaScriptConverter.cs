/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: JavaScriptConverter.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Reflection;

namespace Fix
{
    public static partial class Dictionary
    {
        public class JavaScriptConverter : System.Web.Script.Serialization.JavaScriptConverter
        {
            readonly Type _type;
            List<Version> _versions;

            public JavaScriptConverter(Type type)
            {
                _type = type;
            }

            public override IEnumerable<Type> SupportedTypes => new[] { _type };

            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                object p = Activator.CreateInstance(_type);

                var props = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (string key in dictionary.Keys)
                {
                    var prop = props.FirstOrDefault(t => t.Name == key);
                   
                    if (prop == null)
                        continue;
                    
                    if (prop.PropertyType == typeof (Version))
                    {
                        if (_versions == null)
                        {
                            _versions = new List<Version>();
                            foreach (var version in Versions)
                            {
                                _versions.Add(version);
                            }
                        }
                            
                        foreach (var version in _versions)
                        {
                            var value = (string)dictionary[key];
                            if (version.BeginString == value)
                            {
                                prop.SetValue(p, version);
                                break;
                            }
                        }
                    }
                    else
                    {
                        prop.SetValue(p, dictionary[key], null);
                    }
                }

                return p;
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                var serialized = new Dictionary<string, object>();

                foreach (PropertyInfo property in _type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.GetCustomAttributes(typeof(ScriptIgnoreAttribute), false).Length > 0)
                        continue;

                    if (property.PropertyType == typeof (Version))
                    {
                        object value = property.GetValue(obj, null);
                        if (value != null)
                        {
                            serialized[property.Name] = ((Version) value).BeginString;
                        }
                    }
                    else
                    {
                        serialized[property.Name] = property.GetValue(obj, null);
                    }
                }

                return serialized;
            }
        }
    }
}
