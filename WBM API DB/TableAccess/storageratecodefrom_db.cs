using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> StorageRateCodeFrom_Insert(wbm_common.DataObjects.StorageRateCodeFrom.dbRow StorageRateCodeFrom)
        {
            try
            {
                await _dbContext.StorageRateCodeFrom.AddAsync(StorageRateCodeFrom);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StorageRateCodeFrom.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StorageRateCodeFrom_Insert - " + StorageRateCodeFrom.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> StorageRateCodeFrom_Update(wbm_common.DataObjects.StorageRateCodeFrom.dbRow StorageRateCodeFrom)
        {
            try
            {
                _dbContext.StorageRateCodeFrom.Attach(StorageRateCodeFrom);
                _dbContext.Entry(StorageRateCodeFrom).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StorageRateCodeFrom.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StorageRateCodeFrom_Update - " + StorageRateCodeFrom.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>> StorageRateCodeFrom_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StorageRateCodeFrom.dbRow StorageRateCodeFrom = await _dbContext.StorageRateCodeFrom.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (StorageRateCodeFrom == null)
                    return Result.Failure<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>("StorageRateCodeFrom not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>(StorageRateCodeFrom);

            }
            catch (Exception ex)
            {
                logger.Error("StorageRateCodeFrom_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>> StorageRateCodeFromList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StorageRateCodeFrom.dbRow StorageRateCodeFrom = await _dbContext.StorageRateCodeFrom.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (StorageRateCodeFrom == null)
                    return Result.Success<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>(new List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>(new List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow> { StorageRateCodeFrom });
            }
            catch (Exception ex)
            {
                logger.Error("StorageRateCodeFromList_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>> StorageRateCodeFrom_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StorageRateCodeFrom.dbRow StorageRateCodeFrom = await _dbContext.StorageRateCodeFrom.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (StorageRateCodeFrom == null)
                    return Result.Failure<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>("StorageRateCodeFrom not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>(StorageRateCodeFrom);
            }
            catch (Exception ex)
            {
                logger.Error("StorageRateCodeFrom_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>> StorageRateCodeFrom_List()
        {
            try
            {
                List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow> StorageRateCodeFroms = await _dbContext.StorageRateCodeFrom.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>(StorageRateCodeFroms);
            }
            catch (Exception ex)
            {
                logger.Error("StorageRateCodeFrom_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>> StorageRateCodeFrom_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("StorageRateCodeFrom_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow> StorageRateCodeFroms = await _dbContext.StorageRateCodeFrom.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>(StorageRateCodeFroms);
                }
                catch (Exception ex)
                {
                    logger.Error("StorageRateCodeFrom_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>> StorageRateCodeFrom_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow> StorageRateCodeFroms = await _dbContext.StorageRateCodeFrom.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>(StorageRateCodeFroms);
            }
            catch (Exception ex)
            {
                logger.Error("StorageRateCodeFrom_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>>("Internal Exception occured");
            }
        }
    }
}
