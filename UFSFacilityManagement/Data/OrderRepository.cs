using UFSFacilityManagement.Models;
using Microsoft.EntityFrameworkCore;
namespace UFSFacilityManagement.Data
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        // Additional methods specific to Order can be added here

        // For example, get orders by a specific user
        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            return _appDbContext.Orders
                .Include(o => o.User)
                .Where(o => o.UserID == userId)
                .ToList();
        }

        // Get orders with their corresponding transactions
        public IEnumerable<Order> GetOrdersWithTransactions()
        {
            return _appDbContext.Orders
                .Include(o => o.Transactions)
                .ToList();
        }
    }
}
