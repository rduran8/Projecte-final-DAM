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
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        SqlConnection cnn;
        private DataSet ds = new DataSet();
        private int id_previsiones = 0;
        private TextBox TextBoxFiltar;
        private DatePicker DatePickerFiltrar;
        Boolean selFecha = false;
        public Window1(int id_previsiones)
        {
            this.id_previsiones = id_previsiones;
            
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            crearTextBox();
            DBConnection conect = new DBConnection();
            conect.connect();
            cnn = conect.GetConnection();
            SqlDataAdapter da = new SqlDataAdapter("SELECT id, producto, fecha, prevision FROM dbo.Prevision where id_Prevision = @idsel", cnn);
            da.SelectCommand.Parameters.AddWithValue("@idsel", id_previsiones);
            //da.Fill(ds, "Prevision");
            ds.Tables.Add("Prevision");
            ds.Tables["Prevision"].Columns.Add("id");
            ds.Tables["Prevision"].Columns.Add("producto");
            ds.Tables["Prevision"].Columns.Add("fecha");
            ds.Tables["Prevision"].Rows.Add(1, "a", DateTime.Now);
            ds.Tables["Prevision"].Rows.Add(21, "a", DateTime.Now);
            ds.Tables["Prevision"].Rows.Add(31, "a", DateTime.Now);
            ds.Tables["Prevision"].Rows.Add(41, "a", DateTime.Now);
            DataGridPrevision.ItemsSource = ds.Tables["Prevision"].DefaultView;
            conect.disconnect();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Previsiones window = new Previsiones();
            window.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MainWindow window = new MainWindow();
            //window.Show();
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
                        ds.Tables["Prevision"].DefaultView.RowFilter = "" + selFiltrar.Text.ToLower() + " = " + TextBoxFiltar.Text;
                    }
                    catch
                    {
                        ds.Tables["Prevision"].DefaultView.RowFilter = string.Format("{0} LIKE '%{1}%'", selFiltrar.Text.ToLower(), TextBoxFiltar.Text);
                    }
                }
                else
                {
                    ds.Tables["Prevision"].DefaultView.RowFilter = String.Empty;
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
