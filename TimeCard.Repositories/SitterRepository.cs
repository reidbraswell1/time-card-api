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
    public class SitterRepository : ISitterRepository
    {
        private readonly IDbConnection _conn;
        private const string SelectSql = "USE TIME_CARD; SELECT `S`.`Id`, `S`.`LastName`, `S`.`FirstName`, `S`.`RateId`, `R`.`Rate`, `S`.`Phone`, `S`.`SMSGateway`, `S`.`MMSGateway`, `S`.`DateModified` FROM `TIME_CARD`.`SITTERS` AS `S` LEFT JOIN `TIME_CARD`.`RATE` AS `R` ON `S`.`RateId` = `R`.`Id`";

        public SitterRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public int AddSitter(Sitter sitter)
        {
            sitter.DateModified = DateTime.UtcNow;
            StringBuilder addSitter = new StringBuilder();
            addSitter.Append("INSERT INTO `TIME_CARD`.`SITTERS` ");
            addSitter.Append("(`LastName`,");
            addSitter.Append("`FirstName`,");
            addSitter.Append("`RateId`,");
            addSitter.Append("`Phone`,");
            addSitter.Append("`SMSGateway`,");
            addSitter.Append("`MMSGateway`,");
            addSitter.Append("`DateModified`");
            addSitter.Append(") VALUES ");
            addSitter.Append("(@LastName,");
            addSitter.Append("@FirstName,");
            addSitter.Append("@RateId,");
            addSitter.Append("@Phone,");
            addSitter.Append("@SMSGateway,");
            addSitter.Append("@MMSGateway,");
            addSitter.Append("@DateModified);");
            try
            {
                using (var conn = _conn)
                {
                    conn.Open();
                    return conn.Execute(addSitter.ToString(), sitter);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int DeleteSitter(int id)
        {
            try
            {
                using (var conn = _conn)
                {
                    conn.Open();
                    return conn.Execute("DELETE FROM `TIME_CARD`.`SITTERS` WHERE Id = @id", new { id });
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int UpdateSitter(Sitter sitter)
        {
            sitter.DateModified = DateTime.UtcNow;
            StringBuilder updateSitter = new StringBuilder();
            updateSitter.Append("UPDATE `TIME_CARD`.`SITTERS` SET ");
            updateSitter.Append($"`DateModified` = @DateModified");
            if (!string.IsNullOrEmpty(sitter.LastName)) { updateSitter.Append(",`LastName` = @LastName"); }
            if (!string.IsNullOrEmpty(sitter.FirstName)) { updateSitter.Append(",`FirstName` = @FirstName"); }
            if (sitter.RateId != 0) { updateSitter.Append(",`RateId` = @RateId"); }
            if (!String.IsNullOrEmpty(sitter.Phone)) { updateSitter.Append(",`Phone` = @Phone"); }
            if (!String.IsNullOrEmpty(sitter.SMSGateway)) { updateSitter.Append(",`SMSGateway` = @SMSGateway"); }
            if (!String.IsNullOrEmpty(sitter.MMSGateway)) { updateSitter.Append(",`MMSGateway` = @MMSGateway"); }
            updateSitter.Append(" ");
            updateSitter.Append("WHERE Id = @Id;");
            try
            {
                using (var conn = _conn)
                {
                    conn.Open();
                    return conn.Execute(updateSitter.ToString(), sitter);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public SittersRoot GetSitter(int id)
        {
            try
            {
                using (var conn = _conn)
                {
                    conn.Open();
                    return new SittersRoot() { Sitter = conn.QueryFirst<Sitter>($"{SelectSql} WHERE `S`.`Id` = @Id;", new { id }) };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public Task<IEnumerable<Sitter>> GetSitters()
        public async Task<SittersRootCollectionJson> GetSitters()
        {
            using (var conn = _conn)
            {
                conn.Open();
                var results = await conn.QueryAsync<SitterJson>($"{SelectSql} ORDER BY `S`.`LastName`;");
                //var t = Task<SittersRoot>.Run(() => new SittersRoot() { Sitters = x });
                return new SittersRootCollectionJson() { SitterJson = results };
            }
        }
    }
}