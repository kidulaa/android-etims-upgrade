using EBM2x.Datafile.trlog;
using EBM2x.Models.ListView;
using EBM2x.RraSdc;
using EBM2x.UI;
using EBM2x.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EBM2x.Process.search
{
    public class SearchReceiptProcess
    {
        // tran
        public static List<SearchReceiptNode> QuerydSearchReceipt()
        {
            if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL && UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServerType.Equals("Slave"))
            {
                SocketModel socketRequestModel = new SocketModel();
                socketRequestModel.WCC = "SearchReceiptNodeList";
                socketRequestModel.JsonRequest = "";

                SocketModel socketResponseModel = Common.Send(socketRequestModel, UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer, 11129);
                if (socketResponseModel != null)
                {
                    if (socketResponseModel.WCC.Equals("SUCCESS"))
                    {
                        List<SearchReceiptNode> list = JsonConvert.DeserializeObject<List<SearchReceiptNode>>(socketResponseModel.JsonRequest);
                        return list;
                    }
                    else
                    {
                        List<SearchReceiptNode> list = TransactionReader.GetSearchReceiptList("");
                        return list;
                    }
                }
                else
                {
                    List<SearchReceiptNode> list = TransactionReader.GetSearchReceiptList("");
                    return list;
                }
            }
            else
            {
                List<SearchReceiptNode> list = TransactionReader.GetSearchReceiptList("");
                return list;
            }
        }
         
        // JINIT_backup
        public static List<SearchReceiptNode> QuerydSearchReceipt(string date)
        {
            if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL && UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServerType.Equals("Slave"))
            {
                SocketModel socketRequestModel = new SocketModel();
                socketRequestModel.WCC = "SearchReceiptNodeList";
                socketRequestModel.JsonRequest = date;

                SocketModel socketResponseModel = Common.Send(socketRequestModel, UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer, 11129);
                if (socketResponseModel != null)
                {
                    if (socketResponseModel.WCC.Equals("SUCCESS"))
                    {
                        List<SearchReceiptNode> list = JsonConvert.DeserializeObject<List<SearchReceiptNode>>(socketResponseModel.JsonRequest);
                        return list;
                    }
                    else
                    {
                        List<SearchReceiptNode> list = TransactionReader.GetSearchReceiptList(date);
                        return list;
                    }
                }
                else
                {
                    List<SearchReceiptNode> list = TransactionReader.GetSearchReceiptList(date);
                    return list;
                }
            }
            else
            {
                List<SearchReceiptNode> list = TransactionReader.GetSearchReceiptList(date);
                return list;
            }
        }
    }
}
