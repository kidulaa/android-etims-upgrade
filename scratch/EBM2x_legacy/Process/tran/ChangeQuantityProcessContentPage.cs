using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.tran;
using EBM2x.UI;
using Xamarin.Forms;

namespace EBM2x.Process.tran
{
    public class ChangeQuantityProcessContentPage
    {
        public static string excuteProcess(ContentPage contentPage, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.TranNode.ItemList.CurrentLineNumber < 1) return StateModel.OP_RETRY;

            double quantity = 0;

            if (!string.IsNullOrEmpty(inputModel.EnteredText))
            {
                if (inputModel.EnteredText.Length > 6)
                {
                    informationModel.SetWarningMessage("Please enter a quantity between 1 and 999999.");
                    contentPage.DisplayAlert("Information", "Please enter a quantity between 1 and 999999.", "Ok");
                    return StateModel.OP_RETRY;
                }
                quantity = double.Parse(inputModel.EnteredText);
                
                if (quantity <= 0 || quantity >= 1000000)
                {
                    informationModel.SetWarningMessage("The quantity must be between 1 and 999999.");
                    contentPage.DisplayAlert("Information", "The quantity must be between 1 and 999999.", "Ok");
                    return StateModel.OP_RETRY;
                }
            }
            else
            {
              
                ItemNode itemNode = posModel.TranModel.TranNode.ItemList.Get(posModel.TranModel.TranNode.ItemList.CurrentLineNumber - 1);
                quantity = itemNode.Quantity + 1;
                
                if (quantity <= 0 || quantity >= 1000000)
                {
                    informationModel.SetWarningMessage("The quantity must be between 1 and 999999.");
                    return StateModel.OP_RETRY;
                }
            }

            posModel.TranModel.TranNode.ItemList.ChangeQuantity(quantity);
            posModel.TranModel.TranNode.CalculateItemList();

            return StateModel.OP_NEXT;
        }
        public static string excuteProcessRefund(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.TranNode.ItemList.CurrentLineNumber < 1) return StateModel.OP_RETRY;

            double quantity = 0;

            if (!string.IsNullOrEmpty(inputModel.EnteredText))
            {
                if (inputModel.EnteredText.Length > 6)
                {
                    informationModel.SetWarningMessage("Please enter a quantity between 1 and 999999.");
                    return StateModel.OP_RETRY;
                }
                quantity = double.Parse(inputModel.EnteredText);

                if (quantity <= 0 || quantity >= 1000000)
                {
                    informationModel.SetWarningMessage("The quantity must be between 1 and 999999.");
                    return StateModel.OP_RETRY;
                }
            }
            else
            {
                
                ItemNode itemNode = posModel.TranModel.TranNode.ItemList.Get(posModel.TranModel.TranNode.ItemList.CurrentLineNumber - 1);
                quantity = itemNode.Quantity + 1;

               
                if (quantity <= 0 || quantity >= 1000000)
                {
                    informationModel.SetWarningMessage("The quantity must be between 1 and 999999.");
                    return StateModel.OP_RETRY;
                }
            }

            if(posModel.TranModel.TranNode.ItemList.GetCurrentItem().OldQuantity == 0)
            {
                posModel.TranModel.TranNode.ItemList.GetCurrentItem().OldQuantity = posModel.TranModel.TranNode.ItemList.GetCurrentItem().Quantity;
            }

            if(posModel.TranModel.TranNode.ItemList.GetCurrentItem().OldQuantity < quantity)
            {
                informationModel.SetWarningMessage("The quantity must be between 1 and " + posModel.TranModel.TranNode.ItemList.GetCurrentItem().OldQuantity + ".");
                return StateModel.OP_RETRY;
            }

            posModel.TranModel.TranNode.ItemList.ChangeQuantity(quantity);
            posModel.TranModel.TranNode.CalculateItemList();

            return StateModel.OP_NEXT;
        }
        public static double Quantity(InputModel inputModel)
        {
            double quantity = 0;

            if (!string.IsNullOrEmpty(inputModel.EnteredText))
            {
                if (inputModel.EnteredText.Length > 3)
                {
                    return 0;
                }
                quantity = double.Parse(inputModel.EnteredText);
            }
            else
            {
                quantity = 1;
            }
            return quantity;
        }
        public static bool checkStockProcess(PosModel posModel, double quantity)
        {
            if (posModel.TranModel.TranNode.ItemList.CurrentLineNumber < 1) return true;

            ItemNode itemNode = posModel.TranModel.TranNode.ItemList.Get(posModel.TranModel.TranNode.ItemList.CurrentLineNumber - 1);

            string Tin = posModel.Environment.EnvPosSetup.GblTaxIdNo; 
            TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
            TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
            bool ret = taxpayerItemMaster.ToRecord(itemRecord, Tin, itemNode.ItemCode, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
            if(ret)
            {
                if (!itemRecord.ItemTyCd.Equals("1") && !itemRecord.ItemTyCd.Equals("2")) return true;
            }

            double qtyTotal = posModel.TranModel.TranNode.ItemList.CalculateItemQuantity(itemNode, posModel.TranModel.TranNode);

            if (quantity > 0)
            {
                quantity = quantity - itemNode.Quantity;
            }

            if ((itemNode.RdsQty - (qtyTotal + quantity)) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool checkStockProcess(PosModel posModel, ItemNode itemNode, int quantityAdd)
        {
            string Tin = posModel.Environment.EnvPosSetup.GblTaxIdNo;
            TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
            TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
            bool ret = taxpayerItemMaster.ToRecord(itemRecord, Tin, itemNode.ItemCode, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
            if (ret)
            {
                if (!itemRecord.ItemTyCd.Equals("1") && !itemRecord.ItemTyCd.Equals("2")) return true;
            }

            double qtyTotal = posModel.TranModel.TranNode.ItemList.CalculateItemQuantity(itemNode, posModel.TranModel.TranNode);

            if ((itemNode.RdsQty - (qtyTotal + quantityAdd)) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
