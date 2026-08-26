using System;
using System.ComponentModel.DataAnnotations;

namespace YuGiOhTurniri.Models
{
    public class RegistracijaTakmicaraVM
    {
        [Required(ErrorMessage = "Ime je obavezno")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ime mora biti između 2 i 100 karaktera")]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Prezime mora biti između 2 i 100 karaktera")]
        public string Prezime { get; set; }

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Datum rodjenja je obavezan")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(RegistracijaTakmicaraVM), "ValidateStarost")]
        public DateTime DatumRodjenja { get; set; }

        [Required(ErrorMessage = "Država je obavezna")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Država mora biti između 2 i 100 karaktera")]
        public string Drzava { get; set; }

        [Required(ErrorMessage = "Pol je obavezan")]
        public string Pol { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Lozinka mora biti najmanje 6 karaktera")]
        public string Lozinka { get; set; }

        [Compare("Lozinka", ErrorMessage = "Lozinke se ne poklapaju")]
        public string PotvrdiLozinku { get; set; }

        public static ValidationResult ValidateStarost(DateTime datumRodjenja)
        {
            int starost = DateTime.Now.Year - datumRodjenja.Year;
            if (datumRodjenja.Date > DateTime.Now.AddYears(-starost)) starost--;

            if (starost < 15)
                return new ValidationResult("Takmičar mora biti star najmanje 15 godina");

            return ValidationResult.Success;
        }
    }
}