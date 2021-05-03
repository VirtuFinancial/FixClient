/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: LogParser.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.IO;

namespace Fix
{
    public abstract class LogParser
    {
        public bool Strict { get; set; }

        public MessageCollection Parse(Stream stream)
        {
            var messages = new MessageCollection();

            using (TextReader reader = new StreamReader(stream))
            {
                while (true)
                {
                    Message? message = ParseMessage(reader);

                    if (message == null)
                        break;

                    messages.Add(message);
                }
            }

            return messages;
        }

        public MessageCollection Parse(Uri uri)
        {
            if (uri.Scheme != "file")
                throw new ArgumentException("Exepected a URI with a 'file' scheme and got '{0}'", uri.Scheme);

            using FileStream stream = new(uri.LocalPath, FileMode.Open);
            return Parse(stream);
        }

        protected abstract Message? ParseMessage(TextReader reader);

    }
}
