using System;
using System.Collections.Generic;

namespace YuGiOhTurniri.Models
{
    /// <summary>
    /// Zajedničke helper klase koje se koriste u više View Models
    /// </summary>

    public class SpilPrikazVM
    {
        public int SpilID { get; set; }
        public string Naziv { get; set; }
        public string Format { get; set; }
        public string Arhetip { get; set; }
        public string Status { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public string NapomenaSudije { get; set; }
    }

    public class KartaPrikazVM
    {
        public int KartaUSpiluID { get; set; }
        public string NazivKarte { get; set; }
        public string Sekcija { get; set; }
        public byte Kolicina { get; set; }
        public string TipKarte { get; set; }
    }

    public class RezultatPrikazVM
    {
        public int Mesto { get; set; }
        public string Takmicari { get; set; }
        public int BrojPobeda { get; set; }
    }

    // Alias za kompatibilnost - samo referenca na istu klasu
    public class RezultatItemVM : RezultatPrikazVM
    {
    }
}