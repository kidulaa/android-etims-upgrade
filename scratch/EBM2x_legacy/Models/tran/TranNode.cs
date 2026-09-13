using EBM2x.Models.customer;
using EBM2x.Models.HotelRoom;
using EBM2x.Models.ListView;
using EBM2x.Models.refund;
using System;
using Xamarin.Forms;

namespace EBM2x.Models.tran
{
    public class TranNode
    {
        public ItemList ItemList { get; set; }              // ITEM LIST
        public TenderList TenderList { get; set; }		 	 // TENDER LIST

        public CustomerNode CustomerNode { get; set; }
        public RefundReasonNode RefundReasonNode { get; set; }
        public SearchInsurerNode InsurerNode { get; set; }

        public string TranFlag { get; set; }          
        public int Sign { get; set; }             
        public double Subtotal { get; set; }          
        public double DiscountAmount { get; set; }    
        public double InsuranceDiscountAmount { get; set; }    
        public double Total { get; set; }             
        public double VatAmount { get; set; }         
        public double NetAmount { get; set; }         
        public double TaxFlagBAmount { get; set; }    // JINIT_201911, TAX B-18% 
        public double TaxFlagCAmount { get; set; }    // JINIT_201911, TAX B 
        public double TaxFlagDAmount { get; set; }    // JINIT_201911, TAX D
        public double TaxFlagEAmount { get; set; }    // JINIT_201911, TAX E-8% 

        public double Receive { get; set; }
        public double Change { get; set; }

        public string IntrlData { get; set; }           // Internal Data
        public string RcptSign { get; set; }            // Receipt Signature

        public HotelRoomNode HotelRoomNode { get; set; }


    public TranNode()
        {
            ItemList = new ItemList();
            TenderList = new TenderList();

            CustomerNode = new CustomerNode();
            RefundReasonNode = new RefundReasonNode();
            InsurerNode = new SearchInsurerNode();

            ItemList.CalculateItemList(this);

            TranFlag = "N";
            Sign = 1;
            Receive = 0.0d;
            Change = 0.0d;

            HotelRoomNode = null;
        }

        public double AmountToReceive() {
            if((Subtotal - Receive) > 0) return (Subtotal - Receive);
            return 0;
        }

        public void CalculateItemList()
        {
            ItemList.CalculateItemList(this);

            MessagingCenter.Send<Object, TranNode>(this, "Tran Node", this);
        }
        public void CalculateTenderList()
        {
            TenderList.CalculateTenderList(this);

            MessagingCenter.Send<Object, TranNode>(this, "Tran Node", this);
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, TranNode>(this, "Tran Node", this);
        }
    }
}
