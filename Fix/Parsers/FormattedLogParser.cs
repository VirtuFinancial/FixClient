/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: FormattedLogParser.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Fix.Parsers
{
    public class FormattedLogParser : LogParser
    {
        //
        //	Glue/Gate Format
        //
        //  BeginString(   8): FIX.4.2
        //      MsgType(  35): A
        //    MsgSeqNum(  34): 1
        //
        //	ITGA libfix (DeskServer + QuoteJockey)
        //
        //  BeginString   (8) - FIX.4.0
        //   BodyLength   (9) - 142
        //      MsgType  (35) - 8
        //
        string _leftOvers;
        Dictionary.Version _version;

        protected bool ParseBody(TextReader reader, Message message)
        {
            while (true)
            {
                string line = string.IsNullOrEmpty(_leftOvers) ? reader.ReadLine() : _leftOvers;
                _leftOvers = null;

                if (line == null)
                    break;

                if (IsMessageTerminator(line) && message.Fields.Count > 0)
                    break;

                Match match = Regex.Match(line, @"^[<>]?.*\(\s*(\d+)\)\s*([:-])\s+(.*)");

                if (!match.Success)
                    continue;

                int tag = Convert.ToInt32(match.Groups[1].Value);
                string value = match.Groups[3].Value;

                // If we haven't loaded a version yet or if the version has changed. The version is unlikely to change but sometimes
                // in dev/testing this happens.
                if (tag == Dictionary.Fields.BeginString.Tag && (_version == null || _version.BeginString != value))
                {
                    _version = value == Dictionary.Versions.FIXT_1_1.BeginString ? Dictionary.Versions.Default : Dictionary.Versions[value];
                }

                //
                // The ITGA and some GATE log messages print a description for
                // enumerated types so we need to ignore that, the description is separated
                // by a '-' character. Only look for the description on known enumerated
                // types so we don't remove text with embedded '-' characters in Text
                // fields etc.
                //
                Dictionary.Field field;

                if (_version == null)
                {
                    Dictionary.Fields.TryGetValue(tag, out field);
                }
                else
                {
                    _version.Fields.TryGetValue(tag, out field);

                    if (field == null && _version == Dictionary.Versions.FIXT_1_1)
                    {

                    }
                }

                if (field != null && (field.EnumeratedType != null || field.Tag == Dictionary.Fields.MsgType.Tag))
                {
                    match = Regex.Match(value, @"\s*([a-zA-Z0-9]+)\s*-");

                    if (match.Success)
                    {
                        value = match.Groups[1].Value;
                    }
                }

                message.Fields.Add(new Field(tag, value.TrimEnd()));

                /*
                //
                // Gate has a generic attributes field with embedded '^B' characters, it
                // is handy to be able to cut/paste them so convert them in here.
                //
                std::string::size_type pos;

                while((pos = field_value.find("^B")) != std::string::npos)
                    field_value.replace(pos, 2, "\002");

                fix::field::field_id id = fix::field::field_id(field_id);
                std::string stripped = strip_whitespace(field_value);

                fix::field field(id, stripped);

                message->fields()->push_back(std::make_pair(field.id(), field));

                if(end == std::string::npos)
                    break;
                */
            }

            return message.Fields.Count > 0;
        }

        protected override Message ParseMessage(TextReader reader)
        {
            var message = new Message();

            message.Fields.Clear();

            bool foundStart = FindStart(reader, message);

            if (Strict && !foundStart)
                return null;

            if (!ParseBody(reader, message))
                return null;

            return message;
        }

        static bool IsMessageTerminator(string line)
        {
            string trimmed = line.Trim();
            return trimmed == "}" ||
                    trimmed.Contains(" Incoming ") ||
                    trimmed.Contains(" Outgoing ") ||
                    string.IsNullOrEmpty(trimmed.Trim());
        }

        bool FindStart(TextReader reader, Message message)
        {
            string previousLine = string.Empty;

            while (true)
            {
                string line = reader.ReadLine();

                if (line == null)
                    return false;

                if (line.Trim() == "{" || (line.Contains("): ") && (!line.Contains(" Detail ") && !line.Contains(" Message "))))
                {
                    message.Incoming = (previousLine.Contains("(received)") || previousLine.Contains("Incoming"));
                    _leftOvers = line;
                    break;
                }

                previousLine = line;
            }

            return true;
        }
    }
}
