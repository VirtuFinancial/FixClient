/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CodeGenerator.Versions.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.CodeDom;

namespace Lexicographer
{
    partial class CodeGenerator
    {
        CodeTypeDeclaration GenerateVersions()
        {
            var dictionaryType = new CodeTypeDeclaration("Dictionary")
            {
                Attributes = MemberAttributes.Public,
                IsPartial = true
            };

            var versionCollectionMember = new CodeMemberField("VersionCollection", "_versions")
            {
                Attributes =
                    MemberAttributes.Private | MemberAttributes.Static,
                InitExpression =
                    new CodeObjectCreateExpression("VersionCollection")
            };
            dictionaryType.Members.Add(versionCollectionMember);

            var versionsProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                Name = "Versions",
                Type = new CodeTypeReference("VersionCollection"),
                GetStatements =
                                                              {
                                                                  new CodeMethodReturnStatement
                                                                      {
                                                                          Expression =
                                                                              new CodeVariableReferenceExpression(
                                                                              "_versions")
                                                                      }
                                                              }
            };
            dictionaryType.Members.Add(versionsProperty);

            var versionsType = new CodeTypeDeclaration("VersionCollection")
            {
                Attributes = MemberAttributes.Public,
                IsPartial = true
            };
            versionsType.BaseTypes.Add(new CodeTypeReference("IEnumerable<Version>"));

            string last = null;

            foreach (Fix.Repository.Version version in Repository.Versions)
            {
                string versionClass = version.BeginString.Replace('.', '_');
                var versionField = new CodeMemberField
                {
                    Attributes = MemberAttributes.Public,
                    Name = versionClass,
                    Type = new CodeTypeReference("Version")
                };
                versionsType.Members.Add(versionField);
                if (!versionField.Name.StartsWith("FIXT_"))
                    last = versionField.Name;
            }

            if (!string.IsNullOrEmpty(last))
            {
                var defaultField = new CodeMemberField
                {
                    Attributes = MemberAttributes.Public,
                    Name = "Default",
                    Type = new CodeTypeReference("Version")
                };
                versionsType.Members.Add(defaultField);
            }

            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };

            var versionsCreate = new CodeArrayCreateExpression("Version");

            foreach (Fix.Repository.Version version in Repository.Versions)
            {
                string versionClass = version.BeginString.Replace('.', '_');

                var assign = new CodeAssignStatement(new CodeVariableReferenceExpression(versionClass),
                                                     new CodeObjectCreateExpression("Version",
                                                                                    new CodePrimitiveExpression(version.BeginString),
                                                                                    new CodeVariableReferenceExpression("Fix.Dictionary." + versionClass + ".Messages"),
                                                                                    new CodeVariableReferenceExpression("Fix.Dictionary." + versionClass + ".Fields")));

                constructor.Statements.Add(assign);



                versionsCreate.Initializers.Add(new CodeVariableReferenceExpression(versionClass));
            }

            var defaultAssign = new CodeAssignStatement(new CodeVariableReferenceExpression("Default"),
                                                        new CodeVariableReferenceExpression(last));
            constructor.Statements.Add(defaultAssign);

            var versionsAssign = new CodeAssignStatement(new CodeVariableReferenceExpression("_versions"), versionsCreate);

            constructor.Statements.Add(versionsAssign);

            versionsType.Members.Add(constructor);

            dictionaryType.Members.Add(versionsType);

            var fieldsProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                Name = "Fields",
                Type = new CodeTypeReference(last + "." + last + "FieldCollection"),
                GetStatements =
                {
                    new CodeMethodReturnStatement
                    {
                        Expression = new CodeVariableReferenceExpression(last + ".Fields")
                    }
                }
            };
            dictionaryType.Members.Add(fieldsProperty);

            var messagesProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                Name = "Messages",
                Type = new CodeTypeReference(last + "." + last + "MessageCollection"),
                GetStatements =
                {
                    new CodeMethodReturnStatement
                    {
                        Expression = new CodeVariableReferenceExpression(last + ".Messages")
                    }
                }
            };
            dictionaryType.Members.Add(messagesProperty);

            var dataTypesProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                Name = "DataTypes",
                Type = new CodeTypeReference(last + "." + last + "DataTypeCollection"),
                GetStatements =
                {
                    new CodeMethodReturnStatement
                    {
                        Expression = new CodeVariableReferenceExpression(last + ".DataTypes")
                    }
                }
            };
            dictionaryType.Members.Add(dataTypesProperty);

            return dictionaryType;
        }
    }
}
