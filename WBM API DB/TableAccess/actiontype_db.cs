using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> ActionType_Insert(wbm_common.DataObjects.ActionType.dbRow ActionType)
        {
            try
            {
                await _dbContext.ActionType.AddAsync(ActionType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ActionType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ActionType_Insert - " + ActionType.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ActionType_Update(wbm_common.DataObjects.ActionType.dbRow ActionType)
        {
            try
            {
                _dbContext.ActionType.Attach(ActionType);
                _dbContext.Entry(ActionType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ActionType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ActionType_Update - " + ActionType.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.ActionType.dbRow>> ActionType_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ActionType.dbRow ActionType = await _dbContext.ActionType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (ActionType == null)
                    return Result.Failure<wbm_common.DataObjects.ActionType.dbRow>("ActionType not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ActionType.dbRow>(ActionType);

            }
            catch (Exception ex)
            {
                logger.Error("ActionType_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ActionType.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.ActionType.dbRow>>> ActionType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ActionType.dbRow ActionType = await _dbContext.ActionType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ActionType == null)
                    return Result.Success<List<wbm_common.DataObjects.ActionType.dbRow>>(new List<wbm_common.DataObjects.ActionType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ActionType.dbRow>>(new List<wbm_common.DataObjects.ActionType.dbRow> { ActionType });
            }
            catch (Exception ex)
            {
                logger.Error("ActionType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ActionType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ActionType.dbRow>> ActionType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ActionType.dbRow ActionType = await _dbContext.ActionType.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ActionType == null)
                    return Result.Failure<wbm_common.DataObjects.ActionType.dbRow>("ActionType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ActionType.dbRow>(ActionType);
            }
            catch (Exception ex)
            {
                logger.Error("ActionType_ReadByID - " + ID .ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ActionType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ActionType.dbRow>>> ActionType_List()
        {
            try
            {
                List<wbm_common.DataObjects.ActionType.dbRow> ActionTypes = await _dbContext.ActionType.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ActionType.dbRow>>(ActionTypes);
            }
            catch (Exception ex)
            {
                logger.Error("ActionType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ActionType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ActionType.dbRow>>> ActionType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ActionType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ActionType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ActionType.dbRow> ActionTypes = await _dbContext.ActionType.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ActionType.dbRow>>(ActionTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("ActionType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ActionType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ActionType.dbRow>>> ActionType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ActionType.dbRow> ActionTypes = await _dbContext.ActionType.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ActionType.dbRow>>(ActionTypes);
            }
            catch (Exception ex)
            {
                logger.Error("ActionType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ActionType.dbRow>>("Internal Exception occured");
            }
        }
    }
}
