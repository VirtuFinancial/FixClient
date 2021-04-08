/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Root.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fix.Repository
{
    public class Root
    {
        public Root(string path)
        {
            Path = path;
            Scan();
        }

        public string Path { get; }
        public IEnumerable<Version> Versions => _versions;

        #region Private Methods

        void Scan()
        {
            _versions = from directory in Directory.EnumerateDirectories(Path, "FIX*")
                        select new Version(directory, System.IO.Path.GetFileName(directory));
        }

        #endregion

        #region Private Members

        IEnumerable<Version> _versions = new List<Version>();

        #endregion

    }
}
