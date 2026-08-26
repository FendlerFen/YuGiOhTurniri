using System;
using System.Collections.Generic;
using KlasePodataka;
using Repozitorijumi;

namespace Prezentaciona_Logika
{
    public class FormaTurniraKlasa
    {
        private string _konekcija;

        public FormaTurniraKlasa(string konekcija)
        {
            _konekcija = konekcija;
        }

        public List<TurnirKlasa> DajSveTurnire()
        {
            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            return repo.DajSveTurnire();
        }

        public TurnirKlasa DajTurnirPoID(int id)
        {
            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            return repo.DajPoID(id);
        }

        public List<TurnirKlasa> DajOtvoreneTurnire()
        {
            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            return repo.DajOtvoreneTurnire();
        }

        public List<TurnirKlasa> DajTurnireOrganizatora(int organizatorID)
        {
            if (organizatorID <= 0)
                return new List<TurnirKlasa>();

            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            return repo.DajTurnireOrganizatora(organizatorID);
        }

        public string KreirajTurnir(TurnirKlasa turnir)
        {
            if (turnir == null)
                return "Turnir ne sme biti null.";

            if (string.IsNullOrWhiteSpace(turnir.Naziv))
                return "Naziv turnira je obavezan.";

            if (turnir.OrganizatorID <= 0)
                return "Organizator ID nije validan.";

            if (turnir.DatumOdrzavanja == DateTime.MinValue)
                return "Datum održavanja nije validan.";

            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            int rezultat = repo.Dodaj(turnir);
            return rezultat > 0 ? "Turnir kreiran." : "Greska pri kreiranju.";
        }

        public string IzmeniTurnir(TurnirKlasa turnir)
        {
            if (turnir == null || turnir.TurnirID <= 0)
                return "Turnir ID nije validan.";

            if (string.IsNullOrWhiteSpace(turnir.Naziv))
                return "Naziv turnira je obavezan.";

            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            bool rezultat = repo.Izmeni(turnir);
            return rezultat ? "Turnir izmenjen." : "Greska pri izmeni.";
        }

        public string ZavrsiTurnir(int turnirID)
        {
            if (turnirID <= 0)
                return "Turnir ID nije validan.";

            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            bool rezultat = repo.ZavrsiTurnir(turnirID);
            return rezultat ? "Turnir zavrsen." : "Greska pri završavanju.";
        }

        public string ProclasiPobjednike(int turnirID, int prvoMestoID, int drugoMestoID, int treceMestoID)
        {
            if (turnirID <= 0 || prvoMestoID <= 0)
                return "ID vrednosti nisu validne.";

            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            bool rezultat = repo.ProclasiPobjednike(turnirID, prvoMestoID, drugoMestoID, treceMestoID);
            return rezultat ? "Pobednici proglaseni." : "Greska pri proglasavanju.";
        }

        public List<RezultatKlasa> DajRezultate(int turnirID)
        {
            if (turnirID <= 0)
                return new List<RezultatKlasa>();

            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            return repo.DajRezultate(turnirID);
        }
    }
}