using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlasePodataka
{
    public class KartaKlasa
    {
        public int KartaUSpiluID { get; set; }
        public int SpilID { get; set; }
        public string NazivKarte { get; set; }
        public string Sekcija { get; set; } // "Main","Extra","Side"
        public byte Kolicina { get; set; }
    }
}
