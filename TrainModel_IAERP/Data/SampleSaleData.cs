using System;
using System.Collections.Generic;
using System.Text;
using TrainModel_IAERP.DataStructures;

namespace TrainModel_IAERP.Data
{
    public static class SampleSaleData
    {
        public static SaleData[] MonthlyData { get; }

        static SampleSaleData()
        {
            //MonthlyData = new SaleData[] {
            //    new SaleData()
            //    {
            //        siguiente = 11,
            //        id = 1,
            //        ano = 2017,
            //        mes = 10,
            //        unidades = 10,
            //        anterior = 9
            //    },
            //    new SaleData()
            //    {
            //        id = 1,
            //        ano = 2017,
            //        mes = 11,
            //        unidades = 11,
            //        anterior = 10
            //    },
            //    new SaleData()
            //    {
            //        id = 1,
            //        ano = 2017,
            //        mes = 12,
            //        anterior = 11
            //    }
            //};
            MonthlyData = new SaleData[] {
                new SaleData()
                {
                    id = 988,
                    mes = 10,
                    ano = 2017,
                    anterior = 1036,
                    siguiente = 1076,
                    unidades = 1094
                },
                new SaleData()
                {
                    id = 988,
                    mes = 11,
                    ano = 2017,
                    anterior = 1094,
                    unidades = 1076
                }
            };
        }
    }
}
