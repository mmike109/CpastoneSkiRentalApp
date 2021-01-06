using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneISki.Data
{
    /// <summary>
    /// Payment class was not used due to the use of PayPal API
    /// </summary>
    public class Payment
    {

        [Key]
        public int TransactionId { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public int UserId { get; set; }

        [StringLength(25)]
        public string CardType { get; set; }

        public int CardNumber { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Expirey Date")]
        public DateTime? ExpiryDate { get; set; }


        public int CCV { get; set; }
    }
}
