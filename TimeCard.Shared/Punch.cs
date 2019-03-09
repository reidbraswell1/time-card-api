using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TimeCard.Shared
{
    public class Punch
    {
        public int Id { get; set; }
        public DateTime TimePunch { get; set; }
        public long TimePunchSeconds { get; set; }
        public int SitterId { get; set; }
        public string Comment { get; set; }
        public DateTime DateModified { get; set; }
    }
    public class PunchJson
    {
        public string Id { get; set; }
        private DateTime timePunch;
        public string TimePunch
        {
            get
            {
                return (timePunch.CompareTo(DateTime.MinValue) == 0) ? null : timePunch.ToLocalTime().ToString("ddd MM/dd/yyyy hh:mm tt");
            }
            set
            {
                timePunch = DateTime.Parse(value);
            }
        }
        private double timePunchSeconds;
        public string TimePunchSeconds
        {
            get
            {
                return (timePunchSeconds == 0) ? null : Convert.ToString(timePunchSeconds);
            }

            set
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan diff = timePunch - origin;
                timePunchSeconds = diff.TotalSeconds;
            }
        }
        public string SitterId { get; set; }
        public string Comment { get; set; }
        private string dateModified;
        public string DateModified
        {
            get
            {
                return (String.IsNullOrEmpty(dateModified)) ? null : DateTime.Parse(dateModified).ToLocalTime().ToString("yyyy-MM-ddThh:mm:ss tt");
            }
            set
            {
                dateModified = value;
            }
        }
    }
    public class PunchRootCollection
    {
        public IEnumerable<Punch> Punches { get; set; }
    }
    public class PunchRootCollectionJson
    {
        public IEnumerable<PunchJson> PunchJson { get; set; }
    }
    public class PunchRoot
    {
        public Punch Punch { get; set; }
    }
    public class PunchRootJson
    {
        public PunchJson PunchJson { get; set; }
    }
}