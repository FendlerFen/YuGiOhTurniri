using System;
using System.Configuration;
using System.Web.Http;
using KlasePodataka;
using Repozitorijumi;

namespace Servisa.Controllers
{
    [RoutePrefix("api/turniri")]
    public class TurnirApiController : ApiController
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        [Route("")]
        [HttpGet]
        public IHttpActionResult DajSve()
        {
            try
            {
                var repo = new TurnirRepozitorijumSP(_konekcija);
                var lista = repo.DajSveTurnire();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Turniri preuzeti", podaci = lista });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("otvoreni")]
        [HttpGet]
        public IHttpActionResult DajOtvorene()
        {
            try
            {
                var repo = new TurnirRepozitorijumSP(_konekcija);
                var lista = repo.DajOtvoreneTurnire();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Otvoreni turniri", podaci = lista });
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
                var repo = new TurnirRepozitorijumSP(_konekcija);
                var t = repo.DajPoID(id);
                if (t == null) return NotFound();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Turnir pronadjen", podaci = t });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("organizator/{organizatorId:int}")]
        [HttpGet]
        public IHttpActionResult DajPoOrganizatoru(int organizatorId)
        {
            try
            {
                var repo = new TurnirRepozitorijumSP(_konekcija);
                var lista = repo.DajTurnireOrganizatora(organizatorId);
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Turniri organizatora", podaci = lista });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Dodaj([FromBody] TurnirKlasa turnir)
        {
            try
            {
                if (turnir == null) return BadRequest("Telo je prazno");
                var repo = new TurnirRepozitorijumSP(_konekcija);
                int id = repo.Dodaj(turnir);
                turnir.TurnirID = id;
                return Content(System.Net.HttpStatusCode.Created,
                    new { uspeh = true, kodStanja = 201, poruka = "Turnir kreiran", podaci = turnir });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("{id:int}/zavrsi")]
        [HttpPut]
        public IHttpActionResult Zavrsi(int id)
        {
            try
            {
                var repo = new TurnirRepozitorijumSP(_konekcija);
                bool ok = repo.ZavrsiTurnir(id);
                return Ok(new { uspeh = ok, kodStanja = 200, poruka = ok ? "Turnir zavrsen" : "Nije zavrsen" });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("{id:int}/rezultati")]
        [HttpGet]
        public IHttpActionResult Rezultati(int id)
        {
            try
            {
                var repo = new TurnirRepozitorijumSP(_konekcija);
                var lista = repo.DajPobednike(id);
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Rezultati", podaci = lista });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }
    }
}
