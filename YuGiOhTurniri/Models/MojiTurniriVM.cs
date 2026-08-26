using System;

namespace YuGiOhTurniri.Models
{
    public class MojiTurniriVM
    {
        public int TurnirID { get; set; }
        public string Naziv { get; set; }
        public string Lokacija { get; set; }
        public string Format { get; set; }
        public DateTime DatumOdrzavanja { get; set; }
        public string Status { get; set; }
        public DateTime DatumKreiranja { get; set; }
    }
}