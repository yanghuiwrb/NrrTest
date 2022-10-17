using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VMFW.MySqlEntity;

namespace VMFW.DB
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class VFMContext : DbContext
    {
        public VFMContext() : base("name=VFMDB")
        {

        }

        public DbSet<PointTB> PointTBs { get; set; }
        public DbSet<AcPointInfo> AcPointInfos { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
