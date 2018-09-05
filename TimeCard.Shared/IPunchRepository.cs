using System.Threading.Tasks;
using System.Collections.Generic;
using System;
namespace TimeCard.Shared
{
    public interface IPunchRepository
    {
        int AddPunch(Punch punch);
        int DeletePunch(int id);
        int UpdatePunch(Punch punch);
        PunchRoot GetPunch(int id);
        //Task<IEnumerable<Sitter>> GetSitters();
        Task<PunchRootCollection> GetPunches();
        Task<PunchRootCollection> GetPunches(int id);
        Task<PunchRootCollectionJson> GetPunches(int id, DateTime periodStart, DateTime periodEnd);
    }
}