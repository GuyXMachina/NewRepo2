using UFSFacilityManagement.Models;

namespace UFSFacilityManagement.Data
{
    public interface IFacilityRepository : IRepositoryBase<Facility>
    {
        IEnumerable<Facility> GetFacilitiesByType(string type);
        IEnumerable<Facility> GetFacilitiesWithBookings();
    }

}