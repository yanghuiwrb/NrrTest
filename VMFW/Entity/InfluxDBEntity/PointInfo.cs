using InfluxDB.Client.Core;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.Atribute;
using VMFW.Operate.OperateObj;

namespace VMFW.Entity.InfluxDBEntity
{
    /// <summary>
    /// 点值信息表,用于InfluxDB
    /// </summary>
    [Measurement("pointinfo")]
    public class PointInfo
    {
        //存入数据库的时间戳
        [Column(IsTimestamp = true)]
        public DateTime TimeStamp { get; set; }

        //井名称
        [Column(IsTag = true)]
        //[MaxLength(40)]
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
        //标定车标定产水量(t/d)
        public double cw { get; set; }
        //标定车标定产气量(t/d)
        public double cg { get; set; }
        //标定车标定产液量(t/d)
        public double cl { get; set; }

        //标定时间,年
        public int year { get; set; }
        //标定时间,月
        public int month { get; set; }
        //标定时间,日
        public int day { get; set; }
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

        #region
        //国际单位与国内单位进行切换的按钮点位,0表示是国内单位，1表示是国际单位
        public int unitType { get; set; }
        #endregion

        #region 中间计算点
        //混合液体密度
        [WriteOPCType(WriteType.Both)]
        public double? rl { get; set; }
        //混合物密度
        [WriteOPCType(WriteType.Both)]
        public double? r { get; set; }
        //质量含气率
        [WriteOPCType(WriteType.Both)]
        public double? x { get; set; }
        //混合液体粘度
        [WriteOPCType(WriteType.Both)]
        public double? ul { get; set; }
        //混合物粘度
        [WriteOPCType(WriteType.Both)]
        public double? u { get; set; }
        //可膨胀系数
        [WriteOPCType(WriteType.Both)]
        public double? E { get; set; }
        //油嘴流量系数
        [WriteOPCType(WriteType.Both)]
        public double? C { get; set; }
        #endregion

        #region 产量输出点（注意：变量名以I结尾的表示是国际单位，否则为国内单位，产量输出前面部分为国内单位，后面为国际单位，各类产量中国内与国际保持对应）
        #region 标定系数
        //产油量标定系数
        [WriteOPCType(WriteType.Both)]
        public double? a1 { get; set; }
        //产水量标定系数
        [WriteOPCType(WriteType.Both)]
        public double? a2 { get; set; }
        //产气量标定系数
        [WriteOPCType(WriteType.Both)]
        public double? a3 { get; set; }
        //产液量标定系数
        [WriteOPCType(WriteType.Both)]
        public double? a4 { get; set; }
        #endregion 标定系数
        ////////////////////////////////////////////////////////////////////////国内单位部分///////////////////////////////////////////////////////////
        #region 产量（国内单位）
        //实时产油量（进行标定后的数据）
        [WriteOPCType(WriteType.Home)]
        public double? Qol { get; set; }

        //实时产水量（进行标定后的数据）
        [WriteOPCType(WriteType.Home)]
        public double? Qwl { get; set; }

        //实时产气量（进行标定后的数据）
        [WriteOPCType(WriteType.Home)]
        public double? Qgl { get; set; }

        //实时产液量（进行标定后的数据）
        [WriteOPCType(WriteType.Home)]
        public double? Ql { get; set; }

        //日产油量
        [WriteOPCType(WriteType.Home)]
        public double? Dol { get; set; }

        //日产水量
        [WriteOPCType(WriteType.Home)]
        public double? Dwl { get; set; }

        //日产气量
        [WriteOPCType(WriteType.Home)]
        public double? Dgl { get; set; }

        //日产液量
        [WriteOPCType(WriteType.Home)]
        public double? Dl { get; set; }

        //月产油量
        [WriteOPCType(WriteType.Home)]
        public double? Mol { get; set; }

        //月产水量
        [WriteOPCType(WriteType.Home)]
        public double? Mwl { get; set; }

        //月产气量
        [WriteOPCType(WriteType.Home)]
        public double? Mgl { get; set; }

        //月产液量
        [WriteOPCType(WriteType.Home)]
        public double? Ml { get; set; }

        //年产油量
        [WriteOPCType(WriteType.Home)]
        public double? Yol { get; set; }

        //年产水量
        [WriteOPCType(WriteType.Home)]
        public double? Ywl { get; set; }

        //年产气量
        [WriteOPCType(WriteType.Home)]
        public double? Ygl { get; set; }

        //年产液量
        [WriteOPCType(WriteType.Home)]
        public double? Yl { get; set; }

        ////每分钟平均产油量
        //[WriteOPCType(WriteType.Home)]
        //public double? EMol { get; set; }

        ////每分钟平均产水量
        //[WriteOPCType(WriteType.Home)]
        //public double? EMwl { get; set; }

        ////每分钟平均产气量
        //[WriteOPCType(WriteType.Home)]
        //public double? EMgl { get; set; }

        ////每分钟平均产液量
        //[WriteOPCType(WriteType.Home)]
        //public double? EMl { get; set; }

        //每小时平均产油量
        [WriteOPCType(WriteType.Home)]
        public double? EHol { get; set; }

        //每小时平均产水量
        [WriteOPCType(WriteType.Home)]
        public double? EHwl { get; set; }

        //每小时平均产气量
        [WriteOPCType(WriteType.Home)]
        public double? EHgl { get; set; }

        //每小时平均产液量
        [WriteOPCType(WriteType.Home)]
        public double? EHl { get; set; }

        //每天平均产油量
        [WriteOPCType(WriteType.Home)]
        public double? EDol { get; set; }

        //每天平均产水量
        [WriteOPCType(WriteType.Home)]
        public double? EDwl { get; set; }

        //每天平均产气量
        [WriteOPCType(WriteType.Home)]
        public double? EDgl { get; set; }

        //每天平均产液量
        [WriteOPCType(WriteType.Home)]
        public double? EDl { get; set; }

        //累计产油量
        [WriteOPCType(WriteType.Home)]
        public double? Tol { get; set; }

        //累计产水量
        [WriteOPCType(WriteType.Home)]
        public double? Twl { get; set; }

        //累计产气量
        [WriteOPCType(WriteType.Home)]
        public double? Tgl { get; set; }

        //累计产液量
        [WriteOPCType(WriteType.Home)]
        public double? Tl { get; set; }
        #endregion 产量（国内单位）

        ////////////////////////////////////////////////////////////////////////国际单位部分///////////////////////////////////////////////////////////
        #region 产量（国际单位）
        //实时产油量（进行标定后的数据）
        [WriteOPCType(WriteType.Internal)]
        public double? QolI { get { return this.Qol / (0.159 * this.ro); } set { } }

        //实时产水量
        [WriteOPCType(WriteType.Internal)]
        public double? QwlI { get { return this.Qwl / (0.159 * this.rw); } set { } }

        //实时产气量
        [WriteOPCType(WriteType.Internal)]
        public double? QglI { get { return this.Qgl / 28.3168; } set { } }

        //实时产液量
        [WriteOPCType(WriteType.Internal)]
        public double? QlI { get; set; }

        //日产油量
        [WriteOPCType(WriteType.Internal)]
        public double? DolI { get { return this.Dol / (0.159 * this.ro); } set { } }

        //日产水量
        [WriteOPCType(WriteType.Internal)]
        public double? DwlI { get { return this.Dwl / (0.159 * this.rw); } set { } }

        //日产气量
        [WriteOPCType(WriteType.Internal)]
        public double? DglI { get { return this.Dgl / 28.3168; } set { } }

        //日产液量
        [WriteOPCType(WriteType.Internal)]
        public double? DlI { get; set; }

        //月产油量
        [WriteOPCType(WriteType.Internal)]
        public double? MolI { get { return this.Mol / (0.159 * this.ro); } set { } }

        //月产水量
        [WriteOPCType(WriteType.Internal)]
        public double? MwlI { get { return this.Mwl / (0.159 * this.rw); } set { } }

        //月产气量
        [WriteOPCType(WriteType.Internal)]
        public double? MglI { get { return this.Mgl / 28.3168; } set { } }

        //月产液量
        [WriteOPCType(WriteType.Internal)]
        public double? MlI { get; set; }

        //年产油量
        [WriteOPCType(WriteType.Internal)]
        public double? YolI { get { return this.Yol / (0.159 * this.ro); } set { } }

        //年产水量
        [WriteOPCType(WriteType.Internal)]
        public double? YwlI { get { return this.Ywl / (0.159 * this.rw); } set { } }

        //年产气量
        [WriteOPCType(WriteType.Internal)]
        public double? YglI { get { return this.Ygl / 28.3168; } set { } }

        //年产液量
        [WriteOPCType(WriteType.Internal)]
        public double? YlI { get; set; }

        ////每分钟平均产油量
        //[WriteOPCType(WriteType.Internal)]
        //public double? EMolI { get { return this.EMol / (0.159 * this.ro); } set { } }

        ////每分钟平均产水量
        //[WriteOPCType(WriteType.Internal)]
        //public double? EMwlI { get { return this.EMwl / (0.159 * this.rw); } set { } }

        ////每分钟平均产气量
        //[WriteOPCType(WriteType.Internal)]
        //public double? EMglI { get { return this.EMgl / 28.3168; } set { } }

        ////每分钟平均产液量
        //[WriteOPCType(WriteType.Internal)]
        //public double? EMlI { get; set; }

        //每小时平均产油量
        [WriteOPCType(WriteType.Internal)]
        public double? EHolI { get { return this.EHol / (0.159 * this.ro); } set { } }

        //每小时平均产水量
        [WriteOPCType(WriteType.Internal)]
        public double? EHwlI { get { return this.EHwl / (0.159 * this.rw); } set { } }

        //每小时平均产气量
        [WriteOPCType(WriteType.Internal)]
        public double? EHglI { get { return this.EHgl / 28.3168; } set { } }

        //每小时平均产液量
        [WriteOPCType(WriteType.Internal)]
        public double? EHlI { get; set; }

        //每天平均产油量
        [WriteOPCType(WriteType.Internal)]
        public double? EDolI { get { return this.EDol / (0.159 * this.ro); } set { } }

        //每天平均产水量
        [WriteOPCType(WriteType.Internal)]
        public double? EDwlI { get { return this.EDwl / (0.159 * this.rw); } set { } }

        //每天平均产气量
        [WriteOPCType(WriteType.Internal)]
        public double? EDglI { get { return this.EDgl / 28.3168; } set { } }

        //每天平均产液量
        [WriteOPCType(WriteType.Internal)]
        public double? EDlI { get; set; }

        //累计产油量
        [WriteOPCType(WriteType.Internal)]
        public double? TolI { get { return this.Tol / (0.159 * this.ro); } set { } }

        //累计产水量
        [WriteOPCType(WriteType.Internal)]
        public double? TwlI { get { return this.Twl / (0.159 * this.rw); } set { } }

        //累计产气量
        [WriteOPCType(WriteType.Internal)]
        public double? TglI { get { return this.Tgl / 28.3168; } set { } }

        //累计产液量
        [WriteOPCType(WriteType.Internal)]
        public double? TlI { get; set; }

        #endregion 产量（国际单位）
        #endregion
        #endregion



    }
}
