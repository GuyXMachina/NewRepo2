using UFSFacilityManagement.Models;
namespace UFSFacilityManagement.Data
{
    public interface IBookingRepository : IRepositoryBase<Booking>
    {
        IEnumerable<Booking> GetBookingsByStatus(string status);
        IEnumerable<Booking> GetBookingsForFacility(int facilityId);
        void ChangeBookingStatus(int bookingId, string newStatus);
    }
}
