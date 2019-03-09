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
    public class PayRepository : IPayRepository
    {
        private readonly IDbConnection _conn;

        public PayRepository(IDbConnection conn)
        {
            _conn = conn;
        }
        private const String SelectSql = "USE TIME_CARD; SELECT `P`.`Id`,`P`.`Pay` AS `PayAmt`, `P`.`Hours`, `P`.`PeriodStart`, `P`.`PeriodEnd`, `P`.`CheckDate`, `P`.`CheckNumber`, `P`.`SitterId`, `C`.`Comment`, `P`.`DateModified` FROM `TIME_CARD`.`PAY` AS P LEFT JOIN `TIME_CARD`.`COMMENTS` AS C ON `P`.`Id` = `C`.`PayId`";

        public int AddPay(Pay pay)
        {
            pay.DateModified = DateTime.UtcNow;
            StringBuilder addPay = new StringBuilder();
            addPay.Append("INSERT INTO `TIME_CARD`.`PAY` ");
            addPay.Append("(`Pay`,");
            addPay.Append("`Hours`,");
            addPay.Append("`PeriodStart`,");
            addPay.Append("`PeriodEnd`,");
            addPay.Append("`CheckDate`,");
            addPay.Append("`CheckNumber`,");
            addPay.Append("`SitterId`,");
            addPay.Append("`DateModified`");
            addPay.Append(") VALUES ");
            addPay.Append("(@PayAmt,");
            addPay.Append("@Hours,");
            addPay.Append("@PeriodStart,");
            addPay.Append("@PeriodEnd,");
            addPay.Append("@CheckDate,");
            addPay.Append("@CheckNumber,");
            addPay.Append("@SitterId,");
            addPay.Append("@DateModified);");
            using (var conn = _conn)
            {
                conn.Open();
                return conn.Execute(addPay.ToString(), pay);
            }
        }
        public int DeletePay(int id)
        {
            using (var conn = _conn)
            {
                conn.Open();
                return conn.Execute("DELETE FROM `TIME_CARD`.`PAY` WHERE Id = @id", new { id });
            }
        }
        public int UpdatePay(Pay pay)
        {
            pay.DateModified = DateTime.UtcNow;
            StringBuilder updatePay = new StringBuilder();
            updatePay.Append("UPDATE `TIME_CARD`.`PAY` SET ");
            updatePay.Append("`DateModified` = @DateModified");
            if (pay.PayAmt > 0) { updatePay.Append(",`Pay` = @PayAmt"); }
            if (pay.Hours > 0) { updatePay.Append(", `Hours` = @Hours"); }
            if (pay.CheckDate.CompareTo(DateTime.MinValue) > 0) { updatePay.Append(",`CheckDate` = @CheckDate"); }
            if (pay.PeriodStart.CompareTo(DateTime.MinValue) > 0) { updatePay.Append(",`PeriodStart` = @PeriodStart"); }
            if (pay.PeriodEnd.CompareTo(DateTime.MinValue) > 0) { updatePay.Append(",`PeriodEnd` = @PeriodEnd"); }
            if (!String.IsNullOrEmpty(pay.CheckNumber)) { updatePay.Append(",`CheckNumber` = @CheckNumber"); }
            if (pay.SitterId > 0) { updatePay.Append(",`SitterId` = @SitterId"); }
            updatePay.Append(" ");
            updatePay.Append("WHERE Id = @Id;");
            using (var conn = _conn)
            {
                conn.Open();
                return conn.Execute(updatePay.ToString(), pay);
            }
        }
        public PayRoot GetPay(int id)
        {
            using (var conn = _conn)
            {
                conn.Open();
                return new PayRoot() { Pay = conn.QueryFirst<Pay>($"{SelectSql} WHERE `P`.`Id` = @Id;", new { id }) };
            }
        }
        public PayRootJson GetPayString(int id)
        {
            using (var conn = _conn)
            {
                conn.Open();
                return new PayRootJson() { PayJson = conn.QueryFirst<PayJson>($"{SelectSql} WHERE `P`.`Id` = @Id;", new { id }) };
            }
        }
        //public async Task<PayRootCollection> GetPays()
        public async Task<PayRootCollectionJson> GetPays()
        {
            using (var conn = _conn)
            {
                conn.Open();
                var results = await conn.QueryAsync<PayJson>($"{SelectSql} ORDER BY YEAR(`P`.`CheckDate`) DESC, `P`.`CheckNumber` DESC;");
                //var t = Task<PunchesRoot>.Run(() => new PunchesRoot() { Punchs = x });
                return new PayRootCollectionJson() { PayJson = results };
            }
        }
        // Get Pays by SitterId
        public async Task<PayRootCollectionJson> GetPays(int id)
        {
            using (var conn = _conn)
            {
                conn.Open();
                var results = await conn.QueryAsync<PayJson>($"{SelectSql} WHERE `P`.`SitterId` = @Id ORDER BY YEAR(`P`.`CheckNumber`), `P`.`CheckNumber` DESC;", new { id });
                //var t = Task<PunchesRoot>.Run(() => new PunchesRoot() { Punchs = x });
                return new PayRootCollectionJson() { PayJson = results };
            }
        }
        public async Task<PayRootCollectionJson> GetPays(int id, DateTime date)
        {
            using (var conn = _conn)
            {
                conn.Open();
                var results = await conn.QueryAsync<PayJson>($"{SelectSql} WHERE `P`.`SitterId` = @Id AND `P`.`CheckDate` >= @Date ORDER BY `P`.`CheckDate` ASC;", new { id, date });
                //var t = Task<PunchesRoot>.Run(() => new PunchesRoot() { Punchs = x });
                return new PayRootCollectionJson() { PayJson = results };
            }
        }
    }
}