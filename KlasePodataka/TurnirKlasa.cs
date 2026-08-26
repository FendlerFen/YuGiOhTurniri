using System;
using System;

namespace KlasePodataka
{
    public class TurnirKlasa
    {
        private int _turnirID;
        private string _naziv;
        private string _lokacija;
        private string _format;
        private DateTime _datumOdrzavanja;
        private string _status;
        private int _organizatorID;
        private DateTime _datumKreiranja;

        public int TurnirID
        {
            get { return _turnirID; }
            set { _turnirID = value; }
        }

        public string Naziv
        {
            get { return _naziv; }
            set { _naziv = value; }
        }

        public string Lokacija
        {
            get { return _lokacija; }
            set { _lokacija = value; }
        }

        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        public DateTime DatumOdrzavanja
        {
            get { return _datumOdrzavanja; }
            set { _datumOdrzavanja = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public int OrganizatorID
        {
            get { return _organizatorID; }
            set { _organizatorID = value; }
        }

        public DateTime DatumKreiranja
        {
            get { return _datumKreiranja; }
            set { _datumKreiranja = value; }
        }
    }
}