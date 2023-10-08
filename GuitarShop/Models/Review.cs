using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuitarShop.Models
{
    public class Review
    {
        [Key]
        public int RatingID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("Facility")]
        public int FacilityID { get; set; }
        public int RatingValue { get; set; } // 1 to 5
        public string Comment { get; set; } // optional
        public DateTime Date { get; set; }
    }
}
