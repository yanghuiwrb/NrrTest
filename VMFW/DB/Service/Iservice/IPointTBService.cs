using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.MySqlEntity;

namespace VMFW.DB.Service.Iservice
{
    public interface IPointTBService
    {
        /// <summary>
        /// 获取所有的数据记录
        /// </summary>
        /// <returns></returns>
        List<PointTB> GetAllList();

        /// <summary>
        /// 获取表中的第一条记录
        /// </summary>
        /// <returns></returns>
        PointTB GetFirstRecord();
    }
}
