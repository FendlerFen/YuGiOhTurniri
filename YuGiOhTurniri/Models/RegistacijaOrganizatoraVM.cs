using System;
using System.ComponentModel.DataAnnotations;

namespace YuGiOhTurniri.Models
{
    public class RegistacijaOrganizatoraVM
    {
        [Required(ErrorMessage = "Naziv organizacije je obavezan")]
        [StringLength(200, MinimumLength = 2)]
        public string NazivOrganizacije { get; set; }

        [Required(ErrorMessage = "Ime osobe odgovorne je obavezno")]
        [StringLength(100, MinimumLength = 2)]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno")]
        [StringLength(100, MinimumLength = 2)]
        public string Prezime { get; set; }

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefonski broj je obavezan")]
        [StringLength(20, MinimumLength = 7)]
        public string TelefonBroj { get; set; }

        [Required(ErrorMessage = "Drzava je obavezna")]
        [StringLength(100, MinimumLength = 2)]
        public string Drzava { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [StringLength(256, MinimumLength = 6)]
        public string Lozinka { get; set; }

        [Compare("Lozinka", ErrorMessage = "Lozinke se ne poklapaju")]
        public string PotvrdiLozinku { get; set; }
    }
}
