using log4net;
using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.DB;
using VMFW.DB.Service.ServiceImpl;
using VMFW.MySqlEntity;
using VMFW.Helper;

namespace VMFW.Operate
{
    public class ReceDataOptor
    {
        //记录一个组内在一段时间间隔内的点信息
        private List<GroupPt> pointsInfo;
        private int upPressureIndex;
        private int downPressureIndex;
        private int yearIndex;
        private int coIndex;
        private ILog _log;
        //private int bdIndex;
        private AcPointInfoService service = new AcPointInfoService();
        public ReceDataOptor(ILog log)
        {
            upPressureIndex = ClassPropertyHelper.GetPropertyIndex(typeof(PointTB), "P1") - 1;/*Convert.ToInt32(ConfigurationManager.AppSettings.Get("UpPressureIndex"));*/
            downPressureIndex = upPressureIndex + 1;/*Convert.ToInt32(ConfigurationManager.AppSettings.Get("DownPressureIndex"));*/
            yearIndex = ClassPropertyHelper.GetPropertyIndex(typeof(PointTB), "year") - 1;//Convert.ToInt32(ConfigurationManager.AppSettings.Get("YearIndex"));
            coIndex = ClassPropertyHelper.GetPropertyIndex(typeof(PointTB), "co") - 1;//Convert.ToInt32(ConfigurationManager.AppSettings.Get("CoIndex"));
            //bdIndex = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CoIndex"));
            pointsInfo = new List<GroupPt>();
            service = new AcPointInfoService();
            this._log = log;
        }

        /// <summary>
        /// 将opc采集到的点的信息转为需要进行计算的点信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Point OpcItemToPt(OPCItem item)
        {
            Point pt = new Point(item.ItemID, item.Parent.Name, item.ClientHandle);
            pt.Value = item.Value;
            pt.TimeStamp = item.TimeStamp;
            pt.Qulity = item.Quality;
            return pt;
        }


        /// <summary>
        /// 记录读取到的数据，若该组数据记录达到记录间隔，则开始进行产量计算，并将结果写入opcclient，同时将该组数据若干记录全部写入数据库，并清空记录集合。
        /// </summary>
        /// <param name="group">读取到数据的组</param>
        /// <param name="wGroup">需要写入数据组</param>
        /// <param name="opcWItems">写入数据组对应的opcitem</param>
        /// <param name="interval">间隔</param>
        public void AddPoint(OPCGroup rGroup, List<int> rServerHandles, OPCGroup wGroup, List<int> wServerHandles, DateTime recDt, int interval)
        {
            #region //将订阅到的数据点的值存起来
            List<Point> pts = new List<Point>();
            foreach (var serverHandle in rServerHandles)
            {
                Point pt = OpcItemToPt(rGroup.OPCItems.GetOPCItem(serverHandle));
                pts.Add(pt);
            }
            GroupPt gpt = new GroupPt(pts);
            gpt.recDt = recDt;

            pointsInfo.Add(gpt);
            #endregion

            //存的次数达到指定值
            if (pointsInfo.Count == interval)
            {
                #region //取该段间隔内的压力值，方便后续计算均值作为压力计算
                string p1s = string.Empty;
                string p2s = string.Empty;
                foreach (var gt in pointsInfo)
                {
                    #region 计算需要的若干个压力数据
                    p1s = $"{p1s}{gt.GetDoubleValue(upPressureIndex)},";
                    p2s = $"{p2s}{gt.GetDoubleValue(downPressureIndex)},";
                    #endregion
                    //gt.Print();
                }
                p1s = p1s.Substring(0, p1s.Length - 1);//去掉最后一个逗号
                p2s = p2s.Substring(0, p2s.Length - 1);//去掉最后一个逗号
                #endregion

                #region //获取标定系数
                double a1, a2, a3, a4;
                GetBDCoef(gpt, wGroup, out a1, out a2, out a3, out a4);
                #endregion

                #region //开始计算流量,并获取需要写入opc的值
                var output = Compute.Do(gpt, p1s, p2s, a1, a2, a3, a4);
                var type = output.GetType();
                var properties = type.GetProperties();
                List<object> values = new List<object>();//写入opc的值
                foreach (var property in properties)
                {
                    var value = property.GetValue(output);
                    values.Add(value);
                }
                #endregion

                #region //写入opc
                //Array errors;
                //int cancelId;
                //var transctionId = (new Random()).Next(0, 100);
                //Array handlesArray = wServerHandles.ToArray();
                //Array valueArray = values.ToArray();
                //wGroup.AsyncWrite(wServerHandles.Count, ref handlesArray, ref valueArray, out errors, transctionId, out cancelId);
                #endregion

                #region //写入数据库

                var pList = service.FillIntervalPInfo(pointsInfo, values);
                if (!service.BulkInsert(pList))
                {
                    this._log.Info("采集数据插入数据库失败！");
                }

                #endregion

                #region//清空已记录数据,方便下一个间隔内记录数据
                pointsInfo.Clear();
                #endregion 
            }
        }

        /// <summary>
        /// 获取标定系数
        /// </summary>
        private void GetBDCoef(GroupPt gpt, OPCGroup wGroup, out double a1, out double a2, out double a3, out double a4)
        {
            var year = gpt.GetIntValue(yearIndex);
            var month = gpt.GetIntValue(yearIndex + 1);
            var day = gpt.GetIntValue(yearIndex + 2);

            #region//计算标定系数
            //获取数据库中最大的标定时间
            DateTime? dt = service.GetMaxBdDate();
            if (!dt.HasValue)//如果当前还没有记录
            {
                a1 = 1;
                a2 = 1;
                a3 = 1;
                a4 = 1;
            }
            else if (dt.Value.Year.Equals(year) && dt.Value.Month.Equals(month) && dt.Value.Day.Equals(day))//如果与当前时间相同，则直接获取该条记录的标定系数
            {
                var acpt = service.GetOneRecord(dt.Value);
                a1 = acpt.a1.Value;
                a2 = acpt.a2.Value;
                a3 = acpt.a3.Value;
                a4 = acpt.a4.Value;
            }
            else//表明是新录入的标定数据，需要重新进行计算标定系数
            {
                var output = service.GetAverageOutput(dt.Value);
                a1 = gpt.GetIntValue(coIndex) / output.Item1;
                a2 = gpt.GetIntValue(coIndex + 1) / output.Item2;
                a3 = gpt.GetIntValue(coIndex + 2) / output.Item3;
                a4 = gpt.GetIntValue(coIndex + 3) / output.Item4;
            }
            #endregion
        }
    }
}
