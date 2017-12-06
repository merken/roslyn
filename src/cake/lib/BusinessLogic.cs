using System;

namespace lib
{
    public class BusinessLogic
    {
        public string GetSwag(string username)
        {
            username = username.ToLower();
            if (username == "bill")
                return "William Gate$$$$$";
            if (username == "elon")
                return "Te$la";
            if (username == "steve")
                return "0xDEADBEAF";

            throw new NotSupportedException($"{username} is not supported.");
        }
    }
}
