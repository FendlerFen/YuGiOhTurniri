using System;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPSpilDBKlasa
    {
        private string _stringKonekcije;

        public SPSpilDBKlasa(string noviStringKonekcije)
        {
            _stringKonekcije = noviStringKonekcije;
        }

        // SVI SPILOVI
        public DataSet DajSveSpilave()
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                try
                {
                    string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                                  ISNULL(NapomenaSudije, '') AS NapomenaSudije 
                                  FROM Spilovi ORDER BY DatumKreiranja DESC";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
                catch
                {
                    // Ako NapomenaSudije kolona ne postoji, koristi fallback
                    string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                                  '' AS NapomenaSudije 
                                  FROM Spilovi ORDER BY DatumKreiranja DESC";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
            }
            return ds;
        }

        // SPIL SA KARTAMA
        public DataSet DajSpilSaKartama(int spilID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                try
                {
                    string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                                  ISNULL(NapomenaSudije, '') AS NapomenaSudije 
                                  FROM Spilovi WHERE SpilID = @SpilID";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.AddWithValue("@SpilID", spilID);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
                catch
                {
                    // Ako NapomenaSudije kolona ne postoji, koristi fallback
                    string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                                  '' AS NapomenaSudije 
                                  FROM Spilovi WHERE SpilID = @SpilID";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.AddWithValue("@SpilID", spilID);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
            }
            return ds;
        }

        // SPILOVI TAKMI?ARA
        public DataSet DajSpiloveTakmicara(int takmicarID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                try
                {
                    string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                                  ISNULL(NapomenaSudije, '') AS NapomenaSudije 
                                  FROM Spilovi WHERE TakmicarID = @TakmicarID ORDER BY DatumKreiranja DESC";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.AddWithValue("@TakmicarID", takmicarID);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
                catch
                {
                    // Ako NapomenaSudije kolona ne postoji, koristi fallback
                    string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                                  '' AS NapomenaSudije 
                                  FROM Spilovi WHERE TakmicarID = @TakmicarID ORDER BY DatumKreiranja DESC";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.AddWithValue("@TakmicarID", takmicarID);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
            }
            return ds;
        }

        // SPILOVI NA ?EKANJU (za sudiju)
        public DataSet DajSpiloveNaCekanju()
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                try
                {
                    string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                                  ISNULL(NapomenaSudije, '') AS NapomenaSudije 
                                  FROM Spilovi WHERE Status = 'Na cekanju' ORDER BY DatumKreiranja DESC";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
                catch
                {
                    // Ako NapomenaSudije kolona ne postoji, koristi fallback
                    string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                                  '' AS NapomenaSudije 
                                  FROM Spilovi WHERE Status = 'Na cekanju' ORDER BY DatumKreiranja DESC";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
            }
            return ds;
        }

        // KREIRAJ SPIL
        public int KreirajSpil(SpilKlasa spil)
        {
            int noviID = 0;
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();
                    string sql = @"INSERT INTO Spilovi (Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja) 
                                   VALUES (@Naziv, @Format, @Arhetip, @TakmicarID, 'Na cekanju', GETDATE()); 
                                   SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.Add("@Naziv", SqlDbType.NVarChar).Value = spil.Naziv ?? "";
                    komanda.Parameters.Add("@Format", SqlDbType.NVarChar).Value = spil.Format ?? "";
                    komanda.Parameters.Add("@Arhetip", SqlDbType.NVarChar).Value = spil.Arhetip ?? "";
                    komanda.Parameters.Add("@TakmicarID", SqlDbType.Int).Value = spil.TakmicarID;

                    object result = komanda.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        noviID = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("KreirajSpil Error: " + ex.Message);
                return 0;
            }
            return noviID;
        }

        // DODAJ KARTU U SPIL
        public bool DodajKartuUSpil(int spilID, string nazivKarte, string sekcija, int kolicina)
        {
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                string sql = @"INSERT INTO KarteUSpilu (SpilID, NazivKarte, Sekcija, Kolicina) 
                              VALUES (@SpilID, @NazivKarte, @Sekcija, @Kolicina)";
                SqlCommand komanda = new SqlCommand(sql, konekcija);
                komanda.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                komanda.Parameters.Add("@NazivKarte", SqlDbType.NVarChar).Value = nazivKarte ?? "";
                komanda.Parameters.Add("@Sekcija", SqlDbType.NVarChar).Value = sekcija ?? "";
                komanda.Parameters.Add("@Kolicina", SqlDbType.Int).Value = kolicina;

                return komanda.ExecuteNonQuery() > 0;
            }
        }

        // DODAJ KARTU U SPIL (sa tipom karte)
        public bool DodajKartuUSpil(int spilID, string nazivKarte, string sekcija, int kolicina, string tipKarte)
        {
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                try
                {
                    // Pokusaj sa TipKarte kolonom
                    string sql = @"INSERT INTO KarteUSpilu (SpilID, NazivKarte, Sekcija, Kolicina, TipKarte) 
                                  VALUES (@SpilID, @NazivKarte, @Sekcija, @Kolicina, @TipKarte)";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                    komanda.Parameters.Add("@NazivKarte", SqlDbType.NVarChar).Value = nazivKarte ?? "";
                    komanda.Parameters.Add("@Sekcija", SqlDbType.NVarChar).Value = sekcija ?? "";
                    komanda.Parameters.Add("@Kolicina", SqlDbType.Int).Value = kolicina;
                    komanda.Parameters.Add("@TipKarte", SqlDbType.NVarChar).Value = tipKarte ?? "";

                    return komanda.ExecuteNonQuery() > 0;
                }
                catch
                {
                    // Ako TipKarte kolona ne postoji, koristi staru verziju bez tipa
                    string sql = @"INSERT INTO KarteUSpilu (SpilID, NazivKarte, Sekcija, Kolicina) 
                                  VALUES (@SpilID, @NazivKarte, @Sekcija, @Kolicina)";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                    komanda.Parameters.Add("@NazivKarte", SqlDbType.NVarChar).Value = nazivKarte ?? "";
                    komanda.Parameters.Add("@Sekcija", SqlDbType.NVarChar).Value = sekcija ?? "";
                    komanda.Parameters.Add("@Kolicina", SqlDbType.Int).Value = kolicina;

                    return komanda.ExecuteNonQuery() > 0;
                }
            }
        }

        // DOHVATI KARTE SPILA
        public DataSet DajKarteSpila(int spilID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                try
                {
                    // Pokusaj sa TipKarte kolonom
                    string sql = @"SELECT KartaUSpiluID, SpilID, NazivKarte, Sekcija, Kolicina, ISNULL(TipKarte, '') AS TipKarte 
                                  FROM KarteUSpilu WHERE SpilID = @SpilID ORDER BY Sekcija, NazivKarte";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.AddWithValue("@SpilID", spilID);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
                catch
                {
                    // Ako TipKarte kolona ne postoji, koristi staru verziju bez tipa
                    string sql = @"SELECT KartaUSpiluID, SpilID, NazivKarte, Sekcija, Kolicina 
                                  FROM KarteUSpilu WHERE SpilID = @SpilID ORDER BY Sekcija, NazivKarte";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.AddWithValue("@SpilID", spilID);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
            }
            return ds;
        }

        // AZURIRAJ KARTU
        public bool AzurirajKartu(int kartaUSpiluID, string nazivKarte, int kolicina, string tipKarte)
        {
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                try
                {
                    // Pokusaj sa TipKarte kolonom
                    string sql = @"UPDATE KarteUSpilu SET NazivKarte = @NazivKarte, Kolicina = @Kolicina, TipKarte = @TipKarte 
                                  WHERE KartaUSpiluID = @KartaUSpiluID";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.Add("@KartaUSpiluID", SqlDbType.Int).Value = kartaUSpiluID;
                    komanda.Parameters.Add("@NazivKarte", SqlDbType.NVarChar).Value = nazivKarte ?? "";
                    komanda.Parameters.Add("@Kolicina", SqlDbType.Int).Value = kolicina;
                    komanda.Parameters.Add("@TipKarte", SqlDbType.NVarChar).Value = tipKarte ?? "";

                    return komanda.ExecuteNonQuery() > 0;
                }
                catch
                {
                    // Ako TipKarte kolona ne postoji, koristi staru verziju bez tipa
                    string sql = @"UPDATE KarteUSpilu SET NazivKarte = @NazivKarte, Kolicina = @Kolicina 
                                  WHERE KartaUSpiluID = @KartaUSpiluID";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.Add("@KartaUSpiluID", SqlDbType.Int).Value = kartaUSpiluID;
                    komanda.Parameters.Add("@NazivKarte", SqlDbType.NVarChar).Value = nazivKarte ?? "";
                    komanda.Parameters.Add("@Kolicina", SqlDbType.Int).Value = kolicina;

                    return komanda.ExecuteNonQuery() > 0;
                }
            }
        }

        // PROMENI STATUS SPILA (Sudija)
        public bool PromeniStatusSpila(int spilID, string noviStatus, string napomena)
        {
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                string sql = @"UPDATE Spilovi SET Status = @NoviStatus, NapomenaSudije = @Napomena WHERE SpilID = @SpilID";
                SqlCommand komanda = new SqlCommand(sql, konekcija);
                komanda.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                komanda.Parameters.Add("@NoviStatus", SqlDbType.NVarChar).Value = noviStatus ?? "";
                komanda.Parameters.Add("@Napomena", SqlDbType.NVarChar).Value = napomena ?? "";

                return komanda.ExecuteNonQuery() > 0;
            }
        }

        // OBRISI SPIL
        public bool ObrisiSpil(int spilID)
        {
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                try
                {
                    // Obrisi karte prvo
                    string deletCardsql = "DELETE FROM KarteUSpilu WHERE SpilID = @SpilID";
                    SqlCommand deleteCardsCmd = new SqlCommand(deletCardsql, konekcija);
                    deleteCardsCmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                    deleteCardsCmd.ExecuteNonQuery();

                    // Obrisi spil
                    string deleteDecksql = "DELETE FROM Spilovi WHERE SpilID = @SpilID";
                    SqlCommand deleteDeckCmd = new SqlCommand(deleteDecksql, konekcija);
                    deleteDeckCmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                    return deleteDeckCmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
