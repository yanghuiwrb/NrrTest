using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.Algorithm;
using VMFW.Helper;

namespace VMFW.Operate
{
    /// <summary>
    /// 调用算法计算产量
    /// </summary>
    public class Compute
    {
        //private static int inputNum = Convert.ToInt32(ConfigurationManager.AppSettings.Get("InputParamNum"));
        //private static int outputIndex = Convert.ToInt32(ConfigurationManager.AppSettings.Get("OutPutIndex"));

        /// <summary>
        /// 根据采集的点表数据，进行产量计算,并返回混合液体的密度，油的密度，水的密度
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="p1s"></param>
        /// <param name="p2s"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="a3"></param>
        /// <param name="a4"></param>
        /// <returns></returns>
        public static OutputParam Do(GroupPt pts, string p1s, string p2s, double a1, double a2, double a3, double a4)
        {
            try
            {
                DPMAlgorithm dpm = new DPMAlgorithm();
                int index = 0;
                dpm.SetFixParam(pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++));
                dpm.SetStaticParam(pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++));
                index += 5;//跳过年月日和上下游压力
                dpm.SetInternalIPParam(pts.GetDoubleValue(index++), pts.GetDoubleValue(index++), pts.GetDoubleValue(index++));
                //index += 7;//跳过中间计算参数
                dpm.SetBdCoef(a1, a2, a3, a4);
                dpm.SetRealParam(p1s, p2s);
                dpm.ComputeOutput();
                return dpm.GetOutput();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"产量运算出错，{ex.Message}");
                return new OutputParam();
            }
        }
    }
}
