using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lab3
{

    internal class V1DataList : V1Data, IEnumerator
    {
        public delegate double[] FDI(double x);
        /// <summary>
        /// public FDI Transform { get; set; } 
        /// </summary>
        public List<DataItem> dataItems { get; set; }

        private int pos = 0;
        public double[] Points { get; set; } = Array.Empty<double>();

        /* конструктор V1DataList (string key, DateTime date)
        для инициализации данных базового класса; в этом конструкторе распределяется
        память для коллекции List<DataItem>; */

        public V1DataList(string key, DateTime date) : base(key, date)
        {
            dataItems = new List<DataItem>();
        }

        /*
         * конструктор
        V1DataList (string key, DateTime date, double[] x, FDI F),
        в который через параметр double[] x передается ссылка на массив с координатами, в
        которых измерено поле; для каждого элемента массива вызывается метод F, который
        вычисляет значения поля { y1 , y2 }; создаётся и добавляется в коллекцию
        List<DataItem> элемент DataItem; для равных элементов массива x в коллекцию
        добавляется только один элемент DataItem;
         */

        public V1DataList(string key, DateTime date, double[] x, FDI F) : this(key, date)
        {
            Points = x;
            for (int i = 0; i < x.Count(); ++i)
            {
                var tmp = F(x[i]);
                dataItems.Add(new DataItem(x[i], tmp[0], tmp[1]));
            }
            ///Transform = F;
        }

        public V1DataList(string key, DateTime date, double[] x, double[][] y) : this(key, date)
        {
            Points = x;
            for (int i = 0; i < x.Count(); ++i)
            {
                var tmp_1 = y[0][i];
                var tmp_2 = y[1][i];
                dataItems.Add(new DataItem(x[i], tmp_1, tmp_2));
            }

        }

        /*
         * реализацию абстрактного свойства MaxDistance типа double, которое возвращает
            максимальное расстояние между точками, в которых измерены значения поля (среди
            всех элементов List<DataItem>);
        */
        public override double MaxDistance
        {
            get
            {
                var All_Points = new double[dataItems.Count];

                for (int i = 0; i < dataItems.Count; ++i)
                {
                    All_Points[i] = dataItems[i].x;
                }

                Array.Sort(All_Points);
                return All_Points[dataItems.Count - 1] - All_Points[0];

            }
        }


        public static explicit operator V1DataArray(V1DataList source)
        {
            var res = new V1DataArray(source.key, source.time);
            res.Xdata = new double[source.dataItems.Count];
            Array.Copy(source.Points, res.Xdata, source.dataItems.Count);
            res.Ydata = new double[2][];
            res.Ydata[0] = new double[source.dataItems.Count];
            res.Ydata[1] = new double[source.dataItems.Count];
            for (int i = 0; i < source.dataItems.Count; ++i)
            {
                res.Ydata[0][i] = source.dataItems[i].y[0];
                res.Ydata[1][i] = source.dataItems[i].y[1];
            }
            return res;
        }

        public override string ToString()
        {
            return $"V1DataList {base.ToString()} {dataItems.Count}";
        }

        public override string ToLongString(string format)
        {
            var data_str = new StringBuilder();

            for (int i = 0; i < dataItems.Count; ++i)
            {
                data_str.Append($"{dataItems[i].ToLongStrting(format)}\n");
            }

            return $"{this}\n{data_str}";
        }


        public override DataItem xMaxItem
        {
            get
            {
                if (dataItems.Count == 0) return new DataItem(-1, -1, -1);

                int iMax = 0;
                var xMax = dataItems[0].x;

                for (int i = 0; i < dataItems.Count; ++i)
                {
                    if (dataItems[i].x > xMax)
                    {
                        xMax = dataItems[i].x;
                        iMax = i;
                    }
                }

                return dataItems[iMax];
            }
        }

        public override DataItem xMinItem
        {
            get
            {
                if (dataItems.Count == 0) return new DataItem(-1, -1, -1);

                int iMax = 0;
                var xMax = dataItems[0].x;

                for (int i = 0; i < dataItems.Count; ++i)
                {
                    if (dataItems[i].x < xMax)
                    {
                        xMax = dataItems[i].x;
                        iMax = i;
                    }
                }

                return dataItems[iMax];
            }
        }

        public object Current
        {
            get
            {
                if (pos < 0 || pos > dataItems.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return dataItems[pos];
            }
        }

        public bool MoveNext()
        {

            {
                if (pos < -1 || pos > dataItems.Count)
                {
                    return false;
                }

                ++pos;
                return true;
            }
        }

        public void Reset() => pos = 0;

        public override IEnumerator<DataItem> GetEnumerator()
        {
            return dataItems.GetEnumerator();
        }


    }
}
