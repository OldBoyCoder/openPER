using System;
using System.Collections.Generic;
using VinSearcher.KtdReader;

namespace VinSearcher
{
    public class VinSearch:IReturnedDataHandler
    {

        public List<VinResult> FindVehiclesByFullVin(string vinNumber)
        {
            var vehicles = new List<VinResult>();

            return vehicles;
        }
        public VinResult FindVehicleByModelAndChassis(string modelNumber, string chassisNumber)
        {
            var x = new KtdReader.KtdReader();
            var searchKey = modelNumber + chassisNumber.PadLeft(8, '0');
            var record = x.RecordsForKey(@"C:\ePer installs\Release 18\SP.CH.00900.FCTLR", searchKey, 1, this);
            if (record != null)
            {
                var vehicle = new VinResult();
                vehicle.MVS = record[0];
                vehicle.Chassis = record[1];
                vehicle.Organization = record[2];
                vehicle.Motor = record[3];
                vehicle.Date = record[4];
                vehicle.InteriorColour = record[5];
                return vehicle;
            }
            return null;
        }

        public bool ProcessRow(List<string> data, string searchKey)
        {
            if (searchKey.Substring(0, 3) == data[0].Substring(0,3) &&
                searchKey.Substring(3, 8) == data[1])
            {
                return true;
            }
            return false;
        }

        private void LoadFileHeaderData()
        {

        }
    }
}
