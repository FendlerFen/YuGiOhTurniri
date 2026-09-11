using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KlasePodataka;
using Newtonsoft.Json;

namespace YuGiOhTurniri.Services
{
    /// <summary>
    /// HTTP Klijent Servis - Komunikacija sa REST API-jem
    /// Koristi HTTP GET, POST metode za pozivanje REST API-ja
    /// Primjer sa Takmi?arima
    /// </summary>
    public class HttpKlijentServis
    {
        private readonly string _bazaUrl;
        private readonly HttpClient _httpKlijent;

        public HttpKlijentServis()
        {
            _bazaUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:50000";
            _httpKlijent = new HttpClient();
        }

        #region TAKMICARI

        /// <summary>
        /// GET /api/takmicari - Preuzima sve takmi?are preko HTTP GET
        /// </summary>
        public async Task<List<TakmicarKlasa>> DajSveTakmicareAsync()
        {
            try
            {
                string url = $"{_bazaUrl}/api/takmicari";
                HttpResponseMessage odgovor = await _httpKlijent.GetAsync(url);

                if (!odgovor.IsSuccessStatusCode)
                    throw new Exception($"Gre?ka pri preuzimanju takmi?ara. Kod: {odgovor.StatusCode}");

                string sadrzaj = await odgovor.Content.ReadAsStringAsync();
                dynamic rezultat = JsonConvert.DeserializeObject(sadrzaj);

                if (rezultat.uspeh != true)
                    throw new Exception(rezultat.poruka ?? "Nepoznata gre?ka");

                string podaciJson = JsonConvert.SerializeObject(rezultat.podaci);
                return JsonConvert.DeserializeObject<List<TakmicarKlasa>>(podaciJson);
            }
            catch (Exception ex)
            {
                throw new Exception("Gre?ka pri preuzimanju takmi?ara", ex);
            }
        }

        /// <summary>
        /// GET /api/takmicari/{id} - Preuzima takmi?ara po ID-u preko HTTP GET
        /// </summary>
        public async Task<TakmicarKlasa> DajTakmicaraPoIDAsync(int id)
        {
            try
            {
                string url = $"{_bazaUrl}/api/takmicari/{id}";
                HttpResponseMessage odgovor = await _httpKlijent.GetAsync(url);

                if (!odgovor.IsSuccessStatusCode)
                    throw new Exception($"Takmi?ar nije prona?en. Kod: {odgovor.StatusCode}");

                string sadrzaj = await odgovor.Content.ReadAsStringAsync();
                dynamic rezultat = JsonConvert.DeserializeObject(sadrzaj);

                if (rezultat.uspeh != true)
                    throw new Exception(rezultat.poruka ?? "Nepoznata gre?ka");

                string podaciJson = JsonConvert.SerializeObject(rezultat.podaci);
                return JsonConvert.DeserializeObject<TakmicarKlasa>(podaciJson);
            }
            catch (Exception ex)
            {
                throw new Exception("Gre?ka pri preuzimanju takmi?ara", ex);
            }
        }

        /// <summary>
        /// POST /api/takmicari - Pravi novog takmi?ara preko HTTP POST
        /// </summary>
        public async Task<int> KreirajTakmicaraAsync(TakmicarKlasa takmicar)
        {
            try
            {
                string url = $"{_bazaUrl}/api/takmicari";
                string json = JsonConvert.SerializeObject(takmicar);
                StringContent sadrzaj = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage odgovor = await _httpKlijent.PostAsync(url, sadrzaj);

                if (!odgovor.IsSuccessStatusCode)
                    throw new Exception($"Gre?ka pri kreiranju takmi?ara. Kod: {odgovor.StatusCode}");

                string odgovorTekst = await odgovor.Content.ReadAsStringAsync();
                dynamic rezultat = JsonConvert.DeserializeObject(odgovorTekst);

                if (rezultat.uspeh != true)
                    throw new Exception(rezultat.poruka ?? "Nepoznata gre?ka");

                return Convert.ToInt32(rezultat.podaci.takmicarID);
            }
            catch (Exception ex)
            {
                throw new Exception("Gre?ka pri kreiranju takmi?ara", ex);
            }
        }

        #endregion
    }
}

