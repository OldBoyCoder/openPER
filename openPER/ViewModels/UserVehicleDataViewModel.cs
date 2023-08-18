namespace openPER.ViewModels
{
    public class UserVehicleDataViewModel
    {
        public string VIN { get; set; }
        public string Description { get; set; }
        public UserVehicleDataViewModel(string vin, string description)
        {
            VIN = vin;
            Description = description;
        }
    }
}