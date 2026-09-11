using System;
using System.Configuration;
using System.Web.Http;
using KlasePodataka;
using Repozitorijumi;

namespace Servisa.Controllers
{
    [RoutePrefix("api/organizatori")]
    public class OrganizatorApiController : ApiController
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        [Route("{id:int}")]
        [HttpGet]
        public IHttpActionResult DajPoId(int id)
        {
            try
            {
                var repo = new OrganizatorRepozitorijumSP(_konekcija);
                var o = repo.DajPoID(id);
                if (o == null) return NotFound();
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Organizator", podaci = o });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Dodaj([FromBody] OrganizatorKlasa organizator)
        {
            try
            {
                if (organizator == null) return BadRequest("Telo je prazno");
                var repo = new OrganizatorRepozitorijumSP(_konekcija);
                int id = repo.Dodaj(organizator);
                organizator.OrganizatorID = id;
                return Content(System.Net.HttpStatusCode.Created,
                    new { uspeh = true, kodStanja = 201, poruka = "Organizator kreiran", podaci = organizator });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }

        [Route("prijava")]
        [HttpPost]
        public IHttpActionResult Prijava([FromBody] dynamic model)
        {
            try
            {
                string email = model?.email ?? model?.Email;
                string lozinka = model?.lozinka ?? model?.Lozinka;
                var repo = new OrganizatorRepozitorijumSP(_konekcija);
                var o = repo.Login(email, lozinka);
                if (o == null)
                    return Content(System.Net.HttpStatusCode.Unauthorized,
                        new { uspeh = false, kodStanja = 401, poruka = "Pogresan email ili lozinka" });
                return Ok(new { uspeh = true, kodStanja = 200, poruka = "Prijava uspesna", podaci = o });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                    new { uspeh = false, kodStanja = 500, poruka = ex.Message });
            }
        }
    }
}
