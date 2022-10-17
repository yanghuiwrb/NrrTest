using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.MySqlEntity;
using VMFW.Operate;

namespace VMFW.DB.Service.Iservice
{
    public interface IAcPointInfoService
    {
        /// <summary>
        /// 获取最大的标定日期
        /// </summary>
        /// <returns></returns>
        DateTime? GetMaxBdDate();

        /// <summary>
        /// 获取指定时间，年月日相同，并且标定数据不为空的第一条记录
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        AcPointInfo GetOneRecord(DateTime dt);

        /// <summary>
        /// 获取指定日期，平均计算产量
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        (double, double, double, double) GetAverageOutput(DateTime dt);

        /// <summary>
        /// 根据已有信息填充实体数据
        /// </summary>
        /// <param name="pointsInfo"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        List<AcPointInfo> FillIntervalPInfo(List<GroupPt> pointsInfo, List<object> values);
    }
}
