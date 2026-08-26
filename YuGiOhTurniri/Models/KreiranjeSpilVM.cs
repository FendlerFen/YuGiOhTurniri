using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuGiOhTurniri.Models
{
    public class KreirajSpilVM
    {
        public int SpilID { get; set; }

        [Required(ErrorMessage = "Naziv spila je obavezan")]
        [StringLength(100, ErrorMessage = "Naziv može imati maksimalno 100 karaktera")]
        public string Naziv { get; set; }

        [Required(ErrorMessage = "Format je obavezan")]
        public string Format { get; set; }

        [StringLength(100, ErrorMessage = "Arhetip može imati maksimalno 100 karaktera")]
        public string Arhetip { get; set; }

        public List<string> Formati { get; set; }

        // Stare karte unos (za kompatibilnost)
        public List<KartaUnosPrikaz> Karte { get; set; }

        // Nove karte unos - odvojene po sekcijama
        public List<MainDeckKarta> MainDeck { get; set; }
        public List<ExtraDeckKarta> ExtraDeck { get; set; }
        public List<SideDeckKarta> SideDeck { get; set; }

        public KreirajSpilVM()
        {
            Karte = new List<KartaUnosPrikaz>();
            MainDeck = new List<MainDeckKarta>();
            ExtraDeck = new List<ExtraDeckKarta>();
            SideDeck = new List<SideDeckKarta>();
        }
    }

    public class KartaUnosPrikaz
    {
        public string NazivKarte { get; set; }
        public string Sekcija { get; set; }
        [Range(1, 3, ErrorMessage = "Količina mora biti između 1 i 3")]
        public int Kolicina { get; set; }
    }

    public class MainDeckKarta
    {
        public string NazivKarte { get; set; }
        public string Tip { get; set; } // Monster, Spell, Trap
        public string Kolicina { get; set; }
    }

    public class ExtraDeckKarta
    {
        public string NazivKarte { get; set; }
        public string Kolicina { get; set; }
    }

    public class SideDeckKarta
    {
        public string NazivKarte { get; set; }
        public string Kolicina { get; set; }
    }

    public class DetaljiSpilVM
    {
        public int SpilID { get; set; }
        public string Naziv { get; set; }
        public string Format { get; set; }
        public string Arhetip { get; set; }
        public string Status { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public string NapomenaSudije { get; set; }
        public List<KartaUSpiluVM> Karte { get; set; }
        public List<string> Formati { get; set; }
    }

    public class KartaUSpiluVM
    {
        public int KartaUSpiluID { get; set; }
        public string NazivKarte { get; set; }
        public string Sekcija { get; set; }
        public int Kolicina { get; set; }
        public string TipKarte { get; set; } // Monster, Spell, Trap
    }

    public class DodajKartuVM
    {
        [Required(ErrorMessage = "Naziv karte je obavezan")]
        public string NazivKarte { get; set; }

        [Required(ErrorMessage = "Sekcija je obavezna")]
        public string Sekcija { get; set; }

        [Required(ErrorMessage = "Količina je obavezna")]
        [Range(1, 3, ErrorMessage = "Količina mora biti između 1 i 3")]
        public byte Kolicina { get; set; }

        public int SpilID { get; set; }
    }

    public class BrojKarataVM
    {
        public int MainDeck { get; set; }
        public int ExtraDeck { get; set; }
        public int SideDeck { get; set; }
    }

    public class MojiSpiloviVM
    {
        public int SpilID { get; set; }
        public string Naziv { get; set; }
        public string Format { get; set; }
        public string Arhetip { get; set; }
        public string Status { get; set; }
        public DateTime DatumKreiranja { get; set; }
    }
}