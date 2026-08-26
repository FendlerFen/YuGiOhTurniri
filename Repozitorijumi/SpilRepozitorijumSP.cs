using System;
using System.Collections.Generic;
using System.Data;
using KlasePodataka;

namespace Repozitorijumi
{
    /// METODA 1: Rad sa Stored Procedures
    public class SpilRepozitorijumSP : ISpilRepozitorijum
    {
        private readonly string _konekcija;

        public SpilRepozitorijumSP(string konekcija)
        {
            _konekcija = konekcija;
        }

        public List<SpilKlasa> DajSveSpilave()
        {
            List<SpilKlasa> lista = new List<SpilKlasa>();
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            DataSet ds = db.DajSveSpilave();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                lista.Add(MapirajRed(r));
            }
            return lista;
        }

        public SpilKlasa DajPoID(int id)
        {
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            DataSet ds = db.DajSpilSaKartama(id);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            return MapirajRed(ds.Tables[0].Rows[0]);
        }

        public List<SpilKlasa> DajSpiloveTakmicara(int takmicarID)
        {
            List<SpilKlasa> lista = new List<SpilKlasa>();
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            DataSet ds = db.DajSpiloveTakmicara(takmicarID);

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                lista.Add(MapirajRed(r));
            }
            return lista;
        }

        public List<SpilKlasa> DajSpiloveNaCekanju()
        {
            List<SpilKlasa> lista = new List<SpilKlasa>();
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            DataSet ds = db.DajSpiloveNaCekanju();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                lista.Add(MapirajRed(r));
            }
            return lista;
        }

        public int Dodaj(SpilKlasa spil)
        {
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            return db.KreirajSpil(spil);
        }

        public bool Obrisi(int id)
        {
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            return db.ObrisiSpil(id);
        }

        public bool DodajKartu(int spilID, string nazivKarte, string sekcija, int kolicina)
        {
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            return db.DodajKartuUSpil(spilID, nazivKarte, sekcija, kolicina);
        }

        public bool DodajKartu(int spilID, string nazivKarte, string sekcija, int kolicina, string tipKarte)
        {
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            return db.DodajKartuUSpil(spilID, nazivKarte, sekcija, kolicina, tipKarte);
        }

        public List<KartaUSpiluKlasa> DajKarteSpila(int spilID)
        {
            List<KartaUSpiluKlasa> lista = new List<KartaUSpiluKlasa>();
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            DataSet ds = db.DajKarteSpila(spilID);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return lista;

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                lista.Add(new KartaUSpiluKlasa
                {
                    KartaUSpiluID = (int)r["KartaUSpiluID"],
                    SpilID = (int)r["SpilID"],
                    NazivKarte = r["NazivKarte"].ToString(),
                    Sekcija = r["Sekcija"].ToString(),
                    Kolicina = (byte)Convert.ToInt32(r["Kolicina"]),
                    TipKarte = r.Table.Columns.Contains("TipKarte") && r["TipKarte"] != DBNull.Value ? r["TipKarte"].ToString() : ""
                });
            }
            return lista;
        }

        public bool AzurirajKartu(int kartaUSpiluID, string nazivKarte, int kolicina, string tipKarte)
        {
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            return db.AzurirajKartu(kartaUSpiluID, nazivKarte, kolicina, tipKarte);
        }

        public bool PromeniStatus(int spilID, string noviStatus, string napomena)
        {
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            return db.PromeniStatusSpila(spilID, noviStatus, napomena);
        }

        public bool Izmeni(SpilKlasa spil)
        {
            SPSpilDBKlasa db = new SPSpilDBKlasa(_konekcija);
            using (System.Data.SqlClient.SqlConnection konekcija = new System.Data.SqlClient.SqlConnection(_konekcija))
            {
                konekcija.Open();
                string sql = @"UPDATE Spilovi SET Naziv = @Naziv, Format = @Format, Arhetip = @Arhetip 
                              WHERE SpilID = @SpilID";
                System.Data.SqlClient.SqlCommand komanda = new System.Data.SqlClient.SqlCommand(sql, konekcija);
                komanda.Parameters.Add("@SpilID", System.Data.SqlDbType.Int).Value = spil.SpilID;
                komanda.Parameters.Add("@Naziv", System.Data.SqlDbType.NVarChar).Value = spil.Naziv ?? "";
                komanda.Parameters.Add("@Format", System.Data.SqlDbType.NVarChar).Value = spil.Format ?? "";
                komanda.Parameters.Add("@Arhetip", System.Data.SqlDbType.NVarChar).Value = spil.Arhetip ?? "";

                return komanda.ExecuteNonQuery() > 0;
            }
        }

        private SpilKlasa MapirajRed(DataRow r)
        {
            return new SpilKlasa
            {
                SpilID = (int)r["SpilID"],
                Naziv = r["Naziv"].ToString(),
                Format = r["Format"].ToString(),
                Arhetip = r["Arhetip"] != DBNull.Value ? r["Arhetip"].ToString() : "",
                Status = r["Status"].ToString(),
                TakmicarID = (int)r["TakmicarID"],
                DatumKreiranja = (DateTime)r["DatumKreiranja"],
                NapomenaSudije = r.Table.Columns.Contains("NapomenaSudije") && r["NapomenaSudije"] != DBNull.Value ? r["NapomenaSudije"].ToString() : ""
            };
        }
    }
}