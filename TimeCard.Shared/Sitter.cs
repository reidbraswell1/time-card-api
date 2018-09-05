using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TimeCard.Shared
{
    public class Sitter
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int RateId { get; set; }
        public decimal Rate { get; set; }
        public string Phone { get; set; }
        public string SMSGateway { get; set; }
        public string MMSGateway { get; set; }
        public DateTime DateModified { get; set; }
    }
    public class SitterJson
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string RateId { get; set; }
        public string Rate { get; set; }
        public string Phone { get; set; }
        public string SMSGateway { get; set; }
        public string MMSGateway { get; set; }
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
    public class SittersRootCollection
    {
        public IEnumerable<Sitter> Sitter { get; set; }
    }
    public class SittersRootCollectionJson
    {
        public IEnumerable<SitterJson> SitterJson { get; set; }
    }
    public class SittersRoot
    {
        public Sitter Sitter { get; set; }
    }
    public class SittersRootJson
    {
        public SitterJson SitterJson { get; set; }
    }
}