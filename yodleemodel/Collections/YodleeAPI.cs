using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using yodleemodel.Models;
using System.Web;

namespace yodleemodel.Collections
{
    public class YodleeAPI
    {
        private string yodlee_consumer_key = "", yodlee_consumer_secret = "", yodlee_app_id = "", yodlee_url = "", yodlee_fastlink_url = "";
        private string yodlee_cobrand_username = "", yodlee_cobrand_password = "";
        private string yodlee_login = "", yodlee_password = "";
        cobLoginAuthDetails cobrand = null;
        userLoginAuthDetails user = null;

        public YodleeAPI()
        {
            yodlee_consumer_key = System.Configuration.ConfigurationManager.AppSettings["yodlee_consumer_key"];
            yodlee_consumer_secret = System.Configuration.ConfigurationManager.AppSettings["yodlee_consumer_secret"];
            yodlee_app_id = System.Configuration.ConfigurationManager.AppSettings["yodlee_app_id"];
            yodlee_url = System.Configuration.ConfigurationManager.AppSettings["yodlee_url"];
            yodlee_fastlink_url = System.Configuration.ConfigurationManager.AppSettings["yodlee_fastlink_url"];

            yodlee_cobrand_username = System.Configuration.ConfigurationManager.AppSettings["yodlee_cobrand_username"];
            yodlee_cobrand_password = System.Configuration.ConfigurationManager.AppSettings["yodlee_cobrand_password"];

            yodlee_login = System.Configuration.ConfigurationManager.AppSettings["yodlee_login"];
            yodlee_password = System.Configuration.ConfigurationManager.AppSettings["yodlee_password"];

            get_cobrand_session_token();
            get_user_session_token();

        }

        #region yodlee credentials

        private void get_cobrand_session_token()
        {
            string url = yodlee_url + "/authenticate/coblogin";
            string query_string = "?cobrandLogin=" + yodlee_cobrand_username 
                                + "&cobrandPassword=" + yodlee_cobrand_password;

            string response = HTTPApiRequest.WebRequest(HTTPApiRequestMethod.POST, (url + query_string), "", "", "");
            cobrand = Newtonsoft.Json.JsonConvert.DeserializeObject<cobLoginAuthDetails>(response);
        }

        private void get_user_session_token()
        {
            string url = yodlee_url + "/authenticate/login";
            string query_string = "?cobSessionToken=" + urlencode_val(cobrand.cobrandConversationCredentials.sessionToken)
                                + "&login=" + yodlee_login
                                + "&password=" + urlencode_val(yodlee_password);

            string response = HTTPApiRequest.WebRequest(HTTPApiRequestMethod.POST, (url + query_string), "", "", "");
            user = Newtonsoft.Json.JsonConvert.DeserializeObject<userLoginAuthDetails>(response);
        }

        private string urlencode_val(string val)
        {
            return System.Web.HttpUtility.UrlEncode(val);
        }

        #endregion


        #region APIs

        public string executeUserSearchRequest()
        {
            string url = yodlee_url + "/jsonsdk/TransactionSearchService/executeUserSearchRequest";
            string query_string = "?cobSessionToken=" + urlencode_val(cobrand.cobrandConversationCredentials.sessionToken)
                                + "&userSessionToken=" + urlencode_val(user.userContext.conversationCredentials.sessionToken)
                                + "&transactionSearchRequest.containerType=bank"
                                + "&transactionSearchRequest.higherFetchLimit=50000"
                                + "&transactionSearchRequest.lowerFetchLimit=1"
                                + "&transactionSearchRequest.resultRange.endNumber=100"
                                + "&transactionSearchRequest.resultRange.startNumber=1"
                                + "&transactionSearchRequest.searchClients.clientId=1"
                                + "&transactionSearchRequest.searchClients.clientName=DataSearchService"
                                + "&transactionSearchRequest.userInput=dag"
                                + "&transactionSearchRequest.searchFilter.transactionSplitType=ALL_TRANSACTION";
            
            return HTTPApiRequest.WebRequest(HTTPApiRequestMethod.POST, (url + query_string), "", "", "");
        }

        public string getUserTransactions(string identifier)
        {
            string url = "/jsonsdk/TransactionSearchService/getUserTransactions";
            string query_string = "?cobSessionToken=" + urlencode_val(cobrand.cobrandConversationCredentials.sessionToken)
                                + "&userSessionToken=" + urlencode_val(user.userContext.conversationCredentials.sessionToken)
                                + "&searchFetchRequest.searchIdentifier.identifier=" + identifier
                                + "&searchFetchRequest.searchResultRange.startNumber=1"
                                + "&searchFetchRequest.searchResultRange.endNumber=500000";

            return HTTPApiRequest.WebRequest(HTTPApiRequestMethod.POST, (url + query_string), "", "", "application/json");
        }

        #endregion


    }
}
