using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> PalletStorageType_Insert(wbm_common.DataObjects.PalletStorageType.dbRow PalletStorageType)
        {
            try
            {
                await _dbContext.PalletStorageType.AddAsync(PalletStorageType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PalletStorageType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PalletStorageType_Insert - " + PalletStorageType.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> PalletStorageType_Update(wbm_common.DataObjects.PalletStorageType.dbRow PalletStorageType)
        {
            try
            {
                _dbContext.PalletStorageType.Attach(PalletStorageType);
                _dbContext.Entry(PalletStorageType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PalletStorageType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PalletStorageType_Update - " + PalletStorageType.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.PalletStorageType.dbRow>> PalletStorageType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PalletStorageType.dbRow PalletStorageType = await _dbContext.PalletStorageType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (PalletStorageType == null)
                    return Result.Failure<wbm_common.DataObjects.PalletStorageType.dbRow>("PalletStorageType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.PalletStorageType.dbRow>(PalletStorageType);

            }
            catch (Exception ex)
            {
                logger.Error("PalletStorageType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PalletStorageType.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.PalletStorageType.dbRow>>> PalletStorageTypeList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PalletStorageType.dbRow PalletStorageType = await _dbContext.PalletStorageType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (PalletStorageType == null)
                    return Result.Success<List<wbm_common.DataObjects.PalletStorageType.dbRow>>(new List<wbm_common.DataObjects.PalletStorageType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PalletStorageType.dbRow>>(new List<wbm_common.DataObjects.PalletStorageType.dbRow> { PalletStorageType });
            }
            catch (Exception ex)
            {
                logger.Error("PalletStorageTypeList_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PalletStorageType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.PalletStorageType.dbRow>> PalletStorageType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PalletStorageType.dbRow PalletStorageType = await _dbContext.PalletStorageType.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (PalletStorageType == null)
                    return Result.Failure<wbm_common.DataObjects.PalletStorageType.dbRow>("PalletStorageType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.PalletStorageType.dbRow>(PalletStorageType);
            }
            catch (Exception ex)
            {
                logger.Error("PalletStorageType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PalletStorageType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PalletStorageType.dbRow>>> PalletStorageType_List()
        {
            try
            {
                List<wbm_common.DataObjects.PalletStorageType.dbRow> PalletStorageTypes = await _dbContext.PalletStorageType.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.PalletStorageType.dbRow>>(PalletStorageTypes);
            }
            catch (Exception ex)
            {
                logger.Error("PalletStorageType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.PalletStorageType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PalletStorageType.dbRow>>> PalletStorageType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("PalletStorageType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.PalletStorageType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.PalletStorageType.dbRow> PalletStorageTypes = await _dbContext.PalletStorageType.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.PalletStorageType.dbRow>>(PalletStorageTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("PalletStorageType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.PalletStorageType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PalletStorageType.dbRow>>> PalletStorageType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.PalletStorageType.dbRow> PalletStorageTypes = await _dbContext.PalletStorageType.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.PalletStorageType.dbRow>>(PalletStorageTypes);
            }
            catch (Exception ex)
            {
                logger.Error("PalletStorageType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PalletStorageType.dbRow>>("Internal Exception occured");
            }
        }
    }
}
