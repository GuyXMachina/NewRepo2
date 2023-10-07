using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuitarShop.Models
{
    public enum PricingType
    {
        Daily,
        Weekly,
        Monthly
    }

    public class Facility
    {
        [Key]
        public int FacilityID { get; set; }

        [Required(ErrorMessage = "Please enter a facility code.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Please enter a facility name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a facility price.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999.99, ErrorMessage = "Price must be between 0 and 9999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please specify a pricing type.")]
        public PricingType PricingType { get; set; }

        public decimal? DiscountPercent { get; set; }
        public decimal DiscountAmount => (DiscountPercent ?? 0) * Price;
        public decimal DiscountPrice => Price - DiscountAmount;

        [Required(ErrorMessage = "Please select a category.")]
        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        public string Location { get; set; }
        public int Capacity { get; set; }
        public string AvailabilityTimes { get; set; }

        // Navigation properties
        public Category Category { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public FacilityManager FacilityManager { get; set; }
    }
}
