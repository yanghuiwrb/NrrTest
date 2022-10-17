using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.Helper
{
    public class ClassPropertyHelper
    {
        /// <summary>
        /// 获取一个类的属性所在的位置，第一个属性位置为0，以此类推
        /// </summary>
        /// <param name="type">类型名</param>
        /// <param name="name">属性名称</param>
        /// <returns></returns>
        public static int GetPropertyIndex(Type type, string name)
        {
            var properties = type.GetProperties();
            int index = -1;
            foreach (var property in properties)
            {
                index++;
                if (property.Name.Equals(name))
                {
                    break;
                }
            }
            return index;
        }
    }
}
