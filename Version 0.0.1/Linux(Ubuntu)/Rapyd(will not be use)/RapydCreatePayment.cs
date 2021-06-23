using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RapydApiRequest
{
    public static class RapydCreatePayment
    {
        public static RapydRequiredDataModel CreatePaymentRequest(String CountryCode) 
        {
            try
            {
                string[] payment_method_type_categories = new string[] { "card"};

                var requestObj = new
                {
                    amount = 5,
                    country = CountryCode,
                    currency = "USD",
                    language = "en",
                    payment_method_type_categories = payment_method_type_categories,
                };

                string request = JsonConvert.SerializeObject(requestObj);

                string result = RapydUtilities.MakeRequest("POST", "/v1/checkout", request);

                JObject JSonResultObjects = new JObject();
                JSonResultObjects = JObject.Parse(result);

                String ID = (String)JSonResultObjects["data"]["id"];

                Boolean Captured = (Boolean)JSonResultObjects["data"]["payment"]["captured"];

                Boolean Refunded = (Boolean)JSonResultObjects["data"]["payment"]["refunded"];

                Boolean Paid = (Boolean)JSonResultObjects["data"]["payment"]["paid"];

                int Paid_At = (int)JSonResultObjects["data"]["payment"]["paid_at"];

                String Redirect_Url = (String)JSonResultObjects["data"]["redirect_url"];

                RapydRequiredDataModel RequiredData = new RapydRequiredDataModel();

                RequiredData.ID = ID;

                RequiredData.Captured = Captured;

                RequiredData.Refunded = Refunded;

                RequiredData.Paid = Paid;

                RequiredData.Paid_At = Paid_At;

                RequiredData.Redirect_Url = Redirect_Url;

                return RequiredData;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error completing request: " + e.Message);

                return null;
            }
        }
    }
}
