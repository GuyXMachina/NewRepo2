using GuitarShop.Models;

namespace GuitarShop.Data
{
    public interface IFacilityRepository : IRepositoryBase<Facility>
    {
        IEnumerable<Facility> GetFacilitiesByType(string type);
        IEnumerable<Facility> GetFacilitiesWithBookings();
    }

}