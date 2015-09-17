using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace yodleemodel.Collections
{
    public class HTTPApiRequest
    {
        public static string WebRequest(HTTPApiRequestMethod method, string url, string postData, string access_token, string accept_type)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ContentType = "application/json";

            if (!string.IsNullOrWhiteSpace(accept_type))
                webRequest.Accept = accept_type;

            //Method types
            if (method == HTTPApiRequestMethod.DELETE)
            {
                if (!string.IsNullOrWhiteSpace(access_token))
                    webRequest.Headers.Add("Authorization", "Bearer " + access_token);
            }
            else if (method == HTTPApiRequestMethod.POST)
            {
                if (!string.IsNullOrWhiteSpace(postData))
                {
                    requestWriter = new StreamWriter(webRequest.GetRequestStream());
                    requestWriter.Write(postData);
                    requestWriter.Close();
                    requestWriter = null;
                }
            }
            
            return WebResponse(webRequest);;
        }        

        private static string WebResponse(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {   
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }
    }

    public enum HTTPApiRequestMethod
    {
        GET,
        POST,
        DELETE,
        PUT,
        PATCH
    }
}
