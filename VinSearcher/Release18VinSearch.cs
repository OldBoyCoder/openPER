using System.Collections.Generic;

namespace VinSearcher
{
    public class Release18VinSearch:IReturnedDataHandler
    {
        private readonly string _pathToVinData;
        public Release18VinSearch(string pathToVinData)
        {
            _pathToVinData = pathToVinData;
        }

        public List<VinResult> FindVehiclesByFullVin(string vinNumber)
        {
            var vehicles = new List<VinResult>();

            return vehicles;
        }
        public VinResult FindVehicleByModelAndChassis(string modelNumber, string chassisNumber)
        {
            var x = new KtdReader.KtdReader();
            var searchKey = modelNumber + chassisNumber.PadLeft(8, '0');
            var record = x.RecordsForKey(_pathToVinData, searchKey, 1, this);
            if (record != null)
            {
                var vehicle = new VinResult
                {
                    Mvs = record[0],
                    Chassis = record[1],
                    Organization = record[2],
                    Motor = record[3],
                    Date = record[4],
                    InteriorColour = record[5]
                };
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
    }
}
