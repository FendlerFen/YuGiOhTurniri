using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using KlasePodataka;

namespace Repozitorijumi
{

    /// BanLista repozitorijum koji koristi direktne SQL upite (DBUtils stil)
    public class BanListaRepozitorijumTabela : IBanListaRepository
    {
        private readonly string _konekcija;

        public BanListaRepozitorijumTabela(string konekcija)
        {
            _konekcija = konekcija;
        }

        public List<BanListaKlasa> DajSvuBanListu()
        {
            return DajBanListuSudije(0);
        }

        public List<BanListaKlasa> DajBanListuSudije(int sudijaID)
        {
            List<BanListaKlasa> lista = new List<BanListaKlasa>();

            using (SqlConnection konekcija = new SqlConnection(_konekcija))
            {
                konekcija.Open();

                string upit;
                if (sudijaID == 0)
                {
                    upit = "SELECT BanListaID, SudijaID, NazivKarte, DatumDodavanja FROM BanLista ORDER BY DatumDodavanja DESC";
                }
                else
                {
                    upit = "SELECT BanListaID, SudijaID, NazivKarte, DatumDodavanja FROM BanLista WHERE SudijaID = @SudijaID ORDER BY DatumDodavanja DESC";
                }

                SqlCommand komanda = new SqlCommand(upit, konekcija);
                if (sudijaID != 0)
                {
                    komanda.Parameters.AddWithValue("@SudijaID", sudijaID);
                }

                SqlDataReader reader = komanda.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new BanListaKlasa
                    {
                        BanListaID = Convert.ToInt32(reader["BanListaID"]),
                        SudijaID = Convert.ToInt32(reader["SudijaID"]),
                        NazivKarte = reader["NazivKarte"].ToString(),
                        DatumDodavanja = Convert.ToDateTime(reader["DatumDodavanja"])
                    });
                }

                reader.Close();
            }

            return lista;
        }

        public int DodajNaBanListu(int sudijaID, string nazivKarte)
        {
            int rezultat = 0;

            using (SqlConnection konekcija = new SqlConnection(_konekcija))
            {
                konekcija.Open();

                string upit = @"
                    INSERT INTO BanLista (SudijaID, NazivKarte, DatumDodavanja) 
                    VALUES (@SudijaID, @NazivKarte, @Datum);
                    SELECT SCOPE_IDENTITY();
                ";

                SqlCommand komanda = new SqlCommand(upit, konekcija);
                komanda.Parameters.AddWithValue("@SudijaID", sudijaID);
                komanda.Parameters.AddWithValue("@NazivKarte", nazivKarte);
                komanda.Parameters.AddWithValue("@Datum", DateTime.Now);

                rezultat = Convert.ToInt32(komanda.ExecuteScalar());
            }

            return rezultat;
        }

        public int ObrisiSaBanListe(int banListaID)
        {
            int rezultat = 0;

            using (SqlConnection konekcija = new SqlConnection(_konekcija))
            {
                konekcija.Open();

                string upit = "DELETE FROM BanLista WHERE BanListaID = @BanListaID";

                SqlCommand komanda = new SqlCommand(upit, konekcija);
                komanda.Parameters.AddWithValue("@BanListaID", banListaID);

                rezultat = komanda.ExecuteNonQuery();
            }

            return rezultat;
        }

        public bool DaLiJeKartaNaBanListi(int sudijaID, string nazivKarte)
        {
            using (SqlConnection konekcija = new SqlConnection(_konekcija))
            {
                konekcija.Open();

                string upit = "SELECT COUNT(*) FROM BanLista WHERE SudijaID = @SudijaID AND NazivKarte = @NazivKarte";

                SqlCommand komanda = new SqlCommand(upit, konekcija);
                komanda.Parameters.AddWithValue("@SudijaID", sudijaID);
                komanda.Parameters.AddWithValue("@NazivKarte", nazivKarte);

                int broj = (int)komanda.ExecuteScalar();
                return broj > 0;
            }
        }
    }
}
