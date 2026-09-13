using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.regitotal
{
    public class UnitTotal
    {
        //----------------------------------------------------------------------------
        // Variables declaration
        //----------------------------------------------------------------------------
        public string Type { get; set; }              
        public string Description { get; set; }     
        public int Count { get; set; }            
        public double Amount { get; set; }         
        public double SubtotalAmount { get; set; }
        public double DiscountAmount { get; set; }

        public UnitTotal(string type, string description)
        {
            Type = type;               
            Description = description;
            Count = 0;                 
            Amount = 0;                
            SubtotalAmount = 0d;       
            DiscountAmount = 0d;     
        }

        public void Clear()
        {
            Count = 0;                 
            Amount = 0;                
            SubtotalAmount = 0d;       
            DiscountAmount = 0d;    
        }

        public void Append(int count, int amount)
        {
            Count = count;
            Amount = amount;
            SubtotalAmount = Amount - DiscountAmount;
        }

        public void Append(int count, double amount, double discountAmount)
        {
            Count = count;
            Amount = amount;
            DiscountAmount = discountAmount;
            SubtotalAmount = Amount - DiscountAmount;
        }
    }
}
