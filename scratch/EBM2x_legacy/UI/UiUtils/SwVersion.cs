using EBM2x.Datafile.env;
using EBM2x.Datafile.trlog;
using EBM2x.Dependency;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.UI.Draw;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EBM2x.UI.UiUtils
{
    public class SwVersion
    {

        public async void SwVersionDownloadProcess(ContentPage page)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 요청자료 중 'TIN'과 'BHFID'는 RequestHeader로 보내야 하며, 내부에 저장된 'cmcKey'를 읽어서 전송해야 한다.
                    RraSdcService.SetDefaultRequestHeaders(client);

                    //// JSON의 형식으로 자료를 서버에 전송한다.
                    SwVersionReq swVersionReq = RraSdcService.GetDefaultRequestHeaders();
                    // POST 할 데이터를 작성한다.
                    NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
                    outgoingQueryString.Add("tin", swVersionReq.tin);
                    outgoingQueryString.Add("bhfId", swVersionReq.bhfId);
                    outgoingQueryString.Add("dvcSrlNo", swVersionReq.dvcSrlNo);
                    outgoingQueryString.Add("fnlIntlVer", UIManager.AppVersion);

                    string postData = outgoingQueryString.ToString();
         
                    ASCIIEncoding ascii = new ASCIIEncoding();
                    string aaa = postData.ToString();
                    byte[] sendByte = ascii.GetBytes(postData.ToString());

                    string url = RraSdcService.EXTERNAL_URL + "/" + "selectSwVersion";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Timeout = UIManager.TIME_OUT;
                    httpWebRequest.Method = "POST"; //POST
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    httpWebRequest.ContentLength = sendByte.Length;

                    Stream requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(sendByte, 0, sendByte.Length);
                    requestStream.Flush();
                    requestStream.Close();

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader readerPost = new StreamReader(responseStream, Encoding.UTF8);

                    string jsonResponse = readerPost.ReadToEnd().Trim();
                
                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        SwVersionRes swVersionRes = JsonConvert.DeserializeObject<SwVersionRes>(jsonResponse);
                        if (swVersionRes.resultCd.Equals("000"))
                        {
                            UIManager.Instance().IsValidDevice = true;

                            SwVersionResData swVersionResData = swVersionRes.data;
                            if (!swVersionResData.ver.Equals(UIManager.AppVersion))
                            {
                                int curInt = 0;
                                int oldInt = 0;
                                try
                                {
                                    // 신규버젼인 경우 "v20210202.NEW.0121"
                                    string curVersion = UIManager.AppVersion.Substring(UIManager.AppVersion.LastIndexOf('.') + 1);
                                    if (curVersion.Length > 4) curVersion = curVersion.Substring(0, 4);
                                    string oldVersion = swVersionResData.ver.Substring(swVersionResData.ver.LastIndexOf('.') + 1);
                                    if (oldVersion.Length > 4) oldVersion = oldVersion.Substring(0, 4);

                                    curInt = Int16.Parse(curVersion);
                                    oldInt = Int16.Parse(oldVersion);
                                }
                                catch (Exception ev)
                                {
                                }

                                if (curInt >= oldInt)
                                {
                                    // 신규 버젼인 경우 프로그램을 갱신하지 않는다.
                                }
                                else
                                {
                                    bool shutdown = await page.DisplayAlert("Info", "Please update the program.\nAPP:[" + UIManager.AppVersion + "].\nRRA:[" + swVersionResData.ver + "]", "Yes", "No");
                                    if (UIManager.Instance().IsWindows)
                                    {
                                        ICloseApplication closeApplication = DependencyService.Get<ICloseApplication>();
                                        if (closeApplication != null)
                                        {
                                            if (!UIManager.DEBUG_MODE) closeApplication.closeApplication();
                                        }
                                        else
                                        {
                                            await page.DisplayAlert("Info", "Please update the program.", "Ok");
                                        }
                                    }
                                    else
                                    {
                                        string uri = RraSdcService.EXTERNAL_URL + "/" + "indexSwDownloadAndroid";
                                        string parm = "swNo=" + swVersionResData.swNo + "&ver=" + swVersionResData.ver + "&tin=" + swVersionReq.tin + "&bhfId=" + swVersionReq.bhfId + "&dvcSrlNo=" + swVersionReq.dvcSrlNo + "";
                                        await Browser.OpenAsync(uri + "?" + parm, BrowserLaunchMode.SystemPreferred);

                                        ICloseApplication closeApplication = DependencyService.Get<ICloseApplication>();
                                        if (closeApplication != null)
                                        {
                                            if (!UIManager.DEBUG_MODE) closeApplication.closeApplication();
                                        }
                                        else
                                        {
                                            await page.DisplayAlert("Info", "Please update the program.", "Ok");
                                        }
                                    }
                                }
                            }
                        }
                        // 901 : It is not valid device.
                        else if (swVersionRes.resultCd.Equals("901")) 
                        {
                            UIManager.Instance().IsValidDevice = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public async void TaxpayerInfoDownloadProcess(ContentPage page)
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            //RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_TAXPAYER_INFO_SELECT);

            CodeReq codeReq = new CodeReq
            {
                lastReqDt = DateTime.Now.ToString("yyyyMMddHHmmss")    // 최종 수신일자  
            };

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    //// JSON의 형식으로 자료를 서버에 전송한다.
                    string jsonRequest = JsonConvert.SerializeObject(codeReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    // 요청자료 중 'TIN'과 'BHFID'는 RequestHeader로 보내야 하며, 내부에 저장된 'cmcKey'를 읽어서 전송해야 한다.
                    RraSdcService.SetDefaultRequestHeaders(client);

                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_TAXPAYER_INFO_SELECT;
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        TaxpayerInfoRes taxpayerInfoRes = JsonConvert.DeserializeObject<TaxpayerInfoRes>(jsonResponse);
                        if (taxpayerInfoRes.resultCd.Equals("000") && taxpayerInfoRes.data != null && taxpayerInfoRes.data.info != null)
                        {
                            TaxpayerInfo taxpayerInfo = taxpayerInfoRes.data.info;

                            // 변경된 .... 처리
                            if (taxpayerInfo.vatTyCd.Equals("1") || taxpayerInfo.vatTyCd.Equals("2"))
                            {
                                if (taxpayerInfo.vatTyCd.Equals("1"))
                                {
                                    UIManager.Instance().PosModel.Environment.EnvPosSetup.ChangeNonVAT = true;
                                    UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT = false;

                                    // Added By Bright 21.6.2021
                                    InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
                                    if (!string.IsNullOrEmpty(taxpayerInfo.taxprNm) && initInfoVO.taxprNm != taxpayerInfo.taxprNm)
                                    {
                                        initInfoVO.taxprNm = taxpayerInfo.taxprNm;
                                        EnvRraSdcService.SaveEnvRraSdc(initInfoVO);
                                    }
                                    // Added By Bright

                                    EnvPosSetupService.SaveEnvPosSetup(UIManager.Instance().PosModel.Environment.EnvPosSetup);
                                }
                                else if (taxpayerInfo.vatTyCd.Equals("2"))
                                {
                                    UIManager.Instance().PosModel.Environment.EnvPosSetup.ChangeNonVAT = true;
                                    UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT = true;

                                    // Added By Bright 21.6.2021
                                    InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
                                    if (!string.IsNullOrEmpty(taxpayerInfo.taxprNm) && initInfoVO.taxprNm != taxpayerInfo.taxprNm)
                                    {
                                        initInfoVO.taxprNm = taxpayerInfo.taxprNm;
                                        EnvRraSdcService.SaveEnvRraSdc(initInfoVO);
                                    }
                                    // Added By Bright

                                    EnvPosSetupService.SaveEnvPosSetup(UIManager.Instance().PosModel.Environment.EnvPosSetup);
                                }
                            }
                        }
                        else if (taxpayerInfoRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_TAXPAYER_INFO_SELECT, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

    }
}
