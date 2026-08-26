using System;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPTurnirDBKlasa
    {
        private readonly string _stringKonekcije;

        public SPTurnirDBKlasa(string noviStringKonekcije)
        {
            _stringKonekcije = noviStringKonekcije;
        }

        // SVI TURNIRI
        public DataSet DajSveTurnire()
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                SqlCommand komanda = new SqlCommand(
                    "SELECT * FROM Turniri ORDER BY DatumOdrzavanja DESC", konekcija);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                adapter.Fill(ds);
            }
            return ds;
        }

        // TURNIRI ORGANIZATORA
        public DataSet DajTurnireOrganizatora(int organizatorID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                SqlCommand komanda = new SqlCommand(
                    "SELECT * FROM Turniri WHERE OrganizatorID = @ID ORDER BY DatumOdrzavanja DESC", konekcija);
                komanda.Parameters.AddWithValue("@ID", organizatorID);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                adapter.Fill(ds);
            }
            return ds;
        }

        // OTVORENI TURNIRI (za prijave)
        public DataSet DajOtvoreneTurnire()
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                SqlCommand komanda = new SqlCommand(
                    "SELECT * FROM Turniri WHERE Status = 'Otvoren' ORDER BY DatumOdrzavanja", konekcija);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                adapter.Fill(ds);
            }
            return ds;
        }

        // KREIRAJ TURNIR
        public int KreirajTurnir(TurnirKlasa turnir)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();

                    // Prvo proveri da li je organizator validan
                    System.Diagnostics.Debug.WriteLine($"KreirajTurnir SQL: Naziv={turnir.Naziv}, OrganizatorID={turnir.OrganizatorID}, DatumOdrzavanja={turnir.DatumOdrzavanja}");

                    SqlCommand komanda = new SqlCommand(
                        "INSERT INTO Turniri (Naziv, Lokacija, Format, DatumOdrzavanja, OrganizatorID, Status, DatumKreiranja) " +
                        "VALUES (@naziv, @lokacija, @format, @datum, @organizatorID, 'Otvoren', @datumKreiranja); " +
                        "SELECT CAST(SCOPE_IDENTITY() AS INT);", konekcija);

                    komanda.Parameters.AddWithValue("@naziv", turnir.Naziv ?? "");
                    komanda.Parameters.AddWithValue("@lokacija", turnir.Lokacija ?? "");
                    komanda.Parameters.AddWithValue("@format", turnir.Format ?? "");
                    komanda.Parameters.AddWithValue("@datum", turnir.DatumOdrzavanja);
                    komanda.Parameters.AddWithValue("@organizatorID", turnir.OrganizatorID);
                    komanda.Parameters.AddWithValue("@datumKreiranja", turnir.DatumKreiranja);

                    object rezultat = komanda.ExecuteScalar();
                    System.Diagnostics.Debug.WriteLine($"KreirajTurnir SQL Result: {rezultat}");

                    if (rezultat != null && rezultat != DBNull.Value)
                    {
                        return Convert.ToInt32(rezultat);
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("KreirajTurnir Error: " + ex.Message + "\n" + ex.StackTrace);
                return 0;
            }
        }

        // DOHVATI TURNIR
        public DataRow DajTurnirPoID(int turnirID)
        {
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                SqlCommand komanda = new SqlCommand(
                    "SELECT * FROM Turniri WHERE TurnirID = @ID", konekcija);
                komanda.Parameters.AddWithValue("@ID", turnirID);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                    return ds.Tables[0].Rows[0];
                return null;
            }
        }

        // PROGLASI POBEDNIKE
        public bool ProglasiPobednike(int turnirID, int prvoMestoID, int drugoMestoID, int treceMestoID)
        {
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                SqlCommand komanda = new SqlCommand(
                    "DELETE FROM Rezultati WHERE TurnirID = @turnirID; " +
                    "INSERT INTO Rezultati (TurnirID, TakmicarID, Mesto) VALUES (@turnirID, @prvi, 1); " +
                    "INSERT INTO Rezultati (TurnirID, TakmicarID, Mesto) VALUES (@turnirID, @drugi, 2); " +
                    "INSERT INTO Rezultati (TurnirID, TakmicarID, Mesto) VALUES (@turnirID, @treci, 3); " +
                    "UPDATE Takmicari SET BrojPobeda = BrojPobeda + 3 WHERE TakmicarID = @prvi; " +
                    "UPDATE Takmicari SET BrojPobeda = BrojPobeda + 2 WHERE TakmicarID = @drugi; " +
                    "UPDATE Takmicari SET BrojPobeda = BrojPobeda + 1 WHERE TakmicarID = @treci;", konekcija);

                komanda.Parameters.AddWithValue("@turnirID", turnirID);
                komanda.Parameters.AddWithValue("@prvi", prvoMestoID);
                komanda.Parameters.AddWithValue("@drugi", drugoMestoID);
                komanda.Parameters.AddWithValue("@treci", treceMestoID);

                return komanda.ExecuteNonQuery() > 0;
            }
        }

        // DOHVATI POBEDNIKE
        public DataSet DajPobednike(int turnirID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                SqlCommand komanda = new SqlCommand(
                    "SELECT r.Mesto, t.Ime, t.Prezime, t.BrojPobeda FROM Rezultati r " +
                    "INNER JOIN Takmicari t ON r.TakmicarID = t.TakmicarID " +
                    "WHERE r.TurnirID = @ID ORDER BY r.Mesto", konekcija);
                komanda.Parameters.AddWithValue("@ID", turnirID);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                adapter.Fill(ds);
            }
            return ds;
        }

        // ZAVRŠI TURNIR
        public bool ZavrsiTurnir(int turnirID)
        {
            using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();
                SqlCommand komanda = new SqlCommand(
                    "UPDATE Turniri SET Status = 'Zavrsen' WHERE TurnirID = @ID", konekcija);
                komanda.Parameters.AddWithValue("@ID", turnirID);
                return komanda.ExecuteNonQuery() > 0;
            }
        }

        // DODAJ TAKMIČARA NA TURNIR
        public bool DodajTakmicaraTurniru(int turnirID, int takmicarID, int spilID)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();

                    // Proveri da li je takmičar već registrovan na turniru
                    SqlCommand provera = new SqlCommand(
                        "SELECT COUNT(*) FROM Prijave WHERE TurnirID = @turnirID AND TakmicarID = @takmicarID",
                        konekcija);
                    provera.Parameters.AddWithValue("@turnirID", turnirID);
                    provera.Parameters.AddWithValue("@takmicarID", takmicarID);

                    int brojPrijava = (int)provera.ExecuteScalar();
                    if (brojPrijava > 0)
                    {
                        return false; 
                    }

                    // Proveri broj takmičara na turniru
                    SqlCommand brojTakmicara = new SqlCommand(
                        "SELECT COUNT(*) FROM Prijave WHERE TurnirID = @turnirID",
                        konekcija);
                    brojTakmicara.Parameters.AddWithValue("@turnirID", turnirID);
                    int trenutanBroj = (int)brojTakmicara.ExecuteScalar();

                    if (trenutanBroj >= 999)
                    {
                        throw new Exception("Turnir je dostignuo maksimalnu kapacitet od 999 takmičara! Ne možete da se prijavite.");
                    }

                    // Dodaj u tabelu Prijave
                    SqlCommand komanda = new SqlCommand(
                        "INSERT INTO Prijave (TurnirID, TakmicarID, SpilID, DatumPrijave, Status) " +
                        "VALUES (@turnirID, @takmicarID, @spilID, @datum, 'Aktivna')",
                        konekcija);

                    komanda.Parameters.AddWithValue("@turnirID", turnirID);
                    komanda.Parameters.AddWithValue("@takmicarID", takmicarID);
                    komanda.Parameters.AddWithValue("@spilID", spilID);
                    komanda.Parameters.AddWithValue("@datum", DateTime.Now);

                    return komanda.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DodajTakmicaraTurniru Error: " + ex.Message);
                return false;
            }
        }

        // IZMENI TURNIR
        public bool IzmeniTurnir(TurnirKlasa turnir)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();
                    SqlCommand komanda = new SqlCommand(
                        "UPDATE Turniri SET Naziv = @naziv, Lokacija = @lokacija, Format = @format, DatumOdrzavanja = @datum " +
                        "WHERE TurnirID = @id", konekcija);

                    komanda.Parameters.AddWithValue("@naziv", turnir.Naziv ?? "");
                    komanda.Parameters.AddWithValue("@lokacija", turnir.Lokacija ?? "");
                    komanda.Parameters.AddWithValue("@format", turnir.Format ?? "");
                    komanda.Parameters.AddWithValue("@datum", turnir.DatumOdrzavanja);
                    komanda.Parameters.AddWithValue("@id", turnir.TurnirID);

                    return komanda.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("IzmeniTurnir Error: " + ex.Message);
                return false;
            }
        }

        // OBRISI TURNIR
        public bool ObrisiTurnir(int turnirID)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();

                    // Prvo obriši sve prijave za ovaj turnir
                    SqlCommand obrisiPrijave = new SqlCommand(
                        "DELETE FROM Prijave WHERE TurnirID = @id", konekcija);
                    obrisiPrijave.Parameters.AddWithValue("@id", turnirID);
                    obrisiPrijave.ExecuteNonQuery();

                    // Obriši sve rezultate za ovaj turnir
                    SqlCommand obrisiRezultate = new SqlCommand(
                        "DELETE FROM Rezultati WHERE TurnirID = @id", konekcija);
                    obrisiRezultate.Parameters.AddWithValue("@id", turnirID);
                    obrisiRezultate.ExecuteNonQuery();

                    // Obriši turnir
                    SqlCommand obrisiTurnir = new SqlCommand(
                        "DELETE FROM Turniri WHERE TurnirID = @id", konekcija);
                    obrisiTurnir.Parameters.AddWithValue("@id", turnirID);

                    return obrisiTurnir.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ObrisiTurnir Error: " + ex.Message);
                return false;
            }
        }

        // DOHVATI TAKMIČARE NA TURNIRU
        public DataSet DajTakmicareNaTurniru(int turnirID)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();
                    SqlCommand komanda = new SqlCommand(
                        "SELECT DISTINCT t.TakmicarID, t.Ime, t.Prezime, s.Naziv AS SpilNaziv " +
                        "FROM Prijave p " +
                        "INNER JOIN Takmicari t ON p.TakmicarID = t.TakmicarID " +
                        "INNER JOIN Spilovi s ON p.SpilID = s.SpilID " +
                        "WHERE p.TurnirID = @turnirID " +
                        "ORDER BY t.Ime, t.Prezime", konekcija);

                    komanda.Parameters.AddWithValue("@turnirID", turnirID);
                    SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                    adapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DajTakmicareNaTurniru Error: " + ex.Message);
            }
            return ds;
        }

        // OBRISI PRIJAVE NA TURNIR (pri izmeni turnira)
        public bool ObrisiBrisanjePrijava(int turnirID)
        {
            try
            {
                using (SqlConnection konekcija = new SqlConnection(_stringKonekcije))
                {
                    konekcija.Open();
                    SqlCommand komanda = new SqlCommand(
                        "DELETE FROM Prijave WHERE TurnirID = @id", konekcija);
                    komanda.Parameters.AddWithValue("@id", turnirID);
                    return komanda.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ObrisiBrisanjePrijava Error: " + ex.Message);
                return false;
            }
        }
    }
}