using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> Location_Insert(wbm_common.DataObjects.Location.dbRow Location)
        {
            try
            {
                await _dbContext.Location.AddAsync(Location);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Location.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Location_Insert - " + Location.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Location_Update(wbm_common.DataObjects.Location.dbRow Location)
        {
            try
            {
                _dbContext.Location.Attach(Location);
                _dbContext.Entry(Location).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Location.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Location_Update - " + Location.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.Location.dbRowExtended>>> Location_ListByPaperlessKeyID(string PaperlessKeyID)
        {
            try
            {
                List<wbm_common.DataObjects.Location.dbRowExtended> LocationDb = await (from Location in _dbContext.Location.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.Paperless_KeyID == PaperlessKeyID)
                                                                                        join site in _dbContext.Site.AsNoTracking()
                                                                                            on Location.SiteID equals site.ID into sitePR
                                                                                        from SPR in sitePR.DefaultIfEmpty()
                                                                                        join company in _dbContext.Company.AsNoTracking()
                                                                                    on Location.CompanyID equals company.ID into companyPR
                                                                                        from CPR in companyPR.DefaultIfEmpty()
                                                                                        join palletStorage in _dbContext.PalletStorageType.AsNoTracking()
                                                                                    on Location.PalletStorageTypeID equals palletStorage.ID into palletStoragePR
                                                                                        from PSPR in palletStoragePR.DefaultIfEmpty()

                                                                                        select new wbm_common.DataObjects.Location.dbRowExtended
                                                                                        {
                                                                                            ID = Location.ID,
                                                                                            CompanyID = Location.CompanyID,
                                                                                            SiteID = Location.SiteID,
                                                                                            PublicLocationID = Location.PublicLocationID,
                                                                                            PalletStorageTypeID = Location.PalletStorageTypeID,
                                                                                            Paperless_KeyID = Location.Paperless_KeyID,
                                                                                            DeletedFlag = Location.DeletedFlag,
                                                                                            CreatedDateTime = Location.CreatedDateTime,
                                                                                            CreatedMethod = Location.CreatedMethod,
                                                                                            CreatedUserID = Location.CreatedUserID,
                                                                                            LastAmendedDateTime = Location.LastAmendedDateTime,
                                                                                            LastAmendedMethod = Location.LastAmendedMethod,
                                                                                            LastAmendedUserID = Location.LastAmendedUserID,

                                                                                            Site_PublicID = SPR.PublicID,
                                                                                            Company_ShortName = CPR.ShortName,
                                                                                            PalletStorageType_PublicID = PSPR.PublicID
                                                                                        }).ToListAsync();

                if (LocationDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(new List<wbm_common.DataObjects.Location.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(LocationDb);
            }
            catch (Exception ex)
            {
                logger.Error("Location_ListByPaperlessKeyID - " + PaperlessKeyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRowExtended>>> Location_ListByCompanyID(int CompanyID)
        {
            try
            {
                List<wbm_common.DataObjects.Location.dbRowExtended> LocationDb = await (from Location in _dbContext.Location.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.CompanyID == CompanyID)
                                                                                        join site in _dbContext.Site.AsNoTracking()
                                                                                            on Location.SiteID equals site.ID into sitePR
                                                                                        from SPR in sitePR.DefaultIfEmpty()
                                                                                        join company in _dbContext.Company.AsNoTracking()
                                                                                    on Location.CompanyID equals company.ID into companyPR
                                                                                        from CPR in companyPR.DefaultIfEmpty()
                                                                                        join palletStorage in _dbContext.PalletStorageType.AsNoTracking()
                                                                                    on Location.PalletStorageTypeID equals palletStorage.ID into palletStoragePR
                                                                                        from PSPR in palletStoragePR.DefaultIfEmpty()

                                                                                        select new wbm_common.DataObjects.Location.dbRowExtended
                                                                                        {
                                                                                            ID = Location.ID,
                                                                                            CompanyID = Location.CompanyID,
                                                                                            SiteID = Location.SiteID,
                                                                                            PublicLocationID = Location.PublicLocationID,
                                                                                            PalletStorageTypeID = Location.PalletStorageTypeID,
                                                                                            Paperless_KeyID = Location.Paperless_KeyID,
                                                                                            DeletedFlag = Location.DeletedFlag,
                                                                                            CreatedDateTime = Location.CreatedDateTime,
                                                                                            CreatedMethod = Location.CreatedMethod,
                                                                                            CreatedUserID = Location.CreatedUserID,
                                                                                            LastAmendedDateTime = Location.LastAmendedDateTime,
                                                                                            LastAmendedMethod = Location.LastAmendedMethod,
                                                                                            LastAmendedUserID = Location.LastAmendedUserID,

                                                                                            Site_PublicID = SPR.PublicID,
                                                                                            Company_ShortName = CPR.ShortName,
                                                                                            PalletStorageType_PublicID = PSPR.PublicID
                                                                                        }).ToListAsync();

                if (LocationDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(new List<wbm_common.DataObjects.Location.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(LocationDb);
            }
            catch (Exception ex)
            {
                logger.Error("Location_ListByCompanyID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRowExtended>>> Location_ListBySiteID(int SiteID)
        {
            try
            {
                List<wbm_common.DataObjects.Location.dbRowExtended> LocationDb = await (from Location in _dbContext.Location.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.SiteID == SiteID || x.SiteID == 0))
                                                                                        join site in _dbContext.Site.AsNoTracking()
                                                                                            on Location.SiteID equals site.ID into sitePR
                                                                                        from SPR in sitePR.DefaultIfEmpty()
                                                                                        join company in _dbContext.Company.AsNoTracking()
                                                                                    on Location.CompanyID equals company.ID into companyPR
                                                                                        from CPR in companyPR.DefaultIfEmpty()
                                                                                        join palletStorage in _dbContext.PalletStorageType.AsNoTracking()
                                                                                    on Location.PalletStorageTypeID equals palletStorage.ID into palletStoragePR
                                                                                        from PSPR in palletStoragePR.DefaultIfEmpty()

                                                                                        select new wbm_common.DataObjects.Location.dbRowExtended
                                                                                        {
                                                                                            ID = Location.ID,
                                                                                            CompanyID = Location.CompanyID,
                                                                                            SiteID = Location.SiteID,
                                                                                            PublicLocationID = Location.PublicLocationID,
                                                                                            PalletStorageTypeID = Location.PalletStorageTypeID,
                                                                                            Paperless_KeyID = Location.Paperless_KeyID,
                                                                                            DeletedFlag = Location.DeletedFlag,
                                                                                            CreatedDateTime = Location.CreatedDateTime,
                                                                                            CreatedMethod = Location.CreatedMethod,
                                                                                            CreatedUserID = Location.CreatedUserID,
                                                                                            LastAmendedDateTime = Location.LastAmendedDateTime,
                                                                                            LastAmendedMethod = Location.LastAmendedMethod,
                                                                                            LastAmendedUserID = Location.LastAmendedUserID,

                                                                                            Site_PublicID = SPR.PublicID,
                                                                                            Company_ShortName = CPR.ShortName,
                                                                                            PalletStorageType_PublicID = PSPR.PublicID
                                                                                        }).ToListAsync();

                if (LocationDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(new List<wbm_common.DataObjects.Location.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(LocationDb);
            }
            catch (Exception ex)
            {
                logger.Error("Location_ListBySiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.Location.dbRow>> Location_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Location.dbRow LocationDb = await _dbContext.Location.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).FirstOrDefaultAsync();

                if (LocationDb == null)
                    return Result.Failure<wbm_common.DataObjects.Location.dbRow>("Location not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Location.dbRow>(LocationDb);

            }
            catch (Exception ex)
            {
                logger.Error("Location_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Location.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.Location.dbRow>> Location_SearchByPublicLocationID(string PublicLocationID)
        {
            try
            {
                wbm_common.DataObjects.Location.dbRow LocationDb = await _dbContext.Location.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.PublicLocationID == PublicLocationID).FirstOrDefaultAsync();

                if (LocationDb == null)
                    return Result.Failure<wbm_common.DataObjects.Location.dbRow>("Location not found : " + PublicLocationID);
                else
                    return Result.Success<wbm_common.DataObjects.Location.dbRow>(LocationDb);

            }
            catch (Exception ex)
            {
                logger.Error("Location_SearchByPublicLocationID - " + PublicLocationID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Location.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.Location.dbRowExtended>>> Location_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.Location.dbRowExtended> LocationDb = await (from Location in _dbContext.Location.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                        join site in _dbContext.Site.AsNoTracking()
                                                                                            on Location.SiteID equals site.ID into sitePR
                                                                                        from SPR in sitePR.DefaultIfEmpty()
                                                                                        join company in _dbContext.Company.AsNoTracking()
                                                                                    on Location.CompanyID equals company.ID into companyPR
                                                                                        from CPR in companyPR.DefaultIfEmpty()
                                                                                        join palletStorage in _dbContext.PalletStorageType.AsNoTracking()
                                                                                    on Location.PalletStorageTypeID equals palletStorage.ID into palletStoragePR
                                                                                        from PSPR in palletStoragePR.DefaultIfEmpty()

                                                                                        select new wbm_common.DataObjects.Location.dbRowExtended
                                                                                        {
                                                                                            ID = Location.ID,
                                                                                            CompanyID = Location.CompanyID,
                                                                                            SiteID = Location.SiteID,
                                                                                            PublicLocationID = Location.PublicLocationID,
                                                                                            PalletStorageTypeID = Location.PalletStorageTypeID,
                                                                                            Paperless_KeyID = Location.Paperless_KeyID,
                                                                                            DeletedFlag = Location.DeletedFlag,
                                                                                            CreatedDateTime = Location.CreatedDateTime,
                                                                                            CreatedMethod = Location.CreatedMethod,
                                                                                            CreatedUserID = Location.CreatedUserID,
                                                                                            LastAmendedDateTime = Location.LastAmendedDateTime,
                                                                                            LastAmendedMethod = Location.LastAmendedMethod,
                                                                                            LastAmendedUserID = Location.LastAmendedUserID,

                                                                                            Site_PublicID = SPR.PublicID,
                                                                                            Company_ShortName = CPR.ShortName,
                                                                                            PalletStorageType_PublicID = PSPR.PublicID
                                                                                        }).ToListAsync();

                if (LocationDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(new List<wbm_common.DataObjects.Location.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(LocationDb);
            }
            catch (Exception ex)
            {
                logger.Error("Location_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Location.dbRow>> Location_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Location.dbRow LocationDb = await _dbContext.Location.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (LocationDb == null)
                    return Result.Failure<wbm_common.DataObjects.Location.dbRow>("Location not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Location.dbRow>(LocationDb);
            }
            catch (Exception ex)
            {
                logger.Error("Location_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Location.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRowExtended>>> Location_List()
        {
            try
            {
                List<wbm_common.DataObjects.Location.dbRowExtended> Locations = await (from Location in _dbContext.Location.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                       join site in _dbContext.Site.AsNoTracking()
                                                                                           on Location.SiteID equals site.ID into sitePR
                                                                                       from SPR in sitePR.DefaultIfEmpty()
                                                                                       join company in _dbContext.Company.AsNoTracking()
                                                                                   on Location.CompanyID equals company.ID into companyPR
                                                                                       from CPR in companyPR.DefaultIfEmpty()
                                                                                       join palletStorage in _dbContext.PalletStorageType.AsNoTracking()
                                                                                   on Location.PalletStorageTypeID equals palletStorage.ID into palletStoragePR
                                                                                       from PSPR in palletStoragePR.DefaultIfEmpty()

                                                                                       select new wbm_common.DataObjects.Location.dbRowExtended
                                                                                       {
                                                                                           ID = Location.ID,
                                                                                           CompanyID = Location.CompanyID,
                                                                                           SiteID = Location.SiteID,
                                                                                           PublicLocationID = Location.PublicLocationID,
                                                                                           PalletStorageTypeID = Location.PalletStorageTypeID,
                                                                                           Paperless_KeyID = Location.Paperless_KeyID,
                                                                                           DeletedFlag = Location.DeletedFlag,
                                                                                           CreatedDateTime = Location.CreatedDateTime,
                                                                                           CreatedMethod = Location.CreatedMethod,
                                                                                           CreatedUserID = Location.CreatedUserID,
                                                                                           LastAmendedDateTime = Location.LastAmendedDateTime,
                                                                                           LastAmendedMethod = Location.LastAmendedMethod,
                                                                                           LastAmendedUserID = Location.LastAmendedUserID,

                                                                                           Site_PublicID = SPR.PublicID,
                                                                                           Company_ShortName = CPR.ShortName,
                                                                                           PalletStorageType_PublicID = PSPR.PublicID
                                                                                       }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(Locations);
            }
            catch (Exception ex)
            {
                logger.Error("Location_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Location.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRowExtended>>> Location_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Location_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Location.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Location.dbRowExtended> Locations = await (from Location in _dbContext.Location.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                           join site in _dbContext.Site.AsNoTracking()
                                                                                               on Location.SiteID equals site.ID into sitePR
                                                                                           from SPR in sitePR.DefaultIfEmpty()
                                                                                           join company in _dbContext.Company.AsNoTracking()
                                                                                       on Location.CompanyID equals company.ID into companyPR
                                                                                           from CPR in companyPR.DefaultIfEmpty()
                                                                                           join palletStorage in _dbContext.PalletStorageType.AsNoTracking()
                                                                                       on Location.PalletStorageTypeID equals palletStorage.ID into palletStoragePR
                                                                                           from PSPR in palletStoragePR.DefaultIfEmpty()

                                                                                           select new wbm_common.DataObjects.Location.dbRowExtended
                                                                                           {
                                                                                               ID = Location.ID,
                                                                                               CompanyID = Location.CompanyID,
                                                                                               SiteID = Location.SiteID,
                                                                                               PublicLocationID = Location.PublicLocationID,
                                                                                               PalletStorageTypeID = Location.PalletStorageTypeID,
                                                                                               Paperless_KeyID = Location.Paperless_KeyID,
                                                                                               DeletedFlag = Location.DeletedFlag,
                                                                                               CreatedDateTime = Location.CreatedDateTime,
                                                                                               CreatedMethod = Location.CreatedMethod,
                                                                                               CreatedUserID = Location.CreatedUserID,
                                                                                               LastAmendedDateTime = Location.LastAmendedDateTime,
                                                                                               LastAmendedMethod = Location.LastAmendedMethod,
                                                                                               LastAmendedUserID = Location.LastAmendedUserID,

                                                                                               Site_PublicID = SPR.PublicID,
                                                                                               Company_ShortName = CPR.ShortName,
                                                                                               PalletStorageType_PublicID = PSPR.PublicID
                                                                                           }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(Locations);
                }
                catch (Exception ex)
                {
                    logger.Error("Location_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Location.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRowExtended>>> Location_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Location.dbRowExtended> Locations = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.Location.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Location.dbRowExtended>>(Locations);
            }
            catch (Exception ex)
            {
                logger.Error("Location_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRowExtended>>("Internal Exception occured");
            }
        }
    }
}
