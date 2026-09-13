using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.signon
{
    public class SignOnNode
    {
        public string CashierID { get; set; }
        public string CashierName { get; set; }
        public string Password { get; set; }
        public string Permission { get; set; }     

        public SignOnNode()
        {
            CashierID = string.Empty;
            CashierName = string.Empty;
            Password = string.Empty;
            Permission = string.Empty;
        }
    }
}
