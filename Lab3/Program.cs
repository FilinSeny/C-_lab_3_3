using Lab3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Lab3
{
    internal class Program
    {
        static double[] CountField(double x)
        {
            var res = new double[2];
            res[0] = x;
            res[1] = 2 * x * x;

            return res;
        }


        static double[] CountField2(double x)
        {
            var res = new double[2];
            res[0] = x * 2;
            res[1] = x * x * x;

            return res;
        }

        static double[] CounField3(double x)
        {
            var res = new double[2];
            res[0] = x * 3 / 5;
            res[1] = x / 5 * 4;
            return res;
        }

        static double[] CounField4(double x)
        {
            var res = new double[2];
            res[0] = x * x;
            res[1] = 0;
            return res;
        }

        static void Task_1()
        {
            var x = new double[4];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = (double)i * 2.0;
            }
            var l1 = new V1DataList("Task1", DateTime.Now, x, CountField);
            Console.WriteLine(l1.ToLongString("0.##"));
            var a1 = (V1DataArray)l1;
            Console.WriteLine(a1.ToLongString("0.##"));

        }


        static void Task_2()
        {
            var x = new double[4];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = (double)i * 1.5;
            }

            V1DataArray Ar1 = new V1DataArray("Task_2", DateTime.Now, x, CountField2);
            Console.WriteLine(Ar1.data_list.ToLongString("0.##"));
        }


        static void Task_3()
        {
            V1MainCollection Col1 = new V1MainCollection(2, 2);
            Console.WriteLine(Col1.ToLongString("0.###"));
        }

        static void Task_4()
        {
            V1MainCollection Col1 = new V1MainCollection(1, 2);
            Console.WriteLine(Col1.ToLongString("0.###"));
            foreach (var x in Col1)
            {
                Console.WriteLine(x.MaxDistance);
            }
        }


        static void Task_5()
        {
            V1MainCollection Col1 = new V1MainCollection(1, 2);
            Console.WriteLine(Col1.ToLongString("0.###"));
            var xMaxL = Col1.xMaxItem;

            foreach (var x in xMaxL)
            {
                Console.WriteLine(x.ToString());
            }
        }


        static void Task_6(string filename = "Storefile")
        {
            var x = new double[4];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = (double)i * 1.5;
            }

            V1DataArray Ar1 = new V1DataArray("Task_6", DateTime.Now, x, CountField2);
            Console.WriteLine(Ar1.ToLongString("0.###"));

            var fl = V1DataArray.Save(filename, Ar1); ;
            if (fl)
            {
                Console.WriteLine("Saved");
            }
        }

        static void Task_7(string filename = "Storefile")
        {
            V1DataArray Ar1 = new V1DataArray("", new DateTime());
            var fl = V1DataArray.Load(filename, ref Ar1);

            if (fl)
            {
                Console.WriteLine("Loaded");
                Console.WriteLine(Ar1.ToLongString("0.##"));
            }

        }

        static void task_8()
        {
            V1MainCollection Col1 = new V1MainCollection(0, 0);
            var x = new double[] { 1, 2, 3 };
            Col1.Add(new V1DataArray("Arr1", DateTime.Now, x, CounField3));
            x = new double[] { 1, 3, 4, 8 };
            Col1.Add(new V1DataArray("Arr2", DateTime.Now, x, CounField3));
            x = new double[] { 3, 4, 5 };
            Col1.Add(new V1DataList("List1", DateTime.Now, x, CounField3));
            x = new double[] { 4, 5, 6, 7 };
            Col1.Add(new V1DataList("List2", DateTime.Now, x, CounField3));
            Col1.Add(new V1DataArray("NullArr", DateTime.Now));
            Col1.Add(new V1DataList("NullList", DateTime.Now));
            Console.WriteLine(Col1.ToLongString("0.###"));

            Console.WriteLine("Средние значения векторов");
            Console.WriteLine(Col1.AverageAbs);

            Console.WriteLine("Максимальное отклонение у точки");
            Console.WriteLine(Col1.MaxDist.ToString());

            Console.WriteLine("Неединственные элементы:");
            foreach (var el in Col1.TakeTwiceElem)
            {
                Console.WriteLine(el.ToString());
            }

        }


        static void task_9(string filename = "SplineInfo.txt")
        {
            double[] x = new double[5];
            x[0] = 1;
            for (int i = 1; i < x.Length; i++)
            {
                x[i] = i + 1;
            }

            V1DataArray Arr1 = new V1DataArray("array", DateTime.Now, x, CounField3);

            SplineData Spline = new SplineData(Arr1, 5, 1000);
            
            Spline.CalcSpline();


            Console.WriteLine(Spline.ToLongString("0.##"));
            Spline.Save(filename);
            Console.WriteLine("Saved");

        }

        static void Main(string[] args)
        {
            ///Task_1();
            ///Task_2();
            /// Task_3();
            //Task_4();
            ///Task_5();
            ///Task_6();
            ///Task_7();
            ///task_8();
            task_9();
        }
    }
}
