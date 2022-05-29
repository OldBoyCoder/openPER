using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinSearcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinSearcher.Tests
{
    [TestClass()]
    public class VinSearchTests
    {
        [TestMethod()]
        public void FindVehiclesByFullVinTest()
        {
            var x = new VinSearch();
            var results = x.FindVehiclesByFullVin("XXXXXXXXXXXXXXXXX");
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod()]
        public void FindVehiclesByModelAndChassisTestWithDobloChassis()
        {
            var x = new VinSearch();
            var result = x.FindVehicleByModelAndChassis("223", "05050806");
            Assert.AreEqual("2608455", result.Motor);
        }
        [TestMethod()]
        public void FindVehiclesByModelAndChassisTestWithInvalidVehicle()
        {
            var x = new VinSearch();
            var result = x.FindVehicleByModelAndChassis("1XX", "01001276");
            Assert.AreEqual(null, result);
        }
    }
}