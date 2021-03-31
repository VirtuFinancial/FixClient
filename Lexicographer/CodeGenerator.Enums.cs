/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CodeGenerator.Enums.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace Lexicographer
{
    partial class CodeGenerator
    {
        IEnumerable<CodeTypeDeclaration> GenerateEnums(Fix.Repository.Version version, bool includeNamespace = true)
        {
            var declarations = new List<CodeTypeDeclaration>();
            CodeTypeDeclaration versionType = null;

            if (includeNamespace)
            {
                var dictionaryType = new CodeTypeDeclaration("Dictionary")
                {
                    Attributes = MemberAttributes.Public,
                    IsPartial = true
                };

                versionType = new CodeTypeDeclaration(version.BeginString.Replace(".", "_"))
                {
                    Attributes = MemberAttributes.Public,
                    IsPartial = true
                };

                dictionaryType.Members.Add(versionType);
                declarations.Add(dictionaryType);
            }

            foreach(int tag in version.Enums.Keys)
            {
                Fix.Repository.Field field;

                if (!version.Fields.TryGetValue(tag, out field))
                    continue;

                var type = new CodeTypeDeclaration(field.Name)
                {
                    IsEnum = true
                };

                List<Fix.Repository.Enum> values = version.Enums[tag];

                if (field.Name == "SessionStatus")
                {
                    foreach (Fix.Repository.Enum value in values)
                    {
                        string symbolicName = value.SymbolicName;

                        var f = new CodeMemberField(field.Name, symbolicName)
                        {
                            CustomAttributes =
                            {
                                new CodeAttributeDeclaration("Description",
                                    new CodeAttributeArgument(
                                        new CodePrimitiveExpression(
                                            value.Description)))
                            },
                            InitExpression = new CodeSnippetExpression(string.Format("{0}", value.Value))
                        };
                        type.Members.Add(f);
                    }
                    // These are HKEX Orion custom values
                    var passwordChangeRequired = new CodeMemberField(field.Name, "PasswordChangeIsRequired")
                    {
                        CustomAttributes =
                        {
                            new CodeAttributeDeclaration("Description",
                                new CodeAttributeArgument(
                                    new CodePrimitiveExpression(
                                        "Password change is required")))
                        },
                        InitExpression = new CodeSnippetExpression("100")
                    };
                    type.Members.Add(passwordChangeRequired);

                    var other = new CodeMemberField(field.Name, "Other")
                    {
                        CustomAttributes =
                        {
                            new CodeAttributeDeclaration("Description",
                                new CodeAttributeArgument(
                                    new CodePrimitiveExpression(
                                        "Other")))
                        },
                        InitExpression = new CodeSnippetExpression("101")
                    };
                    type.Members.Add(other);
                }
                else if (field.Name == "TrdType")
                {
                    foreach (Fix.Repository.Enum value in values)
                    {
                        string symbolicName = value.SymbolicName;

                        var f = new CodeMemberField(field.Name, symbolicName)
                        {
                            CustomAttributes =
                            {
                                new CodeAttributeDeclaration("Description",
                                    new CodeAttributeArgument(
                                        new CodePrimitiveExpression(
                                            value.Description)))
                            },
                            InitExpression = new CodeSnippetExpression(string.Format("{0}", value.Value))
                        };
                        type.Members.Add(f);
                    }
                    // These are HKEX Orion custom values
                    var oddLotTrade = new CodeMemberField(field.Name, "OddLotTrade")
                    {
                        CustomAttributes =
                        {
                            new CodeAttributeDeclaration("Description",
                                new CodeAttributeArgument(
                                    new CodePrimitiveExpression(
                                        "Odd Lot Trade")))
                        },
                        InitExpression = new CodeSnippetExpression("102")
                    };
                    type.Members.Add(oddLotTrade);

                    var overseasTrade = new CodeMemberField(field.Name, "OverseasTrade")
                    {
                        CustomAttributes =
                        {
                            new CodeAttributeDeclaration("Description",
                                new CodeAttributeArgument(
                                    new CodePrimitiveExpression(
                                        "Overseas Trade")))
                        },
                        InitExpression = new CodeSnippetExpression("104")
                    };
                    type.Members.Add(overseasTrade);
                }
                else
                {
                    //
                    // TODO -   just avoid multi character values for now as we would need to encode
                    //          them to fit into an int for an enum value
                    //
                    var singleCharacterValues = from e in values where e.Value.Length == 1 select e;

                    foreach (Fix.Repository.Enum value in singleCharacterValues)
                    {
                        string symbolicName = value.SymbolicName;

                        if (version.BeginString == "FIX.5.0SP2")
                        {
                            if (field.Name == "ApplReqType" && value.Value == "5")
                                symbolicName = "CancelRetransmission"; // FIX.5.0SP2 name clash with value 0
                            else if (field.Name == "MassCancelRequestType" && value.Value == "B")
                                symbolicName = "CancelForSecurityIssuer"; // FIX.5.0SP2 name clash with value 1
                            else if (field.Name == "UnderlyingPriceDeterminationMethod" && value.Value == "4")
                                symbolicName = "AverageValue";
                        }

                        var f = new CodeMemberField(field.Name, symbolicName)
                        {
                            CustomAttributes =
                            {
                                new CodeAttributeDeclaration("Description",
                                    new CodeAttributeArgument(
                                        new CodePrimitiveExpression(
                                            value.Description)))
                            },
                            InitExpression = new CodeSnippetExpression(string.Format("'{0}'", value.Value))
                        };

                        if (!string.IsNullOrEmpty(value.Added))
                        {
                            f.CustomAttributes.Add(
                                new CodeAttributeDeclaration("Added",
                                    new CodeAttributeArgument(
                                        new CodePrimitiveExpression(value.Added))));
                        }

                        type.Members.Add(f);
                    }
                }

                if (includeNamespace)
                {
                    versionType.Members.Add(type);
                }
                else
                {
                    declarations.Add(type);
                }
            }

            return declarations;
        }
    }
}
