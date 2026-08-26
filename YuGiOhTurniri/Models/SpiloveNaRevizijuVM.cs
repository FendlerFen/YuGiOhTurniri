using System;

namespace YuGiOhTurniri.Models
{
    public class SpiloveNaRevizijuVM
    {
        public int SpilID { get; set; }
        public string Naziv { get; set; }
        public string Format { get; set; }
        public string Arhetip { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public string VlasnikIme { get; set; }
        public string VlasnikPrezime { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }
}