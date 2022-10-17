using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using VMFW.DB.Service.ServiceImpl;
using VMFW.MySqlEntity;
using VMFW.Helper;
using VMFW.DB.Service.Iservice;
using VMFW.Entity.InfluxDBEntity;
using VMFW.Operate.OperateObj;
using VMFW.Algorithm;
using VMFW.Atribute;

namespace VMFW.Operate
{
    public class ReceDataOptor
    {
        private string _name;//rtu或者井的名称
        private OPCGroup _rGroup;//需要进行读取的OPCGroup
        private OPCGroup _wGroup;//需要进行写入的OPCGroup
        private List<int> _rServerHandles;//需要进行读取的item对应的ServerHandle;
        private List<int> _wServerHandles;//需要进行写入的item对应的ServerHandle;
        private int _unitServerHandle;//单位切换的OPCItem的ServerHandle
        private List<GroupPt> _pointsInfo; //记录一个组内在一段时间间隔内的点信息
        private int _upPressureIndex;//上游压力在订阅中的序号
        private int _downPressureIndex;//下游压力在订阅中的序号
        private int _yearIndex;//标定日期中的年在订阅中的序号(年，月，日顺序进行订阅)
        private int _coIndex;//第一个标定值在订阅中的序号（按照产油，产水，产气，产液顺序进行订阅）
        private AverageOutput _averageOutput; //记录该井场的平均数据（每小时平均，每天平均，定时操作存放的数据，每次从中获取即可）
        private IPointInfoService _service;//对点各种操作接口对象

        public ReceDataOptor(string name, OPCGroup rGroup, OPCGroup wGroup, List<int> rServerHandles, List<int> wServerHandles, int unitServerHandle)
        {
            _upPressureIndex = ClassPropertyHelper.GetPropertyIndex(typeof(PointTB), "P1") - 2;//wellname和unittype要排除
            _downPressureIndex = _upPressureIndex + 1;
            _yearIndex = ClassPropertyHelper.GetPropertyIndex(typeof(PointTB), "year") - 2;
            _coIndex = ClassPropertyHelper.GetPropertyIndex(typeof(PointTB), "co") - 2;
            _pointsInfo = new List<GroupPt>();
            _service = new PointInfoService();
            _averageOutput = new AverageOutput();

            this._name = name;
            this._rGroup = rGroup;
            this._wGroup = wGroup;
            this._rServerHandles = rServerHandles;
            this._wServerHandles = wServerHandles;
            this._unitServerHandle = unitServerHandle;
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
        /// 获取该井或者rtu的名称
        /// </summary>
        public string Name { get { return this._name; } set { this._name = value; } }

        /// <summary>
        /// 记录读取到的数据，若该组数据记录达到记录间隔，则开始进行产量计算，并将结果写入opcclient，同时将该组数据若干记录全部写入数据库，并清空记录集合。
        /// </summary>
        /// <param name="recDt">数据时间</param>
        /// <param name="interval">间隔</param>
        public async Task AddPoint(DateTime recDt, int interval)
        {
            #region //将订阅到的数据点的值存起来
            List<Point> pts = new List<Point>();
            foreach (var serverHandle in _rServerHandles)
            {
                Point pt = OpcItemToPt(_rGroup.OPCItems.GetOPCItem(serverHandle));
                pts.Add(pt);
            }
            GroupPt gpt = new GroupPt(pts);
            gpt.WellName = _name;
            gpt.recDt = recDt;

            _pointsInfo.Add(gpt);
            #endregion

            //存的次数达到指定值
            if (_pointsInfo.Count == interval || interval == 1)
            {
                try
                {
                    #region 虚拟计量，获取流量数据
                    var pointInfos = await GetVFMData(gpt);
                    #endregion

                    #region 数据插入数据库以及进行查询操作
                    await QueryInfo(pointInfos);
                    LogHelper.Info(gpt.WellName + "数据插入数据库以及进行查询操作完成");
                    #endregion

                    #region //写入opc
                    var values = GetOPCWriteValues(pointInfos.Last());
                    this.WriteToOPC(values);
                    LogHelper.Info(gpt.WellName + "写入opc完成");
                    #endregion
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message + " " + _name);
                }
                finally
                {
                    #region//清空已记录数据,方便下一个间隔内记录数据
                    _pointsInfo.Clear();
                    #endregion
                }
            }
        }

        /// <summary>
        /// 在指定的group中对应的指定的serverHandle中写入数据
        /// </summary>
        /// <param name="group"></param>
        /// <param name="serverHandles"></param>
        /// <param name="values"></param>
        public void WriteToOPC(List<object> values)
        {
            if (values.Count > 0)
            {
                var transctionId = (new Random()).Next(0, 100);
                Array handlesArray = (new List<int> { 0 }).Concat(_wServerHandles.Take(values.Count)).ToArray();
                Array valueArray = (new List<object> { "" }).Concat(values).ToArray();
                _wGroup.AsyncWrite(values.Count, ref handlesArray, ref valueArray, out Array errors, transctionId, out int cancelId);
            }
        }

        /// <summary>
        /// 获取标定系数,前4个系数为标定系数，最后一个参数为：标定系数是否为重新计算的标定系数。如果为重新计算的标定系数，则为true，否则为false.
        /// </summary>
        private async Task<(double, double, double, double)> GetBDCoef(GroupPt gpt)
        {
            double a1 = 1, a2 = 1, a3 = 1, a4 = 1;
            var year = gpt.GetIntValue(_yearIndex);
            var month = gpt.GetIntValue(_yearIndex + 1);
            var day = gpt.GetIntValue(_yearIndex + 2);
            //var wellName = gpt.WellName;

            #region//计算标定系数
            //获取数据库中最大的标定时间
            //DateTime? dt = service.GetMaxBdDate();
            try
            {
                if (year.Equals(0) || month.Equals(0) || day.Equals(0))
                {
                    return (a1, a2, a3, a4);
                }
                var pInfo = await _service.GetMaxBDRecord(_name);
                if (pInfo == null)//如果当前还没有记录，或者还没有标定过
                {
                    a1 = 1;
                    a2 = 1;
                    a3 = 1;
                    a4 = 1;
                }
                else if (pInfo.year.Equals(year) && pInfo.month.Equals(month) && pInfo.day.Equals(day))//如果与当前时间相同，则直接获取该条记录的标定系数
                {
                    var acpt = await _service.GetOneRecord(_name, year, month, day);
                    a1 = acpt.a1.Value;
                    a2 = acpt.a2.Value;
                    a3 = acpt.a3.Value;
                    a4 = acpt.a4.Value;
                }
                else//表明是新录入的标定数据，需要重新进行计算标定系数
                {
                    var uintType = _rGroup.OPCItems.GetOPCItem(_unitServerHandle).Value;
                    var output = new Dictionary<string, double>();
                    if (uintType == ((int)UintType.HomeType))//说明是国内单位,计算该日内的国内单位的日产量
                    {
                        output = await _service.ComputeHDayOutput(_name, year, month, day);
                    }
                    else if (uintType == ((int)UintType.InternalType))//说明是国际单位,计算该日内的国际单位的日产量
                    {
                        output = await _service.ComputeIDayOutput(_name, year, month, day);
                    }
                    if (output != null && output.Count > 0)
                    {
                        var value = 0.0;
                        a1 = output.TryGetValue("ol", out value) ? gpt.GetDoubleValue(_coIndex) / value : 1;
                        a2 = output.TryGetValue("gl", out value) ? gpt.GetDoubleValue(_coIndex + 1) / value : 1;
                        a3 = output.TryGetValue("wl", out value) ? gpt.GetDoubleValue(_coIndex + 2) / value : 1;
                        a4 = output.TryGetValue("l", out value) ? gpt.GetDoubleValue(_coIndex + 3) / value : 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"{_name}计算标定系数发生错误！{ex.Message}");
            }
            return (a1, a2, a3, a4);
            #endregion
        }

        /// <summary>
        /// 根据接收的数据，进行虚拟计量，计算实时流量
        /// </summary>
        /// <param name="gpt"></param>
        /// <param name="wGroup"></param>
        /// <returns></returns>
        private async Task<List<PointInfo>> GetVFMData(GroupPt gpt)
        {
            #region //取该段间隔内的压力值，方便后续计算均值作为压力计算
            string p1s = string.Empty;
            string p2s = string.Empty;
            this.UpDownPressureIntervalAverage(ref p1s, ref p2s);
            #endregion

            #region //获取标定系数
            var coef = await GetBDCoef(gpt);
            var a1 = coef.Item1;
            var a2 = coef.Item2;
            var a3 = coef.Item3;
            var a4 = coef.Item4;
            #endregion

            #region //开始计算流量
            var outputParam = Compute.Do(gpt, p1s, p2s, a1, a2, a3, a4);
            #endregion

            //将接收到的数据转为PointInfo，并用计算得来的数据进行填充
            return _service.FillIntervalPInfo(_pointsInfo, outputParam);
        }

        /// <summary>
        /// 对实时数据进行相应的查询操作
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private async Task QueryInfo(List<PointInfo> pList)
        {
            try
            {
                #region //将实时数据写入数据库
                await _service.WriteDatasAsync(pList);
                #endregion

                var pInfo = pList.Last();

                #region //计算实时日产量，并更新数据库中的日产量数据
                await _service.ComputeRealDayOutput(pInfo);
                await _service.WriteDataAsync(pInfo);
                #endregion

                #region //计算实时月产量，并更新数据库中的月产量数据
                await _service.ComputeRealMonthOutput(pInfo);
                await _service.WriteDataAsync(pInfo);
                #endregion

                #region //计算实时年产量，并更新数据库中的年产量数据
                await _service.ComputeRealYearOutput(pInfo);
                await _service.WriteDataAsync(pInfo);
                #endregion

                #region //计算累计产量，并更新数据库中的累计产量，以及小时平均和年平均
                await _service.ComputeTotalOutput(pInfo);
                this.UpdateAverageOutput(pInfo);
                await _service.WriteDataAsync(pInfo);
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error($"数据库查询操作出错,{ex.Message}");
            }
        }

        private List<object> GetOPCWriteValues(PointInfo pInfo)
        {
            var values = new List<object>();
            try
            {
                var unitType = _rGroup.OPCItems.GetOPCItem(_unitServerHandle).Value;
                if (pInfo == null)
                {
                    return null;
                }

                if (unitType == ((int)UintType.HomeType))
                {
                    values = GetOPCHWriteValues(pInfo);
                }
                else if (unitType == ((int)UintType.InternalType))
                {
                    values = GetOPCIWriteValues(pInfo);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"获取写入OPC的数据时，发生错误,{ex.Message}");
            }
            return values;
        }

        /// <summary>
        /// 获取需要写入opc的数据，单位为国内单位
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        private List<object> GetOPCHWriteValues(PointInfo pInfo)
        {
            var type = pInfo.GetType();
            var properties = type.GetProperties();
            var values = new List<object>();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(WriteOPCTypeAttribute));
                if (attributes.Count() > 0)
                {
                    var flag = ((WriteOPCTypeAttribute)(attributes.FirstOrDefault())).type;
                    if (flag == WriteType.Both || flag == WriteType.Home)
                    {
                        values.Add(property.GetValue(pInfo));
                    }
                };
            }
            return values;
        }

        /// <summary>
        /// 获取需要写入opc的数据，单位为国际单位
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        private List<object> GetOPCIWriteValues(PointInfo pInfo)
        {
            var type = pInfo.GetType();
            var properties = type.GetProperties();
            var values = new List<object>();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(WriteOPCTypeAttribute));
                if (attributes.Count() > 0)
                {
                    var flag = ((WriteOPCTypeAttribute)(attributes.FirstOrDefault())).type;
                    if (flag == WriteType.Both || flag == WriteType.Internal)
                    {
                        values.Add(property.GetValue(pInfo));
                    }
                };
            }
            return values;
        }

        private void UpDownPressureIntervalAverage(ref string p1s, ref string p2s)
        {
            foreach (var gt in _pointsInfo)
            {
                #region 计算需要的若干个压力数据
                p1s = $"{p1s}{gt.GetDoubleValue(_upPressureIndex)},";
                p2s = $"{p2s}{gt.GetDoubleValue(_downPressureIndex)},";
                #endregion
                //gt.Print();
            }
            p1s = p1s.Substring(0, p1s.Length - 1);//去掉最后一个逗号
            p2s = p2s.Substring(0, p2s.Length - 1);//去掉最后一个逗号
        }

        private void UpdateAverageOutput(PointInfo pInfo)
        {
            var daAverageOutput = _averageOutput.GetDayAverageOutput();
            pInfo.EDol = daAverageOutput.EDol;
            pInfo.EDwl = daAverageOutput.EDwl;
            pInfo.EDgl = daAverageOutput.EDgl;
            pInfo.EDl = daAverageOutput.EDl;
            pInfo.EDolI = daAverageOutput.EDolI;
            pInfo.EDwlI = daAverageOutput.EDwlI;
            pInfo.EDglI = daAverageOutput.EDglI;
            pInfo.EDlI = daAverageOutput.EDlI;

            var haAverageOutput = _averageOutput.GetHourAverageOutput();
            pInfo.EHol = haAverageOutput.EHol;
            pInfo.EHwl = haAverageOutput.EHwl;
            pInfo.EHgl = haAverageOutput.EHgl;
            pInfo.EHl = haAverageOutput.EHl;
            pInfo.EHolI = haAverageOutput.EHolI;
            pInfo.EHwlI = haAverageOutput.EHwlI;
            pInfo.EHglI = haAverageOutput.EHglI;
            pInfo.EHlI = haAverageOutput.EHlI;
        }

        public void SetAverageHourOutput(double ol, double wl, double gl, double l, double oli, double wli, double gli, double li)
        {
            _averageOutput.SetHourAverageOutput(ol, wl, gl, l, oli, wli, gli, li);
        }

        public void SetAverageDayOutput(double ol, double wl, double gl, double l, double oli, double wli, double gli, double li)
        {
            _averageOutput.SetDayAverageOutput(ol, wl, gl, l, oli, wli, gli, li);
        }

        public AverageOutput GetAverageHourOutput()
        {
            return _averageOutput;
        }
    }
}
