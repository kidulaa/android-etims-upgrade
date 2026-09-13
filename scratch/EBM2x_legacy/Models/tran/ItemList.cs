using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.tran
{
    public class ItemList
    {
        
        public string SubtotalDiscountFlag { get; set; }            
        public int SubtotalDiscountRate { get; set; }              
        public double SubtotalDiscountAmount { get; set; }          
        public string SubtotalDiscountType { get; set; }          
        public string SubtotalDiscountType2 { get; set; }          
        public double SubtotalRemainderDiscount { get; set; }       
        public List<ItemNode> List { get; set; }                    

        public int CurrentLineNumber { get; set; }                  
        public int LinesAtWhichPageBegins { get; set; }             
        public int CountOfItemsToDisplayOnOnePage { get; set; }     

        public ItemList()
        {
            SubtotalDiscountFlag = "N";           
            SubtotalDiscountRate = 0;             
            SubtotalDiscountAmount = 0;           
            SubtotalDiscountType = string.Empty;  
            SubtotalDiscountType2 = string.Empty;
            SubtotalRemainderDiscount = 0;        

            List = new List<ItemNode>();         

            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;
            CountOfItemsToDisplayOnOnePage = 0;
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this); 
        }

        public int Count()
        {
            return List.Count;
        }

        public void ChangeQuantity(double quantity)
        {
            ItemNode itemNode = Get(CurrentLineNumber - 1);

            itemNode.Quantity = quantity;

            itemNode.CalculateI();

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }
        public void ChangePrice(double price)
        {
            ItemNode itemNode = Get(CurrentLineNumber - 1);

            itemNode.Price = price;
            itemNode.DiscountFlag = "";  
            itemNode.DiscountRate = 0;    
            itemNode.DiscountPrice = 0;  
            itemNode.DiscountAmount = 0;

            itemNode.CalculateI();

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }
        public void ChangePrice(bool service, double price)
        {
            ItemNode itemNode = Get(CurrentLineNumber - 1);
            itemNode.IsService = service;
            itemNode.Price = price;
            itemNode.DiscountFlag = "";  
            itemNode.DiscountRate = 0;   
            itemNode.DiscountPrice = 0; 
            itemNode.DiscountAmount = 0;

            itemNode.CalculateI();

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }

        public void ChangeDiscount(int discount)
        {
            ItemNode itemNode = Get(CurrentLineNumber - 1);

            itemNode.DiscountRate = discount;
            if(discount > 0)
            {
                itemNode.DiscountFlag = "R";                                
                itemNode.DiscountRate = discount;                           
                itemNode.DiscountPrice = (itemNode.Price * discount) / 100; 

                itemNode.DiscountAmount = itemNode.DiscountPrice * itemNode.Quantity;
            }
            else
            {
                itemNode.DiscountFlag = "";   
                itemNode.DiscountRate = 0;    
                itemNode.DiscountPrice = 0;  

                itemNode.DiscountAmount = 0;
            }

            itemNode.CalculateI();

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }
        public void ChangeDiscountAll(int discount, string docs)
        {
            Get(CurrentLineNumber - 1);
            for (int i = 0; i < List.Count; i++)
            {
                ItemNode itemNode = List[i];

                itemNode.DiscountRate = discount;
                if (discount > 0)
                {
                    itemNode.DiscountFlag = "R";                                
                    itemNode.DiscountRate = discount;                           
                    itemNode.DiscountPrice = (itemNode.Price * discount) / 100;

                    itemNode.DiscountAmount = itemNode.DiscountPrice * itemNode.Quantity;
                }
                else
                {
                    itemNode.DiscountFlag = "";   
                    itemNode.DiscountRate = 0;    
                    itemNode.DiscountPrice = 0;  

                    itemNode.DiscountAmount = 0;
                }

                itemNode.CalculateI();
            }

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }
        public void ChangeDiscountInsurers(int discount, string code, string name)
        {
            Get(CurrentLineNumber - 1);
            for (int i = 0; i < List.Count; i++)
            {
                ItemNode itemNode = List[i];

                if (discount > 0 && itemNode.IsrcAplcbYn.Equals("Y"))
                {
                    itemNode.DiscountFlag = "R";                               
                    itemNode.DiscountRate = discount;                           
                    itemNode.DiscountPrice = (itemNode.Price * discount) / 100;

                    itemNode.DiscountAmount = itemNode.DiscountPrice * itemNode.Quantity;

                    itemNode.InsurerCode = code;
                    itemNode.InsurerName = name;
                    itemNode.InsurerRate = discount;
                    itemNode.DiscountType = "I"; 

                }
                else
                {
                    if (!string.IsNullOrEmpty(itemNode.InsurerCode))
                    {
                        itemNode.DiscountFlag = "";   
                        itemNode.DiscountRate = 0;    
                        itemNode.DiscountPrice = 0;  

                        itemNode.DiscountAmount = 0;

                        itemNode.InsurerCode = string.Empty;
                        itemNode.InsurerName = string.Empty;
                        itemNode.InsurerRate = 0;
                        itemNode.DiscountType = string.Empty; // JINIT_201911
                    }
                }

                itemNode.CalculateI();
            }

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }
        public void CancelDiscountInsurers()
        {
            Get(CurrentLineNumber - 1);
            for (int i = 0; i < List.Count; i++)
            {
                ItemNode itemNode = List[i];

                if (itemNode.IsrcAplcbYn.Equals("Y") && !string.IsNullOrEmpty(itemNode.InsurerCode))
                {
                    itemNode.DiscountFlag = "";   
                    itemNode.DiscountRate = 0;    
                    itemNode.DiscountPrice = 0; 

                    itemNode.DiscountAmount = 0;

                    itemNode.InsurerCode = string.Empty;
                    itemNode.InsurerName = string.Empty;
                    itemNode.InsurerRate = 0;
                }

                itemNode.CalculateI();
            }

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }
        public bool IsDiscountInsurers()
        {
            for (int i = 0; i < List.Count; i++)
            {
                ItemNode itemNode = List[i];
                if (itemNode.IsrcAplcbYn.Equals("Y")) return true;
            }
            return false;
        }
        public bool IsItemDiscountInsurers()
        {
            ItemNode itemNode = Get(CurrentLineNumber - 1);
            if (itemNode.IsrcAplcbYn.Equals("Y")) return true;
            else return false;
        }

        // JINIT_201911
        public ItemNode CheckInsurance()
        {
            for (int i = 0; i < List.Count; i++)
            {
                ItemNode itemNode = List[i];

                if (itemNode.IsrcAplcbYn.Equals("Y") && itemNode.DiscountType.Equals("I"))
                {
                    if (itemNode.DiscountAmount != 0) return itemNode;
                }
            }
            return null;
        }

        public void CancelItem()
        {
            ItemNode itemNode = Get(CurrentLineNumber - 1);

            itemNode.CancelQty = itemNode.Quantity; 
            itemNode.Quantity = 0;                  
            itemNode.DiscountFlag = "";            
            itemNode.DiscountRate = 0;              
            itemNode.DiscountPrice = 0;             

            itemNode.CalculateI();

            CurrentLineNumber = 0;

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }
        public ItemNode GetCurrentItem()
        {
            return Get(CurrentLineNumber - 1);
        }

        public void DeleteItem()
        {
            ItemNode itemNode = Get(CurrentLineNumber - 1);

            List.Remove(itemNode);

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }

        public void SetCurrent(int index)
        {
            if ((LinesAtWhichPageBegins + index) <= List.Count)
            {
                CurrentLineNumber = LinesAtWhichPageBegins + index;

                MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
            }
        }
        public int FindItem(ItemNode itemNode)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (itemNode.ItemCode.Equals(List[i].ItemCode) && itemNode.Price == List[i].Price) return i;
            }
            return -1;
        }

        public void PageUp()
        {
            if (LinesAtWhichPageBegins <= CountOfItemsToDisplayOnOnePage) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins - CountOfItemsToDisplayOnOnePage;

            CurrentLineNumber = 0;

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }

        public void PageDown()
        {
            if ((LinesAtWhichPageBegins + CountOfItemsToDisplayOnOnePage) > List.Count) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins + CountOfItemsToDisplayOnOnePage;

            CurrentLineNumber = 0;

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }

        public ItemNode Get(int index)
        {
            return List[index];
        }
        public ItemNode GetLast()
        {
            int index = List.Count - 1;
            return List[index];
        }

        public void Add(ItemNode itemNode)
        {
            List.Add(itemNode);

            CurrentLineNumber = List.Count;

            int pageCount = List.Count - ((List.Count - 1) % CountOfItemsToDisplayOnOnePage);
            LinesAtWhichPageBegins = pageCount;

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }

        public void Clear()
        {
            List.Clear();

            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;

            MessagingCenter.Send<Object, ItemList>(this, "Item Node", this);
        }

        public void CalculateItemList(TranNode tranNode)
        {
            tranNode.Subtotal = 0;                    
            tranNode.DiscountAmount = 0;           
            tranNode.InsuranceDiscountAmount = 0;
            tranNode.Total = 0;                       
            tranNode.VatAmount = 0;                   
            tranNode.NetAmount = 0;                   
            tranNode.TaxFlagBAmount = 0;              // JINIT_201911, TAX B 18% 
            tranNode.TaxFlagCAmount = 0;              // JINIT_201911, TAX B
            tranNode.TaxFlagDAmount = 0;              // JINIT_201911, TAX  

            for (int i = 0; i < List.Count; i++)
            {
                List[i].CalculateI();

                tranNode.DiscountAmount = tranNode.DiscountAmount + List[i].DiscountAmount;
                if (!List[i].IsrcAplcbYn.Equals("N"))
                {
                    tranNode.InsuranceDiscountAmount = tranNode.InsuranceDiscountAmount + List[i].DiscountAmount;
                }
                tranNode.Total = tranNode.Total + List[i].Total;
                tranNode.Subtotal = tranNode.Subtotal + List[i].Subtotal;

                tranNode.VatAmount = tranNode.VatAmount + List[i].VatAmount;
                tranNode.NetAmount = tranNode.NetAmount + List[i].NetAmount;

                // JINIT_201911
                tranNode.TaxFlagBAmount += List[i].TaxFlagBAmount;
                tranNode.TaxFlagCAmount += List[i].TaxFlagCAmount;
                tranNode.TaxFlagDAmount += List[i].TaxFlagDAmount;

            }
        }
        public double CalculateItemQuantity(ItemNode itemNode, TranNode tranNode)
        {
            double quantity = 0;

            for (int i = 0; i < List.Count; i++)
            {
                if(itemNode.ItemCode.Equals(List[i].ItemCode)) quantity += List[i].Quantity;
            }
            return quantity;
        }

        public IEnumerator<ItemNode> GetEnumerator()
        {
            for (int i = 0; i < List.Count; i++)
            {
                yield return List[i];
            }
        }
    }
}
