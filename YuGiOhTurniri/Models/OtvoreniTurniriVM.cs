using System;

namespace YuGiOhTurniri.Models
{
    public class OtvoreniTurniriVM
    {
        public int TurnirID { get; set; }
        public string Naziv { get; set; }
        public string Lokacija { get; set; }
        public string Format { get; set; }
        public DateTime DatumOdrzavanja { get; set; }
        public string Organizator { get; set; }
        public string OrganizatorIme { get; set; }
        public int BrojPrijavljenih { get; set; }
        public string Status { get; set; }
        public bool IsUserRegistered { get; set; }
    }
}