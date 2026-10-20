using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using wbm_common.NonDatabaseObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> Counters_GetCounter(int CounterID)
        {
            try
            {
                var sqlparams = new SqlParameter[2]
                {
                    new SqlParameter("@returnVal", SqlDbType.Int) { Direction = ParameterDirection.Output },
                    new SqlParameter("@CounterDefnID", SqlDbType.Int) { Direction = ParameterDirection.Input, Value = CounterID }
                };
                int result = await _dbContext.Database.ExecuteSqlRawAsync("EXEC @returnVal=" + "dbo.Counters_GetCounter @CounterDefnID", sqlparams);

                return Result.Success<int>((int)sqlparams[0].Value);
            }
            catch (Exception ex)
            {
                logger.Error("Counters.GetCounter: " + ex);
                return Result.Exception<int>(ex.ToString());
            }
        }
    }
}