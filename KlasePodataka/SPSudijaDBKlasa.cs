using System;
using System;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPSudijaDBKlasa
    {
        private string _stringKonekcije;

        public SPSudijaDBKlasa(string noviStringKonekcije)
        {
            _stringKonekcije = noviStringKonekcije;
        }

        // PRIJAVA SUDIJE - Koristi bazu umesto hardkoda
        public int PrijavaS(string email, string lozinka)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();

                    // Prvo poku?aj sa email i lozinkom (novi pristup)
                    string sql = "SELECT SudijaID FROM Sudije WHERE Email = @Email AND Lozinka = @Lozinka";
                    using (SqlCommand komanda = new SqlCommand(sql, konekcija))
                    {
                        komanda.Parameters.AddWithValue("@Email", email);
                        komanda.Parameters.AddWithValue("@Lozinka", lozinka);

                        object result = komanda.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Gre?ka pri prijavi
            }

            // Ako ne na?e sa email, vrati 0 (neuspe?na prijava)
            return 0;
        }

        // Legacy support za stare hardkodirane sudije
        public SudijaKlasa GetSudijaByID(int sudijaID)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();
                    string sql = "SELECT SudijaID, Ime, Prezime, Email, Lozinka, DatumRegistracije FROM Sudije WHERE SudijaID = @ID";

                    using (SqlCommand komanda = new SqlCommand(sql, konekcija))
                    {
                        komanda.Parameters.AddWithValue("@ID", sudijaID);

                        using (SqlDataReader reader = komanda.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SudijaKlasa
                                {
                                    SudijaID = (int)reader["SudijaID"],
                                    Ime = reader["Ime"].ToString(),
                                    Prezime = reader["Prezime"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Lozinka = reader["Lozinka"].ToString(),
                                    DatumRegistracije = (DateTime)reader["DatumRegistracije"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        // DOHVATI SUDIJU PO EMAILU
        public SudijaKlasa GetSudijaByEmail(string email)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();
                    string sql = "SELECT SudijaID, Ime, Prezime, Email, Lozinka, DatumRegistracije FROM Sudije WHERE Email = @Email";

                    using (SqlCommand komanda = new SqlCommand(sql, konekcija))
                    {
                        komanda.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader reader = komanda.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SudijaKlasa
                                {
                                    SudijaID = (int)reader["SudijaID"],
                                    Ime = reader["Ime"].ToString(),
                                    Prezime = reader["Prezime"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Lozinka = reader["Lozinka"].ToString(),
                                    DatumRegistracije = (DateTime)reader["DatumRegistracije"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SPSudijaDBKlasa.GetSudijaByEmail Error: " + ex.Message);
            }

            return null;
        }

        // REGISTRACIJA SUDIJE
        public bool RegistrujSudiju(string ime, string prezime, string email, string lozinka)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();

                    string sql = @"INSERT INTO Sudije (Ime, Prezime, Email, Lozinka, DatumRegistracije) 
                                   VALUES (@Ime, @Prezime, @Email, @Lozinka, GETDATE())";

                    using (SqlCommand komanda = new SqlCommand(sql, konekcija))
                    {
                        komanda.Parameters.AddWithValue("@Ime", ime ?? "");
                        komanda.Parameters.AddWithValue("@Prezime", prezime ?? "");
                        komanda.Parameters.AddWithValue("@Email", email ?? "");
                        komanda.Parameters.AddWithValue("@Lozinka", lozinka ?? "");

                        int rezultat = komanda.ExecuteNonQuery();
                        return rezultat > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // DOHVATI SVE SUDIJE
        public DataSet DajSveSudije()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();
                    string sql = "SELECT SudijaID, Ime, Prezime, Email, Lozinka, DatumRegistracije FROM Sudije ORDER BY Ime, Prezime";

                    using (SqlCommand komanda = new SqlCommand(sql, konekcija))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                        adapter.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                // Gre?ka pri u?itavanju sudija
            }

            return ds;
        }

        // OBRISI SUDIJU
        public bool ObrisiSudiju(int sudijaID)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();
                    string sql = "DELETE FROM Sudije WHERE SudijaID = @ID";

                    using (SqlCommand komanda = new SqlCommand(sql, konekcija))
                    {
                        komanda.Parameters.AddWithValue("@ID", sudijaID);
                        int rezultat = komanda.ExecuteNonQuery();
                        return rezultat > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
