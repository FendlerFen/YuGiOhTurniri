using System;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPOrganizatorDBKlasa
    {
        private string _konekcija;

        public SPOrganizatorDBKlasa(string konekcija)
        {
            _konekcija = konekcija;
        }

        public OrganizatorKlasa Login(string email, string lozinka)
        {
            using (SqlConnection conn = new SqlConnection(_konekcija))
            {
                try
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine($"[DB LOGIN] Tra?im organizatora: Email='{email}', Lozinka du?ina={lozinka?.Length ?? 0}");

                    string sql = "SELECT OrganizatorID, NazivOrganizacije, Ime, Prezime, Email, TelefonBroj, Drzava, Lozinka, DatumRegistracije FROM Organizatori WHERE Email = @email AND Lozinka = @lozinka";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@email", email ?? "");
                        cmd.Parameters.AddWithValue("@lozinka", lozinka ?? "");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine($"[DB LOGIN] PRONA?EN - ID={reader["OrganizatorID"]}, Email={reader["Email"]}");

                                return new OrganizatorKlasa
                                {
                                    OrganizatorID = (int)reader["OrganizatorID"],
                                    NazivOrganizacije = reader["NazivOrganizacije"].ToString(),
                                    Ime = reader["Ime"].ToString(),
                                    Prezime = reader["Prezime"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    TelefonBroj = reader["TelefonBroj"].ToString(),
                                    Drzava = reader["Drzava"].ToString(),
                                    Lozinka = reader["Lozinka"].ToString(),
                                    DatumRegistracije = (DateTime)reader["DatumRegistracije"]
                                };
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"[DB LOGIN] NIJE PRONA?EN - Nema rezultata za email='{email}'");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[DB LOGIN] EXCEPTION: {ex.Message}");
                    throw new Exception("Greska pri loggovanju organizatora: " + ex.Message);
                }
            }

            return null;
        }

        public int Registruj(OrganizatorKlasa organizator)
        {
            using (SqlConnection conn = new SqlConnection(_konekcija))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO Organizatori (NazivOrganizacije, Ime, Prezime, Email, TelefonBroj, Drzava, Lozinka, DatumRegistracije)
                                   VALUES (@naziv, @ime, @prezime, @email, @telefon, @drzava, @lozinka, @datum);
                                   SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@naziv", organizator.NazivOrganizacije);
                        cmd.Parameters.AddWithValue("@ime", organizator.Ime);
                        cmd.Parameters.AddWithValue("@prezime", organizator.Prezime);
                        cmd.Parameters.AddWithValue("@email", organizator.Email);
                        cmd.Parameters.AddWithValue("@telefon", organizator.TelefonBroj);
                        cmd.Parameters.AddWithValue("@drzava", organizator.Drzava);
                        cmd.Parameters.AddWithValue("@lozinka", organizator.Lozinka);
                        cmd.Parameters.AddWithValue("@datum", DateTime.Now);

                        object rezultat = cmd.ExecuteScalar();
                        if (rezultat != null && rezultat != DBNull.Value)
                        {
                            return Convert.ToInt32(rezultat);
                        }
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Greska pri registraciji organizatora: " + ex.Message);
                }
            }
        }

        public OrganizatorKlasa DajPoID(int organizatorID)
        {
            using (SqlConnection conn = new SqlConnection(_konekcija))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT OrganizatorID, NazivOrganizacije, Ime, Prezime, Email, TelefonBroj, Drzava, Lozinka, DatumRegistracije FROM Organizatori WHERE OrganizatorID = @id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", organizatorID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new OrganizatorKlasa
                                {
                                    OrganizatorID = (int)reader["OrganizatorID"],
                                    NazivOrganizacije = reader["NazivOrganizacije"].ToString(),
                                    Ime = reader["Ime"].ToString(),
                                    Prezime = reader["Prezime"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    TelefonBroj = reader["TelefonBroj"].ToString(),
                                    Drzava = reader["Drzava"].ToString(),
                                    Lozinka = reader["Lozinka"].ToString(),
                                    DatumRegistracije = (DateTime)reader["DatumRegistracije"]
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Greska pri preuzimanju organizatora: " + ex.Message);
                }
            }

            return null;
        }
    }
}
