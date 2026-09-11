using System;
using System;
using KlasePodataka;
using Repozitorijumi;

namespace Prezentaciona_Logika
{
    public class FormaTakmicaraKlasa
    {
        private string _konekcija;

        public FormaTakmicaraKlasa(string konekcija)
        {
            _konekcija = konekcija;
        }

        public string RegistrujTakmicara(string ime, string prezime, string email, DateTime datumRodjenja, string drzava, string pol, string lozinka)
        {
            if (string.IsNullOrWhiteSpace(ime) || string.IsNullOrWhiteSpace(prezime))
                return "Ime i prezime su obavezni.";

            if (string.IsNullOrWhiteSpace(email))
                return "Email je obavezan.";

            if (datumRodjenja > DateTime.Now)
                return "Datum rodjenja nije validan.";

            // Provera starosti - mora biti bar 15 godina
            int starost = DateTime.Now.Year - datumRodjenja.Year;
            if (datumRodjenja.Date > DateTime.Now.AddYears(-starost)) starost--;

            if (starost < 15)
                return "Takmicara mora biti star najmanje 15 godina.";

            if (string.IsNullOrWhiteSpace(lozinka) || lozinka.Length < 6)
                return "Lozinka mora imati najmanje 6 karaktera.";

            TakmicarKlasa takmicar = new TakmicarKlasa
            {
                Ime = ime,
                Prezime = prezime,
                Email = email,
                DatumRodjenja = datumRodjenja,
                Drzava = drzava,
                Pol = pol,
                Lozinka = lozinka
            };

            try
            {
                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                int rezultat = repo.Dodaj(takmicar);

                if (rezultat > 0)
                    return "Takmicara uspesno registrovan.";
                else
                    return "Greska pri registraciji - Email mozda vec postoji.";
            }
            catch (Exception ex)
            {
                return "Greska pri registraciji: " + ex.Message;
            }
        }

        public string RegistrujTakmicaraSaPorukom(TakmicarKlasa takmicar)
        {
            if (takmicar == null)
                return "Takmičar objekat je null.";

            if (string.IsNullOrWhiteSpace(takmicar.Ime) || string.IsNullOrWhiteSpace(takmicar.Prezime))
                return "Ime i prezime su obavezni.";

            if (string.IsNullOrWhiteSpace(takmicar.Email))
                return "Email je obavezan.";

            if (takmicar.DatumRodjenja > DateTime.Now)
                return "Datum rodjenja nije validan.";

            // Provera starosti - mora biti bar 15 godina
            int starost = DateTime.Now.Year - takmicar.DatumRodjenja.Year;
            if (takmicar.DatumRodjenja.Date > DateTime.Now.AddYears(-starost)) starost--;

            if (starost < 15)
                return "Takmičara mora biti star najmanje 15 godina.";

            if (string.IsNullOrWhiteSpace(takmicar.Lozinka) || takmicar.Lozinka.Length < 6)
                return "Lozinka mora imati najmanje 6 karaktera.";

            try
            {
                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                int rezultat = repo.Dodaj(takmicar);

                if (rezultat > 0)
                    return "Takmičara uspešno registrovan.";
                else
                    return "Greška pri registraciji - Email možda već postoji.";
            }
            catch (Exception ex)
            {
                return "Greška pri registraciji: " + ex.Message;
            }
        }

        public TakmicarKlasa LoginTakmicar(string email, string lozinka)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(lozinka))
                return null;

            ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
            return repo.Login(email, lozinka);
        }
    }
}