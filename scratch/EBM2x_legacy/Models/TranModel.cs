using EBM2x.Models.config;
using EBM2x.Models.journal;
using EBM2x.Models.open;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using System.Runtime.Serialization;

namespace EBM2x.Models
{
    [DataContract]
    public class TranModel
    {
        
        [DataMember(EmitDefaultValue = true, Order = 0)]
        public TranInformation TranInformation { get; set; }

        // TranNode
        [DataMember(EmitDefaultValue = true, Order = 1)]
        public TranNode TranNode { get; set; }

        [DataMember(EmitDefaultValue = true, Order = 2)]
        public OpenNode OpenNode { get; set; }
        [DataMember(EmitDefaultValue = true, Order = 3)]
        public SignOnNode SignOnNode { get; set; }

        [DataMember(EmitDefaultValue = true, Order = 4)]
        public JournalModel Journal { get; set; }                 

        [DataMember(EmitDefaultValue = true, Order = 5)]
        public int ItemListCountOfItemsToDisplayOnOnePage { get; set; }
        [DataMember(EmitDefaultValue = true, Order = 6)]
        public int TenderListCountOfItemsToDisplayOnOnePage { get; set; }


        public string OverwriteSaledate { get; set; }
        public string OverwriteOrgBarcodeNo { get; set; }


        public TranModel()
        {
            TranInformation = new TranInformation();

            Clear();

            ItemListCountOfItemsToDisplayOnOnePage = 0;
            TenderListCountOfItemsToDisplayOnOnePage = 0;
        }

        public void SetTranModel(TranInformation tranInformation)
        {
            TranInformation.initialize(tranInformation);
        }

        public void Clear()
        {
            TranNode = null;
            OpenNode = null;
            SignOnNode = null;

            Journal = null;
        }
    }
}
