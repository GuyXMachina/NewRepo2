using Microsoft.AspNetCore.Identity;

namespace GuitarShop.Data
{
    public interface IUserRepository : IRepositoryBase<IdentityUser>
    {
        IEnumerable<IdentityUser> GetAllUsers();
        IdentityUser GetUserByUsername(string username);
        IdentityUser GetUserByEmail(string email);
        // Add more methods as needed
    }
}