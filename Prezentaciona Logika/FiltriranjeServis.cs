using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KlasePodataka;

namespace Prezentaciona_Logika
{
    /// Servis za filtriranje podataka prema parametrima iz JSON-a
    /// Koristi se za implementaciju poslovnih pravila filtriranja
    public class FiltriranjeServis
    {
        private dynamic _parametri;

        public FiltriranjeServis()
        {
            UcitajParametre();
        }

        private void UcitajParametre()
        {
            try
            {
                string putanja = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ogranicenja.json"
                );

                if (!File.Exists(putanja))
                {
                    throw new FileNotFoundException($"Fajl nije pronaden: {putanja}");
                }

                string json = File.ReadAllText(putanja);
                _parametri = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FiltriranjeServis.UcitajParametre Error: {ex.Message}");
                _parametri = null;
            }
        }


        /// Filtrira spilove po formatu
        public List<SpilKlasa> FiltrirajPoFormatu(List<SpilKlasa> spilovi, string format)
        {
            if (string.IsNullOrWhiteSpace(format) || spilovi == null)
                return spilovi ?? new List<SpilKlasa>();

            return spilovi
                .Where(s => s.Format.Equals(format, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// Filtrira spilove po imenu (sadrzi pretragu)
        public List<SpilKlasa> FiltrirajPoImenu(List<SpilKlasa> spilovi, string imePretraga)
        {
            if (string.IsNullOrWhiteSpace(imePretraga) || spilovi == null)
                return spilovi ?? new List<SpilKlasa>();

            string pretraga = imePretraga.ToLower().Trim();
            return spilovi
                .Where(s => s.Naziv.ToLower().Contains(pretraga))
                .ToList();
        }


        /// Filtrira spilove po statusu
        public List<SpilKlasa> FiltrirajPoStatusu(List<SpilKlasa> spilovi, string status)
        {
            if (string.IsNullOrWhiteSpace(status) || spilovi == null)
                return spilovi ?? new List<SpilKlasa>();

            return spilovi
                .Where(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// Kombinovano filtriranje - format i naziv
        public List<SpilKlasa> Filtriraj(List<SpilKlasa> spilovi, string format, string imePretraga, string status = "")
        {
            var rezultat = spilovi ?? new List<SpilKlasa>();

            if (!string.IsNullOrWhiteSpace(format))
                rezultat = FiltrirajPoFormatu(rezultat, format);

            if (!string.IsNullOrWhiteSpace(imePretraga))
                rezultat = FiltrirajPoImenu(rezultat, imePretraga);

            if (!string.IsNullOrWhiteSpace(status))
                rezultat = FiltrirajPoStatusu(rezultat, status);

            return rezultat;
        }

        /// Dohvata sve dostupne formate iz parametara
        public List<string> DajDostupneFormate()
        {
            try
            {
                if (_parametri == null || _parametri.formati == null)
                    return new List<string> { "TCG", "OCG", "Speed Duel" };

                var formati = new List<string>();
                foreach (var f in _parametri.formati)
                {
                    formati.Add((string)f.naziv);
                }
                return formati;
            }
            catch
            {
                return new List<string> { "TCG", "OCG", "Speed Duel" };
            }
        }

        /// Dohvata sve dostupne statuse iz parametara
        public List<string> DajDostupneStatuse()
        {
            try
            {
                if (_parametri == null || _parametri.statusiSpila == null)
                    return new List<string> { "Na cekanju", "Odobren", "Odbijen" };

                var statusi = new List<string>();
                foreach (var s in _parametri.statusiSpila)
                {
                    statusi.Add((string)s.naziv);
                }
                return statusi;
            }
            catch
            {
                return new List<string> { "Na cekanju", "Odobren", "Odbijen" };
            }
        }

        /// Filtrira turnire po formatu
        public List<TurnirKlasa> FiltrirajTurnirePoFormatu(List<TurnirKlasa> turniri, string format)
        {
            if (string.IsNullOrWhiteSpace(format) || turniri == null)
                return turniri ?? new List<TurnirKlasa>();

            return turniri
                .Where(t => t.Format.Equals(format, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// Filtrira turnire po imenu
        public List<TurnirKlasa> FiltrirajTurnirePoImenu(List<TurnirKlasa> turniri, string imePretraga)
        {
            if (string.IsNullOrWhiteSpace(imePretraga) || turniri == null)
                return turniri ?? new List<TurnirKlasa>();

            string pretraga = imePretraga.ToLower().Trim();
            return turniri
                .Where(t => t.Naziv.ToLower().Contains(pretraga))
                .ToList();
        }

        /// Kombinovano filtriranje za turnire
        public List<TurnirKlasa> FiltrirajTurnire(List<TurnirKlasa> turniri, string format, string imePretraga, string status = "")
        {
            var rezultat = turniri ?? new List<TurnirKlasa>();

            if (!string.IsNullOrWhiteSpace(format))
                rezultat = FiltrirajTurnirePoFormatu(rezultat, format);

            if (!string.IsNullOrWhiteSpace(imePretraga))
                rezultat = FiltrirajTurnirePoImenu(rezultat, imePretraga);

            if (!string.IsNullOrWhiteSpace(status))
                rezultat = rezultat.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

            return rezultat;
        }
    }
}
