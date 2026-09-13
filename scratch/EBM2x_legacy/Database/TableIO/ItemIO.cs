using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Database.TableIO
{
    public class ItemIO
    {
        public string ITEM_CD { get; set; }
        public string ITEM_CLS_CD { get; set; }
        public string ITEM_NM { get; set; }
        public string ITEM_TY_CD { get; set; }
        public double INITQTY { get; set; }
        public double PURCHASE { get; set; }
        public double PRICE { get; set; }
        public double QTY { get; set; }

        public ItemIO(string iITEM_CD, string iITEM_CLS_CD, string iITEM_NM, string iITEM_TY_CD, double iINITQTY, double iPURCHASE, double iPRICE, double iQTY)
        {
            ITEM_CD = iITEM_CD;
            ITEM_CLS_CD = iITEM_CLS_CD;
            ITEM_NM = iITEM_NM;
            ITEM_TY_CD = iITEM_TY_CD;
            INITQTY = iINITQTY;
            PURCHASE = iPURCHASE;
            PRICE = iPRICE;
            QTY = iQTY;
        }

        public string getItem_Cd()
        {
            return ITEM_CD;
        }

        public string getItemClsCd()
        {
            return ITEM_CLS_CD;
        }

        public string getItemNm()
        {
            return ITEM_NM;
        }

        public string getItemTyCd()
        {
            return ITEM_TY_CD;
        }

        public double getInitQty()
        {
            return INITQTY;
        }

        public double getPurchase()
        {
            return PURCHASE;
        }

        public double getPrice()
        {
            return PRICE;
        }

        public double getQty()
        {
            return QTY;
        }
    }
}
