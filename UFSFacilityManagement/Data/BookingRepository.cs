using UFSFacilityManagement.Models;

namespace UFSFacilityManagement.Data
{
    public class BookingRepository : RepositoryBase<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext appdbContext) : base(appdbContext) { }

        public IEnumerable<Booking> GetBookingsByStatus(string status)
        {
            return _appDbContext.Bookings.Where(b => b.Status == status).ToList();
        }

        public IEnumerable<Booking> GetBookingsForFacility(int facilityId)
        {
            return _appDbContext.Bookings.Where(b => b.FacilityID == facilityId).ToList();
        }

        public void ChangeBookingStatus(int bookingId, string newStatus)
        {
            // Get the booking with the specified ID
            var booking = _appDbContext.Bookings.FirstOrDefault(b => b.BookingID == bookingId);

            // Check if booking exists
            if (booking != null)
            {
                // Update the status
                booking.Status = newStatus;

                // Save the changes
                _appDbContext.SaveChanges();
            }
            else
            {
                // Handle the case when the booking is not found, e.g., throw an exception or log an error
                throw new ArgumentException($"No booking found with ID {bookingId}");
            }
        }

    }

}
