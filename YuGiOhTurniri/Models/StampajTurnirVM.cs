using System;
using System.Collections.Generic;

namespace YuGiOhTurniri.Models
{
    public class StampajTurnirVM
    {
        public int TurnirID { get; set; }
        public string Naziv { get; set; }
        public string Lokacija { get; set; }
        public string Format { get; set; }
        public DateTime DatumOdrzavanja { get; set; }
        public string Status { get; set; }
        public string Organizator { get; set; }
        public List<RezultatPrikazVM> Rezultati { get; set; }
        public int BrojUcesnika { get; set; }
    }
}