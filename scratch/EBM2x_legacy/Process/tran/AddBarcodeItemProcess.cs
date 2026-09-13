using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.config;
using EBM2x.Models.ListView;
using EBM2x.Models.tran;
using EBM2x.Utils;
using System;
using System.Collections.Generic;

namespace EBM2x.Process.tran
{
    public class AddBarcodeItemProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if(string.IsNullOrEmpty(inputModel.EnteredText))
            {
                informationModel.SetWarningMessage("Enter bar code!");
                return StateModel.OP_RETRY;
            }

            EnvPosSetup envPosSetup = posModel.Environment.EnvPosSetup;
            SearchTaxpayerItemListMaster searchTaxpayerItemListMaster = new SearchTaxpayerItemListMaster();
            List<SearchItemNode> itemList = searchTaxpayerItemListMaster.GetTableBarcode(envPosSetup.GblTaxIdNo, inputModel.EnteredText);

            if(itemList == null || itemList.Count <= 0)
            {
                informationModel.SetWarningMessage("It is an unregistered product. Check the Bar-Code!!");
                return StateModel.OP_RETRY;
            }

            ItemNode itemnode = AddBarcodeItemProcess.GetItemNode(posModel, envPosSetup.GblTaxIdNo, itemList[0].ItemCode);
            if (itemnode != null)
            {
                if (itemnode.UseExpiration.Equals("Y") && !string.IsNullOrEmpty(itemnode.ExpirationDate))
                {
                    DateTime ExpirationDate = Common.DateFormat(itemnode.ExpirationDate);
                    TimeSpan TS = ExpirationDate - DateTime.Now;

                    int diffDay = TS.Days; 
                    if (diffDay < 0)
                    {
                        informationModel.AlertTitle = "";
                        informationModel.AlertMessage = "There is an expired product. Please Check.";
                        return StateModel.OP_ALERT;
                    }
                }

                itemnode.TranFlag = posModel.TranModel.TranNode.TranFlag;
                itemnode.Sign = posModel.TranModel.TranNode.Sign;

                int index = posModel.TranModel.TranNode.ItemList.FindItem(itemnode);
                if (index >= 0)
                {
                    posModel.TranModel.TranNode.ItemList.SetCurrent(index);
                    if (ChangeQuantityProcess.checkStockProcess(posModel, posModel.TranModel.TranNode.ItemList.Get(index), 1))
                    {
                        posModel.TranModel.TranNode.ItemList.Get(index).Quantity += 1;
                    }
                    else
                    {
                        informationModel.SetWarningMessage("Greater than current stock.");
                        informationModel.AlertTitle = "";
                        informationModel.AlertMessage = "Greater than current stock.";
                        return StateModel.OP_ALERT;
                    }
                }
                else
                {
                    posModel.TranModel.TranNode.ItemList.Add(itemnode);
                }

                posModel.TranModel.TranNode.CalculateItemList();

                return StateModel.OP_NEXT;
            }
            else
            {
                informationModel.SetWarningMessage("It is an unregistered product. Check the Bar-Code!!");
                return StateModel.OP_RETRY;
            }
        }
        public static ItemNode GetItemNode(PosModel posModel, string tin, string itemCode)
        {
            TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
            TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
            bool ret = taxpayerItemMaster.ToRecord(itemRecord, tin, itemCode, posModel.Environment.EnvPosSetup.NonVAT);
            if (ret)
            {
                if (!itemRecord.UseYn.ToUpper().Equals("Y"))
                {
                    return null;
                }

                ItemClassRecord classRecord = new ItemClassRecord();
                if (itemRecord.ItemClsCd.Length < 4) itemRecord.ItemClsCd = "9999999900";

                ItemClassMaster itemClassMaster = new ItemClassMaster();
                bool clsret = itemClassMaster.ToRecord(classRecord, itemRecord.ItemClsCd);
                
                if (!clsret)
                {
                    classRecord.ItemClsCd = "9999999900";
                    classRecord.ItemClsNm = "Unregistered";
                }

                ItemNode itemnode = new ItemNode();
                itemnode.ItemFlag = "I";                                
                itemnode.ItemCode = itemRecord.ItemCd;                  
                itemnode.ClassCode = itemRecord.ItemClsCd;             
                itemnode.ItemName = itemRecord.ItemNm;                 
                itemnode.ClassName = classRecord.ItemClsNm;             
                itemnode.DiscountYN = "Y";                              

                itemnode.UseExpiration = itemRecord.UseExpiration;
                itemnode.ExpirationDate = itemRecord.ExpirationDate;

                itemnode.TaxFlag = itemRecord.TaxTyCd;

                itemnode.Price = itemRecord.DftPrc;

                if (posModel.TranModel.TranNode.CustomerNode != null)
                {
                    if (posModel.TranModel.TranNode.CustomerNode.CustGroup.Equals("L1"))
                    {
                        if (itemRecord.GrpPrcL1 != 0)
                        {
                            itemnode.Price = itemRecord.GrpPrcL1;
                        }
                    }
                    else if (posModel.TranModel.TranNode.CustomerNode.CustGroup.Equals("L2"))
                    {
                        if (itemRecord.GrpPrcL2 != 0)
                        {
                            itemnode.Price = itemRecord.GrpPrcL2;
                        }
                    }
                    else if (posModel.TranModel.TranNode.CustomerNode.CustGroup.Equals("L3"))
                    {
                        if (itemRecord.GrpPrcL3 != 0)
                        {
                            itemnode.Price = itemRecord.GrpPrcL3;
                        }
                    }
                    else if (posModel.TranModel.TranNode.CustomerNode.CustGroup.Equals("L4"))
                    {
                        if (itemRecord.GrpPrcL4 != 0)
                        {
                            itemnode.Price = itemRecord.GrpPrcL4;
                        }
                    }
                    else if (posModel.TranModel.TranNode.CustomerNode.CustGroup.Equals("L5"))
                    {
                        if (itemRecord.GrpPrcL5 != 0)
                        {
                            itemnode.Price = itemRecord.GrpPrcL5;
                        }
                    }
                }

                
                itemnode.NormalPrice = itemRecord.DftPrc;

                itemnode.Quantity = 1;  

                itemnode.PointSaveupYn = "N";                        
                itemnode.PointSaveupAmt = 0;                       

                itemnode.InstoreCode = itemRecord.ItemCd;           
                itemnode.Barcode = itemRecord.Bcd;                  

                itemnode.Subtotal = itemnode.Price * itemnode.Quantity;
                itemnode.Total = itemnode.Subtotal;


                itemnode.IsrcAplcbYn = itemRecord.IsrcAplcbYn;

                itemnode.RdsQty = itemRecord.RdsQty;

                return itemnode;
            }
            else
            {
                return null;
            }
        }
    }
}
