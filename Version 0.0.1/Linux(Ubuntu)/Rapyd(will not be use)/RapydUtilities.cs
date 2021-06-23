using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace RapydApiRequest
{
    public static class RapydUtilities
    {
        private const string accessKey = "{Rapyd API access Key}";
        private const string secretKey = "{Rapyd API secret Key}";

        public static string MakeRequest(string method, string urlPath, string body = null)
        {
            try
            {
                string httpMethod = method;
                Uri httpBaseURL = new Uri("https://sandboxapi.rapyd.net");
                string httpURLPath = urlPath;
                string httpBody = body;
                string salt = GenerateRandomString(8);
                long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                string signature = Sign(httpMethod, httpURLPath, salt, timestamp, httpBody);

                Uri httpRequestURL = new Uri(httpBaseURL, urlPath);
                WebRequest request = HttpWebRequest.Create(httpRequestURL);
                request.Method = httpMethod;
                request.ContentType = "application/json";
                request.Headers.Add("salt", salt);
                request.Headers.Add("timestamp", timestamp.ToString());
                request.Headers.Add("signature", signature);
                request.Headers.Add("access_key", accessKey);

                return HttpRequest(request, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error generating request options: ", ex);
                throw;
            }
        }


        private static string Sign(string method, string urlPath, string salt, long timestamp, string body)
        {

            try
            {
                string bodyString = String.Empty;
                if (!String.IsNullOrWhiteSpace(body))
                {
                    bodyString = body == "{}" ? "" : body;
                }

                string toSign = method.ToLower() + urlPath + salt + timestamp + accessKey + secretKey + bodyString;

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] secretKeyBytes = encoding.GetBytes(secretKey);
                byte[] signatureBytes = encoding.GetBytes(toSign);
                string signature = String.Empty;
                using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
                {
                    byte[] signatureHash = hmac.ComputeHash(signatureBytes);
                    string signatureHex = String.Concat(Array.ConvertAll(signatureHash, x => x.ToString("x2")));
                    signature = Convert.ToBase64String(encoding.GetBytes(signatureHex));
                }

                return signature;
            }
            catch (Exception)
            {
                Console.WriteLine("Error generating signature");
                throw;
            }

        }
        private static string GenerateRandomString(int size)
        {
            try
            {
                using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
                {
                    byte[] randomBytes = new byte[size];
                    rng.GetBytes(randomBytes);
                    return String.Concat(Array.ConvertAll(randomBytes, x => x.ToString("x2")));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error generating salt");
                throw;
            }
        }

        private static string HttpRequest(WebRequest request, string body)
        {
            string response = String.Empty;

            try
            {
                if (!String.IsNullOrWhiteSpace(body))
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        byte[] bodyBytes = encoding.GetBytes(body);
                        requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                        requestStream.Close();
                    }
                }

                using (WebResponse webResponse = request.GetResponse())
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            response = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (StreamReader streamReader = new StreamReader(e.Response.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occurred: " + e.Message);
            }

            return response;
        }
    }
}
