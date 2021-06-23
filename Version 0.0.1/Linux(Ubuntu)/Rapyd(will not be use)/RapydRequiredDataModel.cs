using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapydApiRequest
{
    public class RapydRequiredDataModel
    {
        public String ID { get; set; }

        public Boolean Captured { get; set; }

        public Boolean Refunded { get; set; }

        public Boolean Paid { get; set; }

        public int Paid_At { get; set; }

        public String Redirect_Url { get; set; }
    }
}
