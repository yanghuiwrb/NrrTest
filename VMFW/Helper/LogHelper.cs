using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VMFW.Operate;

namespace VMFW.Helper
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class LogHelper
    {
        public static ILog Log { get; } = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Writes an Info level logging message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void Info(object message)
        {
            //Log.InfoFormat("【{0}】【{1}】【{2}】【{3}】", sourceFilePath.Split('\\').Last(), sourceLineNumber, memberName, message);
            Log.Info(message);
        }

        /// <summary>
        /// Writes an error level logging message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void Error(object message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log.ErrorFormat("【{0}】【{1}】【{2}】【{3}】", sourceFilePath.Split('\\').Last(), sourceLineNumber, memberName, message);
        }

        static LogHelper()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
