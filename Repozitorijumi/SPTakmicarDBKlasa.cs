using System;
using System.Data;
using System.Data.SqlClient;
using KlasePodataka;

namespace Repozitorijumi
{
    /// <summary>
    /// SP Takmi?ar DB Klasa - Samo StoredProcedure operacije
    /// Koristi samo StoredProcedure: dbo.RegistrujTakmicara
    /// </summary>
    public class SPTakmicarDBKlasa : BazniRepozitorijum
    {
        public SPTakmicarDBKlasa(string noviStringKonekcije) : base(noviStringKonekcije)
        {
        }

        // =========================
        // REGISTRACIJA TAKMICARA (StoredProcedure)
        // =========================
        public bool RegistrujTakmicara(TakmicarKlasa takmicar)
        {
            using (SqlConnection konekcija = DajKonekciju())
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

                        System.Diagnostics.Debug.WriteLine("RegistrujTakmicara: Takmicara registrovan sa ID: " + noviID);

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
    }
}
