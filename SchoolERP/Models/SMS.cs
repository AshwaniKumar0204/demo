using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Security;

/// <summary>
/// Summary description for SMS
/// </summary>
public class SMS
{
    public SMS()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //public static string UserName = ConfigurationManager.AppSettings["SMSUserName"].ToString();
    //public static string Password = ConfigurationManager.AppSettings["SMSPassword"].ToString();
    //public static string SenderId = ConfigurationManager.AppSettings["SMSSenderId"].ToString();

    public static int SendSms(string mobileNo, string message)
    {
        int retVal = 0;
        try
        {
            message = HttpUtility.UrlEncode(message, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            //string result = APICallOld("http://sms.wipenex.in/new/api/api_http.php?username=ggpsbokaro&password=Bokaro@Ggps2&senderid=GGPSBK&to=" + mobileNo + "&text=" + message + "&route=Transaction&type=text");
            string result = APICallOld("http://180.179.218.150/sendurlcomma.aspx?user=20079517&pwd=GGPS@Bokaro&msgtext="+message+"&mobileno="+mobileNo+"&senderid=GGPSCH&smstype=0");
            if (!result.StartsWith("Wrong Username or Password"))
                retVal = 1;
            else
                retVal = 0;
        }
        catch (Exception ex)
        {
            //throw ex;
        }
        return retVal;
    }

    public static int SendSmsNew(string mobileNo, string message)
    {
        int retVal = 0;
        try
        {
            message = HttpUtility.UrlEncode(message, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            string result = APICallOld("http://180.179.215.100/sendurlcomma.aspx?user=20079517&pwd=GGPS@Bokaro&senderid=GGPSCH&CountryCode=91&mobileno="+mobileNo+"&msgtext="+message+"&smstype=0");
            if (!result.StartsWith("Wrong Username or Password"))
                retVal = 1;
            else
                retVal = 0;
        }
        catch (Exception ex)
        {
            //throw ex;
        }
        return retVal;
    }


    public static int SendBulkSms(string mobileNos, string message)
    {
        int retVal = 0;
        try
        {
            //string result = APICallOld("http://sms.wipenex.in/new/api/api_http.php?username=ggpsbokaro&password=Bokaro@Ggps2&senderid=GGPSBK&to=" + mobileNo + "&text=" + message + "&route=Transaction&type=text");
            string result = APICallOld("http://180.179.218.150/sendurlcomma.aspx?user=20079517&pwd=GGPS@Bokaro&msgtext=" + message + "&mobileno=" + mobileNos + "&senderid=GGPSCH&smstype=0");
            if (!result.StartsWith("Wrong Username or Password"))
                retVal = 1;
            else
                retVal = 0;
        }
        catch (Exception ex)
        {
            //throw ex;
        }
        return retVal;
    }

    private static string APICallOld(string url)
    {
        HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);
        try
        {
            HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
            StreamReader sr = new StreamReader(httpres.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();
            return results;
        }
        catch
        {
            return "0";
        }
    }
    private static string APICall(string url)
    {
        HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
        try
        {
            HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
            StreamReader sr = new StreamReader(httpres.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();
            return results;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}
