using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainModel_IAERP.DataStructures
{
    public class SaleData
    {
        [LoadColumn(0)]
        public float siguiente;

        [LoadColumn(1)]
        public float id;

        [LoadColumn(2)]
        public float ano;

        [LoadColumn(3)]
        public float mes;

        [LoadColumn(4)]
        public float unidades;

        [LoadColumn(6)]
        public float anterior;

        public override string ToString()
        {
            return $"ProductData [ id: {id}, mes: {mes:00}, unidades: {unidades:0000}, siguiente: {siguiente}, anterior: {anterior}]";
            //descripcion: {descripcion}
        }
    }
}
