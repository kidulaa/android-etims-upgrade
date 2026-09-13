using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.config;
using EBM2x.Models.regitotal;
using EBM2x.RraSdc;
using EBM2x.Services;
using EBM2x.UI;
using EBM2x.Utils;
using Newtonsoft.Json;

namespace EBM2x.Process.signon
{
    public class InputOperatorCodeProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.SignOnNode != null)
            {
                // UserID != InputUserID
                RegiHeader regiHeader = posModel.RegiTotal.RegiHeader;
                OperHeader operHeader = posModel.OperTotal.OperHeader;

                OperatorRecord record = null;
                if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL && UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServerType.Equals("Slave"))
                {
                    SocketModel socketRequestModel = new SocketModel();
                    socketRequestModel.WCC = "OperatorLoad";
                    socketRequestModel.JsonRequest = inputModel.EnteredText;

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
                    record = OperatorService.Load(inputModel.EnteredText);
                }

                if (!string.IsNullOrEmpty(record.OperatorCode))
                {
                    posModel.TranModel.SignOnNode.CashierID = record.OperatorCode;
                    posModel.TranModel.SignOnNode.CashierName = record.OperatorName;

                    return StateModel.OP_NEXT;
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
