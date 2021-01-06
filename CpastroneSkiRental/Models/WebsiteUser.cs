using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CpastroneSkiRental.Models
{
    /// <summary>
    /// Custom user data used for registration
    /// </summary>
    public class WebsiteUser: IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public DateTime? BirthDate { get; set; }
        [PersonalData]
        public int Height { get; set; }
        [PersonalData]
        public int Weight { get; set; }
        [PersonalData]
        public string Gender { get; set; }
        [PersonalData]
        public double ShoeSize { get; set; }
        [PersonalData]
        public double PantSize { get; set; }
        [PersonalData]
        public string HelmetSize { get; set; }
        [PersonalData]
        public string GloveSize { get; set; }
        [PersonalData]
        public string Level { get; set; }
    }
}
