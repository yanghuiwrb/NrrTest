using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.Algorithm
{
    public class DPMAlgorithm
    {
        //TODO 输入相对静态参数
        //工况下甲烷气体粘度(methane gas viscosity)(kg/(m*s))
        private double _ug;

        //工况下水粘度(water viscosity)（kg/(m*s)）
        private double _uw;

        //原油粘度(crude viscosity)（kg/(m*s)）
        private double _uo;

        //甲烷气体密度(methane gas density)（kg/m3）
        private double _ρg;

        //油嘴上游平均压力(nozzle mean upstream pressure)(Pa)
        private double _P;

        //油田现场平均温度(field mean temperature)(℃)
        private double _To;

        //油嘴处平均温度(nozzle mean temperature)(℃)
        private double _T;

        //含水率(moisture content)
        private double _fw;

        //生产气油比(produced GOR)
        private double _Rρ;

        //油嘴上游溶解气油比(nozzle upstream dissolved GOR)
        private double _Rs;

        //标定车标定产油量(calibrate oil)(t/d)
        private double _co;

        //标定车标定产气量(calibrate gas)(t/d)
        private double _cg;

        //标定车标定产水量(calibrate water)(t/d)
        private double _cw;

        //标定车标定产液量(calibrate liquid)(t/d)
        private double _cl;

        //TODO 输入固定参数
        //水密度(water density)(kg/m3)
        private double _ρw;

        //原油密度(crude density)(kg/m3)
        private double _ρo;

        //节流器开口直径(throttle opening diameter)(m)
        private double _d;

        //管线直径(pipe diameter)(m)
        private double _D;

        //TODO 输入实时动态参数
        //油嘴上游压力(nozzle upstream pressure)（Pa)
        //多个上游压力，以逗号作为分隔符，类似于1.2,1.3,....,1.4
        private string _P1s;

        //油嘴下游压力(nozzle downstream pressure)（Pa)
        //多个下游压力，以逗号作为分隔符，类似于1.2,1.3,....,1.4
        private string _P2s;

        //TODO 中间输入参数
        //气体等熵指数
        private double _K;

        //雷诺数Re
        private double _Re;

        //体积含气率
        private double _y;

        //产油量标定系数
        private double _a1;

        //产水量标定系数
        private double _a2;

        //产气量标定系数
        private double _a3;

        //产液量标定系数
        private double _a4;

        //TODO 中间计算参数
        //直径比
        private double _β;

        //混合液体的密度
        private double _pl;

        //质量含气率
        private double _x;

        //密度
        private double _ρ;

        //混合液体粘度
        private double _ul;

        //混合物粘度
        private double _u;

        //流量系数
        private double _C;

        //可膨胀系数
        private double _E;

        //差压值
        private double _diffP;

        //上下游压力比值
        private double _ratioP;

        //油嘴上游压力下的甲烷密度
        private double _ρgl;

        //TODO 输出参数
        private OutputParam output;

        public OutputParam GetOutput()
        {
            return output;
        }

        public DPMAlgorithm()
        {
            output = new OutputParam();
            this._a1 = 1;
            this._a2 = 1;
            this._a3 = 1;
            this._a4 = 1;
        }

        //TODO 设置静态参数
        public void SetStaticParam(double ug, double uw, double uo, double ρg, double p, double to, double t, double fw, double rρ, double rs, double co, double cg, double cw, double cl)
        {
            this._ug = ug;
            this._uw = uw;
            this._uo = uo;
            this._ρg = ρg;
            this._P = p * 1000000;
            this._To = to;
            this._T = t;
            this._fw = fw;
            this._Rρ = rρ;
            this._Rs = rs;

            this._co = co;
            this._cg = cg * 0.0007174;
            this._cw = cw;
            this._cl = cl;

            SetMLD();
            SetMLV();
        }

        //TODO 设置固定参数
        public void SetFixParam(double ρw, double ρo, double d, double D)
        {
            this._ρw = ρw;
            this._ρo = ρo;
            this._d = d * 0.001;
            this._D = D * 0.001;
            Setβ();
        }

        //TODO 设置实时动态数据
        public void SetRealParam(string p1s, string p2s)
        {
            this._P1s = p1s;
            this._P2s = p2s;

            SetDiffP();
            SetRatioP();

        }


        //TODO 设置中间输入参数
        public void SetInternalIPParam(double k, double re, double y)
        {
            this._K = k;
            this._Re = re;
            this._y = y;

            SetMS();
            SetFC();
        }

        //TODO 设置标定系数
        public void SetBdCoef(double a1, double a2, double a3, double a4)
        {
            this._a1 = a1;
            this._a2 = a2;
            this._a3 = a3;
            this._a4 = a4;
        }

        //TODO 计算混合液体密度(mixed liquid density )
        //计算公式为pl = pw*fw+po*(1-fw)
        private void SetMLD()
        {
            this._pl = this._ρw * this._fw + this._ρo * (1 - this._fw);
        }

        //TODO 计算质量含气率(mass air content)
        // 计算公式为x=pg*y/(pg*y+pl*(1-y))
        private void SetMAC()
        {
            this._x = this._ρg * this._y / (this._ρg * this._y + this._pl * (1 - this._y));
        }

        //ToDO 假设油井产出液为均相流，密度的计算公式
        // 计算公式为p=pg*pl/(x*pl+pg*(1-x))
        private void SetDensity()
        {
            this._ρ = this._ρgl * this._pl / (this._x * this._pl + this._ρgl * (1 - this._x));
        }

        //TODO 混合液体粘度(mixed liquid viscosity)
        //计算公式为ul = uo*(1-fw)+fw*uw
        private void SetMLV()
        {
            this._ul = this._uo * (1 - this._fw) + this._fw * this._uw;
        }

        //TODO 计算混合物粘度(mixture viscosity)
        //计算公式为 u = (1-y)*ul+y*ug;
        private void SetMS()
        {
            this._u = (1 - this._y) * this._ul + this._y * this._ug;
        }

        //TODO 计算β ,β直径比，无量纲
        // 计算公式 β=d/D
        private void Setβ()
        {
            this._β = this._d / this._D;
        }

        //TODO 流量系数计算flow coefficient
        //计算公式为0.99-0.2262*β(4.1)-(0.00175*β(2)-0.0033*β(4.15))*((10(6)/(re)))(1.15)
        private void SetFC()
        {
            double tempX = Math.Pow(10, 6);
            /*   double tempY = Re;*/
            double temp = Math.Pow(tempX / this._Re, 1.15);

            this._C = 0.99 - 0.2262 * Math.Pow(this._β, 4.1) - (0.00175 * Math.Pow(this._β, 2) - 0.0033 * Math.Pow(this._β, 4.15)) * temp;
        }

        //TODO 计算膨胀系数 E
        //计算公式为：y*(e1*e2*e3)(1/2)+(1-y)
        //e1 =k*t(2/k)/(k-1)
        //e2 =(1-β(4))/(1-β(4)*t(2/k))
        //e3 = (1-t((k-1)/k))/(1-t)
        private void SetE()
        {
            double t = this._ratioP;//油嘴上游压力与油嘴下游压力的比值
            double e1 = this._K * Math.Pow(t, (2.0 / this._K)) / (this._K - 1.0);
            double e2 = (1 - Math.Pow(this._β, 4)) / (1 - Math.Pow(this._β, 4) * Math.Pow(t, (2.0 / this._K)));
            double e3 = (1 - Math.Pow(t, (this._K - 1.0) / this._K)) / (1.0 - t);
            this._E = this._y * Math.Pow(e1 * e2 * e3, 0.5) + (1 - this._y);

        }

        private double GetAvgP(string ps)
        {
            List<double> listP = ParsePressureStr(ps);
            return listP.Average();
        }

        //TODO 计算雷诺数，如果不能提供雷诺数，则由该公式进行计算
        private double GetReD()
        {
            double pi = 3.14;
            double v = (this._cl + this._cg) / (21.6 * pi * this._D * this._D);
            return this._ρ * v * this._D / this._u;
        }

        //TODO 获取油嘴上游压力下的甲烷密度
        private void SetPgl()
        {
            double avgP = GetAvgP(this._P1s);//油嘴上游压力,防止油嘴压力波动太大，取多次的平均值
            this._ρgl = this._ρg * (273.0 + this._To) / (273.0 + this._T) * avgP / 101325;
        }

        //TODO 解析字符串，字符串格式为 1.2,1.3,1.4   将字符串格式转为double类型的List
        private List<double> ParsePressureStr(string ps)
        {
            List<double> lists = new List<double>();
            ps.Split(',').ToList().ForEach(x => lists.Add(Convert.ToDouble(x) * 1000000));
            return lists;
        }

        /* //TODO 计算上游压力与下游压力比值，由于可能有多个上油压力和下游压力，取各个比值平方和开方根作为最后结果
         private  double getPressureRatio(){
             List<double> upList = ParsePressureStr(nups);
             List<double> downList = ParsePressureStr(ndps);
             List<double> rationList = new ArrayList<double>();
             int length = (int) Math.min(upList.stream().count(),downList.stream().count());
             for(int i =0;i<length;i++){
                 double up = upList.get(i);
                 double down = downList.get(i);
                 if (!double.isNaN(up)&&!double.isNaN(down)){
                     double ratio = upList.get(i)/downList.get(i);
                     rationList.add(ratio);
                 }
             }
             double temp =  rationList.stream().collect(Collectors.averagingDouble(x->Math.pow(x,2)));
             return Math.pow(temp,1/2);
         }*/

        //TODO 计算上下游压力差值或者比值（具体取决于传递的lamda参数），由于可能有多个上油压力和下游压力，取各个比值或者差值（具体取决于传递的lamda参数）平方和开方根作为最后结果
        private double GetPressureOpt(Func<double, double, double> func)
        {
            List<double> upList = ParsePressureStr(this._P1s);
            List<double> downList = ParsePressureStr(this._P2s);
            List<double> rationList = new List<double>();
            int length = Math.Min(upList.Count, downList.Count);
            for (int i = 0; i < length; i++)
            {
                double up = upList[i];
                double down = downList[i];
                if (!Double.IsNaN(up) && !Double.IsNaN(down))
                {
                    /* double diff = upList.get(i)-downList.get(i);//上游-下游*/
                    /* double diff = opt.DoubleBinaryOperator(upList.get(i),downList.get(i));*/
                    double diff = func(up, down);

                    rationList.Add(diff);
                }
            }
            if (rationList.Count == 1)
            {
                return rationList[0];
            }
            //double temp = rationList.stream().collect(Collectors.averagingDouble(x->Math.pow(x, 2)));

            List<double> retList = new List<double>();
            rationList.ForEach(x => retList.Add(Math.Pow(x, 2)));
            return Math.Pow(retList.Average(), 0.5);
        }

        //计算上下游压力差值
        private void SetDiffP()
        {
            this._diffP = GetPressureOpt((a, b) =>
            {
                return (a - b);
            });
        }

        //计算上下游压力比值
        private void SetRatioP()
        {
            this._ratioP = GetPressureOpt((a, b) =>
            {
                return b / a;
            });
        }

        //TODO 计算总流量，Q = c/((1-β4)(1/2))*e*pai/4*(d)(2)*(2*p*diffP)(1/2)
        private double GetTotalQ()
        {
            double pai = 3.14;
            SetPgl();//计算pgl
            SetMAC();//计算x
            SetDensity();//计算ρ
            SetE();//计算E
            return this._C / Math.Pow((1 - Math.Pow(this._β, 4)), 0.5) * this._E * pai / 4 * Math.Pow(this._d, 2) * Math.Pow(2 * this._ρ * this._diffP, 0.5);
        }

        //TODO 计算流量,单位为国内单位
        public void ComputeOutput()
        {
            #region 根据差压法计算产量
            this.ComputeHomeOutput();
            #endregion

            #region 根据标定系数，对实时产量进行调整
            this.BDOutput();
            #endregion

            #region 计算国际单位的实时产量
            this.ComputeInternationalOutput();
            #endregion

            //更新输出数据中的其他参数
            this.UpdateOutput();
        }

        //TODO 根据差压法计算产量
        private void ComputeHomeOutput()
        {
            //计算总流量
            var Q = GetTotalQ();

            #region 根据公式计算实时产量
            output.QL = Q * (1 - this._x) * 86.4;
            output.QO = output.QL * (1 - this._fw);
            output.QW = output.QL * this._fw;
            output.QG = Q * this._x * 86.4 / 0.0007174;
            #endregion
        }

        //TODO 根据标定系数修正实时产量
        private void BDOutput()
        {
            output.QO = this._a1 * output.QO;
            output.QW = this._a2 * output.QW;
            output.QG = this._a3 * output.QG;
            output.QL = this._a4 * output.QL;
        }

        //TODO 计算流量,单位为国际单位
        private void ComputeInternationalOutput()
        {
            output.QLI = output.QL / (0.159 * this._pl);
            output.QOI = output.QO / (0.159 * this._ρo);
            output.QWI = output.QW / (0.159 * this._ρw);
            output.QGI = output.QG / 28.3186;
        }

        //TODO 更新输出数据中的其他参数
        private void UpdateOutput()
        {
            output.a1 = this._a1;
            output.a2 = this._a2;
            output.a3 = this._a3;
            output.a4 = this._a4;
            output.C = this._C;
            output.E = this._E;
            output.r = this._ρ;
            output.rl = this._pl;
            output.u = this._u;
            output.x = this._x;
            output.ul = this._ul;
        }

        /// <summary>
        /// 获取混合液体，油，水的密度
        /// </summary>
        /// <returns></returns>
        public (double, double, double) GetDensitys()
        {
            return (this._pl, this._ρo, this._ρw);
        }

        public override string ToString()
        {
            return "DPMAlgorithm{" +
                    "ug=" + this._ug +
                    ", uw=" + this._uw +
                    ", uo=" + this._uo +
                    ", ρg=" + this._ρg +
                    ", P=" + this._P +
                    ", To=" + this._To +
                    ", T=" + this._T +
                    ", fw=" + this._fw +
                    ", Rρ=" + this._Rρ +
                    ", Rs=" + this._Rs +
                    ", ρw=" + this._ρw +
                    ", ρo=" + this._ρo +
                    ", d=" + this._d +
                    ", D=" + this._D +
                    ", P1s='" + this._P1s + '\'' +
                    ", P2s='" + this._P2s + '\'' +
                    ", K=" + this._K +
                    ", Re=" + this._Re +
                    ", y=" + this._y +
                    ", a1=" + this._a1 +
                    ", a2=" + this._a2 +
                    ", a3=" + this._a3 +
                    ", a4=" + this._a4 +
                    ", β=" + this._β +
                    ", pl=" + this._pl +
                    ", x=" + this._x +
                    ", ρ=" + this._ρ +
                    ", ul=" + this._ul +
                    ", u=" + this._u +
                    ", C=" + this._C +
                    ", E=" + this._E +
                    ", diffP=" + this._diffP +
                    ", ratioP=" + this._ratioP +
                    ", ρgl=" + this._ρgl +
                    ", output=" + output.ToString() +
                    '}';
        }
    }
}
