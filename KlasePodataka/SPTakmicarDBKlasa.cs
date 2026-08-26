using System;
using System;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPTakmicarDBKlasa
    {
        private string _stringKonekcije;

        public SPTakmicarDBKlasa(string noviStringKonekcije)
        {
            _stringKonekcije = noviStringKonekcije;
        }

        // =========================
        // SVI TAKMIČARI
        // =========================
        public DataSet DajSveTakmicara()
        {
            DataSet ds = new DataSet();

            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();

                SqlCommand komanda = new SqlCommand("SELECT TakmicarID, Ime, Prezime, Email, BrojPobeda, DatumRegistracije FROM Takmicari ORDER BY Ime, Prezime", konekcija);
                komanda.CommandType = CommandType.Text;

                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                adapter.Fill(ds);
            }

            return ds;
        }

        // =========================
        // REGISTRACIJA TAKMIČARA
        // =========================
        public bool RegistrujTakmicara(TakmicarKlasa takmicar)
        {
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();

                SqlCommand komanda = new SqlCommand("dbo.RegistrujTakmicara", konekcija);
                komanda.CommandType = CommandType.StoredProcedure;

                komanda.Parameters.Add("@Ime", SqlDbType.NVarChar).Value = takmicar.Ime ?? "";
                komanda.Parameters.Add("@Prezime", SqlDbType.NVarChar).Value = takmicar.Prezime ?? "";
                komanda.Parameters.Add("@Email", SqlDbType.NVarChar).Value = takmicar.Email ?? "";
                komanda.Parameters.Add("@DatumRodjenja", SqlDbType.Date).Value = takmicar.DatumRodjenja;
                komanda.Parameters.Add("@Drzava", SqlDbType.NVarChar).Value = takmicar.Drzava ?? "";
                komanda.Parameters.Add("@Pol", SqlDbType.NVarChar).Value = takmicar.Pol ?? "";
                komanda.Parameters.Add("@Lozinka", SqlDbType.NVarChar).Value = takmicar.Lozinka ?? "";

                SqlParameter noviIDParam = komanda.Parameters.Add("@NoviID", SqlDbType.Int);
                noviIDParam.Direction = ParameterDirection.Output;

                try
                {
                    komanda.ExecuteNonQuery();

                    if (noviIDParam.Value != DBNull.Value && noviIDParam.Value != null)
                    {
                        int noviID = Convert.ToInt32(noviIDParam.Value);
                        takmicar.TakmicarID = noviID;

                        System.Diagnostics.Debug.WriteLine("RegistrujTakmicara: Takmičar registrovan sa ID: " + noviID);

                        return noviID > 0;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("RegistrujTakmicara: Output parameter je null - vjerovatno email vec postoji");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("RegistrujTakmicara Error: " + ex.Message + "\n" + ex.StackTrace);
                    return false;
                }
            }
        }

        // =========================
        // LOGIN TAKMIČARA
        // =========================
        public DataRow LoginTakmicar(string email, string lozinka)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] LoginTakmicar called - Email: {email}");
            DataTable dt = new DataTable();

            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();

                // Koristi direktni SQL umjesto stored procedure
                string query = "SELECT TakmicarID, Ime, Prezime, Email, DatumRodjenja, Drzava, Pol FROM Takmicari WHERE Email = @Email AND Lozinka = @Lozinka";
                SqlCommand komanda = new SqlCommand(query, konekcija);
                komanda.CommandType = CommandType.Text;

                komanda.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;
                komanda.Parameters.Add("@Lozinka", SqlDbType.NVarChar).Value = lozinka;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(dt);
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] LoginTakmicar query returned {dt.Rows.Count} rows");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] LoginTakmicar SQL Error: {ex.Message}");
                    throw;
                }
            }

            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            else
                return null;
        }

        // =========================
        // DOHVATI TAKMIČARA PO ID
        // =========================
        public DataRow DajTakmicaraPoID(int takmicarID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();

                SqlCommand komanda = new SqlCommand(
                    "SELECT TakmicarID, Ime, Prezime, Email, DatumRodjenja, Drzava, Pol FROM Takmicari WHERE TakmicarID = @TakmicarID",
                    konekcija);
                komanda.CommandType = CommandType.Text;

                komanda.Parameters.Add("@TakmicarID", SqlDbType.Int).Value = takmicarID;

                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                adapter.Fill(dt);
            }

            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            else
                return null;
        }
    }
}