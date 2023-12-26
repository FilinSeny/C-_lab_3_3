using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    internal struct SplineDataItem
    {
        public double x { get; set; }
        public double y { get; set; }
        public double calculated_y { get; set; }

        public SplineDataItem(double _x, double _y, double calced)
        {
            x = _x;
            y = _y;
            calculated_y = calced;
        }

        public string ToString(string format)
        {
            return $"x = {x.ToString(format)} y = {y.ToString(format)}, calculated_y = {calculated_y.ToString(format)}";
        }

        public override string ToString()
        {
            return $"{x.ToString()} {y.ToString()} {calculated_y.ToString()}";
        }

    }
}
