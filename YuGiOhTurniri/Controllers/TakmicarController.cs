using System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using KlasePodataka;
using Prezentaciona_Logika;
using Repozitorijumi;
using YuGiOhTurniri.Models;
using YuGiOhTurniri.Services;

namespace YuGiOhTurniri.Controllers
{
    public class TakmicarController : Controller
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;
        private readonly HttpKlijentServis _http = new HttpKlijentServis();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["takmicarID"] == null)
                filterContext.Result = RedirectToAction("PrijaviTakmicara", "Nalog");
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BanLista()
        {
            try
            {
                List<BanListaKlasa> banLista = null;

                // Prvo poku?aj API
                try
                {
                    // GET /api/ban-lista
                    banLista = _http.DajBanListuAsync().GetAwaiter().GetResult();
                }
                catch
                {
                    // Ako API ne radi, ide direktno u bazu
                    var repo = new BanListaRepozitorijumSP(_konekcija);
                    banLista = repo.DajSvuBanListu();
                }

                banLista = banLista ?? new List<BanListaKlasa>();
                var vm = banLista.Select(s => new BanListaPrikazVM
                {
                    BanListaID = s.BanListaID,
                    NazivKarte = s.NazivKarte,
                    DatumDodavanja = s.DatumDodavanja
                }).ToList();
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = ex.Message;
                return View(new List<BanListaPrikazVM>());
            }
        }

        public ActionResult MojiSpilovi(string format = "", string pretraga = "", string status = "")
        {
            try
            {
                int takmicarID = Convert.ToInt32(Session["takmicarID"]);
                List<SpilKlasa> spilovi = null;

                // Prvo poku?aj API
                try
                {
                    // GET /api/spilovi/takmicar/{id}
                    spilovi = _http.DajSpiloveTakmicaraAsync(takmicarID).GetAwaiter().GetResult();
                }
                catch
                {
                    // Ako API ne radi, ide direktno u bazu
                    var forma = new FormaSpilKlasa(_konekcija);
                    spilovi = forma.DajSpiloveTakmicara(takmicarID);
                }

                spilovi = spilovi ?? new List<SpilKlasa>();

                var filtriranje = new FiltriranjeServis();
                spilovi = filtriranje.Filtriraj(spilovi, format, pretraga);
                if (!string.IsNullOrEmpty(status))
                    spilovi = spilovi.Where(s => s.Status == status).ToList();

                var vm = spilovi.Select(s => new MojiSpiloviVM
                {
                    SpilID = s.SpilID,
                    Naziv = s.Naziv,
                    Format = s.Format,
                    Arhetip = s.Arhetip,
                    Status = s.Status,
                    DatumKreiranja = s.DatumKreiranja
                }).ToList();

                ViewBag.Formati = filtriranje.DajDostupneFormate();
                ViewBag.IzabranFormat = format;
                ViewBag.IzabranStatus = status;
                ViewBag.Pretraga = pretraga;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = ex.Message;
                return View(new List<MojiSpiloviVM>());
            }
        }

        public ActionResult OtvoreniTurniri()
        {
            try
            {
                List<TurnirKlasa> turniri = null;

                // Prvo poku?aj API
                try
                {
                    // GET /api/turniri/otvoreni
                    turniri = _http.DajOtvoreneTurnireAsync().GetAwaiter().GetResult();
                }
                catch
                {
                    // Ako API ne radi, ide direktno u bazu
                    var forma = new FormaTurniraKlasa(_konekcija);
                    turniri = forma.DajOtvoreneTurnire();
                }

                turniri = turniri ?? new List<TurnirKlasa>();

                var vm = turniri.Select(t => new OtvoreniTurniriVM
                {
                    TurnirID = t.TurnirID,
                    Naziv = t.Naziv,
                    Lokacija = t.Lokacija,
                    Format = t.Format,
                    DatumOdrzavanja = t.DatumOdrzavanja,
                    Status = t.Status,
                    Organizator = "Organizator: " + t.OrganizatorID
                }).ToList();

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = ex.Message;
                return View(new List<OtvoreniTurniriVM>());
            }
        }

        public ActionResult DetaljiSpila(int id)
        {
            try
            {
                var spil = _http.DajSpilPoIDAsync(id).GetAwaiter().GetResult();
                if (spil == null) return HttpNotFound();
                var karte = _http.DajKarteSpilaAsync(id).GetAwaiter().GetResult() ?? new List<KartaUSpiluKlasa>();
                ViewBag.Karte = karte;
                return View(spil);
            }
            catch (Exception ex)
            {
                TempData["Greska"] = ex.Message;
                return RedirectToAction("MojiSpilovi");
            }
        }

        public ActionResult KreirajSpil()
        {
            try
            {
                var forma = new FiltriranjeServis();
                var vm = new KreirajSpilVM
                {
                    Formati = forma.DajDostupneFormate()
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = ex.Message;
                return View(new KreirajSpilVM());
            }
        }

        public ActionResult IzmeniSpil(int id)
        {
            try
            {
                int takmicarID = Convert.ToInt32(Session["takmicarID"]);

                var forma = new FormaSpilKlasa(_konekcija);
                var spil = forma.DajSpilPoID(id);

                if (spil == null || spil.TakmicarID != takmicarID)
                    return HttpNotFound();

                var karte = forma.DajKarteSpila(id);

                var vm = new DetaljiSpilVM
                {
                    SpilID = spil.SpilID,
                    Naziv = spil.Naziv,
                    Format = spil.Format,
                    Arhetip = spil.Arhetip,
                    Status = spil.Status,
                    Karte = karte.Select(k => new KartaUSpiluVM
                    {
                        KartaUSpiluID = k.KartaUSpiluID,
                        NazivKarte = k.NazivKarte,
                        Sekcija = k.Sekcija,
                        Kolicina = (int)k.Kolicina
                    }).ToList()
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Greska"] = ex.Message;
                return RedirectToAction("MojiSpilovi");
            }
        }

        [HttpPost]
        public ActionResult IzmeniSpil(int id, DetaljiSpilVM model)
        {
            try
            {
                int takmicarID = Convert.ToInt32(Session["takmicarID"]);

                var forma = new FormaSpilKlasa(_konekcija);
                var spil = forma.DajSpilPoID(id);

                if (spil == null || spil.TakmicarID != takmicarID)
                    return HttpNotFound();

                var azuriranSpil = new SpilKlasa
                {
                    SpilID = id,
                    Naziv = model.Naziv,
                    Format = model.Format,
                    Arhetip = model.Arhetip,
                    TakmicarID = takmicarID,
                    Status = spil.Status,
                    DatumKreiranja = spil.DatumKreiranja
                };

                var repo = new SpilRepozitorijumSP(_konekcija);
                bool uspeh = repo.Izmeni(azuriranSpil);

                if (uspeh)
                {
                    TempData["Poruka"] = "Spil uspešno izmenjen!";
                    return RedirectToAction("MojiSpilovi");
                }
                else
                {
                    TempData["Greska"] = "Greška pri izmeni spila!";
                    return RedirectToAction("IzmeniSpil", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["Greska"] = ex.Message;
                return RedirectToAction("MojiSpilovi");
            }
        }

        public ActionResult ObrisiSpil(int id)
        {
            try
            {
                int takmicarID = Convert.ToInt32(Session["takmicarID"]);

                var forma = new FormaSpilKlasa(_konekcija);
                var spil = forma.DajSpilPoID(id);

                if (spil == null || spil.TakmicarID != takmicarID)
                    return HttpNotFound();

                var repo = new SpilRepozitorijumSP(_konekcija);
                bool uspeh = repo.Obrisi(id);

                if (uspeh)
                {
                    TempData["Poruka"] = "Spil uspešno obrisan!";
                }
                else
                {
                    TempData["Greska"] = "Greška pri brisanju spila!";
                }

                return RedirectToAction("MojiSpilovi");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = ex.Message;
                return RedirectToAction("MojiSpilovi");
            }
        }

        [HttpPost]
        public ActionResult KreirajSpil(KreirajSpilVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                string errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
                ViewBag.Greska = "Gre?ke pri validaciji: " + errorMessage;
                var forma = new FiltriranjeServis();
                model.Formati = forma.DajDostupneFormate();
                return View(model);
            }

            try
            {
                int takmicarID = Convert.ToInt32(Session["takmicarID"]);
                var spil = new SpilKlasa
                {
                    Naziv = model.Naziv,
                    Format = model.Format,
                    Arhetip = model.Arhetip,
                    TakmicarID = takmicarID,
                    Status = "Na ?ekanju",
                    DatumKreiranja = DateTime.Now
                };

                int noviSpilID = -1;

                // Prvo poku?aj API
                try
                {
                    // POST /api/spilovi
                    noviSpilID = _http.KreirajSpilAsync(spil).GetAwaiter().GetResult();
                }
                catch
                {
                    // Ako API ne radi, ide direktno u bazu
                    var formaSpil = new FormaSpilKlasa(_konekcija);
                    var rezultat = formaSpil.KreirajSpil(spil.Naziv, spil.Format, spil.Arhetip, spil.TakmicarID);
                    if (rezultat.Contains("kreiran"))
                        noviSpilID = 1; // Barem znamo da je kreirano
                    else
                        throw new Exception(rezultat);
                }

                if (noviSpilID <= 0)
                {
                    ViewBag.Greska = "Gre?ka pri kreiranju spila";
                    var forma = new FiltriranjeServis();
                    model.Formati = forma.DajDostupneFormate();
                    return View(model);
                }

                TempData["Poruka"] = "Spil uspe?no kreiran!";
                return RedirectToAction("MojiSpilovi");
            }
            catch (Exception ex)
            {
                ViewBag.Greska = ex.Message ?? "Gre?ka pri kreiranju spila";
                var forma = new FiltriranjeServis();
                model.Formati = forma.DajDostupneFormate();
                return View(model);
            }
        }
    }
}
