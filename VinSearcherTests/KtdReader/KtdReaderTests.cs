using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VinSearcher.KtdReader.Tests
{
    [TestClass()]
    public class KtdReaderTests
    {
        [TestMethod()]
        public void GetIndexBlocksTest()
        {
            var y = new KtdReader();
            var blocks = y.GetIndexBlocks(@"C:\ePer installs\Release 20\SP.CH.00900.FCTLR", 1);
            Assert.AreEqual(14009, blocks.Count);

        }

        [TestMethod()]
        public void FindKeyInIndexTest()
        {
            var y = new KtdReader();
            var block = y.FindIndexBlockForKey(@"C:\ePer installs\Release 20\SP.CH.00900.FCTLR", "18399999999");
            Assert.AreEqual("18401001276", block.Key);
        }
    }
}