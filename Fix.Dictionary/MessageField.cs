using System;

namespace Fix
{
    public static partial class Dictionary
    {
        public class MessageField
        {
            public MessageField(VersionField field, bool required)
            {
                this.field = field;
                Required = required;
            }

            public int Tag => field.Tag;
            public string Name => field.Name;
            public string Description => field.Description;
   
            public bool Required { get; }

            VersionField field;
        }
    }
}