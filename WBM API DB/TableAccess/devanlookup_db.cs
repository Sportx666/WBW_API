using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> DevanLookup_Insert(wbm_common.DataObjects.DevanLookup.dbRow DevanLookup)
        {
            try
            {
                await _dbContext.DevanLookup.AddAsync(DevanLookup);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(DevanLookup.ID);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_Insert - " + DevanLookup.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> DevanLookup_Update(wbm_common.DataObjects.DevanLookup.dbRow DevanLookup)
        {
            try
            {
                _dbContext.DevanLookup.Attach(DevanLookup);
                _dbContext.Entry(DevanLookup).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(DevanLookup.ID);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_Update - " + DevanLookup.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.DevanLookup.dbRow>> DevanLookup_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.DevanLookup.dbRow DevanLookup = await _dbContext.DevanLookup.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (DevanLookup == null)
                    return Result.Failure<wbm_common.DataObjects.DevanLookup.dbRow>("DevanLookup not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.DevanLookup.dbRow>(DevanLookup);

            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.DevanLookup.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.DevanLookup.dbRow>>> DevanLookupList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.DevanLookup.dbRow DevanLookup = await _dbContext.DevanLookup.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (DevanLookup == null)
                    return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRow>>(new List<wbm_common.DataObjects.DevanLookup.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRow>>(new List<wbm_common.DataObjects.DevanLookup.dbRow> { DevanLookup });
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.DevanLookup.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.DevanLookup.dbRow>> DevanLookup_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.DevanLookup.dbRow DevanLookup = await _dbContext.DevanLookup.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (DevanLookup == null)
                    return Result.Failure<wbm_common.DataObjects.DevanLookup.dbRow>("DevanLookup not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.DevanLookup.dbRow>(DevanLookup);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.DevanLookup.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.DevanLookup.dbRow>>> DevanLookup_List()
        {
            try
            {
                List<wbm_common.DataObjects.DevanLookup.dbRow> DevanLookups = await _dbContext.DevanLookup.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRow>>(DevanLookups);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.DevanLookup.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.DevanLookup.dbRow>>> DevanLookup_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("DevanLookup_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.DevanLookup.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.DevanLookup.dbRow> DevanLookups = await _dbContext.DevanLookup.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRow>>(DevanLookups);
                }
                catch (Exception ex)
                {
                    logger.Error("DevanLookup_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.DevanLookup.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.DevanLookup.dbRow>>> DevanLookup_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.DevanLookup.dbRow> DevanLookups = await _dbContext.DevanLookup.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRow>>(DevanLookups);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.DevanLookup.dbRow>>("Internal Exception occured");
            }
        }
    }
}
