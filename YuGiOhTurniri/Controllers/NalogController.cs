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
                ViewBag.Greska = "Gre?ke pri validaciji: " + errorMessage;
                return View(model);
            }

            // Prijava - direktno iz baze
            try
            {
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

                ViewBag.Greska = "Pogre?an email ili lozinka!";
            }
            catch (Exception ex)
            {
                ViewBag.Greska = ex.Message ?? "Gre?ka pri loginu.";
            }

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

            // Registracija - direktno u bazu
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
                var forma = new FormaTakmicaraKlasa(_konekcija);
                string poruka = forma.RegistrujTakmicaraSaPorukom(takmicar);

                if (poruka.Contains("uspe?no"))
                {
                    ViewBag.Poruka = poruka;
                    return RedirectToAction("PrijaviTakmicara");
                }

                ViewBag.Greska = poruka;
            }
            catch (Exception ex)
            {
                ViewBag.Greska = ex.Message ?? "Gre?ka pri registraciji.";
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Kuca");
        }
    }
}
