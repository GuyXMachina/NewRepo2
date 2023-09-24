namespace GuitarShop.Models.ViewModels
{
    public class FacilityListViewModel
    {
        public IEnumerable<Facility> Facilities { get; set; }
        public string SelectedCategory { get; set; }
        public PagingInfoViewModel PagingInfo { get; set; }
    }
}
