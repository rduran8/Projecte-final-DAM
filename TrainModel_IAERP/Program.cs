using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.TimeSeries;
using TrainModel_IAERP.Data;
using TrainModel_IAERP.DataStructures;
using System.Drawing;

namespace TrainModel_IAERP
{
    class Program
    {
        private static readonly string BaseDatasetsRelativePath = @"../../../Data";
        private static readonly string SaleDataRelativePath = $"{BaseDatasetsRelativePath}/sales.stats.csv";
        private static readonly string SaleDataPath = GetAbsolutePath(SaleDataRelativePath);

        private static string BaseModelsRelativePath = @"../../../MLModels";
        private static string ModelRelativePath = $"{BaseModelsRelativePath}/product_month_fastTreeTweedie.zip";
        private static string ModelPath = GetAbsolutePath(ModelRelativePath);


        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext(seed: 1);
            string dataPath = SaleDataPath;
            float id = 988;
            CreateAndSaveModelTimeSeries(mlContext, dataPath, id);
            CreateAndSaveModelRegression(mlContext,dataPath);
        }

        //private static void CreateAndSaveModelRegression(MLContext mlContext, string dataPath, string outputModelPath = "product_month_fastTreeTweedie.zip")
        private static void CreateAndSaveModelRegression(MLContext mlContext, string dataPath)
        {
            //if(File.Exists(outputModelPath))
            if (File.Exists(ModelPath))
            {
                //File.Delete(outputModelPath);
                File.Delete(ModelPath);
            }

            Console.WriteLine("Cargando datos");
            var trainingDataView = mlContext.Data.LoadFromTextFile<SaleData>(dataPath, hasHeader: true, separatorChar: ',');

            Console.WriteLine("Creando trainer");
            var trainer = mlContext.Regression.Trainers.FastTreeTweedie(labelColumnName: "Label", featureColumnName: "Features");

            Console.WriteLine("Creando Pipeline");
            var trainingPipeline = mlContext.Transforms.Concatenate(outputColumnName: "NumFeatures", nameof(SaleData.ano), nameof(SaleData.mes), nameof(SaleData.unidades), nameof(SaleData.anterior))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CatFeatures", inputColumnName: nameof(SaleData.id)))
                    .Append(mlContext.Transforms.Concatenate(outputColumnName: "Features", "NumFeatures", "CatFeatures"))
                    .Append(mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(SaleData.siguiente)))
                    .Append(trainer);

            Console.WriteLine("Validando");
            var crossValidationResults = mlContext.Regression.CrossValidate(data: trainingDataView, estimator: trainingPipeline, numberOfFolds: 2, labelColumnName: "Label");
            Console.WriteLine(trainer.ToString(), crossValidationResults);

            var model = trainingPipeline.Fit(trainingDataView);

            // Save the model for later comsumption from end-user apps.
            //mlContext.Model.Save(model, trainingDataView.Schema, outputModelPath);
            mlContext.Model.Save(model, trainingDataView.Schema, ModelPath);

            //TestPredictionRegression(mlContext,outputModelPath,dataPath);
            TestPredictionRegression(mlContext, ModelPath, dataPath);
        }

        private static void TestPredictionRegression(MLContext mlContext, string outputModelPath, string dataPath)
        {
            Console.WriteLine("Testing Product Unit Sales Forecast Regression model");

            // Read the model that has been previously saved by the method SaveModel.

            ITransformer trainedModel;
            using (var stream = File.OpenRead(outputModelPath))
            {
                trainedModel = mlContext.Model.Load(stream, out var modelInputSchema);
            }

            //using (var reader = new StreamReader(dataPath))
            //{
            //    reader.ReadLine();
            //    while (!reader.EndOfStream)
            //    {
            //        var line = reader.ReadLine();
            //        var values = line.Split(',');
            //        int value = Convert.ToInt32(values[4]);
            //        for (int i = 0; i < value - 1; i++)
            //        {
            //            Console.Write("*");
            //        }
            //        Console.Write("*\n");
            //    }
            //}

            var predictionEngine = mlContext.Model.CreatePredictionEngine<SaleData, ProductUnitRegressionPrediction>(trainedModel);
            Console.WriteLine("** Testing Product **");

            // Predict the nextperiod/month forecast to the one provided
            ProductUnitRegressionPrediction prediction = predictionEngine.Predict(SampleSaleData.MonthlyData[0]);
            Console.WriteLine($"Product: {SampleSaleData.MonthlyData[0].id}, month: {SampleSaleData.MonthlyData[0].mes + 1}, year: {SampleSaleData.MonthlyData[0].ano} - Real value (units): {SampleSaleData.MonthlyData[0].siguiente}, Forecast Prediction (units): {prediction.Score}");

            // Predicts the nextperiod/month forecast to the one provided
            prediction = predictionEngine.Predict(SampleSaleData.MonthlyData[1]);
            Console.WriteLine($"Product: {SampleSaleData.MonthlyData[1].id}, month: {SampleSaleData.MonthlyData[1].mes + 1}, year: {SampleSaleData.MonthlyData[1].ano} - Forecast Prediction (units): {prediction.Score}");

            //prediction = predictionEngine.Predict(SampleSaleData.MonthlyData[2]);
            //Console.WriteLine($"Product: {SampleSaleData.MonthlyData[2].id}, month: {SampleSaleData.MonthlyData[2].mes + 1}, year: {SampleSaleData.MonthlyData[2].ano} - Forecast Prediction (units): {prediction.Score}");
        }


        /// <summary>
        /// Crea el modelo y lo guarda en el path entrado
        /// </summary>
        /// <param name="mlContext">ML.NET context.</param>
        /// <param name="productId">Id of the product series to forecast.</param>
        /// <param name="dataPath">Input data file path.</param>
        private static void CreateAndSaveModelTimeSeries(MLContext mlContext, string dataPath, float id)
        {
            var productModelPath = $"product{id}_month_timeSeriesSSA.zip";

            if (File.Exists(productModelPath))
            {
                File.Delete(productModelPath);
            }
            IDataView saleDataView = LoadData(mlContext, dataPath, id);
            var singleProductDataSeries = mlContext.Data.CreateEnumerable<SaleData>(saleDataView, false).OrderBy(p => p.ano).ThenBy(p => p.mes);
            SaleData lastMonthSalesData = singleProductDataSeries.Last();

            Console.WriteLine("Creando el modelo: Time Series Model");

            const int numSeriesDataPoints = 34;
            IEstimator<ITransformer> forecastEstimator = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(ProductUnitTimeSeriesPrediction.ForecastedProductUnits),
                inputColumnName: nameof(SaleData.unidades), // This is the column being forecasted.
                windowSize: 12, // Window size is set to the time period represented in the product data cycle; our product cycle is based on 12 months, so this is set to a factor of 12, e.g. 3.
                seriesLength: numSeriesDataPoints, // This parameter specifies the number of data points that are used when performing a forecast.
                trainSize: numSeriesDataPoints, // This parameter specifies the total number of data points in the input time series, starting from the beginning.
                horizon: 2, // Indicates the number of values to forecast; 2 indicates that the next 2 months of product units will be forecasted.
                confidenceLevel: 0.95f, // Indicates the likelihood the real observed value will fall within the specified interval bounds.
                confidenceLowerBoundColumn: nameof(ProductUnitTimeSeriesPrediction.ConfidenceLowerBound), //This is the name of the column that will be used to store the lower interval bound for each forecasted value.
                confidenceUpperBoundColumn: nameof(ProductUnitTimeSeriesPrediction.ConfidenceUpperBound)); //This is the name of the column that will be used to store the upper interval bound for each forecasted value.

            // Fit the forecasting model to the specified product's data series.
            ITransformer forecastTransformer = forecastEstimator.Fit(saleDataView);

            // Create the forecast engine used for creating predictions.
            TimeSeriesPredictionEngine<SaleData, ProductUnitTimeSeriesPrediction> forecastEngine = forecastTransformer.CreateTimeSeriesEngine<SaleData, ProductUnitTimeSeriesPrediction>(mlContext);

            // Save the forecasting model so that it can be loaded within an end-user app.
            forecastEngine.CheckPoint(mlContext, productModelPath);
            TestPrediction(mlContext, lastMonthSalesData, productModelPath);
        }

        private static IDataView LoadData(MLContext mlContext, string dataPath, float id)
        {
            IDataView allProductsDataView = mlContext.Data.LoadFromTextFile<SaleData>(dataPath, hasHeader: true, separatorChar: ',');
            IDataView productDataView = mlContext.Data.FilterRowsByColumn(allProductsDataView, nameof(SaleData.id), id, id + 1);

            return productDataView;
        }

        private static void TestPrediction(MLContext mlContext, SaleData lastMonthProductData, string outputModelPath)
        {
            Console.WriteLine("Testing product unit sales forecast Time Series model");

            // Load the forecast engine that has been previously saved.
            ITransformer forecaster;
            using (var file = File.OpenRead(outputModelPath))
            {
                forecaster = mlContext.Model.Load(file, out DataViewSchema schema);
            }

            // We must create a new prediction engine from the persisted model.
            TimeSeriesPredictionEngine<SaleData, ProductUnitTimeSeriesPrediction> forecastEngine = forecaster.CreateTimeSeriesEngine<SaleData, ProductUnitTimeSeriesPrediction>(mlContext);

            // Get the prediction; this will include the forecasted product units sold for the next 2 months since this the time period specified in the `horizon` parameter when the forecast estimator was originally created.
            Console.WriteLine("\n** Original prediction **");
            ProductUnitTimeSeriesPrediction originalSalesPrediction = forecastEngine.Predict();

            // Compare the units of the first forecasted month to the actual units sold for the next month.
            var predictionMonth = lastMonthProductData.mes == 12 ? 1 : lastMonthProductData.mes + 1;
            var predictionYear = predictionMonth < lastMonthProductData.mes ? lastMonthProductData.ano + 1 : lastMonthProductData.ano;
            Console.WriteLine($"Product: {lastMonthProductData.id}, Mes: {predictionMonth}, Año: {predictionYear} " +
                $"- Real Value (units): {lastMonthProductData.siguiente}, Forecast Prediction (units): {originalSalesPrediction.ForecastedProductUnits[0]}");

            // Get the first forecasted month's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{originalSalesPrediction.ConfidenceLowerBound[0]} - {originalSalesPrediction.ConfidenceUpperBound[0]}]\n");

            // Get the units of the second forecasted month.
            Console.WriteLine($"Product: {lastMonthProductData.id}, Mes: {lastMonthProductData.mes + 2}, Año: {lastMonthProductData.ano}, " +
                $"Forecast (units): {originalSalesPrediction.ForecastedProductUnits[1]}");

            // Get the second forecasted month's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{originalSalesPrediction.ConfidenceLowerBound[1]} - {originalSalesPrediction.ConfidenceUpperBound[1]}]\n");

            // Get the units of the second forecasted month.
            //Console.WriteLine($"Product: {lastMonthProductData.id}, Mes: {lastMonthProductData.mes}, Año: {lastMonthProductData.ano}, " +
                //$"Forecast (units): {originalSalesPrediction.ForecastedProductUnits[0]}");

            // Get the second forecasted month's confidence interval bounds.
            //Console.WriteLine($"Confidence interval: [{originalSalesPrediction.ConfidenceLowerBound[0]} - {originalSalesPrediction.ConfidenceUpperBound[0]}]\n");

            // Update the forecasting model with the next month's actual product data to get an updated prediction; this time, only forecast product sales for 1 month ahead.
            Console.WriteLine("** Updated prediction **");
            ProductUnitTimeSeriesPrediction updatedSalesPrediction = forecastEngine.Predict(SampleSaleData.MonthlyData[1]);

            // Save a checkpoint of the forecasting model.
            forecastEngine.CheckPoint(mlContext, outputModelPath);

            // Get the units of the updated forecast.
            predictionMonth = lastMonthProductData.mes == 12 ? 1 : lastMonthProductData.mes + 1;
            predictionYear = predictionMonth < lastMonthProductData.mes ? lastMonthProductData.ano + 1 : lastMonthProductData.ano;
            Console.WriteLine($"Product: {lastMonthProductData.id}, Mes: {predictionMonth}, Año: {predictionYear} " +
                $"- Real Value (units): {lastMonthProductData.siguiente}, Forecast Prediction (units): {updatedSalesPrediction.ForecastedProductUnits[0]}");

            // Get the updated forecast's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{updatedSalesPrediction.ConfidenceLowerBound[0]} - {updatedSalesPrediction.ConfidenceUpperBound[0]}]\n");


            Console.WriteLine($"Product: {lastMonthProductData.id}, Mes: {lastMonthProductData.mes + 2}, Año: {lastMonthProductData.ano}, " +
                $"Forecast (units): {updatedSalesPrediction.ForecastedProductUnits[1]}");

            // Get the second forecasted month's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{updatedSalesPrediction.ConfidenceLowerBound[1]} - {updatedSalesPrediction.ConfidenceUpperBound[1]}]\n");

            Console.WriteLine("** Updated prediction **");
            updatedSalesPrediction = forecastEngine.Predict(SampleSaleData.MonthlyData[1]);

            // Save a checkpoint of the forecasting model.
            //forecastEngine.CheckPoint(mlContext, outputModelPath);

            // Get the units of the updated forecast.
            predictionMonth = lastMonthProductData.mes == 12 ? 1 : lastMonthProductData.mes + 1;
            predictionYear = predictionMonth < lastMonthProductData.mes ? lastMonthProductData.ano + 1 : lastMonthProductData.ano;
            Console.WriteLine($"Product: {lastMonthProductData.id}, Mes: {predictionMonth}, Año: {predictionYear} " +
                $"- Real Value (units): {lastMonthProductData.siguiente}, Forecast Prediction (units): {updatedSalesPrediction.ForecastedProductUnits[0]}");

            // Get the updated forecast's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{updatedSalesPrediction.ConfidenceLowerBound[0]} - {updatedSalesPrediction.ConfidenceUpperBound[0]}]\n");


            Console.WriteLine($"Product: {lastMonthProductData.id}, Mes: {lastMonthProductData.mes + 2}, Año: {lastMonthProductData.ano}, " +
                $"Forecast (units): {updatedSalesPrediction.ForecastedProductUnits[1]}");

            // Get the second forecasted month's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{updatedSalesPrediction.ConfidenceLowerBound[1]} - {updatedSalesPrediction.ConfidenceUpperBound[1]}]\n");
        }

        public static string GetAbsolutePath(string relativeDatasetPath)
        {
            var _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativeDatasetPath);

            return fullPath;
        }
    }
}
