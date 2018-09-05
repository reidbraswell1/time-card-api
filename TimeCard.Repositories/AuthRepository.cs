using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TimeCard.Shared;
using Dapper;
using System.Text;
using System;
namespace TimeCard.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _conn;

        public AuthRepository(IDbConnection conn)
        {
            _conn = conn;
        }
        public AuthRoot GetAuth(string id)
        {
            using (var conn = _conn)
            {
                conn.Open();
                return new AuthRoot() { Auth = conn.QueryFirst<Auth>("SELECT * FROM `TIME_CARD`.`API_AUTH` WHERE Id = @Id;", new { id }) };
            }
        }
    }
}