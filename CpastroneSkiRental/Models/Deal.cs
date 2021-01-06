using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Deal model represents deal items
/// </summary>
namespace CpastroneSkiRental.Models
{
    public class Deal
    {
        [Required]
        [Key]
        [Display(Name = "Deal ID")]
        public Guid DealId { get; set; }
        [Required]
        [Display(Name = "Item description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Price")]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Deal Category")]
        public string RentType { get; set; }

        [Required]
        [StringLength(8)]
        [Display(Name = "Gender")]
        public string DealGender { get; set; }

        [Display(Name = "Size (Numeric)")]
        public double ItemSize { get; set; }

        [StringLength(5)]
        [Display(Name = "Alternative Size (S,L,M)")]
        public string ItemAltSize { get; set; }

        
        [Display(Name = "Status")]
        public string ItemStatus { get; set; }
        [Display(Name ="Image")]
        public string ItemImage { get; set; }
    }
}
