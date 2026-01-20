using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WBM_API.Models;


namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public bool Log_APITransactionToDB(ref wbm_common.DataObjects.Log.dbRow log)
        {
            try
            {
                //_dbContext.Log.Add(log);
                _dbContext.Log.Update(log);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Error persisting APITransactionLog to DB(Log_APITransactionToDB) " + ex.Message, ex);
            }

            return false;
        }

        public bool APICallStats_Update(string APIEndpoint, DateTime callStart)
        {
            try
            {
                wbm_common.DataObjects.APICallStats.dbRow CSrow = _dbContext.APICallStats.FirstOrDefault(x => x.APIEndPoint == APIEndpoint && x.Date.Date == callStart.Date);
                if (CSrow == null)
                {
                    CSrow = new wbm_common.DataObjects.APICallStats.dbRow
                    {
                        APIEndPoint = APIEndpoint,
                        Date = callStart.Date,
                        NumberOfCalls = 1,
                        TotalCallLength = (int)(DateTime.Now - callStart).TotalMilliseconds
                    };
                    _dbContext.APICallStats.Add(CSrow);
                }
                else
                {
                    CSrow.NumberOfCalls++;
                    CSrow.TotalCallLength += (int)(DateTime.Now - callStart).TotalMilliseconds;
                    _dbContext.APICallStats.Update(CSrow);
                }
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Error persisting APITransactionLog to DB(APICallStats_Update) " + ex.Message, ex);
            }

            return false;
        }
        public async Task<Result<List<wbm_common.DataObjects.Log.dbRow>>> Log_List()
        {
            try
            {
                List<wbm_common.DataObjects.Log.dbRow> Logs = await _dbContext.Log.AsNoTracking().ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Log.dbRow>>(Logs);
            }
            catch (Exception ex)
            {
                logger.Error("Log_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Log.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Log.dbRow>>> Log_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Log_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Log.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Log.dbRow> Logs = await _dbContext.Log.AsNoTracking().Where(x => IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Log.dbRow>>(Logs);
                }
                catch (Exception ex)
                {
                    logger.Error("Log_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Log.dbRow>>("Internal Exception occured");
                }
            }
        }

    }
}