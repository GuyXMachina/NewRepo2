using UFSFacilityManagement.Models;
namespace UFSFacilityManagement.Data
{
    public interface ITransactionRepository : IRepositoryBase<Transaction>
    {
        // Get transactions related to a specific Facility Manager
        IEnumerable<Transaction> GetByFacilityManager(string facilityManagerUserName);

        // Get transactions related to a specific Facility In-charge
        IEnumerable<Transaction> GetByInCharge(string inChargeUserName);

    }
}