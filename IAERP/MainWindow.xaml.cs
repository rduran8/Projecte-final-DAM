using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace IAERP
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Boolean ModuloOk = false;
        private Boolean RepeticionesOK =false;
        private Boolean TemporalidadOK = false;
        private Boolean FechaOK = false;
        private DBManagement dbManager = null;
        public static MainWindow mainWindow;
        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            this.Title = "IAERP";
            dbManager = new DBManagement();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GraphTest window = new GraphTest();
            window.ShowDialog();
        }

        private void CalcularButton_Click(object sender, RoutedEventArgs e)
        {
            if(ModuloOk && RepeticionesOK && TemporalidadOK && FechaOK)
            {
                SqlParameter[] parameters = dbManager.getParametersArray();
                parameters[0].Value = (selModulo.SelectedItem as ListBoxItem).Content.ToString();           //MODULO
                parameters[1].Value = int.Parse(selRepeticiones.Text);                                      //REPETICIONES
                parameters[2].Value = (selTemporalidad.SelectedItem as ComboBoxItem).Content.ToString();    //TEMPORALIDAD
                parameters[3].Value = selFecha.SelectedDate;                                                //FECHA
                int id = dbManager.insertPrevisiones(parameters);
                Predict.PredictionRegression(id, parameters);
                Previsiones window = new Previsiones(id);
                window.ShowDialog();
            }
            else
            {
                if(!ModuloOk)
                {
                    textModulo.Foreground = Brushes.Red;
                }
                if (!TemporalidadOK)
                {
                    textTemporalidad.Foreground = Brushes.Red;
                }
                if (!FechaOK)
                {
                    textFecha.Foreground = Brushes.Red;
                }
                if (!RepeticionesOK)
                {
                    textRepeticiones.Foreground = Brushes.Red;
                }
                MessageBox.Show("No se han rellenado todos los campos correctamente.\nCompruebe los campos y vuelva a intentarlo");
            }
        }

        private void selModulo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems[0] as ListBoxItem) != null)
            {
                ModuloOk = true;
                textModulo.Foreground = Brushes.Black;
            }
        }

        private void selTemporalidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems[0] as ComboBoxItem) != null)
            {
                TemporalidadOK = true;
                textTemporalidad.Foreground = Brushes.Black;
                comprobarRepeticiones();
                establecerTiempoDataPicker();
            }
        }

        private void establecerTiempoDataPicker()
        {
            //selFecha.DisplayDate. = FindResource("MES") as Style;
        }

        private void selFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FechaOK = true;
            textFecha.Foreground = Brushes.Black;
        }

        private void selRepeticiones_LostFocus(object sender, RoutedEventArgs e)
        {
            comprobarRepeticiones();
        }

        private void comprobarRepeticiones()
        { 
            try
            {
                if (selRepeticiones.Text != "")
                {
                    int num = Convert.ToInt32(selRepeticiones.Text);
                    if (TemporalidadOK)
                    {
                        switch ((selTemporalidad.SelectedItem as ComboBoxItem).Content.ToString())
                        {
                            case "DIA":
                                if (DateTime.IsLeapYear(DateTime.Now.Year))
                                {
                                    if (num <= 366)
                                    {
                                        RepeticionesOK = true;
                                        textRepeticiones.Foreground = Brushes.Black;
                                    }
                                    else
                                    {
                                        textRepeticiones.Foreground = Brushes.Red;
                                        MessageBox.Show("El número de repeticiones no puede ser superior a 366 dias");
                                    }
                                }
                                else
                                {
                                    if (num <= 365)
                                    {
                                        RepeticionesOK = true;
                                        textRepeticiones.Foreground = Brushes.Black;
                                    }
                                    else
                                    {
                                        textRepeticiones.Foreground = Brushes.Red;
                                        MessageBox.Show("El número de repeticiones no puede ser superior a 365 dias");
                                    }
                                }
                                break;
                            case "SEMANA":
                                if (num <= 53)
                                {
                                    RepeticionesOK = true;
                                    textRepeticiones.Foreground = Brushes.Black;
                                }
                                else
                                {
                                    textRepeticiones.Foreground = Brushes.Red;
                                    MessageBox.Show("El número de repeticiones no puede ser superior a 53 semanas");
                                }
                                break;
                            case "MES":
                                if (num <= 12)
                                {
                                    RepeticionesOK = true;
                                    textRepeticiones.Foreground = Brushes.Black;
                                }
                                else
                                {
                                    MessageBox.Show("El número de repeticiones no puede ser superior a 12 meses");
                                    textRepeticiones.Foreground = Brushes.Red;
                                }
                                break;
                            case "AÑO":
                                if (num <= 20)
                                {
                                    RepeticionesOK = true;
                                    textRepeticiones.Foreground = Brushes.Black;
                                }
                                else
                                {
                                    MessageBox.Show("El número de repeticiones no puede ser superior a 20 años");
                                    textRepeticiones.Foreground = Brushes.Red;
                                }
                                break;
                            default:
                                MessageBox.Show("Antes de entrar las repeticiones se debe seleccionar la temporalidad.");
                                textTemporalidad.Foreground = Brushes.Red;
                                textRepeticiones.Foreground = Brushes.Red;
                                TemporalidadOK = false;
                                RepeticionesOK = false;
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Antes de entrar las repeticiones se debe seleccionar la temporalidad.");
                        textTemporalidad.Foreground = Brushes.Red;
                        textRepeticiones.Foreground = Brushes.Red;
                        TemporalidadOK = false;
                        RepeticionesOK = false;
                    }
                }
            }
            catch
            {
                MessageBox.Show("El campo repeticiones debe ser un número.");
                textRepeticiones.Foreground = Brushes.Red;
                RepeticionesOK = false;
            }
        }

        private void Calendar_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Calendar calObj = sender as Calendar;

            calObj.DisplayMode = CalendarMode.Year;
        }

        private void Calendar_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            Calendar calObj = sender as Calendar;
            //< EventSetter Event = "DisplayModeChanged" Handler = "Calendar_DisplayModeChanged" />
            calObj.DisplayMode = CalendarMode.Year;
        }

        private void selFecha_CalendarOpened(object sender, RoutedEventArgs e)
        {
            //Calendar calObj = sender as Calendar;

            //calObj.DisplayMode = CalendarMode.Year;
        }

        private void ConsultarButton_Click(object sender, RoutedEventArgs e)
        {
            Previsiones window = new Previsiones();
            window.ShowDialog();
        }

        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //private static readonly string BaseDatasetsRelativePath = @"../../../IAERP/Data";
        //private static readonly string SaleDataRelativePath = $"{BaseDatasetsRelativePath}/sales.stats.csv";
        //private static readonly string SaleDataPath = GetAbsolutePath(SaleDataRelativePath);

        //public static string GetAbsolutePath(string relativeDatasetPath)
        //{
        //    var _dataRoot = new FileInfo(typeof(MainWindow).Assembly.Location);
        //    string assemblyFolderPath = _dataRoot.Directory.FullName;

        //    string fullPath = System.IO.Path.Combine(assemblyFolderPath, relativeDatasetPath);

        //    return fullPath;
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    DataTable tblcsv = new DataTable();
        //    //creating columns  
        //    tblcsv.Columns.Add("siguiente");
        //    tblcsv.Columns.Add("id");
        //    tblcsv.Columns.Add("ano");
        //    tblcsv.Columns.Add("mes");
        //    tblcsv.Columns.Add("unidades");
        //    tblcsv.Columns.Add("anterior");
        //    tblcsv.Columns.Add("id_record");
        //    //getting full file path of Uploaded file  
        //    string CSVFilePath = @""+SaleDataPath;
        //    //Reading All text  
        //    string ReadCSV = File.ReadAllText(SaleDataPath);
        //    //spliting row after new line  
        //    Boolean si = false;
        //    int i = 0;
        //    foreach (string csvRow in ReadCSV.Split('\n'))
        //    {
        //        i++;
        //        if (!si)
        //        {
        //            si = true;
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(csvRow))
        //            {
        //                //Adding each row into datatable  
        //                tblcsv.Rows.Add();
        //                int count = 0;
        //                foreach (string FileRec in csvRow.Split(','))
        //                {
        //                    Int32.Parse(FileRec);
        //                    tblcsv.Rows[tblcsv.Rows.Count - 1][count] = FileRec;
        //                    count++;
        //                }
        //                tblcsv.Rows[tblcsv.Rows.Count - 1][count] = i;
        //            }
        //        }


        //    }
        //    //Calling insert Functions  
        //    InsertCSVRecords(tblcsv);
        //}

        //private void InsertCSVRecords(DataTable csvdt)
        //{
        //    SqlConnection cnn;
        //    DBConnection conect = new DBConnection();
        //    conect.connect();
        //    cnn = conect.GetConnection();
        //    //creating object of SqlBulkCopy    
        //    SqlBulkCopy objbulk = new SqlBulkCopy(cnn);
        //    //assigning Destination table name    
        //    objbulk.DestinationTableName = "sales";
        //    //Mapping Table column    
        //    objbulk.ColumnMappings.Add("siguiente", "siguiente");
        //    objbulk.ColumnMappings.Add("id", "id");
        //    objbulk.ColumnMappings.Add("ano", "ano");
        //    objbulk.ColumnMappings.Add("mes", "mes");
        //    objbulk.ColumnMappings.Add("unidades", "unidades");
        //    objbulk.ColumnMappings.Add("anterior", "anterior");
        //    objbulk.ColumnMappings.Add("id_record", "id_record");
        //    //inserting Datatable Records to DataBase    
        //    objbulk.WriteToServer(csvdt);
        //    cnn.Close();
        //}
    }
}
