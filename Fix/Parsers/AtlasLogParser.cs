/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: AtlasLogParser.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Fix.Parsers
{
    public class AtlasLogParser : LogParser
    {
        //
        // An Atlas FIXENGINE, MCFIX log looks like this
        //
        // (11:44:31.07700) IN 8=FIX.4.2^A9=72^A35=A^A49=FIX_CLIENT^A56=FIX_ENGINE^A34=12^A52=20110114-00:44:30^A98=0^A108=60^A10=090^A
        // (11:44:31.86800) OUT 8=FIX.4.2^A9=75^A35=A^A34=1^A49=FIX_ENGINE^A56=FIX_CLIENT^A52=20110114-00:44:31.868^A98=0^A108=60^A10=000^A
        //
        protected override Message ParseMessage(TextReader reader)
        {
            string direction;
            string body = string.Empty;

            for (;;)
            {
                try
                {
                    string line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line))
                        return null;

                    Match match = Regex.Match(line, @"\(.*\)\s+([INOUT]*)\s+(.*)");

                    if (!match.Success)
                    {
                        body = string.Empty;
                        continue;
                    }

                    direction = match.Groups[1].Value;
                    body += match.Groups[2].Value;

                    if (body.Contains("10="))
                        break;
                }
                catch (Exception)
                {
                }
            }

            Message message;

            try
            {
                message = new Message(body)
                {
                    Incoming = direction == "IN"
                };
            }
            catch (Exception)
            {
                return null;
            }

            return message;
        }
    }
}
