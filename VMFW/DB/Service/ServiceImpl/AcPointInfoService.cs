//using EF6.BulkInsert.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.MySqlEntity;
using VMFW.Operate;

namespace VMFW.DB.Service.ServiceImpl
{
    public class AcPointInfoService
    {
        public DateTime? GetMaxBdDate()
        {
            using (VFMContext dbContext = new VFMContext())
            {
                if (dbContext.AcPointInfos.Count() > 0)
                {
                    DateTime dt = dbContext.AcPointInfos.Max(n => n.bdDate);
                    return dt;
                }
                return null;
            }
        }

        public AcPointInfo GetOneRecord(DateTime dt)
        {
            using (VFMContext dbContext = new VFMContext())
            {
                return dbContext.AcPointInfos.Where(n =>
                  n.bdDate.Day == dt.Day && n.bdDate.Month == dt.Month && n.bdDate.Year == dt.Year &&
                    n.a1.HasValue).FirstOrDefault();
            }
        }

        public (double, double, double, double) GetAverageOutput(DateTime dt)
        {
            using (VFMContext dbContext = new VFMContext())
            {
                //var recored = dbContext.AcPointInfos.Where(n =>
                //   n.bdDate.ToString("yyyy-MM-dd").Equals(dt.ToString("yyyy-MM-dd")) &&
                //   n.a1.HasValue).Where(n => n.Qol.HasValue);
                var recored = dbContext.AcPointInfos.Where(n => n.Qol.HasValue && 
                  n.bdDate.Day == dt.Day && n.bdDate.Month == dt.Month && n.bdDate.Year == dt.Year);
                var Qol = recored.Average(n => n.Qol.GetValueOrDefault());
                var Qgl = recored.Average(n => n.Qgl.GetValueOrDefault());
                var Qwl = recored.Average(n => n.Qwl.GetValueOrDefault());
                var Ql = recored.Average(n => n.Ql.GetValueOrDefault());
                return (Qol, Qgl, Qwl, Ql);
            }
        }

        public List<AcPointInfo> FillIntervalPInfo(List<GroupPt> pointsInfo, List<object> values)
        {
            List<AcPointInfo> acPointInfos = new List<AcPointInfo>();
            for (var i = 0; i < pointsInfo.Count; i++)
            {
                AcPointInfo pInfo = new AcPointInfo();
                pInfo.TimeStamp = pointsInfo[i].recDt;
                pInfo.WellName = pointsInfo[i].WellName;
                var type = pInfo.GetType();
                var properities = type.GetProperties();
                int index = 2;
                for (var j = 0; j < pointsInfo[i].GetCount(); j++, index++)
                {
                    if (properities[index].Name.Equals("bdDate"))
                    {
                        var year = pointsInfo[i].GetDoubleValue(j++);
                        var month = pointsInfo[i].GetDoubleValue(j++);
                        var day = pointsInfo[i].GetDoubleValue(j);

                        DateTime dt = DateTime.Parse($"{year}/{month}/{day}");
                        properities[index].SetValue(pInfo, dt);
                    }
                    else
                    {
                        properities[index].SetValue(pInfo, pointsInfo[i].GetDoubleValue(j));
                    }

                }

                if (i == pointsInfo.Count - 1)//最后一组数据需要插入计算数据
                {
                    foreach (var value in values)
                    {
                        properities[index++].SetValue(pInfo, value);
                    }
                }

                acPointInfos.Add(pInfo);

            }

            return acPointInfos;
        }

        public bool BulkInsert(List<AcPointInfo> entityList)
        {
            try
            {
                using (VFMContext dbContext = new VFMContext())
                {
                    dbContext.BulkInsert(entityList);
                    dbContext.BulkSaveChanges();
                    return true;
                }

            }
            catch (Exception )
            {
                //throw ex;
                return false;
            }
        }

    }
}
