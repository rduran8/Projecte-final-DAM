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
        private TextBox TextBoxFiltar;
        private DatePicker DatePickerFiltrar;
        Boolean selFecha = false;
        /// <summary>
        /// 
        /// </summary>
        public Previsiones()
        {
            InitializeComponent();
            this.Title = "IAERP - Previsiones";
        }

        public Previsiones(int id)
        {
            this.id = id;
            InitializeComponent();
            this.Title = "IAERP - Previsiones | Filtro: ID_Previsiones: " + id;
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
            crearTextBox();
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
            //ds.Tables.Add("Previsiones");
            //ds.Tables["Previsiones"].Columns.Add("id");
            //ds.Tables["Previsiones"].Columns.Add("Modulo");
            //ds.Tables["Previsiones"].Columns.Add("Temporalidad");
            //ds.Tables["Previsiones"].Columns.Add("Repeticiones");
            //ds.Tables["Previsiones"].Columns.Add("Fecha");
            //ds.Tables["Previsiones"].Rows.Add(1, "a", "a", 1, DateTime.Now);
            //ds.Tables["Previsiones"].Rows.Add(21, "a", "a", 1, DateTime.Now);
            //ds.Tables["Previsiones"].Rows.Add(31, "a", "a", 1, DateTime.Now);
            //ds.Tables["Previsiones"].Rows.Add(41, "a", "a", 1, DateTime.Now);
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
                Window1 window = new Window1((int)(row.Row["id"]));
                window.Show();
                this.Close();
            }
            catch
            {
                Window1 window = new Window1(1);
                window.Show();
                this.Close();
            }
        }

        private void Inicio_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.Focus();
            this.Close();
        }

        private void BuscarButton_Click(object sender, RoutedEventArgs e)
        {
            if (!selFecha)
            {
                if (!TextBoxFiltar.Text.Equals(""))
                {
                    //ds.Tables["Prevision"].DefaultView.RowFilter = string.Format("'%{0}%' LIKE '%{1}%'", selFiltrar.Text,TextBoxFiltar.Text);
                    try
                    {
                        ds.Tables["Previsiones"].DefaultView.RowFilter = "" + selFiltrar.Text.ToLower() + " = " + TextBoxFiltar.Text;
                    }
                    catch
                    {
                        ds.Tables["Previsiones"].DefaultView.RowFilter = string.Format("{0} LIKE '%{1}%'", selFiltrar.Text.ToLower(), TextBoxFiltar.Text);
                        this.Title = "IAERP - Previsiones | Filtro: " + selFiltrar.Text + ": " + TextBoxFiltar.Text;
                    }
                }
                else
                {
                    ds.Tables["Previsiones"].DefaultView.RowFilter = String.Empty;
                    this.Title = "IAERP - Previsiones";
                }
            }
            else
            {

            }
        }

        private void selFiltarFecha(object sender, RoutedEventArgs e)
        {
            crearDatePicker();
            selFecha = true;
        }

        private void selFiltarText(object sender, RoutedEventArgs e)
        {
            crearTextBox();
            selFecha = false;
        }

        private void crearTextBox()
        {
            DatePickerFiltrar = null;
            TextBoxFiltar = new TextBox();
            TextBoxFiltar.Name = "TextBoxFiltar";
            TextBoxFiltar.Margin = new Thickness(5, 5, 10, 10);
            TextBoxFiltar.Background = ButtonBuscar.Background.CloneCurrentValue();
            TextBoxFiltar.BorderBrush = ButtonBuscar.BorderBrush.CloneCurrentValue();
            TextBoxFiltar.PreviewKeyUp += ComprobarEnter;
            Grid.SetRow(TextBoxFiltar, 1);
            Grid.SetColumn(TextBoxFiltar, 2);
            ButtonsGrid.Children.Add(TextBoxFiltar);
        }

        private void crearDatePicker()
        {
            TextBoxFiltar = null;
            DatePickerFiltrar = new DatePicker();
            DatePickerFiltrar.Name = "DatePickerFiltrar";
            DatePickerFiltrar.Margin = new Thickness(5, 5, 10, 10);
            DatePickerFiltrar.Background = ButtonBuscar.Background.CloneCurrentValue();
            DatePickerFiltrar.BorderBrush = ButtonBuscar.BorderBrush.CloneCurrentValue();
            DatePickerFiltrar.PreviewKeyUp += ComprobarEnter;
            Grid.SetRow(DatePickerFiltrar, 1);
            Grid.SetColumn(DatePickerFiltrar, 2);
            ButtonsGrid.Children.Add(DatePickerFiltrar);
        }

        private void ComprobarEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BuscarButton_Click(sender, e);
            }
        }
    }
}