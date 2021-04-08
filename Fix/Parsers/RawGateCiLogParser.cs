/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: RawGateCiLogParser.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Fix.Parsers
{
    public class RawGateCiLogParser : LogParser
    {
        //
        //  A Raw GATE CI log looks like this
        // 
        //  00:30:03.315698 YSDEST         Detail      Connection ITGI is ready {6 usec}
        //  00:30:03.315710 YSDEST         Detail      DC:connectionReady for connection : ITGI {3 usec}
        //  00:30:03.315722 YSDEST         Message     User YSDEST(155) Status Registered {4 usec}
        //  00:30:03.412008 YSDEST         Message     The destination YSDEST2 has the new status OK. {4 usec}
        //  00:30:30.364677 YSDEST         Detail      Outgoing FIX RAW 8=FIX.4.2^A9=49^A35=0^A34=3^A49=YSDEST^A56=ITGI^A52=20100915-07:30:30^A10=019^A {19 usec}
        //  00:30:30.365177 YSDEST         Detail      Incoming FIX RAW 8=FIX.4.2^A9=49^A35=0^A34=3^A49=ITGI^A56=YSDEST^A52=20100915-07:30:30^A10=019^A {267 usec}
        //  00:31:00.642240 YSDEST         Detail      Incoming FIX RAW 8=FIX.4.2^A9=49^A35=0^A34=4^A49=ITGI^A56=YSDEST^A52=20100915-07:31:00^A10=018^A {10 usec}
        //  00:31:00.642657 YSDEST         Detail      Outgoing FIX RAW 8=FIX.4.2^A9=49^A35=0^A34=4^A49=YSDEST^A56=ITGI^A52=20100915-07:31:00^A10=018^A {237 usec}
        //
        protected override Message ParseMessage(TextReader reader)
        {
            Message message;

            for (; ; )
            {
                string line = reader.ReadLine();

                if (line == null)
                    return null;

                if (!line.Contains("FIX RAW")) // This makes it faster - improve the regex?
                    continue;

                Match match = Regex.Match(line, @".*\s+(.*) FIX RAW (.*)");

                if (!match.Success)
                    continue;

                string direction = match.Groups[1].Value;
                string body = match.Groups[2].Value;

                try
                {
                    message = new Message(body)
                    {
                        Incoming = direction == "Incoming"
                    };
                }
                catch (Exception)
                {
                    continue;
                }

                break;
            }

            return message;
        }
    }
}
