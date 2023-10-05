using System.ComponentModel.DataAnnotations;

namespace GuitarShop.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Please enter a category name.")]
        public string CategoryName { get; set; }

        //Navigation property
        public List<Facility> Facilities { get; set; }
    }
}
