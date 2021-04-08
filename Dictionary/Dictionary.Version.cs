/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Dictionary.Version.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace Fix
{
    public partial class Dictionary
    {
        public class Version
        {
            public Version(string beginString, MessageCollection messages, VersionFieldCollection fields)
            {
                BeginString = beginString;
                Messages = messages;
                Fields = fields;
            }

            public string BeginString { get; }
            public MessageCollection Messages { get; }
            public VersionFieldCollection Fields { get; }

            public string SanitisedBeginString => BeginString.Replace(".", "");

            public string ApplVerID
            {
                get
                {
                    Dictionary.FIX_5_0SP2.ApplVerID result;
                    if (!Enum.TryParse(SanitisedBeginString, out result))
                        return null;
                    return Convert.ToChar(result).ToString();
                }
            }

            #region Object Methods

            public override string ToString() => BeginString;

            public override int GetHashCode() => BeginString.GetHashCode();

            public override bool Equals(object obj)
            {
                var version = obj as Version;
                if (version == null)
                    return false;
                return BeginString.Equals(version.BeginString);
            }

            #endregion

            public static bool operator ==(Version a, Version b)
            {
                if (ReferenceEquals(a, b))
                    return true;
                if ((object)a == null || (object)b == null)
                    return false;
                return a.BeginString == b.BeginString;
            }

            public static bool operator !=(Version a, Version b)
            {
                return !(a == b);
            }
        }

        public partial class VersionCollection : IEnumerable<Version>
        {
            public virtual int Count => _versions.Length;

            public virtual Version this[int index] => _versions[index];

            public virtual Version this[string beginString]
            {
                get
                {
                    return (from version in _versions where version.BeginString == beginString select version).FirstOrDefault();
                }
            }

            public virtual IEnumerator<Version> GetEnumerator()
            {
                foreach (Version version in _versions)
                    yield return version;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            protected Version[] _versions;
        }
    }
}
