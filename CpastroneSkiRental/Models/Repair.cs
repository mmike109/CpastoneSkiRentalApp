using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CpastroneSkiRental.Models
{
    /// <summary>
    /// Repairs model represents the items on repair
    /// </summary>
    public class Repair
    {
        [Key]
        public int RepairId { get; set; }
        [ForeignKey("Rental")]
        [Display(Name = "Product ID")]
        public int ProductId { get; set; }

        [ForeignKey("Item")]
        [Display(Name = "Item ID")]
        public int ItemId { get; set; }
        [ForeignKey("Deal")]
        [Display(Name = "Deal ID")]
        public Guid DealId { get; set; }
        [Required]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Required]
        public string Condition { get; set; }
    }
}
