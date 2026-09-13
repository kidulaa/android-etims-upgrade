using EBM2x.Database;
using EBM2x.Database.Master;
using EBM2x.Datafile.env;
using EBM2x.Datafile.trlog;
using EBM2x.Models;
using EBM2x.Models.config;
using EBM2x.Models.ListView;
using EBM2x.Process.start;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.Services;
using EBM2x.UI;
using EBM2x.UI.Setup;
using EBM2x.UI.Tablet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace EBM2x
{
    public partial class AppTablet : Application
    {
        public bool listenFlag = false;

        public Thread myListener = null;
        public Thread myServer = null;
        public Socket listener = null;

        public int thcnt = 0; // 소켓연결제한갯수
        
        public UIManager __UIManager = null;

        public AppTablet(bool isWindows, bool isMysql, bool isMobile)
        {
            __UIManager = UIManager.Instance();
            __UIManager.IsWindows = isWindows;
            __UIManager.IsMySQL = isMysql;
            // 2021.01 Mobile
            __UIManager.IsMobile = isMobile;

            InitializeComponent();

            MessagingCenter.Subscribe<Object, string>(this, "It was entered Number", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Append(arg);
                });
            });

            MessagingCenter.Subscribe<Object, string>(this, "It was entered Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    if (arg.Equals("Backspace"))
                    {
                        if (UIManager.Instance().InputModel.EnteredText.Length > 0)
                        {
                            UIManager.Instance().InputModel.DeleteLastOne();
                        }
                        else
                        {
                            MessagingCenter.Send<Object, string>(this, "Function", arg);
                        }
                    }
                    else
                    {
                        MessagingCenter.Send<Object, string>(this, "Function", arg);
                    }
                });
            });

            if (!EnvRraSdcService.IsEnvRraSdc() || !EnvPosSetupService.IsEnvPosSetup())
            {
                //
               // using (System.IO.StreamWriter file = new System.IO.StreamWriter("/storage/emulated/0/EBM2xData/test.json", false))
               // {
               //     file.Write("1234567890");
               // }

                // 환경설정파일이 존재하지 않습니다
                MainPage = new NavigationPage(new EBM2xSetupPage());
            }
            else
            {
                string result = "";
                result = Initialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

                UIManager.Instance().PosModel.Environment.EnvPosSetup.Init();
                EnvPosSetupService.SaveEnvPosSetup(UIManager.Instance().PosModel.Environment.EnvPosSetup);

                int days = UIManager.Instance().PosModel.Environment.EnvPosSetup.GetDayCount();
                if(days == 0)
                {
                    StockIoMaster stockIoMaster = new StockIoMaster();
                    UIManager.Instance().PosModel.Environment.EnvPosSetup.ChangePosInstallDate(stockIoMaster.GetStockIoDate());
                    EnvPosSetupService.SaveEnvPosSetup(UIManager.Instance().PosModel.Environment.EnvPosSetup);
                }

                //=========================================================================================
                // Z-REPORT Table 생성
                Ebm2xCreateTables.CreateZreportTables();

                //=========================================================================================

                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.PrinterPaperSize.Equals("80mm"))
                {
                    // 영수증 사이즈를 80mm로 설정
                    __UIManager.Is58mmPrinter = false;
                }
                else
                {
                    // 영수증 사이즈를 58mm로 설정
                    __UIManager.Is58mmPrinter = true;
                }

                // ================================================================================== //
                if (__UIManager.IsWindows && __UIManager.IsMySQL && __UIManager.PosModel.Environment.EnvPosSetup.MySQLServerType.Equals("Master"))
                {
                    StartListening();
                }
                // ================================================================================== //

                //// DEBUG
                //UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT = false;
                //EnvPosSetupService.SaveEnvPosSetup(UIManager.Instance().PosModel.Environment.EnvPosSetup);

                MainPage = new NavigationPage(new SalesMenuPage());
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        // MySQL ================================================================================== //

        public bool StartListening()
        {
            try
            {
                listenFlag = true;

                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11129);

                // Create a TCP/IP socket.
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.

                listener.Bind(localEndPoint);

                listener.Listen(0); // 소갯대기열을 0으로설정

                myListener = new Thread(new ParameterizedThreadStart(ListenerStart));
                myListener.Start(listener);

                return true;

            }
            catch (ThreadAbortException)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void ListenerStart(object socket)
        {
            Socket listen = (Socket)socket;
            while (listenFlag)
            {

                try
                {
                    int delayCount = 200;
                    try
                    {
                        Socket handler = listen.Accept();

                        // 소켓연결갯수 제한(쓰레드 생성을 하지 않음)
                        if (thcnt < delayCount)
                        {
                            myServer = new Thread(new ParameterizedThreadStart(ServerStart));
                            myServer.Start(handler);

                            thcnt += 1; // 소켓연결갯수
                        }
                        else
                        {
                            handler.Close();
                        }
                    }
                    catch(System.Net.Sockets.SocketException se)
                    {
                        break;
                    }
                }
                catch (System.ObjectDisposedException ex)
                {
                    // listener가 끊어진 경우에는 쓰레드를 종료시킴
                    break;
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        private void ServerStart(object socket)
        {
            Socket handler = (Socket)socket;

            // client socket ip
            string m_Ip = ((System.Net.IPEndPoint)(handler.RemoteEndPoint)).Address.ToString();

            try
            {
                byte[] sizeBuf = new byte[4];

                while (listenFlag)
                {

                    if (handler.Connected)
                    {
                        //=================================================  
                        // receive data size 구하기
                        handler.Receive(sizeBuf, 0, sizeBuf.Length, 0);
                        int size = BitConverter.ToInt32(sizeBuf, 0);

                        MemoryStream ms = new MemoryStream();
                        while (size > 0)
                        {
                            byte[] buffer;
                            if (size < handler.ReceiveBufferSize) buffer = new byte[size];
                            else buffer = new byte[handler.ReceiveBufferSize];

                            int rec = handler.Receive(buffer, 0, buffer.Length, 0);

                            size -= rec;

                            ms.Write(buffer, 0, buffer.Length);
                        }
                        ms.Close();
                        byte[] data = ms.ToArray();
                        ms.Dispose();

                        string recvData = Encoding.UTF8.GetString(data);
                        string respData = ExcuteProcess(m_Ip, recvData);

                        //=================================================  
                        // Encode the data string into a byte array.  
                        byte[] respBytes = Encoding.UTF8.GetBytes(respData);

                        // return data size 구하기
                        // Send the data through the socket.  
                        handler.Send(BitConverter.GetBytes(respBytes.Length), 0, 4, 0);
                        int bytesSent = handler.Send(respBytes);
                    }
                    else
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException ex)
            {
            }
            catch (Exception ex)
            {
            }

            finally
            {
                if (handler != null)
                {
                    handler.Close();
                    handler = null;
                }

                thcnt -= 1; // 소켓연결갯수
                if (thcnt < 0) thcnt = 0;
            }
        }

        // 수신 및 송신 데이타 처리
        public string ExcuteProcess(string ip, string jsonRequest)
        {
            SocketModel socketResponseModel = new SocketModel();
            //=============================================
            // Data Receive 처리
            //=============================================
            try
            {
                SocketModel socketRequestModel = JsonConvert.DeserializeObject<SocketModel>(jsonRequest);

                if (socketRequestModel.WCC.Equals("RraSdcUploadModel"))
                {
                    RraSdcUploadModel rraSdcUploadModel = JsonConvert.DeserializeObject<RraSdcUploadModel>(socketRequestModel.JsonRequest);
                    if (RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel))
                    {
                        socketResponseModel.WCC = "SUCCESS";
                        socketResponseModel.JsonRequest = "The file save was successful.";
                    }
                    else
                    {
                        socketResponseModel.WCC = "ERROR";
                        socketResponseModel.JsonRequest = "File save failed.";
                    }

                    string jsonResponse = JsonConvert.SerializeObject(socketResponseModel, Formatting.Indented);
                    return jsonResponse;
                }
                else if (socketRequestModel.WCC.Equals("TranModel"))
                {
                    TranModel tranModel = JsonConvert.DeserializeObject<TranModel>(socketRequestModel.JsonRequest);
                    if (TransactionWriter.write(tranModel))
                    {
                        socketResponseModel.WCC = "SUCCESS";
                        socketResponseModel.JsonRequest = "The file save was successful.";
                    }
                    else
                    {
                        socketResponseModel.WCC = "ERROR";
                        socketResponseModel.JsonRequest = "File save failed.";
                    }

                    string jsonResponse = JsonConvert.SerializeObject(socketResponseModel, Formatting.Indented);
                    return jsonResponse;
                }
                else if (socketRequestModel.WCC.Equals("SearchReceiptNodeList"))
                {
                    try
                    {
                        List<SearchReceiptNode> listTranModelModel = TransactionReader.GetSearchReceiptList(socketRequestModel.JsonRequest);
                        socketResponseModel.WCC = "SUCCESS";
                        socketResponseModel.JsonRequest = JsonConvert.SerializeObject(listTranModelModel, Formatting.Indented);
                    }
                    catch (Exception ex)
                    {
                        socketResponseModel.WCC = "ERROR";
                        socketResponseModel.JsonRequest = "File load failed.";
                    }

                    string jsonResponse = JsonConvert.SerializeObject(socketResponseModel, Formatting.Indented);
                    return jsonResponse;
                }
                else if (socketRequestModel.WCC.Equals("LoadReceiptNode"))
                {
                    try
                    {
                        TranModel tranModel = TransactionReader.readSystempath(socketRequestModel.JsonRequest);
                        socketResponseModel.WCC = "SUCCESS";
                        socketResponseModel.JsonRequest = JsonConvert.SerializeObject(tranModel, Formatting.Indented);
                    }
                    catch (Exception ex)
                    {
                        socketResponseModel.WCC = "ERROR";
                        socketResponseModel.JsonRequest = "File load failed.";
                    }

                    string jsonResponse = JsonConvert.SerializeObject(socketResponseModel, Formatting.Indented);
                    return jsonResponse;
                }
                else if (socketRequestModel.WCC.Equals("OperatorLoad"))
                {
                    OperatorRecord record = OperatorService.Load(socketRequestModel.JsonRequest);
                    if(record != null)
                    {
                        socketResponseModel.WCC = "SUCCESS";
                        socketResponseModel.JsonRequest = JsonConvert.SerializeObject(record, Formatting.Indented);
                    }
                    else
                    {
                        socketResponseModel.WCC = "ERROR";
                        OperatorRecord record2 = new OperatorRecord();
                        socketResponseModel.JsonRequest = JsonConvert.SerializeObject(record2, Formatting.Indented);
                    }

                    string jsonResponse = JsonConvert.SerializeObject(socketResponseModel, Formatting.Indented);
                    return jsonResponse;
                }
                else if (socketRequestModel.WCC.Equals("Overwrite"))
                {
                    bool ret = false;
                    TranModel tranModel = TransactionReader.read(socketRequestModel.JsonRequest);
                    if (tranModel != null)
                    {
                        tranModel.TranInformation.RefundBarcodeNo = socketRequestModel.JsonRequest;
                        ret = TransactionWriter.overwrite(tranModel, socketRequestModel.JsonRequest);
                    }
                    else
                    {
                        tranModel = TransactionReader.read(socketRequestModel.JsonRequest.Substring(1, 8), socketRequestModel.JsonRequest);
                        if (tranModel != null)
                        {
                            tranModel.TranInformation.RefundBarcodeNo = socketRequestModel.JsonRequest;
                            ret = TransactionWriter.overwrite(tranModel, socketRequestModel.JsonRequest.Substring(1, 8), socketRequestModel.JsonRequest);
                        }
                    }
                    if (ret)
                    {
                        socketResponseModel.WCC = "SUCCESS";
                        socketResponseModel.JsonRequest = "The file overwrite was successful.";
                    }
                    else
                    {
                        socketResponseModel.WCC = "ERROR";
                        socketResponseModel.JsonRequest = "File overwrite failed.";
                    }

                    string jsonResponse = JsonConvert.SerializeObject(socketResponseModel, Formatting.Indented);
                    return jsonResponse;
                }
                else if (socketRequestModel.WCC.Equals("InitInfoVO"))
                {
                    InitInfoReq initInfoReq = JsonConvert.DeserializeObject<InitInfoReq>(socketRequestModel.JsonRequest);
                    InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();

                    if(initInfoReq.tin == initInfoVO.tin && initInfoReq.bhfId == initInfoVO.bhfId)
                    {
                        socketResponseModel.WCC = "SUCCESS";
                        socketResponseModel.JsonRequest = JsonConvert.SerializeObject(initInfoVO, Formatting.Indented);
                    }
                    else
                    {
                        socketResponseModel.WCC = "ERROR";
                        socketResponseModel.JsonRequest = "[InitInfo] Parameter error.";
                    }

                    string jsonResponse = JsonConvert.SerializeObject(socketResponseModel, Formatting.Indented);
                    return jsonResponse;
                }
                else
                {
                    socketResponseModel.WCC = "ERROR";
                    socketResponseModel.JsonRequest = "";

                    string jsonResponse = JsonConvert.SerializeObject(socketResponseModel, Formatting.Indented);
                    return jsonResponse;
                }
            }
            catch(Exception ex)
            {
                socketResponseModel.WCC = "ERROR";
                socketResponseModel.JsonRequest = ex.ToString();

                string jsonResponse = JsonConvert.SerializeObject(socketResponseModel, Formatting.Indented);
                return jsonResponse;
            }
        }


        // listener stop
        public bool StopListening()
        {
            try
            {
                listenFlag = false;

                if (myServer != null)
                {
                    myServer.Abort();
                    myServer = null;
                }

                if (listener != null)
                {
                    listener.Close(30);
                    listener = null;
                }

            }
            catch
            {
            }

            thcnt = 0; // 소켓연결제한갯수

            return true;
        }
    }
}
