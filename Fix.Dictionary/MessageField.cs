using System;

namespace Fix
{
    public static partial class Dictionary
    {
        public class MessageField
        {
            public MessageField(VersionField field)
            {
                this.field = field;
            }

            public int Tag => field.Tag;
            public string Name => field.Name;
            public string Description => field.Description;
   
            VersionField field;
        }
    }
}