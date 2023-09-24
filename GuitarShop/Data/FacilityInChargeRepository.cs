using GuitarShop.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarShop.Data
{
    public class FacilityInChargeRepository : RepositoryBase<FacilityInCharge> ,IFacilityInChargeRepository
    {
        public FacilityInChargeRepository(AppDbContext appDbContext) : base(appDbContext) { }
   

        public IEnumerable<Booking> GetBookingsForFacility(int facilityId)
        {
            return _appDbContext.Bookings
                .Include(b => b.Facility)
                .Where(b => b.FacilityID == facilityId)
                .ToList();
        }

        public FacilityInCharge GetProfile(string user)
        {
            // Assuming user is a username or email or some unique identifier for the FacilityInCharge
            return _appDbContext.FacilitiesInCharge.FirstOrDefault(f => f.UserName == user || f.Email == user);
        }

        public void UpdateProfile(FacilityInCharge inCharge)
        {
            _appDbContext.FacilitiesInCharge.Update(inCharge);
            _appDbContext.SaveChanges();
        }

        public void ChangeBookingStatus(int bookingId, string newStatus)
        {
            var booking = _appDbContext.Bookings.FirstOrDefault(b => b.BookingID == bookingId);
            if (booking != null)
            {
                booking.Status = newStatus;
                _appDbContext.Bookings.Update(booking);
                _appDbContext.SaveChanges();
            }
        }
    }
}
