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
    public class PunchRepository : IPunchRepository
    {
        private readonly IDbConnection _conn;
        private const String SelectSql = "USE TIME_CARD; SELECT `P`.`Id`, `P`.`Punch` AS `TimePunch`, UNIX_TIMESTAMP(`P`.`Punch`) AS `TimePunchSeconds`, `P`.`SitterId`, `C`.`Comment`, `P`.`DateModified` FROM `TIME_CARD`.`PUNCH` AS P LEFT JOIN `TIME_CARD`.`COMMENTS` AS C ON DATE(`P`.`PUNCH`) = `C`.`Id` AND `P`.`SitterId` = `C`.`SitterId`";
        public PunchRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public int AddPunch(Punch punch)
        {
            punch.DateModified = DateTime.UtcNow;
            punch.TimePunch = punch.TimePunch.ToUniversalTime();
            var commentDate = DateTime.Parse((punch.TimePunch).ToString("yyyy-MM-dd"));
            StringBuilder addPunch = new StringBuilder();
            addPunch.Append("INSERT INTO `TIME_CARD`.`PUNCH` ");
            addPunch.Append("(`Punch`,");
            addPunch.Append("`SitterId`,");
            addPunch.Append("`DateModified`");
            addPunch.Append(") VALUES ");
            addPunch.Append("(@TimePunch,");
            addPunch.Append("@SitterId,");
            addPunch.Append("@DateModified);");
            using (var conn = _conn)
            {
                conn.Open();
                var result = conn.Execute(addPunch.ToString(), punch);
                if (result > 0 && !String.IsNullOrEmpty(punch.Comment))
                {
                    Comments comment = new Comments()
                    {
                        Id = DateTime.Parse((punch.TimePunch).ToString("yyyy-MM-dd")),
                        Comment = punch.Comment,
                        SitterId = punch.SitterId,
                        PayId = 0,
                        DateModified = punch.DateModified = DateTime.UtcNow
                    };
                    AddUpdateComment(comment, conn);
                }
                return result;
            }
        }
        int AddUpdateComment(Comments comment, IDbConnection conn)
        {
            var result = conn.Execute("DELETE FROM `TIME_CARD`.`COMMENTS` WHERE `Id` = @Id AND `SitterId` = @SitterId", comment);
            result = conn.Execute("INSERT INTO `TIME_CARD`.`COMMENTS` (`Id`,`Comment`,`SitterId`,`PayId`,`DateModified`) VALUES (@Id,@Comment,@SitterId,@PayId,@DateModified)", comment);
            return result;
        }
        public int DeletePunch(int id)
        {
            var punchRootJson = GetPunch(id);
            var punch = punchRootJson.PunchJson;
            using (var conn = _conn)
            {
                conn.Open();
                var result = conn.Execute("DELETE FROM `TIME_CARD`.`PUNCH` WHERE Id = @id", new { id });
                if (result > 0 && punch != null)
                {
                    Comments comment = new Comments()
                    {
                        Id = DateTime.Parse((DateTime.Parse(punch.TimePunch).ToString("yyyy-MM-dd"))),
                        SitterId = Convert.ToInt32(punch.SitterId)
                    };
                    DeleteComment(comment, conn);
                }
                return result;
            }
        }
        int DeleteComment(Comments comment, IDbConnection conn)
        {
            var results = conn.Query<PunchJson>($"{SelectSql} WHERE DATE(`P`.`Punch`) = DATE(@Id) AND `P`.`SitterId` = @SitterId ORDER BY `P`.`Punch` DESC;", comment);
            if (!results.Any())
            {
                return conn.Execute("DELETE FROM `TIME_CARD`.`COMMENTS` WHERE `Id` = @Id AND `SitterId` = @SitterId", comment);
            }
            return 0;
        }
        public int UpdatePunch(Punch punch)
        {
            punch.DateModified = DateTime.UtcNow;
            punch.TimePunch = punch.TimePunch.ToUniversalTime();
            StringBuilder updatePunch = new StringBuilder();
            updatePunch.Append("UPDATE `TIME_CARD`.`PUNCH` SET ");
            updatePunch.Append("`DateModified` = @DateModified");
            if (punch.TimePunch.CompareTo(DateTime.MinValue) > 0) { updatePunch.Append(",`Punch` = @TimePunch"); }
            if (punch.SitterId > 0) { updatePunch.Append(",`SitterId` = @SitterId"); }
            updatePunch.Append(" ");
            updatePunch.Append("WHERE Id = @Id;");
            using (var conn = _conn)
            {
                conn.Open();
                var result = conn.Execute(updatePunch.ToString(), punch);
                if (result > 0 && !String.IsNullOrEmpty(punch.Comment))
                {
                    Comments comment = new Comments()
                    {
                        Id = DateTime.Parse((punch.TimePunch).ToString("yyyy-MM-dd")),
                        Comment = punch.Comment,
                        SitterId = punch.SitterId,
                        PayId = 0,
                        DateModified = punch.DateModified = DateTime.UtcNow
                    };
                    AddUpdateComment(comment, conn);
                }
                return result;
            }
        }
        public PunchRootJson GetPunch(int id)
        {
            using (var conn = _conn)
            {
                conn.Open();
                return new PunchRootJson() { PunchJson = conn.QueryFirst<PunchJson>($"{SelectSql} WHERE `P`.`Id` = @Id;", new { id }) };
            }
        }
        //public Task<IEnumerable<Punch>> GetPunches()
        public async Task<PunchRootCollection> GetPunches()
        {
            using (var conn = _conn)
            {
                conn.Open();
                var results = await conn.QueryAsync<Punch>($"{SelectSql} ORDER BY `P`.`Punch` DESC;");
                //var t = Task<PunchesRoot>.Run(() => new PunchesRoot() { Punchs = x });
                return new PunchRootCollection() { Punches = results };
            }
        }
        public async Task<PunchRootCollectionJson> GetPunches(int id)
        {
            using (var conn = _conn)
            {
                conn.Open();
                var results = await conn.QueryAsync<PunchJson>($"{SelectSql} WHERE `P`.`SitterId` = @Id ORDER BY `P`.`Punch` DESC;", new { id });
                //var t = Task<PunchesRoot>.Run(() => new PunchesRoot() { Punchs = x });
                return new PunchRootCollectionJson() { PunchJson = results };
            }
        }
        public async Task<PunchRootCollectionJson> GetPunches(int id, DateTime periodStart, DateTime periodEnd)
        {
            using (var conn = _conn)
            {
                //periodStart = periodStart.Date;
                //periodEnd = periodEnd.Date;
                //var ts = new TimeSpan(23,59,59);
                //periodEnd = periodEnd + ts;
                //ts = new TimeSpan(6,0,0);
                //periodStart = periodStart + ts;
                periodStart = periodStart.ToUniversalTime();
                periodEnd = periodEnd.ToUniversalTime();
                conn.Open();
                var results = await conn.QueryAsync<PunchJson>($"{SelectSql} WHERE `P`.`SitterId` = @Id AND `P`.`Punch` >= @PeriodStart AND `P`.`PUNCH` <= @PeriodEnd ORDER BY `P`.`Punch` DESC;", new { id, periodStart, periodEnd });
                //var t = Task<PunchesRoot>.Run(() => new PunchesRoot() { Punchs = x });
                return new PunchRootCollectionJson() { PunchJson = results };
            }
        }
    }
}