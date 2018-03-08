//@Neke Kwetey
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace atmSimulator.Models
{   //for modelling out the transactions in Transactions.cshtml
    public class Transaction
    {

        public string user_id;
        public int trans_type;
        public string trans_amount;
        public string trans_to;
    }
}
