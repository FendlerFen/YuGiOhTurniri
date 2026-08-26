using System;

namespace KlasePodataka
{
    public class OrganizatorKlasa
    {
        public int OrganizatorID { get; set; }
        public string NazivOrganizacije { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string TelefonBroj { get; set; }
        public string Drzava { get; set; }
        public string Lozinka { get; set; }
        public DateTime DatumRegistracije { get; set; }
    }
}
