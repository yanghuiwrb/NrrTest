using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.Algorithm
{
    public class OutputParam
    {
        //混合液体密度
        public double rl { get; set; }

        //混合物密度
        public double r { get; set; }

        //质量含气率
        public double x { get; set; }

        //混合液体粘度
        public double ul { get; set; }

        //混合物粘度
        public double u { get; set; }

        //可膨胀系数
        public double E { get; set; }

        //油嘴流量系数
        public double C { get; set; }

        //产油量标定系数
        public double a1 { get; set; }

        //产水量标定系数
        public double a2 { get; set; }

        //产气量标定系数
        public double a3 { get; set; }

        //产液量标定系数
        public double a4 { get; set; }

        //油井最终产油量(单位为吨)
        public double QO { get; set; }

        //油井最终产水量(单位为吨)
        public double QW { get; set; }

        //油井最终产气量(单位为吨)
        public double QG { get; set; }

        //油井最终产液量(单位为吨)
        public double QL { get; set; }

        //油井最终产油量(单位为国际单位)
        public double QOI { get; set; }

        //油井最终产水量(单位为国际单位)
        public double QWI { get; set; }

        //油井最终产气量(单位为国际单位)
        public double QGI { get; set; }

        //油井最终产液量(单位为国际单位)
        public double QLI { get; set; }

        public override string ToString()
        {
            return "OutputParam{" +
                    //", Ql=" + Ql +
                    //", Qol=" + Qol +
                    //", Qwl=" + Qwl +
                    //", Qgl=" + Qgl +
                    ", QL=" + QL +
                    ", QO=" + QO +
                    ", QW=" + QW +
                    ", QG=" + QG +
                    '}';
        }

    }
}
