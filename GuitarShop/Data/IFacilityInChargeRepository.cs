using GuitarShop.Models;

namespace GuitarShop.Data
{
    public interface IFacilityInChargeRepository : IRepositoryBase<FacilityInCharge>
    {
        FacilityInCharge GetProfile(string user);
        void UpdateProfile(FacilityInCharge inCharge);
    }
}