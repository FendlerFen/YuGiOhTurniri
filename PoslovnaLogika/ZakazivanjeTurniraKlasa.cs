using System;
using KlasePodataka;
using Repozitorijumi;

namespace PoslovnaLogika
{
    public class ZakazivanjeTurniraKlasa
    {
        private string _konekcija;

        public ZakazivanjeTurniraKlasa(string konekcija)
        {
            _konekcija = konekcija;
        }

        public bool ValidanTurnir(TurnirKlasa turnir)
        {
            if (turnir == null)
                return false;

            if (string.IsNullOrWhiteSpace(turnir.Naziv))
                return false;

            if (turnir.OrganizatorID <= 0)
                return false;

            if (turnir.DatumOdrzavanja == DateTime.MinValue)
                return false;

            if (turnir.DatumOdrzavanja < DateTime.Now)
                return false;

            return true;
        }

        public bool ImaSlodanaMjesta(int turnirID)
        {
            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            TurnirKlasa turnir = repo.DajPoID(turnirID);

            if (turnir == null)
                return false;

            return true;
        }

        public int DajBrojUcesnika(int turnirID)
        {
            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            TurnirKlasa turnir = repo.DajPoID(turnirID);

            if (turnir == null)
                return 0;

            return 0;
        }
    }
}
