using System;
using System.Collections.Generic;

namespace YuGiOhTurniri.Models
{
    public class StampajSpilVM
    {
        public SpilPrikazVM Spil { get; set; }
        public List<KartaPrikazVM> Karte { get; set; }
        public string Vlasnik { get; set; }
        public string TakmicarIme { get; set; }
        public Dictionary<string, int> BrojKarataPoSekcijama { get; set; }
    }
}