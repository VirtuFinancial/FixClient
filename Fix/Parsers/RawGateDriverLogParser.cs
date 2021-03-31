/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: RawGateDriverLogParser.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Fix.Parsers
{
    public class RawGateDriverLogParser : LogParser
    {
        //
        //  A Raw GATE Driver log looks like this
        // 
        //  21:51:50.533435 YSFIXDRV/em0.Y Message     CFIXemStateHandler.cxx:73 <Sender:YSDRV,Target:YSDEST> Leaving: Start --> Entering: InitiatingLogon {16 usec}
        //  21:51:50.538406 YSFIXDRV/em0.Y Message     OpenHandler::handleOpen(): setting TCP_NODELAY to file descriptor 76 {238 usec}
        //  21:51:50.538464 YSFIXDRV/em0.Y Message     <Sender:YSDRV,Target:YSDEST>CDefaultInitHandler::notifyTransportOpen() session opened transport connection. {16 usec}
        //  21:51:50.538833 YSFIXDRV/em0.Y Detail      Sending  8=FIX.4.2^A9=64^A35=A^A34=734^A49=YSDRV^A56=YSDEST^A52=20100915-04:51:50^A98=0^A108=30^A10=018^A {6 usec}
        //  21:51:50.539114 YSFIXDRV/em0.Y Detail      <YSDRV:YSDEST> Outgoing LOGON: {6 usec}
        //
        protected override Message ParseMessage(TextReader reader)
        {
            Message message;

            for (;;)
            {
                string line = reader.ReadLine();

                if(line == null)
                    return null;

                if (!line.Contains(" 8=FIX")) // This makes it faster - improve the regex?
                    continue;

                Match match = Regex.Match(line, @".*Detail\s+(.*ing)\s+(8=FIX.*)");

                if (!match.Success)
                    continue;

                string direction = match.Groups[1].Value;
                string body = match.Groups[2].Value;

                try
                {
                    message = new Message(body)
                    {
                        Incoming = direction == "Receiving"
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
