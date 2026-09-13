using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.xml", Watch = true)]
namespace EBM2x.Utils
{
    public class LogWriter
    {
        #region 변수/상수
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string GetLogDirectory()
        {
            string directory = "log";

            //string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //"C:\Users\jcna2\AppData\Local\Packages\f61cc05d-0a27-4757-ac27-82863b16546a_k4dq9n6dmk6w2\LocalState\master"
            ISave iSave = DependencyService.Get<ISave>();
            if (iSave == null)
            {
            }
            string localApplicationData = iSave.GetEBM2xDataFolderPath();

            string pathName = Path.Combine(localApplicationData, directory);
            if (!Directory.Exists(pathName))
            {
                Directory.CreateDirectory(pathName);
            }

            return pathName;
        }

        #endregion

        #region Error Log - Exception Log
        /// <summary>
        /// ERROR로그 기록
        /// </summary>
        /// <param name="strMsg"></param>
        public static void ErrorLog(string strMsg)
        {
            log.ErrorFormat("[{0}] - {1}", GetParentNamespace(), strMsg);
        }

        public static void ErrorLog(string strMsg, Exception ex)
        {
            log.Error(strMsg, ex);
        }
        #endregion

        #region Debug Log - DEBUG 일 경우 처리 로그
        /// <summary>
		/// DEBUG로그 기록
		/// </summary>
		/// <param name="strData"></param>
        public static void DebugLog(string strMsg)
        {

#if (DEBUG)
            log.DebugFormat("[{0}] - {1}", GetParentNamespace(), strMsg);
#endif
        }
        #endregion

        #region 입력 로그
        /// <summary>
        /// INPUT로그 기록
        /// </summary>
        /// <param name="strMsg"></param>
        public static void InputLog(string strData)
        {
            //			logWriter(_INPUT, " " + strData);
        }
        #endregion

        #region 통신 로그
        /// <summary>
		/// 통신로그기록
		/// </summary>
		/// <param name="strFlag">"SEND","RECV"</param>
		/// <param name="strMsg">전문 내용</param>
		public static void SendLog(string ip, int port, byte[] buffer)
        {
            SendLog(ip, port, System.Text.Encoding.Default.GetString(buffer, 0, buffer.Length));
        }

        public static void SendLog(string ip, int port, string strmsg)
        {
            log.WarnFormat("[{0}:{1}][SEND] - [{2}]", ip.PadRight(11, ' '), port.ToString().PadRight(6, ' '), strmsg);
        }

        public static void ReceiveLog(string ip, int port, byte[] buffer)
        {
            ReceiveLog(ip, port, System.Text.Encoding.Default.GetString(buffer, 0, buffer.Length));
        }

        public static void ReceiveLog(string ip, int port, string strmsg)
        {
            log.WarnFormat("[{0}:{1}][RECV] - [{2}]", ip.PadRight(11, ' '), port.ToString().PadRight(6, ' '), strmsg);

        }
        #endregion

        #region Trace Log
        /// <summary>
		/// TRACE로그 기록
		/// </summary>
		/// <param name="nameSpace"></param>
		/// <param name="className"></param>
		/// <param name="functionName"></param>
		/// <param name="strMsg"></param>
		public static void TraceLog(string nameSpace, string className, string functionName, string strMsg)
        {
            log.InfoFormat("[{0}.{1}.{2}] - {3} ", nameSpace, className, functionName, strMsg);

        }

        public static void TraceLog(string strMsg)
        {
            log.InfoFormat("[{0}] - {1}", GetParentNamespace(), strMsg);
        }
        #endregion

        #region 기타
        /// <summary>
		/// 로그 기록
		/// </summary>
		/// <param name="section"></param>
		/// <param name="text"></param>
		public static void logWriter(string section, string text)
        {
            log.InfoFormat("[{0}] - [{1}] {2}", GetParentNamespace(), section, text);
        }

        public static void ComLogWriter(string filename, string text)
        {
            System.IO.StreamWriter sw = null;

            try
            {
                sw = new System.IO.StreamWriter(GetLogDirectory() + "/" + filename, true, System.Text.Encoding.Default);
                sw.WriteLine(text);
            }
            catch
            {
            }
            finally
            {
                sw.Close();
            }

        }

        /// <summary>
        /// 호출한 상위 메소드 Get
        /// </summary>
        /// <returns></returns>
        private static string GetParentNamespace()
        {
            System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(2, true);
            return string.Format("{0}.{1}.{2}", sf.GetMethod().ReflectedType.Namespace, sf.GetMethod().DeclaringType.Name, sf.GetMethod().Name);
        }

        #endregion
    }
}
