using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneISki.Data
{
    /// <summary>
    /// User model class was ultimately not used due to usage of the secured identity system 
    /// </summary>
    public class User
    {
        [Required]
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [StringLength(40)]
        public string Email { get; set; }

        [Required]
        public double ShoeSize { get; set; } 

        [Required]
        public double PantsSize { get; set; }

        [Required]
        [StringLength(2)]
        public string HelmetSize { get; set; }

        [Required]
        [StringLength(2)]
        public string GlovesSize { get; set; }

        [Required]
        public double Height { get; set; }

        [Required]
        public double Weight { get; set; }

        [Required]
        [StringLength(15)]
        public string Level { get; set; }

        [Required]
        public bool IsAdmin { get; set; }


    }
}
