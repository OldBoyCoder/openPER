using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VinSearcherTests.KtdReader
{
    [TestClass()]
    public class KtdReaderTests
    {
        private const string PathToVindata = @"c:\dev\openPER\openPER\Data\Release18\VinData\SP.CH.00900.FCTLR";

        [TestMethod()]
        public void GetIndexBlocksTest()
        {
            var y = new VinSearcher.KtdReader.KtdReader();
            var blocks = y.GetIndexBlocks(PathToVindata, 1);
            Assert.AreEqual(14009, blocks.Count);

        }

        [TestMethod()]
        public void FindKeyInIndexTest()
        {
            var y = new VinSearcher.KtdReader.KtdReader();
            var block = y.FindIndexBlockForKey(PathToVindata, "18399999999");
            Assert.AreEqual("18401001276", block.Key);
        }
    }
}