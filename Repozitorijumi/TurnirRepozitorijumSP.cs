using System;
using System.Collections.Generic;
using System.Data;
using KlasePodataka;

namespace Repozitorijumi
{
    public class TurnirRepozitorijumSP : ITurnirRepozitorijum
    {
        private readonly string _konekcija;

        public TurnirRepozitorijumSP(string konekcija)
        {
            _konekcija = konekcija;
        }

        public List<TurnirKlasa> DajSveTurnire()
        {
            var db = new SPTurnirDBKlasa(_konekcija);
            var ds = db.DajSveTurnire();
            return MapirajDataSet(ds);
        }

        public TurnirKlasa DajPoID(int id)
        {
            var db = new SPTurnirDBKlasa(_konekcija);
            var row = db.DajTurnirPoID(id);

            if (row == null)
            {
                System.Diagnostics.Debug.WriteLine($"[TURNIR REPO] DajPoID({id}) - Nije pronađen!");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"[TURNIR REPO] DajPoID({id}) - Pronađen: {row["Naziv"]}");

            return new TurnirKlasa
            {
                TurnirID = Convert.ToInt32(row["TurnirID"]),
                Naziv = row["Naziv"].ToString(),
                Lokacija = row["Lokacija"].ToString(),
                Format = row["Format"].ToString(),
                DatumOdrzavanja = Convert.ToDateTime(row["DatumOdrzavanja"]),
                Status = row["Status"].ToString(),
                OrganizatorID = Convert.ToInt32(row["OrganizatorID"]),
                DatumKreiranja = Convert.ToDateTime(row["DatumKreiranja"])
            };
        }

        public List<TurnirKlasa> DajOtvoreneTurnire()
        {
            var db = new SPTurnirDBKlasa(_konekcija);
            var ds = db.DajOtvoreneTurnire();
            return MapirajDataSet(ds);
        }

        public List<TurnirKlasa> DajTurnireOrganizatora(int organizatorID)
        {
            var db = new SPTurnirDBKlasa(_konekcija);
            var ds = db.DajTurnireOrganizatora(organizatorID);
            return MapirajDataSet(ds);
        }

        public int Dodaj(TurnirKlasa turnir)
        {
            var db = new SPTurnirDBKlasa(_konekcija);
            return db.KreirajTurnir(turnir);
        }

        public bool Izmeni(TurnirKlasa turnir)
        {
            var db = new SPTurnirDBKlasa(_konekcija);
            return db.IzmeniTurnir(turnir);
        }

        public bool ZavrsiTurnir(int id)
        {
            var db = new SPTurnirDBKlasa(_konekcija);
            return db.ZavrsiTurnir(id);
        }

        public bool ProclasiPobjednike(int turnirID, int prvoMestoID, int drugoMestoID, int treceMestoID)
        {
            var db = new SPTurnirDBKlasa(_konekcija);
            return db.ProglasiPobednike(turnirID, prvoMestoID, drugoMestoID, treceMestoID);
        }

        public List<RezultatKlasa> DajRezultate(int turnirID)
        {
            var db = new SPTurnirDBKlasa(_konekcija);
            var ds = db.DajPobednike(turnirID);

            List<RezultatKlasa> rezultati = new List<RezultatKlasa>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    string ime = r["Ime"].ToString().Trim();
                    string prezime = r["Prezime"].ToString().Trim();

                    rezultati.Add(new RezultatKlasa
                    {
                        Mesto = Convert.ToInt32(r["Mesto"]),
                        Takmicari = (ime + " " + prezime).Trim(),
                        Ime = ime,
                        Prezime = prezime,
                        BrojPobeda = Convert.ToInt32(r["BrojPobeda"])
                    });
                }
            }

            return rezultati;
        }

        public List<RezultatKlasa> DajPobednike(int turnirID)
        {
            return DajRezultate(turnirID);
        }

        private List<TurnirKlasa> MapirajDataSet(DataSet ds)
        {
            List<TurnirKlasa> lista = new List<TurnirKlasa>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    lista.Add(new TurnirKlasa
                    {
                        TurnirID = Convert.ToInt32(r["TurnirID"]),
                        Naziv = r["Naziv"].ToString(),
                        Lokacija = r["Lokacija"].ToString(),
                        Format = r["Format"].ToString(),
                        DatumOdrzavanja = Convert.ToDateTime(r["DatumOdrzavanja"]),
                        Status = r["Status"].ToString(),
                        OrganizatorID = Convert.ToInt32(r["OrganizatorID"]),
                        DatumKreiranja = r.Table.Columns.Contains("DatumKreiranja") ? Convert.ToDateTime(r["DatumKreiranja"]) : DateTime.Now
                    });
                }
            }

            return lista;
        }
    }
}