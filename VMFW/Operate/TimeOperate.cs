using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.DB.Service.Iservice;
using VMFW.DB.Service.ServiceImpl;
using VMFW.Operate.OperateObj;

namespace VMFW.Operate
{
    public class TimeOperate
    {
        //用于计算平均每小时产量定时操作
        private System.Timers.Timer hourTimer;

        //用于计算平均每天产量定时操作
        private System.Timers.Timer dayTimer;

        //用于计算日志清理的定时操作
        private System.Timers.Timer logClearTimer;

        //用于定时计算产量的目标
        private Dictionary<string, AverageOutput> outputTarget;

        public TimeOperate()
        {
            hourTimer = new System.Timers.Timer();
            dayTimer = new System.Timers.Timer();
            logClearTimer = new System.Timers.Timer();
        }

        public void Init()
        {
            hourTimer.Interval = 1000;//1s执行一次
            hourTimer.Elapsed += Timer_Elapsed;

            dayTimer.Interval = 1000;//1s执行一次
            dayTimer.Elapsed += Timer_Elapsed;

            logClearTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings.Get("LogClearInternal"));//默认值为每天执行一次
            logClearTimer.Elapsed += new System.Timers.ElapsedEventHandler(DoClearLog);
        }

        public void SetOutputTimeTarget(Dictionary<string ,AverageOutput> target)
        {
            outputTarget = target;
        }

        /// <summary>
        /// VFM执行定时任务
        /// </summary>
        public void DoTask()
        {
            hourTimer.Start();
            dayTimer.Start();
            logClearTimer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var dt = DateTime.Now;
            if (dt.Minute == 59)
            {
                Task.Run(async () => await DoComputeAverageHourOutput());
            }
            if (dt.Hour == 23 && dt.Minute == 59)
            {
                Task.Run(async () => await DoComputeAverageDayOutput());
            }
        }

        private void DoClearLog(object sender, System.Timers.ElapsedEventArgs e)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var logsPath = $"{path}Logs";
            var files = Directory.GetFiles(logsPath);
            foreach (var file in files)
            {
                var date = file.Split('.')[0];
                var tArray = date.Split('\\');
                date = tArray[tArray.Length - 1];
                var dt = DateTime.Parse(date);
                if ((DateTime.Now - dt).Days > Convert.ToInt32(ConfigurationManager.AppSettings.Get("LogExiredDay")))
                {
                    File.Delete(file);
                }
            }
        }

        private async Task DoComputeAverageHourOutput()
        {
            var wellNames = outputTarget.Keys;
            IPointInfoService service = new PointInfoService();
            foreach (var wellName in wellNames)
            {
                var output = await service.ComputeWellHourAverageOutput(wellName);
                AverageOutput averageOutput = null;
                outputTarget.TryGetValue(wellName, out averageOutput);
                averageOutput.SetHourAverageOutput(output);
            }
        }

        private async Task DoComputeAverageDayOutput()
        {
            var wellNames = outputTarget.Keys;
            IPointInfoService service = new PointInfoService();
            foreach (var wellName in wellNames)
            {
                var output = await service.ComputeWellDayAverageOutput(wellName);
                outputTarget.TryGetValue(wellName, out AverageOutput averageOutput);
                averageOutput.SetDayAverageOutput(output);
            }
        }

        public void Exit()
        {
            hourTimer.Stop();
            dayTimer.Stop();
            logClearTimer.Stop();
        }
    }
}
