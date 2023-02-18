using System.Collections.Generic;

namespace VinSearcher
{
    public class Release84VinSearch : IReturnedDataHandler
    {
        private readonly string _pathToVinDataCH;
        private readonly string _pathToVinDataRT;
        public Release84VinSearch(string pathToVinDataCH, string pathToVinDataRT)
        {
            _pathToVinDataCH = pathToVinDataCH;
            _pathToVinDataRT = pathToVinDataRT;
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
            var record = x.RecordsForKey(_pathToVinDataCH, searchKey, 1, this);
            if (record != null)
            {
                var vehicle = new VinResult
                {
                    Mvs = record[0],
                    Chassis = record[1],
                    Organization = record[2],
                    Motor = record[3],
                    Date = record[4],
                    InteriorColour = record[5],
                    VIN = record[6]
                };
                // Possibly enhance the record
                GetExtraVehicleData(modelNumber, chassisNumber, vehicle);
                return vehicle;
            }
            return null;
        }
        private void GetExtraVehicleData(string modelNumber, string chassisNumber, VinResult vehicleData)
        {
            var x = new KtdReader.KtdReader();
            if (chassisNumber.Length > 7)
                chassisNumber = chassisNumber.Substring(chassisNumber.Length - 7);
            var searchKey = modelNumber + chassisNumber.PadLeft(7, '0');
            var record = x.RecordsForKey(_pathToVinDataRT, searchKey, 2, this);
            if (record != null)
            {
                vehicleData.Series = record[9];
                vehicleData.Guide = record[10];
                vehicleData.ShopEquipment = record[11];
                vehicleData.InteriorColour = record[7];
                vehicleData.ExteriorColour = record[8];
                vehicleData.Options = record[17];
                vehicleData.Caratt = record[20];
            }

        }
        public bool ProcessRow(List<string> data, string searchKey)
        {
            
            if (searchKey.Length == 10)
            {
                if (searchKey == data[0])
                    return true;
            }
            else
            {
                if (searchKey[..3] == data[0][..3] &&
                    searchKey.Substring(3, 8) == data[1])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
