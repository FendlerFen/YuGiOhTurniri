using System;
using KlasePodataka;

namespace PoslovnaLogika
{
    public class TerminKlasa
    {
        public int TakmicarID { get; set; }
        public DateTime Datum { get; set; }
        public TimeSpan Vreme { get; set; }

        public bool ValidanTermin()
        {
            TimeSpan pocetak = new TimeSpan(8, 0, 0);
            TimeSpan kraj = new TimeSpan(20, 0, 0);
            return (Vreme >= pocetak && Vreme <= kraj);
        }

        public bool ValidanRazmak(TimeSpan drugoVreme)
        {
            return Math.Abs((Vreme - drugoVreme).TotalMinutes) >= 15;
        }
    }
}
