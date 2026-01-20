using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<string> ApiTransActionReferenceNumber_Get(int id)
        {
            try
            {
                DateTime dateNow = DateTime.Now.Date;

                var sqlparams = new SqlParameter[3]
                {
                    new SqlParameter("@returnVal", SqlDbType.Int) { Direction = ParameterDirection.Output },
                    new SqlParameter("@CounterDefnID", SqlDbType.Int) { Direction = ParameterDirection.Input, Value = id },
                    new SqlParameter("@Date", SqlDbType.DateTime) { Direction = ParameterDirection.Input, Value = dateNow }
                };
                int result = await _dbContext.Database.ExecuteSqlRawAsync("EXEC @returnVal=" + "dbo.DateBasedCounters_GetCounter @CounterDefnID, @Date", sqlparams);

                return string.Format("{0:yyyyMMddHHmmss}{1}", dateNow, sqlparams[0].Value.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("ApiTransActionReferenceNumber_Get: " + ex);
                return null;
            }
        }
    }
}