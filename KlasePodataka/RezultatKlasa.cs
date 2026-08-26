using System;

namespace KlasePodataka
{
    public class RezultatKlasa
    {
        public int RezultatID { get; set; }
        public int TurnirID { get; set; }
        public int TakmicarID { get; set; }
        public int Mesto { get; set; }
        public DateTime DatumProglasenja { get; set; }

        // Za prikaz
        public string Takmicari { get; set; }
        public int BrojPobeda { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
    }
}