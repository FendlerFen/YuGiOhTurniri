using System;
using System.Data;
using System.Data.SqlClient;

namespace DBUtils
{
    /// <summary>
    /// Bazna klasa za rad sa bazom podataka kroz SQL konekciju
    /// </summary>
    public class KonekcijaKlasa
    {
        private readonly string _konekcijskiString;

        public KonekcijaKlasa(string konekcijskiString)
        {
            _konekcijskiString = konekcijskiString;
        }

        /// <summary>
        /// Izvršava SELECT upit i vraća DataSet
        /// </summary>
        public DataSet IzvrsiUpit(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(_konekcijskiString))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Greška pri izvršavanju upita: " + ex.Message);
            }
            return ds;
        }

        /// <summary>
        /// Izvršava INSERT, UPDATE, DELETE i vraća broj promijenjenih redova
        /// </summary>
        public int IzvrsiNeupit(string sql)
        {
            int rezultat = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_konekcijskiString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    rezultat = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Greška pri izvršavanju nke-upita: " + ex.Message);
            }
            return rezultat;
        }

        /// <summary>
        /// Izvršava upit i vraća skalarnu vrijednost (jedan podatak)
        /// </summary>
        public object IzvrsiSkalar(string sql)
        {
            object rezultat = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(_konekcijskiString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    rezultat = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Greška pri izvršavanju skalarnog upita: " + ex.Message);
            }
            return rezultat;
        }

        /// <summary>
        /// Izvršava Stored Procedure
        /// </summary>
        public DataSet IzvrsiStoredProceduru(string proceduraIme, SqlParameter[] parametri = null)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(_konekcijskiString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(proceduraIme, conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (parametri != null)
                    {
                        cmd.Parameters.AddRange(parametri);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Greška pri izvršavanju stored procedure: " + ex.Message);
            }
            return ds;
        }
    }
}