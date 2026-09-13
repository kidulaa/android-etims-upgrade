using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.ListView;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;
using EBM2x.Services;

namespace EBM2x.Process.customer
{
    public class AddSearchCustomerProcess
    {
        public static string excuteProcess(SearchCustomerNode searchCustomerNode, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if(searchCustomerNode != null)
            {
                posModel.TranModel.TranNode.CustomerNode.Tin = searchCustomerNode.Tin;
                posModel.TranModel.TranNode.CustomerNode.CustomerCode = searchCustomerNode.CustomerCode;
                posModel.TranModel.TranNode.CustomerNode.CustomerName = searchCustomerNode.CustomerName;
                posModel.TranModel.TranNode.CustomerNode.CustGroup = searchCustomerNode.CustGroup;
            }
            else
            {
                posModel.TranModel.TranNode.CustomerNode.Tin = string.Empty;
                posModel.TranModel.TranNode.CustomerNode.CustomerCode = string.Empty;
                posModel.TranModel.TranNode.CustomerNode.CustomerName = string.Empty;
                posModel.TranModel.TranNode.CustomerNode.CustGroup = "DF";
            }

            posModel.TranModel.TranNode.CalculateItemList();
            return StateModel.OP_NEXT;
        }
    }
}
