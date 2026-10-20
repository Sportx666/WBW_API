using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> Carrier_Insert(wbm_common.DataObjects.Carrier.dbRow carrier)
        {
            try
            {
                await _dbContext.Carrier.AddAsync(carrier);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(carrier.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_Insert - " + carrier.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Carrier_Update(wbm_common.DataObjects.Carrier.dbRow carrier)
        {
            try
            {
                _dbContext.Carrier.Attach(carrier);
                _dbContext.Entry(carrier).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(carrier.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_Update - " + carrier.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        private IQueryable<Carrier.dbRowExtended> CarrierQuery()
        {
            return
                from carrier in _dbContext.Carrier.AsNoTracking()
                join site in _dbContext.Site.AsNoTracking()
                    on carrier.SiteID equals site.ID into siteGroup
                from site in siteGroup.DefaultIfEmpty()

                select new Carrier.dbRowExtended
                {
                    ID = carrier.ID,
                    PublicID = carrier.PublicID,
                    SiteID = carrier.SiteID,
                    Description = carrier.Description,
                    SortOrder = carrier.SortOrder,
                    Paperless_KeyID = carrier.Paperless_KeyID,
                    DeletedFlag = carrier.DeletedFlag,
                    CreatedDateTime =carrier.CreatedDateTime,
                    CreatedMethod = carrier.CreatedMethod,
                    CreatedUserID = carrier.CreatedUserID,
                    LastAmendedDateTime = carrier.LastAmendedDateTime,
                    LastAmendedMethod = carrier.LastAmendedMethod,
                    LastAmendedUserID = carrier.LastAmendedUserID,

                    Site_PublicID = site.PublicID
                };
        }

        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRowExtended>>> Carrier_ListByPaperlessKeyID(string PaperlessKeyID)
        {
            try
            {
                wbm_common.DataObjects.Carrier.dbRowExtended carrierDb = await (from carrier in _dbContext.Carrier.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.Paperless_KeyID == PaperlessKeyID).Take(1)
                                                                        join site in _dbContext.Site.AsNoTracking()
                                                                         on carrier.SiteID equals site.ID into siteGroup
                                                                        from site in siteGroup.DefaultIfEmpty()

                                                                        select new Carrier.dbRowExtended
                                                                        {
                                                                            ID = carrier.ID,
                                                                            PublicID = carrier.PublicID,
                                                                            SiteID = carrier.SiteID,
                                                                            Description = carrier.Description,
                                                                            SortOrder = carrier.SortOrder,
                                                                            Paperless_KeyID = carrier.Paperless_KeyID,
                                                                            DeletedFlag = carrier.DeletedFlag,
                                                                            CreatedDateTime = carrier.CreatedDateTime,
                                                                            CreatedMethod = carrier.CreatedMethod,
                                                                            CreatedUserID = carrier.CreatedUserID,
                                                                            LastAmendedDateTime = carrier.LastAmendedDateTime,
                                                                            LastAmendedMethod = carrier.LastAmendedMethod,
                                                                            LastAmendedUserID = carrier.LastAmendedUserID,

                                                                            Site_PublicID = site.PublicID
                                                                        }).FirstOrDefaultAsync();
                if (carrierDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRowExtended>>(new List<wbm_common.DataObjects.Carrier.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRowExtended>>(new List<wbm_common.DataObjects.Carrier.dbRowExtended> { carrierDb });
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_ListByPaperlessKeyID - " + PaperlessKeyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.Carrier.dbRow>> Carrier_SearchByPublicIDSiteID(string PublicID, int siteID)
        {
            try
            {
                wbm_common.DataObjects.Carrier.dbRow carrierDb = await _dbContext.Carrier.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.PublicID == PublicID && x.SiteID == siteID).FirstOrDefaultAsync();

                if (carrierDb == null)
                    return Result.Failure<wbm_common.DataObjects.Carrier.dbRow>("Carrier not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.Carrier.dbRow>(carrierDb);

            }
            catch (Exception ex)
            {
                logger.Error("Carrier_SearchByPublicIDSiteID - PublicID " + PublicID + ", SiteID " + siteID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Carrier.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRowExtended>>> Carrier_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Carrier.dbRowExtended carrierDb = await (from carrier in _dbContext.Carrier.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).Take(1)
                                                                        join site in _dbContext.Site.AsNoTracking()
                                                                            on carrier.SiteID equals site.ID into siteGroup
                                                                        from site in siteGroup.DefaultIfEmpty()

                                                                        select new Carrier.dbRowExtended
                                                                        {
                                                                            ID = carrier.ID,
                                                                            PublicID = carrier.PublicID,
                                                                            SiteID = carrier.SiteID,
                                                                            Description = carrier.Description,
                                                                            SortOrder = carrier.SortOrder,
                                                                            Paperless_KeyID = carrier.Paperless_KeyID,
                                                                            DeletedFlag = carrier.DeletedFlag,
                                                                            CreatedDateTime = carrier.CreatedDateTime,
                                                                            CreatedMethod = carrier.CreatedMethod,
                                                                            CreatedUserID = carrier.CreatedUserID,
                                                                            LastAmendedDateTime = carrier.LastAmendedDateTime,
                                                                            LastAmendedMethod = carrier.LastAmendedMethod,
                                                                            LastAmendedUserID = carrier.LastAmendedUserID,

                                                                            Site_PublicID = site.PublicID
                                                                        }).FirstOrDefaultAsync();

                if (carrierDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRowExtended>>(new List<wbm_common.DataObjects.Carrier.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRowExtended>>(new List<wbm_common.DataObjects.Carrier.dbRowExtended> { carrierDb });
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Carrier.dbRow>> Carrier_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Carrier.dbRow carrierDb = await _dbContext.Carrier.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (carrierDb == null)
                    return Result.Failure<wbm_common.DataObjects.Carrier.dbRow>("Carrier not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Carrier.dbRow>(carrierDb);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Carrier.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRowExtended>>> Carrier_List()
        {
            try
            {
                List<wbm_common.DataObjects.Carrier.dbRowExtended> carriers = await (from carrier in _dbContext.Carrier.AsNoTracking()
                                                                             .Where(x => !x.DeletedFlag)
                                                                             join site in _dbContext.Site.AsNoTracking()
                                                                                 on carrier.SiteID equals site.ID into siteGroup
                                                                             from site in siteGroup.DefaultIfEmpty()

                                                                             select new Carrier.dbRowExtended
                                                                             {
                                                                                 ID = carrier.ID,
                                                                                 PublicID = carrier.PublicID,
                                                                                 SiteID = carrier.SiteID,
                                                                                 Description = carrier.Description,
                                                                                 SortOrder = carrier.SortOrder,
                                                                                 Paperless_KeyID = carrier.Paperless_KeyID,
                                                                                 DeletedFlag = carrier.DeletedFlag,
                                                                                 CreatedDateTime = carrier.CreatedDateTime,
                                                                                 CreatedMethod = carrier.CreatedMethod,
                                                                                 CreatedUserID = carrier.CreatedUserID,
                                                                                 LastAmendedDateTime = carrier.LastAmendedDateTime,
                                                                                 LastAmendedMethod = carrier.LastAmendedMethod,
                                                                                 LastAmendedUserID = carrier.LastAmendedUserID,

                                                                                 Site_PublicID = site.PublicID
                                                                             }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Carrier.dbRowExtended>>(carriers);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRowExtended>>> Carrier_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Carrier_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Carrier.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Carrier.dbRowExtended> carriers = await (from carrier in _dbContext.Carrier.AsNoTracking()
                                                                                 .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                 join site in _dbContext.Site.AsNoTracking()
                                                                                     on carrier.SiteID equals site.ID into siteGroup
                                                                                 from site in siteGroup.DefaultIfEmpty()

                                                                                 select new Carrier.dbRowExtended
                                                                                 {
                                                                                     ID = carrier.ID,
                                                                                     PublicID = carrier.PublicID,
                                                                                     SiteID = carrier.SiteID,
                                                                                     Description = carrier.Description,
                                                                                     SortOrder = carrier.SortOrder,
                                                                                     Paperless_KeyID = carrier.Paperless_KeyID,
                                                                                     DeletedFlag = carrier.DeletedFlag,
                                                                                     CreatedDateTime = carrier.CreatedDateTime,
                                                                                     CreatedMethod = carrier.CreatedMethod,
                                                                                     CreatedUserID = carrier.CreatedUserID,
                                                                                     LastAmendedDateTime = carrier.LastAmendedDateTime,
                                                                                     LastAmendedMethod = carrier.LastAmendedMethod,
                                                                                     LastAmendedUserID = carrier.LastAmendedUserID,

                                                                                     Site_PublicID = site.PublicID
                                                                                 }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRowExtended>>(carriers);
                }
                catch (Exception ex)
                {
                    logger.Error("Carrier_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRowExtended>>> Carrier_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Carrier.dbRowExtended> carriers = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.Carrier.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Carrier.dbRowExtended>>(carriers);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRowExtended>>("Internal Exception occured");
            }
        }
    }
}
