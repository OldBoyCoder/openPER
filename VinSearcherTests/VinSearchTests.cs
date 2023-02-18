using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinSearcher;

namespace VinSearcherTests
{
    [TestClass()]
    public class VinSearchTests
    {
        private const string PathToVindataCH = @"c:\dev\openPER\openPER\Data\Release84\VinData\SP.CH.04210.FCTLR";
        private const string PathToVindataRT = @"c:\dev\openPER\openPER\Data\Release84\VinData\SP.RT.04210.FCTLR";
        [TestMethod()]
        public void FindVehiclesByFullVinTest()
        {
            var x = new Release84VinSearch(PathToVindataCH, PathToVindataRT);
            var results = x.FindVehiclesByFullVin("XXXXXXXXXXXXXXXXX");
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod()]
        public void FindVehiclesByModelAndChassisTestWithDobloChassis()
        {
            var x = new Release84VinSearch(PathToVindataCH, PathToVindataRT);
            var result = x.FindVehicleByModelAndChassis("223", "05050806");
            Assert.AreEqual("2608455", result.Motor);
        }
        [TestMethod()]
        public void FindVehiclesByModelAndChassisTestWithInvalidVehicle()
        {
            var x = new Release84VinSearch(PathToVindataCH, PathToVindataRT);
            var result = x.FindVehicleByModelAndChassis("1XX", "01001276");
            Assert.AreEqual(null, result);
        }
        [TestMethod()]
        public void FindVehiclesByVINWithExtraInfoBravo()
        {
            var x = new Release84VinSearch(PathToVindataCH, PathToVindataRT);
            var result = x.FindVehicleByModelAndChassis("198", "04275255");
            Assert.AreEqual("6416415", result.Motor);
        }

    }
}