using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> ZoneProcessing_Insert(wbm_common.DataObjects.ZoneProcessing.dbRow ZoneProcessing)
        {
            try
            {
                await _dbContext.ZoneProcessing.AddAsync(ZoneProcessing);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ZoneProcessing.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_Insert - " + ZoneProcessing.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ZoneProcessing_Update(wbm_common.DataObjects.ZoneProcessing.dbRow ZoneProcessing)
        {
            try
            {
                _dbContext.ZoneProcessing.Attach(ZoneProcessing);
                _dbContext.Entry(ZoneProcessing).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ZoneProcessing.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_Update - " + ZoneProcessing.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>> ZoneProcessing_ListBySingleCompanyID(int CompanyID)
        {
            try
            {
                wbm_common.DataObjects.ZoneProcessing.dbRow ZoneProcessing = await _dbContext.ZoneProcessing.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.CompanyID == CompanyID);

                if (ZoneProcessing == null)
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRow> { ZoneProcessing });
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ListBySingleCompanyID - " + CompanyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>> ZoneProcessing_ListBySingleSiteID(int SiteID)
        {
            try
            {
                wbm_common.DataObjects.ZoneProcessing.dbRow ZoneProcessing = await _dbContext.ZoneProcessing.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.SiteID == SiteID);

                if (ZoneProcessing == null)
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRow> { ZoneProcessing });
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ListBySingleSiteID - " + SiteID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ZoneProcessing.dbRow>> ZoneProcessing_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ZoneProcessing.dbRow ZoneProcessing = await _dbContext.ZoneProcessing.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ZoneProcessing == null)
                    return Result.Failure<wbm_common.DataObjects.ZoneProcessing.dbRow>("ZoneProcessing not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ZoneProcessing.dbRow>(ZoneProcessing);

            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ZoneProcessing.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>> ZoneProcessingList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ZoneProcessing.dbRow ZoneProcessing = await _dbContext.ZoneProcessing.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ZoneProcessing == null)
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRow> { ZoneProcessing });
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ZoneProcessing.dbRow>> ZoneProcessing_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ZoneProcessing.dbRow ZoneProcessing = await _dbContext.ZoneProcessing.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ZoneProcessing == null)
                    return Result.Failure<wbm_common.DataObjects.ZoneProcessing.dbRow>("ZoneProcessing not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ZoneProcessing.dbRow>(ZoneProcessing);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ZoneProcessing.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>> ZoneProcessing_List()
        {
            try
            {
                List<wbm_common.DataObjects.ZoneProcessing.dbRow> ZoneProcessings = await _dbContext.ZoneProcessing.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>(ZoneProcessings);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>> ZoneProcessing_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ZoneProcessing_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ZoneProcessing.dbRow> ZoneProcessings = await _dbContext.ZoneProcessing.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>(ZoneProcessings);
                }
                catch (Exception ex)
                {
                    logger.Error("ZoneProcessing_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>> ZoneProcessing_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ZoneProcessing.dbRow> ZoneProcessings = await _dbContext.ZoneProcessing.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>(ZoneProcessings);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRow>>("Internal Exception occured");
            }
        }
    }
}
