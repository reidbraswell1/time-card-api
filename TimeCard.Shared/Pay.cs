using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TimeCard.Shared
{
    public class Pay
    {
        public int Id { get; set; }
        public decimal PayAmt { get; set; }
        public decimal Hours { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime CheckDate { get; set; }
        public string CheckNumber { get; set; }
        public int SitterId { get; set; }
        public string Comment { get; set; }
        public DateTime DateModified { get; set; }
    }
    public class PayJson
    {
        public string Id { get; set; }
        public string PayAmt { get; set; }
        public string Hours { get; set; }
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }
        private string checkDate;
        public string CheckDate
        {
            get
            {
                return (checkDate.Length >= 7) ? checkDate.Substring(0, checkDate.IndexOf(' ')) : checkDate;
            }
            set { checkDate = value; }
        }
        public string CheckNumber { get; set; }
        public string SitterId { get; set; }
        public string Comment { get; set; }
        private string dateModified;
        public string DateModified
        {
            get
            {
                return DateTime.Parse(dateModified).ToLocalTime().ToString("yyyy-MM-ddThh:mm:ss tt");
            }
            set
            { dateModified = value; }
        }
    }
    public class PayRootCollection
    {
        public IEnumerable<Pay> Pay { get; set; }
    }
    public class PayRootCollectionJson
    {
        public IEnumerable<PayJson> PayJson { get; set; }
    }
    public class PayRoot
    {
        public Pay Pay { get; set; }
    }
    public class PayRootJson
    {
        public PayJson PayJson { get; set; }
    }
}