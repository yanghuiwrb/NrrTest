using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.Algorithm;
using VMFW.Entity.InfluxDBEntity;
using VMFW.Operate;
using VMFW.Operate.OperateObj;

namespace VMFW.DB.Service.Iservice
{
    public interface IPointInfoService
    {
        /// <summary>
        /// 写入一组PointInfo类型的数据
        /// </summary>
        /// <param name="pDatas"></param>
        Task WriteDatasAsync(List<PointInfo> pDatas);

        /// <summary>
        /// 写入一PointInfo类型的数据
        /// </summary>
        /// <param name="pDatas"></param>
        Task WriteDataAsync(PointInfo pData);

        /// <summary>
        ///获取PointInfo中年月日与指定时间相同的最新记录
        /// </summary>
        /// <param name="wellName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        Task<PointInfo> GetOneRecord(string wellName, int year, int month, int day);

        /// <summary>
        /// 获取最后的标定记录
        /// </summary>
        /// <param name="wellName"></param>
        /// <returns></returns>
        Task<PointInfo> GetMaxBDRecord(string wellName);

        /// <summary>
        /// 计算指定井，在指定年月日的日产量(单位为国内单位)
        /// </summary>
        /// <param name="wellName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        Task<Dictionary<string, double>> ComputeHDayOutput(string wellName, int year, int month, int day);

        /// <summary>
        /// 计算指定井，在指定年月日的小时平均产量(单位为国内单位)
        /// </summary>
        /// <param name="wellName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        Task<Dictionary<string, double>> ComputeHHEverageOutput(string wellName, int year, int month, int day);

        /// <summary>
        /// 计算指定井，在指定年月日的日产量(单位为国际单位)
        /// </summary>
        /// <param name="wellName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        Task<Dictionary<string, double>> ComputeIDayOutput(string wellName, int year, int month, int day);

        /// <summary>
        /// 计算指定井，在指定年月日的小时平均产量(单位为国际单位)
        /// </summary>
        /// <param name="wellName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        Task<Dictionary<string, double>> ComputeIHEverageOutput(string wellName, int year, int month, int day);

        ///// <summary>
        ///// 获取指定年月日的产量的均值
        ///// </summary>
        ///// <param name="wellName"></param>
        ///// <param name="year"></param>
        ///// <param name="month"></param>
        ///// <param name="day"></param>
        ///// <returns></returns>
        //Task<List<double>> GetAverageOutput(string wellName, int year, int month, int day);

        /// <summary>
        /// 将数据转为PointInfo数据
        /// </summary>
        /// <param name="gPts"></param>
        /// <param name="outputParam"></param>
        /// <returns></returns>
        List<PointInfo> FillIntervalPInfo(List<GroupPt> gPts, OutputParam outputParam);

        /// <summary>
        /// 获取某口井的某一数据的最后一条记录的时间
        /// </summary>
        /// <param name="wellName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<DateTime?> GetLastRecordTime(string wellName, string param);

        /// <summary>
        /// 获取某口井的第一条数据的第一条记录的时间
        /// </summary>
        /// <param name="wellName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<DateTime?> GetFirstRecordTime(string wellName);

        /// <summary>
        /// 计算实时日产量，并更新pInfo中的日产量数据
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        Task ComputeRealDayOutput(PointInfo pInfo);

        /// <summary>
        /// 计算实时月产量，并跟新pInfo中的月产量数据
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        Task ComputeRealMonthOutput(PointInfo pInfo);

        /// <summary>
        /// 计算实时年产量，并跟新pInfo中的年产量数据
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        Task ComputeRealYearOutput(PointInfo pInfo);

        ///// <summary>
        ///// 计算每小时平均产量数据，并跟新pInfo中的每小时平均产量数据
        ///// </summary>
        ///// <param name="pInfo"></param>
        ///// <returns></returns>
        //Task ComputeEHourOutput(PointInfo pInfo);

        ///// <summary>
        /////  计算每年平均产量数据，并跟新pInfo中的每年平均产量数据,同时将新更新数据存入values中
        ///// </summary>
        ///// <param name="pInfo"></param>
        ///// <returns></returns>
        //Task ComputeEDayOutput(PointInfo pInfo);

        /// <summary>
        /// 计算某口井的小时平均产量
        /// </summary>
        /// <param name="wellName"></param>
        /// <returns></returns>
        Task<HourAverageOutput> ComputeWellHourAverageOutput(string wellName);

        /// <summary>
        /// 计算某口井的日平均产量
        /// </summary>
        /// <param name="wellName"></param>
        /// <returns></returns>
        Task<DayAverageOutput> ComputeWellDayAverageOutput(string wellName);

        /// <summary>
        /// 计算累计产量
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        Task ComputeTotalOutput(PointInfo pInfo);
        ///// <summary>
        ///// 计算某口井的指定年月日，混合液体密度，水，油密度的平均值。
        ///// </summary>
        ///// <param name="wellName"></param>
        ///// <param name="year"></param>
        ///// <param name="month"></param>
        ///// <param name="day"></param>
        ///// <returns>混合液体密度，水密度，油密度</returns>
        //Task<double> GetAverageDensity(string wellName, int year, int month, int day);
    }
}
