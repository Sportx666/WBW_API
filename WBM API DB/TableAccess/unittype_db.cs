using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> UnitType_Insert(wbm_common.DataObjects.UnitType.dbRow UnitType)
        {
            try
            {
                await _dbContext.UnitType.AddAsync(UnitType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(UnitType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("UnitType_Insert - " + UnitType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> UnitType_Update(wbm_common.DataObjects.UnitType.dbRow UnitType)
        {
            try
            {
                _dbContext.UnitType.Attach(UnitType);
                _dbContext.Entry(UnitType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(UnitType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("UnitType_Update - " + UnitType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.UnitType.dbRow>> UnitType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.UnitType.dbRow UnitType = await _dbContext.UnitType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (UnitType == null)
                    return Result.Failure<wbm_common.DataObjects.UnitType.dbRow>("UnitType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.UnitType.dbRow>(UnitType);

            }
            catch (Exception ex)
            {
                logger.Error("UnitType_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.UnitType.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.UnitType.dbRow>>> UnitTypeList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.UnitType.dbRow UnitType = await _dbContext.UnitType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (UnitType == null)
                    return Result.Success<List<wbm_common.DataObjects.UnitType.dbRow>>(new List<wbm_common.DataObjects.UnitType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.UnitType.dbRow>>(new List<wbm_common.DataObjects.UnitType.dbRow> { UnitType });
            }
            catch (Exception ex)
            {
                logger.Error("UnitType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.UnitType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.UnitType.dbRow>> UnitType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.UnitType.dbRow UnitType = await _dbContext.UnitType.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (UnitType == null)
                    return Result.Failure<wbm_common.DataObjects.UnitType.dbRow>("UnitType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.UnitType.dbRow>(UnitType);
            }
            catch (Exception ex)
            {
                logger.Error("UnitType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.UnitType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.UnitType.dbRow>>> UnitType_List()
        {
            try
            {
                List<wbm_common.DataObjects.UnitType.dbRow> UnitTypes = await _dbContext.UnitType.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.UnitType.dbRow>>(UnitTypes);
            }
            catch (Exception ex)
            {
                logger.Error("UnitType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.UnitType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.UnitType.dbRow>>> UnitType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("UnitType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.UnitType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.UnitType.dbRow> UnitTypes = await _dbContext.UnitType.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.UnitType.dbRow>>(UnitTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("UnitType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.UnitType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.UnitType.dbRow>>> UnitType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.UnitType.dbRow> UnitTypes = await _dbContext.UnitType.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.UnitType.dbRow>>(UnitTypes);
            }
            catch (Exception ex)
            {
                logger.Error("UnitType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.UnitType.dbRow>>("Internal Exception occured");
            }
        }
    }
}
