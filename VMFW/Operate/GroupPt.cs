using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMFW.Operate
{
    public class GroupPt
    {
        //当前组一次性接收的所有数据
        private List<Point> _pts;
        //当前组接收数据的时间
        private DateTime _recDt;
        //当前接收的这组数据对应的井或者rtu的名称
        private string _name;
        public GroupPt()
        {
            this._pts = new List<Point>();
        }

        public GroupPt(List<Point> pts)
        {
            this._pts = pts;
        }

        public DateTime recDt
        {
            get { return this._recDt; }
            set { this._recDt = value; }
        }

        public void Print()
        {
            foreach (var Point in _pts)
            {
                if (Point.Value != null)
                    Console.WriteLine(Point.TimeStamp.ToString());
            }
        }

        public Point GetPoint(int index)
        {
            return _pts[index];
        }

        public object GetPointValue(int index)
        {
            return _pts[index];
        }

        public double GetDoubleValue(int index)
        {
            return Convert.ToDouble(GetPoint(index).Value);
        }


        public double? GetNullableDoubleValue(int index)
        {
            if (GetPoint(index) == null)
            {
                return null;
            }
            return Convert.ToDouble(GetPoint(index).Value);
        }

        public int GetIntValue(int index)
        {
            return Convert.ToInt32(GetPoint(index).Value);
        }

        public void SetDoubleValue(int index, double value)
        {
            GetPoint(index).Value = value;
        }

        public void SetIntValue(int index, int value)
        {
            GetPoint(index).Value = value;
        }

        public string WellName { get { return _name; }set { _name = value; } }

        public int GetCount()
        {
            return _pts.Count;
        }
    }
}
