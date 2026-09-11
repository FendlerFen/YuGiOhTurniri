using System;
using System.Configuration;
using System.Web.Http;
using KlasePodataka;
using Repozitorijumi;

namespace Servisa.Controllers
{
    [RoutePrefix("api/ban-lista")]
    public class BanListaApiController : ApiController
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        [Route("")]
        [HttpGet]
        public IHttpActionResult DajSve()
        {
            try
            {
                var repo = new BanListaRepozitorijumSP(_konekcija);
                var lista = repo.DajSvuBanListu();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Ban lista", podaci = lista });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Dodaj([FromBody] dynamic body)
        {
            try
            {
                int sudijaId = body?.sudijaID ?? body?.SudijaID ?? 0;
                string naziv = body?.nazivKarte ?? body?.NazivKarte ?? "";
                if (sudijaId <= 0 || string.IsNullOrWhiteSpace(naziv))
                    return BadRequest("SudijaID i NazivKarte su obavezni");

                var repo = new BanListaRepozitorijumSP(_konekcija);
                int id = repo.DodajNaBanListu(sudijaId, naziv);
                return Content(System.Net.HttpStatusCode.Created,
                    new { uspeh = true, kodStanja = 201, poruka = "Dodato na ban listu", podaci = new { banListaID = id } });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("{id:int}")]
        [HttpDelete]
        public IHttpActionResult Obrisi(int id)
        {
            try
            {
                var repo = new BanListaRepozitorijumSP(_konekcija);
                int n = repo.ObrisiSaBanListe(id);
                return Ok(new { uspeh = n > 0, kodStanja = 200, poruka = n > 0 ? "Obrisano" : "Nije pronadjeno" });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }
    }
}
