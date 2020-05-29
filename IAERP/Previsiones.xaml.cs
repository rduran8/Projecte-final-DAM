using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IAERP
{
    /// <summary>
    /// Lógica de interacción para Previsiones.xaml
    /// </summary>
    public partial class Previsiones : Window
    {
        SqlConnection cnn;
        private DataSet ds = new DataSet();
        private int id = 0;
        public Previsiones()
        {
            InitializeComponent();
        }

        public Previsiones(int id)
        {
            this.id = id;
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DBManager manager = new DBManager();
            //DBConnection conect = new DBConnection();
            //conect.connect();
            //SqlConnection cnn = conect.GetConnection();
            //SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM dbo.LinAlbCompra", cnn);
            //DataSet ds = new DataSet();
            //da.Fill(ds, "LinAlbCompra");
            //DataGridPrevisiones.ItemsSource = ds.Tables["LinAlbCompra"].DefaultView;
            DBConnection conect = new DBConnection();
            conect.connect();
            cnn = conect.GetConnection();
            SqlDataAdapter da;
            if (id != 0)
            {
                da = new SqlDataAdapter("SELECT id,Modulo,Temporalidad,Repeticiones,Fecha FROM dbo.Previsiones where id = @idsel", cnn);
                da.SelectCommand.Parameters.AddWithValue("@idsel", id);
            }
            else
            {
                da = new SqlDataAdapter("SELECT id,Modulo,Temporalidad,Repeticiones,Fecha FROM dbo.Previsiones", cnn);
            }
            da.Fill(ds, "Previsiones");
            DataGridPrevisiones.ItemsSource = ds.Tables["Previsiones"].DefaultView;
            conect.disconnect();
            
        }

        private void DataGridPrevisiones_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid send = sender as DataGrid;
            DataRowView row = send.CurrentItem as DataRowView;
            try
            {
                row = send.SelectedItems[0] as DataRowView;
                //row.Row["ID"] = (int)(row.Row["ID"]) + 1;
                Window1 window = new Window1((int)(row.Row["ID"]));
                window.Show();
                this.Close();
            }
            catch
            {

            }
        }
    }
}