using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using System.CodeDom;

namespace Lab3
{

    internal class V1DataArray : V1Data
    {
        public delegate double[] FValues(double x);
        public double[] Xdata { get; set; }
        public double[][] Ydata { get; set; }

        private int pos = 0;
        private int GetEnumPos = 0;

        public FValues Transform { get; set; }

        public V1DataArray(string key, DateTime date) : base(key, date)
        {
            Xdata = Array.Empty<double>();
            Ydata = new double[2][];
            Ydata[0] = Array.Empty<double>();
            Ydata[1] = Array.Empty<double>();
        }

        public V1DataArray(string key, DateTime date, double[] x, FValues F) : base(key, date)
        {
            Transform = F;
            Xdata = new double[x.Length];
            //x = x.Distinct().ToArray();

            Array.Copy(x, Xdata, x.Length);

            Ydata = new double[2][];
            Ydata[0] = new double[x.Length];
            Ydata[1] = new double[x.Length];


            for (int i = 0; i < Xdata.Length; i++)
            {
                var y_x = Transform(Xdata[i]);
                Ydata[0][i] = y_x[0];
                Ydata[1][i] = y_x[1];
            }
        }

        public V1DataArray(string key, DateTime date, int nX, double xL, double xR, FValues F) :
            base(key, date)
        {
            Transform = F;
            Xdata = new double[nX];

            Ydata = new double[2][];
            Ydata[0] = new double[nX];
            Ydata[1] = new double[nX];

            double step = (xR - xL) / (nX - 1);
            for (int i = 0; i < nX; ++i)
            {

                var xM = xR + i * step;
                Xdata[i] = xM;

                var Yi = Transform(Xdata[i]);
                Ydata[0][i] = Yi[0];
                Ydata[1][i] += Yi[1];
            }
        }

        public double[] this[int index]
        {
            //мб выдавать ошибку при index != 0, 1?
            get => Ydata[index];
        }


        public V1DataList data_list
        {
            get { return new V1DataList(key, time, Xdata, Ydata); }
        }

        public override double MaxDistance
        {
            get
            {
                var Allx = new double[Xdata.Length];
                Array.Copy(Xdata, Allx, Xdata.Length);

                Array.Sort(Allx);

                return Allx[Allx.Length - 1] - Allx[0];
            }


        }

        public override string ToString()
        {
            return $"V1DataArray \n {base.ToString()}";
        }

        public override string ToLongString(string format)
        {
            var tmp_str = new StringBuilder();
            for (int i = 0; i < Xdata.Length; ++i)
            {
                tmp_str.Append($"{Xdata[i].ToString(format)} {Ydata[0][i]} {Ydata[1][i]} \n");
            }
            return $"V1DataArray {base.ToString()} \n{tmp_str}";
        }

        public override DataItem xMaxItem
        {
            get
            {
                if (Xdata.Length <= 0) { return new DataItem(-1, -1, -1); }
                var xMax = Xdata[0];
                var iMax = 0;
                for (int i = 0; i < Xdata.Length; ++i)
                {
                    if (Xdata[i] > xMax)
                    {
                        iMax = i;
                        xMax = Xdata[i];
                    }
                }
                return new DataItem(xMax, Ydata[0][iMax], Ydata[1][iMax]);
            }
        }


        public override DataItem xMinItem
        {
            get
            {
                if (Xdata.Length <= 0) { return new DataItem(-1, -1, -1); }
                var xMax = Xdata[0];
                var iMax = 0;
                for (int i = 0; i < Xdata.Length; ++i)
                {
                    if (Xdata[i] < xMax)
                    {
                        iMax = i;
                        xMax = Xdata[i];
                    }
                }
                return new DataItem(xMax, Ydata[0][iMax], Ydata[1][iMax]);
            }
        }


        /*public IEnumerator<DataItem> Current
        {
            get
            {
                if (pos < 0 || pos > Xdata.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                var res = new DataItem(Xdata[pos], Ydata[0][pos], Ydata[1][pos]);
                return res.;
            }
        }

        public bool MoveNext()
        {
            if (pos < -1 || pos > Xdata.Length)
            {
                return false;
            }
            if (pos < Xdata.Length - 1)
            {
                pos++;
                return true;
            }

            return false;
        }

        public void Reset() =>  pos = 0;*/


        public override IEnumerator<DataItem> GetEnumerator()
        {
            var list = new List<DataItem>();
            for (int i = 0; i < Xdata.Count(); ++i)
            {
                list.Add(new DataItem(Xdata[i], Ydata[0][i], Ydata[1][i]));
            }

            return list.GetEnumerator();

        }



        public static bool Save(string filename, V1DataArray v1Arr)
        {
            /*Метод Save сохраняет все данные объекта (в том числе данные из базового класса) в файле с
            именем filename. Метод Load восстанавливает все данные объекта из файла с именем
            filename. Для сохранения/восстановления объекта типа V1DataArray можно использовать
            JSON-сериализацию или методы для записи/чтения из классов BinaryWriter/BinaryReader
            или StreamWriter/ StreamReader.
            Коды, которые сохраняют данные в файле, читают данные из файла и преобразуют их в
            объекты соответствующего типа, должны находиться в блоке try-catch-finally и обрабатывать
            исключения, которые могут быть брошены при записи и чтении из файла.
            */
            string BufStr = v1Arr.ToLongString("0.##");
            StreamWriter fs = null;
            try
            {
                fs = new StreamWriter(filename);
                fs.WriteLine(BufStr);
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

        public static bool Load(string filename, ref V1DataArray v1Arr)
        {
            StreamReader InFs = null;
            try
            {
                InFs = new StreamReader(filename);
                string s = InFs.ReadLine();
                string[] strings = s.Split(' ');
                var key = strings[1];
                var dateStr = strings[2];
                var dateStrSplited = dateStr.Split('.');
                var date = int.Parse(dateStrSplited[0]);
                var month = int.Parse(dateStrSplited[1]);
                var year = int.Parse(dateStrSplited[2]);
                var timeStr = strings[3];
                var timeStrSplited = timeStr.Split(':');
                var hour = int.Parse(timeStrSplited[0]);
                var min = int.Parse((timeStrSplited[1]));
                var sec = int.Parse(timeStrSplited[2]);
                ///var DataList = new V1DataList(key, new DateTime(year, month, date, hour, min, sec)); 
                var ListX = new List<double>();
                var ListY1 = new List<double>();
                var ListY0 = new List<double>();
                s = InFs.ReadLine();
                while (s != "")
                {
                    if (s == "") continue;
                    strings = s.Split(' ');
                    String spotStr = strings[0];
                    String FirstVal = strings[1];
                    String LastVal = strings[2];
                    var x = double.Parse(spotStr);

                    var y0 = double.Parse(FirstVal);
                    var y1 = double.Parse(LastVal);
                    ///DataList.myAdd(new  DataItem(x, y1, y2));
                    ListX.Add(x);

                    ListY0.Add(y0);
                    ListY1.Add(y1);
                    s = InFs.ReadLine();
                }

                var ListY = new double[2][];
                ListY[0] = ListY0.ToArray();
                ListY[1] = ListY1.ToArray();

                var DataList = new V1DataList(key, new DateTime(year, month, date, hour, min, sec), ListX.ToArray(), ListY.ToArray());
                v1Arr = (V1DataArray)DataList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally { if (InFs != null) InFs.Close(); }

            return true;
        }


    }
}
