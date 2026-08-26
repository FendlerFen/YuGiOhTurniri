using KlasePodataka;
using System.Collections.Generic;
using KlasePodataka;

namespace Repozitorijumi
{
    public interface IBanListaRepository
    {
        List<BanListaKlasa> DajSvuBanListu();
        List<BanListaKlasa> DajBanListuSudije(int sudijaID);
        int DodajNaBanListu(int sudijaID, string nazivKarte);
        int ObrisiSaBanListe(int banListaID);
        bool DaLiJeKartaNaBanListi(int sudijaID, string nazivKarte);
    }
}
