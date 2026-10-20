using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> ActionRateCode_Insert(wbm_common.DataObjects.ActionRateCode.dbRow ActionRateCode)
        {
            try
            {
                await _dbContext.ActionRateCode.AddAsync(ActionRateCode);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ActionRateCode.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_Insert - " + ActionRateCode.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ActionRateCode_Update(wbm_common.DataObjects.ActionRateCode.dbRow ActionRateCode)
        {
            try
            {
                _dbContext.ActionRateCode.Attach(ActionRateCode);
                _dbContext.Entry(ActionRateCode).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ActionRateCode.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_Update - " + ActionRateCode.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>> ActionRateCode_ListBySingleOwnerID(int OwnerID)
        {
            try
            {
                List<wbm_common.DataObjects.ActionRateCode.dbRowExtended> ActionRateCodeDb = await (from ActionRateCode in _dbContext.ActionRateCode.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.OwnerID == OwnerID || x.OwnerID == 0))
                                                                                                    join site in _dbContext.Site.AsNoTracking()
                                                                                                        on ActionRateCode.SiteID equals site.ID into sitePR
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                          on ActionRateCode.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()
                                                                                                    join Owner in _dbContext.Owner.AsNoTracking()
                                                                                          on ActionRateCode.OwnerID equals Owner.ID into OwnerPR
                                                                                                    from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                    join ActionType in _dbContext.ActionType.AsNoTracking()
                                                                                          on ActionRateCode.ActionTypeID equals ActionType.ID into ActionTypePR
                                                                                                    from ATPR in ActionTypePR.DefaultIfEmpty()
                                                                                                    join UnitType in _dbContext.UnitType.AsNoTracking()
                                                                                          on ActionRateCode.UnitTypeID equals UnitType.ID into UnitTypePR
                                                                                                    from UTPR in UnitTypePR.DefaultIfEmpty()
                                                                                                    join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on ActionRateCode.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                    from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                                    join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                          on ActionRateCode.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                                    from SRCPR in StockRateCategoryPR.DefaultIfEmpty()

                                                                                                    select new ActionRateCode.dbRowExtended
                                                                                                    {
                                                                                                        ID = ActionRateCode.ID,
                                                                                                        CompanyID = ActionRateCode.CompanyID,
                                                                                                        OwnerID = ActionRateCode.OwnerID,
                                                                                                        SiteID = ActionRateCode.SiteID,
                                                                                                        IsPriority = ActionRateCode.IsPriority,
                                                                                                        ActionTypeID = ActionRateCode.ActionTypeID,
                                                                                                        UnitTypeID = ActionRateCode.UnitTypeID,
                                                                                                        StockRateCategoryID = ActionRateCode.StockRateCategoryID,
                                                                                                        WeightBreak = ActionRateCode.WeightBreak,
                                                                                                        LengthBreak = ActionRateCode.LengthBreak,
                                                                                                        RateCollectionRateChargeCode = ActionRateCode.RateCollectionRateChargeCode,
                                                                                                        DeletedFlag = ActionRateCode.DeletedFlag,
                                                                                                        CreatedDateTime = ActionRateCode.CreatedDateTime,
                                                                                                        CreatedMethod = ActionRateCode.CreatedMethod,
                                                                                                        CreatedUserID = ActionRateCode.CreatedUserID,
                                                                                                        LastAmendedDateTime = ActionRateCode.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = ActionRateCode.LastAmendedMethod,
                                                                                                        LastAmendedUserID = ActionRateCode.LastAmendedUserID,

                                                                                                        Site_PublicID = SPR.PublicID,
                                                                                                        Company_ShortName = CPR.ShortName,
                                                                                                        Owner_Name = OPR.Name,
                                                                                                        ActionType_PublicID = ATPR.PublicID,
                                                                                                        UnitType_PublicID = UTPR.PublicID,
                                                                                                        StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                                        Created_UserPublicID = SU.PublicID
                                                                                                    }).ToListAsync();

                if (ActionRateCodeDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>(new List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>(ActionRateCodeDb);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_ListBySingleOwnerID - " + OwnerID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>> ActionRateCode_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ActionRateCode.dbRowExtended ActionRateCodeDb = await (from ActionRateCode in _dbContext.ActionRateCode.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).Take(1)
                                                                                              join site in _dbContext.Site.AsNoTracking()
                                                                                                  on ActionRateCode.SiteID equals site.ID into sitePR
                                                                                              from SPR in sitePR.DefaultIfEmpty()
                                                                                              join company in _dbContext.Company.AsNoTracking()
                                                                                    on ActionRateCode.CompanyID equals company.ID into companyPR
                                                                                              from CPR in companyPR.DefaultIfEmpty()
                                                                                              join Owner in _dbContext.Owner.AsNoTracking()
                                                                                    on ActionRateCode.OwnerID equals Owner.ID into OwnerPR
                                                                                              from OPR in OwnerPR.DefaultIfEmpty()
                                                                                              join ActionType in _dbContext.ActionType.AsNoTracking()
                                                                                    on ActionRateCode.ActionTypeID equals ActionType.ID into ActionTypePR
                                                                                              from ATPR in ActionTypePR.DefaultIfEmpty()
                                                                                              join UnitType in _dbContext.UnitType.AsNoTracking()
                                                                                    on ActionRateCode.UnitTypeID equals UnitType.ID into UnitTypePR
                                                                                              from UTPR in UnitTypePR.DefaultIfEmpty()
                                                                                              join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                    on ActionRateCode.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                              from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                              join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                    on ActionRateCode.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                              from SRCPR in StockRateCategoryPR.DefaultIfEmpty()

                                                                                              select new ActionRateCode.dbRowExtended
                                                                                              {
                                                                                                  ID = ActionRateCode.ID,
                                                                                                  CompanyID = ActionRateCode.CompanyID,
                                                                                                  OwnerID = ActionRateCode.OwnerID,
                                                                                                  SiteID = ActionRateCode.SiteID,
                                                                                                  IsPriority = ActionRateCode.IsPriority,
                                                                                                  ActionTypeID = ActionRateCode.ActionTypeID,
                                                                                                  UnitTypeID = ActionRateCode.UnitTypeID,
                                                                                                  StockRateCategoryID = ActionRateCode.StockRateCategoryID,
                                                                                                  WeightBreak = ActionRateCode.WeightBreak,
                                                                                                  LengthBreak = ActionRateCode.LengthBreak,
                                                                                                  RateCollectionRateChargeCode = ActionRateCode.RateCollectionRateChargeCode,
                                                                                                  DeletedFlag = ActionRateCode.DeletedFlag,
                                                                                                  CreatedDateTime = ActionRateCode.CreatedDateTime,
                                                                                                  CreatedMethod = ActionRateCode.CreatedMethod,
                                                                                                  CreatedUserID = ActionRateCode.CreatedUserID,
                                                                                                  LastAmendedDateTime = ActionRateCode.LastAmendedDateTime,
                                                                                                  LastAmendedMethod = ActionRateCode.LastAmendedMethod,
                                                                                                  LastAmendedUserID = ActionRateCode.LastAmendedUserID,

                                                                                                  Site_PublicID = SPR.PublicID,
                                                                                                  Company_ShortName = CPR.ShortName,
                                                                                                  Owner_Name = OPR.Name,
                                                                                                  ActionType_PublicID = ATPR.PublicID,
                                                                                                  UnitType_PublicID = UTPR.PublicID,
                                                                                                  StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                                  Created_UserPublicID = SU.PublicID
                                                                                              }).FirstOrDefaultAsync();

                if (ActionRateCodeDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>(new List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>(new List<wbm_common.DataObjects.ActionRateCode.dbRowExtended> { ActionRateCodeDb });
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ActionRateCode.dbRow>> ActionRateCode_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ActionRateCode.dbRow ActionRateCodeDb = await _dbContext.ActionRateCode.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (ActionRateCodeDb == null)
                    return Result.Failure<wbm_common.DataObjects.ActionRateCode.dbRow>("ActionRateCode not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ActionRateCode.dbRow>(ActionRateCodeDb);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ActionRateCode.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>> ActionRateCode_List()
        {
            try
            {
                List<wbm_common.DataObjects.ActionRateCode.dbRowExtended> ActionRateCodes = await (from ActionRateCode in _dbContext.ActionRateCode.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                   join site in _dbContext.Site.AsNoTracking()
                                                                                                       on ActionRateCode.SiteID equals site.ID into sitePR
                                                                                                   from SPR in sitePR.DefaultIfEmpty()
                                                                                                   join company in _dbContext.Company.AsNoTracking()
                                                                                         on ActionRateCode.CompanyID equals company.ID into companyPR
                                                                                                   from CPR in companyPR.DefaultIfEmpty()
                                                                                                   join Owner in _dbContext.Owner.AsNoTracking()
                                                                                         on ActionRateCode.OwnerID equals Owner.ID into OwnerPR
                                                                                                   from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                   join ActionType in _dbContext.ActionType.AsNoTracking()
                                                                                         on ActionRateCode.ActionTypeID equals ActionType.ID into ActionTypePR
                                                                                                   from ATPR in ActionTypePR.DefaultIfEmpty()
                                                                                                   join UnitType in _dbContext.UnitType.AsNoTracking()
                                                                                         on ActionRateCode.UnitTypeID equals UnitType.ID into UnitTypePR
                                                                                                   from UTPR in UnitTypePR.DefaultIfEmpty()
                                                                                                   join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                         on ActionRateCode.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                   from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                                   join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                         on ActionRateCode.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                                   from SRCPR in StockRateCategoryPR.DefaultIfEmpty()

                                                                                                   select new ActionRateCode.dbRowExtended
                                                                                                   {
                                                                                                       ID = ActionRateCode.ID,
                                                                                                       CompanyID = ActionRateCode.CompanyID,
                                                                                                       OwnerID = ActionRateCode.OwnerID,
                                                                                                       SiteID = ActionRateCode.SiteID,
                                                                                                       IsPriority = ActionRateCode.IsPriority,
                                                                                                       ActionTypeID = ActionRateCode.ActionTypeID,
                                                                                                       UnitTypeID = ActionRateCode.UnitTypeID,
                                                                                                       StockRateCategoryID = ActionRateCode.StockRateCategoryID,
                                                                                                       WeightBreak = ActionRateCode.WeightBreak,
                                                                                                       LengthBreak = ActionRateCode.LengthBreak,
                                                                                                       RateCollectionRateChargeCode = ActionRateCode.RateCollectionRateChargeCode,
                                                                                                       DeletedFlag = ActionRateCode.DeletedFlag,
                                                                                                       CreatedDateTime = ActionRateCode.CreatedDateTime,
                                                                                                       CreatedMethod = ActionRateCode.CreatedMethod,
                                                                                                       CreatedUserID = ActionRateCode.CreatedUserID,
                                                                                                       LastAmendedDateTime = ActionRateCode.LastAmendedDateTime,
                                                                                                       LastAmendedMethod = ActionRateCode.LastAmendedMethod,
                                                                                                       LastAmendedUserID = ActionRateCode.LastAmendedUserID,

                                                                                                       Site_PublicID = SPR.PublicID,
                                                                                                       Company_ShortName = CPR.ShortName,
                                                                                                       Owner_Name = OPR.Name,
                                                                                                       ActionType_PublicID = ATPR.PublicID,
                                                                                                       UnitType_PublicID = UTPR.PublicID,
                                                                                                       StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                                       Created_UserPublicID = SU.PublicID
                                                                                                   }).Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>(ActionRateCodes);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>> ActionRateCode_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ActionRateCode.dbRowExtended> ActionRateCodes = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.ActionRateCode.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>(ActionRateCodes);
            }
            catch (Exception ex)
            {
                logger.Error("ActionRateCode_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ActionRateCode.dbRowExtended>>("Internal Exception occured");
            }
        }
    }
}
