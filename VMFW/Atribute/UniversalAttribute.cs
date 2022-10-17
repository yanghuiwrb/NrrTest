using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.Atribute
{
    /// <summary>
    /// 表示是所有井场通用的OPC地址，并且不需要采用从数据库中读取wellname!OPC地址进行拼接。直接从数据库中读取值作为OPC地址即可，配置的时候需要特别注意
    /// </summary>
    public class UniversalAttribute : Attribute
    {
        private bool _isUniversal;
        public bool IsUniversal { get { return this._isUniversal; } set { _isUniversal = value; } }
        public UniversalAttribute(bool t)
        {
            IsUniversal = t;
        }
    }
}
