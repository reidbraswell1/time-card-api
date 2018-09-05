using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TimeCard.Shared
{
    public class Auth
    {
        public string Id { get; set; }
        public string AuthKey { get; set; }
        public DateTime DateModified { get; set; }
    }
    public class AuthJson
    {
        public string Id { get; set; }
        public string AuthKey { get; set; }
        private string dateModified;
        public string DateModified
        {
            get
            {
                return DateTime.Parse(dateModified).ToLocalTime().ToString("yyyy-MM-ddThh:mm:ss tt");
            }
            set { dateModified = value; }
        }
    }
    public class AuthRootCollection
    {
        public IEnumerable<Auth> Auth { get; set; }
    }
    public class AuthRootCollectionJson
    {
        public IEnumerable<AuthJson> AuthJson { get; set; }
    }
    public class AuthRoot
    {
        public Auth Auth { get; set; }
    }
    public class AuthRootJson
    {
        public AuthJson AuthJson { get; set; }
    }
}