using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.MySqlEntity
{
    /// <summary>
    /// 点值信息表
    /// </summary>
    [Table("AcPointInfo")]
    public class AcPointInfo
    {
        //存入数据库的时间戳
        [Key] 
        [Column(Order = 1)]
        public DateTime TimeStamp { get; set; }

        //井名称
        [Key]
        [Column(Order = 2,TypeName = "varchar")]
        [MaxLength(40)]
        public string WellName { get; set; }

        #region 各种点名
        #region 固定点
        //水密度(kg/m3)
        public double rw { get; set; }
        //原油密度(kg/m3)
        public double ro { get; set; }
        //节流器开口直径(mm)
        public double ud { get; set; }
        //管线直径(mm)
        public double dD { get; set; }
        #endregion

        #region 相对静态点
        //工况下甲烷气体粘度(kg/(m*s))
        public double ug { get; set; }
        //工况下水粘度（kg/(m*s)）
        public double uw { get; set; }
        //原油粘度（kg/(m*s)）
        public double uo { get; set; }
        //甲烷气体密度（kg/m3）
        public double rg { get; set; }
        //油嘴上游平均压力(Pa)
        public double P { get; set; }
        //油田现场平均温度(℃)
        public double To { get; set; }
        //油嘴处平均温度(℃)
        public double T { get; set; }
        //含水率
        public double fw { get; set; }
        //生产气油比
        public double Rr { get; set; }
        //油嘴上游溶解气油比
        public double Rs { get; set; }
        //标定车标定产油量(t/d)
        public double co { get; set; }
        //标定车标定产气量(t/d)
        public double cg { get; set; }
        //标定车标定产水量(t/d)
        public double cw { get; set; }
        //标定车标定产液量(t/d)
        public double cl { get; set; }
        //标定时间
        [Column(TypeName = "date")]
        public DateTime bdDate { get; set; }
        #endregion

        #region 实时动态点
        //油嘴上游压力
        public double P1 { get; set; }
        //油嘴下游压力
        public double P2 { get; set; }
        #endregion

        #region 中间输入点
        //气体等熵指数
        public double K { get; set; }
        //雷诺数
        public double Re { get; set; }
        //体积含气率
        public double y { get; set; }
        #endregion

        #region 中间计算点
        //混合液体密度
        public double? rl { get; set; }
        //混合物密度
        public double? r { get; set; }
        //质量含气率
        public double? x { get; set; }
        //混合液体粘度
        public double? ul { get; set; }
        //混合物粘度
        public double? u { get; set; }
        //可膨胀系数
        public double? E { get; set; }
        //油嘴流量系数
        public double? C { get; set; }
        #endregion

        #region 产量输出点
        //产油量标定系数
        public double? a1 { get; set; }
        //产水量标定系数
        public double? a2 { get; set; }
        //产气量标定系数
        public double? a3 { get; set; }
        //产液量标定系数
        public double? a4 { get; set; }
        //实时产油量
        public double? Qol { get; set; }
        //实时产气量
        public double? Qgl { get; set; }
        //实时产水量
        public double? Qwl { get; set; }
        //实时产液量
        public double? Ql { get; set; }
        //最终产油量
        public double? LQO { get; set; }
        //最终产气量
        public double? LQG { get; set; }
        //最终产水量
        public double? LQW { get; set; }
        //最终产液量
        public double? LQL { get; set; }
        #endregion

       
        #endregion
    }
}
