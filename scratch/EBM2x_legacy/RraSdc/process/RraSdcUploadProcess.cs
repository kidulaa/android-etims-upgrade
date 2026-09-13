using EBM2x.Datafile.env;
using EBM2x.Datafile.trlog;
using EBM2x.Models;
using EBM2x.RraSdc.model;
using EBM2x.UI;
using EBM2x.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EBM2x.RraSdc.process
{
    public class RraSdcUploadProcess
    {
        public void UploadProcess()
        {
            UploadProcess(false);
        }

        
        public void UploadProcess(bool backup)
        {
            if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL && UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServerType.Equals("Slave"))
            {
                UploadSocketProcess(backup);

                Task.Delay(TimeSpan.FromSeconds(0.1));

                UploadTranNodeSocketProcess(backup);
            }
            else
            {
                UploadHttpProcess(backup);
            }
        }
        public void UploadClearProcess(bool backup)
        {
            UploadClearHttpProcess(backup);
        }

        public async void UploadHttpProcess(bool backup)
        {
            try
            {
                List<RraSdcUploadModel> listRraSdcUploadModel = RraSdcJsonWriter.GetTransactionList();
                for (int i = 0; i < listRraSdcUploadModel.Count; i++)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        RraSdcService.SetDefaultRequestHeaders(client);

                        await Task.Delay(TimeSpan.FromSeconds(0.1));
                        RraSdcUploadModel rraSdcUploadModel = listRraSdcUploadModel[i];

                        string url = RraSdcService.EXTERNAL_URL + "/" + rraSdcUploadModel.FunctionName;
                        string jsonRequest = rraSdcUploadModel.JsonRequest;
                        //Added By Bright 23.4.2022
                        //TIN From Config and Tin from JSON File must be the same
                        JObject parsedjson = JObject.Parse(jsonRequest);
                        InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();

                        if (initInfoVO.tin.Equals(parsedjson["tin"].ToString())) ////Added IF By Bright 23.4.2022
                        {

                        HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(url, content);
                        if (response != null)
                        {
                            string jsonResponse = response.Content.ReadAsStringAsync().Result;
                            RraSdcUploadRes rraSdcUploadRes = JsonConvert.DeserializeObject<RraSdcUploadRes>(jsonResponse);

                            rraSdcUploadModel.ResultCd = rraSdcUploadRes.resultCd;
                            rraSdcUploadModel.ResultMsg = rraSdcUploadRes.resultMsg;
                            rraSdcUploadModel.ResultDt = rraSdcUploadRes.resultDt;
                            if (rraSdcUploadRes.resultCd.Equals("000") || rraSdcUploadRes.resultCd.Equals("994"))
                            {
                                RraSdcJsonWriter.BackupSuccessFile(rraSdcUploadModel);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(rraSdcUploadRes.resultCd) && !rraSdcUploadRes.resultCd.Equals("999"))
                                {
                                    //if (backup)
                                    //{
                                    //    //RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    //}
                                    //else 
                                    if (rraSdcUploadRes.resultCd.Equals("910") && rraSdcUploadModel.ResultMsg.IndexOf("<cfmDt>") > 0)
                                    {
                                        RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    }
                                    else if (rraSdcUploadRes.resultCd.Equals("910") && rraSdcUploadModel.ResultMsg.IndexOf("<totItemCnt>") > 0)
                                    {
                                        RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    }
                                    else if (rraSdcUploadRes.resultCd.Equals("910") && rraSdcUploadModel.ResultMsg.IndexOf("<email>") > 0)
                                    {
                                        RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    }
                                    else if (rraSdcUploadRes.resultCd.Equals("922") && rraSdcUploadModel.ResultMsg.IndexOf("invoice data can be received") > 0)
                                    {
                                        RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            DateTime oDate = DateTime.ParseExact(rraSdcUploadModel.RequestDt, "yyyyMMddHHmmss", null);
                                            DateTime cDate = DateTime.ParseExact("20200531235959", "yyyyMMddHHmmss", null);
                                            if (oDate < cDate)
                                            {
                                                RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                else if (rraSdcUploadRes.resultCd.Equals("999") && rraSdcUploadModel.FunctionName.Equals("saveItem"))
                                {
                                    RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                }
                                else
                                {
                                    //string 
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        }  //END IF
                        else
                        {
                            RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                            
                        }//END ELSE
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async void UploadClearHttpProcess(bool backup)
        {
            try
            {
                List<RraSdcUploadModel> listRraSdcUploadModel = RraSdcJsonWriter.GetTransactionList();
                for (int i = 0; i < listRraSdcUploadModel.Count; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.1));
                    RraSdcUploadModel rraSdcUploadModel = listRraSdcUploadModel[i];
                    RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async void UploadHttpProcessII(bool backup)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    RraSdcService.SetDefaultRequestHeaders(client);

                    List<RraSdcUploadModel> listRraSdcUploadModel = RraSdcJsonWriter.GetTransactionList();
                    for (int i = 0; i < listRraSdcUploadModel.Count; i++)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.1));
                        RraSdcUploadModel rraSdcUploadModel = listRraSdcUploadModel[i];

                        string url = RraSdcService.EXTERNAL_URL + "/" + rraSdcUploadModel.FunctionName;
                        string jsonRequest = rraSdcUploadModel.JsonRequest;
                        HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(url, content);
                        if (response != null)
                        {
                            string jsonResponse = response.Content.ReadAsStringAsync().Result;
                            RraSdcUploadRes rraSdcUploadRes = JsonConvert.DeserializeObject<RraSdcUploadRes>(jsonResponse);

                            rraSdcUploadModel.ResultCd = rraSdcUploadRes.resultCd;
                            rraSdcUploadModel.ResultMsg = rraSdcUploadRes.resultMsg;
                            rraSdcUploadModel.ResultDt = rraSdcUploadRes.resultDt;
                            if (rraSdcUploadRes.resultCd.Equals("000") || rraSdcUploadRes.resultCd.Equals("994"))
                            {
                                RraSdcJsonWriter.BackupSuccessFile(rraSdcUploadModel);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(rraSdcUploadRes.resultCd) && !rraSdcUploadRes.resultCd.Equals("999"))
                                {
                                    //if (backup)
                                    //{
                                    //    //
                                    //    //RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    //}
                                    //else 
                                    if (rraSdcUploadRes.resultCd.Equals("910") && rraSdcUploadModel.ResultMsg.IndexOf("<cfmDt>") > 0)
                                    {
                                        RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    }
                                    else if (rraSdcUploadRes.resultCd.Equals("910") && rraSdcUploadModel.ResultMsg.IndexOf("<totItemCnt>") > 0)
                                    {
                                        RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    }
                                    else if (rraSdcUploadRes.resultCd.Equals("910") && rraSdcUploadModel.ResultMsg.IndexOf("<email>") > 0)
                                    {
                                        RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    }
                                    else if (rraSdcUploadRes.resultCd.Equals("922") && rraSdcUploadModel.ResultMsg.IndexOf("invoice data can be received") > 0)
                                    {
                                        RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            DateTime oDate = DateTime.ParseExact(rraSdcUploadModel.RequestDt, "yyyyMMddHHmmss", null);
                                            DateTime cDate = DateTime.ParseExact("20200531235959", "yyyyMMddHHmmss", null);
                                            if (oDate < cDate)
                                            {
                                                RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                else if (rraSdcUploadRes.resultCd.Equals("999") && rraSdcUploadModel.FunctionName.Equals("saveItem"))
                                {
                                    RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                                }
                                else
                                {
                                    //string 
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void UploadSocketProcess(bool backup)
        {
            try
            {
                List<RraSdcUploadModel> listRraSdcUploadModel = RraSdcJsonWriter.GetTransactionList();
                for (int i = 0; i < listRraSdcUploadModel.Count; i++)
                {
                    Task.Delay(TimeSpan.FromSeconds(0.1));
                    RraSdcUploadModel rraSdcUploadModel = listRraSdcUploadModel[i];

                    SocketModel socketRequestModel = new SocketModel();
                    socketRequestModel.WCC = "RraSdcUploadModel";
                    socketRequestModel.JsonRequest = JsonConvert.SerializeObject(rraSdcUploadModel, Formatting.Indented);

                    SocketModel socketResponseModel = Common.Send(socketRequestModel, UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer, 11129);
                    if (socketResponseModel != null)
                    {
                        if (socketResponseModel.WCC.Equals("SUCCESS"))
                        {
                            RraSdcJsonWriter.BackupSuccessFile(rraSdcUploadModel);
                        }
                        else
                        {
                            if (socketResponseModel.WCC.Equals("ERROR"))
                            {
                                if (backup) RraSdcJsonWriter.BackupErrorFile(rraSdcUploadModel);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void UploadTranNodeSocketProcess(bool backup)
        {
            try
            {
                List<TranModel> listTranModelModel = TransactionReader.GetTransactionList();
                for (int i = 0; i < listTranModelModel.Count; i++)
                {
                    Task.Delay(TimeSpan.FromSeconds(0.1));
                    TranModel tranModel = listTranModelModel[i];

                    SocketModel socketRequestModel = new SocketModel();
                    socketRequestModel.WCC = "TranModel";
                    socketRequestModel.JsonRequest = JsonConvert.SerializeObject(tranModel, Formatting.Indented);

                    SocketModel socketResponseModel = Common.Send(socketRequestModel, UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer, 11129);
                    if (socketResponseModel != null)
                    {
                        if (socketResponseModel.WCC.Equals("SUCCESS"))
                        {
                            TransactionWriter.DeleteFile(tranModel);
                        }
                        else
                        {
                            if (socketResponseModel.WCC.Equals("ERROR"))
                            {
                                //BackupErrorFile();
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
