using System;
using System.Collections.Generic;
using System.Text;
using KlasePodataka;

namespace Prezentaciona_Logika
{
    /// Servis za generisanje parametarske stampe (master-detail dokumenata)
    /// Koristi se za ispis spilova, turnira i njihovih detalja
    public class StampacijuServis
    {
        /// Generise HTML stampu za spil sa svim kartama (master-detail)
        public string GenerirajStampuSpila(SpilKlasa spil, List<KartaUSpiluKlasa> karte, string takmicarIme = "")
        {
            if (spil == null)
                return "";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='utf-8'>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            sb.AppendLine(".header { text-align: center; margin-bottom: 20px; border-bottom: 2px solid #000; padding-bottom: 10px; }");
            sb.AppendLine(".header h1 { margin: 0; }");
            sb.AppendLine(".header h3 { margin: 5px 0; color: #666; }");
            sb.AppendLine(".section { margin: 20px 0; }");
            sb.AppendLine(".section h2 { background-color: #f0f0f0; padding: 10px; border-left: 4px solid #333; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 10px 0; }");
            sb.AppendLine("th, td { padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("th { background-color: #f0f0f0; font-weight: bold; }");
            sb.AppendLine("tr:hover { background-color: #f9f9f9; }");
            sb.AppendLine(".status { padding: 2px 8px; border-radius: 3px; font-weight: bold; }");
            sb.AppendLine(".status-odobren { background-color: #d4edda; color: #155724; }");
            sb.AppendLine(".status-cekanje { background-color: #fff3cd; color: #856404; }");
            sb.AppendLine(".status-odbijen { background-color: #f8d7da; color: #721c24; }");
            sb.AppendLine(".summary { background-color: #f9f9f9; padding: 10px; margin: 10px 0; border-left: 4px solid #007bff; }");
            sb.AppendLine("@media print { body { margin: 0; } .no-print { display: none; } }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // HEADER
            sb.AppendLine("<div class='header'>");
            sb.AppendLine($"<h1>Yu-Gi-Oh! SPIL</h1>");
            sb.AppendLine($"<h3>Dekl broj: {spil.SpilID}</h3>");
            sb.AppendLine("</div>");

            // OSNOVNO INFORMACIJE O SPILU (Master)
            sb.AppendLine("<div class='section'>");
            sb.AppendLine("<h2>Osnovne Informacije</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Svojstvo</th><th>Vrijednost</th></tr>");
            sb.AppendLine($"<tr><td><strong>Naziv Spila:</strong></td><td>{spil.Naziv}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Format:</strong></td><td>{spil.Format}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Arhetip:</strong></td><td>{spil.Arhetip}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Status:</strong></td><td><span class='status status-{spil.Status.ToLower().Replace(" ", "")}'>{spil.Status}</span></td></tr>");
            sb.AppendLine($"<tr><td><strong>Datum Kreiranja:</strong></td><td>{spil.DatumKreiranja:dd.MM.yyyy}</td></tr>");
            if (!string.IsNullOrEmpty(takmicarIme))
                sb.AppendLine($"<tr><td><strong>Vlasnik:</strong></td><td>{takmicarIme}</td></tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            // KARTE (Detail)
            if (karte != null && karte.Count > 0)
            {
                // Grupiraj po sekciji
                var mainDeck = new List<KartaUSpiluKlasa>();
                var extraDeck = new List<KartaUSpiluKlasa>();
                var sideDeck = new List<KartaUSpiluKlasa>();

                foreach (var karta in karte)
                {
                    if (karta.Sekcija == "Main")
                        mainDeck.Add(karta);
                    else if (karta.Sekcija == "Extra")
                        extraDeck.Add(karta);
                    else if (karta.Sekcija == "Side")
                        sideDeck.Add(karta);
                }

                // Main Deck
                if (mainDeck.Count > 0)
                {
                    sb.AppendLine("<div class='section'>");
                    sb.AppendLine($"<h2>Main Deck ({mainDeck.Count} karata)</h2>");
                    sb.AppendLine("<table>");
                    sb.AppendLine("<tr><th>Red.Br.</th><th>Naziv Karte</th><th>Koli?ina</th><th>Tip</th></tr>");
                    int redBr = 1;
                    foreach (var karta in mainDeck)
                    {
                        sb.AppendLine($"<tr><td>{redBr}</td><td>{karta.NazivKarte}</td><td>{karta.Kolicina}</td><td>{(string.IsNullOrEmpty(karta.TipKarte) ? "-" : karta.TipKarte)}</td></tr>");
                        redBr++;
                    }
                    sb.AppendLine("</table>");
                    sb.AppendLine("</div>");
                }

                // Extra Deck
                if (extraDeck.Count > 0)
                {
                    sb.AppendLine("<div class='section'>");
                    sb.AppendLine($"<h2>Extra Deck ({extraDeck.Count} karata)</h2>");
                    sb.AppendLine("<table>");
                    sb.AppendLine("<tr><th>Red.Br.</th><th>Naziv Karte</th><th>Koli?ina</th><th>Tip</th></tr>");
                    int redBr = 1;
                    foreach (var karta in extraDeck)
                    {
                        sb.AppendLine($"<tr><td>{redBr}</td><td>{karta.NazivKarte}</td><td>{karta.Kolicina}</td><td>{(string.IsNullOrEmpty(karta.TipKarte) ? "-" : karta.TipKarte)}</td></tr>");
                        redBr++;
                    }
                    sb.AppendLine("</table>");
                    sb.AppendLine("</div>");
                }

                // Side Deck
                if (sideDeck.Count > 0)
                {
                    sb.AppendLine("<div class='section'>");
                    sb.AppendLine($"<h2>Side Deck ({sideDeck.Count} karata)</h2>");
                    sb.AppendLine("<table>");
                    sb.AppendLine("<tr><th>Red.Br.</th><th>Naziv Karte</th><th>Koli?ina</th><th>Tip</th></tr>");
                    int redBr = 1;
                    foreach (var karta in sideDeck)
                    {
                        sb.AppendLine($"<tr><td>{redBr}</td><td>{karta.NazivKarte}</td><td>{karta.Kolicina}</td><td>{(string.IsNullOrEmpty(karta.TipKarte) ? "-" : karta.TipKarte)}</td></tr>");
                        redBr++;
                    }
                    sb.AppendLine("</table>");
                    sb.AppendLine("</div>");
                }

                sb.AppendLine("<div class='section'>");
                sb.AppendLine("<h2>Rezime</h2>");
                sb.AppendLine($"<div class='summary'>");
                sb.AppendLine($"<p><strong>Ukupno karata:</strong> {karte.Count}</p>");
                sb.AppendLine($"<p><strong>Main Deck:</strong> {mainDeck.Count} karata</p>");
                sb.AppendLine($"<p><strong>Extra Deck:</strong> {extraDeck.Count} karata</p>");
                sb.AppendLine($"<p><strong>Side Deck:</strong> {sideDeck.Count} karata</p>");
                sb.AppendLine($"</div>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("<hr>");
            sb.AppendLine($"<p style='text-align: center; color: #999; font-size: 12px;'>Ispisano: {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        /// Generise HTML stampu za turnir sa takmicarima (master-detail)
        public string GenerirajStampuTurnira(TurnirKlasa turnir, List<string> takmicari, List<RezultatKlasa> rezultati)
        {
            if (turnir == null)
                return "";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='utf-8'>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            sb.AppendLine(".header { text-align: center; margin-bottom: 20px; border-bottom: 2px solid #000; padding-bottom: 10px; }");
            sb.AppendLine(".header h1 { margin: 0; }");
            sb.AppendLine(".section { margin: 20px 0; }");
            sb.AppendLine(".section h2 { background-color: #f0f0f0; padding: 10px; border-left: 4px solid #333; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 10px 0; }");
            sb.AppendLine("th, td { padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("th { background-color: #f0f0f0; font-weight: bold; }");
            sb.AppendLine("tr:hover { background-color: #f9f9f9; }");
            sb.AppendLine("@media print { body { margin: 0; } }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // HEADER
            sb.AppendLine("<div class='header'>");
            sb.AppendLine($"<h1>Yu-Gi-Oh! TURNIR</h1>");
            sb.AppendLine($"<h3>Broj turnira: {turnir.TurnirID}</h3>");
            sb.AppendLine("</div>");

            // OSNOVNO INFORMACIJE O TURNIRU (Master)
            sb.AppendLine("<div class='section'>");
            sb.AppendLine("<h2>Osnovne Informacije</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Svojstvo</th><th>Vrijednost</th></tr>");
            sb.AppendLine($"<tr><td><strong>Naziv Turnira:</strong></td><td>{turnir.Naziv}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Lokacija:</strong></td><td>{turnir.Lokacija}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Format:</strong></td><td>{turnir.Format}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Datum Odrzavanja:</strong></td><td>{turnir.DatumOdrzavanja:dd.MM.yyyy}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Status:</strong></td><td>{turnir.Status}</td></tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            // TAKMI?ARI (Detail)
            if (takmicari != null && takmicari.Count > 0)
            {
                sb.AppendLine("<div class='section'>");
                sb.AppendLine($"<h2>Prijavljeni Takmi?ari ({takmicari.Count})</h2>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Red.Br.</th><th>Takmi?ar</th></tr>");
                int redBr = 1;
                foreach (var takmicari_item in takmicari)
                {
                    sb.AppendLine($"<tr><td>{redBr}</td><td>{takmicari_item}</td></tr>");
                    redBr++;
                }
                sb.AppendLine("</table>");
                sb.AppendLine("</div>");
            }

            // REZULTATI (Detail)
            if (rezultati != null && rezultati.Count > 0)
            {
                sb.AppendLine("<div class='section'>");
                sb.AppendLine($"<h2>Rezultati</h2>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Mjesto</th><th>Takmi?ar</th><th>Broj Pobjeda</th></tr>");
                foreach (var rezultat in rezultati)
                {
                    sb.AppendLine($"<tr><td>{rezultat.Mesto}.</td><td>{rezultat.Takmicari}</td><td>{rezultat.BrojPobeda}</td></tr>");
                }
                sb.AppendLine("</table>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("<hr>");
            sb.AppendLine($"<p style='text-align: center; color: #999; font-size: 12px;'>Ispisano: {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }
}
