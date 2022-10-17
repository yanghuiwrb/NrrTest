using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMFW.Operate
{
    public class Point
    {
        //井名称
        private string _wellName;

        //点名称tag
        private string _name;

        //点的值
        private object _value;

        //点的时间戳
        private object _timeStamp;

        //点的品质
        private object _qulity;

        //opc 通信时，在client端的句柄
        private int _clientHandle;

        //opc 通信时，在server端的句柄
        private int _serverHandle;

        #region get/set
        public string WellName { get { return this._wellName; } set { this._wellName = value; } }
        public string Name { get { return this._name; } set { this._name = value; } }
        public object Value { get { return this._value; } set { this._value = value; } }
        public object TimeStamp { get { return this._timeStamp; } set { this._timeStamp = value; } }
        public object Qulity { get { return this._qulity; } set { this._qulity = value; } }
        public int ClientHandle { get { return this._clientHandle; } set { this._clientHandle = value; } }
        public int ServerHandle { get { return this._serverHandle; } set { this._serverHandle = value; } }
        #endregion


        public Point(string name, string well, int clientHandle)
        {
            this._name = name;
            this._wellName = well;
            this._clientHandle = clientHandle;
        }

    }
}
