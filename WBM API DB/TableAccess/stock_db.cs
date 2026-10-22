using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region Stock
        public async Task<Result<int>> Stock_Insert(wbm_common.DataObjects.Stock.dbRow Stock)
        {
            try
            {
                await _dbContext.Stock.AddAsync(Stock);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Stock.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_Insert - " + Stock.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Stock_Update(wbm_common.DataObjects.Stock.dbRow Stock)
        {
            try
            {
                _dbContext.Stock.Attach(Stock);
                _dbContext.Entry(Stock).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Stock.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_Update - " + Stock.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // TODO is category ProcessCategory or security item
        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> Stock_ListByPaperlessKeyID(string PaperlessKeyID)
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRowExtended> StockDb = await (from Stock in _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.Paperless_KeyID == PaperlessKeyID)
                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                      on Stock.SiteID equals site.ID into sitePR
                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                      on Stock.CompanyID equals company.ID into companyPR
                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                      on Stock.OwnerID equals Owner.ID into OwnerPR
                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                  join Category in _dbContext.ProcessCategory.AsNoTracking()
                                                                                      on Stock.CategoryID equals Category.ID into CategoryPR
                                                                                  from CatPR in CategoryPR.DefaultIfEmpty()
                                                                                  join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                      on Stock.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                  from SRCPR in StockRateCategoryPR.DefaultIfEmpty()
                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                      on Stock.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                  from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                  join PalletStorageType in _dbContext.PalletStorageType.AsNoTracking()
                                                                                      on Stock.PalletStorageTypeID equals PalletStorageType.ID into PalletStorageTypePR
                                                                                  from PSTPR in PalletStorageTypePR.DefaultIfEmpty()

                                                                                  select new wbm_common.DataObjects.Stock.dbRowExtended
                                                                                  {
                                                                                      ID = Stock.ID,
                                                                                      CompanyID = Stock.CompanyID,
                                                                                      OwnerID = Stock.OwnerID,
                                                                                      SiteID = Stock.SiteID,
                                                                                      ProductCode = Stock.ProductCode,
                                                                                      Description = Stock.Description,
                                                                                      CategoryID = Stock.CategoryID,
                                                                                      Length = Stock.Length,
                                                                                      Width = Stock.Width,
                                                                                      Height = Stock.Height,
                                                                                      Weight = Stock.Weight,
                                                                                      PalletStorageTypeID = Stock.PalletStorageTypeID,
                                                                                      StockRateCategoryID = Stock.StockRateCategoryID,
                                                                                      ItemsPerPallet = Stock.ItemsPerPallet,
                                                                                      ItemsPerLayer = Stock.ItemsPerLayer,
                                                                                      LayerPickEligible = Stock.LayerPickEligible,
                                                                                      Paperless_KeyID = Stock.Paperless_KeyID,
                                                                                      DeletedFlag = Stock.DeletedFlag,
                                                                                      CreatedDateTime = Stock.CreatedDateTime,
                                                                                      CreatedMethod = Stock.CreatedMethod,
                                                                                      CreatedUserID = Stock.CreatedUserID,
                                                                                      LastAmendedDateTime = Stock.LastAmendedDateTime,
                                                                                      LastAmendedMethod = Stock.LastAmendedMethod,
                                                                                      LastAmendedUserID = Stock.LastAmendedUserID,

                                                                                      Site_PublicID = SPR.PublicID,
                                                                                      Company_ShortName = CPR.ShortName,
                                                                                      Owner_Name = OPR.Name,
                                                                                      CategoryID_PublicID = CatPR.PublicID,
                                                                                      StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                      PalletStorageType_PublicID = PSTPR.PublicID,
                                                                                      Created_UserPublicID = PSTPR.PublicID
                                                                                  }).ToListAsync();

                if (StockDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(new List<wbm_common.DataObjects.Stock.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(StockDb);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListByPaperlessKeyID - " + PaperlessKeyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> Stock_ListID(List<int> IDs)
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRowExtended> StockDb = await (from Stock in _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                      on Stock.SiteID equals site.ID into sitePR
                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                      on Stock.CompanyID equals company.ID into companyPR
                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                      on Stock.OwnerID equals Owner.ID into OwnerPR
                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                  join Category in _dbContext.ProcessCategory.AsNoTracking()
                                                                                      on Stock.CategoryID equals Category.ID into CategoryPR
                                                                                  from CatPR in CategoryPR.DefaultIfEmpty()
                                                                                  join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                      on Stock.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                  from SRCPR in StockRateCategoryPR.DefaultIfEmpty()
                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                      on Stock.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                  from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                  join PalletStorageType in _dbContext.PalletStorageType.AsNoTracking()
                                                                                      on Stock.PalletStorageTypeID equals PalletStorageType.ID into PalletStorageTypePR
                                                                                  from PSTPR in PalletStorageTypePR.DefaultIfEmpty()

                                                                                  select new wbm_common.DataObjects.Stock.dbRowExtended
                                                                                  {
                                                                                      ID = Stock.ID,
                                                                                      CompanyID = Stock.CompanyID,
                                                                                      OwnerID = Stock.OwnerID,
                                                                                      SiteID = Stock.SiteID,
                                                                                      ProductCode = Stock.ProductCode,
                                                                                      Description = Stock.Description,
                                                                                      CategoryID = Stock.CategoryID,
                                                                                      Length = Stock.Length,
                                                                                      Width = Stock.Width,
                                                                                      Height = Stock.Height,
                                                                                      Weight = Stock.Weight,
                                                                                      PalletStorageTypeID = Stock.PalletStorageTypeID,
                                                                                      StockRateCategoryID = Stock.StockRateCategoryID,
                                                                                      ItemsPerPallet = Stock.ItemsPerPallet,
                                                                                      ItemsPerLayer = Stock.ItemsPerLayer,
                                                                                      LayerPickEligible = Stock.LayerPickEligible,
                                                                                      Paperless_KeyID = Stock.Paperless_KeyID,
                                                                                      DeletedFlag = Stock.DeletedFlag,
                                                                                      CreatedDateTime = Stock.CreatedDateTime,
                                                                                      CreatedMethod = Stock.CreatedMethod,
                                                                                      CreatedUserID = Stock.CreatedUserID,
                                                                                      LastAmendedDateTime = Stock.LastAmendedDateTime,
                                                                                      LastAmendedMethod = Stock.LastAmendedMethod,
                                                                                      LastAmendedUserID = Stock.LastAmendedUserID,

                                                                                      Site_PublicID = SPR.PublicID,
                                                                                      Company_ShortName = CPR.ShortName,
                                                                                      Owner_Name = OPR.Name,
                                                                                      CategoryID_PublicID = CatPR.PublicID,
                                                                                      StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                      PalletStorageType_PublicID = PSTPR.PublicID,
                                                                                      Created_UserPublicID = SU.PublicID
                                                                                  }).ToListAsync();

                if (StockDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(new List<wbm_common.DataObjects.Stock.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(StockDb);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListID - " + string.Join(',', IDs.Select(x => x.ToString())) + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> Stock_ListByOwnerIDListProductCode(List<string> ProductCodes, int OwnerID)
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRowExtended> StockDb = await (from Stock in _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.OwnerID == OwnerID && ProductCodes.Contains(x.ProductCode))
                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                      on Stock.SiteID equals site.ID into sitePR 
                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                      on Stock.CompanyID equals company.ID into companyPR 
                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                      on Stock.OwnerID equals Owner.ID into OwnerPR 
                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                  join Category in _dbContext.ProcessCategory.AsNoTracking()
                                                                                      on Stock.CategoryID equals Category.ID into CategoryPR 
                                                                                  from CatPR in CategoryPR.DefaultIfEmpty()
                                                                                  join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                      on Stock.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR 
                                                                                  from SRCPR in StockRateCategoryPR.DefaultIfEmpty()
                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                     on Stock.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                  from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                  join PalletStorageType in _dbContext.PalletStorageType.AsNoTracking()
                                                                                      on Stock.PalletStorageTypeID equals PalletStorageType.ID into PalletStorageTypePR 
                                                                                  from PSTPR in PalletStorageTypePR.DefaultIfEmpty()

                                                                                  select new wbm_common.DataObjects.Stock.dbRowExtended
                                                                                  {
                                                                                      ID = Stock.ID,
                                                                                      CompanyID = Stock.CompanyID,
                                                                                      OwnerID = Stock.OwnerID,
                                                                                      SiteID = Stock.SiteID,
                                                                                      ProductCode = Stock.ProductCode,
                                                                                      Description = Stock.Description,
                                                                                      CategoryID = Stock.CategoryID,
                                                                                      Length = Stock.Length,
                                                                                      Width = Stock.Width,
                                                                                      Height = Stock.Height,
                                                                                      Weight = Stock.Weight,
                                                                                      PalletStorageTypeID = Stock.PalletStorageTypeID,
                                                                                      StockRateCategoryID = Stock.StockRateCategoryID,
                                                                                      ItemsPerPallet = Stock.ItemsPerPallet,
                                                                                      ItemsPerLayer = Stock.ItemsPerLayer,
                                                                                      LayerPickEligible = Stock.LayerPickEligible,
                                                                                      Paperless_KeyID = Stock.Paperless_KeyID,
                                                                                      DeletedFlag = Stock.DeletedFlag,
                                                                                      CreatedDateTime = Stock.CreatedDateTime,
                                                                                      CreatedMethod = Stock.CreatedMethod,
                                                                                      CreatedUserID = Stock.CreatedUserID,
                                                                                      LastAmendedDateTime = Stock.LastAmendedDateTime,
                                                                                      LastAmendedMethod = Stock.LastAmendedMethod,
                                                                                      LastAmendedUserID = Stock.LastAmendedUserID,

                                                                                      Site_PublicID = SPR.PublicID,
                                                                                      Company_ShortName = CPR.ShortName,
                                                                                      Owner_Name = OPR.Name,
                                                                                      CategoryID_PublicID = CatPR.PublicID,
                                                                                      StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                      PalletStorageType_PublicID = PSTPR.PublicID,
                                                                                      Created_UserPublicID = SU.PublicID
                                                                                  }).ToListAsync();

                if (StockDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(new List<wbm_common.DataObjects.Stock.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(StockDb);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListByOwnerIDListProductCode - " + string.Join(',', ProductCodes) + " and OwnerID " + OwnerID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> Stock_ListBySingleSiteID(int SiteID)
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRowExtended> StockDb = await (from Stock in _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.SiteID == SiteID || x.SiteID == 0))
                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                      on Stock.SiteID equals site.ID into sitePR
                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                      on Stock.CompanyID equals company.ID into companyPR
                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                      on Stock.OwnerID equals Owner.ID into OwnerPR
                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                  join Category in _dbContext.ProcessCategory.AsNoTracking()
                                                                                      on Stock.CategoryID equals Category.ID into CategoryPR
                                                                                  from CatPR in CategoryPR.DefaultIfEmpty()
                                                                                  join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                      on Stock.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                  from SRCPR in StockRateCategoryPR.DefaultIfEmpty()
                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                      on Stock.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                  from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                  join PalletStorageType in _dbContext.PalletStorageType.AsNoTracking()
                                                                                      on Stock.PalletStorageTypeID equals PalletStorageType.ID into PalletStorageTypePR
                                                                                  from PSTPR in PalletStorageTypePR.DefaultIfEmpty()

                                                                                  select new wbm_common.DataObjects.Stock.dbRowExtended
                                                                                  {
                                                                                      ID = Stock.ID,
                                                                                      CompanyID = Stock.CompanyID,
                                                                                      OwnerID = Stock.OwnerID,
                                                                                      SiteID = Stock.SiteID,
                                                                                      ProductCode = Stock.ProductCode,
                                                                                      Description = Stock.Description,
                                                                                      CategoryID = Stock.CategoryID,
                                                                                      Length = Stock.Length,
                                                                                      Width = Stock.Width,
                                                                                      Height = Stock.Height,
                                                                                      Weight = Stock.Weight,
                                                                                      PalletStorageTypeID = Stock.PalletStorageTypeID,
                                                                                      StockRateCategoryID = Stock.StockRateCategoryID,
                                                                                      ItemsPerPallet = Stock.ItemsPerPallet,
                                                                                      ItemsPerLayer = Stock.ItemsPerLayer,
                                                                                      LayerPickEligible = Stock.LayerPickEligible,
                                                                                      Paperless_KeyID = Stock.Paperless_KeyID,
                                                                                      DeletedFlag = Stock.DeletedFlag,
                                                                                      CreatedDateTime = Stock.CreatedDateTime,
                                                                                      CreatedMethod = Stock.CreatedMethod,
                                                                                      CreatedUserID = Stock.CreatedUserID,
                                                                                      LastAmendedDateTime = Stock.LastAmendedDateTime,
                                                                                      LastAmendedMethod = Stock.LastAmendedMethod,
                                                                                      LastAmendedUserID = Stock.LastAmendedUserID,

                                                                                      Site_PublicID = SPR.PublicID,
                                                                                      Company_ShortName = CPR.ShortName,
                                                                                      Owner_Name = OPR.Name,
                                                                                      CategoryID_PublicID = CatPR.PublicID,
                                                                                      StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                      PalletStorageType_PublicID = PSTPR.PublicID,
                                                                                      Created_UserPublicID = SU.PublicID
                                                                                  }).ToListAsync();

                if (StockDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(new List<wbm_common.DataObjects.Stock.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(StockDb);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListBySingleSiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> Stock_ListBySingleOwnerID(int OwnerID)
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRowExtended> StockDb = await (from Stock in _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.OwnerID == OwnerID || x.OwnerID == 0))
                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                      on Stock.SiteID equals site.ID into sitePR
                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                      on Stock.CompanyID equals company.ID into companyPR
                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                      on Stock.OwnerID equals Owner.ID into OwnerPR
                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                  join Category in _dbContext.ProcessCategory.AsNoTracking()
                                                                                      on Stock.CategoryID equals Category.ID into CategoryPR
                                                                                  from CatPR in CategoryPR.DefaultIfEmpty()
                                                                                  join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                      on Stock.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                  from SRCPR in StockRateCategoryPR.DefaultIfEmpty()
                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                      on Stock.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                  from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                  join PalletStorageType in _dbContext.PalletStorageType.AsNoTracking()
                                                                                      on Stock.PalletStorageTypeID equals PalletStorageType.ID into PalletStorageTypePR
                                                                                  from PSTPR in PalletStorageTypePR.DefaultIfEmpty()

                                                                                  select new wbm_common.DataObjects.Stock.dbRowExtended
                                                                                  {
                                                                                      ID = Stock.ID,
                                                                                      CompanyID = Stock.CompanyID,
                                                                                      OwnerID = Stock.OwnerID,
                                                                                      SiteID = Stock.SiteID,
                                                                                      ProductCode = Stock.ProductCode,
                                                                                      Description = Stock.Description,
                                                                                      CategoryID = Stock.CategoryID,
                                                                                      Length = Stock.Length,
                                                                                      Width = Stock.Width,
                                                                                      Height = Stock.Height,
                                                                                      Weight = Stock.Weight,
                                                                                      PalletStorageTypeID = Stock.PalletStorageTypeID,
                                                                                      StockRateCategoryID = Stock.StockRateCategoryID,
                                                                                      ItemsPerPallet = Stock.ItemsPerPallet,
                                                                                      ItemsPerLayer = Stock.ItemsPerLayer,
                                                                                      LayerPickEligible = Stock.LayerPickEligible,
                                                                                      Paperless_KeyID = Stock.Paperless_KeyID,
                                                                                      DeletedFlag = Stock.DeletedFlag,
                                                                                      CreatedDateTime = Stock.CreatedDateTime,
                                                                                      CreatedMethod = Stock.CreatedMethod,
                                                                                      CreatedUserID = Stock.CreatedUserID,
                                                                                      LastAmendedDateTime = Stock.LastAmendedDateTime,
                                                                                      LastAmendedMethod = Stock.LastAmendedMethod,
                                                                                      LastAmendedUserID = Stock.LastAmendedUserID,

                                                                                      Site_PublicID = SPR.PublicID,
                                                                                      Company_ShortName = CPR.ShortName,
                                                                                      Owner_Name = OPR.Name,
                                                                                      CategoryID_PublicID = CatPR.PublicID,
                                                                                      StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                      PalletStorageType_PublicID = PSTPR.PublicID,
                                                                                      Created_UserPublicID = SU.PublicID
                                                                                  }).ToListAsync();

                if (StockDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(new List<wbm_common.DataObjects.Stock.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(StockDb);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListBySingleOwnerID - " + OwnerID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Stock.dbRow>> Stock_SearchByProductCode(string ProductCode)
        {
            try
            {
                wbm_common.DataObjects.Stock.dbRow StockDb = await _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ProductCode == ProductCode).FirstOrDefaultAsync();

                if (StockDb == null)
                    return Result.Failure<wbm_common.DataObjects.Stock.dbRow>("Stock not found : " + ProductCode);
                else
                    return Result.Success<wbm_common.DataObjects.Stock.dbRow>(StockDb);

            }
            catch (Exception ex)
            {
                logger.Error("Stock_SearchByProductCode - " + ProductCode + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Stock.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> Stock_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRowExtended> StockDb = await (from Stock in _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                      on Stock.SiteID equals site.ID into sitePR
                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                      on Stock.CompanyID equals company.ID into companyPR
                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                      on Stock.OwnerID equals Owner.ID into OwnerPR
                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                  join Category in _dbContext.ProcessCategory.AsNoTracking()
                                                                                      on Stock.CategoryID equals Category.ID into CategoryPR
                                                                                  from CatPR in CategoryPR.DefaultIfEmpty()
                                                                                  join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                      on Stock.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                  from SRCPR in StockRateCategoryPR.DefaultIfEmpty()
                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                      on Stock.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                  from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                  join PalletStorageType in _dbContext.PalletStorageType.AsNoTracking()
                                                                                      on Stock.PalletStorageTypeID equals PalletStorageType.ID into PalletStorageTypePR
                                                                                  from PSTPR in PalletStorageTypePR.DefaultIfEmpty()

                                                                                  select new wbm_common.DataObjects.Stock.dbRowExtended
                                                                                  {
                                                                                      ID = Stock.ID,
                                                                                      CompanyID = Stock.CompanyID,
                                                                                      OwnerID = Stock.OwnerID,
                                                                                      SiteID = Stock.SiteID,
                                                                                      ProductCode = Stock.ProductCode,
                                                                                      Description = Stock.Description,
                                                                                      CategoryID = Stock.CategoryID,
                                                                                      Length = Stock.Length,
                                                                                      Width = Stock.Width,
                                                                                      Height = Stock.Height,
                                                                                      Weight = Stock.Weight,
                                                                                      PalletStorageTypeID = Stock.PalletStorageTypeID,
                                                                                      StockRateCategoryID = Stock.StockRateCategoryID,
                                                                                      ItemsPerPallet = Stock.ItemsPerPallet,
                                                                                      ItemsPerLayer = Stock.ItemsPerLayer,
                                                                                      LayerPickEligible = Stock.LayerPickEligible,
                                                                                      Paperless_KeyID = Stock.Paperless_KeyID,
                                                                                      DeletedFlag = Stock.DeletedFlag,
                                                                                      CreatedDateTime = Stock.CreatedDateTime,
                                                                                      CreatedMethod = Stock.CreatedMethod,
                                                                                      CreatedUserID = Stock.CreatedUserID,
                                                                                      LastAmendedDateTime = Stock.LastAmendedDateTime,
                                                                                      LastAmendedMethod = Stock.LastAmendedMethod,
                                                                                      LastAmendedUserID = Stock.LastAmendedUserID,

                                                                                      Site_PublicID = SPR.PublicID,
                                                                                      Company_ShortName = CPR.ShortName,
                                                                                      Owner_Name = OPR.Name,
                                                                                      CategoryID_PublicID = CatPR.PublicID,
                                                                                      StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                      PalletStorageType_PublicID = PSTPR.PublicID,
                                                                                      Created_UserPublicID = SU.PublicID
                                                                                  }).ToListAsync();

                if (StockDb == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(new List<wbm_common.DataObjects.Stock.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(StockDb);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Stock.dbRow>> Stock_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Stock.dbRow StockDb = await _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (StockDb == null)
                    return Result.Failure<wbm_common.DataObjects.Stock.dbRow>("Stock not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Stock.dbRow>(StockDb);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Stock.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> Stock_List()
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRowExtended> Stocks = await (from Stock in _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                 join site in _dbContext.Site.AsNoTracking()
                                                                                     on Stock.SiteID equals site.ID into sitePR
                                                                                 from SPR in sitePR.DefaultIfEmpty()
                                                                                 join company in _dbContext.Company.AsNoTracking()
                                                                                     on Stock.CompanyID equals company.ID into companyPR
                                                                                 from CPR in companyPR.DefaultIfEmpty()
                                                                                 join Owner in _dbContext.Owner.AsNoTracking()
                                                                                     on Stock.OwnerID equals Owner.ID into OwnerPR
                                                                                 from OPR in OwnerPR.DefaultIfEmpty()
                                                                                 join Category in _dbContext.ProcessCategory.AsNoTracking()
                                                                                     on Stock.CategoryID equals Category.ID into CategoryPR
                                                                                 from CatPR in CategoryPR.DefaultIfEmpty()
                                                                                 join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                     on Stock.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                 from SRCPR in StockRateCategoryPR.DefaultIfEmpty()
                                                                                 join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                     on Stock.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                 from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                 join PalletStorageType in _dbContext.PalletStorageType.AsNoTracking()
                                                                                     on Stock.PalletStorageTypeID equals PalletStorageType.ID into PalletStorageTypePR
                                                                                 from PSTPR in PalletStorageTypePR.DefaultIfEmpty()

                                                                                 select new wbm_common.DataObjects.Stock.dbRowExtended
                                                                                 {
                                                                                     ID = Stock.ID,
                                                                                     CompanyID = Stock.CompanyID,
                                                                                     OwnerID = Stock.OwnerID,
                                                                                     SiteID = Stock.SiteID,
                                                                                     ProductCode = Stock.ProductCode,
                                                                                     Description = Stock.Description,
                                                                                     CategoryID = Stock.CategoryID,
                                                                                     Length = Stock.Length,
                                                                                     Width = Stock.Width,
                                                                                     Height = Stock.Height,
                                                                                     Weight = Stock.Weight,
                                                                                     PalletStorageTypeID = Stock.PalletStorageTypeID,
                                                                                     StockRateCategoryID = Stock.StockRateCategoryID,
                                                                                     ItemsPerPallet = Stock.ItemsPerPallet,
                                                                                     ItemsPerLayer = Stock.ItemsPerLayer,
                                                                                     LayerPickEligible = Stock.LayerPickEligible,
                                                                                     Paperless_KeyID = Stock.Paperless_KeyID,
                                                                                     DeletedFlag = Stock.DeletedFlag,
                                                                                     CreatedDateTime = Stock.CreatedDateTime,
                                                                                     CreatedMethod = Stock.CreatedMethod,
                                                                                     CreatedUserID = Stock.CreatedUserID,
                                                                                     LastAmendedDateTime = Stock.LastAmendedDateTime,
                                                                                     LastAmendedMethod = Stock.LastAmendedMethod,
                                                                                     LastAmendedUserID = Stock.LastAmendedUserID,

                                                                                     Site_PublicID = SPR.PublicID,
                                                                                     Company_ShortName = CPR.ShortName,
                                                                                     Owner_Name = OPR.Name,
                                                                                     CategoryID_PublicID = CatPR.PublicID,
                                                                                     StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                     PalletStorageType_PublicID = PSTPR.PublicID,
                                                                                     Created_UserPublicID = SU.PublicID
                                                                                 }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(Stocks);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> Stock_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Stock_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Stock.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Stock.dbRowExtended> Stocks = await (from Stock in _dbContext.Stock.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                     join site in _dbContext.Site.AsNoTracking()
                                                                                         on Stock.SiteID equals site.ID into sitePR
                                                                                     from SPR in sitePR.DefaultIfEmpty()
                                                                                     join company in _dbContext.Company.AsNoTracking()
                                                                                         on Stock.CompanyID equals company.ID into companyPR
                                                                                     from CPR in companyPR.DefaultIfEmpty()
                                                                                     join Owner in _dbContext.Owner.AsNoTracking()
                                                                                         on Stock.OwnerID equals Owner.ID into OwnerPR
                                                                                     from OPR in OwnerPR.DefaultIfEmpty()
                                                                                     join Category in _dbContext.ProcessCategory.AsNoTracking()
                                                                                         on Stock.CategoryID equals Category.ID into CategoryPR
                                                                                     from CatPR in CategoryPR.DefaultIfEmpty()
                                                                                     join StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                                         on Stock.StockRateCategoryID equals StockRateCategory.ID into StockRateCategoryPR
                                                                                     from SRCPR in StockRateCategoryPR.DefaultIfEmpty()
                                                                                     join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                         on Stock.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                     from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                     join PalletStorageType in _dbContext.PalletStorageType.AsNoTracking()
                                                                                         on Stock.PalletStorageTypeID equals PalletStorageType.ID into PalletStorageTypePR
                                                                                     from PSTPR in PalletStorageTypePR.DefaultIfEmpty()

                                                                                     select new wbm_common.DataObjects.Stock.dbRowExtended
                                                                                     {
                                                                                         ID = Stock.ID,
                                                                                         CompanyID = Stock.CompanyID,
                                                                                         OwnerID = Stock.OwnerID,
                                                                                         SiteID = Stock.SiteID,
                                                                                         ProductCode = Stock.ProductCode,
                                                                                         Description = Stock.Description,
                                                                                         CategoryID = Stock.CategoryID,
                                                                                         Length = Stock.Length,
                                                                                         Width = Stock.Width,
                                                                                         Height = Stock.Height,
                                                                                         Weight = Stock.Weight,
                                                                                         PalletStorageTypeID = Stock.PalletStorageTypeID,
                                                                                         StockRateCategoryID = Stock.StockRateCategoryID,
                                                                                         ItemsPerPallet = Stock.ItemsPerPallet,
                                                                                         ItemsPerLayer = Stock.ItemsPerLayer,
                                                                                         LayerPickEligible = Stock.LayerPickEligible,
                                                                                         Paperless_KeyID = Stock.Paperless_KeyID,
                                                                                         DeletedFlag = Stock.DeletedFlag,
                                                                                         CreatedDateTime = Stock.CreatedDateTime,
                                                                                         CreatedMethod = Stock.CreatedMethod,
                                                                                         CreatedUserID = Stock.CreatedUserID,
                                                                                         LastAmendedDateTime = Stock.LastAmendedDateTime,
                                                                                         LastAmendedMethod = Stock.LastAmendedMethod,
                                                                                         LastAmendedUserID = Stock.LastAmendedUserID,

                                                                                         Site_PublicID = SPR.PublicID,
                                                                                         Company_ShortName = CPR.ShortName,
                                                                                         Owner_Name = OPR.Name,
                                                                                         CategoryID_PublicID = CatPR.PublicID,
                                                                                         StockRateCategory_PublicID = SRCPR.PublicID,
                                                                                         PalletStorageType_PublicID = PSTPR.PublicID,
                                                                                         Created_UserPublicID = SU.PublicID
                                                                                     }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(Stocks);
                }
                catch (Exception ex)
                {
                    logger.Error("Stock_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> Stock_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRowExtended> Stocks = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.Stock.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Stock.dbRowExtended>>(Stocks);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion

        #region StockOnHand
        public async Task<Result<int>> StockOnHand_Insert(wbm_common.DataObjects.StockOnHand.dbRow StockOnHand)
        {
            try
            {
                await _dbContext.StockOnHand.AddAsync(StockOnHand);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StockOnHand.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_Insert - " + StockOnHand.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> StockOnHand_Update(wbm_common.DataObjects.StockOnHand.dbRow StockOnHand)
        {
            try
            {
                _dbContext.StockOnHand.Attach(StockOnHand);
                _dbContext.Entry(StockOnHand).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StockOnHand.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_Update - " + StockOnHand.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>> StockOnHand_ListBySingleSiteID(int SiteID)
        {
            try
            {
                List<wbm_common.DataObjects.StockOnHand.dbRowExtended> StockOnHandDb = await (from StockOnHand in _dbContext.StockOnHand.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.SiteID == SiteID || x.SiteID == 0))
                                                                                              join site in _dbContext.Site.AsNoTracking()
                                                                                                  on StockOnHand.SiteID equals site.ID into sitePR
                                                                                              from SPR in sitePR.DefaultIfEmpty()
                                                                                              join company in _dbContext.Company.AsNoTracking()
                                                                                          on StockOnHand.CompanyID equals company.ID into companyPR
                                                                                              from CPR in companyPR.DefaultIfEmpty()
                                                                                              join Owner in _dbContext.Owner.AsNoTracking()
                                                                                          on StockOnHand.OwnerID equals Owner.ID into OwnerPR
                                                                                              from OPR in OwnerPR.DefaultIfEmpty()
                                                                                              join Stock in _dbContext.Stock.AsNoTracking()
                                                                                          on StockOnHand.StockID equals Stock.ID into StockPR
                                                                                              from StkPR in StockPR.DefaultIfEmpty()
                                                                                              join Location in _dbContext.Location.AsNoTracking()
                                                                                          on StockOnHand.LocationID equals Location.ID into LocationPR
                                                                                              from LPR in LocationPR.DefaultIfEmpty()
                                                                                              join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                          on StockOnHand.RateCodeID equals RateCode.ID into RateCodePR
                                                                                              from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                              join StorageRateCodeFrom in _dbContext.StorageRateCodeFrom.AsNoTracking()
                                                                                          on StockOnHand.StorageRateCodeFromID equals StorageRateCodeFrom.ID into StorageRateCodeFromPR
                                                                                              from SRCPR in StorageRateCodeFromPR.DefaultIfEmpty()
                                                                                              join ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                                                  on StockOnHand.ZoneProcessingID equals ZoneProcessing.ID into ZoneProcessingPR
                                                                                              from ZPPR in ZoneProcessingPR.DefaultIfEmpty()
                                                                                              join PalletType in _dbContext.PalletType.AsNoTracking()
                                                                                                  on StockOnHand.PalletTypeID equals PalletType.ID into PalletTypePR
                                                                                              from PTPR in PalletTypePR.DefaultIfEmpty()
                                                                                              join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                  on StockOnHand.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                              from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                              select new wbm_common.DataObjects.StockOnHand.dbRowExtended
                                                                                              {
                                                                                                  ID = StockOnHand.ID,
                                                                                                  CompanyID = StockOnHand.CompanyID,
                                                                                                  OwnerID = StockOnHand.OwnerID,
                                                                                                  SiteID = StockOnHand.SiteID,
                                                                                                  StockOnHandDate = StockOnHand.StockOnHandDate,
                                                                                                  LocationID = StockOnHand.LocationID,
                                                                                                  StockID = StockOnHand.StockID,
                                                                                                  Quantity = StockOnHand.Quantity,
                                                                                                  RateCodeID = StockOnHand.RateCodeID,
                                                                                                  StorageRateCodeFromID = StockOnHand.StorageRateCodeFromID,
                                                                                                  PalletNumber = StockOnHand.PalletNumber,
                                                                                                  ZoneProcessingID = StockOnHand.ZoneProcessingID,
                                                                                                  Status = StockOnHand.Status,
                                                                                                  UseByDate = StockOnHand.UseByDate,
                                                                                                  LotNumber = StockOnHand.LotNumber,
                                                                                                  PalletWeight = StockOnHand.PalletWeight,
                                                                                                  PalletWidth = StockOnHand.PalletWidth,
                                                                                                  PalletDepth = StockOnHand.PalletDepth,
                                                                                                  PalletHeight = StockOnHand.PalletHeight,
                                                                                                  PalletTypeID = StockOnHand.PalletTypeID,
                                                                                                  DeletedFlag = StockOnHand.DeletedFlag,
                                                                                                  CreatedDateTime = StockOnHand.CreatedDateTime,
                                                                                                  CreatedMethod = StockOnHand.CreatedMethod,
                                                                                                  CreatedUserID = StockOnHand.CreatedUserID,
                                                                                                  LastAmendedDateTime = StockOnHand.LastAmendedDateTime,
                                                                                                  LastAmendedMethod = StockOnHand.LastAmendedMethod,
                                                                                                  LastAmendedUserID = StockOnHand.LastAmendedUserID,

                                                                                                  Site_PublicID = SPR.PublicID,
                                                                                                  Company_ShortName = CPR.ShortName,
                                                                                                  Owner_Name = OPR.Name,
                                                                                                  Stock_ProductCode = StkPR.ProductCode,
                                                                                                  Location_PublicLocationID = LPR.PublicLocationID,
                                                                                                  RateCode_RateCode = RCPR.RateCode,
                                                                                                  StorageRateCodeFrom_PublicID = SRCPR.PublicID,
                                                                                                  ZoneProcessing_ZoneNumber = ZPPR.ZoneNumber,
                                                                                                  PalletType_PublicID = PTPR.PublicID,
                                                                                                  Created_UserPublicID = SU.PublicID
                                                                                              }).ToListAsync();

                if (StockOnHandDb == null)
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>(new List<wbm_common.DataObjects.StockOnHand.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>(StockOnHandDb);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_ListBySingleSiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>> StockOnHand_ListBySingleOwnerID(int OwnerID)
        {
            try
            {
                List<wbm_common.DataObjects.StockOnHand.dbRowExtended> StockOnHandDb = await (from StockOnHand in _dbContext.StockOnHand.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.OwnerID == OwnerID || x.OwnerID == 0))
                                                                                              join site in _dbContext.Site.AsNoTracking()
                                                                                                  on StockOnHand.SiteID equals site.ID into sitePR
                                                                                              from SPR in sitePR.DefaultIfEmpty()
                                                                                              join company in _dbContext.Company.AsNoTracking()
                                                                                          on StockOnHand.CompanyID equals company.ID into companyPR
                                                                                              from CPR in companyPR.DefaultIfEmpty()
                                                                                              join Owner in _dbContext.Owner.AsNoTracking()
                                                                                          on StockOnHand.OwnerID equals Owner.ID into OwnerPR
                                                                                              from OPR in OwnerPR.DefaultIfEmpty()
                                                                                              join Stock in _dbContext.Stock.AsNoTracking()
                                                                                          on StockOnHand.StockID equals Stock.ID into StockPR
                                                                                              from StkPR in StockPR.DefaultIfEmpty()
                                                                                              join Location in _dbContext.Location.AsNoTracking()
                                                                                          on StockOnHand.LocationID equals Location.ID into LocationPR
                                                                                              from LPR in LocationPR.DefaultIfEmpty()
                                                                                              join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                          on StockOnHand.RateCodeID equals RateCode.ID into RateCodePR
                                                                                              from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                              join StorageRateCodeFrom in _dbContext.StorageRateCodeFrom.AsNoTracking()
                                                                                          on StockOnHand.StorageRateCodeFromID equals StorageRateCodeFrom.ID into StorageRateCodeFromPR
                                                                                              from SRCPR in StorageRateCodeFromPR.DefaultIfEmpty()
                                                                                              join ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                                                  on StockOnHand.ZoneProcessingID equals ZoneProcessing.ID into ZoneProcessingPR
                                                                                              from ZPPR in ZoneProcessingPR.DefaultIfEmpty()
                                                                                              join PalletType in _dbContext.PalletType.AsNoTracking()
                                                                                                  on StockOnHand.PalletTypeID equals PalletType.ID into PalletTypePR
                                                                                              from PTPR in PalletTypePR.DefaultIfEmpty()
                                                                                              join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                  on StockOnHand.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                              from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                              select new wbm_common.DataObjects.StockOnHand.dbRowExtended
                                                                                              {
                                                                                                  ID = StockOnHand.ID,
                                                                                                  CompanyID = StockOnHand.CompanyID,
                                                                                                  OwnerID = StockOnHand.OwnerID,
                                                                                                  SiteID = StockOnHand.SiteID,
                                                                                                  StockOnHandDate = StockOnHand.StockOnHandDate,
                                                                                                  LocationID = StockOnHand.LocationID,
                                                                                                  StockID = StockOnHand.StockID,
                                                                                                  Quantity = StockOnHand.Quantity,
                                                                                                  RateCodeID = StockOnHand.RateCodeID,
                                                                                                  StorageRateCodeFromID = StockOnHand.StorageRateCodeFromID,
                                                                                                  PalletNumber = StockOnHand.PalletNumber,
                                                                                                  ZoneProcessingID = StockOnHand.ZoneProcessingID,
                                                                                                  Status = StockOnHand.Status,
                                                                                                  UseByDate = StockOnHand.UseByDate,
                                                                                                  LotNumber = StockOnHand.LotNumber,
                                                                                                  PalletWeight = StockOnHand.PalletWeight,
                                                                                                  PalletWidth = StockOnHand.PalletWidth,
                                                                                                  PalletDepth = StockOnHand.PalletDepth,
                                                                                                  PalletHeight = StockOnHand.PalletHeight,
                                                                                                  PalletTypeID = StockOnHand.PalletTypeID,
                                                                                                  DeletedFlag = StockOnHand.DeletedFlag,
                                                                                                  CreatedDateTime = StockOnHand.CreatedDateTime,
                                                                                                  CreatedMethod = StockOnHand.CreatedMethod,
                                                                                                  CreatedUserID = StockOnHand.CreatedUserID,
                                                                                                  LastAmendedDateTime = StockOnHand.LastAmendedDateTime,
                                                                                                  LastAmendedMethod = StockOnHand.LastAmendedMethod,
                                                                                                  LastAmendedUserID = StockOnHand.LastAmendedUserID,

                                                                                                  Site_PublicID = SPR.PublicID,
                                                                                                  Company_ShortName = CPR.ShortName,
                                                                                                  Owner_Name = OPR.Name,
                                                                                                  Stock_ProductCode = StkPR.ProductCode,
                                                                                                  Location_PublicLocationID = LPR.PublicLocationID,
                                                                                                  RateCode_RateCode = RCPR.RateCode,
                                                                                                  StorageRateCodeFrom_PublicID = SRCPR.PublicID,
                                                                                                  ZoneProcessing_ZoneNumber = ZPPR.ZoneNumber,
                                                                                                  PalletType_PublicID = PTPR.PublicID,
                                                                                                  Created_UserPublicID = SU.PublicID
                                                                                              }).ToListAsync();

                if (StockOnHandDb == null)
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>(new List<wbm_common.DataObjects.StockOnHand.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>(StockOnHandDb);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_ListBySingleOwnerID - " + OwnerID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>> StockOnHand_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.StockOnHand.dbRowExtended> StockOnHandDb = await (from StockOnHand in _dbContext.StockOnHand.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                              join site in _dbContext.Site.AsNoTracking()
                                                                                                  on StockOnHand.SiteID equals site.ID into sitePR
                                                                                              from SPR in sitePR.DefaultIfEmpty()
                                                                                              join company in _dbContext.Company.AsNoTracking()
                                                                                          on StockOnHand.CompanyID equals company.ID into companyPR
                                                                                              from CPR in companyPR.DefaultIfEmpty()
                                                                                              join Owner in _dbContext.Owner.AsNoTracking()
                                                                                          on StockOnHand.OwnerID equals Owner.ID into OwnerPR
                                                                                              from OPR in OwnerPR.DefaultIfEmpty()
                                                                                              join Stock in _dbContext.Stock.AsNoTracking()
                                                                                          on StockOnHand.StockID equals Stock.ID into StockPR
                                                                                              from StkPR in StockPR.DefaultIfEmpty()
                                                                                              join Location in _dbContext.Location.AsNoTracking()
                                                                                          on StockOnHand.LocationID equals Location.ID into LocationPR
                                                                                              from LPR in LocationPR.DefaultIfEmpty()
                                                                                              join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                          on StockOnHand.RateCodeID equals RateCode.ID into RateCodePR
                                                                                              from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                              join StorageRateCodeFrom in _dbContext.StorageRateCodeFrom.AsNoTracking()
                                                                                          on StockOnHand.StorageRateCodeFromID equals StorageRateCodeFrom.ID into StorageRateCodeFromPR
                                                                                              from SRCPR in StorageRateCodeFromPR.DefaultIfEmpty()
                                                                                              join ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                                                  on StockOnHand.ZoneProcessingID equals ZoneProcessing.ID into ZoneProcessingPR
                                                                                              from ZPPR in ZoneProcessingPR.DefaultIfEmpty()
                                                                                              join PalletType in _dbContext.PalletType.AsNoTracking()
                                                                                                  on StockOnHand.PalletTypeID equals PalletType.ID into PalletTypePR
                                                                                              from PTPR in PalletTypePR.DefaultIfEmpty()
                                                                                              join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                  on StockOnHand.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                              from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                              select new wbm_common.DataObjects.StockOnHand.dbRowExtended
                                                                                              {
                                                                                                  ID = StockOnHand.ID,
                                                                                                  CompanyID = StockOnHand.CompanyID,
                                                                                                  OwnerID = StockOnHand.OwnerID,
                                                                                                  SiteID = StockOnHand.SiteID,
                                                                                                  StockOnHandDate = StockOnHand.StockOnHandDate,
                                                                                                  LocationID = StockOnHand.LocationID,
                                                                                                  StockID = StockOnHand.StockID,
                                                                                                  Quantity = StockOnHand.Quantity,
                                                                                                  RateCodeID = StockOnHand.RateCodeID,
                                                                                                  StorageRateCodeFromID = StockOnHand.StorageRateCodeFromID,
                                                                                                  PalletNumber = StockOnHand.PalletNumber,
                                                                                                  ZoneProcessingID = StockOnHand.ZoneProcessingID,
                                                                                                  Status = StockOnHand.Status,
                                                                                                  UseByDate = StockOnHand.UseByDate,
                                                                                                  LotNumber = StockOnHand.LotNumber,
                                                                                                  PalletWeight = StockOnHand.PalletWeight,
                                                                                                  PalletWidth = StockOnHand.PalletWidth,
                                                                                                  PalletDepth = StockOnHand.PalletDepth,
                                                                                                  PalletHeight = StockOnHand.PalletHeight,
                                                                                                  PalletTypeID = StockOnHand.PalletTypeID,
                                                                                                  DeletedFlag = StockOnHand.DeletedFlag,
                                                                                                  CreatedDateTime = StockOnHand.CreatedDateTime,
                                                                                                  CreatedMethod = StockOnHand.CreatedMethod,
                                                                                                  CreatedUserID = StockOnHand.CreatedUserID,
                                                                                                  LastAmendedDateTime = StockOnHand.LastAmendedDateTime,
                                                                                                  LastAmendedMethod = StockOnHand.LastAmendedMethod,
                                                                                                  LastAmendedUserID = StockOnHand.LastAmendedUserID,

                                                                                                  Site_PublicID = SPR.PublicID,
                                                                                                  Company_ShortName = CPR.ShortName,
                                                                                                  Owner_Name = OPR.Name,
                                                                                                  Stock_ProductCode = StkPR.ProductCode,
                                                                                                  Location_PublicLocationID = LPR.PublicLocationID,
                                                                                                  RateCode_RateCode = RCPR.RateCode,
                                                                                                  StorageRateCodeFrom_PublicID = SRCPR.PublicID,
                                                                                                  ZoneProcessing_ZoneNumber = ZPPR.ZoneNumber,
                                                                                                  PalletType_PublicID = PTPR.PublicID,
                                                                                                  Created_UserPublicID = SU.PublicID
                                                                                              }).ToListAsync();

                if (StockOnHandDb == null)
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>(new List<wbm_common.DataObjects.StockOnHand.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>(StockOnHandDb);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.StockOnHand.dbRow>> StockOnHand_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StockOnHand.dbRow StockOnHandDb = await _dbContext.StockOnHand.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (StockOnHandDb == null)
                    return Result.Failure<wbm_common.DataObjects.StockOnHand.dbRow>("StockOnHand not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.StockOnHand.dbRow>(StockOnHandDb);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.StockOnHand.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>> StockOnHand_List()
        {
            try
            {
                List<wbm_common.DataObjects.StockOnHand.dbRowExtended> StockOnHands = await (from StockOnHand in _dbContext.StockOnHand.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                             join site in _dbContext.Site.AsNoTracking()
                                                                                                 on StockOnHand.SiteID equals site.ID into sitePR
                                                                                             from SPR in sitePR.DefaultIfEmpty()
                                                                                             join company in _dbContext.Company.AsNoTracking()
                                                                                         on StockOnHand.CompanyID equals company.ID into companyPR
                                                                                             from CPR in companyPR.DefaultIfEmpty()
                                                                                             join Owner in _dbContext.Owner.AsNoTracking()
                                                                                         on StockOnHand.OwnerID equals Owner.ID into OwnerPR
                                                                                             from OPR in OwnerPR.DefaultIfEmpty()
                                                                                             join Stock in _dbContext.Stock.AsNoTracking()
                                                                                         on StockOnHand.StockID equals Stock.ID into StockPR
                                                                                             from StkPR in StockPR.DefaultIfEmpty()
                                                                                             join Location in _dbContext.Location.AsNoTracking()
                                                                                         on StockOnHand.LocationID equals Location.ID into LocationPR
                                                                                             from LPR in LocationPR.DefaultIfEmpty()
                                                                                             join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                         on StockOnHand.RateCodeID equals RateCode.ID into RateCodePR
                                                                                             from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                             join StorageRateCodeFrom in _dbContext.StorageRateCodeFrom.AsNoTracking()
                                                                                         on StockOnHand.StorageRateCodeFromID equals StorageRateCodeFrom.ID into StorageRateCodeFromPR
                                                                                             from SRCPR in StorageRateCodeFromPR.DefaultIfEmpty()
                                                                                             join ZoneProcessing in _dbContext.ZoneProcessing.AsNoTracking()
                                                                                                 on StockOnHand.ZoneProcessingID equals ZoneProcessing.ID into ZoneProcessingPR
                                                                                             from ZPPR in ZoneProcessingPR.DefaultIfEmpty()
                                                                                             join PalletType in _dbContext.PalletType.AsNoTracking()
                                                                                                 on StockOnHand.PalletTypeID equals PalletType.ID into PalletTypePR
                                                                                             from PTPR in PalletTypePR.DefaultIfEmpty()
                                                                                             join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                 on StockOnHand.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                             from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                             select new wbm_common.DataObjects.StockOnHand.dbRowExtended
                                                                                             {
                                                                                                 ID = StockOnHand.ID,
                                                                                                 CompanyID = StockOnHand.CompanyID,
                                                                                                 OwnerID = StockOnHand.OwnerID,
                                                                                                 SiteID = StockOnHand.SiteID,
                                                                                                 StockOnHandDate = StockOnHand.StockOnHandDate,
                                                                                                 LocationID = StockOnHand.LocationID,
                                                                                                 StockID = StockOnHand.StockID,
                                                                                                 Quantity = StockOnHand.Quantity,
                                                                                                 RateCodeID = StockOnHand.RateCodeID,
                                                                                                 StorageRateCodeFromID = StockOnHand.StorageRateCodeFromID,
                                                                                                  PalletNumber = StockOnHand.PalletNumber,
                                                                                                  ZoneProcessingID = StockOnHand.ZoneProcessingID,
                                                                                                  Status = StockOnHand.Status,
                                                                                                  UseByDate = StockOnHand.UseByDate,
                                                                                                  LotNumber = StockOnHand.LotNumber,
                                                                                                  PalletWeight = StockOnHand.PalletWeight,
                                                                                                  PalletWidth = StockOnHand.PalletWidth,
                                                                                                  PalletDepth = StockOnHand.PalletDepth,
                                                                                                  PalletHeight = StockOnHand.PalletHeight,
                                                                                                  PalletTypeID = StockOnHand.PalletTypeID,
                                                                                                 DeletedFlag = StockOnHand.DeletedFlag,
                                                                                                 CreatedDateTime = StockOnHand.CreatedDateTime,
                                                                                                 CreatedMethod = StockOnHand.CreatedMethod,
                                                                                                 CreatedUserID = StockOnHand.CreatedUserID,
                                                                                                 LastAmendedDateTime = StockOnHand.LastAmendedDateTime,
                                                                                                 LastAmendedMethod = StockOnHand.LastAmendedMethod,
                                                                                                 LastAmendedUserID = StockOnHand.LastAmendedUserID,

                                                                                                 Site_PublicID = SPR.PublicID,
                                                                                                 Company_ShortName = CPR.ShortName,
                                                                                                 Owner_Name = OPR.Name,
                                                                                                 Stock_ProductCode = StkPR.ProductCode,
                                                                                                 Location_PublicLocationID = LPR.PublicLocationID,
                                                                                                 RateCode_RateCode = RCPR.RateCode,
                                                                                                 StorageRateCodeFrom_PublicID = SRCPR.PublicID,
                                                                                                 ZoneProcessing_ZoneNumber = ZPPR.ZoneNumber,
                                                                                                 PalletType_PublicID = PTPR.PublicID,
                                                                                                 Created_UserPublicID = SU.PublicID
                                                                                             }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>(StockOnHands);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>> StockOnHand_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.StockOnHand.dbRowExtended> StockOnHands = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.StockOnHand.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>(StockOnHands);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion

        #region StockRateCategory
        public async Task<Result<int>> StockRateCategory_Insert(wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategory)
        {
            try
            {
                await _dbContext.StockRateCategory.AddAsync(StockRateCategory);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StockRateCategory.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_Insert - " + StockRateCategory.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> StockRateCategory_Update(wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategory)
        {
            try
            {
                _dbContext.StockRateCategory.Attach(StockRateCategory);
                _dbContext.Entry(StockRateCategory).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StockRateCategory.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_Update - " + StockRateCategory.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>> StockRateCategory_ListBySingleSiteID(int SiteID)
        {
            try
            {
                List<wbm_common.DataObjects.StockRateCategory.dbRowExtended> StockRateCategoryDb = await (from StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.SiteID == SiteID || x.SiteID == 0))
                                                                                                          join site in _dbContext.Site.AsNoTracking()
                                                                                                              on StockRateCategory.SiteID equals site.ID into sitePR
                                                                                                          from SPR in sitePR.DefaultIfEmpty()
                                                                                                          join company in _dbContext.Company.AsNoTracking()
                                                                                                      on StockRateCategory.CompanyID equals company.ID into companyPR
                                                                                                          from CPR in companyPR.DefaultIfEmpty()

                                                                                                          select new wbm_common.DataObjects.StockRateCategory.dbRowExtended
                                                                                                          {
                                                                                                              ID = StockRateCategory.ID,
                                                                                                              CompanyID = StockRateCategory.CompanyID,
                                                                                                              PublicID = StockRateCategory.PublicID,
                                                                                                              SiteID = StockRateCategory.SiteID,
                                                                                                              Description = StockRateCategory.Description,
                                                                                                              SortOrder = StockRateCategory.SortOrder,
                                                                                                              DeletedFlag = StockRateCategory.DeletedFlag,
                                                                                                              CreatedDateTime = StockRateCategory.CreatedDateTime,
                                                                                                              CreatedMethod = StockRateCategory.CreatedMethod,
                                                                                                              CreatedUserID = StockRateCategory.CreatedUserID,
                                                                                                              LastAmendedDateTime = StockRateCategory.LastAmendedDateTime,
                                                                                                              LastAmendedMethod = StockRateCategory.LastAmendedMethod,
                                                                                                              LastAmendedUserID = StockRateCategory.LastAmendedUserID,

                                                                                                              Site_PublicID = SPR.PublicID,
                                                                                                              Company_ShortName = CPR.ShortName
                                                                                                          }).ToListAsync();

                if (StockRateCategoryDb == null)
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>(new List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>(StockRateCategoryDb);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_ListBySingleSiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>> StockRateCategory_ListBySingleCompanyID(int CompanyID)
        {
            try
            {
                List<wbm_common.DataObjects.StockRateCategory.dbRowExtended> StockRateCategoryDb = await (from StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.CompanyID == CompanyID || x.CompanyID == 0))
                                                                                                          join site in _dbContext.Site.AsNoTracking()
                                                                                                              on StockRateCategory.SiteID equals site.ID into sitePR
                                                                                                          from SPR in sitePR.DefaultIfEmpty()
                                                                                                          join company in _dbContext.Company.AsNoTracking()
                                                                                                      on StockRateCategory.CompanyID equals company.ID into companyPR
                                                                                                          from CPR in companyPR.DefaultIfEmpty()

                                                                                                          select new wbm_common.DataObjects.StockRateCategory.dbRowExtended
                                                                                                          {
                                                                                                              ID = StockRateCategory.ID,
                                                                                                              CompanyID = StockRateCategory.CompanyID,
                                                                                                              PublicID = StockRateCategory.PublicID,
                                                                                                              SiteID = StockRateCategory.SiteID,
                                                                                                              Description = StockRateCategory.Description,
                                                                                                              SortOrder = StockRateCategory.SortOrder,
                                                                                                              DeletedFlag = StockRateCategory.DeletedFlag,
                                                                                                              CreatedDateTime = StockRateCategory.CreatedDateTime,
                                                                                                              CreatedMethod = StockRateCategory.CreatedMethod,
                                                                                                              CreatedUserID = StockRateCategory.CreatedUserID,
                                                                                                              LastAmendedDateTime = StockRateCategory.LastAmendedDateTime,
                                                                                                              LastAmendedMethod = StockRateCategory.LastAmendedMethod,
                                                                                                              LastAmendedUserID = StockRateCategory.LastAmendedUserID,

                                                                                                              Site_PublicID = SPR.PublicID,
                                                                                                              Company_ShortName = CPR.ShortName
                                                                                                          }).ToListAsync();

                if (StockRateCategoryDb == null)
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>(new List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>(StockRateCategoryDb);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_ListBySingleCompanyID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.StockRateCategory.dbRow>> StockRateCategory_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategoryDb = await _dbContext.StockRateCategory.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.PublicID == PublicID).FirstOrDefaultAsync();

                if (StockRateCategoryDb == null)
                    return Result.Failure<wbm_common.DataObjects.StockRateCategory.dbRow>("StockRateCategory not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.StockRateCategory.dbRow>(StockRateCategoryDb);

            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.StockRateCategory.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>> StockRateCategory_SearchByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.StockRateCategory.dbRowExtended> StockRateCategoryDb = await (from StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                          join site in _dbContext.Site.AsNoTracking()
                                                                                                              on StockRateCategory.SiteID equals site.ID into sitePR
                                                                                                          from SPR in sitePR.DefaultIfEmpty()
                                                                                                          join company in _dbContext.Company.AsNoTracking()
                                                                                                      on StockRateCategory.CompanyID equals company.ID into companyPR
                                                                                                          from CPR in companyPR.DefaultIfEmpty()

                                                                                                          select new wbm_common.DataObjects.StockRateCategory.dbRowExtended
                                                                                                          {
                                                                                                              ID = StockRateCategory.ID,
                                                                                                              CompanyID = StockRateCategory.CompanyID,
                                                                                                              PublicID = StockRateCategory.PublicID,
                                                                                                              SiteID = StockRateCategory.SiteID,
                                                                                                              Description = StockRateCategory.Description,
                                                                                                              SortOrder = StockRateCategory.SortOrder,
                                                                                                              DeletedFlag = StockRateCategory.DeletedFlag,
                                                                                                              CreatedDateTime = StockRateCategory.CreatedDateTime,
                                                                                                              CreatedMethod = StockRateCategory.CreatedMethod,
                                                                                                              CreatedUserID = StockRateCategory.CreatedUserID,
                                                                                                              LastAmendedDateTime = StockRateCategory.LastAmendedDateTime,
                                                                                                              LastAmendedMethod = StockRateCategory.LastAmendedMethod,
                                                                                                              LastAmendedUserID = StockRateCategory.LastAmendedUserID,

                                                                                                              Site_PublicID = SPR.PublicID,
                                                                                                              Company_ShortName = CPR.ShortName
                                                                                                          }).ToListAsync();

                if (StockRateCategoryDb == null)
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>(new List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>(StockRateCategoryDb);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.StockRateCategory.dbRow>> StockRateCategory_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategoryDb = await _dbContext.StockRateCategory.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (StockRateCategoryDb == null)
                    return Result.Failure<wbm_common.DataObjects.StockRateCategory.dbRow>("StockRateCategory not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.StockRateCategory.dbRow>(StockRateCategoryDb);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.StockRateCategory.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>> StockRateCategory_List()
        {
            try
            {
                List<wbm_common.DataObjects.StockRateCategory.dbRowExtended> StockRateCategorys = await (from StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                         join site in _dbContext.Site.AsNoTracking()
                                                                                                             on StockRateCategory.SiteID equals site.ID into sitePR
                                                                                                         from SPR in sitePR.DefaultIfEmpty()
                                                                                                         join company in _dbContext.Company.AsNoTracking()
                                                                                                     on StockRateCategory.CompanyID equals company.ID into companyPR
                                                                                                         from CPR in companyPR.DefaultIfEmpty()

                                                                                                         select new wbm_common.DataObjects.StockRateCategory.dbRowExtended
                                                                                                         {
                                                                                                             ID = StockRateCategory.ID,
                                                                                                             CompanyID = StockRateCategory.CompanyID,
                                                                                                             PublicID = StockRateCategory.PublicID,
                                                                                                             SiteID = StockRateCategory.SiteID,
                                                                                                             Description = StockRateCategory.Description,
                                                                                                             SortOrder = StockRateCategory.SortOrder,
                                                                                                             DeletedFlag = StockRateCategory.DeletedFlag,
                                                                                                             CreatedDateTime = StockRateCategory.CreatedDateTime,
                                                                                                             CreatedMethod = StockRateCategory.CreatedMethod,
                                                                                                             CreatedUserID = StockRateCategory.CreatedUserID,
                                                                                                             LastAmendedDateTime = StockRateCategory.LastAmendedDateTime,
                                                                                                             LastAmendedMethod = StockRateCategory.LastAmendedMethod,
                                                                                                             LastAmendedUserID = StockRateCategory.LastAmendedUserID,

                                                                                                             Site_PublicID = SPR.PublicID,
                                                                                                             Company_ShortName = CPR.ShortName
                                                                                                         }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>(StockRateCategorys);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>> StockRateCategory_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("StockRateCategory_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.StockRateCategory.dbRowExtended> StockRateCategorys = await (from StockRateCategory in _dbContext.StockRateCategory.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                             join site in _dbContext.Site.AsNoTracking()
                                                                                                                 on StockRateCategory.SiteID equals site.ID into sitePR
                                                                                                             from SPR in sitePR.DefaultIfEmpty()
                                                                                                             join company in _dbContext.Company.AsNoTracking()
                                                                                                         on StockRateCategory.CompanyID equals company.ID into companyPR
                                                                                                             from CPR in companyPR.DefaultIfEmpty()

                                                                                                             select new wbm_common.DataObjects.StockRateCategory.dbRowExtended
                                                                                                             {
                                                                                                                 ID = StockRateCategory.ID,
                                                                                                                 CompanyID = StockRateCategory.CompanyID,
                                                                                                                 PublicID = StockRateCategory.PublicID,
                                                                                                                 SiteID = StockRateCategory.SiteID,
                                                                                                                 Description = StockRateCategory.Description,
                                                                                                                 SortOrder = StockRateCategory.SortOrder,
                                                                                                                 DeletedFlag = StockRateCategory.DeletedFlag,
                                                                                                                 CreatedDateTime = StockRateCategory.CreatedDateTime,
                                                                                                                 CreatedMethod = StockRateCategory.CreatedMethod,
                                                                                                                 CreatedUserID = StockRateCategory.CreatedUserID,
                                                                                                                 LastAmendedDateTime = StockRateCategory.LastAmendedDateTime,
                                                                                                                 LastAmendedMethod = StockRateCategory.LastAmendedMethod,
                                                                                                                 LastAmendedUserID = StockRateCategory.LastAmendedUserID,

                                                                                                                 Site_PublicID = SPR.PublicID,
                                                                                                                 Company_ShortName = CPR.ShortName
                                                                                                             }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>(StockRateCategorys);
                }
                catch (Exception ex)
                {
                    logger.Error("StockRateCategory_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>> StockRateCategory_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.StockRateCategory.dbRowExtended> StockRateCategorys = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.StockRateCategory.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>(StockRateCategorys);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
