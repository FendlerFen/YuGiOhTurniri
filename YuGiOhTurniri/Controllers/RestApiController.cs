using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using KlasePodataka;
using Repozitorijumi;
using Servisi;

namespace YuGiOhTurniri.Controllers
{
    /// <summary>
    /// REST Web API 
    /// </summary>
    [RoutePrefix("api")]
    public class RestApiController : Controller
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        #region PARAMETRI OGRANI?ENJA SPILA - NAMENA 1

        /// <summary>
        /// GET /api/ogranicenja-spila
        /// </summary>
        [Route("ogranicenja-spila")]
        [HttpGet]
        public JsonResult OgraniceSpila()
        {
            try
            {
                var servis = new OgranicenjaServis();
                var ogranicenja = new
                {
                    mainDeckMin = servis.DajMinBrojKarataMain(),
                    mainDeckMax = servis.DajMaxBrojKarataMain(),
                    extraDeckMax = servis.DajMaxBrojKarataExtra(),
                    sideDeckMax = servis.DajMaxBrojKarataSide(),
                    maxTakmicaraNaTurniru = servis.DajMaxBrojTakmicara()
                };

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Ogranicenja uspesno preuzeta",
                    data = ogranicenja 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri preuzimanju ograni?enja",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region TAKMI?ARI - CRUD OPERACIJE - NAMENA 2

        /// <summary>
        /// GET /api/takmicari
        /// </summary>
        [Route("takmicari")]
        [HttpGet]
        public JsonResult GetAll()
        {
            try
            {
                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                List<TakmicarKlasa> takmicari = repo.DajSveTakmicara();

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Takmi?ari uspesno preuzeti",
                    count = takmicari.Count,
                    data = takmicari 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri preuzimanju takmicara",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GET /api/takmicari/5
        /// </summary>
        [Route("takmicari/{id:int}")]
        [HttpGet]
        public JsonResult GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Response.StatusCode = 400;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 400,
                        message = "ID mora biti ve?i od 0"
                    }, JsonRequestBehavior.AllowGet);
                }

                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                TakmicarKlasa takmicar = repo.DajPoID(id);

                if (takmicar == null)
                {
                    Response.StatusCode = 404;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 404,
                        message = "Takmicar nije prona?en"
                    }, JsonRequestBehavior.AllowGet);
                }

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Takmicar uspesno pronaden",
                    data = takmicar 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri preuzimanju takmicara",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// POST /api/takmicari
        /// </summary>
        [Route("takmicari")]
        [HttpPost]
        public JsonResult Create(TakmicarKlasa takmicar)
        {
            try
            {
                if (takmicar == null)
                {
                    Response.StatusCode = 400;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 400,
                        message = "Takmicar ne moze biti null"
                    });
                }

                if (string.IsNullOrWhiteSpace(takmicar.Email))
                {
                    Response.StatusCode = 400;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 400,
                        message = "Email je obavezan"
                    });
                }

                if (string.IsNullOrWhiteSpace(takmicar.Ime) || string.IsNullOrWhiteSpace(takmicar.Prezime))
                {
                    Response.StatusCode = 400;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 400,
                        message = "Ime i prezime su obavezni"
                    });
                }

                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                int noviID = repo.Dodaj(takmicar);

                if (noviID > 0)
                {
                    takmicar.TakmicarID = noviID;
                    Response.StatusCode = 201; // Created
                    return Json(new 
                    { 
                        success = true, 
                        statusCode = 201,
                        message = "Takmicar uspesno kreiran",
                        data = takmicar
                    });
                }
                else
                {
                    Response.StatusCode = 400;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 400,
                        message = "Takmicar sa ovim emailom vec postoji"
                    });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri kreiranju takmicara",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region TURNIRI - ?ITANJA

        /// <summary>
        /// GET /api/turniri
        /// </summary>
        [Route("turniri")]
        [HttpGet]
        public JsonResult GetAllTurniri()
        {
            try
            {
                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<TurnirKlasa> turniri = repo.DajSveTurnire();

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Turniri uspesno preuzeti",
                    count = turniri.Count,
                    data = turniri 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri preuzimanju turnira",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GET /api/turniri/5
        /// </summary>
        [Route("turniri/{id:int}")]
        [HttpGet]
        public JsonResult GetTurnirById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Response.StatusCode = 400;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 400,
                        message = "ID mora biti veci od 0"
                    }, JsonRequestBehavior.AllowGet);
                }

                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                TurnirKlasa turnir = repo.DajPoID(id);

                if (turnir == null)
                {
                    Response.StatusCode = 404;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 404,
                        message = "Turnir nije pronaden"
                    }, JsonRequestBehavior.AllowGet);
                }

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Turnir uspesno pronaden",
                    data = turnir 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri preuzimanju turnira",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GET /api/turniri/otvoreni
        /// </summary>
        [Route("turniri/otvoreni")]
        [HttpGet]
        public JsonResult GetOtvoreneTurnire()
        {
            try
            {
                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<TurnirKlasa> turniri = repo.DajOtvoreneTurnire();

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Otvoreni turniri uspesno preuzeti",
                    count = turniri.Count,
                    data = turniri 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri preuzimanju otvorenih turnira",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region SPILOVI - ?ITANJA

        /// <summary>
        /// GET /api/spilovi/5
        /// </summary>
        [Route("spilovi/{id:int}")]
        [HttpGet]
        public JsonResult GetSpilById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Response.StatusCode = 400;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 400,
                        message = "ID mora biti veci od 0"
                    }, JsonRequestBehavior.AllowGet);
                }

                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                SpilKlasa spil = repo.DajPoID(id);

                if (spil == null)
                {
                    Response.StatusCode = 404;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 404,
                        message = "Spil nije pronaden"
                    }, JsonRequestBehavior.AllowGet);
                }

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Spil uspesno pronaden",
                    data = spil 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Gre?ka pri preuzimanju spila",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GET /api/spilovi-takmicara/1
        /// </summary>
        [Route("spilovi-takmicara/{takmicarID:int}")]
        [HttpGet]
        public JsonResult GetSpiloveTakmicara(int takmicarID)
        {
            try
            {
                if (takmicarID <= 0)
                {
                    Response.StatusCode = 400;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 400,
                        message = "ID takmicara mora biti veci od 0"
                    }, JsonRequestBehavior.AllowGet);
                }

                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                List<SpilKlasa> spilovi = repo.DajSpiloveTakmicara(takmicarID);

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Spilovi takmicara uspesno preuzeti",
                    count = spilovi.Count,
                    data = spilovi 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri preuzimanju spilova takmicara",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region BAN LISTA - ?ITANJA

        /// <summary>
        /// GET /api/ban-lista
        /// </summary>
        [Route("ban-lista")]
        [HttpGet]
        public JsonResult GetBanLista()
        {
            try
            {
                IBanListaRepository repo = new BanListaRepozitorijumSP(_konekcija);
                List<BanListaKlasa> banLista = repo.DajSvuBanListu();

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Ban lista uspesno preuzeta",
                    count = banLista.Count,
                    data = banLista 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri preuzimanju ban liste",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region REZULTATI - ?ITANJA

        /// <summary>
        /// GET /api/rezultati/5
        /// </summary>
        [Route("rezultati/{turnirID:int}")]
        [HttpGet]
        public JsonResult GetPobednici(int turnirID)
        {
            try
            {
                if (turnirID <= 0)
                {
                    Response.StatusCode = 400;
                    return Json(new 
                    { 
                        success = false, 
                        statusCode = 400,
                        message = "ID turnira mora biti veci od 0"
                    }, JsonRequestBehavior.AllowGet);
                }

                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<RezultatKlasa> rezultati = repo.DajPobednike(turnirID);

                Response.StatusCode = 200;
                return Json(new 
                { 
                    success = true, 
                    statusCode = 200,
                    message = "Pobednici uspesno preuzeti",
                    count = rezultati.Count,
                    data = rezultati 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new 
                { 
                    success = false, 
                    statusCode = 500,
                    message = "Greska pri preuzimanju pobednika",
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region SPILOVI - CRUD OPERACIJE

        /// <summary>
        /// POST /api/spilovi
        /// Kreiranje novog spila
        /// </summary>
        [Route("spilovi")]
        [HttpPost]
        public JsonResult CreateSpil(SpilKlasa spil)
        {
            try
            {
                if (spil == null)
                {
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        success = false,
                        statusCode = 400,
                        message = "Spil ne moze biti null"
                    });
                }

                if (string.IsNullOrWhiteSpace(spil.Naziv))
                {
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        success = false,
                        statusCode = 400,
                        message = "Naziv spila je obavezan"
                    });
                }

                if (string.IsNullOrWhiteSpace(spil.Format))
                {
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        success = false,
                        statusCode = 400,
                        message = "Format spila je obavezan"
                    });
                }

                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                int noviID = repo.Dodaj(spil);

                if (noviID > 0)
                {
                    spil.SpilID = noviID;
                    Response.StatusCode = 201; // Created
                    return Json(new
                    {
                        success = true,
                        statusCode = 201,
                        message = "Spil uspesno kreiran",
                        data = spil
                    });
                }
                else
                {
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        success = false,
                        statusCode = 400,
                        message = "Greska pri kreiranju spila"
                    });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    statusCode = 500,
                    message = "Greska pri kreiranju spila",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region TURNIRI - CRUD OPERACIJE

        /// <summary>
        /// POST /api/turniri
        /// Kreiranje novog turnira
        /// </summary>
        [Route("turniri")]
        [HttpPost]
        public JsonResult CreateTurnir(TurnirKlasa turnir)
        {
            try
            {
                if (turnir == null)
                {
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        success = false,
                        statusCode = 400,
                        message = "Turnir ne moze biti null"
                    });
                }

                if (string.IsNullOrWhiteSpace(turnir.Naziv))
                {
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        success = false,
                        statusCode = 400,
                        message = "Naziv turnira je obavezan"
                    });
                }

                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                int noviID = repo.Dodaj(turnir);

                if (noviID > 0)
                {
                    turnir.TurnirID = noviID;
                    Response.StatusCode = 201; // Created
                    return Json(new
                    {
                        success = true,
                        statusCode = 201,
                        message = "Turnir uspesno kreiran",
                        data = turnir
                    });
                }
                else
                {
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        success = false,
                        statusCode = 400,
                        message = "Greska pri kreiranju turnira"
                    });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    statusCode = 500,
                    message = "Greska pri kreiranju turnira",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// DELETE /api/spilovi/5
        /// Brisanje spila
        /// </summary>
        [Route("spilovi/{id:int}")]
        [HttpDelete]
        public JsonResult DeleteSpil(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        success = false,
                        statusCode = 400,
                        message = "ID mora biti veci od 0"
                    });
                }

                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                bool uspeh = repo.Obrisi(id);

                if (uspeh)
                {
                    Response.StatusCode = 200;
                    return Json(new
                    {
                        success = true,
                        statusCode = 200,
                        message = "Spil uspesno obrisan"
                    });
                }
                else
                {
                    Response.StatusCode = 404;
                    return Json(new
                    {
                        success = false,
                        statusCode = 404,
                        message = "Spil nije pronaden"
                    });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    statusCode = 500,
                    message = "Greska pri brisanju spila",
                    error = ex.Message
                });
            }
        }

        #endregion
    }
}
