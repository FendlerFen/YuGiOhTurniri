using System;
using System;
using System.Collections.Generic;
using System.Data;
using KlasePodataka;

namespace Repozitorijumi
{
    public class TakmicarRepozitorijumSP : ITakmicarRepozitorijum
    {
        private readonly string _konekcija;

        public TakmicarRepozitorijumSP(string konekcija)
        {
            _konekcija = konekcija;
        }

        public List<TakmicarKlasa> DajSveTakmicara()
        {
            var db = new SPTakmicarDBKlasa(_konekcija);
            var ds = db.DajSveTakmicara();
            return MapirajDataSet(ds);
        }

        public TakmicarKlasa DajPoID(int id)
        {
            var db = new SPTakmicarDBKlasa(_konekcija);
            var dr = db.DajTakmicaraPoID(id);

            if (dr == null)
                return null;

            return new TakmicarKlasa
            {
                TakmicarID = Convert.ToInt32(dr["TakmicarID"]),
                Ime = dr["Ime"].ToString(),
                Prezime = dr["Prezime"].ToString(),
                Email = dr["Email"].ToString(),
                DatumRodjenja = dr.Table.Columns.Contains("DatumRodjenja") ? Convert.ToDateTime(dr["DatumRodjenja"]) : DateTime.MinValue,
                Drzava = dr.Table.Columns.Contains("Drzava") ? dr["Drzava"].ToString() : "",
                Pol = dr.Table.Columns.Contains("Pol") ? dr["Pol"].ToString() : ""
            };
        }

        public int Dodaj(TakmicarKlasa takmicar)
        {
            var db = new SPTakmicarDBKlasa(_konekcija);
            bool uspeh = db.RegistrujTakmicara(takmicar);

            if (uspeh && takmicar.TakmicarID > 0)
                return takmicar.TakmicarID;
            else
                return 0;
        }

        public TakmicarKlasa Login(string email, string lozinka)
        {
            var db = new SPTakmicarDBKlasa(_konekcija);
            var dr = db.LoginTakmicar(email, lozinka);

            if (dr == null)
                return null;

            return new TakmicarKlasa
            {
                TakmicarID = Convert.ToInt32(dr["TakmicarID"]),
                Ime = dr["Ime"].ToString(),
                Prezime = dr["Prezime"].ToString(),
                Email = dr["Email"].ToString(),
                DatumRodjenja = dr.Table.Columns.Contains("DatumRodjenja") ? Convert.ToDateTime(dr["DatumRodjenja"]) : DateTime.MinValue,
                Drzava = dr.Table.Columns.Contains("Drzava") ? dr["Drzava"].ToString() : "",
                Pol = dr.Table.Columns.Contains("Pol") ? dr["Pol"].ToString() : ""
            };
        }

        private List<TakmicarKlasa> MapirajDataSet(DataSet ds)
        {
            List<TakmicarKlasa> lista = new List<TakmicarKlasa>();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                lista.Add(new TakmicarKlasa
                {
                    TakmicarID = Convert.ToInt32(r["TakmicarID"]),
                    Ime = r["Ime"].ToString(),
                    Prezime = r["Prezime"].ToString(),
                    Email = r["Email"].ToString(),
                    DatumRodjenja = Convert.ToDateTime(r["DatumRodjenja"]),
                    Drzava = r["Drzava"].ToString(),
                    Pol = r["Pol"].ToString()
                });
            }

            return lista;
        }
    }
}