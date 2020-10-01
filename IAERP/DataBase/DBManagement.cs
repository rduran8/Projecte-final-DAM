using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainModel_IAERP.DataStructures;

namespace IAERP
{
    class DBManagement
    {
        private DBConnection conect = null;
        private SqlParameter[] parameters = null;
        private SqlParameter[] parametersPrevision = null;
        public DBManagement()
        {
            conect = new DBConnection();
        }
        //public DataSet createDataSet(String dbo, String query)
        //{
        //    DBConnection conect = new DBConnection();
        //    conect.connect();
        //    SqlConnection cnn = conect.GetConnection();
        //    cnn.Close();
        //    using (var com = cnn.CreateCommand())
        //    {
        //        SqlDataAdapter da = new SqlDataAdapter();
        //        com.CommandText = query;
        //        com.Parameters.AddWithValue("@dbo", dbo);
        //        com.CommandType = CommandType.Text;
        //        da.SelectCommand = com;
        //        DataSet ds = new DataSet();
        //        cnn.Open();
        //        da.Fill(ds, "LinAlbCompra");
        //        cnn.Close();
        //        return ds;
        //    }
        //}
        //"Select * from dbo.LinAlbCompra", cnn

        //int id, string modulo, string repeticiones, string temporalidad, DateTime fecha, int IDPrevisiones
        public int insertPrevisiones(SqlParameter[] parameters)
        {
            SqlConnection cnn = conect.connect();
            using (var com = cnn.CreateCommand())
            {
                string query1 = "insert into dbo.previsiones (Modulo, Repeticiones, Temporalidad, Fecha) OUTPUT INSERTED.ID values(@Modulo, @Repeticiones, @Temporalidad, @Fecha)";
                com.CommandText = query1;
                com.Parameters.AddRange(parameters);
                com.CommandType = CommandType.Text;
                com.ExecuteNonQuery();
                string query2 = "Select @@Identity as id from dbo.previsiones";
                com.CommandText = query2;
                com.CommandType = CommandType.Text;
                com.Connection = cnn;
                int id = Convert.ToInt32(com.ExecuteScalar());
                cnn.Close();
                return id;
            }
        }

        public void insertPrevision(SqlParameter[] parametersPrevision)
        {
            SqlConnection cnn = conect.connect();
            using (var com = cnn.CreateCommand())
            {
                string query1 = "insert into dbo.prevision (idprevision, producto, fecha, prevision) OUTPUT INSERTED.ID values(@idprevision, @producto, @fecha, @prevision)";
                com.CommandText = query1;
                com.Parameters.AddRange(parametersPrevision);
                com.CommandType = CommandType.Text;
                com.ExecuteNonQuery();
                cnn.Close();
            }
        }

        public SaleData getLastMonthSaleData(int id, int ano, int mes)
        {
            SqlDataReader record = null;
            SqlConnection cnn = conect.connect();
            using (var com = cnn.CreateCommand())
            {
                SqlParameter[] parSelect = getParSelectArray();
                parSelect[0].Value = id;
                parSelect[1].Value = ano;
                parSelect[2].Value = mes;
                string query1 = "select siguiente, id, ano, mes, unidades, anterior from dbo.SaleData where id = @id and ano = @ano and mes = @mes";
                com.CommandText = query1;
                com.Parameters.AddRange(parSelect);
                com.CommandType = CommandType.Text;
                record = com.ExecuteReader();
                
            }
            SaleData lastMonth = new SaleData();
            if (record.HasRows)
            {
                while (record.Read())
                {
                    lastMonth.siguiente = record.GetInt32(0);
                    lastMonth.id = record.GetInt32(1);
                    lastMonth.ano = record.GetInt32(2);
                    lastMonth.mes = record.GetInt32(3);
                    lastMonth.unidades = record.GetInt32(4);
                    lastMonth.anterior = record.GetInt32(5);
                }
            }
            cnn.Close();
            return lastMonth;
        }

        public SqlParameter[] getParametersArray()
        {
            parameters = new SqlParameter[4];
            parameters[0] = new SqlParameter("@Modulo", SqlDbType.VarChar);
            parameters[1] = new SqlParameter("@Repeticiones", SqlDbType.Int);
            parameters[2] = new SqlParameter("@Temporalidad", SqlDbType.VarChar);
            parameters[3] = new SqlParameter("@Fecha", SqlDbType.Date);
            return parameters;
        }
        public SqlParameter[] getparametersPrevisionArray()
        {
            parametersPrevision = new SqlParameter[4];
            parametersPrevision[0] = new SqlParameter("@idprevision", SqlDbType.Int);
            parametersPrevision[1] = new SqlParameter("@producto", SqlDbType.VarChar);
            parametersPrevision[2] = new SqlParameter("@fecha", SqlDbType.Date);
            parametersPrevision[3] = new SqlParameter("@prevision", SqlDbType.Int);
            return parametersPrevision;
        }

        public SqlParameter[] getParSelectArray()
        {
            SqlParameter[] parSelect = new SqlParameter[3];
            parSelect[0] = new SqlParameter("@id", SqlDbType.Int);
            parSelect[1] = new SqlParameter("@ano", SqlDbType.Int);
            parSelect[2] = new SqlParameter("@mes", SqlDbType.Int);
            return parSelect;
        }
    }
}
