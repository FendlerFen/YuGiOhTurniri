using System;

namespace KlasePodataka
{
    public class SpilKlasa
    {
        private int _spilID;
        private string _naziv;
        private string _format;
        private string _arhetip;
        private string _status;
        private int _takmicarID;
        private DateTime _datumKreiranja;
        private string _napomenaSudije;

        public int SpilID
        {
            get { return _spilID; }
            set { _spilID = value; }
        }

        public string Naziv
        {
            get { return _naziv; }
            set { _naziv = value; }
        }

        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        public string Arhetip
        {
            get { return _arhetip; }
            set { _arhetip = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public int TakmicarID
        {
            get { return _takmicarID; }
            set { _takmicarID = value; }
        }

        public DateTime DatumKreiranja
        {
            get { return _datumKreiranja; }
            set { _datumKreiranja = value; }
        }

        public string NapomenaSudije
        {
            get { return _napomenaSudije; }
            set { _napomenaSudije = value; }
        }
    }
}