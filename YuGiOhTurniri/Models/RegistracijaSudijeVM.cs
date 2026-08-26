using System.ComponentModel.DataAnnotations;

namespace YuGiOhTurniri.Models
{
    public class RegistracijaSudijeVM
    {
        [Required(ErrorMessage = "Ime je obavezno")]
        [StringLength(100, MinimumLength = 2)]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno")]
        [StringLength(100, MinimumLength = 2)]
        public string Prezime { get; set; }

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Unesite validan email")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Lozinka mora biti izme?u 6 karaktera")]
        public string Lozinka { get; set; }

        [Required(ErrorMessage = "Potvrda lozinke je obavezna")]
        [Compare("Lozinka", ErrorMessage = "Lozinke se ne podudaraju")]
        public string PotvrdaLozinke { get; set; }
    }
}
