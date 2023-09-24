using GuitarShop.Models;
namespace GuitarShop.Data
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        IEnumerable<Order> GetOrdersByUserId(string userId);
        IEnumerable<Order> GetOrdersWithTransactions();
    }
}