using System;
using System.Collections.Generic;
using KlasePodataka;

namespace YuGiOhTurniri.Models
{
    public class OdaberiSpilZaTurnirVM
    {
        public int TurnirID { get; set; }
        public string TurnirNaziv { get; set; }
        public string TurnirFormat { get; set; }
        public List<SpilKlasa> Spilovi { get; set; }

        public OdaberiSpilZaTurnirVM()
        {
            Spilovi = new List<SpilKlasa>();
        }
    }
}
