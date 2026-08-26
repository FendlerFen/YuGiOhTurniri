using System;
using System.Collections.Generic;
using KlasePodataka;

namespace Repozitorijumi
{
    public interface ITurnirRepozitorijum
    {
        List<TurnirKlasa> DajSveTurnire();
        TurnirKlasa DajPoID(int id);
        List<TurnirKlasa> DajOtvoreneTurnire();
        List<TurnirKlasa> DajTurnireOrganizatora(int organizatorID);
        int Dodaj(TurnirKlasa turnir);
        bool Izmeni(TurnirKlasa turnir);
        bool ZavrsiTurnir(int id);
        bool ProclasiPobjednike(int turnirID, int prvoMestoID, int drugoMestoID, int treceMestoID);
        List<RezultatKlasa> DajRezultate(int turnirID);
        List<RezultatKlasa> DajPobednike(int turnirID);
    }
}