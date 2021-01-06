using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CpastroneSkiRental.Models
{
    /// <summary>
    /// AgeValidation class to make sure peole registrationg are over 18 years old
    /// </summary>
    public class AgeValidation : ValidationAttribute
    {
        private int AgeLimit;
        public AgeValidation(int ageLimit)
        {
            this.AgeLimit = ageLimit;
        }
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            DateTime birthDate = DateTime.Parse(value.ToString());
            DateTime todayDate = DateTime.Today;
          
            if (birthDate > todayDate.AddYears(-(todayDate.Year - birthDate.Year)))
            {
                _ = (todayDate.Year - birthDate.Year)-1;
            }
            if ((todayDate.Year - birthDate.Year) < AgeLimit)
            {

                var validationResult = new ValidationResult("Age must be 18 and older");
                return validationResult;
            }


            return null;
        }
    }
}
