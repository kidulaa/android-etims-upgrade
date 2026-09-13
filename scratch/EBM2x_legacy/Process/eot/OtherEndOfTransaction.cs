using EBM2x.Datafile.regitotal;
using EBM2x.Datafile.trlog;
using EBM2x.Models;
using EBM2x.Models.regitotal;

namespace EBM2x.Process.eot
{
    public class OtherEndOfTransaction
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            
            posModel.RegiTotal.RegiHeader.UpdateDateTime();

            ////JCNA 20191203
            //posModel.RegiTotal.RegiHeader.decreaseReceiptNo();
            // RegiTotal json 
            RegiTotalWriter.write(posModel);
            // OperTotal json 
            OperTotalWriter.write(posModel);
            //posModel.RegiTotal.RegiHeader.increaseReceiptNo();

            return StateModel.OP_FAR;
        }
    }
}
