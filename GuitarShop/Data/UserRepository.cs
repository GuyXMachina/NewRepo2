using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GuitarShop.Models;

namespace GuitarShop.Data
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }


        // Fetch all users
        public IEnumerable<User> GetAllUsers()
        {
            return _appDbContext.Users.ToList();
        }

        // Fetch a user by username
        public User GetUserByUsername(string username)
        {
            return _appDbContext.Users.FirstOrDefault(u => u.UserName == username);
        }

        // Fetch a user by email
        public User GetUserByEmail(string email)
        {
            return _appDbContext.Users.FirstOrDefault(u => u.Email == email);
        }

        // Add more methods as needed, for example, to fetch bookings for a user, etc.
    }
}
