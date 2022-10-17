using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.Atribute
{
    /// <summary>
    /// 是否需要写入opc,属性IsWrite为true，则表示该值是需要写入OPC，主要用于判断是否需要订阅数据，用于表PointTB
    /// </summary>
    public class OperateFlagAttribute : Attribute
    {
        private bool isWrite;
        public bool IsWrite { get { return isWrite; } }
        public OperateFlagAttribute(bool flag)
        {
            isWrite = flag;
        }
    }
}
