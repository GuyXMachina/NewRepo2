using GuitarShop.Models;
using Microsoft.AspNetCore.Identity;

namespace GuitarShop.Data
{
    public interface IRepositoryWrapper
    {
        ICategoryRepository Category { get; }
        IFacilityRepository Facility { get; }
        IFacilityInChargeRepository FacilityInCharge { get; }
        IFacilityManagerRepository FacilityManager { get; }
        IOrderRepository Order { get; }
        IUserRepository User { get; }
        IBookingRepository Booking { get; }
        ITransactionRepository Transaction { get; }
        
        void Save();
    }

}
