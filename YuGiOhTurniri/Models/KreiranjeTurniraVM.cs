using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace YuGiOhTurniri.Models
{
    public class KreirajTurnirVM
    {
        [Required(ErrorMessage = "Naziv je obavezan")]
        [StringLength(150, ErrorMessage = "Naziv moze imati maksimalno 150 karaktera")]
        public string Naziv { get; set; }

        [Required(ErrorMessage = "Lokacija je obavezna")]
        [StringLength(200, ErrorMessage = "Lokacija moze imati maksimalno 200 karaktera")]
        public string Lokacija { get; set; }

        [Required(ErrorMessage = "Format je obavezan")]
        public string Format { get; set; }

        [Required(ErrorMessage = "Datum odrzavanja je obavezan")]
        [DataType(DataType.Date)]
        public DateTime DatumOdrzavanja { get; set; }

        public List<SelectListItem> Formati { get; set; }
    }

    public class ProclasiPobjednikeVM
    {
        public int TurnirID { get; set; }

        [Required(ErrorMessage = "Prvo mesto je obavezno")]
        public int PrvoMestoID { get; set; }

        [Required(ErrorMessage = "Drugo mesto je obavezno")]
        public int DrugoMestoID { get; set; }

        [Required(ErrorMessage = "Trece mesto je obavezno")]
        public int TreceMestoID { get; set; }

        public List<SelectListItem> Takmicari { get; set; }
    }
}
