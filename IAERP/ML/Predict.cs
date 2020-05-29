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
        public static void PredictionRegression(int id_predict ,string outputModelPath = "product_month_fastTreeTweedie.zip")
        {
            MLContext mlContext = new MLContext(seed: 1);

            // Read the model that has been previously saved by the method SaveModel.
            ITransformer trainedModel;
            using (var stream = File.OpenRead(outputModelPath))
            {
                trainedModel = mlContext.Model.Load(stream, out var modelInputSchema);
            }

            var predictionEngine = mlContext.Model.CreatePredictionEngine<SaleData, ProductUnitRegressionPrediction>(trainedModel);
            DBManagement connect = new DBManagement();

            

            SaleData lastMonth = connect.getLastMonthSaleData(988 , 2017, 10);

            // Predict the nextperiod/month forecast to the one provided
            ProductUnitRegressionPrediction prediction = predictionEngine.Predict(lastMonth);
            
            SqlParameter[] parametersPrevision = connect.getparametersPrevisionArray();
            parametersPrevision[0].Value = id_predict;           
            parametersPrevision[1].Value = lastMonth.id;
            parametersPrevision[2].Value = new DateTime((int)lastMonth.ano, (int)lastMonth.mes + 1, 1);
            parametersPrevision[3].Value = prediction.Score;
            connect.insertPrevision(parametersPrevision);

        }
    }
}
