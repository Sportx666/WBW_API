using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> DevanLookup_Insert(wbm_common.DataObjects.DevanLookup.dbRow DevanLookup)
        {
            try
            {
                await _dbContext.DevanLookup.AddAsync(DevanLookup);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(DevanLookup.ID);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_Insert - " + DevanLookup.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> DevanLookup_Update(wbm_common.DataObjects.DevanLookup.dbRow DevanLookup)
        {
            try
            {
                _dbContext.DevanLookup.Attach(DevanLookup);
                _dbContext.Entry(DevanLookup).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(DevanLookup.ID);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_Update - " + DevanLookup.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>> DevanLookup_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.DevanLookup.dbRowExtended> DevanLookupDb = await (from DevanLookup in _dbContext.DevanLookup.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                              join site in _dbContext.Site.AsNoTracking()
                                                                                                  on DevanLookup.SiteID equals site.ID into sitePR
                                                                                              from SPR in sitePR.DefaultIfEmpty()
                                                                                              join company in _dbContext.Company.AsNoTracking()
                                                                                          on DevanLookup.CompanyID equals company.ID into companyPR
                                                                                              from CPR in companyPR.DefaultIfEmpty()
                                                                                              join Owner in _dbContext.Owner.AsNoTracking()
                                                                                          on DevanLookup.OwnerID equals Owner.ID into OwnerPR
                                                                                              from OPR in OwnerPR.DefaultIfEmpty()
                                                                                              join ContainerSize in _dbContext.ContainerSize.AsNoTracking()
                                                                                          on DevanLookup.ContainerSizeID equals ContainerSize.ID into ContainerSizePR
                                                                                              from CSPR in ContainerSizePR.DefaultIfEmpty()
                                                                                              join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                          on DevanLookup.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                              from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                              join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on DevanLookup.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                              from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                              select new wbm_common.DataObjects.DevanLookup.dbRowExtended
                                                                                              {
                                                                                                  ID = DevanLookup.ID,
                                                                                                  CompanyID = DevanLookup.CompanyID,
                                                                                                  OwnerID = DevanLookup.OwnerID,
                                                                                                  SiteID = DevanLookup.SiteID,
                                                                                                  ContainerSizeID = DevanLookup.ContainerSizeID,
                                                                                                  IsContainerLoose = DevanLookup.IsContainerLoose,
                                                                                                  MaximumSKUCount = DevanLookup.MaximumSKUCount,
                                                                                                  RateCollectionRateChargeCode = DevanLookup.RateCollectionRateChargeCode,
                                                                                                  DeletedFlag = DevanLookup.DeletedFlag,
                                                                                                  CreatedDateTime = DevanLookup.CreatedDateTime,
                                                                                                  CreatedMethod = DevanLookup.CreatedMethod,
                                                                                                  CreatedUserID = DevanLookup.CreatedUserID,
                                                                                                  LastAmendedDateTime = DevanLookup.LastAmendedDateTime,
                                                                                                  LastAmendedMethod = DevanLookup.LastAmendedMethod,
                                                                                                  LastAmendedUserID = DevanLookup.LastAmendedUserID,

                                                                                                  Site_PublicID = SPR.PublicID,
                                                                                                  Company_ShortName = CPR.ShortName,
                                                                                                  ContainerSize_PublicID = CSPR.PublicID,
                                                                                                  Owner_Name = OPR.Name,
                                                                                                  RateCode_RateCode = RCPR.Description,
                                                                                                  Created_UserPublicID = SU.PublicID
                                                                                              }).ToListAsync();

                if (DevanLookupDb == null)
                    return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>(new List<wbm_common.DataObjects.DevanLookup.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>(DevanLookupDb);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.DevanLookup.dbRow>> DevanLookup_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.DevanLookup.dbRow DevanLookupDb = await _dbContext.DevanLookup.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (DevanLookupDb == null)
                    return Result.Failure<wbm_common.DataObjects.DevanLookup.dbRow>("DevanLookup not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.DevanLookup.dbRow>(DevanLookupDb);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.DevanLookup.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>> DevanLookup_List()
        {
            try
            {
                List<wbm_common.DataObjects.DevanLookup.dbRowExtended> DevanLookups = await (from DevanLookup in _dbContext.DevanLookup.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                             join site in _dbContext.Site.AsNoTracking()
                                                                                                 on DevanLookup.SiteID equals site.ID into sitePR
                                                                                             from SPR in sitePR.DefaultIfEmpty()
                                                                                             join company in _dbContext.Company.AsNoTracking()
                                                                                         on DevanLookup.CompanyID equals company.ID into companyPR
                                                                                             from CPR in companyPR.DefaultIfEmpty()
                                                                                             join Owner in _dbContext.Owner.AsNoTracking()
                                                                                         on DevanLookup.OwnerID equals Owner.ID into OwnerPR
                                                                                             from OPR in OwnerPR.DefaultIfEmpty()
                                                                                             join ContainerSize in _dbContext.ContainerSize.AsNoTracking()
                                                                                         on DevanLookup.ContainerSizeID equals ContainerSize.ID into ContainerSizePR
                                                                                             from CSPR in ContainerSizePR.DefaultIfEmpty()
                                                                                             join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                         on DevanLookup.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                             from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                             join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                         on DevanLookup.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                             from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                             select new wbm_common.DataObjects.DevanLookup.dbRowExtended
                                                                                             {
                                                                                                 ID = DevanLookup.ID,
                                                                                                 CompanyID = DevanLookup.CompanyID,
                                                                                                 OwnerID = DevanLookup.OwnerID,
                                                                                                 SiteID = DevanLookup.SiteID,
                                                                                                 ContainerSizeID = DevanLookup.ContainerSizeID,
                                                                                                 IsContainerLoose = DevanLookup.IsContainerLoose,
                                                                                                 MaximumSKUCount = DevanLookup.MaximumSKUCount,
                                                                                                 RateCollectionRateChargeCode = DevanLookup.RateCollectionRateChargeCode,
                                                                                                 DeletedFlag = DevanLookup.DeletedFlag,
                                                                                                 CreatedDateTime = DevanLookup.CreatedDateTime,
                                                                                                 CreatedMethod = DevanLookup.CreatedMethod,
                                                                                                 CreatedUserID = DevanLookup.CreatedUserID,
                                                                                                 LastAmendedDateTime = DevanLookup.LastAmendedDateTime,
                                                                                                 LastAmendedMethod = DevanLookup.LastAmendedMethod,
                                                                                                 LastAmendedUserID = DevanLookup.LastAmendedUserID,

                                                                                                 Site_PublicID = SPR.PublicID,
                                                                                                 Company_ShortName = CPR.ShortName,
                                                                                                 ContainerSize_PublicID = CSPR.PublicID,
                                                                                                 Owner_Name = OPR.Name,
                                                                                                 RateCode_RateCode = RCPR.Description,
                                                                                                 Created_UserPublicID = SU.PublicID
                                                                                             }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>(DevanLookups);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>> DevanLookup_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("DevanLookup_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.DevanLookup.dbRowExtended> DevanLookups = await (from DevanLookup in _dbContext.DevanLookup.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                 join site in _dbContext.Site.AsNoTracking()
                                                                                                     on DevanLookup.SiteID equals site.ID into sitePR
                                                                                                 from SPR in sitePR.DefaultIfEmpty()
                                                                                                 join company in _dbContext.Company.AsNoTracking()
                                                                                             on DevanLookup.CompanyID equals company.ID into companyPR
                                                                                                 from CPR in companyPR.DefaultIfEmpty()
                                                                                                 join Owner in _dbContext.Owner.AsNoTracking()
                                                                                             on DevanLookup.OwnerID equals Owner.ID into OwnerPR
                                                                                                 from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                 join ContainerSize in _dbContext.ContainerSize.AsNoTracking()
                                                                                             on DevanLookup.ContainerSizeID equals ContainerSize.ID into ContainerSizePR
                                                                                                 from CSPR in ContainerSizePR.DefaultIfEmpty()
                                                                                                 join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                             on DevanLookup.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                                 from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                 join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                             on DevanLookup.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                 from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                                 select new wbm_common.DataObjects.DevanLookup.dbRowExtended
                                                                                                 {
                                                                                                     ID = DevanLookup.ID,
                                                                                                     CompanyID = DevanLookup.CompanyID,
                                                                                                     OwnerID = DevanLookup.OwnerID,
                                                                                                     SiteID = DevanLookup.SiteID,
                                                                                                     ContainerSizeID = DevanLookup.ContainerSizeID,
                                                                                                     IsContainerLoose = DevanLookup.IsContainerLoose,
                                                                                                     MaximumSKUCount = DevanLookup.MaximumSKUCount,
                                                                                                     RateCollectionRateChargeCode = DevanLookup.RateCollectionRateChargeCode,
                                                                                                     DeletedFlag = DevanLookup.DeletedFlag,
                                                                                                     CreatedDateTime = DevanLookup.CreatedDateTime,
                                                                                                     CreatedMethod = DevanLookup.CreatedMethod,
                                                                                                     CreatedUserID = DevanLookup.CreatedUserID,
                                                                                                     LastAmendedDateTime = DevanLookup.LastAmendedDateTime,
                                                                                                     LastAmendedMethod = DevanLookup.LastAmendedMethod,
                                                                                                     LastAmendedUserID = DevanLookup.LastAmendedUserID,

                                                                                                     Site_PublicID = SPR.PublicID,
                                                                                                     Company_ShortName = CPR.ShortName,
                                                                                                     ContainerSize_PublicID = CSPR.PublicID,
                                                                                                     Owner_Name = OPR.Name,
                                                                                                     RateCode_RateCode = RCPR.Description,
                                                                                                     Created_UserPublicID = SU.PublicID
                                                                                                 }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>(DevanLookups);
                }
                catch (Exception ex)
                {
                    logger.Error("DevanLookup_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>> DevanLookup_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.DevanLookup.dbRowExtended> DevanLookups = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.DevanLookup.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>(DevanLookups);
            }
            catch (Exception ex)
            {
                logger.Error("DevanLookup_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.DevanLookup.dbRowExtended>>("Internal Exception occured");
            }
        }
    }
}
