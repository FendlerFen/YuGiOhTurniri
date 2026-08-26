using KlasePodataka;
using System.Collections.Generic;

namespace Repozitorijumi
{
    public interface ISpilRepozitorijum
    {
        List<SpilKlasa> DajSveSpilave();
        SpilKlasa DajPoID(int id);
        List<SpilKlasa> DajSpiloveTakmicara(int takmicarID);
        List<SpilKlasa> DajSpiloveNaCekanju();
        int Dodaj(SpilKlasa spil);
        bool Obrisi(int id);
        bool DodajKartu(int spilID, string nazivKarte, string sekcija, int kolicina);
        bool DodajKartu(int spilID, string nazivKarte, string sekcija, int kolicina, string tipKarte);
        bool AzurirajKartu(int kartaUSpiluID, string nazivKarte, int kolicina, string tipKarte);
        List<KartaUSpiluKlasa> DajKarteSpila(int spilID);
        bool PromeniStatus(int spilID, string noviStatus, string napomena);
        bool Izmeni(SpilKlasa spil);
    }
}