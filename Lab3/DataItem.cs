using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    //struct DataItem для хранения данных, связанных с одной точкой;
    struct DataItem
    {
        public double x { get; set; }
        // типа double с координатой x точки, в которой измерено поле;
        public double[] y { get; set; }
        //два свойства типа double для двух значений поля { y1 , y2 } в этой точке.

        //конструктор DataItem (double x, double y1, double y2) для инициализации данных
        //структуры;
        public DataItem(double x, double y1, double y2)
        {
            this.x = x;
            this.y = new double[] { y1, y2 };

        }

        /*метод string ToLongString(string format), возвращающий строку, которая содержит
        значение координаты точки измерения и значения поля; параметр format задает
        формат вывода чисел с плавающей запятой;*/

        public string ToLongStrting(string format)
        {
            return $"{x.ToString(format)} {y[0].ToString(format)} {y[1].ToString(format)}";
        }

        public override string ToString()
        {
            return $"{x} {y[0]} {y[1]}";
        }


        public double Abs() { return Math.Sqrt(y[0] * y[0] + y[1] * y[1]); }
    }



    /// <summary>
    /// массивах, в классе V1DataList данные измерений хранятся в коллекции
    ///List<DataItem>;
    /// </summary>

    /*класс V1MainCollection для коллекции объектов типа V1DataList и V1DataArray;*/


    /*делегат void FValues(double x, ref double y1, ref double y2);*/

    /*делегат DataItem FDI (double x);*/
}
