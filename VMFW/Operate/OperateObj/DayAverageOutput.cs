using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.Operate.OperateObj
{
    /// <summary>
    /// 日平均产量
    /// </summary>
    public class DayAverageOutput
    {
        public DayAverageOutput()
        {
            EDol = 0.0;
            EDwl = 0.0;
            EDgl = 0.0;
            EDl = 0.0;
            EDolI = 0.0;
            EDwlI = 0.0;
            EDglI = 0.0;
            EDlI = 0.0;
        }

        #region 国内单位
        //每天平均产油量
        public double? EDol { get; set; }

        //每天平均产水量
        public double? EDwl { get; set; }

        //每天平均产气量
        public double? EDgl { get; set; }

        //每天平均产液量
        public double? EDl { get; set; }

        #endregion 国内单位
        //每天平均产油量
        public double? EDolI { get; set; }

        //每天平均产水量
        public double? EDwlI { get; set; }

        //每天平均产气量
        public double? EDglI { get; set; }

        //每天平均产液量
        public double? EDlI { get; set; }
        #region 国际单位
        #endregion

        public void SetOutput( double ol, double wl, double gl,double l,  double oli, double wli,double gli, double li)
        {
            EDol = ol;
            EDwl = wl;
            EDgl = gl;
            EDl = l;
            EDolI = oli;
            EDwlI = wli;
            EDglI = gli;
            EDlI = li;
        }

        public void SetOutput(DayAverageOutput output)
        {
            EDol = output.EDol;
            EDwl = output.EDwl;
            EDgl = output.EDgl;
            EDl = output.EDl;
            EDolI = output.EDolI;
            EDwlI = output.EDwlI;
            EDglI = output.EDglI;
            EDlI = output.EDlI;
        }
    }
}
