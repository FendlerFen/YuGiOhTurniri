using System;
using System.Data;
using System.Data.SqlClient;
using KlasePodataka;

namespace Repozitorijumi
{
    /// <summary>
    /// Takmi?ar DB Klasa - Direktan SQL pristup
    /// Sadr?i sve SQL operacije za takmi?are (bez StoredProcedure)
    /// </summary>
    public class TakmicarDBKlasa : BazniRepozitorijum
    {
        public TakmicarDBKlasa(string noviStringKonekcije) : base(noviStringKonekcije)
        {
        }

        // =========================
        // SVI TAKMICARI
        // =========================
        public DataSet DajSveTakmicara()
        {
            DataSet ds = new DataSet();

            using (SqlConnection konekcija = DajKonekciju())
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
        // LOGIN TAKMICARA
        // =========================
        public DataRow LoginTakmicar(string email, string lozinka)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] LoginTakmicar called - Email: {email}");
            DataTable dt = new DataTable();

            using (SqlConnection konekcija = DajKonekciju())
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
        // DOHVATI TAKMICARA PO ID
        // =========================
        public DataRow DajTakmicaraPoID(int takmicarID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection konekcija = DajKonekciju())
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
