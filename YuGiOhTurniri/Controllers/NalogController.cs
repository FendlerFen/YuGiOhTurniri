using Prezentaciona_Logika;
using KlasePodataka;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using YuGiOhTurniri.Services;
using Prezentaciona_Logika;

namespace YuGiOhTurniri.Controllers
{
    public class NalogController : Controller
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;
        private readonly HttpKlijentServis _httpServis = new HttpKlijentServis();

        public ActionResult PrijaviTakmicara()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PrijaviTakmicara(Models.PrijavaVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                string errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
                ViewBag.Greska = "Greske pri validaciji: " + errorMessage;
                return View(model);
            }

            // Prijava i dalje ide preko prezentacione logike (nema /api/login endpointa)
            var forma = new FormaTakmicaraKlasa(_konekcija);
            TakmicarKlasa takmicar = forma.LoginTakmicar(model.Email, model.Lozinka);

            if (takmicar != null)
            {
                Session["takmicarID"] = takmicar.TakmicarID;
                Session["ime"] = takmicar.Ime;
                Session["prezime"] = takmicar.Prezime;
                Session["email"] = takmicar.Email;
                return RedirectToAction("Index", "Takmicar");
            }

            ViewBag.Greska = "Pogresan email ili lozinka!";
            return View(model);
        }

        public ActionResult RegistrujTakmicara()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrujTakmicara(Models.RegistracijaTakmicaraVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                string errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
                ViewBag.Greska = "Gre?ke pri validaciji: " + errorMessage;
                return View(model);
            }

            // Registracija preko REST API-ja: POST /api/takmicari
            var takmicar = new TakmicarKlasa
            {
                Ime = model.Ime,
                Prezime = model.Prezime,
                Email = model.Email,
                DatumRodjenja = model.DatumRodjenja,
                Drzava = model.Drzava,
                Pol = model.Pol,
                Lozinka = model.Lozinka
            };

            try
            {
                var kreirajTask = _httpServis.KreirajTakmicaraAsync(takmicar);
                kreirajTask.Wait();
                int noviID = kreirajTask.Result;

                ViewBag.Poruka = "Takmi?ar uspe?no registrovan! Molim vas prijavite se.";
                return View("../Nalog/PrijaviTakmicara");
            }
            catch (Exception ex)
            {
                ViewBag.Greska = ex.Message ?? "Registracija nije uspela.";
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Kuca");
        }
    }
}
