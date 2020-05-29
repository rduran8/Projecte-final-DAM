using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Lógica de interacción para GraphTest.xaml
    /// </summary>
    public partial class GraphTest : Window
    {
        private static readonly string BaseDatasetsRelativePath = @"../../../Data";
        private static readonly string SaleDataRelativePath = $"{BaseDatasetsRelativePath}/sales.stats.csv";
        private static readonly string SaleDataPath = GetAbsolutePath(SaleDataRelativePath);

        public GraphTest()
        {
            InitializeComponent();
        }

        // Draw a simple graph.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Make some data sets.
            //using (var reader = new StreamReader(dataPath))
            //{
            //    Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
            //    PointCollection points = new PointCollection();
            //    double x = 0 + margin;
            //    reader.ReadLine();
            //    while (!reader.EndOfStream)
            //    {
            //        var line = reader.ReadLine();
            //        var values = line.Split(',');
            //        double value = ymax - Convert.ToInt32(values[4])/step;
            //        points.Add(new Point(x, value));
            //        x += step;
            //    }

            //    Polyline polyline = new Polyline();
            //    polyline.StrokeThickness = 1;
            //    polyline.Stroke = brushes[0];
            //    polyline.Points = points;
            //    canGraph.Children.Add(polyline);
            //}
        }

        public static string GetAbsolutePath(string relativeDatasetPath)
        {
            var _dataRoot = new FileInfo(typeof(GraphTest).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName + "\\IAERP";

            string fullPath = System.IO.Path.Combine(assemblyFolderPath, relativeDatasetPath);

            return fullPath;
        }
    }
    
}

