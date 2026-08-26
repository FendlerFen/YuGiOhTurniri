using System;
using System.Collections.Generic;
using KlasePodataka;

namespace Repozitorijumi
{
    public interface ITakmicarRepozitorijum
    {
        List<TakmicarKlasa> DajSveTakmicara();
        TakmicarKlasa DajPoID(int id);
        int Dodaj(TakmicarKlasa takmicar);
        TakmicarKlasa Login(string email, string lozinka);
    }
}