// This is a place holder and needs to be generated
// TODO - change the generator to take a list of orchestrations and pull the version + prefix from the file
using System;

namespace Fix
{
    public partial class Dictionary
    {
        public static VersionCollection Versions { get; } = new VersionCollection(
            new Version("FIX.4.4", FIX_4_4.DataTypes, FIX_4_4.Fields, FIX_4_4.Messages),
            new Version("FIX.4.2", FIX_4_2.DataTypes, FIX_4_2.Fields, FIX_4_2.Messages),
            new Version("FIX.5.0SP2", FIX_5_0SP2.DataTypes, FIX_5_0SP2.Fields, FIX_5_0SP2.Messages),
            // There isn't a separate transport orchestra and generating one from the repository doesn't work
            // due to errors in the repository so just cheat. It's only the session messages so this gives us
            // what we need.
            new Version("FIXT.1.1", FIX_5_0SP2.DataTypes, FIX_5_0SP2.Fields, FIX_5_0SP2.Messages));
    }
}

