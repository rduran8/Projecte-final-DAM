using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.TimeSeries;
using TrainModel_IAERP.Data;
using TrainModel_IAERP.DataStructures;
using System.Drawing;
using IAERP;

namespace TrainModel_IAERP
{
    class Predict
    {
        private static void TestPredictionRegression(MLContext mlContext, string outputModelPath, string dataPath)
        {
            Console.WriteLine("Testing Product Unit Sales Forecast Regression model");

            // Read the model that has been previously saved by the method SaveModel.

            ITransformer trainedModel;
            using (var stream = File.OpenRead(outputModelPath))
            {
                trainedModel = mlContext.Model.Load(stream, out var modelInputSchema);
            }

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
    }
}
