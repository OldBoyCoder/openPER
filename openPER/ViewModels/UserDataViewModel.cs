using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class UserDataViewModel
    {
        public int UserId { get; set; }
        public bool AdvertFree { get; set; }
        public List<UserVehicleDataViewModel> Vehicles { get; set; } = new List<UserVehicleDataViewModel>();
    }
}