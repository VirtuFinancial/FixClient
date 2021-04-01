/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CodeGenerator.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Lexicographer
{
    partial class CodeGenerator
    {
        public string OutputPath { get; set; }
        public Fix.Repository.Root Repository { get; set; }

        public void Run()
        {
            GenerateFile(string.Empty, new [] { GenerateVersions() });
            
            foreach(Fix.Repository.Version version in Repository.Versions)
            {
                GenerateFile(version.BeginString + "_Messages", new [] { GenerateMessages(version) });    

                foreach(Fix.Repository.Message message in version.Messages)
                {
                    string name = message.Name;

                    if (name == "SecurityStatus")
                    {
                        name = name + "Message";
                    }

                    GenerateFile(version.BeginString + "_Messages_" + name, new [] { GenerateMessage(version, message) });
                }
            }

            foreach (Fix.Repository.Version version in Repository.Versions)
            {
                GenerateFile(version.BeginString + "_Fields", new [] { GenerateFields(version) });
            }

            Fix.Repository.Version last = null;
            foreach (Fix.Repository.Version version in Repository.Versions)
            {
                GenerateFile(version.BeginString + "_Enums", GenerateEnums(version));
                if (last == null || !version.BeginString.StartsWith("FIXT"))
                {
                    last = version;
                }
            }

            if (last != null)
            {
                GenerateFile("Enums", GenerateEnums(last, false));        
            }

            foreach(Fix.Repository.Version version in Repository.Versions)
            {
                GenerateFile(version.BeginString + "_DataTypes", new [] { GenerateDataTypes(version) });
            }
        }

        void GenerateFile(string filenameSuffix, IEnumerable<CodeTypeDeclaration> types)
        {
            var globalNamespace = new CodeNamespace
            {
                Imports =   
                {
                    new CodeNamespaceImport("System"),
                    new CodeNamespaceImport("System.Linq"),
                    new CodeNamespaceImport("System.Collections.Generic"),
                    new CodeNamespaceImport("System.ComponentModel")
                }
            };

            var codeNamespace = new CodeNamespace("Fix");
            codeNamespace.Types.AddRange(types.ToArray());

            var codeUnit = new CodeCompileUnit
            {
                Namespaces = { globalNamespace, codeNamespace }
            };
            
            var codeProvider = new CSharpCodeProvider();

            string filename = OutputPath + Path.DirectorySeparatorChar + string.Format("Dictionary{0}.", string.IsNullOrEmpty(filenameSuffix) ? "" : "_" + filenameSuffix.Replace(".", "_")) + codeProvider.FileExtension;

            var writer = new IndentedTextWriter(new StreamWriter(filename, false), "    ");

            codeProvider.GenerateCodeFromCompileUnit(codeUnit, writer, new CodeGeneratorOptions());

            writer.Close();
        }
    }
}
