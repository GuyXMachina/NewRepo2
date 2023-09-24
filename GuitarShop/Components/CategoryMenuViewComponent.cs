using GuitarShop.Data;
using GuitarShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShop.Components
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private IRepositoryWrapper _repository;
        public CategoryMenuViewComponent(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            var model = new CategoryListViewModel
            {
                Categories = _repository.Category.FindAll(),
                SelectedCategory = (string)RouteData?.Values["id"]
            };
            return View(model);
        }

    }
}