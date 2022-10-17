using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.Operate.OperateObj
{
    /// <summary>
    ///平均产量
    /// </summary>
    public class AverageOutput
    {
        private DayAverageOutput daOutput;
        private HourAverageOutput haOutput;
        public AverageOutput()
        {
            daOutput = new DayAverageOutput();
            haOutput = new HourAverageOutput();
        }

        public DayAverageOutput GetDayAverageOutput()
        {
            return daOutput;
        }

        public HourAverageOutput GetHourAverageOutput()
        {
            return haOutput;
        }

        public void SetDayAverageOutput(double ol, double wl, double gl, double l, double oli, double wli, double gli, double li)
        {
            daOutput.SetOutput(ol, wl, gl, l, oli, wli, gli, li);
        }

        public void SetDayAverageOutput(DayAverageOutput output)
        {
            daOutput.SetOutput(output);
        }

        public void SetHourAverageOutput(double ol, double wl, double gl, double l, double oli, double wli, double gli, double li)
        {
            haOutput.SetOutput(ol, wl, gl, l, oli, wli, gli, li);
        }

        public void SetHourAverageOutput(HourAverageOutput output)
        {
            haOutput.SetOutput(output);
        }


    }
}
