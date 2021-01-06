using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneISki.Data
{
    /// <summary>
    /// Rental class represents the rental orders
    /// </summary>
    public class Rental
    {
        [Required]
        [Key]
        public int ProductId { get; set; }
       
        [ForeignKey("Item")]
        [Display(Name = "Item ID")]
        public int ItemId { get; set; }
        [ForeignKey("Deal")]
        [Display(Name = "Deal ID")]
        public Guid DealId { get; set; }
        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Required]
        [StringLength(40)]
        [Display(Name = "User")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Item description")]
        public string ItemDescription { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Rent Date")]
        public DateTime? RentalStartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        public DateTime? RentalReturnDate { get; set; }

        [Required]
        [Display(Name = "Days Rented")]
        public int DayesRented { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string ItemStatus { get;set; }
    }
}
