using System;

namespace EBM2x.Models.tran
{
    public class ItemNode
    {
        public const string INPUT_ITEM = "I";
        public const string INPUT_CLASS = "C";

        public const string INPUT_SCANNING = "S";
        public const string INPUT_MANUAL = "M";

        public const string GOODS_SIDE_MENU = "2";

        public string TranFlag { get; set; }         
        public double Sign { get; set; }             
        public string ItemFlag { get; set; }        
        public string InputFlag { get; set; }    
        public string ItemCode { get; set; }         
        public string ClassCode { get; set; }    
        public string ItemName { get; set; }       
        public string ClassName { get; set; }        
        public double Price { get; set; }           
        public double Quantity { get; set; }       
        public double OldQuantity { get; set; }     

        public string InstoreCode { get; set; }       
        public string Barcode { get; set; }           
        public string ScanCode { get; set; }          


        public double Subtotal { get; set; }       
        public double DiscountAmount { get; set; }  
        public double Total { get; set; }            
        public double VatAmount { get; set; }        
        public double NetAmount { get; set; }        
        public double TaxFlagBAmount { get; set; }   
        public double TaxFlagCAmount { get; set; }    // JINIT_201911, TAX B
        public double TaxFlagDAmount { get; set; }    // JINIT_201911, TAX

        public string DiscountYN { get; set; }       
        public string DiscountFlag { get; set; }      
        public int DiscountRate { get; set; }         
        public double DiscountPrice { get; set; }    
        public string DiscountType { get; set; }      
        public double DiscountSubtotal { get; set; }  
        public string TaxFlag { get; set; }          
        public double CancelQty { get; set; }         
        public bool PriceFlag { get; set; }          
        public double ChangePrice { get; set; }
        public double NormalPrice { get; set; }


        public string PointSaveupYn { get; set; }  
        public double PointSaveupAmt { get; set; }    

        public string FoodGrpNo { get; set; }
        public string FoodTblNo { get; set; }
        public int FoodOrdTblSeqNo { get; set; }
        public int FoodOrdTblGoodsSeqNo { get; set; }
        public int FoodAddedQty { get; set; }
        public bool IsFoodSetMenu { get; set; }
        public string FoodSetMenuNo { get; set; }
        public int FoodSetMenuCategoryNo { get; set; }
        public bool IsService { get; set; }

        public string KitchenPrt { get; set; }

        public string IsrcAplcbYn { get; set; }         // Insurance Appicable(Y/N)
        public string InsurerCode { get; set; }
        public string InsurerName { get; set; }
        public int InsurerRate { get; set; }
        public double RdsQty { get; set; }                
        public string UseExpiration { get; set; }          
        public string ExpirationDate { get; set; }         

        public ItemNode()
        {
            clear();
        }
        public ItemNode(string tranFlag)
        {
            clear();
            TranFlag = tranFlag;   
        }

        public void clear()
        {
            TranFlag = "N";                  
            Sign = 1;                        
            ItemFlag = string.Empty;         
            InputFlag = string.Empty;        
            ItemCode = string.Empty;         
            ClassCode = string.Empty;        
            ItemName = string.Empty;        
            ClassName = string.Empty;       
            Price = 0;                       
            Quantity = 0;                    
            OldQuantity = 0;                   
            DiscountYN = "Y";               
            DiscountFlag = "N";              
            DiscountRate = 0;                
            DiscountPrice = 0;               
            DiscountType = string.Empty;     
            DiscountSubtotal = 0;            
            TaxFlag = "B";                   
            CancelQty = 0;                   
            PriceFlag = false;              
            ChangePrice = 0;
            NormalPrice = 0;

            InstoreCode = string.Empty;      
            Barcode = string.Empty;
            ScanCode = string.Empty;         

            Subtotal = 0;                    
            DiscountAmount = 0;             
            Total = 0;                       
            VatAmount = 0;                   
            NetAmount = 0;                 
            TaxFlagBAmount = 0;              // JINIT_201911, 
            TaxFlagCAmount = 0;              // JINIT_201911, 
            TaxFlagDAmount = 0;              // JINIT_201911, 

            PointSaveupYn = "Y";             
            PointSaveupAmt = 0;              

            FoodOrdTblSeqNo = 0;
            FoodAddedQty = 0;
            IsFoodSetMenu = false;
            FoodSetMenuNo = string.Empty;
            FoodSetMenuCategoryNo = 0;
            IsService = false;

            KitchenPrt = string.Empty;

            IsrcAplcbYn = "N";
            InsurerCode = string.Empty;
            InsurerName = string.Empty;
            InsurerRate = 0;

            UseExpiration = "N";
            ExpirationDate = "";
    }

    public void CalculateI()
        {
            if (TranFlag.Equals(TranDefine.TRAN_RETURN)) Sign = -1;
            else Sign = 1;
            
            DiscountAmount = DiscountPrice * Quantity; 
            
            Total = Price * Quantity;                 
            
            Subtotal = Total - DiscountAmount;         

            

            switch (TaxFlag)
            {
                case "A" : // TAX A-EX
                    NetAmount = Subtotal;
                    break;
                case "B": // TAX B-18.00%
                    if (!IsrcAplcbYn.Equals("N"))
                    {
                        TaxFlagBAmount = Total;
                        double vatRate = 16;
                        VatAmount = (Total / 1.16) * (vatRate / 100);
                        VatAmount = Math.Round(VatAmount, 2);
                    }
                    else
                    {
                        TaxFlagBAmount = Subtotal;
                        double vatRate = 16;
                        VatAmount = (Subtotal / 1.16) * (vatRate / 100);
                        VatAmount = Math.Round(VatAmount, 2);
                    }
                    break;
                case "E": // TAX B-18.00%
                    if (!IsrcAplcbYn.Equals("N"))
                    {
                        TaxFlagBAmount = Total;
                        double vatRate = 8;
                        VatAmount = (Total / 1.08) * (vatRate / 100);
                        VatAmount = Math.Round(VatAmount, 2);
                    }
                    else
                    {
                        TaxFlagBAmount = Subtotal;
                        double vatRate = 8;
                        VatAmount = (Subtotal / 1.08) * (vatRate / 100);
                        VatAmount = Math.Round(VatAmount, 2);
                    }
                    break;
                case "C": // TAX B
                    TaxFlagCAmount = Subtotal;
                    break;
                case "D": // TAX
                    TaxFlagDAmount = Subtotal;
                    break;
                default:
                    break;
            }

        }

        public ItemNode ShallowCopy()
        {
            return (ItemNode)this.MemberwiseClone();
        }
    }
}
