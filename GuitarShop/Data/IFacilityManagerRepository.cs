using GuitarShop.Models;

namespace GuitarShop.Data
{
    public interface IFacilityManagerRepository : IRepositoryBase<FacilityManager>
    {
        // Additional methods specific to FacilityManager
        IEnumerable<FacilityManager> GetAllManagers();
        FacilityManager GetManagerById(int id);
        void UpdateManager(FacilityManager managerToUpdate);

        // ... any other methods needed
    }
}