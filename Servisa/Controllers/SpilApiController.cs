using System;
using System.Configuration;
using System.Web.Http;
using KlasePodataka;
using Repozitorijumi;

namespace Servisa.Controllers
{
    [RoutePrefix("api/spilovi")]
    public class SpilApiController : ApiController
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        [Route("")]
        [HttpGet]
        public IHttpActionResult DajSve()
        {
            try
            {
                var repo = new SpilRepozitorijumSP(_konekcija);
                var lista = repo.DajSveSpilave();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Spilovi preuzeti", podaci = lista });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("na-cekanju")]
        [HttpGet]
        public IHttpActionResult NaCekanju()
        {
            try
            {
                var repo = new SpilRepozitorijumSP(_konekcija);
                var lista = repo.DajSpiloveNaCekanju();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Spilovi na cekanju", podaci = lista });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("{id:int}")]
        [HttpGet]
        public IHttpActionResult DajPoId(int id)
        {
            try
            {
                var repo = new SpilRepozitorijumSP(_konekcija);
                var s = repo.DajPoID(id);
                if (s == null) return NotFound();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Spil pronadjen", podaci = s });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("takmicar/{takmicarId:int}")]
        [HttpGet]
        public IHttpActionResult PoTakmicaru(int takmicarId)
        {
            try
            {
                var repo = new SpilRepozitorijumSP(_konekcija);
                var lista = repo.DajSpiloveTakmicara(takmicarId);
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Spilovi takmicara", podaci = lista });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("{id:int}/karte")]
        [HttpGet]
        public IHttpActionResult Karte(int id)
        {
            try
            {
                var repo = new SpilRepozitorijumSP(_konekcija);
                var lista = repo.DajKarteSpila(id);
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Karte spila", podaci = lista });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Dodaj([FromBody] SpilKlasa spil)
        {
            try
            {
                if (spil == null) return BadRequest("Telo je prazno");
                var repo = new SpilRepozitorijumSP(_konekcija);
                int id = repo.Dodaj(spil);
                spil.SpilID = id;
                return Content(System.Net.HttpStatusCode.Created,
                    new { uspeh = true, kodStanja = 201, poruka = "Spil kreiran", podaci = spil });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("{id:int}/status")]
        [HttpPut]
        public IHttpActionResult PromeniStatus(int id, [FromBody] dynamic body)
        {
            try
            {
                string status = body?.status ?? body?.Status ?? "";
                string napomena = body?.napomena ?? body?.Napomena ?? "";
                var repo = new SpilRepozitorijumSP(_konekcija);
                bool ok = repo.PromeniStatus(id, status, napomena);
                return Ok(new { uspeh = ok, kodStanja = 200, poruka = ok ? "Status promenjen" : "Nije promenjen" });
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
                var repo = new SpilRepozitorijumSP(_konekcija);
                bool ok = repo.Obrisi(id);
                return Ok(new { uspeh = ok, kodStanja = 200, poruka = ok ? "Spil obrisan" : "Nije obrisan" });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }
    }
}
