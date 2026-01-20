using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> PaperlessDataType_Insert(wbm_common.DataObjects.PaperlessDataType.dbRow PaperlessDataType)
        {
            try
            {
                await _dbContext.PaperlessDataType.AddAsync(PaperlessDataType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PaperlessDataType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessDataType_Insert - " + PaperlessDataType.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> PaperlessDataType_Update(wbm_common.DataObjects.PaperlessDataType.dbRow PaperlessDataType)
        {
            try
            {
                _dbContext.PaperlessDataType.Attach(PaperlessDataType);
                _dbContext.Entry(PaperlessDataType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PaperlessDataType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessDataType_Update - " + PaperlessDataType.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.PaperlessDataType.dbRow>> PaperlessDataType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PaperlessDataType.dbRow PaperlessDataType = await _dbContext.PaperlessDataType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (PaperlessDataType == null)
                    return Result.Failure<wbm_common.DataObjects.PaperlessDataType.dbRow>("PaperlessDataType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.PaperlessDataType.dbRow>(PaperlessDataType);

            }
            catch (Exception ex)
            {
                logger.Error("PaperlessDataType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PaperlessDataType.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>> PaperlessDataTypeList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PaperlessDataType.dbRow PaperlessDataType = await _dbContext.PaperlessDataType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (PaperlessDataType == null)
                    return Result.Success<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>(new List<wbm_common.DataObjects.PaperlessDataType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>(new List<wbm_common.DataObjects.PaperlessDataType.dbRow> { PaperlessDataType });
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessDataTypeList_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.PaperlessDataType.dbRow>> PaperlessDataType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PaperlessDataType.dbRow PaperlessDataType = await _dbContext.PaperlessDataType.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (PaperlessDataType == null)
                    return Result.Failure<wbm_common.DataObjects.PaperlessDataType.dbRow>("PaperlessDataType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.PaperlessDataType.dbRow>(PaperlessDataType);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessDataType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PaperlessDataType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>> PaperlessDataType_List()
        {
            try
            {
                List<wbm_common.DataObjects.PaperlessDataType.dbRow> PaperlessDataTypes = await _dbContext.PaperlessDataType.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>(PaperlessDataTypes);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessDataType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>> PaperlessDataType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("PaperlessDataType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.PaperlessDataType.dbRow> PaperlessDataTypes = await _dbContext.PaperlessDataType.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>(PaperlessDataTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("PaperlessDataType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>> PaperlessDataType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.PaperlessDataType.dbRow> PaperlessDataTypes = await _dbContext.PaperlessDataType.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>(PaperlessDataTypes);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessDataType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PaperlessDataType.dbRow>>("Internal Exception occured");
            }
        }
    }
}
