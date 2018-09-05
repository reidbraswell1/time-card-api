using System.Threading.Tasks;
using System.Collections.Generic;
using System;
namespace TimeCard.Shared
{
    public interface IPayRepository
    {
        int AddPay(Pay pay);
        int DeletePay(int id);
        int UpdatePay(Pay pay);
        PayRoot GetPay(int id);
        //Task<IEnumerable<Sitter>> GetSitters();
        PayRootJson GetPayString(int id);
        Task<PayRootCollectionJson> GetPays();
        Task<PayRootCollectionJson> GetPays(int id);
        Task<PayRootCollectionJson> GetPays(int id, DateTime date);
    }
}