using log4net;
using OPCAutomation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMFW.Algorithm;
using VMFW.Communication;
using VMFW.Helper;
using VMFW.Operate;

namespace VMFW
{
    public partial class Form1 : Form
    {

        //private static ILog log = LogManager.GetLogger(typeof(Form1));
        //private System.Timers.Timer time;
        private VFMOperator opt;
        public Form1()
        {
            InitializeComponent();
           
            opt = new VFMOperator();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region 算法测试
            DPMAlgorithm dpmal = new DPMAlgorithm();
            //输入参数
            dpmal.SetFixParam(1000, 850, 50.8, 63.5);
            dpmal.SetStaticParam(0.000001, 0.05, 0.1, 0.67, 0.001, 25, 30, 0.4, 0.3, 0.2, 1, 1, 1, 1);
            dpmal.SetInternalIPParam(1.34, 300000, 0.3);
            dpmal.SetRealParam("4.5", "3.75");

            dpmal.ComputeOutput();
            dpmal.GetOutput();
            Console.WriteLine(dpmal.ToString());
            #endregion

            #region 读取输入点名称
            //Dictionary<string, Config.Point> ptMap = PtConfiguration.GetInputPoint();
            //foreach (string key in ptMap.Keys)
            //{
            //    Console.WriteLine(key);
            //}
            #endregion

            #region 读取输出点名称
            //PtConfiguration.GetOutputPoint();
            #endregion

            #region VFMOperator 测试
            //VFMOperator opt = new VFMOperator();
            //opt.Init();
            #endregion
        }

        private void FillServerName()
        {
            //var list = client.GetServerName(OPCServerIPTB.Text);

            //foreach(var item in list)
            //{
            //    if (!OPCServerNameCB.Items.Contains(item))
            //    {
            //        OPCServerNameCB.Items.Add(item);
            //    }
            //}
        }


        private void OnServerNameCmbDropDown(object sender, EventArgs e)
        {
            //ComboBox cb = (ComboBox)sender;
            //FillServerName();
            //Console.WriteLine("Test");
        }

        //连接server
        private void ConBtn_Click(object sender, EventArgs e)
        {
            //if (opcServer == null)
            //{
            //    opcServer = new OPCServer();
            //}
            //opcServer.Connect(((ComboBox)sender).SelectedItem.ToString(), OPCServerIPTB.Text);
            //if (opcServer.ServerState == (int)OPCServerState.OPCRunning)
            //{
            //    bco
            //}
        }

        private void From_Load(object sender, EventArgs e)
        {

            if (!opt.Init())
            {
                LogHelper.Info("初始化失败，应用程序即将退出");
                MessageBox.Show("初始化失败，应用程序即将退出,具体信息查阅日志", "提示", MessageBoxButtons.OK);
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            #region 算法测试
            DPMAlgorithm dpmal = new DPMAlgorithm();
            //输入参数
            dpmal.SetFixParam(1000, 894.6, 12, 53.18);
            dpmal.SetStaticParam(0.000006, 0.0005497, 0.0251, 0.7, 2000000, 50, 50, 0.2, 5.3428, 0, 1, 1, 1, 1);
            dpmal.SetInternalIPParam(1.3, 200000, 0.258);
            dpmal.SetRealParam("2", "1.92");

            dpmal.ComputeOutput();
            dpmal.GetOutput();
            Console.WriteLine(dpmal.ToString());
            #endregion

        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            opt.Exit();
            LogHelper.Info("应用程序退出中……");
        }


        //Task.Run(async () =>
        //{
        //    if (await opt.Init())
        //    {
        //        log.Info("初始化失败，应用程序即将退出");
        //        MessageBox.Show("初始化失败，应用程序即将退出,具体信息查阅日志");
        //        this.Close();
        //    }
        //});
    }
}
