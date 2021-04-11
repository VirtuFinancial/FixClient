/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CodeGenerator.Fields.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Lexicographer
{
    partial class CodeGenerator
    {
        static CodeTypeDeclaration GenerateFields(Fix.Repository.Version version)
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

            var fieldsProperty = new CodeMemberProperty
            {
                Attributes =
                    MemberAttributes.Public | MemberAttributes.Static,
                Name = "Fields",
                Type = new CodeTypeReference(versionType.Name + "FieldCollection"),
                GetStatements =
                                                                {
                                                                    new CodeMethodReturnStatement
                                                                        {
                                                                            Expression =
                                                                                new CodeVariableReferenceExpression(
                                                                                "_fieldCollection")
                                                                        }
                                                                }
            };
            versionType.Members.Add(fieldsProperty);

            var fieldsType = new CodeTypeDeclaration(versionType.Name + "FieldCollection")
            {
                Attributes = MemberAttributes.Public,
                IsPartial = false
            };
            fieldsType.BaseTypes.Add(new CodeTypeReference("VersionFieldCollection"));

            var fieldCollectionConstructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };

            var itemsCreate = new CodeArrayCreateExpression("Field");
            //
            // We want to be able to index fields directly in the array using the numeric value of their tag, there are
            // however some missing tags ie. 101, so we need to generate a null in it's place.
            //
            int expectedTag = 1;

            foreach (Fix.Repository.Field field in version.Fields.Values)
            {
                int actualTag = Convert.ToInt32(field.Tag);

                while (actualTag > expectedTag)
                {
                    itemsCreate.Initializers.Add(new CodePrimitiveExpression(null));
                    expectedTag++;
                }

                itemsCreate.Initializers.Add(new CodeTypeReferenceExpression(field.Name));
                expectedTag++;
            }

            var assign = new CodeAssignStatement(new CodeVariableReferenceExpression("_fields"), itemsCreate);

            fieldCollectionConstructor.Statements.Add(assign);

            fieldsType.Members.Add(fieldCollectionConstructor);
            //
            // private FieldDefinition Account = new FieldDefinition(1, "Account", "Account mnemonic as agreed between broker and institution.", 0);
            //
            foreach (Fix.Repository.Field field in version.Fields.Values)
            {
                string enumType = null;

                if (version.Enums.TryGetValue(field.Tag, out List<Fix.Repository.Enum> _))
                {
                    //
                    // TODO -   just avoid multi character values for now as we would need to encode
                    //          them to fit into an int for an enum value
                    //
                    /*
                    int longValues = (from e in enumeratedValues
                                      where e.Value.Length > 1
                                      select e).Count();

                    if (longValues == 0)
                    {
                        enumType = dictionaryType.Name + "." + versionType.Name + "." + field.Name;
                    }
                    */
                    enumType = dictionaryType.Name + "." + versionType.Name + "." + field.Name;
                }

                CodeExpression enumExpression = enumType == null
                                                    ? (CodeExpression)new CodePrimitiveExpression(null)
                                                    : new CodeTypeOfExpression(enumType);

                string typeName = field.Type[0].ToString().ToUpper() + field.Type[1..];

                var fieldField = new CodeMemberField("readonly FieldDefinition", field.Name + "Definition")
                {
                    Attributes = MemberAttributes.Private | MemberAttributes.Static,
                    InitExpression = new CodeObjectCreateExpression("FieldDefinition",
                                                                    new CodePrimitiveExpression(field.Tag),
                                                                    new CodePrimitiveExpression(field.Name),
                                                                    new CodePrimitiveExpression(field.Description),
                                                                    new CodeVariableReferenceExpression(version.BeginString.Replace(".", "_") + ".DataTypes." + typeName),
                                                                    enumExpression,
                                                                    new CodePrimitiveExpression(field.Added))
                };
                versionType.Members.Add(fieldField);
            }

            var fieldCollectionMember = new CodeMemberField(versionType.Name + "FieldCollection", "_fieldCollection")
            {
                Attributes = MemberAttributes.Private | MemberAttributes.Static,
                InitExpression = new CodeObjectCreateExpression(versionType.Name + "FieldCollection")
            };
            versionType.Members.Add(fieldCollectionMember);

            //
            // public Field Account = new Field(AccountDefinition);
            //
            foreach (Fix.Repository.Field field in version.Fields.Values)
            {
                var fieldField = new CodeMemberField("readonly Field", field.Name)
                {
                    Attributes = MemberAttributes.Public,
                    InitExpression = new CodeObjectCreateExpression("Field", new CodeVariableReferenceExpression(versionType.Name + "." + field.Name + "Definition"))
                };
                fieldsType.Members.Add(fieldField);
            }

            versionType.Members.Add(fieldsType);
            dictionaryType.Members.Add(versionType);

            return dictionaryType;
        }
    }
}
