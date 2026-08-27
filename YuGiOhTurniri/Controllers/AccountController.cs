using System.Web.Mvc;
using Prezentaciona_Logika;
using KlasePodataka;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace YuGiOhTurniri.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        public ActionResult PrijaviTakmicara()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PrijaviTakmicara(Models.PrijavaVM model)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] PrijaviTakmicara POST - Email: {model?.Email}, Password length: {model?.Lozinka?.Length ?? 0}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                string errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
                System.Diagnostics.Debug.WriteLine($"[DEBUG] ModelState invalid: {errorMessage}");
                ViewBag.Greska = "Greske pri validaciji: " + errorMessage;
                return View(model);
            }

            var forma = new FormaTakmicaraKlasa(_konekcija);
            TakmicarKlasa takmicar = forma.LoginTakmicar(model.Email, model.Lozinka);

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Login result: {(takmicar != null ? "SUCCESS - ID: " + takmicar.TakmicarID : "FAILED")}");

            if (takmicar != null)
            {
                Session["takmicarID"] = takmicar.TakmicarID;
                Session["ime"] = takmicar.Ime;
                Session["prezime"] = takmicar.Prezime;
                Session["email"] = takmicar.Email;

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Session set, redirecting to Takmicar/Index");
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
                // Postavi debug info
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                string errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
                ViewBag.Greska = "Greske pri validaciji: " + errorMessage;
                return View(model);
            }

            var forma = new FormaTakmicaraKlasa(_konekcija);

            string rezultat = forma.RegistrujTakmicara(
                model.Ime,
                model.Prezime,
                model.Email,
                model.DatumRodjenja,
                model.Drzava,
                model.Pol,
                model.Lozinka
            );

            if (rezultat.Contains("registrovan"))
            {
                ViewBag.Poruka = "Takmicara uspesno registrovan! Molimo prijavite se.";
                return View("../Account/PrijaviTakmicara");
            }

            ViewBag.Greska = rezultat;
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("PrijaviTakmicara");
        }
    }
}

