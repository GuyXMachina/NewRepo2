using UFSFacilityManagement.Models;

namespace UFSFacilityManagement.Data
{
    public interface IFacilityInChargeRepository : IRepositoryBase<FacilityInCharge>
    {
        FacilityInCharge GetProfile(string user);
        void UpdateProfile(FacilityInCharge inCharge);
    }
}