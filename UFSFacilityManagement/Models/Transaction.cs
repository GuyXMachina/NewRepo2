using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UFSFacilityManagement.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int BookingID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentMethod { get; set; } // Credit Card, Cash, etc.

        // Navigation property
        public Booking Booking { get; set; }
    }
}
