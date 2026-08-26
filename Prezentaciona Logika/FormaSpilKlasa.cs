using System;
using System.Collections.Generic;
using KlasePodataka;
using Repozitorijumi;

namespace Prezentaciona_Logika
{
    public class FormaSpilKlasa
    {
        private string _konekcija;

        public FormaSpilKlasa(string konekcija)
        {
            _konekcija = konekcija;
        }

        public List<SpilKlasa> DajSveSpilove()
        {
            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            return repo.DajSveSpilave();
        }

        public SpilKlasa DajSpilPoID(int id)
        {
            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            return repo.DajPoID(id);
        }

        public List<SpilKlasa> DajSpiloveTakmicara(int takmicarID)
        {
            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            return repo.DajSpiloveTakmicara(takmicarID);
        }

        public List<SpilKlasa> DajSpiloveNaCekanju()
        {
            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            return repo.DajSpiloveNaCekanju();
        }

        public string KreirajSpil(string naziv, string format, string arhetip, int takmicarID)
        {
            if (string.IsNullOrWhiteSpace(naziv))
                return "Naziv spila je obavezan.";

            if (takmicarID <= 0)
                return "Takmičar ID nije validan.";

            SpilKlasa spil = new SpilKlasa
            {
                Naziv = naziv,
                Format = format,
                Arhetip = arhetip,
                TakmicarID = takmicarID,
                Status = "Na cekanju"
            };

            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            int rezultat = repo.Dodaj(spil);
            return rezultat > 0 ? "Spil kreiran." : "Greska pri kreiranju.";
        }

        public string DodajKartuUSpil(int spilID, string nazivKarte, string sekcija, byte kolicina)
        {
            if (string.IsNullOrWhiteSpace(nazivKarte))
                return "Naziv karte je obavezan.";

            if (kolicina < 1 || kolicina > 3)
                return "Količina mora biti između 1 i 3.";

            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            bool rezultat = repo.DodajKartu(spilID, nazivKarte, sekcija, kolicina);
            return rezultat ? "Karta dodata u spil." : "Greska pri dodavanju karte.";
        }

        public List<KartaUSpiluKlasa> DajKarteSpila(int spilID)
        {
            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            return repo.DajKarteSpila(spilID);
        }

        public string OdlukaOSpilu(int spilID, string status, string napomena)
        {
            if (status != "Odobren" && status != "Odbijen")
                return "Status nije validan.";

            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            bool rezultat = repo.PromeniStatus(spilID, status, napomena);
            return rezultat ? "Odluka zapisana." : "Greska pri zapisivanju odluke.";
        }
    }
}