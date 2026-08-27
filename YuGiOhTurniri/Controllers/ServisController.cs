using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using KlasePodataka;
using Repozitorijumi;
using Newtonsoft.Json;
using Servisi;

namespace YuGiOhTurniri.Controllers
{
    // REST Servis - Parametrizacija i CRUD operacije
    // Obezbeđuje parametre za poslovnu logiku
    // Međusloj između prezentacionog sloja i sloja za rad sa podacima
    public class ServisController : Controller
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        // ====== PARAMETRI OGRANIČENJA SPILA (JSON) ======
        [HttpGet]
        public JsonResult OgraniceSpila()
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

            return Json(ogranicenja, JsonRequestBehavior.AllowGet);
        }

        // ====== TAKMIČARI - CRUD ======

        // READ: Svi takmičari
        [HttpGet]
        public JsonResult DajSveTakmicara()
        {
            try
            {
                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                List<TakmicarKlasa> takmicari = repo.DajSveTakmicara();
                return Json(new { success = true, data = takmicari }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // READ: Takmičar po ID-u
        [HttpGet]
        public JsonResult DajTakmicara(int id)
        {
            try
            {
                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                TakmicarKlasa takmicar = repo.DajPoID(id);

                if (takmicar == null)
                    return Json(new { success = false, message = "Takmičar nije pronađen" }, JsonRequestBehavior.AllowGet);

                return Json(new { success = true, data = takmicar }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ====== TURNIRI - CRUD ======

        // READ: Svi turniri
        [HttpGet]
        public JsonResult DajSveTurnire()
        {
            try
            {
                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<TurnirKlasa> turniri = repo.DajSveTurnire();
                return Json(new { success = true, data = turniri }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // READ: Turnir po ID-u
        [HttpGet]
        public JsonResult DajTurnir(int id)
        {
            try
            {
                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                TurnirKlasa turnir = repo.DajPoID(id);

                if (turnir == null)
                    return Json(new { success = false, message = "Turnir nije pronađen" }, JsonRequestBehavior.AllowGet);

                return Json(new { success = true, data = turnir }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // READ: Otvoreni turniri
        [HttpGet]
        public JsonResult DajOtvoreneTurnire()
        {
            try
            {
                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<TurnirKlasa> turniri = repo.DajOtvoreneTurnire();
                return Json(new { success = true, data = turniri }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ====== SPILOVI - CRUD ======

        // READ: Spil po ID-u
        [HttpGet]
        public JsonResult DajSpil(int id)
        {
            try
            {
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                SpilKlasa spil = repo.DajPoID(id);

                if (spil == null)
                    return Json(new { success = false, message = "Spil nije pronađen" }, JsonRequestBehavior.AllowGet);

                return Json(new { success = true, data = spil }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // READ: Spilovi takmičara
        [HttpGet]
        public JsonResult DajSpiloveTakmicara(int takmicarID)
        {
            try
            {
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                List<SpilKlasa> spilovi = repo.DajSpiloveTakmicara(takmicarID);
                return Json(new { success = true, data = spilovi }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ====== BAN LISTA - READ ======

        // READ: Ban lista (sve zabranjene karte)
        [HttpGet]
        public JsonResult DajBanListu()
        {
            try
            {
                IBanListaRepository repo = new BanListaRepozitorijumSP(_konekcija);
                List<BanListaKlasa> banLista = repo.DajSvuBanListu();
                return Json(new { success = true, data = banLista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ====== TOP LISTA (TAKMIČARI) ======

        [HttpGet]
        public JsonResult DajTopListu()
        {
            try
            {
                ITakmicarRepozitorijum repo = new TakmicarRepozitorijumSP(_konekcija);
                List<TakmicarKlasa> topLista = repo.DajSveTakmicara();
                return Json(new { success = true, data = topLista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ====== REZULTATI ======

        // READ: Pobednici turnira
        [HttpGet]
        public JsonResult DajPobednike(int turnirID)
        {
            try
            {
                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<RezultatKlasa> rezultati = repo.DajPobednike(turnirID);
                return Json(new { success = true, data = rezultati }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}