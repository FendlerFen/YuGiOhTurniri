using System;

namespace KlasePodataka
{
    public class KartaUSpiluKlasa
    {
        private int _kartaUSpiluID;
        private int _spilID;
        private string _nazivKarte;
        private string _sekcija;
        private byte _kolicina;
        private string _tipKarte;

        public int KartaUSpiluID
        {
            get { return _kartaUSpiluID; }
            set { _kartaUSpiluID = value; }
        }

        public int SpilID
        {
            get { return _spilID; }
            set { _spilID = value; }
        }

        public string NazivKarte
        {
            get { return _nazivKarte; }
            set { _nazivKarte = value; }
        }

        public string Sekcija
        {
            get { return _sekcija; }
            set { _sekcija = value; }
        }

        public byte Kolicina
        {
            get { return _kolicina; }
            set { _kolicina = value; }
        }

        public string TipKarte
        {
            get { return _tipKarte; }
            set { _tipKarte = value; }
        }
    }
}