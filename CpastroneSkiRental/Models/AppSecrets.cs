using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CpastroneSkiRental.Models
{
    /// <summary>
    /// Holds user and program sensetive data in secrets file
    /// </summary>
    public class AppSecrets
    {
        public string adminPsw { get; set; }
        public string clientId { get; set; }
        public string secret { get; set; }
        public string urlApi { get; set; }
        public string returnUrl { get; set; }
        public string cancelUrl { get; set; }
    }
}
