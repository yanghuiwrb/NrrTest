using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.Communication;
using VMFW.DB;
using VMFW.DB.Service.ServiceImpl;
using VMFW.Helper;
using VMFW.Operate.OperateObj;

namespace VMFW.Operate
{
    public class VFMOperator
    {
        private OPCClient client;

        public bool Init()
        {
            #region 连接OPC Server
            client = new OPCClient();
            if (!client.Connect())
            {
                return false;
            }

            //连接数据库,如果连接并且有点信息，则在OPCClient中进行配点并进行订阅
            if (!ConnectDB())
            {
                return false;
            }
           
            #endregion
            return true;
        }

        private bool ConnectDB()
        {
            try
            {
                //获取所有的站场信息
                PointTBService pSevice = new PointTBService();
                var stations = pSevice.GetAllList();

                if (stations.Count > 0)
                {
                    //配点成功
                    if (client.SetPoints(stations))
                    {
                        client.DoTime();
                        return true;
                    }
                    //client.DisConnect();
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return false;
            }
        }

       

        public bool Exit()
        {
            //LogHelper.Log.Info("与OPC服务端断开连接！");
            return client.DisConnect();
        }
    }
}
