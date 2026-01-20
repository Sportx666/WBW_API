using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> ActionRateCode_Insert(wbm_common.DataObjects.ActionRateCode.dbRow ActionRateCode)
        {
            try
            {
                await _dbContext.ActionRateCode.AddAsync(ActionRateCode);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ActionRateCode.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_Insert - " + ActionRateCode.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ActionRateCode_Update(wbm_common.DataObjects.ActionRateCode.dbRow ActionRateCode)
        {
            try
            {
                _dbContext.ActionRateCode.Attach(ActionRateCode);
                _dbContext.Entry(ActionRateCode).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ActionRateCode.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_Update - " + ActionRateCode.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.ActionRateCode.dbRow>>> ActionRateCode_ListBySingleOwnerID(int OwnerID)
        {
            try
            {
                wbm_common.DataObjects.ActionRateCode.dbRow ActionRateCode = await _dbContext.ActionRateCode.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.OwnerID == OwnerID);

                if (ActionRateCode == null)
                    return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRow>>(new List<wbm_common.DataObjects.ActionRateCode.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRow>>(new List<wbm_common.DataObjects.ActionRateCode.dbRow> { ActionRateCode });
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_ListBySingleOwnerID - " + OwnerID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ActionRateCode.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ActionRateCode.dbRow>> ActionRateCode_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ActionRateCode.dbRow ActionRateCode = await _dbContext.ActionRateCode.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ActionRateCode == null)
                    return Result.Failure<wbm_common.DataObjects.ActionRateCode.dbRow>("ActionRateCode not found : " + ID);
                else
                    return Result.Success<wbm_common.DataObjects.ActionRateCode.dbRow>(ActionRateCode);

            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ActionRateCode.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ActionRateCode.dbRow>>> ActionRateCodeList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ActionRateCode.dbRow ActionRateCode = await _dbContext.ActionRateCode.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ActionRateCode == null)
                    return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRow>>(new List<wbm_common.DataObjects.ActionRateCode.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRow>>(new List<wbm_common.DataObjects.ActionRateCode.dbRow> { ActionRateCode });
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ActionRateCode.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ActionRateCode.dbRow>> ActionRateCode_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ActionRateCode.dbRow ActionRateCode = await _dbContext.ActionRateCode.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ActionRateCode == null)
                    return Result.Failure<wbm_common.DataObjects.ActionRateCode.dbRow>("ActionRateCode not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ActionRateCode.dbRow>(ActionRateCode);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ActionRateCode.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ActionRateCode.dbRow>>> ActionRateCode_List()
        {
            try
            {
                List<wbm_common.DataObjects.ActionRateCode.dbRow> ActionRateCodes = await _dbContext.ActionRateCode.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRow>>(ActionRateCodes);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ActionRateCode.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ActionRateCode.dbRow>>> ActionRateCode_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ActionRateCode.dbRow> ActionRateCodes = await _dbContext.ActionRateCode.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRow>>(ActionRateCodes);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ActionRateCode.dbRow>>("Internal Exception occured");
            }
        }
    }
}
