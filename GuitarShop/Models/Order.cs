using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GuitarShop.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Status could be something like "Pending", "Completed", "Shipped", etc.
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        // Navigation property for the user
        public IdentityUser User { get; set; }

        // Navigation property for the transactions related to this order
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
