using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RapydApiRequest
{
    public class RapydCheckPaymentHasMade
    {
        public static Boolean CheckPaymentHasMade(String CheckOutPageID)
        {
            try
            {
                string result = RapydUtilities.MakeRequest("GET", $"/v1/checkout/{CheckOutPageID}");

                JObject JSonResultObjects = new JObject();
                JSonResultObjects = JObject.Parse(result);

                int Paid_At = (int)JSonResultObjects["data"]["payment"]["paid_at"];

                if (Paid_At != 0) 
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error completing request: " + e.Message);
                return false;
            }
        }
    }
}
