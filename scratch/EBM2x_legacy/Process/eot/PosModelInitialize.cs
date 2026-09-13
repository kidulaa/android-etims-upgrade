using EBM2x.Models;

namespace EBM2x.Process.eot
{
    public class PosModelInitialize
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.TranNode != null)
            {
                posModel.TranModel.TranNode.ItemList.Clear();
                posModel.TranModel.TranNode.TenderList.Clear();

                posModel.TranModel.TranNode.CalculateItemList();
                posModel.TranModel.TranNode.CalculateTenderList();
            }

            // posModel 
            posModel.InitailizeTran();

            return StateModel.OP_FAR;
        }
    }
}
