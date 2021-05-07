/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Program.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace DictionaryPerformanceTest
{
    class Counters
    {
        public int Messages { get; set; }
        public int UniqueFields { get; set; }
        public int NullUniqueFields { get; set; }
        public int TotalFields { get; set; }
        public int NullTotalFields { get; set; }
        public Dictionary<string, int> MessageFields = new();
        public Dictionary<string, TimeSpan> MessageFieldsIterationTimes = new();
    }

    class Program
    {
        static readonly Dictionary<string, Counters> Counters = new();

        static void CountMessages(Fix.Dictionary.Version version)
        {
            Counters counters = Counters[version.BeginString];
            foreach (var _ in version.Messages)
            {
                ++counters.Messages;
            }
        }

        static void CountUniqueFields(Fix.Dictionary.Version version)
        {
            Counters counters = Counters[version.BeginString];
            foreach (var field in version.Fields)
            {
                if (field == null)
                    ++counters.NullUniqueFields;
                else
                    ++counters.UniqueFields;
            }
        }

        static void CountTotalFields(Fix.Dictionary.Version version)
        {
            Counters counters = Counters[version.BeginString];
            foreach (var message in version.Messages)
            {
                foreach (var field in message.Fields)
                {
                    if (field == null)
                        ++counters.NullTotalFields;
                    else
                        ++counters.TotalFields;
                }
            }
        }

        static void CountMessageFields(Fix.Dictionary.Version version)
        {
            Counters counters = Counters[version.BeginString];
            foreach (var message in version.Messages)
            {
                var watch = new Stopwatch();
                watch.Start();
                int count = message.FieldCount;
                watch.Stop();
                counters.MessageFields[message.Name] = count;
                counters.MessageFieldsIterationTimes[message.Name] = watch.Elapsed;
            }
        }

        static void RandomlyAccessMessageFields()
        {
            Fix.Dictionary.Message message = Fix.Dictionary.FIX_5_0SP2.Messages.ExecutionReport;

            var random = new Random();

            for (int i = 0; i < 100000; ++i)
            {
                int index = random.Next(message.FieldCount - 1);
                _ = message.Fields[index];
            }
        }

        static void InstantiateMessage()
        {
            //Fix.Dictionary.Message message = new Fix.Dictionary.FIX_5_0SP2.ExecutionReport();
            //var field = message.Fields;
            /*
            for (int i = 0; i < 3000; ++i)
            {
                Fix.Dictionary.Message message = new Fix.Dictionary.FIX_5_0SP2.ExecutionReport();
                var field = message.Fields;
            }
             */
        }

        delegate void ActionDelegate();

        static void TimeOperation(ActionDelegate action, string label)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            Console.WriteLine("{0} - {1}", stopwatch.Elapsed, label);
        }

        static void Main()
        {
            /*
            TimeOperation(InstantiateMessage, "Instantiate Message");

            Console.ReadKey();


            for (int i = 0; i < 5; ++i)
            {
                TimeOperation(RandomlyAccessMessageFields,
                    "Randomly access message fields");
            }

            Console.ReadKey();
            */

            TimeOperation(() =>
            {
                Fix.Dictionary.Message message = Fix.Dictionary.Messages.ExecutionReport;
                foreach (var field in message.Fields)
                {
                }
            },
            "FIX.5.0SP2 ExecutionReport Fields");

            TimeOperation(() =>
            {
                Fix.Dictionary.Message message = Fix.Dictionary.Messages.ExecutionReport;
                foreach (var field in message.Fields)
                {
                }
            },
"FIX.5.0SP2 ExecutionReport Fields");

            TimeOperation(() =>
            {
                Fix.Dictionary.Message message = Fix.Dictionary.Messages.PartyRiskLimitCheckRequestAck
;
                foreach (var field in message.Fields)
                {
                }
            },
"FIX.5.0SP2 PartyRiskLimitCheckRequestAck Fields");

            TimeOperation(() =>
            {
                Fix.Dictionary.Message message = Fix.Dictionary.Messages.PartyRiskLimitCheckRequestAck
;
                foreach (var field in message.Fields)
                {
                }
            },
"FIX.5.0SP2 PartyRiskLimitCheckRequestAck Fields");

            TimeOperation(() =>
            {
                foreach (var version in Fix.Dictionary.Versions)
                {
                    Counters[version.BeginString] = new Counters();
                }
            },
            "Iterate over versions");

            foreach (var version in Fix.Dictionary.Versions)
            {
                TimeOperation(() => CountMessages(version), string.Format("Iterate over messages in {0}", version.BeginString));
            }

            foreach (var version in Fix.Dictionary.Versions)
            {
                TimeOperation(() => CountMessageFields(version), string.Format("Iterate over message fields in {0}", version.BeginString));
            }

            foreach (var version in Fix.Dictionary.Versions)
            {
                TimeOperation(() => CountUniqueFields(version), string.Format("Iterate over fields in {0}", version.BeginString));
            }

            foreach (var version in Fix.Dictionary.Versions)
            {
                TimeOperation(() => CountTotalFields(version), string.Format("Iterate over total fields in {0}", version.BeginString));
            }

            using StreamWriter writer = new("C:\\workspace\\FixDictionaryStats.csv", false);
            writer.WriteLine("Version,Messages,UniqueFields,TotalFields,NullUniqueFields,NullTotalFields");

            foreach (var counter in Counters)
            {
                writer.WriteLine("{0},{1},{2},{3},{4},{5}",
                                  counter.Key,
                                  counter.Value.Messages,
                                  counter.Value.UniqueFields,
                                  counter.Value.TotalFields,
                                  counter.Value.NullUniqueFields,
                                  counter.Value.NullTotalFields);
            }

            writer.WriteLine();

            var builder = new StringBuilder("Message,");

            foreach (var counter in Counters)
            {
                builder.AppendFormat("{0},,", counter.Key);
            }

            writer.WriteLine(builder);

            var buffers = new Dictionary<string, StringBuilder>();

            foreach (var counter in Counters)
            {
                foreach (var message in counter.Value.MessageFields.Keys)
                {
                    buffers[message] = new StringBuilder();
                }
            }

            foreach (var counter in Counters)
            {
                foreach (var item in buffers)
                {
                    if (counter.Value.MessageFields.TryGetValue(item.Key, out int count))
                    {
                        if (counter.Value.MessageFieldsIterationTimes.TryGetValue(item.Key, out TimeSpan time))
                        {
                            item.Value.AppendFormat("{0},{1},", count, time.TotalMilliseconds);
                        }
                    }
                    else
                    {
                        item.Value.AppendFormat(",,");
                    }
                }
            }

            foreach (var buffer in buffers)
            {
                writer.WriteLine(buffer.Key + "," + buffer.Value);
            }
        }
    }
}
