using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

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

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>> ZoneProcessing_ListBySingleCompanyID(int CompanyID)
        {
            try
            {
                List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended> ZoneProcessingDb = await (from ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.CompanyID == CompanyID)
                                                                                                    join site in _dbContext.Site.AsNoTracking()
                                                                                                        on ZoneProcessing.SiteID equals site.ID into sitePR
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                              on ZoneProcessing.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.ZoneProcessing.dbRowExtended
                                                                                                    {
                                                                                                        ID = ZoneProcessing.ID,
                                                                                                        CompanyID = ZoneProcessing.CompanyID,
                                                                                                        SiteID = ZoneProcessing.SiteID,
                                                                                                        ZoneNumber = ZoneProcessing.ZoneNumber,
                                                                                                        Description = ZoneProcessing.Description,
                                                                                                        IsPickFace = ZoneProcessing.IsPickFace,
                                                                                                        IsReserve = ZoneProcessing.IsReserve,
                                                                                                        IsCartonConsolidation = ZoneProcessing.IsCartonConsolidation,
                                                                                                        IsReceiving = ZoneProcessing.IsReceiving,
                                                                                                        DeletedFlag = ZoneProcessing.DeletedFlag,
                                                                                                        CreatedDateTime = ZoneProcessing.CreatedDateTime,
                                                                                                        CreatedMethod = ZoneProcessing.CreatedMethod,
                                                                                                        CreatedUserID = ZoneProcessing.CreatedUserID,
                                                                                                        LastAmendedDateTime = ZoneProcessing.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = ZoneProcessing.LastAmendedMethod,
                                                                                                        LastAmendedUserID = ZoneProcessing.LastAmendedUserID,

                                                                                                        Site_PublicID = SPR.PublicID,
                                                                                                        Company_ShortName = CPR.ShortName
                                                                                                    }).ToListAsync();

                if (ZoneProcessingDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(ZoneProcessingDb);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ListBySingleCompanyID - " + CompanyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>> ZoneProcessing_ListBySingleSiteID(int SiteID)
        {
            try
            {
                List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended> ZoneProcessingDb = await (from ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.SiteID == SiteID || x.SiteID == 0))
                                                                                            join site in _dbContext.Site.AsNoTracking()
                                                                                                on ZoneProcessing.SiteID equals site.ID
                                                                                            join company in _dbContext.Company.AsNoTracking()
                                                                                                on ZoneProcessing.CompanyID equals company.ID

                                                                                            select new wbm_common.DataObjects.ZoneProcessing.dbRowExtended
                                                                                            {
                                                                                                ID = ZoneProcessing.ID,
                                                                                                CompanyID = ZoneProcessing.CompanyID,
                                                                                                SiteID = ZoneProcessing.SiteID,
                                                                                                ZoneNumber = ZoneProcessing.ZoneNumber,
                                                                                                Description = ZoneProcessing.Description,
                                                                                                IsPickFace = ZoneProcessing.IsPickFace,
                                                                                                IsReserve = ZoneProcessing.IsReserve,
                                                                                                IsCartonConsolidation = ZoneProcessing.IsCartonConsolidation,
                                                                                                IsReceiving = ZoneProcessing.IsReceiving,
                                                                                                DeletedFlag = ZoneProcessing.DeletedFlag,
                                                                                                CreatedDateTime = ZoneProcessing.CreatedDateTime,
                                                                                                CreatedMethod = ZoneProcessing.CreatedMethod,
                                                                                                CreatedUserID = ZoneProcessing.CreatedUserID,
                                                                                                LastAmendedDateTime = ZoneProcessing.LastAmendedDateTime,
                                                                                                LastAmendedMethod = ZoneProcessing.LastAmendedMethod,
                                                                                                LastAmendedUserID = ZoneProcessing.LastAmendedUserID,

                                                                                                Site_PublicID = site.PublicID,
                                                                                                Company_ShortName = company.ShortName
                                                                                            }).ToListAsync();

                if (ZoneProcessingDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(ZoneProcessingDb);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ListBySingleSiteID - " + SiteID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>> ZoneProcessing_ListBySingleSiteIDZoneNumber(int SiteID, string ZoneNumber)
        {
            try
            {
                List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended> ZoneProcessingDb = await (from ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.SiteID == SiteID || x.SiteID == 0) && x.ZoneNumber == ZoneNumber)
                                                                                                    join site in _dbContext.Site.AsNoTracking()
                                                                                                        on ZoneProcessing.SiteID equals site.ID into sitePR
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                              on ZoneProcessing.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.ZoneProcessing.dbRowExtended
                                                                                                    {
                                                                                                        ID = ZoneProcessing.ID,
                                                                                                        CompanyID = ZoneProcessing.CompanyID,
                                                                                                        SiteID = ZoneProcessing.SiteID,
                                                                                                        ZoneNumber = ZoneProcessing.ZoneNumber,
                                                                                                        Description = ZoneProcessing.Description,
                                                                                                        IsPickFace = ZoneProcessing.IsPickFace,
                                                                                                        IsReserve = ZoneProcessing.IsReserve,
                                                                                                        IsCartonConsolidation = ZoneProcessing.IsCartonConsolidation,
                                                                                                        IsReceiving = ZoneProcessing.IsReceiving,
                                                                                                        DeletedFlag = ZoneProcessing.DeletedFlag,
                                                                                                        CreatedDateTime = ZoneProcessing.CreatedDateTime,
                                                                                                        CreatedMethod = ZoneProcessing.CreatedMethod,
                                                                                                        CreatedUserID = ZoneProcessing.CreatedUserID,
                                                                                                        LastAmendedDateTime = ZoneProcessing.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = ZoneProcessing.LastAmendedMethod,
                                                                                                        LastAmendedUserID = ZoneProcessing.LastAmendedUserID,

                                                                                                        Site_PublicID = SPR.PublicID,
                                                                                                        Company_ShortName = CPR.ShortName
                                                                                                    }).ToListAsync();

                if (ZoneProcessingDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(ZoneProcessingDb);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ListBySingleSiteIDZoneNumber - " + SiteID + ", " + ZoneNumber + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ZoneProcessing.dbRow>> ZoneProcessing_SearchByZoneNumber(string ZoneNumber)
        {
            try
            {
                wbm_common.DataObjects.ZoneProcessing.dbRow ZoneProcessingDb = await _dbContext.ZoneProcessing.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ZoneNumber == ZoneNumber).FirstOrDefaultAsync();

                if (ZoneProcessingDb == null)
                    return Result.Failure<wbm_common.DataObjects.ZoneProcessing.dbRow>("ZoneProcessing not found : " + ZoneNumber);
                else
                    return Result.Success<wbm_common.DataObjects.ZoneProcessing.dbRow>(ZoneProcessingDb);

            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_SearchByZoneNumber - " + ZoneNumber + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ZoneProcessing.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>> ZoneProcessing_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended> ZoneProcessingDb = await (from ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                    join site in _dbContext.Site.AsNoTracking()
                                                                                                        on ZoneProcessing.SiteID equals site.ID into sitePR
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                              on ZoneProcessing.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.ZoneProcessing.dbRowExtended
                                                                                                    {
                                                                                                        ID = ZoneProcessing.ID,
                                                                                                        CompanyID = ZoneProcessing.CompanyID,
                                                                                                        SiteID = ZoneProcessing.SiteID,
                                                                                                        ZoneNumber = ZoneProcessing.ZoneNumber,
                                                                                                        Description = ZoneProcessing.Description,
                                                                                                        IsPickFace = ZoneProcessing.IsPickFace,
                                                                                                        IsReserve = ZoneProcessing.IsReserve,
                                                                                                        IsCartonConsolidation = ZoneProcessing.IsCartonConsolidation,
                                                                                                        DeletedFlag = ZoneProcessing.DeletedFlag,
                                                                                                        CreatedDateTime = ZoneProcessing.CreatedDateTime,
                                                                                                        CreatedMethod = ZoneProcessing.CreatedMethod,
                                                                                                        CreatedUserID = ZoneProcessing.CreatedUserID,
                                                                                                        LastAmendedDateTime = ZoneProcessing.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = ZoneProcessing.LastAmendedMethod,
                                                                                                        LastAmendedUserID = ZoneProcessing.LastAmendedUserID,

                                                                                                        Site_PublicID = SPR.PublicID,
                                                                                                        Company_ShortName = CPR.ShortName
                                                                                                    }).ToListAsync();

                if (ZoneProcessingDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(new List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(ZoneProcessingDb);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ZoneProcessing.dbRow>> ZoneProcessing_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ZoneProcessing.dbRow ZoneProcessingDb = await _dbContext.ZoneProcessing.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (ZoneProcessingDb == null)
                    return Result.Failure<wbm_common.DataObjects.ZoneProcessing.dbRow>("ZoneProcessing not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ZoneProcessing.dbRow>(ZoneProcessingDb);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ZoneProcessing.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>> ZoneProcessing_List()
        {
            try
            {
                List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended> ZoneProcessings = await (from ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                   join site in _dbContext.Site.AsNoTracking()
                                                                                                       on ZoneProcessing.SiteID equals site.ID into sitePR
                                                                                                   from SPR in sitePR.DefaultIfEmpty()
                                                                                                   join company in _dbContext.Company.AsNoTracking()
                                                                                             on ZoneProcessing.CompanyID equals company.ID into companyPR
                                                                                                   from CPR in companyPR.DefaultIfEmpty()

                                                                                                   select new wbm_common.DataObjects.ZoneProcessing.dbRowExtended
                                                                                                   {
                                                                                                       ID = ZoneProcessing.ID,
                                                                                                       CompanyID = ZoneProcessing.CompanyID,
                                                                                                       SiteID = ZoneProcessing.SiteID,
                                                                                                       ZoneNumber = ZoneProcessing.ZoneNumber,
                                                                                                       Description = ZoneProcessing.Description,
                                                                                                       IsPickFace = ZoneProcessing.IsPickFace,
                                                                                                       IsReserve = ZoneProcessing.IsReserve,
                                                                                                       IsCartonConsolidation = ZoneProcessing.IsCartonConsolidation,
                                                                                                       IsReceiving = ZoneProcessing.IsReceiving,
                                                                                                       DeletedFlag = ZoneProcessing.DeletedFlag,
                                                                                                       CreatedDateTime = ZoneProcessing.CreatedDateTime,
                                                                                                       CreatedMethod = ZoneProcessing.CreatedMethod,
                                                                                                       CreatedUserID = ZoneProcessing.CreatedUserID,
                                                                                                       LastAmendedDateTime = ZoneProcessing.LastAmendedDateTime,
                                                                                                       LastAmendedMethod = ZoneProcessing.LastAmendedMethod,
                                                                                                       LastAmendedUserID = ZoneProcessing.LastAmendedUserID,

                                                                                                       Site_PublicID = SPR.PublicID,
                                                                                                       Company_ShortName = CPR.ShortName
                                                                                                   }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(ZoneProcessings);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>> ZoneProcessing_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ZoneProcessing_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended> ZoneProcessings = await (from ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                       join site in _dbContext.Site.AsNoTracking()
                                                                                                           on ZoneProcessing.SiteID equals site.ID into sitePR
                                                                                                       from SPR in sitePR.DefaultIfEmpty()
                                                                                                       join company in _dbContext.Company.AsNoTracking()
                                                                                                 on ZoneProcessing.CompanyID equals company.ID into companyPR
                                                                                                       from CPR in companyPR.DefaultIfEmpty()

                                                                                                       select new wbm_common.DataObjects.ZoneProcessing.dbRowExtended
                                                                                                       {
                                                                                                           ID = ZoneProcessing.ID,
                                                                                                           CompanyID = ZoneProcessing.CompanyID,
                                                                                                           SiteID = ZoneProcessing.SiteID,
                                                                                                           ZoneNumber = ZoneProcessing.ZoneNumber,
                                                                                                           Description = ZoneProcessing.Description,
                                                                                                           IsPickFace = ZoneProcessing.IsPickFace,
                                                                                                           IsReserve = ZoneProcessing.IsReserve,
                                                                                                           IsCartonConsolidation = ZoneProcessing.IsCartonConsolidation,
                                                                                                           IsReceiving = ZoneProcessing.IsReceiving,
                                                                                                           DeletedFlag = ZoneProcessing.DeletedFlag,
                                                                                                           CreatedDateTime = ZoneProcessing.CreatedDateTime,
                                                                                                           CreatedMethod = ZoneProcessing.CreatedMethod,
                                                                                                           CreatedUserID = ZoneProcessing.CreatedUserID,
                                                                                                           LastAmendedDateTime = ZoneProcessing.LastAmendedDateTime,
                                                                                                           LastAmendedMethod = ZoneProcessing.LastAmendedMethod,
                                                                                                           LastAmendedUserID = ZoneProcessing.LastAmendedUserID,

                                                                                                           Site_PublicID = SPR.PublicID,
                                                                                                           Company_ShortName = CPR.ShortName
                                                                                                       }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(ZoneProcessings);
                }
                catch (Exception ex)
                {
                    logger.Error("ZoneProcessing_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>> ZoneProcessing_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended> ZoneProcessings = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>(ZoneProcessings);
            }
            catch (Exception ex)
            {
                logger.Error("ZoneProcessing_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>>("Internal Exception occured");
            }
        }
    }
}
