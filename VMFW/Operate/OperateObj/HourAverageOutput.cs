using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.Operate.OperateObj
{
    /// <summary>
    /// 小时平均产量
    /// </summary>
    public class HourAverageOutput
    {
        public HourAverageOutput()
        {
            this.EHol = 0.0;
            this.EHwl = 0.0;
            this.EHgl = 0.0;
            this.EHl = 0.0;
            this.EHolI = 0.0;
            this.EHwlI = 0.0;
            this.EHglI = 0.0;
            this.EHlI = 0.0;
        }

        #region 国内单位
        //每小时平均产油量
        public double? EHol { get; set; }

        //每小时平均产水量
        public double? EHwl { get; set; }

        //每小时平均产气量
        public double? EHgl { get; set; }

        //每小时平均产液量
        public double? EHl { get; set; }
        #endregion 国内单位

        #region 国际单位
        //每小时平均产油量
        public double? EHolI { get; set; }

        //每小时平均产水量
        public double? EHwlI { get; set; }

        //每小时平均产气量
        public double? EHglI { get; set; }

        //每小时平均产液量
        public double? EHlI { get; set; }
        #endregion 国内单位

        public void SetOutput(double ol, double wl, double gl, double l, double oli, double wli, double gli, double li)
        {
            this.EHol = ol;
            this.EHwl = wl;
            this.EHgl = gl;
            this.EHl = l;
            this.EHolI = oli;
            this.EHwlI = wli;
            this.EHglI = gli;
            this.EHlI = li;
        }

        public void SetOutput(HourAverageOutput output)
        {
            this.EHol = output.EHol;
            this.EHwl = output.EHwl;
            this.EHgl = output.EHgl;
            this.EHl = output.EHl;
            this.EHolI = output.EHolI;
            this.EHwlI = output.EHwlI;
            this.EHglI = output.EHglI;
            this.EHlI = output.EHlI;
        }

        /// <summary>
        /// 设置国内单位的小时平均产量
        /// </summary>
        /// <param name="dic"></param>
        public void SetHOutput(Dictionary<string, double> dic)
        {
            var value = 0.0;
            dic.TryGetValue("ol", out value);
            this.EHol = value;
            dic.TryGetValue("wl", out value);
            this.EHwl = value;
            dic.TryGetValue("gl", out value);
            this.EHgl = value;
            dic.TryGetValue("l", out value);
            this.EHl = value;
        }

        /// <summary>
        /// 设置国际单位的小时平均产量
        /// </summary>
        /// <param name="dic"></param>
        public void SetIOutput(Dictionary<string, double> dic)
        {
            var value = 0.0;
            dic.TryGetValue("ol", out value);
            this.EHolI = value;
            dic.TryGetValue("wl", out value);
            this.EHwlI = value;
            dic.TryGetValue("gl", out value);
            this.EHglI = value;
            dic.TryGetValue("l", out value);
            this.EHlI = value;
        }

    }
}
