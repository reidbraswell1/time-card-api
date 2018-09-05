using System.Threading.Tasks;
using System.Collections.Generic;
namespace TimeCard.Shared
{
    public interface IAuthRepository
    {
        AuthRoot GetAuth(string id);
    }
}