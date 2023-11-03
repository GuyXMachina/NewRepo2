using UFSFacilityManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace UFSFacilityManagement.Data
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

        public FacilityManager GetManagerById(string id)
        {
            return _appDbContext.FacilityManagers
                .Include(fm => fm.Facilities)
                .FirstOrDefault(fm => fm.Id == id);  // Compare the Id property, not the object
        }

        public void UpdateManager(FacilityManager managerToUpdate)
        {
            _appDbContext.FacilityManagers.Update(managerToUpdate);
            _appDbContext.SaveChanges();
        }
    }
}
