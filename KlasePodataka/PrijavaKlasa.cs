using System;

namespace KlasePodataka
{
    public class PrijavaKlasa
    {
        private int _prijavaID;
        private int _turnirID;
        private int _takmicarID;
        private int _spilID;
        private DateTime _datumPrijave;

        public int PrijavaID
        {
            get { return _prijavaID; }
            set { _prijavaID = value; }
        }

        public int TurnirID
        {
            get { return _turnirID; }
            set { _turnirID = value; }
        }

        public int TakmicarID
        {
            get { return _takmicarID; }
            set { _takmicarID = value; }
        }

        public int SpilID
        {
            get { return _spilID; }
            set { _spilID = value; }
        }

        public DateTime DatumPrijave
        {
            get { return _datumPrijave; }
            set { _datumPrijave = value; }
        }
    }
}