using System;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace IAERP
{
    /// <summary>
    /// Lógica de interacción para PointShapeLineExample.xaml
    /// </summary>
    public partial class PointShapeLineExample : UserControl
    {
        private static readonly string BaseDatasetsRelativePath = @"../../../Data";
        private static readonly string SaleDataRelativePath = $"{BaseDatasetsRelativePath}/sales.stats.csv";
        private static readonly string SaleDataPath = GetAbsolutePath(SaleDataRelativePath);
        public PointShapeLineExample()
        {
            InitializeComponent();
            string dataPath = SaleDataPath;
            SeriesCollection = new SeriesCollection { };
            //modifying the series collection will animate and update the chart
            SeriesCollection.Add(new LineSeries
            {
                Title = "Original",
                Values = new ChartValues<double> { },
                LineSmoothness = 0.5, //0: straight lines, 1: really smooth lines
                //PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
                //PointGeometrySize = 50,
                //PointForeground = Brushes.Red
            }) ;

            //SeriesCollection.Add(new LineSeries
            //{
            //    Title = "Predicted",
            //    Values = new ChartValues<double> { 5, 3, 2, 4 },
            //    LineSmoothness = 0, //0: straight lines, 1: really smooth lines
            //    //PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
            //    //PointGeometrySize = 50,
            //    //PointForeground = Brushes.Blue
            //});
            String[]  Labelss = new String[] {};

            using (var reader = new StreamReader(dataPath))
            {
                int i = 0;
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var valuest = line.Split(',');
                    double value = Convert.ToInt32(valuest[4]);
                    SeriesCollection[0].Values.Add(value);
                    Array.Resize(ref Labelss, Labelss.Length + 1);
                    Labelss.SetValue(valuest[3]+"-"+valuest[2], Labelss.Length -1);
                    i++;
                }
            }

            Labels = Labelss;
            YFormatter = value => value.ToString("");

            //modifying any series values will also animate and update the chart
           
            //SeriesCollection[1].Values.Add(5d);

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public static string GetAbsolutePath(string relativeDatasetPath)
        {
            var _dataRoot = new FileInfo(typeof(GraphTest).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName + "\\IAERP";

            string fullPath = System.IO.Path.Combine(assemblyFolderPath, relativeDatasetPath);

            return fullPath;
        }
    }
    
}
