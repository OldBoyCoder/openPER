using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinSearcher;

namespace VinSearcherTests
{
    [TestClass()]
    public class VinSearchTests
    {
        private const string PathToVindataCh = @"c:\dev\openPER\openPER\Data\Release84\SP.CH.04210.FCTLR";
        private const string PathToVindataRt = @"c:\dev\openPER\openPER\Data\Release84\SP.RT.04210.FCTLR";

        [TestMethod()]
        public void FindVehiclesByModelAndChassisTestWithDobloChassis()
        {
            var x = new Release84VinSearch(PathToVindataCh, PathToVindataRt);
            var result = x.FindVehicleByModelAndChassis("223", "05050806");
            Assert.AreEqual("2608455", result.Motor);
        }
        [TestMethod()]
        public void FindVehiclesByModelAndChassisTestWithInvalidVehicle()
        {
            var x = new Release84VinSearch(PathToVindataCh, PathToVindataRt);
            var result = x.FindVehicleByModelAndChassis("1XX", "01001276");
            Assert.AreEqual(null, result);
        }
        [TestMethod()]
        public void FindVehiclesByVinWithExtraInfoBravo()
        {
            var x = new Release84VinSearch(PathToVindataCh, PathToVindataRt);
            var result = x.FindVehicleByModelAndChassis("198", "04275255");
            Assert.AreEqual("6416415", result.Motor);
        }

    }
}