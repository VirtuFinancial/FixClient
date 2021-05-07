/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Json.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fix
{
    public class Json
    {
        public class FixVersionConverter : JsonConverter
        {
            readonly List<Dictionary.Version> _versions = new();

            public FixVersionConverter()
            {
                foreach (var version in Dictionary.Versions)
                {
                    if (version.BeginString.StartsWith("FIX.5.0"))
                        continue;
                    _versions.Add(version);
                }
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Fix.Dictionary.Version);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.String)
                {
                    throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing Fix.Dictionary.Version.");
                }

                string value = reader.Value.ToString();

                return (from version in _versions where version.BeginString == value select version).FirstOrDefault();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }

                var version = (Dictionary.Version)value;
                writer.WriteValue(version.BeginString);
            }
        }
    }
}
