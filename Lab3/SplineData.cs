using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{


    internal class SplineData
    {
        public V1DataArray data_arr { get; set; }
        public int max_iterations { get; set; }
        public int iterations { get; set; }
        public int m_nodes { get; set; }
        public double[] NewValues { get; set; }
        public int info { get; set; }
        public double min_discrepancy { get; set; }

        private int len;
        public List<SplineDataItem> calculatedSpline { get; set; }




        public SplineData(V1DataArray dat_arr, int m, int max_it)
        {
            max_iterations = max_it;
            m_nodes = m;
            this.data_arr = dat_arr;
            len = data_arr.Xdata.Length;
            calculatedSpline = new List<SplineDataItem>();
        }


        public string ToLongString(string format)
        {
            var arr_str = new StringBuilder();
            ///var calculated_data_str = new StringBuilder();
            

            arr_str.AppendLine($" \nCoordinate StartValue NewValue");
            for (int i = 0; i < len; ++i)
            {
                arr_str.AppendLine($"{calculatedSpline[i]}");
            }



            return $" {arr_str} \n" +
                $"min discrepancy: {min_discrepancy.ToString(format)} \ninfo stop: {info} \n" +
                $"n iterations: {iterations} \n";
        }


        public bool Save(string filename = "saved_spline", string format = "0.##")
        {

            string buf_str = this.ToLongString("0.##");
            StreamWriter fs = null;
            try
            {
                fs = new StreamWriter(filename);
                fs.WriteLine(buf_str);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Cannot open file");
                Console.WriteLine(e.Message);
                return false;
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("File is closed");
                Console.WriteLine(e.Message);
                if (fs != null) fs.Close();
                return false;
            }
            catch (EncoderFallbackException e)
            {
                Console.WriteLine("Problems with output");
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                if (fs != null) fs.Close();
            }

            return true;

        }
        [DllImport("C:\\inf_c#\\Lab3\\x64\\Debug\\Dll.dll",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern
        void calculateSpline2(int nX, double[] Xs, ///вектор иксов
            double[] Ys, double[] Y1s,///вектор значений поля
            int Nnodes, ///количество точек
            ///нормально распределенные точки
            int MaxIterations, ref int NumberOfCalculatedIterations,
            ref double ResFinal, ref int Code); ///информация о работе вычислений

        public void CalcSpline()
        {
            var Min = data_arr.xMinItem;
            var Max = data_arr.xMaxItem;
            var lBorder = Min.x;
            var rBorder = Max.x;
           
            double[] NonUniformGridValues = new double[data_arr.Xdata.Length];
            double step = (rBorder - lBorder) / (m_nodes - 1);
            
            double[] UniformValues = null;

            

            int NIterations = 0;
            double Disperency = 10;
            int ErrorCode = 0;
            double[] X = data_arr.Xdata;
            for (int i = 0; i < data_arr.Xdata.Length; ++i)
            {
                ///Console.Write(X[i]);    
            }

            
            
            calculateSpline2(data_arr.Xdata.Length, X, data_arr.Ydata[0], NonUniformGridValues,
                m_nodes,
                max_iterations, ref NIterations, ref Disperency, ref ErrorCode);

            
            iterations = NIterations;
            min_discrepancy = Disperency;
            if (info != 0)
            {
                Console.WriteLine($"Stop reason number: {info}");
            }

            for (int i = 0; i < data_arr.Xdata.Length; i++)
            {
                calculatedSpline.Add(new SplineDataItem(data_arr.Xdata[i], data_arr.Ydata[0][i], NonUniformGridValues[i]));
            }
            NewValues = NonUniformGridValues;

            


        }

        [DllImport("C:\\inf_c#\\Lab3\\x64\\Debug\\Dll.dll",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void test(int LenGrid, double[] GridSpots, 
            double[] GridValues, double[] NewValues, 
            int GridLen);

        public void Try(int M, double[] Vals)
        {
            Console.WriteLine("Try");
            double[] NewValues = new double[data_arr.Xdata.Length];
            test(data_arr.Xdata.Length, data_arr.Xdata, NewValues, Vals , M);
        }





    }
}
