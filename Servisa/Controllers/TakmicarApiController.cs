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

                return Ok(new
                {
                    uspeh = true,
                    kodStanja = 200,
                    poruka = "Takmi?ari uspe?no preuzeti",
                    broj = takmicari.Count,
                    podaci = takmicari
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Gre?ka: " + ex.Message));
            }
        }

        [Route("{id:int}")]
        [HttpGet]
        public IHttpActionResult DajPoId(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("ID mora biti ve?i od 0");

                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                var takmicar = repo.DajPoID(id);

                if (takmicar == null)
                    return NotFound();

                return Ok(new
                {
                    uspeh = true,
                    kodStanja = 200,
                    poruka = "Takmi?ar prona?en",
                    podaci = takmicar
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Gre?ka: " + ex.Message));
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Dodaj([FromBody] TakmicarKlasa takmicar)
        {
            try
            {
                if (takmicar == null)
                    return BadRequest("Takmi?ar je obavezan");

                if (string.IsNullOrWhiteSpace(takmicar.Email))
                    return BadRequest("Email je obavezan");

                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                int noviID = repo.Dodaj(takmicar);

                if (noviID <= 0)
                    return BadRequest("Email ve? postoji");

                takmicar.TakmicarID = noviID;

                return Created(Request.RequestUri + "/" + noviID, new
                {
                    uspeh = true,
                    kodStanja = 201,
                    poruka = "Takmi?ar kreiran",
                    podaci = takmicar
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Gre?ka: " + ex.Message));
            }
        }

        [Route("{id:int}")]
        [HttpPut]
        public IHttpActionResult Izmeni(int id, [FromBody] TakmicarKlasa takmicar)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("ID mora biti ve?i od 0");

                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                var postojeci = repo.DajPoID(id);

                if (postojeci == null)
                    return NotFound();

                takmicar.TakmicarID = id;
                repo.Izmeni(id, takmicar);

                return Ok(new
                {
                    uspeh = true,
                    kodStanja = 200,
                    poruka = "Takmi?ar a?uriran",
                    podaci = takmicar
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Gre?ka: " + ex.Message));
            }
        }

        [Route("{id:int}")]
        [HttpDelete]
        public IHttpActionResult Obrisi(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("ID mora biti ve?i od 0");

                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                bool uspeh = repo.Obrisi(id);

                if (!uspeh)
                    return NotFound();

                return Ok(new
                {
                    uspeh = true,
                    kodStanja = 200,
                    poruka = "Takmi?ar obrisan"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Gre?ka: " + ex.Message));
            }
        }
    }
}
