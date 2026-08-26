using System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using YuGiOhTurniri.Models;
using Repozitorijumi;
using KlasePodataka;

namespace YuGiOhTurniri.Controllers
{
    public class SudijaController : Controller
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            if (Session["sudijaID"] == null && actionName != "PrijavaS")
            {
                filterContext.Result = RedirectToAction("PrijavaS", "Sudija");
            }
            base.OnActionExecuting(filterContext);
        }

        // ====== PO?ETNA STRANICA ======
        public ActionResult Index()
        {
            if (Session["sudijaID"] == null)
                return RedirectToAction("PrijavaS");

            return RedirectToAction("Dashboard");
        }

        // ====== PRIJAVA ======
        public ActionResult PrijavaS()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PrijavaS(string email, string lozinka)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(lozinka))
            {
                ModelState.AddModelError("", "Email i lozinka su obavezni");
                return View();
            }

            try
            {
                // Hardcoded sudija kredencijali
                if (email == "SudijaJV@gmail.com" && lozinka == "Loz123")
                {
                    Session["sudijaID"] = 1;
                    Session["sudijaIme"] = "Sudija";
                    Session["sudijaPrezime"] = "JV";
                    Session["sudijaEmail"] = email;

                    return RedirectToAction("SpiloveNaCekanju", "Sudija");
                }

                // Ako ne poklapa hardcoded kredencijale, poku?aj iz baze
                SPSudijaDBKlasa db = new SPSudijaDBKlasa(_konekcija);
                int sudijaID = db.PrijavaS(email, lozinka);

                if (sudijaID > 0)
                {
                    // Preuzmi podatke sudije
                    SudijaKlasa sudija = db.GetSudijaByID(sudijaID);

                    Session["sudijaID"] = sudijaID;
                    Session["sudijaIme"] = sudija.Ime;
                    Session["sudijaPrezime"] = sudija.Prezime;
                    Session["sudijaEmail"] = sudija.Email;

                    return RedirectToAction("SpiloveNaCekanju", "Sudija");
                }
                else
                {
                    ModelState.AddModelError("", "Pogresan email ili lozinka");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Greska pri prijavi: " + ex.Message);
                return View();
            }
        }

        // ====== SPILOVI NA ?EKANJU ======
        public ActionResult SpiloveNaCekanju()
        {
            return Dashboard();
        }

        // ====== DASHBOARD ======
        public ActionResult Dashboard()
        {
            try
            {
                int sudijaID = Convert.ToInt32(Session["sudijaID"]);

                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                List<SpilKlasa> spilovi = repo.DajSpiloveNaCekanju();

                List<SpiloveNaRevizijuVM> vm = new List<SpiloveNaRevizijuVM>();
                foreach (var spil in spilovi)
                {
                    vm.Add(new SpiloveNaRevizijuVM
                    {
                        SpilID = spil.SpilID,
                        Naziv = spil.Naziv,
                        Format = spil.Format,
                        Arhetip = spil.Arhetip,
                        Status = spil.Status,
                        DatumKreiranja = spil.DatumKreiranja,
                        VlasnikIme = "",
                        VlasnikPrezime = "",
                        Email = ""
                    });
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greska pri ucitavanju spilova: " + ex.Message;
                return View(new List<SpiloveNaRevizijuVM>());
            }
        }

        // ====== ODOGOVRITE SPIL ======
        [HttpPost]
        public ActionResult OdobriSpil(int spilID)
        {
            try
            {
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                bool uspeh = repo.PromeniStatus(spilID, "Odobren", "");

                TempData["Poruka"] = uspeh ? "Spil odobren!" : "Gre?ka pri odobravanju!";
                return RedirectToAction("SpiloveNaCekanju");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Gre?ka: " + ex.Message;
                return RedirectToAction("SpiloveNaCekanju");
            }
        }

        // ====== ODBIJ SPIL ======
        [HttpPost]
        public ActionResult OdbiSpil(int spilID, string napomena = "")
        {
            try
            {
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                bool uspeh = repo.PromeniStatus(spilID, "Odbijen", napomena);

                TempData["Poruka"] = uspeh ? "Spil odbijen!" : "Gre?ka pri odbijanju!";
                return RedirectToAction("SpiloveNaCekanju");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Gre?ka: " + ex.Message;
                return RedirectToAction("SpiloveNaCekanju");
            }
        }

        // ====== PREGLEDA SPIL ======
        public ActionResult PregledajSpil(int id)
        {
            try
            {
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                SpilKlasa spil = repo.DajPoID(id);

                if (spil == null)
                {
                    TempData["Greska"] = "Spil nije prona?en!";
                    return RedirectToAction("SpiloveNaCekanju");
                }

                // Dohvati sve karte iz spila
                List<KartaUSpiluKlasa> karteDB = repo.DajKarteSpila(id);

                var karte = new List<Models.KartaPrikazVM>();

                foreach (var karta in karteDB)
                {
                    karte.Add(new Models.KartaPrikazVM
                    {
                        NazivKarte = karta.NazivKarte,
                        Sekcija = karta.Sekcija,
                        Kolicina = karta.Kolicina,
                        TipKarte = karta.TipKarte ?? ""
                    });
                }

                // Dohvati takmi?ara koji je kreirao spil
                string takmicarIme = "";
                if (spil.TakmicarID > 0)
                {
                    try
                    {
                        SPTakmicarDBKlasa takmicariDB = new SPTakmicarDBKlasa(_konekcija);
                        DataRow takmicariRow = takmicariDB.DajTakmicaraPoID(spil.TakmicarID);

                        if (takmicariRow != null)
                        {
                            takmicarIme = takmicariRow["Ime"].ToString() + " " + takmicariRow["Prezime"].ToString();
                        }
                    }
                    catch { }
                }

                var vm = new Models.StampajSpilVM
                {
                    Spil = new Models.SpilPrikazVM
                    {
                        SpilID = spil.SpilID,
                        Naziv = spil.Naziv,
                        Format = spil.Format,
                        Arhetip = spil.Arhetip,
                        DatumKreiranja = spil.DatumKreiranja
                    },
                    Karte = karte,
                    TakmicarIme = takmicarIme,
                    Vlasnik = takmicarIme
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Gre?ka pri u?itavanju spila: " + ex.Message;
                return RedirectToAction("SpiloveNaCekanju");
            }
        }

        // ====== ODLUKA O SPILU ======
        [HttpPost]
        public ActionResult OdlukaOSpilu(int SpilID, string Status, string Napomena = "")
        {
            try
            {
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);

                if (string.IsNullOrWhiteSpace(Status))
                {
                    TempData["Greska"] = "Trebate odabrati odluku!";
                    return RedirectToAction("PregledajSpil", new { id = SpilID });
                }

                bool uspeh = repo.PromeniStatus(SpilID, Status, Napomena);

                if (uspeh)
                {
                    TempData["Poruka"] = Status == "Odobren" ? "Spil odobren!" : "Spil odbijen!";
                }
                else
                {
                    TempData["Greska"] = "Gre?ka pri promjeni statusa spila!";
                }

                return RedirectToAction("SpiloveNaCekanju");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Gre?ka: " + ex.Message;
                return RedirectToAction("SpiloveNaCekanju");
            }
        }

        // ====== ODJAVA ======
        public ActionResult OdjavaS()
        {
            Session.Abandon();
            return RedirectToAction("PrijavaS");
        }

        // ====== BAN LISTA ======
        public ActionResult BanLista()
        {
            try
            {
                int sudijaID = Convert.ToInt32(Session["sudijaID"]);

                IBanListaRepository repo = new BanListaRepozitorijumSP(_konekcija);
                List<BanListaKlasa> banLista = repo.DajBanListuSudije(sudijaID);

                return View(banLista);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greska pri ucitavanju ban liste: " + ex.Message;
                return View(new List<BanListaKlasa>());
            }
        }

        [HttpPost]
        public ActionResult DodajNaBanListu(string nazivKarte)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nazivKarte))
                {
                    TempData["Greska"] = "Naziv karte je obavezan!";
                    return RedirectToAction("BanLista");
                }

                int sudijaID = Convert.ToInt32(Session["sudijaID"]);

                IBanListaRepository repo = new BanListaRepozitorijumSP(_konekcija);
                int rezultat = repo.DodajNaBanListu(sudijaID, nazivKarte);

                if (rezultat > 0)
                {
                    TempData["Poruka"] = "Karta uspesno dodana na ban listu!";
                }
                else
                {
                    TempData["Greska"] = "Greska pri dodavanju na ban listu!";
                }
                return RedirectToAction("BanLista");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greska: " + ex.Message;
                return RedirectToAction("BanLista");
            }
        }

        [HttpPost]
        public ActionResult ObrisiSaBanListe(int banListaID)
        {
            try
            {
                IBanListaRepository repo = new BanListaRepozitorijumSP(_konekcija);
                int rezultat = repo.ObrisiSaBanListe(banListaID);

                if (rezultat > 0)
                {
                    TempData["Poruka"] = "Karta uklonjena sa ban liste!";
                }
                else
                {
                    TempData["Greska"] = "Greska pri uklanjanju sa ban liste!";
                }
                return RedirectToAction("BanLista");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greska: " + ex.Message;
                return RedirectToAction("BanLista");
            }
        }


    }
}
