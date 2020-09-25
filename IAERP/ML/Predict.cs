using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.TimeSeries;
using TrainModel_IAERP.Data;
using TrainModel_IAERP.DataStructures;
using IAERP;
using System.Data.SqlClient;

namespace IAERP
{
    class Predict
    {
        private static string BaseModelsRelativePath = @"../../MLModels";
        private static string ModelRelativePath = $"{BaseModelsRelativePath}/product_month_fastTreeTweedie.zip";
        private static string ModelPath = GetAbsolutePath(ModelRelativePath);
        public static void PredictionRegression(int id_predict, SqlParameter[] parameters, string outputModelPath = "product_month_fastTreeTweedie.zip")
        {
            MLContext mlContext = new MLContext(seed: 1);

            // Read the model that has been previously saved by the method SaveModel.
            ITransformer trainedModel;
            using (var stream = File.OpenRead(ModelPath))
            {
                trainedModel = mlContext.Model.Load(stream, out var modelInputSchema);
            }

            var predictionEngine = mlContext.Model.CreatePredictionEngine<SaleData, ProductUnitRegressionPrediction>(trainedModel);
            DBManagement connect = new DBManagement();
            ProductUnitRegressionPrediction prediction = null;
            int mescalc = ((DateTime)parameters[3].Value).Month;
            int anocalc = ((DateTime)parameters[3].Value).Year;
            int anterior = 0;

            for (int i = 0; i < (int)parameters[1].Value; i++)
            {
                SaleData lastMonth = null;
                
                //int mescalc = ((DateTime)parameters[3].Value).Month;
                //int anocalc = ((DateTime)parameters[3].Value).Year;
                if (i == 0)
                {
                    lastMonth = connect.getLastMonthSaleData(988, anocalc, mescalc);
                }
                else
                {
                    lastMonth = new SaleData()
                    {
                        siguiente = 0,
                        id = (float)988,
                        ano = (float)anocalc,
                        mes = (float)mescalc,
                        unidades = (float)prediction.Score,
                        anterior = (float)anterior
                    };
                }
                // Predict the nextperiod/month forecast to the one provided
                prediction = predictionEngine.Predict(lastMonth);
                anterior = (int)lastMonth.unidades;

                SqlParameter[] parametersPrevision = connect.getparametersPrevisionArray();
                parametersPrevision[0].Value = id_predict;
                parametersPrevision[1].Value = lastMonth.id;
                if ((int)lastMonth.mes + 1 <= 12)
                {
                    parametersPrevision[2].Value = new DateTime((int)lastMonth.ano, ((int)lastMonth.mes + 1), 1);
                }
                else
                {
                    parametersPrevision[2].Value = new DateTime((int)lastMonth.ano + 1, 1, 1);
                }
                parametersPrevision[3].Value = prediction.Score;
                connect.insertPrevision(parametersPrevision);
                mescalc = (((DateTime)parameters[3].Value).Month + i) % 12 + 1;
                anocalc = ((DateTime)parameters[3].Value).Year + (((DateTime)parameters[3].Value).Month + i) / 12;
            }
        }

        public static string GetAbsolutePath(string relativeDatasetPath)
        {
            var _dataRoot = new FileInfo(typeof(Predict).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativeDatasetPath);

            return fullPath;
        }
    }
}
