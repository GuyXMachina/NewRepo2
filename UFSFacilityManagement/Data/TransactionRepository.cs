using UFSFacilityManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace UFSFacilityManagement.Data
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        // Fetch transactions related to facilities managed by this manager
        public IEnumerable<Transaction> GetByFacilityManager(string facilityManagerUserName)
        {
            return _appDbContext.Transactions
                .Include(t => t.Booking)
                .Where(t => t.Booking.FacilityManager.UserName == facilityManagerUserName)
                .ToList();
        }

        // Fetch transactions related to bookings this in-charge is responsible for
        public IEnumerable<Transaction> GetByInCharge(string inChargeUserName)
        {
            return _appDbContext.Transactions
                .Include(t => t.Booking)
                .Where(t => t.Booking.FacilityInCharge.UserName == inChargeUserName)
                .ToList();
        }
    }


}
