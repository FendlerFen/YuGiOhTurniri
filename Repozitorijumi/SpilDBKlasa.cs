using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using KlasePodataka;

namespace Repozitorijumi
{
    public class SpilDBKlasa : BazniRepozitorijum
    {
        public SpilDBKlasa(string noviStringKonekcije) : base(noviStringKonekcije)
        {
        }

        // SVI SPILOVI
        public DataSet DajSveSpilave()
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = DajKonekciju())
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

        public DataSet DajSpilSaKartama(int spilID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = DajKonekciju())
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

        public DataSet DajSpiloveTakmicara(int takmicarID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = DajKonekciju())
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

        public DataSet DajSpiloveNaCekanju()
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = DajKonekciju())
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

        public int KreirajSpil(SpilKlasa spil)
        {
            int noviID = 0;
            try
            {
                using (SqlConnection konekcija = DajKonekciju())
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

        public bool DodajKartuUSpil(int spilID, string nazivKarte, string sekcija, int kolicina)
        {
            using (SqlConnection konekcija = DajKonekciju())
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

        public bool DodajKartuUSpil(int spilID, string nazivKarte, string sekcija, int kolicina, string tipKarte)
        {
            using (SqlConnection konekcija = DajKonekciju())
            {
                konekcija.Open();
                try
                {
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

        public DataSet DajKarteSpila(int spilID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = DajKonekciju())
            {
                konekcija.Open();
                try
                {
                    string sql = @"SELECT KartaUSpiluID, SpilID, NazivKarte, Sekcija, Kolicina, ISNULL(TipKarte, '') AS TipKarte 
                                  FROM KarteUSpilu WHERE SpilID = @SpilID ORDER BY Sekcija, NazivKarte";
                    SqlCommand komanda = new SqlCommand(sql, konekcija);
                    komanda.Parameters.AddWithValue("@SpilID", spilID);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
                catch
                {
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

        public bool AzurirajKartu(int kartaUSpiluID, string nazivKarte, int kolicina, string tipKarte)
        {
            using (SqlConnection konekcija = DajKonekciju())
            {
                konekcija.Open();
                try
                {
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

        public bool PromeniStatusSpila(int spilID, string noviStatus, string napomena)
        {
            using (SqlConnection konekcija = DajKonekciju())
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

        public int KreirajSpilSaKartama(SpilKlasa spil, List<(string NazivKarte, string Sekcija, int Kolicina)> karte)
        {
            int noviID = 0;
            using (SqlConnection konekcija = DajKonekciju())
            {
                konekcija.Open();
                SqlTransaction transakcija = konekcija.BeginTransaction();
                try
                {
                    string sql = @"INSERT INTO Spilovi (Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja) 
                                   VALUES (@Naziv, @Format, @Arhetip, @TakmicarID, 'Na cekanju', GETDATE()); 
                                   SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    SqlCommand komanda = new SqlCommand(sql, konekcija, transakcija);
                    komanda.Parameters.Add("@Naziv", SqlDbType.NVarChar).Value = spil.Naziv ?? "";
                    komanda.Parameters.Add("@Format", SqlDbType.NVarChar).Value = spil.Format ?? "";
                    komanda.Parameters.Add("@Arhetip", SqlDbType.NVarChar).Value = spil.Arhetip ?? "";
                    komanda.Parameters.Add("@TakmicarID", SqlDbType.Int).Value = spil.TakmicarID;

                    object result = komanda.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        noviID = Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new Exception("Spil ID nije vra?en.");
                    }

                    foreach (var karta in karte)
                    {
                        string sqlKarta = @"INSERT INTO KarteUSpilu (SpilID, NazivKarte, Sekcija, Kolicina) 
                                           VALUES (@SpilID, @NazivKarte, @Sekcija, @Kolicina)";
                        SqlCommand komandaKarta = new SqlCommand(sqlKarta, konekcija, transakcija);
                        komandaKarta.Parameters.Add("@SpilID", SqlDbType.Int).Value = noviID;
                        komandaKarta.Parameters.Add("@NazivKarte", SqlDbType.NVarChar).Value = karta.NazivKarte ?? "";
                        komandaKarta.Parameters.Add("@Sekcija", SqlDbType.NVarChar).Value = karta.Sekcija ?? "";
                        komandaKarta.Parameters.Add("@Kolicina", SqlDbType.Int).Value = karta.Kolicina;

                        komandaKarta.ExecuteNonQuery();
                    }

                    transakcija.Commit();
                }
                catch (Exception ex)
                {
                    transakcija.Rollback();
                    System.Diagnostics.Debug.WriteLine("KreirajSpilSaKartama Error: " + ex.Message);
                    return 0;
                }
            }
            return noviID;
        }

        public bool ObrisiSpil(int spilID)
        {
            using (SqlConnection konekcija = DajKonekciju())
            {
                konekcija.Open();
                try
                {
                    string deleteCardsql = "DELETE FROM KarteUSpilu WHERE SpilID = @SpilID";
                    SqlCommand deleteCardsCmd = new SqlCommand(deleteCardsql, konekcija);
                    deleteCardsCmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                    deleteCardsCmd.ExecuteNonQuery();

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

        public bool IzmeniSpil(SpilKlasa spil)
        {
            using (SqlConnection konekcija = DajKonekciju())
            {
                konekcija.Open();
                string sql = @"UPDATE Spilovi SET Naziv = @Naziv, Format = @Format, Arhetip = @Arhetip 
                              WHERE SpilID = @SpilID";
                SqlCommand komanda = new SqlCommand(sql, konekcija);
                komanda.Parameters.Add("@SpilID", SqlDbType.Int).Value = spil.SpilID;
                komanda.Parameters.Add("@Naziv", SqlDbType.NVarChar).Value = spil.Naziv ?? "";
                komanda.Parameters.Add("@Format", SqlDbType.NVarChar).Value = spil.Format ?? "";
                komanda.Parameters.Add("@Arhetip", SqlDbType.NVarChar).Value = spil.Arhetip ?? "";

                return komanda.ExecuteNonQuery() > 0;
            }
        }
    }
}
