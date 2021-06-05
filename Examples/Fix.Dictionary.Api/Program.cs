using System;
using static Fix.Dictionary;

namespace Fix.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            EnumerateFixVersions();
        }

        static void EnumerateFixVersions()
        {
            foreach (var version in Versions)
            {
                Console.WriteLine(version.BeginString);
            }
        }

    }
}
