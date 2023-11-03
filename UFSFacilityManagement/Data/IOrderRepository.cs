using UFSFacilityManagement.Models;
namespace UFSFacilityManagement.Data
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        IEnumerable<Order> GetOrdersByUserId(string userId);
        IEnumerable<Order> GetOrdersWithTransactions();
    }
}