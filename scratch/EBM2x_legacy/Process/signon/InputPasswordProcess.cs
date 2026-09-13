using EBM2x.Datafile.regitotal;
using EBM2x.Models;
using EBM2x.Models.config;
using EBM2x.Models.regitotal;
using EBM2x.Models.tran;
using EBM2x.RraSdc;
using EBM2x.Services;
using EBM2x.UI;
using EBM2x.Utils;
using Newtonsoft.Json;

namespace EBM2x.Process.signon
{
    public class InputPasswordProcess
    {
        public static string excuteProcess(string permission, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.SignOnNode != null)
            {
                RegiHeader regiHeader = posModel.RegiTotal.RegiHeader;

                OperatorRecord record = null;
                if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL && UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServerType.Equals("Slave"))
                {
                    SocketModel socketRequestModel = new SocketModel();
                    socketRequestModel.WCC = "OperatorLoad";
                    socketRequestModel.JsonRequest = posModel.TranModel.SignOnNode.CashierID;

                    SocketModel socketResponseModel = Common.Send(socketRequestModel, UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer, 11129);
                    if (socketResponseModel != null)
                    {
                        if (socketResponseModel.WCC.Equals("SUCCESS"))
                        {
                            try
                            {
                                record = JsonConvert.DeserializeObject<OperatorRecord>(socketResponseModel.JsonRequest);
                            }
                            catch
                            {
                                record = new OperatorRecord();
                            }
                        }
                        else
                        {
                            record = new OperatorRecord();
                        }
                    }
                    else
                    {
                        record = new OperatorRecord();
                    }
                }
                else
                {
                    record = OperatorService.Load(posModel.TranModel.SignOnNode.CashierID);
                }

                if (permission.Equals("admin"))
                {
                    if(!record.Permission.Equals("admin"))
                    {
                        informationModel.SetWarningMessage("This permission is not available.");
                        return StateModel.OP_RETRY;
                    }
                }

                if (!string.IsNullOrEmpty(record.OperatorCode))
                {
                    posModel.TranModel.SignOnNode.CashierID = record.OperatorCode;
                    posModel.TranModel.SignOnNode.CashierName = record.OperatorName;
                    posModel.TranModel.SignOnNode.Permission = record.Permission;
                    //node.UserID = record.OperatorCode;
                    //node.UserName = record.OperatorName;
                    //node.BusinessFlag = record.Permission;
                    //node.setLoginDate();

                    if (inputModel.EnteredText.Equals(record.Password))
                    {
                        posModel.TranModel.SignOnNode.Password = "********";
                       
                        posModel.RegiTotal.RegiHeader.UserID = posModel.TranModel.SignOnNode.CashierID;
                        posModel.RegiTotal.RegiHeader.UserName = posModel.TranModel.SignOnNode.CashierName;

                        posModel.TranInformation.UserCode = posModel.TranModel.SignOnNode.CashierID;
                        posModel.TranInformation.UserName = posModel.TranModel.SignOnNode.CashierName;

                        posModel.TranInformation.LogFlag = TranDefine.LOG_SIGN_ON;

                        //////////////////////////////
                        ///  LOAD
                        //////////////////////////////
                        OperTotalReader.read(posModel);
                        if (posModel.OperTotal == null)
                        {
                            posModel.OperTotal = new OperTotal();

                            posModel.OperTotal.OperHeader.StoreCode = regiHeader.StoreCode;    // 점코드
                            posModel.OperTotal.OperHeader.RegiNo = regiHeader.RegiNo;          // 레지번호(POS번호)
                            posModel.OperTotal.OperHeader.OpenDate = regiHeader.OpenDate;      // 개설일자(개설TR에 계산원합계가 올라감에 따라서 설정 필수)
                            posModel.OperTotal.OperHeader.OpenFlag = false;                    // 개설 여부
                            posModel.OperTotal.OperHeader.OpenSequence = 1;                    // 레지순번
                            posModel.OperTotal.OperHeader.CloseFlag = true;                    // 정산 Flag
                            posModel.OperTotal.OperHeader.CloseSequence = 0;                   // 정산 순번
                            //20200226 JCNA : posModel.OperTotal.OperHeader.ReceiptNo = regiHeader.ReceiptNo;    // 영수증번호
                            posModel.OperTotal.OperHeader.PreOperCashStock = 0;                // 전계산원시재점검 합계
                            
                            posModel.OperTotal.OperHeader.UserID = posModel.TranModel.SignOnNode.CashierID;
                            posModel.OperTotal.OperHeader.UserName = posModel.TranModel.SignOnNode.CashierName;
                        }

                        return StateModel.OP_NEXT;
                    }
                    else
                    {
                        informationModel.SetWarningMessage("Password is wrong. Please check again.");
                        return StateModel.OP_RETRY;
                    }
                }
                else
                {
                    informationModel.SetWarningMessage("Unregistered USER. Please check the USER ID!");
                    return StateModel.OP_RETRY;
                }
            }
            else
            {
                informationModel.SetWarningMessage("Please restart the program!");
                return StateModel.OP_RETRY;
            }
        }
    }
}
