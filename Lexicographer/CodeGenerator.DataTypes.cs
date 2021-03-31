/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CodeGenerator.DataTypes.cs
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
        CodeTypeDeclaration GenerateDataTypes(Fix.Repository.Version version)
        {
            var dictionaryType = new CodeTypeDeclaration("Dictionary")
            {
                Attributes = MemberAttributes.Public,
                IsPartial = true
            };

            var versionType = new CodeTypeDeclaration(version.BeginString.Replace(".", "_"))
            {
                Attributes = MemberAttributes.Public,
                IsPartial = true
            };

            var dataTypeCollectionMember = new CodeMemberField(versionType.Name + "DataTypeCollection", "_dataTypeCollection")
            {
                Attributes = MemberAttributes.Private | MemberAttributes.Static,
                InitExpression = new CodeObjectCreateExpression(versionType.Name + "DataTypeCollection")
            };
            versionType.Members.Add(dataTypeCollectionMember);

            var dataTypesProperty = new CodeMemberProperty
            {
                Attributes =
                    MemberAttributes.Public | MemberAttributes.Static,
                Name = "DataTypes",
                Type = new CodeTypeReference(versionType.Name + "DataTypeCollection"),
                GetStatements =
                                                                {
                                                                    new CodeMethodReturnStatement
                                                                        {
                                                                            Expression =
                                                                                new CodeVariableReferenceExpression(
                                                                                "_dataTypeCollection")
                                                                        }
                                                                }
            };
            versionType.Members.Add(dataTypesProperty);

            var dataTypesType = new CodeTypeDeclaration(versionType.Name + "DataTypeCollection")
            {
                Attributes = MemberAttributes.Public,
                IsPartial = false
            };
            dataTypesType.BaseTypes.Add(new CodeTypeReference("DataTypeCollection"));

            foreach (string dataType in version.DataTypes)
            {
                string typeName = dataType[0].ToString().ToUpper() + dataType.Substring(1);
                var field = new CodeMemberField("readonly string", typeName)
                {
                    Attributes = MemberAttributes.Public,
                    InitExpression = new CodePrimitiveExpression(dataType)
                };
                dataTypesType.Members.Add(field);
            }
         
            var itemsCreate = new CodeArrayCreateExpression("System.String");
            var assign = new CodeAssignStatement(new CodeVariableReferenceExpression("_dataTypes"), itemsCreate);

            var dataTypeCollectionConstructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            dataTypeCollectionConstructor.Statements.Add(assign);

            dataTypesType.Members.Add(dataTypeCollectionConstructor);
            
            foreach (string dataType in version.DataTypes)
            {
                string typeName = dataType[0].ToString().ToUpper() + dataType.Substring(1);
                itemsCreate.Initializers.Add(new CodeTypeReferenceExpression(typeName));
            }
          
            versionType.Members.Add(dataTypesType);
            dictionaryType.Members.Add(versionType);

            return dictionaryType;
        }
    }
}
