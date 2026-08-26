using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPBanListaDBKlasa
    {
        private string _konekcija;

        public SPBanListaDBKlasa(string konekcija)
        {
            _konekcija = konekcija;
        }

        public List<BanListaKlasa> DajBanListuSudije(int sudijaID)
        {
            List<BanListaKlasa> lista = new List<BanListaKlasa>();

            using (SqlConnection conn = new SqlConnection(_konekcija))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT BannedKartaID, NazivKarte, Razlog, DatumBana FROM BannedKarte ORDER BY DatumBana DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new BanListaKlasa
                                {
                                    BanListaID = (int)reader["BannedKartaID"],
                                    SudijaID = sudijaID, // Za kompatibilnost, koristimo prosle?eni ID
                                    NazivKarte = reader["NazivKarte"].ToString(),
                                    DatumDodavanja = (DateTime)reader["DatumBana"]
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Greska pri preuzimanju ban liste: " + ex.Message);
                }
            }

            return lista;
        }

        public int DodajNaBanListu(int sudijaID, string nazivKarte)
        {
            using (SqlConnection conn = new SqlConnection(_konekcija))
            {
                try
                {
                    conn.Open();
                    string sql = "INSERT INTO BannedKarte (NazivKarte, Razlog, DatumBana) VALUES (@naziv, @razlog, @datum)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@naziv", nazivKarte);
                        cmd.Parameters.AddWithValue("@razlog", "Dodala sudija");
                        cmd.Parameters.AddWithValue("@datum", DateTime.Now);

                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Greska pri dodavanju na ban listu: " + ex.Message);
                }
            }
        }

        public int ObrisiSaBanListe(int banListaID)
        {
            using (SqlConnection conn = new SqlConnection(_konekcija))
            {
                try
                {
                    conn.Open();
                    string sql = "DELETE FROM BannedKarte WHERE BannedKartaID = @id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", banListaID);

                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Greska pri brisanju sa ban liste: " + ex.Message);
                }
            }
        }

        public bool DaLiJeKartaNaBanListi(int sudijaID, string nazivKarte)
        {
            using (SqlConnection conn = new SqlConnection(_konekcija))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT COUNT(*) as cnt FROM BannedKarte WHERE NazivKarte = @naziv";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@naziv", nazivKarte);

                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Greska pri proveri ban liste: " + ex.Message);
                }
            }
        }
    }
}
