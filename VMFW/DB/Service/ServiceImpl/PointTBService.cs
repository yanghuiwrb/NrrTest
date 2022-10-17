using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.DB.Service.Iservice;
using VMFW.MySqlEntity;

namespace VMFW.DB.Service.ServiceImpl
{
    public class PointTBService : IPointTBService
    {
        public  List<PointTB> GetAllList()
        {
            using (VFMContext dbContext = new VFMContext())
            {
                //Console.WriteLine(dbContext.PointTBs.Count());
                return dbContext.PointTBs.ToList();
            }
        }

        public PointTB GetFirstRecord()
        {
            using (VFMContext dbContext = new VFMContext())
            {
                return dbContext.PointTBs.FirstOrDefault();
            }
        }
    }
}
