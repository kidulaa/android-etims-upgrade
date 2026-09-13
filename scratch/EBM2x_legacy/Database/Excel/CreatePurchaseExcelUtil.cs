using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EBM2x.Database.Master;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EBM2x.Database.Excel
{
    public class CreatePurchaseExcelUtil
    {
        public void CreateSpreadsheetWorkbook(Stream stream, List<TrnsPurchaseRecord> listTrnsPurchaseRecord)
        {
            string jsonRequest = JsonConvert.SerializeObject(listTrnsPurchaseRecord);
            JArray jArray = JArray.Parse(jsonRequest);
            if (jArray.Count > 0)
            {
                JObject jObject0 = (JObject)jArray[0];
                IList<string> keys = jObject0.Properties().Select(p => p.Name).ToList();
                CreateSpreadsheetWorkbook(stream, keys, jsonRequest, listTrnsPurchaseRecord);
            }
        }
        public void CreateSpreadsheetWorkbook(Stream stream, IList<string> titles, string jsonRequest, List<TrnsPurchaseRecord> listTrnsPurchaseRecord)
        {
            TrnsPurchaseItemMaster masterItems = new TrnsPurchaseItemMaster();

            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "mySheet"
            };
            sheets.Append(sheet);

            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            UInt32Value rowIndex = 1;
            Row row = null;
            JArray jArray = JArray.Parse(jsonRequest);

            if (jArray.Count > 0)
            {
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject jObject0 = (JObject)jArray[0];
                    IList<string> keys = jObject0.Properties().Select(p => p.Name).ToList();
                    row = new Row() { RowIndex = rowIndex };
                    for (int k = 0; k < keys.Count; k++)
                    {
                        string strValue = titles[k];

                        Cell cell = new Cell()
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(strValue)
                        };
                        row.InsertAt(cell, k);
                    }
                    sheetData.Append(row);
                    rowIndex++;

                    row = new Row() { RowIndex = rowIndex };
                    JObject jObject = (JObject)jArray[i];
                    IList<JToken> values = jObject.Properties().Select(p => p.Value).ToList(); ;

                    for (int j = 0; j < values.Count; j++)
                    {
                        JToken jObjectValue = (JToken)values[j];
                        string strValue = jObjectValue.ToString();

                        Cell cell = new Cell()
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(strValue)
                        };
                        row.InsertAt(cell, j);
                    }
                    sheetData.Append(row);
                    rowIndex++;
                    rowIndex++;

                    // Item 
                    TrnsPurchaseRecord record = listTrnsPurchaseRecord[i];
                    List < TrnsPurchaseItemRecord > listTrnsPurchaseItemRecord = masterItems.getTrnsPurchaseItemTable(record.Tin, record.BhfId, record.SpplrTin, record.InvcNo);
                    string jsonRequestItem = JsonConvert.SerializeObject(listTrnsPurchaseItemRecord);
                    JArray jArrayItem = JArray.Parse(jsonRequestItem);
                    if (jArrayItem.Count > 0)
                    {
                        JObject jObjectItem0 = (JObject)jArrayItem[0];
                        IList<string> keyItems = jObjectItem0.Properties().Select(p => p.Name).ToList();
                        rowIndex = CreateSpreadsheetWorkbookItem(sheetData, rowIndex, keyItems, jsonRequestItem);
                        rowIndex++;
                        rowIndex++;
                        rowIndex++;
                    }

                }
            }

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }

        public UInt32Value CreateSpreadsheetWorkbookItem(SheetData sheetData, UInt32Value rowIndexStart, IList<string> titles, string jsonRequest)
        {
            UInt32Value rowIndex = rowIndexStart;
            Row row = null;
            JArray jArray = JArray.Parse(jsonRequest);

            if (jArray.Count > 0)
            {
                JObject jObject0 = (JObject)jArray[0];
                IList<string> keys = jObject0.Properties().Select(p => p.Name).ToList();
                row = new Row() { RowIndex = rowIndex };
                for (int k = 0; k < keys.Count; k++)
                {
                    string strValue = titles[k];

                    Cell cell = new Cell()
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(strValue)
                    };
                    row.InsertAt(cell, k);
                }
                sheetData.Append(row);
                rowIndex++;

                for (int i = 0; i < jArray.Count; i++)
                {
                    row = new Row() { RowIndex = rowIndex };

                    JObject jObject = (JObject)jArray[i];
                    IList<JToken> values = jObject.Properties().Select(p => p.Value).ToList(); ;

                    for (int j = 0; j < values.Count; j++)
                    {
                        JToken jObjectValue = (JToken)values[j];
                        string strValue = jObjectValue.ToString();

                        Cell cell = new Cell()
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(strValue)
                        };
                        row.InsertAt(cell, j);
                    }
                    sheetData.Append(row);
                    rowIndex++;
                }
            }

            return rowIndex;
        }
    }
}
