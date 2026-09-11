using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using KlasePodataka;

namespace Repozitorijumi
{
    /// METODA 2: Rad sa SQL komandama kroz BazniRepozitorijum
    public class SpilRepozitorijumSQL : BazniRepozitorijum, ISpilRepozitorijum
    {
        public SpilRepozitorijumSQL(string konekcija) : base(konekcija)
        {
        }

        public List<SpilKlasa> DajSveSpilave()
        {
            List<SpilKlasa> lista = new List<SpilKlasa>();
            try
            {
                string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                              ISNULL(NapomenaSudije, '') AS NapomenaSudije 
                              FROM Spilovi ORDER BY DatumKreiranja DESC";

                using (SqlDataReader citac = IzvrsiCitac(sql))
                {
                    while (citac.Read())
                    {
                        lista.Add(new SpilKlasa
                        {
                            SpilID = (int)citac["SpilID"],
                            Naziv = citac["Naziv"].ToString(),
                            Format = citac["Format"].ToString(),
                            Arhetip = citac["Arhetip"] != DBNull.Value ? citac["Arhetip"].ToString() : "",
                            Status = citac["Status"].ToString(),
                            TakmicarID = (int)citac["TakmicarID"],
                            DatumKreiranja = (DateTime)citac["DatumKreiranja"],
                            NapomenaSudije = citac["NapomenaSudije"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DajSveSpilave SQL Error: " + ex.Message);
            }
            return lista;
        }

        public SpilKlasa DajPoID(int id)
        {
            SpilKlasa spil = null;
            try
            {
                string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                              ISNULL(NapomenaSudije, '') AS NapomenaSudije 
                              FROM Spilovi WHERE SpilID = @SpilID";

                using (SqlDataReader citac = IzvrsiCitac(sql, null, cmd => 
                    cmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = id))
                {
                    if (citac.Read())
                    {
                        spil = new SpilKlasa
                        {
                            SpilID = (int)citac["SpilID"],
                            Naziv = citac["Naziv"].ToString(),
                            Format = citac["Format"].ToString(),
                            Arhetip = citac["Arhetip"] != DBNull.Value ? citac["Arhetip"].ToString() : "",
                            Status = citac["Status"].ToString(),
                            TakmicarID = (int)citac["TakmicarID"],
                            DatumKreiranja = (DateTime)citac["DatumKreiranja"],
                            NapomenaSudije = citac["NapomenaSudije"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DajPoID SQL Error: " + ex.Message);
            }
            return spil;
        }

        public List<SpilKlasa> DajSpiloveTakmicara(int takmicarID)
        {
            List<SpilKlasa> lista = new List<SpilKlasa>();
            try
            {
                string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                              ISNULL(NapomenaSudije, '') AS NapomenaSudije 
                              FROM Spilovi WHERE TakmicarID = @TakmicarID ORDER BY DatumKreiranja DESC";

                using (SqlDataReader citac = IzvrsiCitac(sql, null, cmd => 
                    cmd.Parameters.Add("@TakmicarID", SqlDbType.Int).Value = takmicarID))
                {
                    while (citac.Read())
                    {
                        lista.Add(new SpilKlasa
                        {
                            SpilID = (int)citac["SpilID"],
                            Naziv = citac["Naziv"].ToString(),
                            Format = citac["Format"].ToString(),
                            Arhetip = citac["Arhetip"] != DBNull.Value ? citac["Arhetip"].ToString() : "",
                            Status = citac["Status"].ToString(),
                            TakmicarID = (int)citac["TakmicarID"],
                            DatumKreiranja = (DateTime)citac["DatumKreiranja"],
                            NapomenaSudije = citac["NapomenaSudije"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DajSpiloveTakmicara SQL Error: " + ex.Message);
            }
            return lista;
        }

        public List<SpilKlasa> DajSpiloveNaCekanju()
        {
            List<SpilKlasa> lista = new List<SpilKlasa>();
            try
            {
                string sql = @"SELECT SpilID, Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja, 
                              ISNULL(NapomenaSudije, '') AS NapomenaSudije 
                              FROM Spilovi WHERE Status = 'Na cekanju' ORDER BY DatumKreiranja DESC";

                using (SqlDataReader citac = IzvrsiCitac(sql))
                {
                    while (citac.Read())
                    {
                        lista.Add(new SpilKlasa
                        {
                            SpilID = (int)citac["SpilID"],
                            Naziv = citac["Naziv"].ToString(),
                            Format = citac["Format"].ToString(),
                            Arhetip = citac["Arhetip"] != DBNull.Value ? citac["Arhetip"].ToString() : "",
                            Status = citac["Status"].ToString(),
                            TakmicarID = (int)citac["TakmicarID"],
                            DatumKreiranja = (DateTime)citac["DatumKreiranja"],
                            NapomenaSudije = citac["NapomenaSudije"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DajSpiloveNaCekanju SQL Error: " + ex.Message);
            }
            return lista;
        }

        public int Dodaj(SpilKlasa spil)
        {
            try
            {
                string sql = @"INSERT INTO Spilovi (Naziv, Format, Arhetip, TakmicarID, Status, DatumKreiranja) 
                              VALUES (@Naziv, @Format, @Arhetip, @TakmicarID, 'Na cekanju', GETDATE()); 
                              SELECT CAST(SCOPE_IDENTITY() AS INT);";

                object rezultat = IzvrsiSkalar(sql, null, cmd =>
                {
                    cmd.Parameters.Add("@Naziv", SqlDbType.NVarChar).Value = spil.Naziv ?? "";
                    cmd.Parameters.Add("@Format", SqlDbType.NVarChar).Value = spil.Format ?? "";
                    cmd.Parameters.Add("@Arhetip", SqlDbType.NVarChar).Value = spil.Arhetip ?? "";
                    cmd.Parameters.Add("@TakmicarID", SqlDbType.Int).Value = spil.TakmicarID;
                });

                return rezultat != null && rezultat != DBNull.Value ? Convert.ToInt32(rezultat) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dodaj SQL Error: " + ex.Message);
                return 0;
            }
        }

        public bool Obrisi(int id)
        {
            try
            {
                string sql = "DELETE FROM Spilovi WHERE SpilID = @SpilID";
                int rezultat = IzvrsiUpit(sql, null, cmd => 
                    cmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = id);
                return rezultat > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Obrisi SQL Error: " + ex.Message);
                return false;
            }
        }

        public bool DodajKartu(int spilID, string nazivKarte, string sekcija, int kolicina)
        {
            try
            {
                string sql = @"INSERT INTO KarteUSpilu (SpilID, NazivKarte, Sekcija, Kolicina) 
                              VALUES (@SpilID, @NazivKarte, @Sekcija, @Kolicina)";

                int rezultat = IzvrsiUpit(sql, null, cmd =>
                {
                    cmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                    cmd.Parameters.Add("@NazivKarte", SqlDbType.NVarChar).Value = nazivKarte ?? "";
                    cmd.Parameters.Add("@Sekcija", SqlDbType.NVarChar).Value = sekcija ?? "";
                    cmd.Parameters.Add("@Kolicina", SqlDbType.Int).Value = kolicina;
                });

                return rezultat > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DodajKartu SQL Error: " + ex.Message);
                return false;
            }
        }

        public bool DodajKartu(int spilID, string nazivKarte, string sekcija, int kolicina, string tipKarte)
        {
            try
            {
                string sql = @"INSERT INTO KarteUSpilu (SpilID, NazivKarte, Sekcija, Kolicina, TipKarte) 
                              VALUES (@SpilID, @NazivKarte, @Sekcija, @Kolicina, @TipKarte)";

                int rezultat = IzvrsiUpit(sql, null, cmd =>
                {
                    cmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                    cmd.Parameters.Add("@NazivKarte", SqlDbType.NVarChar).Value = nazivKarte ?? "";
                    cmd.Parameters.Add("@Sekcija", SqlDbType.NVarChar).Value = sekcija ?? "";
                    cmd.Parameters.Add("@Kolicina", SqlDbType.Int).Value = kolicina;
                    cmd.Parameters.Add("@TipKarte", SqlDbType.NVarChar).Value = tipKarte ?? "";
                });

                return rezultat > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DodajKartu sa tipom SQL Error: " + ex.Message);
                return false;
            }
        }

        public List<KartaUSpiluKlasa> DajKarteSpila(int spilID)
        {
            List<KartaUSpiluKlasa> lista = new List<KartaUSpiluKlasa>();
            try
            {
                string sql = @"SELECT KartaUSpiluID, SpilID, NazivKarte, Sekcija, Kolicina, ISNULL(TipKarte, '') AS TipKarte 
                              FROM KarteUSpilu WHERE SpilID = @SpilID ORDER BY Sekcija, NazivKarte";

                using (SqlDataReader citac = IzvrsiCitac(sql, null, cmd => 
                    cmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID))
                {
                    while (citac.Read())
                    {
                        lista.Add(new KartaUSpiluKlasa
                        {
                            KartaUSpiluID = (int)citac["KartaUSpiluID"],
                            SpilID = (int)citac["SpilID"],
                            NazivKarte = citac["NazivKarte"].ToString(),
                            Sekcija = citac["Sekcija"].ToString(),
                            Kolicina = (byte)Convert.ToInt32(citac["Kolicina"]),
                            TipKarte = citac["TipKarte"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DajKarteSpila SQL Error: " + ex.Message);
            }
            return lista;
        }

        public bool AzurirajKartu(int kartaUSpiluID, string nazivKarte, int kolicina, string tipKarte)
        {
            try
            {
                string sql = @"UPDATE KarteUSpilu SET NazivKarte = @NazivKarte, Kolicina = @Kolicina, TipKarte = @TipKarte 
                              WHERE KartaUSpiluID = @KartaUSpiluID";

                int rezultat = IzvrsiUpit(sql, null, cmd =>
                {
                    cmd.Parameters.Add("@KartaUSpiluID", SqlDbType.Int).Value = kartaUSpiluID;
                    cmd.Parameters.Add("@NazivKarte", SqlDbType.NVarChar).Value = nazivKarte ?? "";
                    cmd.Parameters.Add("@Kolicina", SqlDbType.Int).Value = kolicina;
                    cmd.Parameters.Add("@TipKarte", SqlDbType.NVarChar).Value = tipKarte ?? "";
                });

                return rezultat > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AzurirajKartu SQL Error: " + ex.Message);
                return false;
            }
        }

        public bool PromeniStatus(int spilID, string noviStatus, string napomena)
        {
            try
            {
                string sql = @"UPDATE Spilovi SET Status = @NoviStatus, NapomenaSudije = @Napomena WHERE SpilID = @SpilID";

                int rezultat = IzvrsiUpit(sql, null, cmd =>
                {
                    cmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = spilID;
                    cmd.Parameters.Add("@NoviStatus", SqlDbType.NVarChar).Value = noviStatus ?? "";
                    cmd.Parameters.Add("@Napomena", SqlDbType.NVarChar).Value = napomena ?? "";
                });

                return rezultat > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PromeniStatus SQL Error: " + ex.Message);
                return false;
            }
        }

        public bool Izmeni(SpilKlasa spil)
        {
            try
            {
                string sql = @"UPDATE Spilovi SET Naziv = @Naziv, Format = @Format, Arhetip = @Arhetip 
                              WHERE SpilID = @SpilID";

                int rezultat = IzvrsiUpit(sql, null, cmd =>
                {
                    cmd.Parameters.Add("@SpilID", SqlDbType.Int).Value = spil.SpilID;
                    cmd.Parameters.Add("@Naziv", SqlDbType.NVarChar).Value = spil.Naziv ?? "";
                    cmd.Parameters.Add("@Format", SqlDbType.NVarChar).Value = spil.Format ?? "";
                    cmd.Parameters.Add("@Arhetip", SqlDbType.NVarChar).Value = spil.Arhetip ?? "";
                });

                return rezultat > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Izmeni SQL Error: " + ex.Message);
                return false;
            }
        }
    }
}
