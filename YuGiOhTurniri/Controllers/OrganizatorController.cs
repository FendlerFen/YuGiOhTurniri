using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using KlasePodataka;
using Repozitorijumi;
using YuGiOhTurniri.Models;
using System.Data;
using Prezentaciona_Logika;
using System.Linq;

namespace YuGiOhTurniri.Controllers
{
    public class OrganizatorController : Controller
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        public ActionResult Index()
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            return RedirectToAction("Kontrolna");
        }

        public ActionResult PrijaviOrganizatora()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PrijaviOrganizatora(Models.PrijavaVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                string errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
                ViewBag.Greska = "Greške pri validaciji: " + errorMessage;
                return View(model);
            }

            var repo = new OrganizatorRepozitorijumSP(_konekcija);
            OrganizatorKlasa organizator = null;

            try
            {
                organizator = repo.Login(model.Email, model.Lozinka);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greška pri prijavi: " + ex.Message;
                return View(model);
            }

            if (organizator != null)
            {
                Session["organizatorID"] = organizator.OrganizatorID;
                Session["organizatorIme"] = organizator.Ime;
                Session["organizatorPrezime"] = organizator.Prezime;
                Session["organizatorEmail"] = organizator.Email;
                Session["organizatorNaziv"] = organizator.NazivOrganizacije;

                return RedirectToAction("Kontrolna", "Organizator");
            }

            ViewBag.Greska = "Pogrešan email ili lozinka!";
            return View(model);
        }

        public ActionResult RegistrujOrganizatora()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrujOrganizatora(RegistacijaOrganizatoraVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var repo = new OrganizatorRepozitorijumSP(_konekcija);

            try
            {
                OrganizatorKlasa organizator = new OrganizatorKlasa
                {
                    NazivOrganizacije = model.NazivOrganizacije,
                    Ime = model.Ime,
                    Prezime = model.Prezime,
                    Email = model.Email,
                    TelefonBroj = model.TelefonBroj,
                    Drzava = model.Drzava,
                    Lozinka = model.Lozinka,
                    DatumRegistracije = DateTime.Now
                };

                int rezultat = repo.Dodaj(organizator);
                System.Diagnostics.Debug.WriteLine($"RegistrujOrganizatora: Rezultat = {rezultat}, Email = {organizator.Email}");

                if (rezultat > 0)
                {
                    TempData["Poruka"] = "Organizator uspesno registrovan! Molimo prijavite se.";
                    return RedirectToAction("PrijaviOrganizatora");
                }

                TempData["Greska"] = "Greska pri registraciji - Email mozda vec postoji.";
                return RedirectToAction("RegistrujOrganizatora");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RegistrujOrganizatora Exception: {ex.Message}\n{ex.StackTrace}");
                TempData["Greska"] = "Greska pri registraciji: " + ex.Message;
                return RedirectToAction("RegistrujOrganizatora");
            }
        }

        [HttpGet]
        public ActionResult Kontrolna()
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            return View();
        }

        // ====== TURNIRI (sa filterima i pretrагом) ======
        public ActionResult MojiTurniri(string format = "", string pretraga = "")
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            try
            {
                int organizatorID = Convert.ToInt32(Session["organizatorID"]);
                System.Diagnostics.Debug.WriteLine($"MojiTurniri: organizatorID={organizatorID}");

                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                var ds = db.DajTurnireOrganizatora(organizatorID);

                System.Diagnostics.Debug.WriteLine($"MojiTurniri: Broj tabela = {ds.Tables.Count}");
                if (ds.Tables.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"MojiTurniri: Broj redova = {ds.Tables[0].Rows.Count}");
                }

                List<TurnirKlasa> turniriSP = new List<TurnirKlasa>();
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in ds.Tables[0].Rows)
                    {
                        // Provjeravamo da li kolona postoji i ima li vrijednost
                        int organizatorIDFromRow = 0;
                        if (ds.Tables[0].Columns.Contains("OrganizatorID"))
                        {
                            organizatorIDFromRow = (int)row["OrganizatorID"];
                        }
                        else if (ds.Tables[0].Columns.Contains("OrganiziatorID"))
                        {
                            // Fallback za typo ako postoji
                            organizatorIDFromRow = (int)row["OrganiziatorID"];
                        }

                        turniriSP.Add(new TurnirKlasa
                        {
                            TurnirID = (int)row["TurnirID"],
                            Naziv = row["Naziv"].ToString(),
                            Lokacija = row["Lokacija"].ToString(),
                            Format = row["Format"].ToString(),
                            DatumOdrzavanja = (DateTime)row["DatumOdrzavanja"],
                            Status = row["Status"].ToString(),
                            OrganizatorID = organizatorIDFromRow,
                            DatumKreiranja = (DateTime)row["DatumKreiranja"]
                        });
                    }
                }

                // Primeni filtre iz JSON parametara
                FiltriranjeServis filtriranjeServis = new FiltriranjeServis();
                turniriSP = filtriranjeServis.FiltrirajTurnire(turniriSP, format, pretraga);

                // Mapiranje na ViewModel
                List<Models.MojiTurniriVM> turniri = new List<Models.MojiTurniriVM>();
                foreach (var turnir in turniriSP)
                {
                    turniri.Add(new Models.MojiTurniriVM
                    {
                        TurnirID = turnir.TurnirID,
                        Naziv = turnir.Naziv,
                        Lokacija = turnir.Lokacija,
                        Format = turnir.Format,
                        DatumOdrzavanja = turnir.DatumOdrzavanja,
                        Status = turnir.Status,
                        DatumKreiranja = turnir.DatumKreiranja
                    });
                }

                // Dostupni formati za dropdown iz JSON parametara
                ViewBag.Formati = filtriranjeServis.DajDostupneFormate();
                ViewBag.IzabranFormat = format;
                ViewBag.Pretraga = pretraga;

                return View(turniri);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MojiTurniri Exception: {ex.Message}\n{ex.StackTrace}");
                ViewBag.Greska = "Greska pri ucitavanju turnira: " + ex.Message;
                return View(new List<Models.MojiTurniriVM>());
            }
        }

        public ActionResult KreirajTurnir()
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            var vm = new Models.KreirajTurnirVM
            {
                DatumOdrzavanja = DateTime.Now.AddDays(7),  
                Formati = new System.Collections.Generic.List<System.Web.Mvc.SelectListItem>
                {
                    new System.Web.Mvc.SelectListItem { Text = "TCG", Value = "TCG" },
                    new System.Web.Mvc.SelectListItem { Text = "OCG", Value = "OCG" },
                    new System.Web.Mvc.SelectListItem { Text = "Speed Duel", Value = "Speed Duel" }
                }
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult KreirajTurnir(Models.KreirajTurnirVM model)
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            // Validacija datuma - mora biti u budućnosti
            if (model.DatumOdrzavanja < DateTime.Today)
            {
                ModelState.AddModelError("DatumOdrzavanja", "Datum turnira mora biti u budućnosti!");
            }

            if (!ModelState.IsValid)
            {
                model.Formati = new System.Collections.Generic.List<System.Web.Mvc.SelectListItem>
                {
                    new System.Web.Mvc.SelectListItem { Text = "TCG", Value = "TCG" },
                    new System.Web.Mvc.SelectListItem { Text = "OCG", Value = "OCG" },
                    new System.Web.Mvc.SelectListItem { Text = "Speed Duel", Value = "Speed Duel" }
                };
                return View(model);
            }

            try
            {
                int organizatorID = Convert.ToInt32(Session["organizatorID"]);

                TurnirKlasa turnir = new TurnirKlasa
                {
                    Naziv = model.Naziv,
                    Lokacija = model.Lokacija,
                    Format = model.Format,
                    DatumOdrzavanja = model.DatumOdrzavanja,
                    OrganizatorID = organizatorID,
                    Status = "Otvoren",
                    DatumKreiranja = DateTime.Now
                };

                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                int noviTurnirID = db.KreirajTurnir(turnir);

                if (noviTurnirID > 0)
                {
                    TempData["Poruka"] = "Turnir uspesno kreiran!";
                    return RedirectToAction("MojiTurniri");
                }

                TempData["Greska"] = "Greska pri kreiranju turnira - ID je 0!";
                return RedirectToAction("KreirajTurnir");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greska pri kreiranju turnira: " + ex.Message;
                return RedirectToAction("KreirajTurnir");
            }
        }

        public ActionResult Izvestaji()
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            return View();
        }

        // ====== DETALJI TURNIRA ======
        public ActionResult DetaljiTurnira(int id)
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            try
            {
                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                DataRow row = db.DajTurnirPoID(id);

                if (row == null)
                {
                    TempData["Greska"] = "Turnir nije prona?en!";
                    return RedirectToAction("MojiTurniri");
                }

                TurnirKlasa turnir = new TurnirKlasa
                {
                    TurnirID = (int)row["TurnirID"],
                    Naziv = row["Naziv"].ToString(),
                    Lokacija = row["Lokacija"].ToString(),
                    Format = row["Format"].ToString(),
                    DatumOdrzavanja = (DateTime)row["DatumOdrzavanja"],
                    Status = row["Status"].ToString(),
                    OrganizatorID = (int)row["OrganizatorID"],
                    DatumKreiranja = (DateTime)row["DatumKreiranja"]
                };

                // Dohvati pobednike ako su dostupni
                var dsPobednici = db.DajPobednike(id);
                List<PobednikDetaljiVM> pobednici = new List<PobednikDetaljiVM>();
                if (dsPobednici.Tables.Count > 0 && dsPobednici.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in dsPobednici.Tables[0].Rows)
                    {
                        pobednici.Add(new PobednikDetaljiVM
                        {
                            Mesto = (int)r["Mesto"],
                            Ime = r["Ime"].ToString(),
                            Prezime = r["Prezime"].ToString(),
                            BrojPobeda = (int)r["BrojPobeda"]
                        });
                    }
                }

                // Dohvati sve takmi?are koji su se prijavili na turnir
                var dsTakmicari = db.DajTakmicareNaTurniru(id);
                List<TakmicarDetaljiVM> takmicari = new List<TakmicarDetaljiVM>();
                if (dsTakmicari.Tables.Count > 0 && dsTakmicari.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in dsTakmicari.Tables[0].Rows)
                    {
                        takmicari.Add(new TakmicarDetaljiVM
                        {
                            TakmicarID = (int)r["TakmicarID"],
                            Ime = r["Ime"].ToString(),
                            Prezime = r["Prezime"].ToString(),
                            SpilNaziv = r.Table.Columns.Contains("SpilNaziv") ? r["SpilNaziv"].ToString() : "N/A"
                        });
                    }
                }

                ViewBag.Pobednici = pobednici;
                ViewBag.Takmicari = takmicari;
                ViewBag.BrojTakmicara = takmicari.Count;
                return View(turnir);
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greska pri ucitavanju turnira: " + ex.Message;
                return RedirectToAction("MojiTurniri");
            }
        }

        // ====== IZMENI TURNIR ======
        public ActionResult IzmeniTurnir(int id)
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            try
            {
                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                DataRow row = db.DajTurnirPoID(id);

                if (row == null)
                {
                    TempData["Greska"] = "Turnir nije prona?en!";
                    return RedirectToAction("MojiTurniri");
                }

                var vm = new Models.KreirajTurnirVM
                {
                    Naziv = row["Naziv"].ToString(),
                    Lokacija = row["Lokacija"].ToString(),
                    Format = row["Format"].ToString(),
                    DatumOdrzavanja = (DateTime)row["DatumOdrzavanja"],
                    Formati = new System.Collections.Generic.List<SelectListItem>
                    {
                        new SelectListItem { Text = "TCG", Value = "TCG", Selected = row["Format"].ToString() == "TCG" },
                        new SelectListItem { Text = "OCG", Value = "OCG", Selected = row["Format"].ToString() == "OCG" },
                        new SelectListItem { Text = "Speed Duel", Value = "Speed Duel", Selected = row["Format"].ToString() == "Speed Duel" }
                    }
                };

                ViewBag.TurnirID = id;
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greska pri ucitavanju turnira: " + ex.Message;
                return RedirectToAction("MojiTurniri");
            }
        }

        [HttpPost]
        public ActionResult IzmeniTurnir(int id, Models.KreirajTurnirVM model)
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            if (!ModelState.IsValid)
            {
                model.Formati = new System.Collections.Generic.List<SelectListItem>
                {
                    new SelectListItem { Text = "TCG", Value = "TCG" },
                    new SelectListItem { Text = "OCG", Value = "OCG" },
                    new SelectListItem { Text = "Speed Duel", Value = "Speed Duel" }
                };
                ViewBag.TurnirID = id;
                return View(model);
            }

            try
            {
                TurnirKlasa turnir = new TurnirKlasa
                {
                    TurnirID = id,
                    Naziv = model.Naziv,
                    Lokacija = model.Lokacija,
                    Format = model.Format,
                    DatumOdrzavanja = model.DatumOdrzavanja,
                    OrganizatorID = Convert.ToInt32(Session["organizatorID"])
                };

                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);

                // Obrisi sve prijave na turnir kada se turnir izmeni
                db.ObrisiBrisanjePrijava(id);

                bool uspeh = db.IzmeniTurnir(turnir);

                if (uspeh)
                {
                    TempData["Poruka"] = "Turnir uspesno izmenjen! Sve prijave su obrisane jer su se parametri turnira promenili.";
                    return RedirectToAction("DetaljiTurnira", new { id = id });
                }

                TempData["Greska"] = "Greska pri izmeni turnira!";
                return RedirectToAction("IzmeniTurnir", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greska pri izmeni turnira: " + ex.Message;
                return RedirectToAction("IzmeniTurnir", new { id = id });
            }
        }

        // ====== OBRISI TURNIR ======
        public ActionResult ObrisiTurnir(int id)
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            try
            {
                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                bool uspeh = db.ObrisiTurnir(id);

                if (uspeh)
                {
                    TempData["Poruka"] = "Turnir uspesno obrisan!";
                    return RedirectToAction("MojiTurniri");
                }

                TempData["Greska"] = "Greska pri brisanju turnira!";
                return RedirectToAction("MojiTurniri");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greska pri brisanju turnira: " + ex.Message;
                return RedirectToAction("MojiTurniri");
            }
        }

        // ====== PROGLASI POBEDNIKE ======
        public ActionResult ProglasiPobednike(int id)
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            try
            {
                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);

                // Dohvati sve takmicare koji su se prijavili na turnir
                DataSet dsTakmicari = db.DajTakmicareNaTurniru(id);

                var vm = new Models.ProclasiPobjednikeVM
                {
                    TurnirID = id,
                    Takmicari = new System.Collections.Generic.List<SelectListItem>()
                };

                if (dsTakmicari.Tables.Count > 0 && dsTakmicari.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dsTakmicari.Tables[0].Rows)
                    {
                        vm.Takmicari.Add(new SelectListItem
                        {
                            Value = row["TakmicarID"].ToString(),
                            Text = row["Ime"].ToString() + " " + row["Prezime"].ToString()
                        });
                    }
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greska pri ucitavanju takmicara: " + ex.Message;
                return RedirectToAction("DetaljiTurnira", new { id = id });
            }
        }

        [HttpPost]
        public ActionResult ProglasiPobednike(int id, Models.ProclasiPobjednikeVM model)
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            // Provera da li je bar prvo mesto odabrano
            if (model.PrvoMestoID <= 0)
            {
                TempData["Greska"] = "Prvo mesto je obavezno!";
                return RedirectToAction("ProglasiPobednike", new { id = id });
            }

            try
            {
                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                bool uspeh = db.ProglasiPobednike(id, model.PrvoMestoID, model.DrugoMestoID, model.TreceMestoID);

                if (uspeh)
                {
                    // Zatvori turnir
                    db.ZavrsiTurnir(id);
                    TempData["Poruka"] = "Pobednici uspesno proglaseni!";
                    return RedirectToAction("DetaljiTurnira", new { id = id });
                }

                TempData["Greska"] = "Greska pri proglasavanju pobednika!";
                return RedirectToAction("ProglasiPobednike", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greska pri proglasavanju pobednika: " + ex.Message;
                return RedirectToAction("ProglasiPobednike", new { id = id });
            }
        }

        // ====== ŠTAMPA TURNIRA (Parametarska Štampa - Master-Detail) ======
        public ActionResult StampajTurnir(int id)
        {
            if (Session["organizatorID"] == null)
                return RedirectToAction("PrijaviOrganizatora");

            try
            {
                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                DataRow row = db.DajTurnirPoID(id);

                if (row == null)
                {
                    TempData["Greska"] = "Turnir nije pronađen!";
                    return RedirectToAction("MojiTurniri");
                }

                TurnirKlasa turnir = new TurnirKlasa
                {
                    TurnirID = (int)row["TurnirID"],
                    Naziv = row["Naziv"].ToString(),
                    Lokacija = row["Lokacija"].ToString(),
                    Format = row["Format"].ToString(),
                    DatumOdrzavanja = (DateTime)row["DatumOdrzavanja"],
                    Status = row["Status"].ToString(),
                    OrganizatorID = (int)row["OrganizatorID"],
                    DatumKreiranja = (DateTime)row["DatumKreiranja"]
                };

                // Dohvati sve takmičare koji su se prijavili na turnir
                var dsTakmicari = db.DajTakmicareNaTurniru(id);
                List<string> takmicari = new List<string>();
                if (dsTakmicari.Tables.Count > 0 && dsTakmicari.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in dsTakmicari.Tables[0].Rows)
                    {
                        takmicari.Add(r["Ime"].ToString() + " " + r["Prezime"].ToString());
                    }
                }

                // Dohvati rezultate ako su dostupni
                var dsRezultati = db.DajPobednike(id);
                List<RezultatKlasa> rezultati = new List<RezultatKlasa>();
                if (dsRezultati.Tables.Count > 0 && dsRezultati.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in dsRezultati.Tables[0].Rows)
                    {
                        rezultati.Add(new RezultatKlasa
                        {
                            Mesto = Convert.ToInt32(r["Mesto"]),
                            Takmicari = r["Ime"].ToString() + " " + r["Prezime"].ToString(),
                            BrojPobeda = Convert.ToInt32(r["BrojPobeda"])
                        });
                    }
                }

                // Generiši HTML štampu sa master-detail podacima
                StampacijuServis stampServis = new StampacijuServis();
                string htmlStampa = stampServis.GenerirajStampuTurnira(turnir, takmicari, rezultati);

                return Content(htmlStampa, "text/html");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greška pri generisanju štampe: " + ex.Message;
                return RedirectToAction("MojiTurniri");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
