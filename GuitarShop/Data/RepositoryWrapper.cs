using GuitarShop.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarShop.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private AppDbContext _appDbContext;

        private ICategoryRepository _category;
        private IBookingRepository _booking;
        private IFacilityRepository _facility;
        private IFacilityManagerRepository _facilityManager;
        private ITransactionRepository _transaction;
        private IFacilityInChargeRepository _facilityInCharge;
        private IOrderRepository _order;
        private IUserRepository _user;

        public RepositoryWrapper(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public ICategoryRepository Category
        {
            get
            {
                if (_category == null)
                {
                    _category = new CategoryRepository(_appDbContext);
                }
                return _category;
            }
        }

        public IBookingRepository Booking
        {
            get
            {
                if (_booking == null)
                {
                    _booking = new BookingRepository(_appDbContext);
                }
                return _booking;
            }
        }

        public IFacilityRepository Facility
        {
            get
            {
                if (_facility == null)
                {
                    _facility = new FacilityRepository(_appDbContext);
                }
                return _facility;
            }
        }

        public IFacilityInChargeRepository FacilityInCharge
        {
            get
            {
                if (_facilityInCharge == null)
                {
                    _facilityInCharge = new FacilityInChargeRepository(_appDbContext);
                }
                return _facilityInCharge;
            }
        }

        public IFacilityManagerRepository FacilityManager
        {
            get
            {
                if (_facilityManager == null)
                {
                    _facilityManager = new FacilityManagerRepository(_appDbContext);
                }
                return _facilityManager;
            }
        }

        public ITransactionRepository Transaction
        {
            get
            {
                if (_transaction == null)
                {
                    _transaction = new TransactionRepository(_appDbContext);
                }
                return _transaction;
            }
        }

        public IOrderRepository Order
        {
            get
            {
                if (_order == null)
                {
                    _order = new OrderRepository(_appDbContext);
                }
                return _order;
            }
        }

        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_appDbContext);
                }
                return _user;
            }
        }

        public void Save()
        {
            try
            {
                _appDbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Log the error, unwrap the inner exception, etc.
                throw new Exception("There was a problem saving changes: " + ex.InnerException?.Message);
            }
        }
    }
}
