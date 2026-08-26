using System;
using System.Collections.Generic;
using System.Data;
using KlasePodataka;
using Repozitorijumi;
using Servisi;

namespace PoslovnaLogika
{
    public class ValidacijaSpillaKlasa
    {
        private readonly string _konekcija;

        public ValidacijaSpillaKlasa(string konekcija)
        {
            _konekcija = konekcija;
        }

        // ========================
        // VALIDACIJA CIJELOG SPILA
        // ========================
        public bool ValidanSpil(SpilKlasa spil, List<KartaUSpiluKlasa> karte)
        {
            if (string.IsNullOrWhiteSpace(spil.Naziv))
                return false;

            if (string.IsNullOrWhiteSpace(spil.Format))
                return false;

            if (!ValidanBrojKarata(karte))
                return false;

            if (!ValidanFormat(spil.Format))
                return false;

            return true;
        }

        // ========================
        // VALIDACIJA POJEDINE KARTE
        // ========================
        public bool ValidnaKarta(string nazivKarte, string sekcija, byte kolicina)
        {
            // Provjeri da li je karta zabranjena
            if (JeKartaZabranjena(nazivKarte))
                return false;

            // Provjeri sekciju
            if (sekcija != "Main" && sekcija != "Extra" && sekcija != "Side")
                return false;

            // Provjeri količinu
            if (kolicina < 1 || kolicina > 3)
                return false;

            return true;
        }

        // ========================
        // BROJ KARATA PO SEKCIJAMA
        // ========================
        private bool ValidanBrojKarata(List<KartaUSpiluKlasa> karte)
        {
            var ogranicenja = new OgranicenjaServis();

            int mainCount = 0;
            int extraCount = 0;
            int sideCount = 0;

            foreach (var karta in karte)
            {
                if (karta.Sekcija == "Main")
                    mainCount += karta.Kolicina;
                else if (karta.Sekcija == "Extra")
                    extraCount += karta.Kolicina;
                else if (karta.Sekcija == "Side")
                    sideCount += karta.Kolicina;
            }

            // Provjeri Main
            if (mainCount < ogranicenja.DajMinBrojKarataMain() ||
                mainCount > ogranicenja.DajMaxBrojKarataMain())
                return false;

            // Provjeri Extra
            if (extraCount > ogranicenja.DajMaxBrojKarataExtra())
                return false;

            // Provjeri Side
            if (sideCount > ogranicenja.DajMaxBrojKarataSide())
                return false;

            return true;
        }

        // ========================
        // VALIDACIJA FORMATA - PUBLIC
        // ========================
        public bool ValidanFormat(string format)
        {
            return format == "TCG" || format == "OCG" || format == "Speed Duel";
        }

        // ========================
        // PROVJERI DA LI JE KARTA ZABRANJENA
        // ========================
        private bool JeKartaZabranjena(string nazivKarte)
        {
            // Provjeri u bazi podataka
            using (System.Data.SqlClient.SqlConnection conn =
                new System.Data.SqlClient.SqlConnection(_konekcija))
            {
                System.Data.SqlClient.SqlCommand cmd =
                    new System.Data.SqlClient.SqlCommand(
                        "SELECT COUNT(*) FROM BannedKarta WHERE NazivKarte = @Naziv AND Aktivan = 1",
                        conn);
                cmd.Parameters.AddWithValue("@Naziv", nazivKarte);

                conn.Open();
                int broj = (int)cmd.ExecuteScalar();

                return broj > 0;
            }
        }
    }
}