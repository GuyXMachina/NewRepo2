using GuitarShop.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarShop.Data
{
    public class FacilityRepository : RepositoryBase<Facility>, IFacilityRepository
    {
        public FacilityRepository(AppDbContext appDbContext) : base(appDbContext) { }

        // Custom methods specific to Facility can go here. For example:
        public IEnumerable<Facility> GetFacilitiesByType(string type)
        {
            return _appDbContext.Facilities.Where(f => f.Category.CategoryName == type).ToList();
        }

        public IEnumerable<Facility> GetFacilitiesWithBookings()
        {
            return _appDbContext.Facilities.Include(f => f.Bookings).ToList();
        }

        // Any more specialized methods can go here
    }

}
