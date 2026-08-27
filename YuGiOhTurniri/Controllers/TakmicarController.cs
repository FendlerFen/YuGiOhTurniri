using System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using YuGiOhTurniri.Models;
using KlasePodataka;
using Repozitorijumi;
using Prezentaciona_Logika;

namespace YuGiOhTurniri.Controllers
{
    public class TakmicarController : Controller
    {
        private readonly string _konekcija = ConfigurationManager.ConnectionStrings["Konekcija"].ConnectionString;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["takmicarID"] == null)
            {
                filterContext.Result = RedirectToAction("PrijaviTakmicara", "Account");
            }
            base.OnActionExecuting(filterContext);
        }

        // ====== POČETNA STRANICA ======
        public ActionResult Index()
        {
            return View();
        }

        // ====== BAN LISTA (PREGLED) ======
        public ActionResult BanLista()
        {
            try
            {
                IBanListaRepository repo = new BanListaRepozitorijumSP(_konekcija);
                // Takmičar vidi sve zabranjene karte na ban listi
                List<BanListaKlasa> banLista = repo.DajSvuBanListu();

                List<BanListaPrikazVM> vm = new List<BanListaPrikazVM>();
                foreach (var stavka in banLista)
                {
                    vm.Add(new BanListaPrikazVM
                    {
                        BanListaID = stavka.BanListaID,
                        NazivKarte = stavka.NazivKarte,
                        DatumDodavanja = stavka.DatumDodavanja
                    });
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greška pri učitavanju ban liste: " + ex.Message;
                return View(new List<BanListaPrikazVM>());
            }
        }

        // ====== MOJI SPILOVI (sa filterima i pretrагом) ======
        public ActionResult MojiSpilovi(string format = "", string pretraga = "", string status = "")
        {
            try
            {
                int takmicarID = Convert.ToInt32(Session["takmicarID"]);

                // KORISTI SAMO SP VERZIJU
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                List<SpilKlasa> spilovi = repo.DajSpiloveTakmicara(takmicarID);

                // Primeni filtre iz JSON parametara
                FiltriranjeServis filtriranjeServis = new FiltriranjeServis();
                spilovi = filtriranjeServis.Filtriraj(spilovi, format, pretraga);

                // Filtriraj po statusu
                if (!string.IsNullOrEmpty(status))
                {
                    spilovi = spilovi.Where(s => s.Status == status).ToList();
                }

                // Mapiranje na ViewModel
                List<MojiSpiloviVM> vm = new List<MojiSpiloviVM>();
                foreach (var spil in spilovi)
                {
                    vm.Add(new MojiSpiloviVM
                    {
                        SpilID = spil.SpilID,
                        Naziv = spil.Naziv,
                        Format = spil.Format,
                        Arhetip = spil.Arhetip,
                        Status = spil.Status,
                        DatumKreiranja = spil.DatumKreiranja
                    });
                }

                // Dostupni formati za dropdown iz JSON parametara
                ViewBag.Formati = filtriranjeServis.DajDostupneFormate();
                ViewBag.IzabranFormat = format;
                ViewBag.IzabranStatus = status;
                ViewBag.Pretraga = pretraga;

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greška pri učitavanju spilova: " + ex.Message;
                return View(new List<MojiSpiloviVM>());
            }
        }

        // ====== KREIRAJ SPIL ======
        public ActionResult KreirajSpil()
        {
            KreirajSpilVM vm = new KreirajSpilVM();
            vm.Formati = new List<string> { "TCG", "OCG", "Speed Duel" };
            return View(vm);
        }

        [HttpPost]
        public ActionResult KreirajSpil(KreirajSpilVM model)
        {
            model.Formati = new List<string> { "TCG", "OCG", "Speed Duel" };

            // VALIDACIJA - Naziv spila
            if (string.IsNullOrEmpty(model.Naziv) || model.Naziv.Trim().Length == 0)
            {
                ModelState.AddModelError("Naziv", "Naziv spila je obavezan");
                return View(model);
            }

            // VALIDACIJA - Format
            if (string.IsNullOrEmpty(model.Format))
            {
                ModelState.AddModelError("Format", "Format je obavezan");
                return View(model);
            }

            // VALIDACIJA - Ban Lista - Provjeri sve karte
            List<string> karteBanListe = new List<string>();
            try
            {
                IBanListaRepository banListaRepo = new BanListaRepozitorijumSP(_konekcija);
                List<BanListaKlasa> banLista = banListaRepo.DajSvuBanListu();
                karteBanListe = banLista.Select(b => b.NazivKarte.Trim().ToLower()).ToList();
            }
            catch { }

            List<string> karteBanListeUSpilu = new List<string>();

            // Provjeri Main Deck
            if (model.MainDeck != null && model.MainDeck.Count > 0)
            {
                foreach (var karta in model.MainDeck)
                {
                    if (!string.IsNullOrEmpty(karta.NazivKarte))
                    {
                        string kartaNormalizovana = karta.NazivKarte.Trim().ToLower();
                        if (karteBanListe.Contains(kartaNormalizovana))
                        {
                            karteBanListeUSpilu.Add(karta.NazivKarte);
                        }
                    }
                }
            }

            // Provjeri Extra Deck
            if (model.ExtraDeck != null && model.ExtraDeck.Count > 0)
            {
                foreach (var karta in model.ExtraDeck)
                {
                    if (!string.IsNullOrEmpty(karta.NazivKarte))
                    {
                        string kartaNormalizovana = karta.NazivKarte.Trim().ToLower();
                        if (karteBanListe.Contains(kartaNormalizovana))
                        {
                            karteBanListeUSpilu.Add(karta.NazivKarte);
                        }
                    }
                }
            }

            // Provjeri Side Deck
            if (model.SideDeck != null && model.SideDeck.Count > 0)
            {
                foreach (var karta in model.SideDeck)
                {
                    if (!string.IsNullOrEmpty(karta.NazivKarte))
                    {
                        string kartaNormalizovana = karta.NazivKarte.Trim().ToLower();
                        if (karteBanListe.Contains(kartaNormalizovana))
                        {
                            karteBanListeUSpilu.Add(karta.NazivKarte);
                        }
                    }
                }
            }

            // Ako ima zabranjenih karata, prikaži grešku
            if (karteBanListeUSpilu.Count > 0)
            {
                ModelState.AddModelError("", "Spil sadrži zabranjene karte sa ban liste: " + string.Join(", ", karteBanListeUSpilu.Distinct()));
                return View(model);
            }

            // VALIDACIJA - Brojanje karata po sekciji
            int mainCount = 0;
            int extraCount = 0;
            int sideCount = 0;
            List<string> greske = new List<string>();

            // Validacija Main Deck
            if (model.MainDeck != null && model.MainDeck.Count > 0)
            {
                HashSet<string> mainDeckNazivi = new HashSet<string>();
                bool mainDeckHasCardType = false;
                foreach (var karta in model.MainDeck)
                {
                    if (!string.IsNullOrEmpty(karta.NazivKarte) && !string.IsNullOrEmpty(karta.Kolicina))
                    {
                        int kolicina = 0;
                        if (!int.TryParse(karta.Kolicina, out kolicina) || kolicina < 1 || kolicina > 3)
                        {
                            greske.Add("Main Deck: Karta '" + karta.NazivKarte + "' - Kolicina mora biti broj od 1 do 3");
                            continue;
                        }

                        // Provjera da li je tip kartе postavljen
                        if (!string.IsNullOrEmpty(karta.Tip))
                        {
                            mainDeckHasCardType = true;
                        }

                        // Provjera duplikata
                        if (mainDeckNazivi.Contains(karta.NazivKarte.Trim().ToLower()))
                        {
                            greske.Add("Main Deck: Karta '" + karta.NazivKarte + "' se pojavljuje vise puta. Dozvoljene su samo 3x po karti.");
                        }
                        else
                        {
                            mainDeckNazivi.Add(karta.NazivKarte.Trim().ToLower());
                            mainCount += kolicina;
                        }
                    }
                }

                // Ako Main Deck ima karata, mora imati bar jedan tip kartе
                if (mainDeckNazivi.Count > 0 && !mainDeckHasCardType)
                {
                    greske.Add("Morate odabrati tip (Monster, Spell ili Trap) za barem jednu kartu u Main Deck-u");
                }
            }

            // Validacija Extra Deck
            if (model.ExtraDeck != null && model.ExtraDeck.Count > 0)
            {
                HashSet<string> extraDeckNazivi = new HashSet<string>();
                foreach (var karta in model.ExtraDeck)
                {
                    if (!string.IsNullOrEmpty(karta.NazivKarte) && !string.IsNullOrEmpty(karta.Kolicina))
                    {
                        int kolicina = 0;
                        if (!int.TryParse(karta.Kolicina, out kolicina) || kolicina < 1 || kolicina > 3)
                        {
                            greske.Add("Extra Deck: Karta '" + karta.NazivKarte + "' - Kolicina mora biti broj od 1 do 3");
                            continue;
                        }

                        if (extraDeckNazivi.Contains(karta.NazivKarte.Trim().ToLower()))
                        {
                            greske.Add("Extra Deck: Karta '" + karta.NazivKarte + "' se pojavljuje vise puta. Dozvoljene su samo 3x po karti.");
                        }
                        else
                        {
                            extraDeckNazivi.Add(karta.NazivKarte.Trim().ToLower());
                            extraCount += kolicina;
                        }
                    }
                }
            }

            // Validacija Side Deck
            if (model.SideDeck != null && model.SideDeck.Count > 0)
            {
                HashSet<string> sideDeckNazivi = new HashSet<string>();
                foreach (var karta in model.SideDeck)
                {
                    if (!string.IsNullOrEmpty(karta.NazivKarte) && !string.IsNullOrEmpty(karta.Kolicina))
                    {
                        int kolicina = 0;
                        if (!int.TryParse(karta.Kolicina, out kolicina) || kolicina < 1 || kolicina > 3)
                        {
                            greske.Add("Side Deck: Karta '" + karta.NazivKarte + "' - Kolicina mora biti broj od 1 do 3");
                            continue;
                        }

                        if (sideDeckNazivi.Contains(karta.NazivKarte.Trim().ToLower()))
                        {
                            greske.Add("Side Deck: Karta '" + karta.NazivKarte + "' se pojavljuje vise puta. Dozvoljene su samo 3x po karti.");
                        }
                        else
                        {
                            sideDeckNazivi.Add(karta.NazivKarte.Trim().ToLower());
                            sideCount += kolicina;
                        }
                    }
                }
            }

            // VALIDACIJA: Provera veličine decka prema YU-GI-OH pravilima
            var ogranicenja = new Servisi.OgranicenjaServis();
            int minMain = ogranicenja.DajMinBrojKarataMain();
            int maxMain = ogranicenja.DajMaxBrojKarataMain();
            int maxExtra = ogranicenja.DajMaxBrojKarataExtra();
            int maxSide = ogranicenja.DajMaxBrojKarataSide();

            if (mainCount < minMain || mainCount > maxMain)
            {
                greske.Add($"Main Deck mora imati {minMain}-{maxMain} karata (trenutno: {mainCount})");
            }

            if (extraCount > maxExtra)
            {
                greske.Add($"Extra Deck sme imati maksimalno {maxExtra} karata (trenutno: {extraCount})");
            }

            if (sideCount > maxSide)
            {
                greske.Add($"Side Deck sme imati maksimalno {maxSide} karata (trenutno: {sideCount})");
            }

            // Ako nema Main Deck-a
            if (mainCount == 0)
            {
                greske.Add("Main Deck je obavezan i mora imati najmanje 40 karata!");
            }

            // Ako ima greski, prikaži ih
            if (greske.Count > 0)
            {
                foreach (var greska in greske)
                {
                    ModelState.AddModelError("", greska);
                }
                model.Formati = new List<string> { "TCG", "OCG", "Speed Duel" };
                return View(model);
            }

            int takmicarID = Convert.ToInt32(Session["takmicarID"]);

            SpilKlasa spil = new SpilKlasa
            {
                Naziv = model.Naziv,
                Format = model.Format,
                Arhetip = model.Arhetip,
                TakmicarID = takmicarID
            };

            try
            {
                // KREIRAJ SPIL
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                int spilID = repo.Dodaj(spil);

                if (spilID <= 0)
                {
                    ModelState.AddModelError("", "Greska pri kreiranju spila!");
                    return View(model);
                }

                // DODAJ MAIN DECK KARTE
                if (model.MainDeck != null && model.MainDeck.Count > 0)
                {
                    var mainDecKarte = model.MainDeck.Where(k => !string.IsNullOrEmpty(k.NazivKarte) && !string.IsNullOrEmpty(k.Kolicina)).ToList();
                    foreach (var karta in mainDecKarte)
                    {
                        int kolicina = 0;
                        if (int.TryParse(karta.Kolicina, out kolicina) && kolicina > 0)
                        {
                            repo.DodajKartu(spilID, karta.NazivKarte.Trim(), "Main", kolicina, karta.Tip);
                        }
                    }
                }

                // DODAJ EXTRA DECK KARTE
                if (model.ExtraDeck != null && model.ExtraDeck.Count > 0)
                {
                    var extraDeckKarte = model.ExtraDeck.Where(k => !string.IsNullOrEmpty(k.NazivKarte) && !string.IsNullOrEmpty(k.Kolicina)).ToList();
                    foreach (var karta in extraDeckKarte)
                    {
                        int kolicina = 0;
                        if (int.TryParse(karta.Kolicina, out kolicina) && kolicina > 0)
                        {
                            repo.DodajKartu(spilID, karta.NazivKarte.Trim(), "Extra", kolicina);
                        }
                    }
                }

                // DODAJ SIDE DECK KARTE
                if (model.SideDeck != null && model.SideDeck.Count > 0)
                {
                    var sideDeckKarte = model.SideDeck.Where(k => !string.IsNullOrEmpty(k.NazivKarte) && !string.IsNullOrEmpty(k.Kolicina)).ToList();
                    foreach (var karta in sideDeckKarte)
                    {
                        int kolicina = 0;
                        if (int.TryParse(karta.Kolicina, out kolicina) && kolicina > 0)
                        {
                            repo.DodajKartu(spilID, karta.NazivKarte.Trim(), "Side", kolicina);
                        }
                    }
                }

                // Za kompatibilnost - ako je model.Karte postavljen
                if (model.Karte != null && model.Karte.Count > 0)
                {
                    foreach (var karta in model.Karte)
                    {
                        if (!string.IsNullOrEmpty(karta.NazivKarte) && !string.IsNullOrEmpty(karta.Sekcija) && karta.Kolicina > 0)
                        {
                            repo.DodajKartu(spilID, karta.NazivKarte, karta.Sekcija, karta.Kolicina);
                        }
                    }
                }

                TempData["Poruka"] = "Spil uspesno kreiran sa kartama!";
                return RedirectToAction("MojiSpilovi");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Greska pri kreiranju spila: " + ex.Message);
                return View(model);
            }
        }


        public ActionResult DetaljiSpila(int id)
        {
            try
            {
                // KORISTI SAMO SP VERZIJU
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                SpilKlasa spil = repo.DajPoID(id);

                if (spil == null)
                    return HttpNotFound();

                List<KartaUSpiluKlasa> karte = repo.DajKarteSpila(id);
                if (karte == null)
                    karte = new List<KartaUSpiluKlasa>();

                DetaljiSpilVM vm = new DetaljiSpilVM
                {
                    SpilID = spil.SpilID,
                    Naziv = spil.Naziv,
                    Format = spil.Format,
                    Arhetip = spil.Arhetip,
                    Status = spil.Status,
                    DatumKreiranja = spil.DatumKreiranja,
                    NapomenaSudije = spil.NapomenaSudije,
                    Karte = new List<KartaUSpiluVM>()
                };

                foreach (var karta in karte)
                {
                    // Koristi učitanu vrijednost iz baze, ili fallback na heurističko određivanje
                    string tipKarte = !string.IsNullOrEmpty(karta.TipKarte) ? karta.TipKarte : DajTipKarte(karta.NazivKarte);

                    vm.Karte.Add(new KartaUSpiluVM
                    {
                        KartaUSpiluID = karta.KartaUSpiluID,
                        NazivKarte = karta.NazivKarte,
                        Sekcija = karta.Sekcija,
                        Kolicina = karta.Kolicina,
                        TipKarte = tipKarte
                    });
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greška pri učitavanju detalja spila: " + ex.Message;
                return RedirectToAction("MojiSpilovi");
            }
        }

        // Pomoćna metoda za određivanje tipa karte
        private string DajTipKarte(string nazivKarte)
        {
            // Jednostavna logika - u realnom sistemu bi se čitalo iz baze
            if (nazivKarte.Contains("Synchro") || nazivKarte.Contains("Xyz") || nazivKarte.Contains("Link"))
                return "Extra";
            else if (nazivKarte.Contains("[") || nazivKarte.Contains("Effect"))
                return "Monster";
            else
                return "Spell";
        }

        // ====== IZMENI SPIL ======
        public ActionResult IzmeniSpil(int id)
        {
            try
            {
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                SpilKlasa spil = repo.DajPoID(id);

                if (spil == null)
                    return HttpNotFound();

                List<KartaUSpiluKlasa> karte = repo.DajKarteSpila(id);
                if (karte == null)
                    karte = new List<KartaUSpiluKlasa>();

                DetaljiSpilVM vm = new DetaljiSpilVM
                {
                    SpilID = spil.SpilID,
                    Naziv = spil.Naziv,
                    Format = spil.Format,
                    Arhetip = spil.Arhetip,
                    Status = spil.Status,
                    DatumKreiranja = spil.DatumKreiranja,
                    NapomenaSudije = spil.NapomenaSudije,
                    Karte = new List<KartaUSpiluVM>(),
                    Formati = new List<string> { "TCG", "OCG", "Speed Duel" }
                };

                foreach (var karta in karte)
                {
                    vm.Karte.Add(new KartaUSpiluVM
                    {
                        KartaUSpiluID = karta.KartaUSpiluID,
                        NazivKarte = karta.NazivKarte,
                        Sekcija = karta.Sekcija,
                        Kolicina = karta.Kolicina,
                        TipKarte = karta.TipKarte ?? ""
                    });
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greška pri učitavanju spila: " + ex.Message;
                return RedirectToAction("MojiSpilovi");
            }
        }

        [HttpPost]
        public ActionResult IzmeniSpil(DetaljiSpilVM model, FormCollection form)
        {
            try
            {
                // Prvo učitaj karte iz baze ako nisu prispele
                if (model.Karte == null || model.Karte.Count == 0)
                {
                    ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                    List<KartaUSpiluKlasa> karteIzBaze = repo.DajKarteSpila(model.SpilID);
                    if (karteIzBaze == null)
                        karteIzBaze = new List<KartaUSpiluKlasa>();

                    model.Karte = new List<KartaUSpiluVM>();
                    foreach (var k in karteIzBaze)
                    {
                        model.Karte.Add(new KartaUSpiluVM
                        {
                            KartaUSpiluID = k.KartaUSpiluID,
                            NazivKarte = k.NazivKarte,
                            Sekcija = k.Sekcija,
                            Kolicina = k.Kolicina,
                            TipKarte = k.TipKarte ?? ""
                        });
                    }
                }

                ISpilRepozitorijum repozitorijum = new SpilRepozitorijumSP(_konekcija);
                IBanListaRepository banRepo = new BanListaRepozitorijumSP(_konekcija);

                // ===== VALIDACIJA PRE IZMENE =====

                // Prikupi sve nove vrednosti iz forme
                Dictionary<int, (string naziv, int kolicina, string tip)> izmeneKarata = new Dictionary<int, (string, int, string)>();
                List<string> greske = new List<string>();

                foreach (var karta in model.Karte)
                {
                    string nazivKey = $"naziv_{karta.KartaUSpiluID}";
                    string tipKey = $"tip_{karta.KartaUSpiluID}";
                    string količinaKey = $"kolicina_{karta.KartaUSpiluID}";

                    if (form.AllKeys.Contains(nazivKey) && form.AllKeys.Contains(količinaKey))
                    {
                        string noviNaziv = form[nazivKey];
                        string noviTip = form.AllKeys.Contains(tipKey) ? form[tipKey] : "";
                        int novaKolicina = 0;

                        if (int.TryParse(form[količinaKey], out novaKolicina) && novaKolicina > 0)
                        {
                            izmeneKarata[karta.KartaUSpiluID] = (noviNaziv, novaKolicina, noviTip);
                        }
                    }
                }

                // Validacija 1: Proverava Banned karte
                List<BanListaKlasa> banLista = banRepo.DajSvuBanListu();
                foreach (var izmena in izmeneKarata.Values)
                {
                    if (banLista.Any(b => b.NazivKarte.ToLower().Trim() == izmena.naziv.ToLower().Trim()))
                    {
                        greske.Add($"Karta '{izmena.naziv}' je na Ban Listi i ne sme se koristiti!");
                    }
                }

                // Validacija 2: Proverava duplikate - Samo ako je NOVA karta (tekst se promenio)
                var kartePoSekciji = model.Karte.ToDictionary(k => k.KartaUSpiluID, k => k);
                foreach (var izmena in izmeneKarata)
                {
                    int kartaID = izmena.Key;
                    string noviNaziv = izmena.Value.naziv;
                    string stariNaziv = kartePoSekciji[kartaID].NazivKarte;

                    // Ako se naziv promenio, proverava duplikate
                    if (!string.Equals(stariNaziv, noviNaziv, StringComparison.OrdinalIgnoreCase))
                    {
                        int brojDuplikata = kartePoSekciji.Values.Count(k => 
                            !string.Equals(k.NazivKarte, stariNaziv, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(k.NazivKarte, noviNaziv, StringComparison.OrdinalIgnoreCase));

                        if (brojDuplikata > 0)
                        {
                            greske.Add($"Karta '{noviNaziv}' već postoji u spilu!");
                        }
                    }
                }

                // Validacija 3: Brojanje karata po sekciji
                var mainDeckIzmene = model.Karte.Where(k => k.Sekcija == "Main" && izmeneKarata.ContainsKey(k.KartaUSpiluID));
                var extraDeckIzmene = model.Karte.Where(k => k.Sekcija == "Extra" && izmeneKarata.ContainsKey(k.KartaUSpiluID));
                var sideDeckIzmene = model.Karte.Where(k => k.Sekcija == "Side" && izmeneKarata.ContainsKey(k.KartaUSpiluID));

                int mainCount = 0;
                int extraCount = 0;
                int sideCount = 0;

                foreach (var karta in model.Karte)
                {
                    if (izmeneKarata.ContainsKey(karta.KartaUSpiluID))
                    {
                        int novaKolicina = izmeneKarata[karta.KartaUSpiluID].kolicina;
                        if (karta.Sekcija == "Main")
                            mainCount += novaKolicina;
                        else if (karta.Sekcija == "Extra")
                            extraCount += novaKolicina;
                        else if (karta.Sekcija == "Side")
                            sideCount += novaKolicina;
                    }
                    else
                    {
                        // Karte koje nisu izmenjene - drži stare vrednosti
                        if (karta.Sekcija == "Main")
                            mainCount += karta.Kolicina;
                        else if (karta.Sekcija == "Extra")
                            extraCount += karta.Kolicina;
                        else if (karta.Sekcija == "Side")
                            sideCount += karta.Kolicina;
                    }
                }

                // Validacija 4: Proverava veličine decka
                var ogranicenja = new Servisi.OgranicenjaServis();
                int minMain = ogranicenja.DajMinBrojKarataMain();
                int maxMain = ogranicenja.DajMaxBrojKarataMain();
                int maxExtra = ogranicenja.DajMaxBrojKarataExtra();
                int maxSide = ogranicenja.DajMaxBrojKarataSide();

                if (mainCount < minMain || mainCount > maxMain)
                {
                    greske.Add($"Main Deck mora imati {minMain}-{maxMain} karata (trenutno: {mainCount})");
                }

                if (extraCount > maxExtra)
                {
                    greske.Add($"Extra Deck sme imati maksimalno {maxExtra} karata (trenutno: {extraCount})");
                }

                if (sideCount > maxSide)
                {
                    greske.Add($"Side Deck sme imati maksimalno {maxSide} karata (trenutno: {sideCount})");
                }

                // Ako ima grešaka, prikaži ih
                if (greske.Count > 0)
                {
                    foreach (var greska in greske)
                    {
                        ModelState.AddModelError("", greska);
                    }
                    model.Formati = new List<string> { "TCG", "OCG", "Speed Duel" };
                    return View(model);
                }

                // ===== IZMENA SPILA =====

                // Ažurira spil metadata
                SpilKlasa spil = new SpilKlasa
                {
                    SpilID = model.SpilID,
                    Naziv = model.Naziv,
                    Format = model.Format,
                    Arhetip = model.Arhetip,
                    Status = "Na cekanju" 
                };

                bool uspeh = repozitorijum.Izmeni(spil);
                if (!uspeh)
                {
                    ModelState.AddModelError("", "Greska pri ažuriranju spila!");
                    model.Formati = new List<string> { "TCG", "OCG", "Speed Duel" };
                    return View(model);
                }

                // Ažurira karte
                foreach (var izmena in izmeneKarata)
                {
                    repozitorijum.AzurirajKartu(izmena.Key, izmena.Value.naziv, izmena.Value.kolicina, izmena.Value.tip);
                }

                TempData["Poruka"] = "Spil uspesno izmenen!";
                return RedirectToAction("DetaljiSpila", new { id = model.SpilID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Greska pri ažuriranju: " + ex.Message);
                if (model != null)
                {
                    model.Formati = new List<string> { "TCG", "OCG", "Speed Duel" };
                }
                return View(model);
            }
        }

        // ====== DODAJ KARTU ======
        [HttpPost]
        public ActionResult DodajKartu(int spilID, string nazivKarte, string sekcija, int kolicina)
        {
            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            bool uspeh = repo.DodajKartu(spilID, nazivKarte, sekcija, kolicina);

            TempData["Poruka"] = uspeh
                ? "Karta dodana!"
                : "Greska pri dodavanju karte!";

            return RedirectToAction("DetaljiSpila", new { id = spilID });
        }

        // ====== OBRISI SPIL ======
        public ActionResult ObrisiSpil(int id)
        {
            // KORISTI SAMO SP VERZIJU
            ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
            bool uspeh = repo.Obrisi(id);

            TempData["Poruka"] = uspeh
                ? "Spil obrisan!"
                : "Greška pri brisanju spila!";

            return RedirectToAction("MojiSpilovi");
        }

        // ====== ŠTAMPA SPILA (Parametarska Štampa - Master-Detail) ======
        public ActionResult StampajSpil(int id)
        {
            try
            {
                ISpilRepozitorijum repo = new SpilRepozitorijumSP(_konekcija);
                SpilKlasa spil = repo.DajPoID(id);

                if (spil == null)
                    return HttpNotFound();

                List<KartaUSpiluKlasa> karte = repo.DajKarteSpila(id);
                if (karte == null)
                    karte = new List<KartaUSpiluKlasa>();

                // Dohvati podatke o takmičaru
                string takmicarIme = "";
                if (spil.TakmicarID > 0)
                {
                    try
                    {
                        ITakmicarRepozitorijum takmicarRepo = new TakmicarRepozitorijumSP(_konekcija);
                        TakmicarKlasa takmicar = takmicarRepo.DajPoID(spil.TakmicarID);
                        if (takmicar != null)
                            takmicarIme = takmicar.Ime + " " + takmicar.Prezime;
                    }
                    catch { }
                }

                // Generiši HTML štampu sa master-detail podacima
                StampacijuServis stampServis = new StampacijuServis();
                string htmlStampa = stampServis.GenerirajStampuSpila(spil, karte, takmicarIme);

                return Content(htmlStampa, "text/html");
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greška pri generisanju štampe: " + ex.Message;
                return View();
            }
        }

        // ====== OTVORENI TURNIRI (Za Prijave) ======
        public ActionResult OtvoreniTurniri()
        {
            try
            {
                int takmicarID = Convert.ToInt32(Session["takmicarID"]);

                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<TurnirKlasa> turniri = repo.DajSveTurnire();

                // Filtriraj samo otvorene turnire
                turniri = turniri.Where(t => t.Status == "Otvoren").ToList();

                List<OtvoreniTurniriVM> vm = new List<OtvoreniTurniriVM>();
                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                SPOrganizatorDBKlasa orgDB = new SPOrganizatorDBKlasa(_konekcija);

                foreach (var turnir in turniri)
                {
                    // Dohvati sve takmičare na turniru
                    DataSet dsTakmicari = db.DajTakmicareNaTurniru(turnir.TurnirID);
                    int brojTakmicara = 0;
                    bool isRegistered = false;

                    if (dsTakmicari.Tables.Count > 0 && dsTakmicari.Tables[0].Rows.Count > 0)
                    {
                        brojTakmicara = dsTakmicari.Tables[0].Rows.Count;
                        isRegistered = dsTakmicari.Tables[0].Rows.Cast<DataRow>()
                            .Any(r => Convert.ToInt32(r["TakmicarID"]) == takmicarID);
                    }

                    // Dohvati organizatora
                    string organizatorIme = "";
                    try
                    {
                        OrganizatorKlasa org = orgDB.DajPoID(turnir.OrganizatorID);
                        if (org != null)
                        {
                            organizatorIme = org.Ime + " " + org.Prezime;
                        }
                    }
                    catch { }

                    vm.Add(new OtvoreniTurniriVM
                    {
                        TurnirID = turnir.TurnirID,
                        Naziv = turnir.Naziv,
                        Lokacija = turnir.Lokacija,
                        Format = turnir.Format,
                        DatumOdrzavanja = turnir.DatumOdrzavanja,
                        Organizator = "Organizator: " + turnir.OrganizatorID,
                        OrganizatorIme = organizatorIme,
                        BrojPrijavljenih = brojTakmicara,
                        Status = turnir.Status,
                        IsUserRegistered = isRegistered
                    });
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greška pri učitavanju turnira: " + ex.Message;
                return View(new List<OtvoreniTurniriVM>());
            }
        }

        // ====== PRIJAVI SE NA TURNIR (GET - Odaberi Spil) ======
        public ActionResult PrijaviSeTurnir(int id)
        {
            try
            {
                int takmicarID = Convert.ToInt32(Session["takmicarID"]);

                // Dohvati turnir
                ITurnirRepozitorijum turnirRepo = new TurnirRepozitorijumSP(_konekcija);
                TurnirKlasa turnir = turnirRepo.DajPoID(id);

                if (turnir == null)
                {
                    TempData["Greska"] = "Turnir nije pronađen.";
                    return RedirectToAction("OtvoreniTurniri");
                }

                // Dohvati sve spilove takmičara
                ISpilRepozitorijum spilRepo = new SpilRepozitorijumSP(_konekcija);
                List<SpilKlasa> spilovi = spilRepo.DajSpiloveTakmicara(takmicarID);

                // Filtriraj samo spilove sa istim formatom kao turnir i koji su ODOBREN
                List<SpilKlasa> odgovarajuciSpilovi = new List<SpilKlasa>();
                foreach (var spil in spilovi)
                {
                    if (spil.Format == turnir.Format && spil.Status == "Odobren")
                    {
                        odgovarajuciSpilovi.Add(spil);
                    }
                }

                // Ako nema odobljenih spilova, prikaži grešku
                if (odgovarajuciSpilovi.Count == 0)
                {
                    TempData["Greska"] = "Nemate kreirane spilove u formatu '" + turnir.Format + "' ili svi vaši spilovi su u procesu revizije. Spil mora biti odobren od strane sudije prije nego što se možete prijaviti.";
                    return RedirectToAction("OtvoreniTurniri");
                }

                OdaberiSpilZaTurnirVM vm = new OdaberiSpilZaTurnirVM
                {
                    TurnirID = id,
                    TurnirNaziv = turnir.Naziv,
                    TurnirFormat = turnir.Format,
                    Spilovi = odgovarajuciSpilovi
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greška pri učitavanju spilova: " + ex.Message;
                return RedirectToAction("OtvoreniTurniri");
            }
        }

        // ====== POTVRDI PRIJAVU NA TURNIR ======
        [HttpPost]
        public ActionResult PotvrdiPrijavuNaTurnir(int turnirID, int spilID)
        {
            try
            {
                int takmicarID = Convert.ToInt32(Session["takmicarID"]);

                // Dohvati turnir
                ITurnirRepozitorijum turnirRepo = new TurnirRepozitorijumSP(_konekcija);
                TurnirKlasa turnir = turnirRepo.DajPoID(turnirID);

                if (turnir == null)
                {
                    TempData["Greska"] = "Turnir nije pronađen.";
                    return RedirectToAction("OtvoreniTurniri");
                }

                // Dohvati spil
                ISpilRepozitorijum spilRepo = new SpilRepozitorijumSP(_konekcija);
                SpilKlasa spil = spilRepo.DajPoID(spilID);

                if (spil == null)
                {
                    TempData["Greska"] = "Spil nije pronađen.";
                    return RedirectToAction("OtvoreniTurniri");
                }

                // Validacija: Format mora biti isti
                if (spil.Format != turnir.Format)
                {
                    TempData["Greska"] = $"Spil ima format '{spil.Format}', a turnir zahteva '{turnir.Format}'!";
                    return RedirectToAction("OtvoreniTurniri");
                }

                // Validacija: Spil mora biti odobren
                if (spil.Status != "Odobren")
                {
                    TempData["Greska"] = "Spil mora biti odobren od strane sudije prije nego što se možete prijaviti.";
                    return RedirectToAction("OtvoreniTurniri");
                }

                // Dodaj takmičara na turnir sa spilom
                SPTurnirDBKlasa turnirDB = new SPTurnirDBKlasa(_konekcija);
                bool uspeh = turnirDB.DodajTakmicaraTurniru(turnirID, takmicarID, spilID);

                if (uspeh)
                {
                    TempData["Poruka"] = $"Uspešno ste se prijavili na turnir '{turnir.Naziv}' sa spilom '{spil.Naziv}'!";
                    return RedirectToAction("OtvoreniTurniri");
                }
                else
                {
                    TempData["Greska"] = "Već ste registrovani za ovaj turnir ili je došlo do greške!";
                    return RedirectToAction("OtvoreniTurniri");
                }
            }
            catch (Exception ex)
            {
                TempData["Greska"] = "Greška pri prijavi: " + ex.Message;
                return RedirectToAction("OtvoreniTurniri");
            }
        }

        // Helper metoda za popunjavanje liste otvorenih turnira
        private List<OtvoreniTurniriVM> GetOtvoreniTurniriVezaForoView(int takmicarID)
        {
            try
            {
                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<TurnirKlasa> turniri = repo.DajSveTurnire();

                // Filtriraj samo otvorene turnire
                turniri = turniri.Where(t => t.Status == "Otvoren").ToList();

                List<OtvoreniTurniriVM> vm = new List<OtvoreniTurniriVM>();
                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                SPOrganizatorDBKlasa orgDB = new SPOrganizatorDBKlasa(_konekcija);

                foreach (var turnir in turniri)
                {
                    // Dohvati sve takmičare na turniru
                    DataSet dsTakmicari = db.DajTakmicareNaTurniru(turnir.TurnirID);
                    int brojTakmicara = 0;
                    bool isRegistered = false;

                    if (dsTakmicari.Tables.Count > 0 && dsTakmicari.Tables[0].Rows.Count > 0)
                    {
                        brojTakmicara = dsTakmicari.Tables[0].Rows.Count;
                        isRegistered = dsTakmicari.Tables[0].Rows.Cast<DataRow>()
                            .Any(r => Convert.ToInt32(r["TakmicarID"]) == takmicarID);
                    }

                    // Dohvati organizatora
                    string organizatorIme = "";
                    try
                    {
                        OrganizatorKlasa org = orgDB.DajPoID(turnir.OrganizatorID);
                        if (org != null)
                        {
                            organizatorIme = org.Ime + " " + org.Prezime;
                        }
                    }
                    catch { }

                    vm.Add(new OtvoreniTurniriVM
                    {
                        TurnirID = turnir.TurnirID,
                        Naziv = turnir.Naziv,
                        Lokacija = turnir.Lokacija,
                        Format = turnir.Format,
                        DatumOdrzavanja = turnir.DatumOdrzavanja,
                        Organizator = "Organizator: " + turnir.OrganizatorID,
                        OrganizatorIme = organizatorIme,
                        BrojPrijavljenih = brojTakmicara,
                        Status = turnir.Status,
                        IsUserRegistered = isRegistered
                    });
                }

                return vm;
            }
            catch
            {
                return new List<OtvoreniTurniriVM>();
            }
        }

        // ====== ZAVRŠENI TURNIRI (Sa Pobjednicima) ======
        public ActionResult ZavrsenTurniri()
        {
            try
            {
                ITurnirRepozitorijum repo = new TurnirRepozitorijumSP(_konekcija);
                List<TurnirKlasa> turniri = repo.DajSveTurnire();

                // Filtriraj samo završene turnire
                turniri = turniri.Where(t => t.Status == "Zavrsen").ToList();

                List<OtvoreniTurniriVM> vm = new List<OtvoreniTurniriVM>();
                SPTurnirDBKlasa db = new SPTurnirDBKlasa(_konekcija);
                SPOrganizatorDBKlasa orgDB = new SPOrganizatorDBKlasa(_konekcija);

                foreach (var turnir in turniri)
                {
                    // Dohvati sve takmičare na turniru
                    DataSet dsTakmicari = db.DajTakmicareNaTurniru(turnir.TurnirID);
                    int brojTakmicara = 0;

                    if (dsTakmicari.Tables.Count > 0 && dsTakmicari.Tables[0].Rows.Count > 0)
                    {
                        brojTakmicara = dsTakmicari.Tables[0].Rows.Count;
                    }

                    // Dohvati organizatora
                    string organizatorIme = "";
                    try
                    {
                        OrganizatorKlasa org = orgDB.DajPoID(turnir.OrganizatorID);
                        if (org != null)
                        {
                            organizatorIme = org.Ime + " " + org.Prezime;
                        }
                    }
                    catch { }

                    vm.Add(new OtvoreniTurniriVM
                    {
                        TurnirID = turnir.TurnirID,
                        Naziv = turnir.Naziv,
                        Lokacija = turnir.Lokacija,
                        Format = turnir.Format,
                        DatumOdrzavanja = turnir.DatumOdrzavanja,
                        Organizator = "Organizator: " + turnir.OrganizatorID,
                        OrganizatorIme = organizatorIme,
                        BrojPrijavljenih = brojTakmicara,
                        Status = turnir.Status,
                        IsUserRegistered = false
                    });
                }

                return View("OtvoreniTurniri", vm);
            }
            catch (Exception ex)
            {
                ViewBag.Greska = "Greška pri učitavanju turnira: " + ex.Message;
                return View("OtvoreniTurniri", new List<OtvoreniTurniriVM>());
            }
        }
    }
}