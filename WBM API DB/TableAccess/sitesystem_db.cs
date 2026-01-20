using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> SiteSystem_Insert(wbm_common.DataObjects.SiteSystem.dbRow SiteSystem)
        {
            try
            {
                await _dbContext.SiteSystem.AddAsync(SiteSystem);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(SiteSystem.ID);
            }
            catch (Exception ex)
            {
                logger.Error("SiteSystem_Insert - " + SiteSystem.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> SiteSystem_Update(wbm_common.DataObjects.SiteSystem.dbRow SiteSystem)
        {
            try
            {
                _dbContext.SiteSystem.Attach(SiteSystem);
                _dbContext.Entry(SiteSystem).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(SiteSystem.ID);
            }
            catch (Exception ex)
            {
                logger.Error("SiteSystem_Update - " + SiteSystem.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.SiteSystem.dbRow>> SiteSystem_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.SiteSystem.dbRow SiteSystem = await _dbContext.SiteSystem.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (SiteSystem == null)
                    return Result.Failure<wbm_common.DataObjects.SiteSystem.dbRow>("SiteSystem not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.SiteSystem.dbRow>(SiteSystem);

            }
            catch (Exception ex)
            {
                logger.Error("SiteSystem_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.SiteSystem.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.SiteSystem.dbRow>>> SiteSystemList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.SiteSystem.dbRow SiteSystem = await _dbContext.SiteSystem.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (SiteSystem == null)
                    return Result.Success<List<wbm_common.DataObjects.SiteSystem.dbRow>>(new List<wbm_common.DataObjects.SiteSystem.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.SiteSystem.dbRow>>(new List<wbm_common.DataObjects.SiteSystem.dbRow> { SiteSystem });
            }
            catch (Exception ex)
            {
                logger.Error("SiteSystem_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.SiteSystem.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.SiteSystem.dbRow>> SiteSystem_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.SiteSystem.dbRow SiteSystem = await _dbContext.SiteSystem.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (SiteSystem == null)
                    return Result.Failure<wbm_common.DataObjects.SiteSystem.dbRow>("SiteSystem not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.SiteSystem.dbRow>(SiteSystem);
            }
            catch (Exception ex)
            {
                logger.Error("SiteSystem_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.SiteSystem.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.SiteSystem.dbRow>>> SiteSystem_List()
        {
            try
            {
                List<wbm_common.DataObjects.SiteSystem.dbRow> SiteSystems = await _dbContext.SiteSystem.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.SiteSystem.dbRow>>(SiteSystems);
            }
            catch (Exception ex)
            {
                logger.Error("SiteSystem_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.SiteSystem.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.SiteSystem.dbRow>>> SiteSystem_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("SiteSystem_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.SiteSystem.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.SiteSystem.dbRow> SiteSystems = await _dbContext.SiteSystem.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.SiteSystem.dbRow>>(SiteSystems);
                }
                catch (Exception ex)
                {
                    logger.Error("SiteSystem_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.SiteSystem.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.SiteSystem.dbRow>>> SiteSystem_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.SiteSystem.dbRow> SiteSystems = await _dbContext.SiteSystem.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.SiteSystem.dbRow>>(SiteSystems);
            }
            catch (Exception ex)
            {
                logger.Error("SiteSystem_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.SiteSystem.dbRow>>("Internal Exception occured");
            }
        }
    }
}
