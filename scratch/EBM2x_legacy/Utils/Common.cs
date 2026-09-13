using EBM2x.Datafile.env;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace EBM2x.Utils
{
    public class Common
    {
        /// <summary>
        /// 내용: 한글을 2BYTE로 SIZE만큼 처리
        /// 이력:
        /// </summary>
        /// <param name="size"></param>
        /// 처리할 SIZE
        /// <param name="str"></param>
        /// 처리할 문자열
        /// <returns></returns>
        /// 처리된 문자열
        public static string StrCopy(int size, string str)
        {
            int strLeng = 0;
            string strTemp = string.Empty;

            foreach (char ch in str)
            {
                // 한글인지 체크
                if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    if ((strLeng + 2) <= size)
                    {
                        strTemp += ch;
                        strLeng += 2;
                    }
                    else
                        break;
                }
                else
                {
                    if ((strLeng + 1) <= size)
                    {
                        strTemp += ch;
                        strLeng += 1;
                    }
                    else
                        break;
                }
            }

            return strTemp;

            //			string spacebuff = string.Empty;
            //			for(int i=0;i< size - strLeng;i++) spacebuff += ' ';
            //
            //			return spacebuff + strTemp;
        }

        /// <summary>
        /// 내용: 한글을 2BYTE로 SIZE만큼 처리(왼쪽을 공백으로 처리)
        /// 이력:
        /// </summary>
        /// <param name="size"></param>
        /// 처리할 SIZE
        /// <param name="str"></param>
        /// 처리할 문자열
        /// <returns></returns>
        /// 처리된 문자열
        public static string RStrCopy(int size, string str)
        {
            int strLeng = 0;
            string strTemp = string.Empty;

            foreach (char ch in str)
            {
                // 한글인지 체크
                if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    if ((strLeng + 2) <= size)
                    {
                        strTemp += ch;
                        strLeng += 2;
                    }
                    else
                        break;
                }
                else
                {
                    if ((strLeng + 1) <= size)
                    {
                        strTemp += ch;
                        strLeng += 1;
                    }
                    else
                        break;
                }
            }

            string spacebuff = string.Empty;
            for (int i = 0; i < size - strLeng; i++) spacebuff += ' ';

            return spacebuff + strTemp;
        }

        /// <summary>
        /// 왼쪽을 공백으로 처리(한글 2byte로 인식)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string rpad(int size, string str)
        {
            int strLeng = 0;
            string strTemp = string.Empty;

            foreach (char ch in str)
            {
                // 한글인지 체크
                if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    if ((strLeng + 2) <= size)
                    {
                        strTemp += ch;
                        strLeng += 2;
                    }
                    else
                        break;
                }
                else
                {
                    if ((strLeng + 1) <= size)
                    {
                        strTemp += ch;
                        strLeng += 1;
                    }
                    else
                        break;
                }
            }

            string spacebuff = string.Empty;
            for (int i = 0; i < size - strLeng; i++) spacebuff += ' ';

            return spacebuff + strTemp;
        }

        /// <summary>
        /// 오른쪽을 공백으로 처리(한글 2byte로 인식)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string lpad(int size, string str)
        {
            int strLeng = 0;
            string strTemp = string.Empty;

            foreach (char ch in str)
            {
                // 한글인지 체크
                if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    if ((strLeng + 2) <= size)
                    {
                        strTemp += ch;
                        strLeng += 2;
                    }
                    else
                        break;
                }
                else
                {
                    if ((strLeng + 1) <= size)
                    {
                        strTemp += ch;
                        strLeng += 1;
                    }
                    else
                        break;
                }
            }

            string spacebuff = string.Empty;
            for (int i = 0; i < size - strLeng; i++) spacebuff += ' ';

            return strTemp + spacebuff;
        }

        public static string lpad(int size, int val)
        {
            return string.Format("{0:#,##0}", val).PadLeft(size, ' ');
        }

        // 날짜체크
        public static bool isDateType(string strDate)
        {
            if (!(strDate.Length == 8 || strDate.Length == 14)) return false;

            string strTime = string.Empty;
            if (strDate.Length == 8) strTime = "00:00:00";
            else strTime = strDate.Substring(8, 6);

            string strDateTime = strDate.Substring(4, 2) + "/"
                + strDate.Substring(6, 2) + "/"
                + strDate.Substring(0, 4) + " "
                + strTime;
            try
            {
                System.DateTime.Parse(strDateTime);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// v2.1_문자열중에 숫자만 RETURN처리
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string getNumber(string str)
        {
            string strTemp = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        strTemp = strTemp + str[i].ToString();
                        break;
                    default:
                        break;
                }
            }
            return strTemp;
        }

        /// <summary>
        /// v2.1_문자열중에 숫자만 RETURN처리
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string getNumeric(string str)
        {
            string strTemp = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '.':
                        strTemp = strTemp + str[i].ToString();
                        break;
                    default:
                        break;
                }
            }
            return strTemp;
        }

        /// <summary>
        /// v3.3_프로세스 실행여부 확인
        /// </summary>
        /// <returns></returns>
        public static bool getProcess(string name)
        {
            //실행중인 프로그램중 현재 기동한 프로그램과 같은 프로그램들 수집
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName(name);

            if (p.Length >= 1) return true;
            else return false;

        }

        /// <summary>
        /// 프로세스 실행
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public static bool startProcess(string path, string file, string parm, bool wait)
        {
            try
            {
                // 최신버전이 존재하면 update프로그램을 실행
                string ExecutablePath = path + "/" + file;
                System.Diagnostics.Process ps = new System.Diagnostics.Process();
                ps.StartInfo.FileName = file;
                ps.StartInfo.WorkingDirectory = path;
                ps.StartInfo.Arguments = parm;
                ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                ps.Start();
                if (wait) ps.WaitForExit(5000);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 프로세스 종료
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public static bool killProcess(string ExecFileName)
        {
            try
            {
                System.Diagnostics.Process[] myProcesses;
                myProcesses = System.Diagnostics.Process.GetProcessesByName(ExecFileName);
                foreach (System.Diagnostics.Process myProcess in myProcesses)
                {
                    myProcess.Kill();
                    break;
                }
                //System.Threading.Thread.Sleep(3000);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// String을 Int로 Conv처리
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int StrToInt(string num)
        {
            int vNum = 0;
            try
            {

                vNum = int.Parse(getNumeric(num));
            }
            catch
            {
            }
            return vNum;
        }

        /// <summary>
        /// String을 Float로 Conv처리
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float StrToFloat(string num)
        {
            float vNum = 0;
            try
            {

                vNum = float.Parse(getNumeric(num));
            }
            catch
            {
            }
            return vNum;
        }

        public static DateTime StrToDate(string strDate)
        {
            if (!(strDate.Length == 8 || strDate.Length == 14))
                return System.DateTime.MinValue;

            string strTime = string.Empty;
            if (strDate.Length == 8) strTime = "00:00:00";
            else strTime = strDate.Substring(8, 2) + ":" +
strDate.Substring(10, 2) + ":" +
strDate.Substring(12, 2);

            string strDateTime = strDate.Substring(4, 2) + "/"
                + strDate.Substring(6, 2) + "/"
                + strDate.Substring(0, 4) + " "
                + strTime;
            try
            {
                return System.DateTime.Parse(strDateTime);
            }
            catch
            {
                return System.DateTime.MinValue;
            }
        }

        public static string StrToDateFormat(string strDate)
        {
            if (strDate.Length != 8)
                return "1900-01-01";

            return strDate.Substring(0, 4) + "-" + strDate.Substring(4, 2) + "-" + strDate.Substring(6, 2);
        }

        public static string StrToDateFormat(string strDate, string flag)
        {
            if (strDate.Length != 8)
                return "1900" + flag + "01" + flag + "01";

            return strDate.Substring(0, 4) + flag + strDate.Substring(4, 2) + flag + strDate.Substring(6, 2);
        }

        public static string StrToDateTimeFormat(string strDateTime)
        {
            if (strDateTime.Length != 14)
                return "1900-01-01 00:00:00";

            return strDateTime.Substring(0, 4) + "-" + strDateTime.Substring(4, 2) + "-" + strDateTime.Substring(6, 2) + " " +
                strDateTime.Substring(8, 2) + ":" + strDateTime.Substring(10, 2) + ":" + strDateTime.Substring(12, 2);
        }

        /// <summary>
        /// 주어진 기준일자에 VALUE를 적용한 일자를 반환한다.
        /// </summary>
        /// <param name="strDateTime">기준일자("yyyyMMddHHmmss")</param>
        /// <param name="p_Value">변경값(예:+10,-10)</param>
        /// <returns></returns>
        public static DateTime toDateTime(string strDateTime, int p_Value)
        {
            if (!(strDateTime.Length == 8 || strDateTime.Length == 14))
                return System.DateTime.MinValue;

            string strTime = string.Empty;
            if (strDateTime.Length == 8) strTime = "00:00:00";
            else strTime = strDateTime.Substring(8, 6);

            string m_DateTime = strDateTime.Substring(4, 2) + "/"
                + strDateTime.Substring(6, 2) + "/"
                + strDateTime.Substring(0, 4) + " "
                + strTime;
            try
            {
                DateTime cnv_DateTime = System.DateTime.Parse(m_DateTime);
                return cnv_DateTime.AddDays(p_Value);
            }
            catch
            {
                return System.DateTime.MinValue;
            }
        }

        /// <summary>
        /// 파일에 내용을 저장한다.(Append)
        /// </summary>
        /// <param name="filename">저장할 파일명</param>
        /// <param name="text">저장할 내용</param>
        /// <returns></returns>
        public static bool FileWrite(string filename, string text)
        {
            System.IO.StreamWriter sw = null;
            try
            {
                sw = new System.IO.StreamWriter(filename, true, System.Text.Encoding.Default);
                sw.WriteLine(text);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }

        /*
		public static void SendTcpData(string sendData, string ip, int port)
		{
			byte[] sendBytes = new byte[512];
			//byte[] receiveBytes = new byte[512];
			Beans.util.Encoding.memset(sendBytes, sendBytes.Length, (byte)0x20);
			//string sendData = posModel.getRegiTotal().getRegiHeader().getBarcodeText();
			Beans.util.Encoding.memmove(sendBytes,0,sendData,sendData.Length);
			
			//Beans.util.SocketClient socket = new Beans.util.SocketClient("127.0.0.1", 7001);
			Beans.util.SocketClient socket = new Beans.util.SocketClient(ip, port);
			socket.requestTCP(sendBytes);
			sendBytes = null;
		}
		*/


        /*
		/// <summary>
		/// SendMessage
		/// </summary>
		/// <param name="name"></param>
		public static void SendMessage(string name, int id, string msg)
		{
			try{
				Beans.util.Log.TraceLog("FindWindow - Start");
				IntPtr hwnd = FindWindow(null, name);
				Beans.util.Log.TraceLog("FindWindow - Start");
				//IntPtr tempPtr;

				if (hwnd != IntPtr.Zero)
				{
					Beans.util.Log.TraceLog("SendMessage - Start");
					SendMessage(hwnd, id, 0, 0);
					Beans.util.Log.TraceLog("SendMessage - Start");
				}
			}
			catch{
			}
		}
		 */


        /// <summary>
        /// 한글 2바이트 처리
        /// </summary>
        /// <param name="src"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public static string getStringByByte(string src, int byteCount)
        {
            if (src.Length == 0) return src;

            System.Text.Encoding myEncoding = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
            byte[] buf = myEncoding.GetBytes(src);

            if (byteCount >= buf.Length)
            {
                return src + string.Empty.PadRight(byteCount - buf.Length);
            }

            int nI = 0;
            while (nI < byteCount)
            {
                if (buf[nI] >= 0x80)
                {
                    nI++;
                }
                nI++;
            }

            int nCopyLen = (nI <= byteCount) ? nI : nI - 2;
            if (nCopyLen >= 1)
            {
                return myEncoding.GetString(buf, 0, nCopyLen);
            }
            return src;
        }

        // JINIT_이미지파일 여부 확인
        bool IsValidImage(string filename)
        {
            /*
            try
            {
                using (Image newImage = Image.FromFile(filename))
                { }
            }
            catch (OutOfMemoryException ex)
            {
                //The file does not have a valid image format.
                //-or- GDI+ does not support the pixel format of the file

                return false;
            }
            */
            return true;
        }

        // changed by clinton 
        public static string getAdminPass()
        {
            //string toDay = System.DateTime.Now.ToString("ddMMyyyy");
            //string mm = toDay.Substring(2, 2);
            //string dd = toDay.Substring(0, 2);
            //int temp = (int.Parse(dd) * int.Parse(mm)) % 10;
            //string AdminPass = dd + mm + temp.ToString();

            InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc(); //add by clinton
            string mrcno = initInfoVO.mrcNo;
            string last3 = mrcno.Substring(mrcno.Length - 3);
            string toDay = System.DateTime.Now.ToString("ddMMyyyy");
            string mm = toDay.Substring(2, 2);
            string dd = toDay.Substring(0, 2);
            int temp = int.Parse(dd)  % 10;
            string AdminPass = last3 + dd + mm + temp.ToString();



            return AdminPass;
        }
        public static string getAdminPassII()
        {
            InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
            string mrcno = initInfoVO.mrcNo;
            string last3 = mrcno.Substring(mrcno.Length - 3);
            string toDay = System.DateTime.Now.ToString("ddMMyyyy");
            string mm = toDay.Substring(2, 2);
            string dd = toDay.Substring(0, 2);
            int temp = (int.Parse(dd) * int.Parse(mm)) % 10;
            string AdminPass = last3 + dd + mm + temp.ToString();

            return AdminPass;
        }

        //===============================================================================
        // JINIT_20191210, 이미지처리 함수 추가
        public static byte[] FileToByteArray(string path)
        {
            byte[] fileBytes = null;
            try
            {
                fileBytes = System.IO.File.ReadAllBytes(path);
                
                //FileStream fileStream = new FileStream(path, FileMode.Open);
                //fileBytes = new byte[fileStream.Length];
                //fileStream.Read(fileBytes, 0, fileBytes.Length);
            }
            catch (Exception e)
            {
            }
            return fileBytes;
        }
        //===============================================================================

        public static string AESEncrypt256(string Password, string _text)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            byte[] PlainText = Encoding.Unicode.GetBytes(_text);
            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(PlainText, 0, PlainText.Length);
            cryptoStream.FlushFinalBlock();

            byte[] CipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(CipherBytes);
        }

        public static string AESDecrypt256(string Password, string _text)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] EncryptedData = Convert.FromBase64String(_text);
            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(EncryptedData);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

            byte[] PlainText = new byte[EncryptedData.Length];
            int DecrypedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
            memoryStream.Close();
            cryptoStream.Close();

            return Encoding.Unicode.GetString(PlainText, 0, DecrypedCount);
        }
        public static DateTime DateFormat(string yyyymmdd)
        {
            //return DateTime.Now;
            if (string.IsNullOrEmpty(yyyymmdd) || yyyymmdd.Length != 8) return DateTime.Now;
            string posTime = yyyymmdd.Substring(0, 4) + "/" + yyyymmdd.Substring(4, 2) + "/" + yyyymmdd.Substring(6, 2);
            return System.Convert.ToDateTime(posTime);
        }
        public static DateTime DateTimeFormat(string yyyyMMddHHmmssmmm)
        {
            //return DateTime.Now;
            if (string.IsNullOrEmpty(yyyyMMddHHmmssmmm) || yyyyMMddHHmmssmmm.Length != 14) return DateTime.Now;
            string posTime = yyyyMMddHHmmssmmm.Substring(0, 4) + "/" + yyyyMMddHHmmssmmm.Substring(4, 2) + "/" + yyyyMMddHHmmssmmm.Substring(6, 2) + " " +
                             yyyyMMddHHmmssmmm.Substring(8, 2) + ":" + yyyyMMddHHmmssmmm.Substring(10, 2) + ":" + yyyyMMddHHmmssmmm.Substring(12, 2);
            return System.Convert.ToDateTime(posTime);
        }

        public static string AES256Password()
        {
            return "PDFKFKFMVHTMZKW";
        }

        public static SocketModel Send(SocketModel socketRequestModel, string ip, int port)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ReceiveTimeout = 15000;
                socket.SendTimeout = 15000;

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    socket.Connect(remoteEP);

                    //=================================================  
                    // Encode the data string into a byte array.  
                    string jsonRequest = JsonConvert.SerializeObject(socketRequestModel, Formatting.Indented);
                    byte[] data = Encoding.UTF8.GetBytes(jsonRequest);

                    // Send the data through the socket.  
                    socket.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);
                    int bytesSent = socket.Send(data);

                    //=================================================  
                    // receive data size 
                    byte[] sizeBuf = new byte[4];
                    socket.Receive(sizeBuf, 0, sizeBuf.Length, 0);
                    int size = BitConverter.ToInt32(sizeBuf, 0);

                    MemoryStream ms = new MemoryStream();

                    while (size > 0)
                    {
                        byte[] buffer;
                        if (size < socket.ReceiveBufferSize) buffer = new byte[size];
                        else buffer = new byte[socket.ReceiveBufferSize];

                        int rec = socket.Receive(buffer, 0, buffer.Length, 0);

                        size -= rec;

                        ms.Write(buffer, 0, buffer.Length);
                    }

                    ms.Close();

                    byte[] dataRecv = ms.ToArray();
                    string recvData = Encoding.UTF8.GetString(dataRecv);

                    // Release the socket.  
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    return JsonConvert.DeserializeObject<SocketModel>(recvData);
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception es)
                {
                    Console.WriteLine("Unexpected exception : {0}", es.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            SocketModel socketResponseModel = new SocketModel();
            socketResponseModel.WCC = "TIMEOUT";
            socketResponseModel.JsonRequest = "";
            return socketResponseModel;
        }
    }
}
