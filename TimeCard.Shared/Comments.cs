using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TimeCard.Shared
{
    public class Comments 
    {
        public DateTime Id { get; set; }
        public string Comment { get; set; }
        public int SitterId { get; set; }
        public int PayId { get; set; }
        public DateTime DateModified { get; set; }
    }
}