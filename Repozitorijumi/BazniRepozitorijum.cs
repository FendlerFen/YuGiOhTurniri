using System;
using System.Data;
using System.Data.SqlClient;

namespace Repozitorijumi
{

    // Bazna tehnolo?ka klasa za sve repozitorijume
    // Sadrzi zajedni?ke metode za pristup bazi podataka
    public abstract class BazniRepozitorijum
    {

        // Konekcioni string za povezivanje sa bazom

        protected string _konekcija;


        // Konstruktor bazne klase

        /// <param name="konekcija">Konekcioni string</param>
        public BazniRepozitorijum(string konekcija)
        {
            _konekcija = konekcija;
        }


        // Kreira novu konekciju sa bazom podataka
        // <returns>SqlConnection objekat</returns>
        protected SqlConnection DajKonekciju()
        {
            return new SqlConnection(_konekcija);
        }


        // Izvr?ava SQL komandu bez vra?anja rezultata (INSERT, UPDATE, DELETE)

        /// <param name="sql">SQL naredba</param>
        /// <param name="transakcija">SqlTransaction (opciono)</param>
        /// <param name="postaviParametre">Akcija za postavljanje parametara</param>
        /// <returns>Broj uticanih redova</returns>
        protected int IzvrsiUpit(string sql, SqlTransaction transakcija = null, Action<SqlCommand> postaviParametre = null)
        {
            using (SqlConnection konekcija = DajKonekciju())
            {
                SqlCommand komanda = new SqlCommand(sql, konekcija, transakcija);
                postaviParametre?.Invoke(komanda);
                if (transakcija == null)
                    konekcija.Open();
                return komanda.ExecuteNonQuery();
            }
        }


        /// Izvrsava SQL komandu i vra?a skalar vrednost (SELECT COUNT, ID, itd.)

        /// <param name="sql">SQL naredba</param>
        /// <param name="transakcija">SqlTransaction (opciono)</param>
        /// <param name="postaviParametre">Akcija za postavljanje parametara</param>
        // <returns>Skalar vrednost</returns>
        protected object IzvrsiSkalar(string sql, SqlTransaction transakcija = null, Action<SqlCommand> postaviParametre = null)
        {
            using (SqlConnection konekcija = DajKonekciju())
            {
                SqlCommand komanda = new SqlCommand(sql, konekcija, transakcija);
                postaviParametre?.Invoke(komanda);
                if (transakcija == null)
                    konekcija.Open();
                return komanda.ExecuteScalar();
            }
        }


        // Izvrsava SQL komandu i vraca SqlDataReader

        /// <param name="sql">SQL naredba</param>
        /// <param name="transakcija">SqlTransaction (opciono)</param>
        /// <param name="postaviParametre">Akcija za postavljanje parametara</param>
        // <returns>SqlDataReader za citanje rezultata</returns>
        protected SqlDataReader IzvrsiCitac(string sql, SqlTransaction transakcija = null, Action<SqlCommand> postaviParametre = null)
        {
            SqlConnection konekcija = DajKonekciju();
            SqlCommand komanda = new SqlCommand(sql, konekcija, transakcija);
            postaviParametre?.Invoke(komanda);
            if (transakcija == null)
                konekcija.Open();
            return komanda.ExecuteReader(CommandBehavior.CloseConnection);
        }


        // Izvrsava SQL komandu i vra?a DataSet

        /// <param name="sql">SQL naredba</param>
        /// <param name="postaviParametre">Akcija za postavljanje parametara</param>
        /// <returns>DataSet sa rezultatima</returns>
        protected DataSet IzvrsiSkupPodataka(string sql, Action<SqlCommand> postaviParametre = null)
        {
            DataSet skupPodataka = new DataSet();
            using (SqlConnection konekcija = DajKonekciju())
            {
                SqlCommand komanda = new SqlCommand(sql, konekcija);
                postaviParametre?.Invoke(komanda);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                adapter.Fill(skupPodataka);
            }
            return skupPodataka;
        }


        // Dodaje SqlParameter u komandu
        /// <param name="komanda">SqlCommand objekat</param>
        /// <param name="imParametra">Ime parametra</param>
        /// <param name="vrednost">Vrednost parametra</param>
        /// <param name="tip">SQL tip (opciono)</param>
        protected void DodajParametar(SqlCommand komanda, string imParametra, object vrednost, SqlDbType? tip = null)
        {
            SqlParameter parametar = new SqlParameter(imParametra, vrednost ?? DBNull.Value);
            if (tip.HasValue)
                parametar.SqlDbType = tip.Value;
            komanda.Parameters.Add(parametar);
        }
    }
}
