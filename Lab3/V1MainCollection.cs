using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace Lab3
{
    internal class V1MainCollection : System.Collections.ObjectModel.ObservableCollection<V1Data>
    {
        public static double[] Func(double x)
        {
            var res = new double[2];
            res[0] = x;
            res[1] = x * x;
            return res;
        }
        public V1DataList DataList { get; set; }
        public V1DataArray DataArray { get; set; }


        public bool Contains(string key)
        {
            return Items.Any(data => data.key == key);
        }

        public V1MainCollection(int nV1DataArray, int nV1DataList)
        {
            for (int i = 0; i < nV1DataArray; i++)
            {
                var x_arr = new double[4];
                x_arr[0] = 2.0 + i;
                x_arr[1] = 2.0 + (1 + 0.5) * i;
                x_arr[2] = 2.0 + (2 + 0.5) * i;
                x_arr[3] = 0.5 + (3 + 0.5) + i;

                V1DataArray v1 = new V1DataArray("testA" + i, DateTime.Now, x_arr, Func);
                base.Add(v1);

            }

            for (int i = 0; i < nV1DataList; i++)
            {
                var x_arr = new double[3];
                x_arr[0] = 1.0 + i;
                x_arr[1] = 1.0 + (1 + 0.5) * i;
                x_arr[2] = 1.0 + (2 + 0.5) * i;


                V1DataList l1 = new V1DataList("testL" + i, DateTime.Now, x_arr, Func);
                base.Add(l1);
            }
        }

        public new bool Add(V1Data data)
        {
            if (Contains(data.key))
            {
                return false;
            }

            base.Add(data);

            return true;
        }

        public override string ToString()
        {
            var tmp_str = new StringBuilder();

            foreach (var el in Items)
            {
                tmp_str.Append($"{el.ToString()} \n");
            }

            return $"{tmp_str.ToString()}";
        }

        public string ToLongString(string format)
        {
            var tmp_str = new StringBuilder();

            foreach (var el in Items)
            {
                tmp_str.Append($"{el.ToLongString(format)} \n");
            }

            return $"{tmp_str.ToString()}";
        }


        public List<DataItem> xMaxItem
        {
            get
            {
                var list = new List<DataItem>();
                foreach (var el in Items)
                {
                    list.Add(el.xMaxItem);
                }

                return list;
            }
        }

        public double AverageAbs
        {
            get
            {
                if (this.Count == 0) return double.NaN;
                var Elems = from di_a in Items
                            from di in di_a
                            select di;
                return Elems.Average(el => el.Abs());
            }
        }
        ///для элементов(списков и массивов) посчитать растояния(в каждом элемнете для всех точек
        ///посчитать кв корни)
        ///
        public DataItem? MaxDist
        {
            get
            {
                if (this.Count == 0) return null;
                var Elems = from di_a in Items
                            from di in di_a
                            select di;
                var absAverange = this.AverageAbs;
                var maxDif = Elems.Max(el => Math.Abs(el.Abs() - absAverange));
                var res = from el in Elems
                          where Math.Abs(el.Abs() - absAverange) == maxDif
                          select el;
                return res.First();
            }
        }

        public IEnumerable<double> TakeTwiceElem
        {
            get
            {
                if (this.Count == 0) return null;
                var Xs = from coll in this
                         where coll.Count() > 0
                         from item in coll
                         select item.x;
                var SortedX = from x in Xs
                              orderby x
                              select x;
                var Dict = SortedX.GroupBy(x => x);
                var res = from x in Dict
                          where x.Count() > 1
                          select x.Key;

                return res;

            }
        }

    }
}
