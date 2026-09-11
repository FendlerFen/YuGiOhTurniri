using System;
using System.Data;
using KlasePodataka;

namespace KlaseMapiranja
{
    public class MaperTakmicar
    {
        public TakmicarKlasa Mapiraj(DataRow row)
        {
            TakmicarKlasa takmicar = new TakmicarKlasa();

            takmicar.TakmicarID = int.Parse(row["TakmicarID"].ToString());
            takmicar.Ime = row["Ime"].ToString();
            takmicar.Prezime = row["Prezime"].ToString();
            takmicar.Email = row["Email"].ToString();
            takmicar.BrojPobeda = row.Table.Columns.Contains("BrojPobeda") && row["BrojPobeda"] != DBNull.Value 
                ? int.Parse(row["BrojPobeda"].ToString()) 
                : 0;
            takmicar.DatumRegistracije = row.Table.Columns.Contains("DatumRegistracije") && row["DatumRegistracije"] != DBNull.Value 
                ? DateTime.Parse(row["DatumRegistracije"].ToString()) 
                : DateTime.Now;
            takmicar.DatumRodjenja = row.Table.Columns.Contains("DatumRodjenja") && row["DatumRodjenja"] != DBNull.Value 
                ? DateTime.Parse(row["DatumRodjenja"].ToString()) 
                : DateTime.MinValue;
            takmicar.Drzava = row.Table.Columns.Contains("Drzava") && row["Drzava"] != DBNull.Value 
                ? row["Drzava"].ToString() 
                : "";
            takmicar.Pol = row.Table.Columns.Contains("Pol") && row["Pol"] != DBNull.Value 
                ? row["Pol"].ToString() 
                : "";

            return takmicar;
        }
    }
}
