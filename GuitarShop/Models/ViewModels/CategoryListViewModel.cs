namespace GuitarShop.Models.ViewModels
{
    public class CategoryListViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public string SelectedCategory { get; set; }
        public string CheckActiveCategory(string category) =>
        category == SelectedCategory ? "active" : "";
    }
}
