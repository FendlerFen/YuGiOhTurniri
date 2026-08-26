using System;
using KlasePodataka;

namespace Repozitorijumi
{
    public interface IOrganizatorRepozitorijum
    {
        int Dodaj(OrganizatorKlasa organizator);
        OrganizatorKlasa DajPoID(int organizatorID);
        OrganizatorKlasa Login(string email, string lozinka);
    }

    public class OrganizatorRepozitorijumSP : IOrganizatorRepozitorijum
    {
        private readonly string _konekcija;

        public OrganizatorRepozitorijumSP(string konekcija)
        {
            _konekcija = konekcija;
        }

        public int Dodaj(OrganizatorKlasa organizator)
        {
            SPOrganizatorDBKlasa db = new SPOrganizatorDBKlasa(_konekcija);
            return db.Registruj(organizator);
        }

        public OrganizatorKlasa DajPoID(int organizatorID)
        {
            SPOrganizatorDBKlasa db = new SPOrganizatorDBKlasa(_konekcija);
            return db.DajPoID(organizatorID);
        }

        public OrganizatorKlasa Login(string email, string lozinka)
        {
            SPOrganizatorDBKlasa db = new SPOrganizatorDBKlasa(_konekcija);
            return db.Login(email, lozinka);
        }
    }
}
