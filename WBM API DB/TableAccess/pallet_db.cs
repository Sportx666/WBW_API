using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region PalletType
        public async Task<Result<int>> PalletType_Insert(wbm_common.DataObjects.PalletType.dbRow PalletType)
        {
            try
            {
                await _dbContext.PalletType.AddAsync(PalletType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PalletType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PalletType_Insert - " + PalletType.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> PalletType_Update(wbm_common.DataObjects.PalletType.dbRow PalletType)
        {
            try
            {
                _dbContext.PalletType.Attach(PalletType);
                _dbContext.Entry(PalletType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PalletType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PalletType_Update - " + PalletType.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PalletType.dbRowExtended>>> PalletType_ListBySingleSiteSystemID(int SiteSystemID)
        {
            try
            {
                List<wbm_common.DataObjects.PalletType.dbRowExtended> PalletTypeDb = await (from PalletType in _dbContext.PalletType.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.SiteSystemID == SiteSystemID || x.SiteSystemID == 0))
                                                                                            join site in _dbContext.Site.AsNoTracking()
                                                                                                on PalletType.SiteID equals site.ID into sitePR
                                                                                            from SPR in sitePR.DefaultIfEmpty()
                                                                                            join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                on PalletType.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                            from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                            join SiteSystem in _dbContext.SiteSystem.AsNoTracking()
                                                                                                on PalletType.SiteSystemID equals SiteSystem.ID into SiteSystemPR
                                                                                            from SS in SiteSystemPR.DefaultIfEmpty()

                                                                                            select new wbm_common.DataObjects.PalletType.dbRowExtended
                                                                                            {
                                                                                                ID = PalletType.ID,
                                                                                                SiteID = PalletType.SiteID,
                                                                                                SiteSystemID = PalletType.SiteSystemID,
                                                                                                PublicID = PalletType.PublicID,
                                                                                                Description = PalletType.Description,
                                                                                                ApplyPalletHire = PalletType.ApplyPalletHire,
                                                                                                SortOrder = PalletType.SortOrder,
                                                                                                DeletedFlag = PalletType.DeletedFlag,
                                                                                                CreatedDateTime = PalletType.CreatedDateTime,
                                                                                                CreatedMethod = PalletType.CreatedMethod,
                                                                                                CreatedUserID = PalletType.CreatedUserID,
                                                                                                LastAmendedDateTime = PalletType.LastAmendedDateTime,
                                                                                                LastAmendedMethod = PalletType.LastAmendedMethod,
                                                                                                LastAmendedUserID = PalletType.LastAmendedUserID,

                                                                                                Site_PublicID = SPR.PublicID,
                                                                                                SiteSystem_PublicID = SS.PublicID,
                                                                                                Created_UserPublicID = SU.PublicID
                                                                                            }).ToListAsync();

                if (PalletTypeDb == null)
                    return Result.Success<List<wbm_common.DataObjects.PalletType.dbRowExtended>>(new List<wbm_common.DataObjects.PalletType.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PalletType.dbRowExtended>>(PalletTypeDb);
            }
            catch (Exception ex)
            {
                logger.Error("PalletType_ListBySingleSiteSystemID - " + SiteSystemID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PalletType.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.PalletType.dbRowExtended>> PalletType_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.PalletType.dbRowExtended PalletTypeDb = await (from PalletType in _dbContext.PalletType.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.PublicID == PublicID)
                                                                                            join site in _dbContext.Site.AsNoTracking()
                                                                                                on PalletType.SiteID equals site.ID into sitePR
                                                                                            from SPR in sitePR.DefaultIfEmpty()
                                                                                            join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                on PalletType.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                            from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                            join SiteSystem in _dbContext.SiteSystem.AsNoTracking()
                                                                                                on PalletType.SiteSystemID equals SiteSystem.ID into SiteSystemPR
                                                                                            from SS in SiteSystemPR.DefaultIfEmpty()

                                                                                            select new wbm_common.DataObjects.PalletType.dbRowExtended
                                                                                            {
                                                                                                ID = PalletType.ID,
                                                                                                SiteID = PalletType.SiteID,
                                                                                                SiteSystemID = PalletType.SiteSystemID,
                                                                                                PublicID = PalletType.PublicID,
                                                                                                Description = PalletType.Description,
                                                                                                ApplyPalletHire = PalletType.ApplyPalletHire,
                                                                                                SortOrder = PalletType.SortOrder,
                                                                                                DeletedFlag = PalletType.DeletedFlag,
                                                                                                CreatedDateTime = PalletType.CreatedDateTime,
                                                                                                CreatedMethod = PalletType.CreatedMethod,
                                                                                                CreatedUserID = PalletType.CreatedUserID,
                                                                                                LastAmendedDateTime = PalletType.LastAmendedDateTime,
                                                                                                LastAmendedMethod = PalletType.LastAmendedMethod,
                                                                                                LastAmendedUserID = PalletType.LastAmendedUserID,

                                                                                                Site_PublicID = SPR.PublicID,
                                                                                                SiteSystem_PublicID = SS.PublicID,
                                                                                                Created_UserPublicID = SU.PublicID
                                                                                            }).FirstAsync();

                if (PalletTypeDb == null)
                    return Result.Failure<wbm_common.DataObjects.PalletType.dbRowExtended>("PalletType not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.PalletType.dbRowExtended>(PalletTypeDb);
            }
            catch (Exception ex)
            {
                logger.Error("PalletType_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PalletType.dbRowExtended>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.PalletType.dbRowExtended>>> PalletType_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.PalletType.dbRowExtended> PalletTypeDb = await (from PalletType in _dbContext.PalletType.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                            join site in _dbContext.Site.AsNoTracking()
                                                                                                on PalletType.SiteID equals site.ID into sitePR
                                                                                            from SPR in sitePR.DefaultIfEmpty()
                                                                                            join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                on PalletType.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                            from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                            join SiteSystem in _dbContext.SiteSystem.AsNoTracking()
                                                                                                on PalletType.SiteSystemID equals SiteSystem.ID into SiteSystemPR
                                                                                            from SS in SiteSystemPR.DefaultIfEmpty()

                                                                                            select new wbm_common.DataObjects.PalletType.dbRowExtended
                                                                                            {
                                                                                                ID = PalletType.ID,
                                                                                                SiteID = PalletType.SiteID,
                                                                                                SiteSystemID = PalletType.SiteSystemID,
                                                                                                PublicID = PalletType.PublicID,
                                                                                                Description = PalletType.Description,
                                                                                                ApplyPalletHire = PalletType.ApplyPalletHire,
                                                                                                SortOrder = PalletType.SortOrder,
                                                                                                DeletedFlag = PalletType.DeletedFlag,
                                                                                                CreatedDateTime = PalletType.CreatedDateTime,
                                                                                                CreatedMethod = PalletType.CreatedMethod,
                                                                                                CreatedUserID = PalletType.CreatedUserID,
                                                                                                LastAmendedDateTime = PalletType.LastAmendedDateTime,
                                                                                                LastAmendedMethod = PalletType.LastAmendedMethod,
                                                                                                LastAmendedUserID = PalletType.LastAmendedUserID,

                                                                                                Site_PublicID = SPR.PublicID,
                                                                                                SiteSystem_PublicID = SS.PublicID,
                                                                                                Created_UserPublicID = SU.PublicID
                                                                                            }).ToListAsync();

                if (PalletTypeDb == null)
                    return Result.Success<List<wbm_common.DataObjects.PalletType.dbRowExtended>>(new List<wbm_common.DataObjects.PalletType.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PalletType.dbRowExtended>>(PalletTypeDb);
            }
            catch (Exception ex)
            {
                logger.Error("PalletType_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PalletType.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.PalletType.dbRow>> PalletType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PalletType.dbRow PalletTypeDb = await _dbContext.PalletType.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (PalletTypeDb == null)
                    return Result.Failure<wbm_common.DataObjects.PalletType.dbRow>("PalletType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.PalletType.dbRow>(PalletTypeDb);
            }
            catch (Exception ex)
            {
                logger.Error("PalletType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PalletType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PalletType.dbRowExtended>>> PalletType_List()
        {
            try
            {
                List<wbm_common.DataObjects.PalletType.dbRowExtended> PalletTypes = await (from PalletType in _dbContext.PalletType.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                           join site in _dbContext.Site.AsNoTracking()
                                                                                               on PalletType.SiteID equals site.ID into sitePR
                                                                                           from SPR in sitePR.DefaultIfEmpty()
                                                                                           join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                               on PalletType.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                           from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                           join SiteSystem in _dbContext.SiteSystem.AsNoTracking()
                                                                                               on PalletType.SiteSystemID equals SiteSystem.ID into SiteSystemPR
                                                                                           from SS in SiteSystemPR.DefaultIfEmpty()

                                                                                           select new wbm_common.DataObjects.PalletType.dbRowExtended
                                                                                           {
                                                                                               ID = PalletType.ID,
                                                                                               SiteID = PalletType.SiteID,
                                                                                               SiteSystemID = PalletType.SiteSystemID,
                                                                                               PublicID = PalletType.PublicID,
                                                                                               Description = PalletType.Description,
                                                                                               ApplyPalletHire = PalletType.ApplyPalletHire,
                                                                                               SortOrder = PalletType.SortOrder,
                                                                                               DeletedFlag = PalletType.DeletedFlag,
                                                                                               CreatedDateTime = PalletType.CreatedDateTime,
                                                                                               CreatedMethod = PalletType.CreatedMethod,
                                                                                               CreatedUserID = PalletType.CreatedUserID,
                                                                                               LastAmendedDateTime = PalletType.LastAmendedDateTime,
                                                                                               LastAmendedMethod = PalletType.LastAmendedMethod,
                                                                                               LastAmendedUserID = PalletType.LastAmendedUserID,

                                                                                               Site_PublicID = SPR.PublicID,
                                                                                               SiteSystem_PublicID = SS.PublicID,
                                                                                               Created_UserPublicID = SU.PublicID
                                                                                           }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.PalletType.dbRowExtended>>(PalletTypes);
            }
            catch (Exception ex)
            {
                logger.Error("PalletType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.PalletType.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PalletType.dbRowExtended>>> PalletType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("PalletType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.PalletType.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.PalletType.dbRowExtended> PalletTypes = await (from PalletType in _dbContext.PalletType.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                               join site in _dbContext.Site.AsNoTracking()
                                                                                                   on PalletType.SiteID equals site.ID into sitePR
                                                                                               from SPR in sitePR.DefaultIfEmpty()
                                                                                               join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                   on PalletType.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                               from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                               join SiteSystem in _dbContext.SiteSystem.AsNoTracking()
                                                                                                   on PalletType.SiteSystemID equals SiteSystem.ID into SiteSystemPR
                                                                                               from SS in SiteSystemPR.DefaultIfEmpty()

                                                                                               select new wbm_common.DataObjects.PalletType.dbRowExtended
                                                                                               {
                                                                                                   ID = PalletType.ID,
                                                                                                   SiteID = PalletType.SiteID,
                                                                                                   SiteSystemID = PalletType.SiteSystemID,
                                                                                                   PublicID = PalletType.PublicID,
                                                                                                   Description = PalletType.Description,
                                                                                                   ApplyPalletHire = PalletType.ApplyPalletHire,
                                                                                                   SortOrder = PalletType.SortOrder,
                                                                                                   DeletedFlag = PalletType.DeletedFlag,
                                                                                                   CreatedDateTime = PalletType.CreatedDateTime,
                                                                                                   CreatedMethod = PalletType.CreatedMethod,
                                                                                                   CreatedUserID = PalletType.CreatedUserID,
                                                                                                   LastAmendedDateTime = PalletType.LastAmendedDateTime,
                                                                                                   LastAmendedMethod = PalletType.LastAmendedMethod,
                                                                                                   LastAmendedUserID = PalletType.LastAmendedUserID,

                                                                                                   Site_PublicID = SPR.PublicID,
                                                                                                   SiteSystem_PublicID = SS.PublicID,
                                                                                                   Created_UserPublicID = SU.PublicID
                                                                                               }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.PalletType.dbRowExtended>>(PalletTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("PalletType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.PalletType.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PalletType.dbRowExtended>>> PalletType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.PalletType.dbRowExtended> PalletTypes = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.PalletType.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.PalletType.dbRowExtended>>(PalletTypes);
            }
            catch (Exception ex)
            {
                logger.Error("PalletType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PalletType.dbRowExtended>>("Internal Exception occured");
            }
        }

        #endregion
        #region PalletStorageType
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

        public async Task<Result<wbm_common.DataObjects.PalletStorageType.dbRow>> PalletStorageType_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.PalletStorageType.dbRow PalletStorageType = await _dbContext.PalletStorageType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (PalletStorageType == null)
                    return Result.Failure<wbm_common.DataObjects.PalletStorageType.dbRow>("PalletStorageType not found : " + PublicID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.PalletStorageType.dbRow>(PalletStorageType);

            }
            catch (Exception ex)
            {
                logger.Error("PalletStorageType_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PalletStorageType.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.PalletStorageType.dbRow>>> PalletStorageType_ListByID(int ID)
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
                logger.Error("PalletStorageType_ListByID - " + ID.ToString() + ": " + ex);
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
        #endregion
    }
}
