using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IAERP
{
    class DBConnection
    {
        private SqlConnection cnn;
        private string connetionString = "Data Source=DESKTOP-OJIIBU7;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False; Database=IAERP";
        //connetionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False";
        public SqlConnection connect()
        {
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }
            return cnn;
        }

        public void disconnect()
        {
            cnn.Close();
        }

        public SqlConnection GetConnection()
        {
            return cnn;
        }
    }
}
