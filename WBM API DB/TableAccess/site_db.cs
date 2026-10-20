using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

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
                wbm_common.DataObjects.Site.dbRow SiteDb = await _dbContext.Site.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).FirstOrDefaultAsync();

                if (SiteDb == null)
                    return Result.Failure<wbm_common.DataObjects.Site.dbRow>("Site not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Site.dbRow>(SiteDb);

            }
            catch (Exception ex)
            {
                logger.Error("Site_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Site.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Site.dbRow>> Site_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.Site.dbRow SiteDb = await _dbContext.Site.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.PublicID == PublicID).FirstOrDefaultAsync();

                if (SiteDb == null)
                    return Result.Failure<wbm_common.DataObjects.Site.dbRow>("Site not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.Site.dbRow>(SiteDb);

            }
            catch (Exception ex)
            {
                logger.Error("Site_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Site.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.Site.dbRowExtended>>> Site_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.Site.dbRowExtended> SiteDb = await (from Site in _dbContext.Site.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                join SiteSystem in _dbContext.SiteSystem.AsNoTracking()
                                                                                    on Site.SiteSystemID equals SiteSystem.ID into SiteSystemPR
                                                                                from SSPR in SiteSystemPR.DefaultIfEmpty()
                                                                                join company in _dbContext.Company.AsNoTracking()
                                                                    on Site.CompanyID equals company.ID into companyPR
                                                                                from CPR in companyPR.DefaultIfEmpty()

                                                                                select new wbm_common.DataObjects.Site.dbRowExtended
                                                                                {
                                                                                    ID = Site.ID,
                                                                                    CompanyID = Site.CompanyID,
                                                                                    PublicID = Site.PublicID,
                                                                                    SiteSystemID = Site.SiteSystemID,
                                                                                    Active = Site.Active,
                                                                                    Description = Site.Description,
                                                                                    SortOrder = Site.SortOrder,
                                                                                    DeletedFlag = Site.DeletedFlag,
                                                                                    CreatedDateTime = Site.CreatedDateTime,
                                                                                    CreatedMethod = Site.CreatedMethod,
                                                                                    CreatedUserID = Site.CreatedUserID,
                                                                                    LastAmendedDateTime = Site.LastAmendedDateTime,
                                                                                    LastAmendedMethod = Site.LastAmendedMethod,
                                                                                    LastAmendedUserID = Site.LastAmendedUserID,

                                                                                    SiteSystem_PublicID = SSPR.PublicID,
                                                                                    Company_ShortName = CPR.ShortName
                                                                                }).ToListAsync();

                if (SiteDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Site.dbRowExtended>>(new List<wbm_common.DataObjects.Site.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Site.dbRowExtended>>(SiteDb);
            }
            catch (Exception ex)
            {
                logger.Error("Site_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Site.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Site.dbRow>> Site_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Site.dbRow SiteDb = await _dbContext.Site.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (SiteDb == null)
                    return Result.Failure<wbm_common.DataObjects.Site.dbRow>("Site not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Site.dbRow>(SiteDb);
            }
            catch (Exception ex)
            {
                logger.Error("Site_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Site.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Site.dbRowExtended>>> Site_List()
        {
            try
            {
                List<wbm_common.DataObjects.Site.dbRowExtended> Sites = await (from Site in _dbContext.Site.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                               join SiteSystem in _dbContext.SiteSystem.AsNoTracking()
                                                                                   on Site.SiteSystemID equals SiteSystem.ID into SiteSystemPR
                                                                               from SSPR in SiteSystemPR.DefaultIfEmpty()
                                                                               join company in _dbContext.Company.AsNoTracking()
                                                                   on Site.CompanyID equals company.ID into companyPR
                                                                               from CPR in companyPR.DefaultIfEmpty()

                                                                               select new wbm_common.DataObjects.Site.dbRowExtended
                                                                               {
                                                                                   ID = Site.ID,
                                                                                   CompanyID = Site.CompanyID,
                                                                                   PublicID = Site.PublicID,
                                                                                   SiteSystemID = Site.SiteSystemID,
                                                                                   Active = Site.Active,
                                                                                   Description = Site.Description,
                                                                                   SortOrder = Site.SortOrder,
                                                                                   DeletedFlag = Site.DeletedFlag,
                                                                                   CreatedDateTime = Site.CreatedDateTime,
                                                                                   CreatedMethod = Site.CreatedMethod,
                                                                                   CreatedUserID = Site.CreatedUserID,
                                                                                   LastAmendedDateTime = Site.LastAmendedDateTime,
                                                                                   LastAmendedMethod = Site.LastAmendedMethod,
                                                                                   LastAmendedUserID = Site.LastAmendedUserID,

                                                                                   SiteSystem_PublicID = SSPR.PublicID,
                                                                                   Company_ShortName = CPR.ShortName
                                                                               }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Site.dbRowExtended>>(Sites);
            }
            catch (Exception ex)
            {
                logger.Error("Site_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Site.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Site.dbRowExtended>>> Site_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Site_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Site.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Site.dbRowExtended> Sites = await (from Site in _dbContext.Site.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                   join SiteSystem in _dbContext.SiteSystem.AsNoTracking()
                                                                                       on Site.SiteSystemID equals SiteSystem.ID into SiteSystemPR
                                                                                   from SSPR in SiteSystemPR.DefaultIfEmpty()
                                                                                   join company in _dbContext.Company.AsNoTracking()
                                                                       on Site.CompanyID equals company.ID into companyPR
                                                                                   from CPR in companyPR.DefaultIfEmpty()

                                                                                   select new wbm_common.DataObjects.Site.dbRowExtended
                                                                                   {
                                                                                       ID = Site.ID,
                                                                                       CompanyID = Site.CompanyID,
                                                                                       PublicID = Site.PublicID,
                                                                                       SiteSystemID = Site.SiteSystemID,
                                                                                       Active = Site.Active,
                                                                                       Description = Site.Description,
                                                                                       SortOrder = Site.SortOrder,
                                                                                       DeletedFlag = Site.DeletedFlag,
                                                                                       CreatedDateTime = Site.CreatedDateTime,
                                                                                       CreatedMethod = Site.CreatedMethod,
                                                                                       CreatedUserID = Site.CreatedUserID,
                                                                                       LastAmendedDateTime = Site.LastAmendedDateTime,
                                                                                       LastAmendedMethod = Site.LastAmendedMethod,
                                                                                       LastAmendedUserID = Site.LastAmendedUserID,

                                                                                       SiteSystem_PublicID = SSPR.PublicID,
                                                                                       Company_ShortName = CPR.ShortName
                                                                                   }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Site.dbRowExtended>>(Sites);
                }
                catch (Exception ex)
                {
                    logger.Error("Site_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Site.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Site.dbRowExtended>>> Site_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Site.dbRowExtended> Sites = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.Site.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Site.dbRowExtended>>(Sites);
            }
            catch (Exception ex)
            {
                logger.Error("Site_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Site.dbRowExtended>>("Internal Exception occured");
            }
        }
    }
}
