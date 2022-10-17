using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.Operate.OperateObj;

namespace VMFW.Atribute
{
    /// <summary>
    /// 主要用于标记数据类型。往OPC写入数据时，获取需要的数据，值有三种：BothAttribute,HomeAttribute,InternalAttribute
    /// </summary>

    public class WriteOPCTypeAttribute : Attribute
    {
        private WriteType _type;
        public WriteType type { get { return this._type; } set { _type = value; } }
        public WriteOPCTypeAttribute(WriteType t)
        {
            type = t;
        }
    }
}
