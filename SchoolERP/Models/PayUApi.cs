using Newtonsoft.Json;
using School;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SchoolERP.Models
{
    public class PayUApi
    {
        public static PayUResponseClass CallStatusAPI(string merchantTransactionIds)
        {
            //string merchantTransactionIds = string.Join("|", transactionIds);
            PayUResponseClass paymentResponses = null;
            string URI = "https://www.payumoney.com/payment/payment/chkMerchantTxnStatus?";
            string myParameters = "merchantKey=" + ConfigurationManager.AppSettings["MerchantKey"] + "&merchantTransactionIds=" + merchantTransactionIds;
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Headers.Add("Authorization", ConfigurationManager.AppSettings["AuthHeader"]);
                var HtmlResult = wc.UploadString(URI, myParameters);
                paymentResponses = JsonConvert.DeserializeObject<PayUResponseClass>(HtmlResult);
            }
            return paymentResponses;
        }
        private static string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }
        public static SettlementRoot SettlementDetails(string date)
        {
            string merchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            string merchantSalt = ConfigurationManager.AppSettings["MerchantSalt"];
            string hash = merchantKey + "|get_settlement_details|" + date + "|" + merchantSalt;
            hash = Generatehash512(hash);

            string URI = "https://info.payu.in/merchant/postservice?form=2";
            string myParameters = "key=" + merchantKey + "&command=get_settlement_details&hash=" + hash + "&var1=" + date + "&var3=2000&var5=1";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var root = new SettlementRoot();
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Headers[HttpRequestHeader.Accept] = "application/json";

                var HtmlResult = wc.UploadString(URI, myParameters);
                root = JsonConvert.DeserializeObject<SettlementRoot>(HtmlResult);
            }
            root.date = date;
            return root;
        }
    }
}