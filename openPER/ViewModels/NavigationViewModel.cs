
namespace openPER.ViewModels
{
    public class NavigationViewModel
    {
        public BreadcrumbViewModel Breadcrumb { get; set; }
        public SideMenuItemsViewModel SideMenuItems { get; set; }
        public string Language { get; set; }
        public FilterModel Filter { get; set; }
    }
}
