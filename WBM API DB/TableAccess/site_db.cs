using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> Site_Insert(wbm_common.DataObjects.Site.dbRow Site)
        {
            try
            {
                await _dbContext.Site.AddAsync(Site);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Site.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Site_Insert - " + Site.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Site_Update(wbm_common.DataObjects.Site.dbRow Site)
        {
            try
            {
                _dbContext.Site.Attach(Site);
                _dbContext.Entry(Site).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Site.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Site_Update - " + Site.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Site.dbRow>> Site_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Site.dbRow Site = await _dbContext.Site.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Site == null)
                    return Result.Failure<wbm_common.DataObjects.Site.dbRow>("Site not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Site.dbRow>(Site);

            }
            catch (Exception ex)
            {
                logger.Error("Site_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Site.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.Site.dbRow>>> SiteList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Site.dbRow Site = await _dbContext.Site.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Site == null)
                    return Result.Success<List<wbm_common.DataObjects.Site.dbRow>>(new List<wbm_common.DataObjects.Site.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Site.dbRow>>(new List<wbm_common.DataObjects.Site.dbRow> { Site });
            }
            catch (Exception ex)
            {
                logger.Error("Site_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Site.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Site.dbRow>> Site_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Site.dbRow Site = await _dbContext.Site.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (Site == null)
                    return Result.Failure<wbm_common.DataObjects.Site.dbRow>("Site not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Site.dbRow>(Site);
            }
            catch (Exception ex)
            {
                logger.Error("Site_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Site.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Site.dbRow>>> Site_List()
        {
            try
            {
                List<wbm_common.DataObjects.Site.dbRow> Sites = await _dbContext.Site.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Site.dbRow>>(Sites);
            }
            catch (Exception ex)
            {
                logger.Error("Site_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Site.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Site.dbRow>>> Site_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Site_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Site.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Site.dbRow> Sites = await _dbContext.Site.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Site.dbRow>>(Sites);
                }
                catch (Exception ex)
                {
                    logger.Error("Site_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Site.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Site.dbRow>>> Site_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Site.dbRow> Sites = await _dbContext.Site.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Site.dbRow>>(Sites);
            }
            catch (Exception ex)
            {
                logger.Error("Site_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Site.dbRow>>("Internal Exception occured");
            }
        }
    }
}
