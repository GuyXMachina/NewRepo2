using Microsoft.AspNetCore.Identity;

namespace GuitarShop.Data
{
    public class UserRepository : RepositoryBase<IdentityUser>, IUserRepository
    {
        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        // Fetch all users
        public IEnumerable<IdentityUser> GetAllUsers()
        {
            return _appDbContext.Users.ToList();
        }

        // Fetch a user by username
        public IdentityUser GetUserByUsername(string username)
        {
            return _appDbContext.Users.FirstOrDefault(u => u.UserName == username);
        }

        // Fetch a user by email
        public IdentityUser GetUserByEmail(string email)
        {
            return _appDbContext.Users.FirstOrDefault(u => u.Email == email);
        }

        // Add more methods as needed, for example, to fetch bookings for a user, etc.
    }
}
