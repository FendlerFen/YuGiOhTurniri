using System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KlasePodataka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YuGiOhTurniri.Services
{
    /// <summary>
    /// HTTP klijent – prezentacioni sloj poziva REST API (Servisa projekat).
    /// </summary>
    public class HttpKlijentServis
    {
        private readonly string _bazaUrl;
        private readonly HttpClient _http;

        public HttpKlijentServis()
        {
            _bazaUrl = (ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:44334").TrimEnd('/');

            // Ignoriši SSL certifikat greške za self-signed sertifikate
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        }

        #region Generički pomoćnici

        private async Task<T> GetPodaciAsync<T>(string relativnaPutanja)
        {
            try
            {
                string url = _bazaUrl + relativnaPutanja;
                HttpResponseMessage odgovor = await _http.GetAsync(url).ConfigureAwait(false);
                string json = await odgovor.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!odgovor.IsSuccessStatusCode)
                    throw new Exception($"HTTP {(int)odgovor.StatusCode}: {json}");

                JObject root = JObject.Parse(json);
                if (root["uspeh"] != null && root["uspeh"].Type != JTokenType.Null && root["uspeh"].Value<bool>() == false)
                    throw new Exception(root["poruka"]?.ToString() ?? "API greška");

                if (root["podaci"] == null || root["podaci"].Type == JTokenType.Null)
                    return default(T);

                return root["podaci"].ToObject<T>();
            }
            catch (HttpRequestException hex)
            {
                throw new Exception($"Greška pri konekciji sa API-jem na {_bazaUrl}{relativnaPutanja}: {hex.Message} (Provjerite da li je API projekt pokrenut)", hex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri GET {relativnaPutanja}: {ex.Message}", ex);
            }
        }

        private async Task<T> PostPodaciAsync<T>(string relativnaPutanja, object telo)
        {
            try
            {
                string url = _bazaUrl + relativnaPutanja;
                var sadrzaj = new StringContent(JsonConvert.SerializeObject(telo), Encoding.UTF8, "application/json");
                HttpResponseMessage odgovor = await _http.PostAsync(url, sadrzaj).ConfigureAwait(false);
                string json = await odgovor.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!odgovor.IsSuccessStatusCode)
                    throw new Exception($"HTTP {(int)odgovor.StatusCode}: {json}");

                JObject root = JObject.Parse(json);
                if (root["uspeh"] != null && root["uspeh"].Type != JTokenType.Null && root["uspeh"].Value<bool>() == false)
                    throw new Exception(root["poruka"]?.ToString() ?? "API greška");

                if (root["podaci"] == null || root["podaci"].Type == JTokenType.Null)
                    return default(T);

                return root["podaci"].ToObject<T>();
            }
            catch (HttpRequestException hex)
            {
                throw new Exception($"Greška pri konekciji sa API-jem na {_bazaUrl}{relativnaPutanja}: {hex.Message} (Provjerite da li je API projekt pokrenut)", hex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri POST {relativnaPutanja}: {ex.Message}", ex);
            }
        }

        private async Task PutAsync(string relativnaPutanja, object telo)
        {
            try
            {
                string url = _bazaUrl + relativnaPutanja;
                var sadrzaj = new StringContent(JsonConvert.SerializeObject(telo ?? new { }), Encoding.UTF8, "application/json");
                HttpResponseMessage odgovor = await _http.PutAsync(url, sadrzaj).ConfigureAwait(false);
                string json = await odgovor.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!odgovor.IsSuccessStatusCode)
                    throw new Exception($"HTTP {(int)odgovor.StatusCode}: {json}");
            }
            catch (HttpRequestException hex)
            {
                throw new Exception($"Greška pri konekciji sa API-jem na {_bazaUrl}{relativnaPutanja}: {hex.Message} (Provjerite da li je API projekt pokrenut)", hex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri PUT {relativnaPutanja}: {ex.Message}", ex);
            }
        }

        private async Task DeleteAsync(string relativnaPutanja)
        {
            try
            {
                string url = _bazaUrl + relativnaPutanja;
                HttpResponseMessage odgovor = await _http.DeleteAsync(url).ConfigureAwait(false);
                string json = await odgovor.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!odgovor.IsSuccessStatusCode)
                    throw new Exception($"HTTP {(int)odgovor.StatusCode}: {json}");
            }
            catch (HttpRequestException hex)
            {
                throw new Exception($"Greška pri konekciji sa API-jem na {_bazaUrl}{relativnaPutanja}: {hex.Message} (Provjerite da li je API projekt pokrenut)", hex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri DELETE {relativnaPutanja}: {ex.Message}", ex);
            }
        }

        // Sinhroni omotači za starije MVC akcije
        public T GetSync<T>(string path) => GetPodaciAsync<T>(path).GetAwaiter().GetResult();
        public T PostSync<T>(string path, object body) => PostPodaciAsync<T>(path, body).GetAwaiter().GetResult();
        public void PutSync(string path, object body) => PutAsync(path, body).GetAwaiter().GetResult();
        public void DeleteSync(string path) => DeleteAsync(path).GetAwaiter().GetResult();

        #endregion

        #region TAKMICARI

        public Task<List<TakmicarKlasa>> DajSveTakmicareAsync()
            => GetPodaciAsync<List<TakmicarKlasa>>("/api/takmicari");

        public Task<TakmicarKlasa> DajTakmicaraPoIDAsync(int id)
            => GetPodaciAsync<TakmicarKlasa>($"/api/takmicari/{id}");

        public async Task<int> KreirajTakmicaraAsync(TakmicarKlasa takmicar)
        {
            var kreiran = await PostPodaciAsync<TakmicarKlasa>("/api/takmicari", takmicar).ConfigureAwait(false);
            return kreiran?.TakmicarID ?? 0;
        }

        public Task<TakmicarKlasa> PrijavaTakmicaraAsync(string email, string lozinka)
            => PostPodaciAsync<TakmicarKlasa>("/api/takmicari/prijava", new { email, lozinka });

        #endregion

        #region TURNIRI

        public Task<List<TurnirKlasa>> DajSveTurnireAsync()
            => GetPodaciAsync<List<TurnirKlasa>>("/api/turniri");

        public Task<List<TurnirKlasa>> DajOtvoreneTurnireAsync()
            => GetPodaciAsync<List<TurnirKlasa>>("/api/turniri/otvoreni");

        public Task<TurnirKlasa> DajTurnirPoIDAsync(int id)
            => GetPodaciAsync<TurnirKlasa>($"/api/turniri/{id}");

        public Task<List<TurnirKlasa>> DajTurnireOrganizatoraAsync(int organizatorId)
            => GetPodaciAsync<List<TurnirKlasa>>($"/api/turniri/organizator/{organizatorId}");

        public async Task<int> KreirajTurnirAsync(TurnirKlasa turnir)
        {
            var t = await PostPodaciAsync<TurnirKlasa>("/api/turniri", turnir).ConfigureAwait(false);
            return t?.TurnirID ?? 0;
        }

        public Task ZavrsiTurnirAsync(int id)
            => PutAsync($"/api/turniri/{id}/zavrsi", null);

        public Task<List<RezultatKlasa>> DajRezultateAsync(int turnirId)
            => GetPodaciAsync<List<RezultatKlasa>>($"/api/turniri/{turnirId}/rezultati");

        #endregion

        #region SPILOVI

        public Task<List<SpilKlasa>> DajSveSpiloveAsync()
            => GetPodaciAsync<List<SpilKlasa>>("/api/spilovi");

        public Task<List<SpilKlasa>> DajSpiloveNaCekanjuAsync()
            => GetPodaciAsync<List<SpilKlasa>>("/api/spilovi/na-cekanju");

        public Task<SpilKlasa> DajSpilPoIDAsync(int id)
            => GetPodaciAsync<SpilKlasa>($"/api/spilovi/{id}");

        public Task<List<SpilKlasa>> DajSpiloveTakmicaraAsync(int takmicarId)
            => GetPodaciAsync<List<SpilKlasa>>($"/api/spilovi/takmicar/{takmicarId}");

        public Task<List<KartaUSpiluKlasa>> DajKarteSpilaAsync(int spilId)
            => GetPodaciAsync<List<KartaUSpiluKlasa>>($"/api/spilovi/{spilId}/karte");

        public async Task<int> KreirajSpilAsync(SpilKlasa spil)
        {
            var s = await PostPodaciAsync<SpilKlasa>("/api/spilovi", spil).ConfigureAwait(false);
            return s?.SpilID ?? 0;
        }

        public Task PromeniStatusSpilaAsync(int id, string status, string napomena)
            => PutAsync($"/api/spilovi/{id}/status", new { status, napomena });

        public Task ObrisiSpilAsync(int id)
            => DeleteAsync($"/api/spilovi/{id}");

        #endregion

        #region BAN LISTA

        public Task<List<BanListaKlasa>> DajBanListuAsync()
            => GetPodaciAsync<List<BanListaKlasa>>("/api/ban-lista");

        public Task DodajNaBanListuAsync(int sudijaId, string nazivKarte)
            => PostPodaciAsync<object>("/api/ban-lista", new { sudijaID = sudijaId, nazivKarte });

        public Task ObrisiSaBanListeAsync(int banListaId)
            => DeleteAsync($"/api/ban-lista/{banListaId}");

        #endregion

        #region ORGANIZATORI

        public Task<OrganizatorKlasa> DajOrganizatoraAsync(int id)
            => GetPodaciAsync<OrganizatorKlasa>($"/api/organizatori/{id}");

        public async Task<int> KreirajOrganizatoraAsync(OrganizatorKlasa o)
        {
            var r = await PostPodaciAsync<OrganizatorKlasa>("/api/organizatori", o).ConfigureAwait(false);
            return r?.OrganizatorID ?? 0;
        }

        public Task<OrganizatorKlasa> PrijavaOrganizatoraAsync(string email, string lozinka)
            => PostPodaciAsync<OrganizatorKlasa>("/api/organizatori/prijava", new { email, lozinka });

        #endregion
    }
}
