using Microsoft.AspNetCore.Identity;
using GuitarShop.Models;

namespace GuitarShop.Data
{
    public interface IUserRepository : IRepositoryBase<User>
    {
      
        IEnumerable<User> GetAllUsers();
        User GetUserByUsername(string username);
        User GetUserByEmail(string email);
        // Add more methods as needed
    }
}