using System;
using System.Collections.Generic;
using System.Text;

namespace TrainModel_IAERP.DataStructures
{
    /// <summary>
    /// This is the output of the scored regression model, the prediction.
    /// </summary>
    public class ProductUnitRegressionPrediction
    {
        // Below columns are produced by the model's predictor.
        public float Score;
    }

    /// <summary>
    /// This is the output of the scored time series model, the prediction.
    /// </summary>
    /// 13,1,2017,10,14,15
            //,1,2017,11,13,14
    /// 
    /// 
    /// 
    /// 
    /// 
    /// //12,1,2017,11,13,14
        //,1,2017,12,12,13
    public class ProductUnitTimeSeriesPrediction
    {
        public float[] ForecastedProductUnits { get; set; }

        public float[] ConfidenceLowerBound { get; set; }

        public float[] ConfidenceUpperBound { get; set; }
    }
}