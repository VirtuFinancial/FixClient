/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Parser.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Fix
{
    public class Parser
    {
        public bool Strict { get; set; }

        public MessageCollection Parse(Stream stream)
        {
            long position = stream.Position;

            using (NonClosingStreamDecorator decorator = new NonClosingStreamDecorator(stream))
            {
#pragma warning disable RECS0091 // Use 'var' keyword when possible
                LogParser parser = new Parsers.RawGateCiLogParser { Strict = Strict };
#pragma warning restore RECS0091 // Use 'var' keyword when possible
                MessageCollection result = parser.Parse(decorator);
                if (result.Count > 0)
                    return result;

                stream.Seek(position, SeekOrigin.Begin);

                parser = new Parsers.RawGateDriverLogParser { Strict = Strict };
                result = parser.Parse(decorator);
                if (result.Count > 0)
                    return result;

                stream.Seek(position, SeekOrigin.Begin);

                parser = new Parsers.FormattedLogParser { Strict = Strict };
                result = parser.Parse(decorator);
                if (result.Count > 0)
                    return result;

                stream.Seek(position, SeekOrigin.Begin);

                parser = new Parsers.KlsLogParser { Strict = Strict };
                result = parser.Parse(decorator);
                if (result.Count > 0)
                    return result;

                stream.Seek(position, SeekOrigin.Begin);

                parser = new Parsers.AtlasLogParser { Strict = Strict };
                result = parser.Parse(decorator);
                if (result.Count > 0)
                    return result;
            }

            stream.Seek(position, SeekOrigin.Begin);

            var messages = new MessageCollection();

            using (Reader reader = new Reader(stream))
            {
                reader.ValidateDataFields = false;

                for (;;)
                {
                    try
                    {
                        Message message = reader.ReadLine();
                        if (message == null)
                            break;
                        messages.Add(message);
                    }
                    catch (Exception)
                    {
                        // TODO - report errors
                        reader.DiscardLine();
                        continue;
                    }
                }
            }

            return messages;
        }

        public MessageCollection Parse(Uri uri)
        {
            if(uri.Scheme != "file")
                throw new ArgumentException("Exepected a URI with a 'file' scheme and got '{0}'", uri.Scheme);

            using (FileStream stream = new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return Parse(stream);
            }
        }
    }
}
