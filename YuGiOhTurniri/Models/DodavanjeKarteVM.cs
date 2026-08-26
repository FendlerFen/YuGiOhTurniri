using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace YuGiOhTurniri.Models
{
    public class DodavanjeKarteVM
    {
        public int SpilID { get; set; }
        public string NazivSpila { get; set; }

        [Required(ErrorMessage = "Naziv karte je obavezan")]
        [StringLength(150)]
        public string NazivKarte { get; set; }

        [Required(ErrorMessage = "Sekcija je obavezna")]
        public string Sekcija { get; set; }

        [Required(ErrorMessage = "Količina je obavezna")]
        [Range(1, 3)]
        public byte Kolicina { get; set; }

        public List<string> Sekcije { get; set; }
    }
}