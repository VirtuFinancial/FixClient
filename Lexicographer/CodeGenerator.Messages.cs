/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CodeGenerator.Messages.cs
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
        public CodeTypeDeclaration GenerateMessages(Fix.Repository.Version version)
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

            var messageCollectionMember = new CodeMemberField(versionType.Name + "MessageCollection", "_messageCollection")
            {
                Attributes = MemberAttributes.Private | MemberAttributes.Static,
                InitExpression = new CodeObjectCreateExpression(versionType.Name + "MessageCollection")
            };
            versionType.Members.Add(messageCollectionMember);

            var messagesProperty = new CodeMemberProperty
            {
                Attributes =
                    MemberAttributes.Public | MemberAttributes.Static,
                Name = "Messages",
                Type = new CodeTypeReference(versionType.Name + "MessageCollection"),
                GetStatements =
                                                                {
                                                                    new CodeMethodReturnStatement
                                                                        {
                                                                            Expression =
                                                                                new CodeVariableReferenceExpression(
                                                                                "_messageCollection")
                                                                        }
                                                                }
            };
            versionType.Members.Add(messagesProperty);

            //
            // public static class Messages : IEnumerable<Message>
            //    {
            //
            var messagesType = new CodeTypeDeclaration(versionType.Name + "MessageCollection")
            {
                Attributes = MemberAttributes.Public,
                IsPartial = false
            };
            messagesType.BaseTypes.Add(new CodeTypeReference("MessageCollection"));

            var messageCollectionConstructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };

            var itemsCreate = new CodeArrayCreateExpression("Message");

            foreach (Fix.Repository.Message message in version.Messages)
            {
                string name = message.Name;

                if (name == "SecurityStatus")
                {
                    name = name + "Message";
                }

                itemsCreate.Initializers.Add(new CodeTypeReferenceExpression(name));
            }

            var assign = new CodeAssignStatement(new CodeVariableReferenceExpression("_messages"), itemsCreate);

            messageCollectionConstructor.Statements.Add(assign);

            messagesType.Members.Add(messageCollectionConstructor);

            //
            // public static HeartbeatMessage Heartbeat = new HeartbeatMessage();
            //
            foreach (Fix.Repository.Message message in version.Messages)
            {
                //
                // readonly is C# specific so not supported by CodeDom.
                //
                string name = message.Name;

                if (name == "SecurityStatus")
                {
                    name = name + "Message";
                }

                var messageField = new CodeMemberField("readonly " + name, name)
                {
                    Attributes = MemberAttributes.Public,
                    InitExpression = new CodeObjectCreateExpression(name)
                };

                messagesType.Members.Add(messageField);
            }

            versionType.Members.Add(messagesType);
            dictionaryType.Members.Add(versionType);

            return dictionaryType;
        }

        public CodeTypeDeclaration GenerateMessage(Fix.Repository.Version version, Fix.Repository.Message message)
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

            dictionaryType.Members.Add(versionType);
            //
            //  private class Hearbeat : Message
            //  {
            //      HearbeatMessage()
            //      :   Message("0", "Heartbeat", "The Heartbeat monitors the status of the communication link and identifies when t" +
            //                       "he last of a string of messages was not received.", "FIX.2.7")
            //      {
            //      }
            //  }
            //
            string name = message.Name;

            if (name == "SecurityStatus")
            {
                name = name + "Message";
            }

            var messageType = new CodeTypeDeclaration(name)
            {
                BaseTypes = { new CodeTypeReference("Message") }
            };

            int fieldCount = GenerateMessageFields(version, message, messageType);

            var messageConstructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Assembly,
                BaseConstructorArgs = { new CodePrimitiveExpression(message.MsgType),
                                        new CodePrimitiveExpression(name),
                                        new CodePrimitiveExpression(message.Description),
                                        new CodePrimitiveExpression(message.Added) }

            };
            messageType.Members.Add(messageConstructor);

            // protected abstract FieldCollection GetFields();
            var fieldsMember = new CodeMemberMethod
            {
                Name = "GetFields",
                Attributes = MemberAttributes.Override | MemberAttributes.Family,
                ReturnType = new CodeTypeReference("MessageFieldCollection")
            };

            fieldsMember.Statements.Add(
                new CodeConditionStatement(new CodeSnippetExpression("_fieldCollection == null"),
                                           new CodeStatement[]
                                           {
                                            new CodeAssignStatement(
                                            new CodeVariableReferenceExpression("_fieldCollection"),
                                            new CodeObjectCreateExpression(message.Name + "FieldCollection"))
                                           }));
            fieldsMember.Statements.Add(
                new CodeMethodReturnStatement
                {
                    Expression = new CodeVariableReferenceExpression("_fieldCollection")
                }
            );
            messageType.Members.Add(fieldsMember);

            // public int FieldCount { get { return 55; } }
            var fieldCountProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Override,
                Name = "FieldCount",
                Type = new CodeTypeReference(typeof(int)),
                GetStatements = {
                    new CodeMethodReturnStatement(new CodePrimitiveExpression(fieldCount))
                }
            };
            messageType.Members.Add(fieldCountProperty);

            versionType.Members.Add(messageType);

            return dictionaryType;
        }

        int GenerateMessageFields(Fix.Repository.Version version,
                                  Fix.Repository.Message message,
                                  CodeTypeDeclaration messageType)
        {
            var fieldCollectionMember = new CodeMemberField(message.Name + "FieldCollection", "_fieldCollection")
            {
                Attributes = MemberAttributes.Private
            };
            messageType.Members.Add(fieldCollectionMember);

            var fieldsProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.New,
                Name = "Fields",
                Type = new CodeTypeReference(message.Name + "FieldCollection"),
                GetStatements = {
                    new CodeMethodReturnStatement(new CodeCastExpression(new CodeTypeReference(message.Name + "FieldCollection"), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, "GetFields"))))
                }
            };
            messageType.Members.Add(fieldsProperty);
            /*
                public class HeartbeatFieldCollection : MessageFieldCollection {
                    public HeartbeatFieldCollection() 
                    :   base(new Field[] {
                                new Field(FIX_4_0.BeginStringDefinition, true, 0, "FIX.2.7"),
                                new Field(FIX_4_0.BodyLengthDefinition, true, 0, "FIX.2.7")
                             })
                    {
                    }
                }
            */
            var fieldsType = new CodeTypeDeclaration(message.Name + "FieldCollection")
            {
                Attributes = MemberAttributes.Public,
                IsPartial = false
            };
            fieldsType.BaseTypes.Add(new CodeTypeReference("MessageFieldCollection"));

            var itemsCreate = new CodeArrayCreateExpression("Field");

            List<FieldDefinition> fieldDefinitions = MessageFields(version, message);

            string versionName = version.BeginString.Replace(".", "_");

            foreach (FieldDefinition definition in fieldDefinitions)
            {
                // ITG customisation
                if (message.MsgType == "UWO" && definition.Field.Tag == 67)
                    continue;

                itemsCreate.Initializers.Add(new CodeObjectCreateExpression("Field",
                                                                    new CodeVariableReferenceExpression(versionName + "." + definition.Field.Name + "Definition"),
                                                                    new CodePrimitiveExpression(definition.Required),
                                                                    new CodePrimitiveExpression(definition.Indent),
                                                                    new CodePrimitiveExpression(definition.Added)));
            }

            var fieldCollectionConstructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public,
                BaseConstructorArgs = { itemsCreate }
            };

            fieldsType.Members.Add(fieldCollectionConstructor);

            messageType.Members.Add(fieldsType);

            return itemsCreate.Initializers.Count;
        }

        class FieldDefinition
        {
            public Fix.Repository.Field Field;
            public bool Required;
            public int Indent;
            public string Added;
        }

        List<FieldDefinition> MessageFields(Fix.Repository.Version version, Fix.Repository.Message message)
        {
            var definitions = new List<FieldDefinition>();
            PopulateFieldDefinitions(message.ComponentID, version, definitions, 0);
            return definitions;
        }

        void PopulateFieldDefinitions(string componentId, Fix.Repository.Version version, List<FieldDefinition> definitions, int parentIndent)
        {
            List<Fix.Repository.MsgContent> msgContents;

            if (!version.MsgContents.TryGetValue(componentId, out msgContents))
            {
                // TODO
                return;
            }

            foreach (Fix.Repository.MsgContent content in msgContents)
            {
                int indent = Convert.ToInt32(content.Indent);

                int tag;

                if (int.TryParse(content.TagText, out tag))
                {
                    Fix.Repository.Field field;

                    if (!version.Fields.TryGetValue(tag, out field))
                    {
                        // TODO - better tell someone.
                        continue;
                    }

                    definitions.Add(new FieldDefinition
                    {
                        Field = field,
                        Required = Convert.ToInt32(content.Reqd) != 0,
                        Indent = parentIndent + indent,
                        Added = content.Added
                    });
                }
                else
                {
                    Fix.Repository.Component component;

                    if (!version.Components.TryGetValue(content.TagText, out component))
                    {
                        // TODO - better tell someone
                        continue;
                    }

                    PopulateFieldDefinitions(component.ComponentID, version, definitions, indent);
                }
            }
        }
    }
}
