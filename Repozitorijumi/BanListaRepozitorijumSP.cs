using System;
using System.Collections.Generic;
using KlasePodataka;

namespace Repozitorijumi
{
    public class BanListaRepozitorijumSP : IBanListaRepository
    {
        private readonly string _konekcija;

        public BanListaRepozitorijumSP(string konekcija)
        {
            _konekcija = konekcija;
        }

        public List<BanListaKlasa> DajBanListuSudije(int sudijaID)
        {
            BanListaDBKlasa db = new BanListaDBKlasa(_konekcija);
            return db.DajBanListuSudije(sudijaID);
        }

        public List<BanListaKlasa> DajSvuBanListu()
        {
            BanListaDBKlasa db = new BanListaDBKlasa(_konekcija);
            return db.DajBanListuSudije(0);
        }

        public int DodajNaBanListu(int sudijaID, string nazivKarte)
        {
            BanListaDBKlasa db = new BanListaDBKlasa(_konekcija);
            return db.DodajNaBanListu(sudijaID, nazivKarte);
        }

        public int ObrisiSaBanListe(int banListaID)
        {
            BanListaDBKlasa db = new BanListaDBKlasa(_konekcija);
            return db.ObrisiSaBanListe(banListaID);
        }

        public bool DaLiJeKartaNaBanListi(int sudijaID, string nazivKarte)
        {
            BanListaDBKlasa db = new BanListaDBKlasa(_konekcija);
            return db.DaLiJeKartaNaBanListi(sudijaID, nazivKarte);
        }
    }
}
