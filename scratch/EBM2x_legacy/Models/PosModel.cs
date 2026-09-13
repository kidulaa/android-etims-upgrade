using EBM2x.Database.Master;
using EBM2x.Datafile.env;
using EBM2x.Datafile.trlog;
using EBM2x.Models.config;
using EBM2x.Models.device;
using EBM2x.Models.journal;
using EBM2x.Models.open;
using EBM2x.Models.regitotal;
using EBM2x.UI;
using System;
using Xamarin.Forms;

namespace EBM2x.Models
{
    public class PosModel
    {
        private string LogoImage = "";
        private string SalesTitleColor = "";
        private string SalesTitleText = "";

        public string DeviceType = "";

        public EnvironmentNode Environment { get; set; }
        public TranInformation TranInformation { get; set; } 
        public ReceiptInfor ReceiptInfor { get; set; }       
        public JournalModel Journal { get; set; }                

        public RegiTotal RegiTotal { get; set; }            // REGI TOTAL
        public OperTotal OperTotal { get; set; }            // OPER TOTAL

        public JournalModel KitchenTitle { get; set; }           
        public JournalModel Kitchen { get; set; }                
        public JournalModel Kitchen2 { get; set; }               

        public DeviceNode DeviceNode { get; set; }

        public bool ExerFlag = false;                      

        public OpenTimeNode OpenTimeNode { get; set; }     

        public bool PreDataCloseFlag { get; set; }        

        public TranModel TranModel { get; set; }                 // TranModel
        public PresetModel PresetModel { get; set; }             // PresetModel

        public DiningTableModel DiningTableModel { get; set; }   // DiningTableModel
        public HotelRoomModel HotelRoomModel { get; set; }       // HotelRoomModel

        public PosModel()
        {
            LogoImage = "EBM2xBlack.png";
            SalesTitleColor = "3f922b";
            SalesTitleText = "ETIMS 1.0";

            DeviceType = string.Empty;

            Environment = new EnvironmentNode();
            TranInformation = new TranInformation(); 
            ReceiptInfor = new ReceiptInfor();       
            Journal = new JournalModel();           

            RegiTotal = new RegiTotal();             // REGI TOTAL
            OperTotal = new OperTotal();             // OPER TOTAL

            KitchenTitle = new JournalModel();           
            Kitchen = new JournalModel();                
            Kitchen2 = new JournalModel();               

            DeviceNode = new DeviceNode();

            ExerFlag = false;                      
            OpenTimeNode = new OpenTimeNode();     

            TranModel = new TranModel();
            TranModel.SetTranModel(TranInformation);

            PresetModel = new PresetModel();

            DiningTableModel = new DiningTableModel();

            HotelRoomModel = new HotelRoomModel();
        }

        public void InitailizeTran()
        {
            if (TranModel.TranNode != null)
            {
                TranModel.TranNode.CustomerNode.Tin = string.Empty;
                TranModel.TranNode.CustomerNode.CustomerCode = string.Empty;
                TranModel.TranNode.CustomerNode.CustomerName = string.Empty;
            }

            TranInformation.initialize(RegiTotal);
            Journal.Clear();
        }
        public void TinInitailizeTran()
        {
            if (TranModel.TranNode != null)
            {
                TranModel.TranNode.CustomerNode.Tin = string.Empty;
                TranModel.TranNode.CustomerNode.CustomerCode = string.Empty;
                TranModel.TranNode.CustomerNode.CustomerName = string.Empty;
            }
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, string>(this, "Logo Image", LogoImage);
            MessagingCenter.Send<Object, string>(this, "Sales Title Color", SalesTitleColor);
            MessagingCenter.Send<Object, string>(this, "Sales Title Text", SalesTitleText);

            //MessagingCenter.Send<Object, string>(this, "Business Day", RegiTotal.RegiHeader.OpenDate);
            MessagingCenter.Send<Object, string>(this, "Business Day", "PIN: " + UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo);

            if (!EnvRraSdcService.IsEnvRraSdc() || !EnvPosSetupService.IsEnvPosSetup())
            {
            }
            else
            {
                TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
                ////string Text1 = string.Format("{0}-{1:0000}", RegiTotal.RegiHeader.RegiNo, trnsSaleReceiptMaster.GetReceiptSeq());
                //////string Text1 = string.Format("{0}-{1:0000}", RegiTotal.RegiHeader.RegiNo, RegiTotal.RegiHeader.ReceiptNo);
                ////MessagingCenter.Send<Object, string>(this, "Receipt Information", Text1);

                string Text1 = string.Format("{0}", trnsSaleReceiptMaster.GetReceiptSeq());
                MessagingCenter.Send<Object, string>(this, "Receipt Information", "R#: " + Text1);

                string Text2 = string.Format("{0} {1}", RegiTotal.RegiHeader.UserID, RegiTotal.RegiHeader.UserName);
                MessagingCenter.Send<Object, string>(this, "User Information", Text2);

                MessagingCenter.Send<Object, string>(this, "Hold Count", string.Format("{0:000}", RegiTotal.RegiHeader.HoldCount));
                //MessagingCenter.Send<Object, string>(this, "Wait Count", string.Format("{0:000}", TransactionReader.GetCount("")));
                MessagingCenter.Send<Object, string>(this, "Wait Count", string.Format("{0:00000000}", RraSdcJsonWriter.GetCount()));
            }
        }
        public void InvalidateSurfaceHoldWait()
        {
            MessagingCenter.Send<Object, string>(this, "Hold Count", string.Format("{0:000}", RegiTotal.RegiHeader.HoldCount));
            MessagingCenter.Send<Object, string>(this, "Wait Count", string.Format("{0:00000000}", RraSdcJsonWriter.GetCount()));
        }

        public void SetLogoImage(string logoImage)
        {
            LogoImage = logoImage;
            //MessagingCenter.Send<Object, string>(this, "Logo Image", LogoImage);
        }
        public void SetSalesTitleColor(string salesTitleColor)
        {
            SalesTitleColor = salesTitleColor;
            //MessagingCenter.Send<Object, string>(this, "Sales Title Color", SalesTitleColor);
        }
        public void SetSalesTitleText(string salesTitleText)
        {
            SalesTitleText = salesTitleText;
            //MessagingCenter.Send<Object, string>(this, "Sales Title Text", SalesTitleText);
        }
        public void SetDeviceType()
        {
            
            DeviceType = "UWP";
        }
    }
}
