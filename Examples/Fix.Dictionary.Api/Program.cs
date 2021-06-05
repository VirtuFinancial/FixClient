using System;
using System.Linq;
using static Fix.Dictionary;

namespace Fix.Examples
{
    class Program
    {
        static void Main()
        {
            EnumerateFixVersions();
            ReferenceSpecificVersion();
            LookupSpecificVersion();
            EnumerateVersionMessages();
            LookupMessageByMsgType();
            EnumerateVersionFields();
            LookupFieldByTag();
            EnumerateVersionDataTypes();
            EnumerateMessageFields();
        }

        static void EnumerateFixVersions()
        {
            Console.Write("\nEnumerate FIX versions");
            foreach (var version in Versions)
            {
                Console.WriteLine($"{version.BeginString} has {version.Messages.Length} Messages, Maximum Field Tag {version.Fields.MaxTag}, and {version.DataTypes.Count} DataTypes");
            }
        }

        static void ReferenceSpecificVersion()
        {
            Console.WriteLine("\nReference a specific FIX version");
            var version = Versions.FIX_5_0SP2;
            Console.WriteLine($"{version.BeginString} has {version.Messages.Length} Messages, Maximum Field Tag {version.Fields.MaxTag}, and {version.DataTypes.Count} DataTypes");
        }

        static void LookupSpecificVersion()
        {
            Console.WriteLine("\nLookup a specific FIX version");
            if (Versions["FIX.5.0SP2"] is Fix.Dictionary.Version version)
            {
                Console.WriteLine($"{version.BeginString} has {version.Messages.Length} Messages, Maximum Field Tag {version.Fields.MaxTag}, and {version.DataTypes.Count} DataTypes");
            }
        }

        static void EnumerateVersionMessages()
        {
            Console.WriteLine("\nEnumerate version messages");
            foreach (var message in Versions.FIX_5_0SP2.Messages)
            {
                Console.WriteLine($"MsgType = {message.MsgType}, Name = {message.Name}, Pedigree = ({message.Pedigree}), Description = {message.Description}");
            }
        }

        static void LookupMessageByMsgType()
        {
            Console.WriteLine("\nLookup message by MsgType");
            if (Versions.FIX_5_0SP2.Messages["D"] is Message message)
            {
                Console.WriteLine($"MsgType = {message.MsgType}, Name = {message.Name}, Pedigree = ({message.Pedigree}), Description = {message.Description}");
            }
        }

        static void EnumerateVersionFields()
        {
            Console.WriteLine("\nEnumerate version fields");
            foreach (var field in Versions.FIX_5_0SP2.Fields.Where(field => field.IsValid))
            {
                Console.WriteLine($"Tag = {field.Tag}, Name = {field.Name}, DataType = {field.DataType}, Pedigree = ({field.Pedigree}), Description = {field.Description}");
            }
        }

        static void LookupFieldByTag()
        {
            Console.WriteLine("\nLookup field by tag");
            if (Versions.FIX_5_0SP2.Fields[38] is VersionField field && field.IsValid)
            {
                Console.WriteLine($"Tag = {field.Tag}, Name = {field.Name}, DataType = {field.DataType}, Pedigree = ({field.Pedigree}), Description = {field.Description}");
            }
        }

        static void EnumerateVersionDataTypes()
        {
            Console.WriteLine("\nEnumerate version data types");
            foreach (var dataType in Versions.FIX_5_0SP2.DataTypes)
            {
                Console.WriteLine($"Name = {dataType.Name}, Description = {dataType.Description}");
            }
        }

        static void EnumerateMessageFields()
        {
            Console.WriteLine("\nEnumerate message fields");
            if (Versions.FIX_5_0SP2.Messages["D"] is Message message)
            {
                foreach (var field in message.Fields)
                {
                    Console.WriteLine($"Tag = {field.Tag}, Name = {field.Name}, Required = {field.Required}, Depth = {field.Depth}, Description = {field.Description}");
                }
            }
        }

    }
}
