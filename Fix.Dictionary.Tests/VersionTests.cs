using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fix;

namespace FixDictionary.Tests
{
    [TestClass]
    public class VersionTests
    {
        [TestMethod]
        public void TestCount()
        {
            Assert.AreEqual(4, Dictionary.Versions.Count);
        }

        [TestMethod]
        public void TestIndexer()
        {
            Assert.AreEqual("FIX.4.2", Dictionary.Versions["FIX.4.2"]!.BeginString);
            Assert.AreEqual("FIX.4.4", Dictionary.Versions["FIX.4.4"]!.BeginString);
            Assert.AreEqual("FIXT.1.1", Dictionary.Versions["FIXT.1.1"]!.BeginString);
            Assert.AreEqual("FIX.5.0SP2", Dictionary.Versions["FIX.5.0SP2"]!.BeginString);
        }
    }
}
