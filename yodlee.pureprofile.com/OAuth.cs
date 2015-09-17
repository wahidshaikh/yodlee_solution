using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace yodlee.pureprofile.com.Controllers
{
    public class OAuth
    {
        #region OAuth

        public string GetPost(string url, string ConsumerKey, string ConsumerSecret, string TokenKey, string TokenSecret, string CallbackURL, string Verifier, WebMethod method)
        {
            string nonce, timestamp;
            string outUrl = "";
            string querystring = "";
            string signature = GetSignature(ConsumerKey, ConsumerSecret, TokenKey, TokenSecret, method, url, out timestamp, out nonce, out outUrl, out querystring, CallbackURL, Verifier);
            querystring += "&oauth_signature=" + signature;
            string response = WebRequestGet(method, url, querystring, querystring);
            return response;
        }

        public string Get(string url, string ConsumerKey, string ConsumerSecret, string TokenKey, string TokenSecret, string CallbackURL, string Verifier, WebMethod method)
        {
            string nonce, timestamp;
            string outUrl = "";
            string querystring = "";
            string signature = GetSignature(ConsumerKey, ConsumerSecret, TokenKey, TokenSecret, method, url, out timestamp, out nonce, out outUrl, out querystring, CallbackURL, Verifier);
            HttpWebRequest request = CreateWebRequest(ConsumerKey, TokenKey, url, method, nonce, timestamp, signature);
            return GetWebResponse(request);
        }

        #region Generate Signature

        #region Variables

        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        protected const string OAuthVersion = "1.0";
        protected const string OAuthParameterPrefix = "oauth_";
        protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
        protected const string OAuthCallbackKey = "oauth_callback";
        protected const string OAuthVersionKey = "oauth_version";
        protected const string OAuthSignatureMethodKey = "oauth_signature_method";
        protected const string OAuthSignatureKey = "oauth_signature";
        protected const string OAuthTimestampKey = "oauth_timestamp";
        protected const string OAuthNonceKey = "oauth_nonce";
        protected const string OAuthTokenKey = "oauth_token";
        protected const string OAuthTokenSecretKey = "oauth_token_secret";
        protected const string OAuthVerifierKey = "oauth_verifier"; // JDevlin        

        protected const string HMACSHA1SignatureType = "HMAC-SHA1";
        protected const string PlainTextSignatureType = "PLAINTEXT";
        protected const string RSASHA1SignatureType = "RSA-SHA1";

        #endregion

        public string GetSignature(string ConsumerKey, string ConsumerSecret, string TokenKey, string TokenSecret, WebMethod method, string url, out string timestamp, out string nonce, out string nurl, out string nrp, string CallbackURL, string Verifier)
        {
            nonce = GenerateNonce();
            timestamp = GenerateTimeStamp();
            //string nurl, nrp;

            Uri uri = new Uri(url);
            string sig = GenerateSignature(
                uri,
                ConsumerKey,
                ConsumerSecret,
                TokenKey,
                TokenSecret,
                method.ToString(),
                timestamp,
                nonce,
                SignatureTypes.HMACSHA1, out nurl, out nrp, CallbackURL, Verifier);

            //  return System.Web.HttpUtility.UrlEncode(sig);
            return sig;
        }

        public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, out string normalizedUrl, out string normalizedRequestParameters, string CallbackURL, string Verifier)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return consumerSecret + "&" + tokenSecret;

                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, HMACSHA1SignatureType, out normalizedUrl, out normalizedRequestParameters, CallbackURL, Verifier);

                    HMACSHA1 hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));

                    return GenerateSignatureUsingHash(signatureBase, hmacsha1);
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        public string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string signatureType, out string normalizedUrl, out string normalizedRequestParameters, string CallbackURL, string Verifier)
        {
            if (token == null)
            {
                token = string.Empty;
            }

            if (tokenSecret == null)
            {
                tokenSecret = string.Empty;
            }

            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            if (string.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            if (string.IsNullOrEmpty(signatureType))
            {
                throw new ArgumentNullException("signatureType");
            }

            normalizedUrl = null;
            normalizedRequestParameters = null;

            List<QueryParameter> parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));
            parameters.Add(new QueryParameter("access_type", "oauthdeeplink"));
            parameters.Add(new QueryParameter("displayMode", "desktop"));
            parameters.Add(new QueryParameter("cbLocation", "top"));


            if (!string.IsNullOrEmpty(CallbackURL))
            {
                parameters.Add(new QueryParameter(OAuthCallbackKey, UrlEncode(CallbackURL)));
                //string encodevalue = UrlEncode("r_emailaddress r_network r_fullprofile");//HttpUtility.UrlEncode("r_emailaddress+r_network");
                //parameters.Add(new QueryParameter("scope", encodevalue));
                //parameters.Add(new QueryParameter("scope", "r_emailaddress+r_network"));
            }

            if (!string.IsNullOrEmpty(token))
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, token));
            }

            if (!string.IsNullOrEmpty(Verifier))
            {
                parameters.Add(new QueryParameter(OAuthVerifierKey, Verifier));
            }


            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        protected class QueryParameterComparer : IComparer<QueryParameter>
        {

            #region IComparer<QueryParameter> Members

            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                {
                    return string.Compare(x.Value, y.Value);
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }

            #endregion
        }

        private List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            List<QueryParameter> result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);




            return Convert.ToBase64String(hashBytes);
        }

        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            //string timestamp = ts.TotalSeconds.ToString();
            //int length = timestamp.IndexOf(".") > -1 ? timestamp.IndexOf(".") : timestamp.Length;
            //timestamp = timestamp.Substring(0, length);
            //return timestamp;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static string GenerateNonce()
        {
            string randonstring = Path.GetRandomFileName();
            randonstring = randonstring.Replace(".", "");
            Random random = new Random();
            // return random.Next(123400, 9999999).ToString();
            return random.Next().ToString();
        }

        protected string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1
        }

        protected class QueryParameter
        {
            private string name = null;
            private string value = null;

            public QueryParameter(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get { return name; }
            }

            public string Value
            {
                get { return value; }
            }
        }

        #endregion

        #region WebRequest

        public enum WebMethod
        {
            GET,
            POST,
            DELETE,
            PUT
        }

        private static HttpWebRequest CreateWebRequest(string ConsumerKey, string AccessTokey, string fullUrl, WebMethod method, string nonce, string timeStamp, string sig)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
            request.Method = method.ToString();
            //request.Proxy = Session.WebProxy;
            string authHeader = CreateAuthHeader(ConsumerKey, AccessTokey, method, nonce, timeStamp, sig);
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", authHeader);

            return request;
        }

        public static string GetWebResponse(HttpWebRequest request)
        {
            string location = string.Empty;
            return GetWebResponse(request, out location);
        }

        public static string GetWebResponse(HttpWebRequest request, out string location)
        {

            WebResponse response = null;
            string data = string.Empty;
            location = string.Empty;
            try
            {
                response = request.GetResponse();
                location = response.Headers["Location"];
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    data = reader.ReadToEnd();

            }
            catch (WebException e)
            {
                data = "-1";

                using (WebResponse response1 = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response1;
                    using (Stream data1 = response1.GetResponseStream())
                    {
                        string text = new StreamReader(data1).ReadToEnd();
                        string text1 = text;
                    }
                }
            }
            finally
            {
                if (response != null)
                    response.Close();

            }

            return data;
        }

        public string WebRequestGet(WebMethod method, string url, string postData, string OAuth_Header)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            if (WebMethod.GET == method)
                url = url + "?" + OAuth_Header;

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;

            if (method == WebMethod.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);
            webRequest = null;
            return responseData;
        }

        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        string text = new StreamReader(data).ReadToEnd();
                        string text1 = text;
                    }
                }

                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }
            return responseData;
        }

        private static string CreateAuthHeader(string ConsumerKey, string AccessTokey, WebMethod method, string nonce, string timeStamp, string sig)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("OAuth ");
            if (method == WebMethod.POST)
                sb.Append("realm=\"" + "\",");
            else
                sb.Append("realm=\"\",");

            string authHeader = "oauth_consumer_key=\"" + ConsumerKey + "\"," +
                                "oauth_token=\"" + AccessTokey + "\"," +
                                "oauth_nonce=\"" + nonce + "\"," +
                                "oauth_timestamp=\"" + timeStamp + "\"," +
                                "oauth_signature_method=\"" + "HMAC-SHA1" + "\"," +
                                "oauth_version=\"" + "1.0" + "\"," +
                                "oauth_signature=\"" + sig + "\"";


            sb.Append(authHeader);
            return sb.ToString();
        }

        private static string CreateAuthHeaderGet(string ConsumerKey, WebMethod method, string nonce, string timeStamp, string sig, string CallBack, string verifier)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("OAuth ");
            if (method == WebMethod.POST)
                sb.Append("realm=\"" + "\",");
            else
                sb.Append("realm=\"\",");

            string authHeader = "oauth_consumer_key=\"" + ConsumerKey + "\"," +
                                "oauth_callback=\"" + CallBack + "\"," +
                                "oauth_nonce=\"" + nonce + "\"," +
                                "oauth_timestamp=\"" + timeStamp + "\"," +
                                "oauth_signature_method=\"" + "HMAC-SHA1" + "\"," +
                                "oauth_version=\"" + "1.0" + "\"," +
                                "oauth_signature=\"" + sig + "\"";
            if (!string.IsNullOrEmpty(verifier))
                authHeader += ",oauth_verifier=\"" + verifier + "\"";


            sb.Append(authHeader);
            return sb.ToString();
        }

        private static string AuthHeader(string ConsumerKey, string AccessTokey, WebMethod method, string nonce, string timeStamp, string sig, string CallbackURL, string Verifier)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("OAuth ");
            //if (method == WebMethod.POST)
            //    sb.Append("realm=\"" + "\",");
            //else
            //    sb.Append("realm=\"\",");

            string authHeader = "oauth_consumer_key=" + ConsumerKey + "&" +
                                "oauth_nonce=" + nonce + "&" +
                                "oauth_timestamp=" + timeStamp + "&" +
                                "oauth_signature_method=" + "HMAC-SHA1" + "&" +
                                "oauth_version=" + "1.0" + "&" +
                                "oauth_signature=" + sig;

            sb.Append(authHeader);
            return sb.ToString();
        }

        #endregion

        #endregion
    }
}