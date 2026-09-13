using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.customer
{
    public class CustomerNode
    {
        public string Tin { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

        public string CustGroup { get; set; }
        

        public CustomerNode()
        {
            clear();
        }
        public CustomerNode(string tin, string customerCode, string customerName, string custGroup)
        {
            Tin = tin;
            CustomerCode = customerCode;
            CustomerName = customerName;
            CustGroup = custGroup;
        }

        public void clear()
        {
            Tin = string.Empty;
            CustomerCode = string.Empty;
            CustomerName = string.Empty;
            CustGroup = "DF";
        }
    }
}
