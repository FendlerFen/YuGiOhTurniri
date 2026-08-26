using System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KlasePodataka;
using Repozitorijumi;

namespace YuGiOhTurniri.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        public ActionResult Index()
        {
            if (Session["organizatorID"] != null)
            {
                return RedirectToAction("Kontrolna", "Organizator");
            }

            if (Session["takmicarID"] != null)
            {
                return RedirectToAction("MojiSpilovi", "Takmicar");
            }

            if (Session["sudijaID"] != null)
            {
                return RedirectToAction("Dashboard", "Sudija");
            }

            try
            {
                // Prikazuj sve turnire za neprijavljene korisnike
                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<TurnirKlasa> turniri = repo.DajSveTurnire();
                return View(turniri);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Index Error: " + ex.Message);
                return View(new List<TurnirKlasa>());
            }
        }

        public ActionResult DetaljiTurnira(int? id)
        {
            if (!id.HasValue || id <= 0)
                return HttpNotFound();

            ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
            TurnirKlasa turnir = repo.DajPoID(id.Value);

            if (turnir == null)
                return HttpNotFound();

            ViewBag.Pobednici = repo.DajRezultate(id.Value);

            // Učitaj takmičare na turniru
            SPTurnirDBKlasa turnirDB = new SPTurnirDBKlasa(_konekcija);
            var dsTakmicari = turnirDB.DajTakmicareNaTurniru(turnir.TurnirID);

            ViewBag.Takmicari = dsTakmicari.Tables.Count > 0 ? dsTakmicari.Tables[0] : new System.Data.DataTable();
            ViewBag.BrojTakmicara = dsTakmicari.Tables.Count > 0 ? dsTakmicari.Tables[0].Rows.Count : 0;

            // Ako je neulogovan korisnik, prikaži samo informacije (bez dugmadi za uređivanje)
            if (Session["organizatorID"] == null)
            {
                ViewBag.IsPublicView = true;
            }

            return View("~/Views/Organizator/DetaljiTurnira.cshtml", turnir);
        }

        public ActionResult About()
        {
            ViewBag.Message = "O Yu-Gi-Oh! Turnirima";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Kontaktirajte nas";
            return View();
        }
    }
}