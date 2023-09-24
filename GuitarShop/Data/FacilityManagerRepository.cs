using GuitarShop.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarShop.Data
{
    public class FacilityManagerRepository : RepositoryBase<FacilityManager>, IFacilityManagerRepository
    {

        public FacilityManagerRepository(AppDbContext appDbContext) : base(appDbContext) { }

        // Additional methods specific to FacilityManager can go here

        public IEnumerable<FacilityManager> GetAllManagers()
        {
            return _appDbContext.FacilityManagers
                .Include(fm => fm.Facilities)  // Assuming a FacilityManager has multiple Facilities
                .ToList();
        }

        public FacilityManager GetManagerById(int id)
        {
            return _appDbContext.FacilityManagers
                .Include(fm => fm.Facilities)
                .FirstOrDefault(fm => fm.FacilityManagerId == id);
        }

        public void UpdateManager(FacilityManager managerToUpdate)
        {
            _appDbContext.FacilityManagers.Update(managerToUpdate);
            _appDbContext.SaveChanges();
        }
    }
}
