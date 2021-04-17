using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Fix
{
    public static partial class Dictionary
    {
        public class VersionCollection : IEnumerable<Version>
        {
            public VersionCollection(Version defaultVersion, params Version[] versions)
            {
                Default = defaultVersion;
                _versions = versions.Append(defaultVersion).ToArray();
            }

            public Version? this[string beginString]
            {
                get
                {
                    return (from version in _versions
                            where version.BeginString == beginString
                            select version).FirstOrDefault();
                }
            }

            public Version Default { get; private set; }

            public int Count => _versions.Length;

            public virtual IEnumerator<Version> GetEnumerator()
            {
                foreach (var version in _versions)
                {
                    yield return version;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            Version[] _versions;
        }
    }
}