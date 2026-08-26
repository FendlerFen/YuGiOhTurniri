using System;
using System.ComponentModel.DataAnnotations;

namespace YuGiOhTurniri.Models
{
    public class PrijavaVM
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [StringLength(256, MinimumLength = 6)]
        public string Lozinka { get; set; }
    }
}