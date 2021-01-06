using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneISki.Data
{
    /// <summary>
    /// Items model represents the products
    /// </summary>
    public class Item
    {
        [Required]
        [Key]
        public int ItemId { get; set; }

        [Required]
        [StringLength(45)]
        [Display(Name = "Manufacturer")]
        public string Maker { get; set; }

        [Required]
        [StringLength(45)]
        [Display(Name = "Type")]
        public string ItemType { get; set; }

        [Required]
        [StringLength(8)]
        [Display(Name = "Gender")]
        public string ItemGender { get; set; }

 
        [Display(Name = "Size (Numeric)")]
        public double ItemSize { get; set; }


        [StringLength(5)]
        [Display(Name = "Alternative Size (S,L,M)")]
        public string ItemAltSize { get; set; }
        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }
        [StringLength(20)]
        public string Level { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string ItemStatus { get; set; }
        [Display(Name = "Add a picture")]
        public string ItemImage { get; set; }

      
    }
}
