using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> Company_Insert(wbm_common.DataObjects.Company.dbRow Company)
        {
            try
            {
                await _dbContext.Company.AddAsync(Company);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Company.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Company_Insert - " + Company.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Company_Update(wbm_common.DataObjects.Company.dbRow Company)
        {
            try
            {
                _dbContext.Company.Attach(Company);
                _dbContext.Entry(Company).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Company.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Company_Update - " + Company.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<wbm_common.DataObjects.Company.dbRowExtended>> Company_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Company.dbRowExtended Companydb = await (from Company in _dbContext.Company.AsNoTracking()
                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                join site in _dbContext.SiteSystem.AsNoTracking()
                                                                            on Company.SystemID equals site.ID into siteGroup
                                                                                from site in siteGroup.DefaultIfEmpty()
                                                                                join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                            on Company.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                      from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                      select new Company.dbRowExtended
                                                                                      {
                                                                                          ID = Company.ID,
                                                                                          ShortName = Company.ShortName,
                                                                                          LongName = Company.LongName,
                                                                                          SystemID = Company.SystemID,
                                                                                          Active = Company.Active,
                                                                                          SortOrder = Company.SortOrder,
                                                                                          Abbreviation = Company.Abbreviation,
                                                                                          AbelCostCentre= Company.AbelCostCentre,
                                                                                          CreatedDateTime = Company.CreatedDateTime,
                                                                                          CreatedMethod = Company.CreatedMethod,
                                                                                          CreatedUserID = Company.CreatedUserID,
                                                                                          LastAmendedDateTime = Company.LastAmendedDateTime,
                                                                                          LastAmendedMethod = Company.LastAmendedMethod,
                                                                                          LastAmendedUserID = Company.LastAmendedUserID,

                                                                                          SiteSystem_PublicID = site.PublicID,
                                                                                          Created_UserPublicID = SU.PublicID
                                                                                      }).FirstOrDefaultAsync();

                if (Companydb == null)
                    return Result.Failure<wbm_common.DataObjects.Company.dbRowExtended>("Company not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Company.dbRowExtended>(Companydb);

            }
            catch (Exception ex)
            {
                logger.Error("Company_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Company.dbRowExtended>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.Company.dbRowExtended>>> Company_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Company.dbRowExtended Company = await (from company in _dbContext.Company.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).Take(1)
                                                                      join site in _dbContext.SiteSystem.AsNoTracking()
                                                                       on company.SystemID equals site.ID into siteGroup
                                                                      from site in siteGroup.DefaultIfEmpty()
                                                                      join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                       on company.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                      from SU in SecurityUserPR.DefaultIfEmpty()
                                                                              select new Company.dbRowExtended
                                                                      {
                                                                          ID = company.ID,
                                                                          ShortName = company.ShortName,
                                                                          LongName = company.LongName,
                                                                          SystemID = company.SystemID,
                                                                          Active = company.Active,
                                                                          SortOrder = company.SortOrder,
                                                                          Abbreviation = company.Abbreviation,
                                                                          AbelCostCentre = company.AbelCostCentre,
                                                                          CreatedDateTime = company.CreatedDateTime,
                                                                          CreatedMethod = company.CreatedMethod,
                                                                          CreatedUserID = company.CreatedUserID,
                                                                          LastAmendedDateTime = company.LastAmendedDateTime,
                                                                          LastAmendedMethod = company.LastAmendedMethod,
                                                                          LastAmendedUserID = company.LastAmendedUserID,

                                                                          SiteSystem_PublicID = site.PublicID,
                                                                          Created_UserPublicID = SU.PublicID
                                                                      }).FirstOrDefaultAsync();

                if (Company == null)
                    return Result.Success<List<wbm_common.DataObjects.Company.dbRowExtended>>(new List<wbm_common.DataObjects.Company.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Company.dbRowExtended>>(new List<wbm_common.DataObjects.Company.dbRowExtended> { Company });
            }
            catch (Exception ex)
            {
                logger.Error("Company_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Company.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Company.dbRow>> Company_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Company.dbRow Company = await _dbContext.Company.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (Company == null)
                    return Result.Failure<wbm_common.DataObjects.Company.dbRow>("Company not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Company.dbRow>(Company);
            }
            catch (Exception ex)
            {
                logger.Error("Company_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Company.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Company.dbRowExtended>>> Company_List()
        {
            try
            {
                List<wbm_common.DataObjects.Company.dbRowExtended> Companys = await (from company in _dbContext.Company.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                             join site in _dbContext.SiteSystem.AsNoTracking()
                                                                              on company.SystemID equals site.ID into siteGroup
                                                                             from site in siteGroup.DefaultIfEmpty()
                                                                             join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                              on company.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                             from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                     select new Company.dbRowExtended
                                                                             {
                                                                                 ID = company.ID,
                                                                                 ShortName = company.ShortName,
                                                                                 LongName = company.LongName,
                                                                                 SystemID = company.SystemID,
                                                                                 Active = company.Active,
                                                                                 SortOrder = company.SortOrder,
                                                                                 Abbreviation = company.Abbreviation,
                                                                                 AbelCostCentre = company.AbelCostCentre,
                                                                                 CreatedDateTime = company.CreatedDateTime,
                                                                                 CreatedMethod = company.CreatedMethod,
                                                                                 CreatedUserID = company.CreatedUserID,
                                                                                 LastAmendedDateTime = company.LastAmendedDateTime,
                                                                                 LastAmendedMethod = company.LastAmendedMethod,
                                                                                 LastAmendedUserID = company.LastAmendedUserID,

                                                                                 SiteSystem_PublicID = site.PublicID,
                                                                                 Created_UserPublicID = SU.PublicID
                                                                                     }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Company.dbRowExtended>>(Companys);
            }
            catch (Exception ex)
            {
                logger.Error("Company_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Company.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Company.dbRowExtended>>> Company_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Company_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Company.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Company.dbRowExtended> Companys = await (from company in _dbContext.Company.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                 join site in _dbContext.SiteSystem.AsNoTracking()
                                                                                  on company.SystemID equals site.ID into siteGroup
                                                                                 from site in siteGroup.DefaultIfEmpty()
                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                  on company.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                 from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                 select new Company.dbRowExtended
                                                                                 {
                                                                                     ID = company.ID,
                                                                                     ShortName = company.ShortName,
                                                                                     LongName = company.LongName,
                                                                                     SystemID = company.SystemID,
                                                                                     Active = company.Active,
                                                                                     SortOrder = company.SortOrder,
                                                                                     Abbreviation = company.Abbreviation,
                                                                                     AbelCostCentre = company.AbelCostCentre,
                                                                                     CreatedDateTime = company.CreatedDateTime,
                                                                                     CreatedMethod = company.CreatedMethod,
                                                                                     CreatedUserID = company.CreatedUserID,
                                                                                     LastAmendedDateTime = company.LastAmendedDateTime,
                                                                                     LastAmendedMethod = company.LastAmendedMethod,
                                                                                     LastAmendedUserID = company.LastAmendedUserID,

                                                                                     SiteSystem_PublicID = site.PublicID,
                                                                                     Created_UserPublicID = SU.PublicID
                                                                                 }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Company.dbRowExtended>>(Companys);
                }
                catch (Exception ex)
                {
                    logger.Error("Company_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Company.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Company.dbRowExtended>>> Company_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Company.dbRowExtended> Companys = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.Company.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Company.dbRowExtended>>(Companys);
            }
            catch (Exception ex)
            {
                logger.Error("Company_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Company.dbRowExtended>>("Internal Exception occured");
            }
        }
    }
}
