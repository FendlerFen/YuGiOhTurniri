using System;
using System.Collections.Generic;
using System.Linq;
using KlasePodataka;

namespace Repozitorijumi
{
    public class BanListaRepozitorijumEF : IBanListaRepository
    {
        private readonly string _konekcija;

        public BanListaRepozitorijumEF(string konekcija)
        {
            _konekcija = konekcija;
        }

        public List<BanListaKlasa> DajSvuBanListu()
        {
            try
            {
                using (var dbContext = new YuGiOhDBEntities1())
                {
                    return DajBanListuSudije(0);
                }
            }
            catch
            {
                return new List<BanListaKlasa>();
            }
        }

        public List<BanListaKlasa> DajBanListuSudije(int sudijaID)
        {
            SPBanListaDBKlasa db = new SPBanListaDBKlasa(_konekcija);
            return db.DajBanListuSudije(sudijaID);
        }

        public int DodajNaBanListu(int sudijaID, string nazivKarte)
        {
            SPBanListaDBKlasa db = new SPBanListaDBKlasa(_konekcija);
            return db.DodajNaBanListu(sudijaID, nazivKarte);
        }

        public int ObrisiSaBanListe(int banListaID)
        {
            SPBanListaDBKlasa db = new SPBanListaDBKlasa(_konekcija);
            return db.ObrisiSaBanListe(banListaID);
        }

        public bool DaLiJeKartaNaBanListi(int sudijaID, string nazivKarte)
        {
            SPBanListaDBKlasa db = new SPBanListaDBKlasa(_konekcija);
            return db.DaLiJeKartaNaBanListi(sudijaID, nazivKarte);
        }
    }
}
