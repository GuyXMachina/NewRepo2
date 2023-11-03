using UFSFacilityManagement.Models;

namespace UFSFacilityManagement.Data
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }
    }
}
