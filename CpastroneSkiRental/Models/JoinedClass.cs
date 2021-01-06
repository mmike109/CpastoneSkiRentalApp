using CapstoneISki.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CpastroneSkiRental.Models
{
    /// <summary>
    /// Joined class represents both single and bundle products
    /// </summary>
    public class JoinedClass
    {
        [Display(Name = "Product ID")]
        public int ProductId { get; set; }
        [Display(Name = "Item ID")]
        public int ItemId { get; set; }
        [Display(Name = "Deal ID")]
        public Guid DealId { get; set; }
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Display(Name = "Rent Date")]
        [DataType(DataType.Date)]
        public DateTime? RentalStartDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        public DateTime? RentalReturnDate { get; set; }
        [Display(Name = "Days Rented")]
        public int DayesRented { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Status")]
        public string ItemStatus { get; set; }
        [Display(Name = "Condition")]
        public string Condition { get; set; }
    }
}
