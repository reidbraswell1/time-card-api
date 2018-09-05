using System.Threading.Tasks;
using System.Collections.Generic;
namespace TimeCard.Shared
{
    public interface ISitterRepository
    {
        int AddSitter(Sitter sitter);
        int DeleteSitter(int id);
        int UpdateSitter(Sitter sitter);
        SittersRoot GetSitter(int id);
        //Task<IEnumerable<Sitter>> GetSitters();
        Task<SittersRootCollectionJson> GetSitters();
    }
}