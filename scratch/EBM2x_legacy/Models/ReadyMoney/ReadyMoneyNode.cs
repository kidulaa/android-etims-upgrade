using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.ReadyMoney
{
    public class ReadyMoneyNode
    {
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Amount { get; set; }

        public ReadyMoneyNode()
        {
            Price = 0;
            Quantity = 0;
            Amount = 0;
        }

        public void SetQuantity(int qty)
        {
            Quantity = qty;
            Amount = Quantity * Price;
        }
    }
}
