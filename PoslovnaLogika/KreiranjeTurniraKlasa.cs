using System;
using System.Collections.Generic;
using KlasePodataka;
using Repozitorijumi;
using Servisi;

namespace PoslovnaLogika
{
    public class KreiranjeTurniraKlasa
    {
        private readonly ITurnirRepozitorijum _turnirRepo;
        private readonly string _konekcija;

        public KreiranjeTurniraKlasa(string konekcija, ITurnirRepozitorijum turnirRepo)
        {
            _konekcija = konekcija;
            _turnirRepo = turnirRepo;
        }

        public int KreirajTurnir(TurnirKlasa turnir)
        {
            if (!ValidanTurnir(turnir))
                return 0;

            return _turnirRepo.Dodaj(turnir);
        }

        private bool ValidanTurnir(TurnirKlasa turnir)
        {
            if (string.IsNullOrWhiteSpace(turnir.Naziv))
                return false;

            if (string.IsNullOrWhiteSpace(turnir.Lokacija))
                return false;

            if (turnir.DatumOdrzavanja <= DateTime.Now)
                return false;

            var ogranicenja = new OgranicenjaServis();

            if (string.IsNullOrWhiteSpace(turnir.Format))
                return false;

            return true;
        }

        // Metoda sada vraća TurnirKlasa umjesto StampajTurnirVM
        // VM klase se koriste samo u Kontrolerima
        public TurnirKlasa DajTurnirZaStampu(int turnirID)
        {
            var turnir = _turnirRepo.DajPoID(turnirID);

            if (turnir == null)
                return null;

            return turnir;
        }
    }
}