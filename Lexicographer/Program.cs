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
using System.IO;

namespace Lexicographer
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Usage: {0} <FIX Repository Path> <Output Path>", args[0]);
                    return 1;
                }

                string repositoryPath = args[0];
                string outputPath = args[1];

                if (!Path.IsPathRooted(repositoryPath))
                    repositoryPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + repositoryPath;

                if (!Path.IsPathRooted(outputPath))
                    outputPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + outputPath;

                if (!Directory.Exists(repositoryPath))
                {
                    Console.WriteLine("Repository path '{0}' does not exist", repositoryPath);
                    return 1;
                }

                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                Console.WriteLine("Using FIX repository located in '{0}' to generate dictionary in '{1}'", repositoryPath, outputPath);

                var generator = new CodeGenerator
                {
                    OutputPath = outputPath,
                    Repository = new Fix.Repository.Root(repositoryPath)
                };

                generator.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }

            return 0;
        }
    }
}
