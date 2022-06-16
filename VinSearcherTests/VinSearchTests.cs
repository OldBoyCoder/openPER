using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinSearcher;

namespace VinSearcherTests
{
    [TestClass()]
    public class VinSearchTests
    {
        private const string PathToVindata = @"c:\dev\openPER\openPER\Data\Release18\VinData\SP.CH.00900.FCTLR";
        [TestMethod()]
        public void FindVehiclesByFullVinTest()
        {
            var x = new Release18VinSearch(PathToVindata);
            var results = x.FindVehiclesByFullVin("XXXXXXXXXXXXXXXXX");
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod()]
        public void FindVehiclesByModelAndChassisTestWithDobloChassis()
        {
            var x = new Release18VinSearch(PathToVindata);
            var result = x.FindVehicleByModelAndChassis("223", "05050806");
            Assert.AreEqual("2608455", result.Motor);
        }
        [TestMethod()]
        public void FindVehiclesByModelAndChassisTestWithInvalidVehicle()
        {
            var x = new Release18VinSearch(PathToVindata);
            var result = x.FindVehicleByModelAndChassis("1XX", "01001276");
            Assert.AreEqual(null, result);
        }
    }
}