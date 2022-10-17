using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.Atribute;

namespace VMFW.MySqlEntity
{
    /// <summary>
    /// 点表 即opc点位
    /// </summary>
    [Table("PointTB")]
    public class PointTB
    {
        //站场名称
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Key]
        public string stationName { get; set; }

        #region 各种opc位号

        #region
        //国际单位与国内单位进行切换的按钮点位
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [Universal(true)]
        public string unitType { get; set; }
        #endregion


        #region 固定点
        //水密度(kg/m3)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string rw { get; set; }

        //原油密度(kg/m3)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string ro { get; set; }

        //节流器开口直径(mm)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string ud { get; set; }

        //管线直径(mm)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string dD { get; set; }
        #endregion

        #region 相对静态点
        //工况下甲烷气体粘度(kg/(m*s))
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string ug { get; set; }

        //工况下水粘度（kg/(m*s)）
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string uw { get; set; }

        //原油粘度（kg/(m*s)）
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string uo { get; set; }

        //甲烷气体密度（kg/m3）
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string rg { get; set; }

        //油嘴上游平均压力(Pa)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string P { get; set; }

        //油田现场平均温度(℃)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string To { get; set; }

        //油嘴处平均温度(℃)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string T { get; set; }

        //含水率
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string fw { get; set; }

        //生产气油比
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string Rr { get; set; }

        //油嘴上游溶解气油比
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string Rs { get; set; }

        //标定车标定产油量(t/d)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string co { get; set; }

        //标定车标定产水量(t/d)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string cw { get; set; }

        //标定车标定产气量(t/d)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string cg { get; set; }

        //标定车标定产液量(t/d)
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string cl { get; set; }

        //标定年点名
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string year { get; set; }

        //标定月点名
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string month { get; set; }

        //标定日点名
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string day { get; set; }


        #endregion

        #region 实时动态点
        //油嘴上游压力
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string P1 { get; set; }

        //油嘴下游压力
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string P2 { get; set; }
        #endregion

        #region 中间输入点
        //气体等熵指数
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string K { get; set; }

        //雷诺数
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string Re { get; set; }

        //体积含气率
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        public string y { get; set; }
        #endregion

        #region 中间计算点
        //混合液体密度
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string rl { get; set; }

        //混合物密度
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string r { get; set; }

        //质量含气率
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string x { get; set; }

        //混合液体粘度
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string ul { get; set; }

        //混合物粘度
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string u { get; set; }

        //可膨胀系数
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string E { get; set; }

        //油嘴流量系数
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string C { get; set; }
        #endregion

        #region 产量输出点
        //产油量标定系数
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string a1 { get; set; }

        //产水量标定系数
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string a2 { get; set; }

        //产气量标定系数
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string a3 { get; set; }

        //产液量标定系数
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string a4 { get; set; }

        //实时产油量，即为每分钟值
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Qol { get; set; }

        //实时产水量，即为每分钟值
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Qwl { get; set; }

        //实时产气量，即为每分钟值
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Qgl { get; set; }

        //实时产液量，即为每分钟值
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Ql { get; set; }

        //日产油量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Dol { get; set; }

        //日产水量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Dwl { get; set; }

        //日产气量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Dgl { get; set; }

        //日产液量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Dl { get; set; }

        //月产油量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Mol { get; set; }

        //月产水量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Mwl { get; set; }

        //月产气量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Mgl { get; set; }

        //月产液量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Ml { get; set; }

        //年产油量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Yol { get; set; }

        //年产水量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Ywl { get; set; }

        //年产气量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Ygl { get; set; }

        //年产液量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Yl { get; set; }

        ////每分钟平均产油量，即为实时值
        //[Column(TypeName = "varchar")]
        //[MaxLength(40)]
        //[Required]
        //[OperateFlag(true)]
        //public string EMol { get; set; }

        ////每分钟平均产水量，即为实时值
        //[Column(TypeName = "varchar")]
        //[MaxLength(40)]
        //[Required]
        //[OperateFlag(true)]
        //public string EMwl { get; set; }

        ////每分钟平均产气量，即为实时值
        //[Column(TypeName = "varchar")]
        //[MaxLength(40)]
        //[Required]
        //[OperateFlag(true)]
        //public string EMgl { get; set; }

        ////每分钟平均产液量，即为实时值
        //[Column(TypeName = "varchar")]
        //[MaxLength(40)]
        //[Required]
        //[OperateFlag(true)]
        //public string EMl { get; set; }

        //每小时平均产油量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string EHol { get; set; }

        //每小时平均产水量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string EHwl { get; set; }

        //每小时平均产气量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string EHgl { get; set; }

        //每小时平均产液量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string EHl { get; set; }

        //每天平均产油量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string EDol { get; set; }

        //每天平均产水量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string EDwl { get; set; }

        //每天平均产气量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string EDgl { get; set; }

        //每天平均产液量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string EDl { get; set; }

        //累计总产油量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Tol { get; set; }

        //累计总产水量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Twl { get; set; }

        //累计总产气量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Tgl { get; set; }

        //累计总产液量
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        [Required]
        [OperateFlag(true)]
        public string Tl { get; set; }
        #endregion

        #endregion
    }
}
