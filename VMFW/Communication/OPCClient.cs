using log4net;
using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VMFW.Operate;
using VMFW.MySqlEntity;
using VMFW.Atribute;
using VMFW.Helper;
using VMFW.Operate.OperateObj;

namespace VMFW.Communication
{
    /// <summary>
    /// 用于订阅读取数据的OPCGroup中的RTU数目的分配记录，考虑到rtuNum整除OPCGroupNum，不一定除的尽，为了提高效率，尽量将所有能添加的OPCGroup都用上，所以前面部分可能和后面部分分配的Rtunum数目不一样（差一个）
    /// </summary>
    class GroupRtuAllocation
    {
        //前面部分的OPCGroup数目
        public int bOPCGroupNum;

        //前面部分对应的每个OPCGroup中的Rtu数目
        public int bRtuNum;

        //后面部分的OPCGroup数目
        public int aOPCGroupNum;

        //后面部分对应的每个OPCGroup中的Rtu数目
        public int aRtuNum;
    }

    public class OPCClient
    {
        //private static ILog log = LogManager.GetLogger(typeof(VFMOperator));
        private OPCServer _opcServer;
        private int _updateRate;
        private int _computeInterval;
        private bool _bConnect;//与OPCServer服务器的连接状态，true为连接，false为断开
        private bool _bTime;//表示定时操作是否在执行中
        private int _MaxOPCGroupNum = 0;
        private string _wOPCGroupName = "write";

        //存放clienthandle和opcitem的映射关系，方便数据订阅到达时，知道是哪个ipcitem
        private Dictionary<int, OPCItem> _dics;

        //存放每个OPCGroup数据接收情况
        private Dictionary<OPCGroup, List<ReceDataOptor>> _rGroups;

        //存放读取OPCGroup,与写入OPCGroup的对应关系
        private Dictionary<OPCGroup, OPCGroup> _rwGroups;

        private TimeOperate _timeOpt;//定时任务操作

        private List<int> _unitServerHandles;

        public OPCClient()
        {
            _opcServer = null;
            _bConnect = false;
            _bTime = false;
            _updateRate = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ClientUpdateRate"));
            _computeInterval = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ComputeInterval"));
            _dics = new Dictionary<int, OPCItem>();
            _rGroups = new Dictionary<OPCGroup, List<ReceDataOptor>>();
            _rwGroups = new Dictionary<OPCGroup, OPCGroup>();
            _unitServerHandles = new List<int>();
            _MaxOPCGroupNum = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MaxGroupNum"));

            // 初始化定时操作
            _timeOpt = new TimeOperate();
            _timeOpt.Init();
        }

        /// <summary>
        /// 根据ip获取opc server的名称
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public List<string> GetServerName(string ip)
        {
            IPHostEntry host = Dns.GetHostEntry(ip);
            try
            {
                if (_opcServer == null)
                {
                    _opcServer = new OPCServer();
                }
                object serverList = _opcServer.GetOPCServers(host.HostName);
                List<string> ret = new List<string>();
                foreach (string item in (Array)serverList)
                {
                    ret.Add(item);
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 连接到opcserver，如果连接不上，进行10次重连后，返回失败
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            string ip = (string)ConfigurationManager.AppSettings.Get("OPCServerIp");
            if (String.IsNullOrEmpty(ip.Trim()))
            {
                ip = "127.0.0.1";
            }

            string serverName = (string)ConfigurationManager.AppSettings.Get("OPCServerName");
            if (String.IsNullOrEmpty(serverName.Trim()))
            {
                serverName = (string)GetServerName(ip)[0];
            }
            else
            {
                if (_opcServer == null)
                {
                    _opcServer = new OPCServer();
                }
            }
            int conTime = 0;
            while (conTime < 10)
            {
                _opcServer.Connect(serverName, ip);
                if (_opcServer.ServerState == (int)OPCServerState.OPCRunning)
                {
                    LogHelper.Info($"连接到ip:{ip}，OPC服务器名称为{serverName}成功");
                    _bConnect = true;
                    return true;
                }
                LogHelper.Info($"连接到ip:{ip}，OPC服务器名称为{serverName}失败，状态为：{_opcServer.ServerState}");
                conTime++;
                Thread.Sleep(100);
            }
            return false;
        }

        /// <summary>
        /// 根据最大的Rtu总数，计算分到OPCGroup的Rtu数目，返回值ret,表示前面ret.item1个OPCGroup需要分ret.item2个RTU，如果ret.item1<rOPCGroupNum，则后面的OPCGroup则需要分配ret.item2-1个RTU
        /// 比如，68个RTU，需要分到19个OPCGroup中，68%19=11,68/19=3,则前面11个OPCGroup需要分配4个RTU，后面的8个需要分配3个RTU，11*4+8*3===68
        /// 比如，57个RTU，需要分到19个OPCGroup中，57%19=0,57/19=3,则前面19个OPCGroup需要分配3个RTU，19*3==57
        /// </summary>
        /// <param name="rtuNum"></param>
        /// <returns></returns>
        private GroupRtuAllocation GetGroupRtuAllocation(int rtuNum)
        {
            GroupRtuAllocation allocation = new GroupRtuAllocation();
            var rOPCGroupNum = _MaxOPCGroupNum - 1;//需要用来进行读取数据的OPCGroup的最大数目，其中有一个OPCGroup用来写入数据
            if (rtuNum % rOPCGroupNum != 0)
            {
                allocation.bOPCGroupNum = rtuNum % rOPCGroupNum;
                allocation.bRtuNum = rtuNum / rOPCGroupNum + 1;
                allocation.aOPCGroupNum = rOPCGroupNum - allocation.bOPCGroupNum;
                allocation.aRtuNum = allocation.bRtuNum - 1;
            }
            else
            {
                allocation.bOPCGroupNum = rOPCGroupNum;
                allocation.bRtuNum = rtuNum / rOPCGroupNum;
                allocation.aOPCGroupNum = 0;
                allocation.aRtuNum = 0;
            }
            return allocation;
        }

        /// <summary>
        /// 判断OPCGroup是否需要add
        /// </summary>
        /// <param name="index">rtu的下标</param>
        /// <param name="allocation"></param>
        /// <returns></returns>
        private bool IsAddOPCGroupForRead(int index, GroupRtuAllocation allocation)
        {
            var groups = _opcServer.OPCGroups;
            #region //判断rGroup，是否是需要重新创建，还是从OPCGroups中获取
            if (allocation.bOPCGroupNum >= groups.Count - 1)//如果是前面多加一个RTU的opcGroup部分
            {
                if (index % allocation.bRtuNum == 0)//说明需要重新add OPCGroup
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if ((index - allocation.bOPCGroupNum * allocation.bRtuNum) % allocation.aRtuNum == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion //判断rGroup，是否是需要重新创建，还是从OPCGroups中获取
        }

        private List<string> GetPropertyNamesForWrite(Type type)
        {
            List<string> propertyNames = new List<string>();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(OperateFlagAttribute));
                if (attributes.Count() > 0 && ((OperateFlagAttribute)attributes.FirstOrDefault()).IsWrite)//是需要进行写的地址
                {
                    propertyNames.Add(property.Name);
                }
            }
            return propertyNames;
        }

        /// <summary>
        /// 在opc client中配置点表
        /// </summary>
        /// <param name="pts"></param>
        public bool SetPoints(List<PointTB> stations)
        {
            var groups = _opcServer.OPCGroups;
            groups.RemoveAll();

            try
            {
                int clientHandle = 0;
                var wGroup = groups.Add(_wOPCGroupName);//专门用于写入数据的OPCGroup
                SetWriteGroupProperty(wGroup);

                List<ReceDataOptor> receDataOptors = null;
                OPCGroup rGroup = null;
                int unitServerHandle = -1;
                var allocation = GetGroupRtuAllocation(stations.Count);
                var writeNames = GetPropertyNamesForWrite(typeof(PointTB));

                for (int i = 0; i < stations.Count; i++)
                {
                    var s = stations[i];
                    var stationName = s.stationName;

                    if (IsAddOPCGroupForRead(i, allocation))
                    {
                        rGroup = groups.Add(groups.Count.ToString());
                        SetReadGroupProperty(rGroup);
                        //将单位切换的地址放入OPCGroup中
                        var unitOPCItem = rGroup.OPCItems.AddItem($"{s.unitType}", ++clientHandle);
                        _dics.Add(clientHandle, unitOPCItem);
                        unitServerHandle = unitOPCItem.ServerHandle;
                        _unitServerHandles.Add(unitServerHandle);
                        receDataOptors = new List<ReceDataOptor>();
                        _rGroups.Add(rGroup, receDataOptors);
                    }
                    else
                    {
                        rGroup = groups.GetOPCGroup((groups.Count - 1).ToString());
                    }

                    var type = s.GetType();
                    var psInfo = type.GetProperties();

                    List<int> rServerHandleList = new List<int>();
                    List<int> wServerHandleList = new List<int>();
                    var bConfigRight = true;//用于标记该井的点信息在opc client中注册是否成功，初始认为为注册成功，失败则为false
                                            //配置组中的点
                    foreach (var pInfo in psInfo)
                    {
                        if (pInfo.Name.Equals("stationName") || pInfo.Name.Equals("unitType"))
                        {
                            continue;
                        }
                       
                        if (writeNames.Contains(pInfo.Name))//是需要进行写的地址
                        {
                            try
                            {
                                var opcItemW = wGroup.OPCItems.AddItem($"{stationName}!{pInfo.GetValue(s)}", ++clientHandle);
                                //LogHelper.Log.Info($"client 添加：站名为{stationName},表中字段名为{pInfo.Name}，OPC位号为{pInfo.GetValue(s)}成功");
                                wServerHandleList.Add(opcItemW.ServerHandle);
                            }
                            catch
                            {
                                LogHelper.Info($"client 添加：站名为{stationName},表中字段名为{pInfo.Name}，OPC位号为{pInfo.GetValue(s)}失败");
                                //return false;
                                bConfigRight = false;//配置出现错误，当前井场的点信息不全，不再进行配置
                                break;//配置出现错误，当前井场的点信息不全，不再进行配置
                            }
                        }
                        else //是需要进行读的地址
                        {
                            //在opc中配置需要订阅的点名信息
                            try
                            {
                                var opcItemR = rGroup.OPCItems.AddItem($"{stationName}!{pInfo.GetValue(s)}", ++clientHandle);
                                //LogHelper.Log.Info($"client 添加：站名为{stationName},表中字段名为{pInfo.Name}，OPC位号为{pInfo.GetValue(s)}成功");
                                rServerHandleList.Add(opcItemR.ServerHandle);
                                _dics.Add(clientHandle, opcItemR);
                            }
                            catch
                            {
                                LogHelper.Info($"client 添加：站名为{stationName},表中字段名为{pInfo.Name}，OPC位号为{pInfo.GetValue(s)}失败");
                                //return false;
                                bConfigRight = false;//配置出现错误，当前井场的点信息不全，不再进行配置
                                break;//配置出现错误，当前井场的点信息不全，不再进行配置
                            }
                        }
                    }
                    if (!bConfigRight)//该井在配置中，点信息不正确，导致在OPC的client中注册出错，需要跳过对该井的配置
                    {
                        //把该站在OPCGroup中注册OPCItems删除，
                        rGroup.OPCItems.Remove(rServerHandleList.Count, rServerHandleList.ToArray(), out Array error);
                        continue;
                    }
                    if (receDataOptors != null)
                    {
                        receDataOptors.Add(new ReceDataOptor(stationName, rGroup, wGroup, rServerHandleList, wServerHandleList, unitServerHandle));
                    }
                }

                LogHelper.Info("配点完成！");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(_opcServer.GetErrorString(ex.HResult));
                LogHelper.Error($"配点失败！错误信息：{ex.Message} {_opcServer.GetErrorString(ex.HResult)}");
                return false;
            }
        }

        private void SetReadGroupProperty(OPCGroup group)
        {
            group.UpdateRate = _updateRate;
            group.DeadBand = 0;
            group.IsSubscribed = true;
            group.IsActive = true;
            group.DataChange += KepGroup_DataChange;
        }

        private void SetWriteGroupProperty(OPCGroup group)
        {
            group.IsSubscribed = true;
            group.IsActive = true;
        }

        /// <summary>
        /// 数据订阅方法
        /// </summary>
        /// <param name="TransactionID">处理ID</param>
        /// <param name="NumItems">项个数</param>
        /// <param name="ClientHandles">OPC客户端的句柄</param>
        /// <param name="ItemValues">节点的值</param>
        /// <param name="Qualities">节点的质量</param>
        /// <param name="TimeStamps">时间戳</param>
        private void KepGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            if (NumItems > 0)
            {
                int clientHandle = (int)ClientHandles.GetValue(1);
                _dics.TryGetValue(clientHandle, out OPCItem item);

                OPCGroup rGroup = item.Parent;
                _rGroups.TryGetValue(rGroup, out List<ReceDataOptor> receDataOptors);

                var wGroup = _opcServer.OPCGroups.GetOPCGroup(_wOPCGroupName);

                //Stopwatch sp = new Stopwatch();
                //sp.Start();
                if (receDataOptors != null)
                {
                    if (_unitServerHandles.Contains(item.ServerHandle))//如果是类似于单位切换等共同需要的OPC点位信息发生变化，则立马进行计算并更新
                    {
                        Parallel.ForEach(receDataOptors, receDataOpt => Task.Run(async () => await receDataOpt.AddPoint(item.TimeStamp, 1)));
                        //foreach (var receDataOpt in receDataOptors)
                        //{
                        //    Task.Run(async () => await receDataOpt.AddPoint(item.TimeStamp, 1));
                        //}
                    }
                    else
                    {
                        Parallel.ForEach(receDataOptors, receDataOpt => Task.Run(async () => await receDataOpt.AddPoint(item.TimeStamp, _computeInterval / (_updateRate / 1000))));
                        //foreach (var receDataOpt in receDataOptors)
                        //{
                        //    Task.Run(async () => await receDataOpt.AddPoint(item.TimeStamp, _computeInterval / (_updateRate / 1000)));
                        //}
                    }
                }

                //sp.Stop();
                //Console.WriteLine(sp.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 返回读取的stationName，以及该station对应的平均产量对象
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, AverageOutput> GetReadOPCGroupAverageOutput()
        {
            Dictionary<string, AverageOutput> info = new Dictionary<string, AverageOutput>();
            foreach (var keypair in _rGroups)
            {
                foreach (var opt in keypair.Value)
                {
                    info.Add(opt.Name, opt.GetAverageHourOutput());
                }
            }
            return info;
        }

        /// <summary>
        /// 执行定时任务
        /// </summary>
        public void DoTime()
        {
            var target = GetReadOPCGroupAverageOutput();
            _timeOpt.SetOutputTimeTarget(target);
            _timeOpt.DoTask();
            _bTime = true;
        }

        public bool DisConnect()
        {
            try
            {
                if (_bTime)
                {
                    _timeOpt.Exit();
                    LogHelper.Info("定时操作取消！");
                    _bTime = false;
                }
                if (_bConnect)
                {
                    _opcServer.OPCGroups.RemoveAll();
                    _opcServer.Disconnect();
                    LogHelper.Info("与OPC服务端断开连接！");
                    _bConnect = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return false;
            }
        }
    }
}
