using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    /// <summary>
    //абстрактный базовый класс V1Data
    /// </summary>
    abstract class V1Data : IEnumerable<DataItem>
    {
        /*два автореализуемых свойства типа string и DateTime; свойство типа string можно
        трактовать как ключ объекта;*/
        public string key { get; set; }
        public DateTime time { get; set; }

        public abstract double MaxDistance { get; }

        //конструктор с параметрами типа string и DateTime;
        public V1Data(string key, DateTime time)
        {
            this.key = key;
            this.time = time;
        }

        //абстрактный метод string ToLongString(string format);
        public abstract string ToLongString(string format);

        public override string ToString()
        {
            return $"{key} {time}";
        }

        public abstract DataItem xMaxItem { get; }
        public abstract DataItem xMinItem { get; }

        public abstract IEnumerator<DataItem> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}
