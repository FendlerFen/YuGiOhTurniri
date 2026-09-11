using System;
using System.Configuration;
using System.Web.Http;
using KlasePodataka;
using Repozitorijumi;

namespace Servisa.Controllers
{
    [RoutePrefix("api/takmicari")]
    public class TakmicarApiController : ApiController
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        [Route("")]
        [HttpGet]
        public IHttpActionResult DajSve()
        {
            try
            {
                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                var takmicari = repo.DajSveTakmicara();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Takmicari uspesno preuzeti", broj = takmicari.Count, podaci = takmicari });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = "Greska: " + ex.Message });
            }
        }

        [Route("{id:int}")]
        [HttpGet]
        public IHttpActionResult DajPoId(int id)
        {
            try
            {
                if (id <= 0) return BadRequest("ID mora biti veci od 0");
                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                var takmicar = repo.DajPoID(id);
                if (takmicar == null) return NotFound();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Takmicar pronadjen", podaci = takmicar });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = "Greska: " + ex.Message });
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Dodaj([FromBody] TakmicarKlasa takmicar)
        {
            try
            {
                if (takmicar == null) return BadRequest("Telo zahteva je prazno");
                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                int noviId = repo.Dodaj(takmicar);
                if (noviId <= 0)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                        new { uspeh = false, kodStanja = 400, poruka = "Registracija nije uspela" });
                takmicar.TakmicarID = noviId;
                return Content(System.Net.HttpStatusCode.Created,
                    new { uspeh = true, kodStanja = 201, poruka = "Takmicar kreiran", podaci = takmicar });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = "Greska: " + ex.Message });
            }
        }

        /// <summary>POST /api/takmicari/prijava</summary>
        [Route("prijava")]
        [HttpPost]
        public IHttpActionResult Prijava([FromBody] dynamic model)
        {
            try
            {
                string email = model?.email ?? model?.Email;
                string lozinka = model?.lozinka ?? model?.Lozinka;
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(lozinka))
                    return BadRequest("Email i lozinka su obavezni");

                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                var takmicar = repo.Login(email, lozinka);
                if (takmicar == null)
                    return Content(System.Net.HttpStatusCode.Unauthorized,
                        new { uspeh = false, kodStanja = 401, poruka = "Pogresan email ili lozinka" });

                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Prijava uspesna", podaci = takmicar });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = "Greska: " + ex.Message });
            }
        }
    }
}
