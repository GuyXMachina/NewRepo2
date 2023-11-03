using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UFSFacilityManagement.Models
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
        [Display(Name = "Facility ID")]
        public int FacilityID { get; set; }

        [Required(ErrorMessage = "Please enter a facility code.")]
        [Display(Name = "Facility Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Please enter a facility name.")]
        [Display(Name = "Facility Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a facility price.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999.99, ErrorMessage = "Price must be between 0 and 9999.99")]
        [Display(Name = "Facility Price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please specify a pricing type.")]
        [Display(Name = "Pricing Type")]
        public PricingType PricingType { get; set; }

        [Display(Name = "Discount Percentage")]
        public decimal? DiscountPercent { get; set; }

        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount => Math.Min(Price, (DiscountPercent ?? 0) * Price / 100);

        [Display(Name = "Discounted Price")]
        public decimal DiscountPrice => Price - DiscountAmount;

        [Required(ErrorMessage = "Please select a category.")]
        [ForeignKey("Category")]
        [Display(Name = "Category ID")]
        public int CategoryID { get; set; }

        [Display(Name = "Facility Location")]
        public string Location { get; set; }

        [Display(Name = "Facility Capacity")]
        public int Capacity { get; set; }

        [Display(Name = "Availability Times")]
        public string AvailabilityTimes { get; set; }

        [Display(Name = "Picture Url")]
        public string Picture { get; set; }

        // Navigation properties
        [Display(Name = "Category")]
        public Category Category { get; set; }

        [Display(Name = "Bookings")]
        public ICollection<Booking> Bookings { get; set; }

        [Display(Name = "Facility Manager")]
        public FacilityManager FacilityManager { get; set; }
    }
}
