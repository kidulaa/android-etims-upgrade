using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.Datafile.trlog;
    using EBM2x.RraSdc;
    using EBM2x.RraSdc.model;
    using EBM2x.Utils;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Description of ItemRraSdcUpload.
    /// </summary>
    public class ItemRraSdcUpload : ModelIO
    {
        public void SendItemSave(string valTin, string valItemCd)
        {
            List<ItemSaveReq> sendList = getItemTable(valTin, valItemCd);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Item";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_ITEM_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        public List<ItemSaveReq> getItemTable(string valTin, string valItemCd)
        {
            List<ItemSaveReq> arrayList = new List<ItemSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerItemTable taxpayerItemTable = new TaxpayerItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerItemTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND ITEM_CD = @ITEM_CD ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = valItemCd;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ItemSaveReq record = new ItemSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.itemCd = reader.GetString(1);               // Item Code
                    if (!reader.IsDBNull(2)) record.itemClsCd = reader.GetString(2);            // Item Classification Code (RRA)
                    if (!reader.IsDBNull(3)) record.itemTyCd = reader.GetString(3);             // Item Type Code
                    if (!reader.IsDBNull(4)) record.itemNm = reader.GetString(4);               // Item Name
                    if (!reader.IsDBNull(5)) record.itemStdNm = reader.GetString(5);            // Item Stand Name
                    if (!reader.IsDBNull(6)) record.orgnNatCd = reader.GetString(6);            // Origin National Code
                    if (!reader.IsDBNull(7)) record.pkgUnitCd = reader.GetString(7);            // Package Unit Code
                    if (!reader.IsDBNull(8)) record.qtyUnitCd = reader.GetString(8);            // Quantity Unit Code
                    if (!reader.IsDBNull(9)) record.taxTyCd = reader.GetString(9);              // Taxation Type Code
                    if (!reader.IsDBNull(10)) record.bcd = reader.GetString(10);                // Barcode
                    if (!reader.IsDBNull(11)) record.regBhfId = reader.GetString(11);           // Branch Office ID
                    if (!reader.IsDBNull(12)) record.useYn = reader.GetString(12);              // Use(Y/N)
                    if (!reader.IsDBNull(13)) record.rraModYn = reader.GetString(13);           // RRA Modified(Y/N)
                    if (!reader.IsDBNull(14)) record.addInfo = reader.GetString(14);            // Additional Information
                    if (!reader.IsDBNull(15)) record.sftyQty = reader.GetDouble(15);            // Safety Quantity
                    if (!reader.IsDBNull(16)) record.isrcAplcbYn = reader.GetString(16);        // Insurance Appicable(Y/N)
                    if (!reader.IsDBNull(17)) record.dftPrc = reader.GetDouble(17);             // Default Price
                    if (!reader.IsDBNull(18)) record.grpPrcL1 = reader.GetDouble(18);           // Group Default Price L1
                    if (!reader.IsDBNull(19)) record.grpPrcL2 = reader.GetDouble(19);           // Group Default Price L2
                    if (!reader.IsDBNull(20)) record.grpPrcL3 = reader.GetDouble(20);           // Group Default Price L3
                    if (!reader.IsDBNull(21)) record.grpPrcL4 = reader.GetDouble(21);           // Group Default Price L4
                    if (!reader.IsDBNull(22)) record.grpPrcL5 = reader.GetDouble(22);           // Group Default Price L5
                    if (!reader.IsDBNull(23)) record.regrId = reader.GetString(23);             // Registrant ID
                    if (!reader.IsDBNull(24)) record.regrNm = reader.GetString(24);             // Registrant Name
                    //if (!reader.IsDBNull(25)) record.regDt = reader.GetString(25);      // Registered Date
                    if (!reader.IsDBNull(26)) record.modrId = reader.GetString(26);             // Modifier ID
                    if (!reader.IsDBNull(27)) record.modrNm = reader.GetString(27);             // Modifier Name
                    //if (!reader.IsDBNull(28)) record.ModDt = reader.GetString(28);      // Modified Date
                    //if (!reader.IsDBNull(29)) record.InitlWhUntpc = reader.GetDouble(29);       // 초기 입고단가
                    //if (!reader.IsDBNull(30)) record.InitlQty = reader.GetDouble(30);           // 초기 입고수량
                    //if (!reader.IsDBNull(31)) record.Rm = reader.GetString(31);                 // 비고
                    //if (!reader.IsDBNull(32)) record.UseBarcode = reader.GetString(32);         // 바코드사용여부
                    //if (!reader.IsDBNull(33)) record.UseAdiYn = reader.GetString(33);           // 부가정보사용여부
                    if (!reader.IsDBNull(34)) record.btchNo = reader.GetString(34);           // BatchNum
                    //if (!reader.IsDBNull(35)) record.ExpirationDtUse = reader.GetString(35);    // Expiration Dt Use

                    //record.OrgnNatName = CodeDtlMaster.OrgnNatName(record.OrgnNatCd);           // Origin National Namee
                    //record.ItemTyName = CodeDtlMaster.ItemTyName(record.ItemTyCd);              // Item Type Name
                    //record.PkgUnitName = CodeDtlMaster.PkgUnitName(record.PkgUnitCd);           // Package Unit Name
                    //record.QtyUnitName = CodeDtlMaster.QtyUnitName(record.QtyUnitCd);           // Quantity Unit Name
                    //record.TaxTyName = CodeDtlMaster.TaxTyName(record.TaxTyCd);                 // Taxation Type Name
                    //record.ItemClsName = ItemClassMaster.GetItemClassName(record.ItemClsCd);    // Item Classification Name (RRA)
                    //record.RdsQty = StockMasterMaster.GetCurrentStock(record.Tin, record.ItemCd); // 현재고 수량
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
        public List<ItemSaveReq> getItemTable()
        {
            List<ItemSaveReq> arrayList = new List<ItemSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerItemTable taxpayerItemTable = new TaxpayerItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerItemTable.GetSelectSQL());

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ItemSaveReq record = new ItemSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.itemCd = reader.GetString(1);               // Item Code
                    if (!reader.IsDBNull(2)) record.itemClsCd = reader.GetString(2);            // Item Classification Code (RRA)
                    if (!reader.IsDBNull(3)) record.itemTyCd = reader.GetString(3);             // Item Type Code
                    if (!reader.IsDBNull(4)) record.itemNm = reader.GetString(4);               // Item Name
                    if (!reader.IsDBNull(5)) record.itemStdNm = reader.GetString(5);            // Item Stand Name
                    if (!reader.IsDBNull(6)) record.orgnNatCd = reader.GetString(6);            // Origin National Code
                    if (!reader.IsDBNull(7)) record.pkgUnitCd = reader.GetString(7);            // Package Unit Code
                    if (!reader.IsDBNull(8)) record.qtyUnitCd = reader.GetString(8);            // Quantity Unit Code
                    if (!reader.IsDBNull(9)) record.taxTyCd = reader.GetString(9);              // Taxation Type Code
                    if (!reader.IsDBNull(10)) record.bcd = reader.GetString(10);                // Barcode
                    if (!reader.IsDBNull(11)) record.regBhfId = reader.GetString(11);           // Branch Office ID
                    if (!reader.IsDBNull(12)) record.useYn = reader.GetString(12);              // Use(Y/N)
                    if (!reader.IsDBNull(13)) record.rraModYn = reader.GetString(13);           // RRA Modified(Y/N)
                    if (!reader.IsDBNull(14)) record.addInfo = reader.GetString(14);            // Additional Information
                    if (!reader.IsDBNull(15)) record.sftyQty = reader.GetDouble(15);            // Safety Quantity
                    if (!reader.IsDBNull(16)) record.isrcAplcbYn = reader.GetString(16);        // Insurance Appicable(Y/N)
                    if (!reader.IsDBNull(17)) record.dftPrc = reader.GetDouble(17);             // Default Price
                    if (!reader.IsDBNull(18)) record.grpPrcL1 = reader.GetDouble(18);           // Group Default Price L1
                    if (!reader.IsDBNull(19)) record.grpPrcL2 = reader.GetDouble(19);           // Group Default Price L2
                    if (!reader.IsDBNull(20)) record.grpPrcL3 = reader.GetDouble(20);           // Group Default Price L3
                    if (!reader.IsDBNull(21)) record.grpPrcL4 = reader.GetDouble(21);           // Group Default Price L4
                    if (!reader.IsDBNull(22)) record.grpPrcL5 = reader.GetDouble(22);           // Group Default Price L5
                    if (!reader.IsDBNull(23)) record.regrId = reader.GetString(23);             // Registrant ID
                    if (!reader.IsDBNull(24)) record.regrNm = reader.GetString(24);             // Registrant Name
                    //if (!reader.IsDBNull(25)) record.regDt = reader.GetString(25);      // Registered Date
                    if (!reader.IsDBNull(26)) record.modrId = reader.GetString(26);             // Modifier ID
                    if (!reader.IsDBNull(27)) record.modrNm = reader.GetString(27);             // Modifier Name
                    //if (!reader.IsDBNull(28)) record.ModDt = reader.GetString(28);      // Modified Date
                    //if (!reader.IsDBNull(29)) record.InitlWhUntpc = reader.GetDouble(29);       // 초기 입고단가
                    //if (!reader.IsDBNull(30)) record.InitlQty = reader.GetDouble(30);           // 초기 입고수량
                    //if (!reader.IsDBNull(31)) record.Rm = reader.GetString(31);                 // 비고
                    //if (!reader.IsDBNull(32)) record.UseBarcode = reader.GetString(32);         // 바코드사용여부
                    //if (!reader.IsDBNull(33)) record.UseAdiYn = reader.GetString(33);           // 부가정보사용여부
                    if (!reader.IsDBNull(34)) record.btchNo = reader.GetString(34);           // BatchNum
                    //if (!reader.IsDBNull(35)) record.ExpirationDtUse = reader.GetString(35);    // Expiration Dt Use

                    //record.OrgnNatName = CodeDtlMaster.OrgnNatName(record.OrgnNatCd);           // Origin National Namee
                    //record.ItemTyName = CodeDtlMaster.ItemTyName(record.ItemTyCd);              // Item Type Name
                    //record.PkgUnitName = CodeDtlMaster.PkgUnitName(record.PkgUnitCd);           // Package Unit Name
                    //record.QtyUnitName = CodeDtlMaster.QtyUnitName(record.QtyUnitCd);           // Quantity Unit Name
                    //record.TaxTyName = CodeDtlMaster.TaxTyName(record.TaxTyCd);                 // Taxation Type Name
                    //record.ItemClsName = ItemClassMaster.GetItemClassName(record.ItemClsCd);    // Item Classification Name (RRA)
                    //record.RdsQty = StockMasterMaster.GetCurrentStock(record.Tin, record.ItemCd); // 현재고 수량
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
    }
}
