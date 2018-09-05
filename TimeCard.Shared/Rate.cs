using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TimeCard.Shared
{
    public class Rate 
    {
        public int Id { get; set; }
        public DateTime TimePunch { get; set; }
        public int SitterId { get; set; }
        public DateTime DateModified { get; set; }
    }
    public class RateRootCollection
    {
        public IEnumerable<Rate> Punches { get; set; }
    }
    public class RateRoot
    {
        public Rate Rate { get; set; }
    }
}