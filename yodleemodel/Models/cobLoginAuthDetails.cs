using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yodleemodel.Models
{
    //Co-brand
    public class cobLoginAuthDetails
    {
        public string cobrandId { get; set; }
        public string channelId { get; set; }
        public string locale { get; set; }
        public string tncVersion { get; set; }
        public string applicationId { get; set; }
        public SessionToekn cobrandConversationCredentials { get; set; }
    }

    //user
    public class userLoginAuthDetails
    {
        public userContext userContext { get; set; }
    }

    public class userContext
    {
        public SessionToekn conversationCredentials { get; set; }
        public bool valid { get; set; }
        public string isPasswordExpired { get; set; }
        public string cobrandId { get; set; }
        public string channelId { get; set; }
        public string locale { get; set; }
        public string tncVersion { get; set; }
        public string applicationId { get; set; }
        public SessionToekn cobrandConversationCredentials { get; set; }
    }

    public class SessionToekn
    {
        public string sessionToken { get; set; }
    }
}
