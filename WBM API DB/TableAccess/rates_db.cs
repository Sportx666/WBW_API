using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region RateCode
        public async Task<Result<int>> RateCode_Insert(wbm_common.DataObjects.RateCode.dbRow RateCode)
        {
            try
            {
                await _dbContext.RateCode.AddAsync(RateCode);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCode.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_Insert - " + RateCode.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> RateCode_Update(wbm_common.DataObjects.RateCode.dbRow RateCode)
        {
            try
            {
                _dbContext.RateCode.Attach(RateCode);
                _dbContext.Entry(RateCode).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCode.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_Update - " + RateCode.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.RateCode.dbRow>> RateCode_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCode.dbRow RateCode = await _dbContext.RateCode.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateCode == null)
                    return Result.Failure<wbm_common.DataObjects.RateCode.dbRow>("RateCode not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCode.dbRow>(RateCode);

            }
            catch (Exception ex)
            {
                logger.Error("RateCode_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCode.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.RateCode.dbRow>> RateCode_SearchByRateCode(string RC)
        {
            try
            {
                wbm_common.DataObjects.RateCode.dbRow RateCode = await _dbContext.RateCode.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.RateCode == RC);

                if (RateCode == null)
                    return Result.Failure<wbm_common.DataObjects.RateCode.dbRow>("RateCode not found : " + RC);
                else
                    return Result.Success<wbm_common.DataObjects.RateCode.dbRow>(RateCode);

            }
            catch (Exception ex)
            {
                logger.Error("RateCode_SearchByRateCode - " + RC + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCode.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.RateCode.dbRow>>> RateCode_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCode.dbRow RateCode = await _dbContext.RateCode.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateCode == null)
                    return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(new List<wbm_common.DataObjects.RateCode.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(new List<wbm_common.DataObjects.RateCode.dbRow> { RateCode });
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCode.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateCode.dbRow>> RateCode_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCode.dbRow RateCode = await _dbContext.RateCode.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (RateCode == null)
                    return Result.Failure<wbm_common.DataObjects.RateCode.dbRow>("RateCode not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCode.dbRow>(RateCode);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCode.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCode.dbRow>>> RateCode_List()
        {
            try
            {
                List<wbm_common.DataObjects.RateCode.dbRow> RateCodes = await _dbContext.RateCode.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(RateCodes);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.RateCode.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCode.dbRow>>> RateCode_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("RateCode_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.RateCode.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.RateCode.dbRow> RateCodes = await _dbContext.RateCode.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(RateCodes);
                }
                catch (Exception ex)
                {
                    logger.Error("RateCode_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.RateCode.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCode.dbRow>>> RateCode_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.RateCode.dbRow> RateCodes = await _dbContext.RateCode.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(RateCodes);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCode.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region RateCollection
        public async Task<Result<int>> RateCollection_Insert(wbm_common.DataObjects.RateCollection.dbRow RateCollection)
        {
            try
            {
                await _dbContext.RateCollection.AddAsync(RateCollection);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCollection.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_Insert - " + RateCollection.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> RateCollection_Update(wbm_common.DataObjects.RateCollection.dbRow RateCollection)
        {
            try
            {
                _dbContext.RateCollection.Attach(RateCollection);
                _dbContext.Entry(RateCollection).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCollection.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_Update - " + RateCollection.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
       
        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>> RateCollection_ListBySingleCompanyID(int CompanyID)
        {
            try
            {
                List<wbm_common.DataObjects.RateCollection.dbRowExtended> RateCollectionDb = await (from RateCollection in _dbContext.RateCollection.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.CompanyID == CompanyID)
                                                                                                    join RateCollectionDefn in _dbContext.RateCollectionDefn.AsNoTracking()
                                                                                                        on RateCollection.RateCollectionDefnID equals RateCollectionDefn.ID into RateCollectionDefnPR
                                                                                                    from RCDPR in RateCollectionDefnPR.DefaultIfEmpty()
                                                                                                    join RateFunction in _dbContext.RateFunction.AsNoTracking()
                                                                                                on RateCollection.RateFunctionID equals RateFunction.ID into RateFunctionPR
                                                                                                    from RFPR in RateFunctionPR.DefaultIfEmpty()
                                                                                                    join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                on RateCollection.RateCodeID equals RateCode.ID into RateCodePR
                                                                                                    from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                    join NewRateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                on RateCollection.NewRateCodeID equals NewRateCode.ID into NewRateCodePR
                                                                                                    from NRCPR in NewRateCodePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                on RateCollection.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.RateCollection.dbRowExtended
                                                                                                    {
                                                                                                        ID = RateCollection.ID,
                                                                                                        CompanyID = RateCollection.CompanyID,
                                                                                                        RateCollectionDefnID = RateCollection.RateCollectionDefnID,
                                                                                                        RateChargeCode = RateCollection.RateChargeCode,
                                                                                                        RateFunctionID = RateCollection.RateFunctionID,
                                                                                                        RateCodeID = RateCollection.RateCodeID,
                                                                                                        Description = RateCollection.Description,
                                                                                                        SecondarySort = RateCollection.SecondarySort,
                                                                                                        DateOfChange = RateCollection.DateOfChange,
                                                                                                        NewRateCodeID = RateCollection.NewRateCodeID,
                                                                                                        DeletedFlag = RateCollection.DeletedFlag,
                                                                                                        CreatedDateTime = RateCollection.CreatedDateTime,
                                                                                                        CreatedMethod = RateCollection.CreatedMethod,
                                                                                                        CreatedUserID = RateCollection.CreatedUserID,
                                                                                                        LastAmendedDateTime = RateCollection.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = RateCollection.LastAmendedMethod,
                                                                                                        LastAmendedUserID = RateCollection.LastAmendedUserID,

                                                                                                        RateCollectionDefn_PublicID = RCDPR.PublicID,
                                                                                                        RateFunction_PublicID = RFPR.PublicID,
                                                                                                        RateCode_RateCode = RCPR.RateCode,
                                                                                                        NewRateCode_RateCode = NRCPR.RateCode,
                                                                                                        Company_ShortName = CPR.ShortName
                                                                                                    }).ToListAsync();

                if (RateCollectionDb == null)
                    return Result.Failure<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>("RateCollection not found : " + CompanyID.ToString());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>(RateCollectionDb);

            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_ListBySingleCompanyID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>> RateCollection_ListBySingleRateCollectionDefnID(int RateCollectionDefnID)
        {
            try
            {
                List<wbm_common.DataObjects.RateCollection.dbRowExtended> RateCollectionDb = await (from RateCollection in _dbContext.RateCollection.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.RateCollectionDefnID == RateCollectionDefnID)
                                                                                                    join RateCollectionDefn in _dbContext.RateCollectionDefn.AsNoTracking()
                                                                                                        on RateCollection.RateCollectionDefnID equals RateCollectionDefn.ID into RateCollectionDefnPR
                                                                                                    from RCDPR in RateCollectionDefnPR.DefaultIfEmpty()
                                                                                                    join RateFunction in _dbContext.RateFunction.AsNoTracking()
                                                                                                on RateCollection.RateFunctionID equals RateFunction.ID into RateFunctionPR
                                                                                                    from RFPR in RateFunctionPR.DefaultIfEmpty()
                                                                                                    join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                on RateCollection.RateCodeID equals RateCode.ID into RateCodePR
                                                                                                    from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                    join NewRateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                on RateCollection.NewRateCodeID equals NewRateCode.ID into NewRateCodePR
                                                                                                    from NRCPR in NewRateCodePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                on RateCollection.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.RateCollection.dbRowExtended
                                                                                                    {
                                                                                                        ID = RateCollection.ID,
                                                                                                        CompanyID = RateCollection.CompanyID,
                                                                                                        RateCollectionDefnID = RateCollection.RateCollectionDefnID,
                                                                                                        RateChargeCode = RateCollection.RateChargeCode,
                                                                                                        RateFunctionID = RateCollection.RateFunctionID,
                                                                                                        RateCodeID = RateCollection.RateCodeID,
                                                                                                        Description = RateCollection.Description,
                                                                                                        SecondarySort = RateCollection.SecondarySort,
                                                                                                        DateOfChange = RateCollection.DateOfChange,
                                                                                                        NewRateCodeID = RateCollection.NewRateCodeID,
                                                                                                        DeletedFlag = RateCollection.DeletedFlag,
                                                                                                        CreatedDateTime = RateCollection.CreatedDateTime,
                                                                                                        CreatedMethod = RateCollection.CreatedMethod,
                                                                                                        CreatedUserID = RateCollection.CreatedUserID,
                                                                                                        LastAmendedDateTime = RateCollection.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = RateCollection.LastAmendedMethod,
                                                                                                        LastAmendedUserID = RateCollection.LastAmendedUserID,

                                                                                                        RateCollectionDefn_PublicID = RCDPR.PublicID,
                                                                                                        RateFunction_PublicID = RFPR.PublicID,
                                                                                                        RateCode_RateCode = RCPR.RateCode,
                                                                                                        NewRateCode_RateCode = NRCPR.RateCode,
                                                                                                        Company_ShortName = CPR.ShortName
                                                                                                    }).ToListAsync();

                if (RateCollectionDb == null)
                    return Result.Failure<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>("RateCollection not found : " + RateCollectionDefnID.ToString());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>(RateCollectionDb);

            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_ListBySingleRateCollectionDefnID - " + RateCollectionDefnID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>> RateCollection_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.RateCollection.dbRowExtended> RateCollectionDb = await (from RateCollection in _dbContext.RateCollection.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                    join RateCollectionDefn in _dbContext.RateCollectionDefn.AsNoTracking()
                                                                                                        on RateCollection.RateCollectionDefnID equals RateCollectionDefn.ID into RateCollectionDefnPR
                                                                                                    from RCDPR in RateCollectionDefnPR.DefaultIfEmpty()
                                                                                                    join RateFunction in _dbContext.RateFunction.AsNoTracking()
                                                                                                on RateCollection.RateFunctionID equals RateFunction.ID into RateFunctionPR
                                                                                                    from RFPR in RateFunctionPR.DefaultIfEmpty()
                                                                                                    join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                on RateCollection.RateCodeID equals RateCode.ID into RateCodePR
                                                                                                    from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                    join NewRateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                on RateCollection.NewRateCodeID equals NewRateCode.ID into NewRateCodePR
                                                                                                    from NRCPR in NewRateCodePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                on RateCollection.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.RateCollection.dbRowExtended
                                                                                                    {
                                                                                                        ID = RateCollection.ID,
                                                                                                        CompanyID = RateCollection.CompanyID,
                                                                                                        RateCollectionDefnID = RateCollection.RateCollectionDefnID,
                                                                                                        RateChargeCode = RateCollection.RateChargeCode,
                                                                                                        RateFunctionID = RateCollection.RateFunctionID,
                                                                                                        RateCodeID = RateCollection.RateCodeID,
                                                                                                        Description = RateCollection.Description,
                                                                                                        SecondarySort = RateCollection.SecondarySort,
                                                                                                        DateOfChange = RateCollection.DateOfChange,
                                                                                                        NewRateCodeID = RateCollection.NewRateCodeID,
                                                                                                        DeletedFlag = RateCollection.DeletedFlag,
                                                                                                        CreatedDateTime = RateCollection.CreatedDateTime,
                                                                                                        CreatedMethod = RateCollection.CreatedMethod,
                                                                                                        CreatedUserID = RateCollection.CreatedUserID,
                                                                                                        LastAmendedDateTime = RateCollection.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = RateCollection.LastAmendedMethod,
                                                                                                        LastAmendedUserID = RateCollection.LastAmendedUserID,

                                                                                                        RateCollectionDefn_PublicID = RCDPR.PublicID,
                                                                                                        RateFunction_PublicID = RFPR.PublicID,
                                                                                                        RateCode_RateCode = RCPR.RateCode,
                                                                                                        NewRateCode_RateCode = NRCPR.RateCode,
                                                                                                        Company_ShortName = CPR.ShortName
                                                                                                    }).ToListAsync();

                if (RateCollectionDb == null)
                    return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>(new List<wbm_common.DataObjects.RateCollection.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>(RateCollectionDb);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateCollection.dbRow>> RateCollection_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCollection.dbRow RateCollectionDb = await _dbContext.RateCollection.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (RateCollectionDb == null)
                    return Result.Failure<wbm_common.DataObjects.RateCollection.dbRow>("RateCollection not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCollection.dbRow>(RateCollectionDb);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCollection.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>> RateCollection_List()
        {
            try
            {
                List<wbm_common.DataObjects.RateCollection.dbRowExtended> RateCollections = await (from RateCollection in _dbContext.RateCollection.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                   join RateCollectionDefn in _dbContext.RateCollectionDefn.AsNoTracking()
                                                                                                       on RateCollection.RateCollectionDefnID equals RateCollectionDefn.ID into RateCollectionDefnPR
                                                                                                   from RCDPR in RateCollectionDefnPR.DefaultIfEmpty()
                                                                                                   join RateFunction in _dbContext.RateFunction.AsNoTracking()
                                                                                               on RateCollection.RateFunctionID equals RateFunction.ID into RateFunctionPR
                                                                                                   from RFPR in RateFunctionPR.DefaultIfEmpty()
                                                                                                   join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                               on RateCollection.RateCodeID equals RateCode.ID into RateCodePR
                                                                                                   from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                   join NewRateCode in _dbContext.RateCode.AsNoTracking()
                                                                                               on RateCollection.NewRateCodeID equals NewRateCode.ID into NewRateCodePR
                                                                                                   from NRCPR in NewRateCodePR.DefaultIfEmpty()
                                                                                                   join company in _dbContext.Company.AsNoTracking()
                                                                                               on RateCollection.CompanyID equals company.ID into companyPR
                                                                                                   from CPR in companyPR.DefaultIfEmpty()

                                                                                                   select new wbm_common.DataObjects.RateCollection.dbRowExtended
                                                                                                   {
                                                                                                       ID = RateCollection.ID,
                                                                                                       CompanyID = RateCollection.CompanyID,
                                                                                                       RateCollectionDefnID = RateCollection.RateCollectionDefnID,
                                                                                                       RateChargeCode = RateCollection.RateChargeCode,
                                                                                                       RateFunctionID = RateCollection.RateFunctionID,
                                                                                                       RateCodeID = RateCollection.RateCodeID,
                                                                                                       Description = RateCollection.Description,
                                                                                                       SecondarySort = RateCollection.SecondarySort,
                                                                                                       DateOfChange = RateCollection.DateOfChange,
                                                                                                       NewRateCodeID = RateCollection.NewRateCodeID,
                                                                                                       DeletedFlag = RateCollection.DeletedFlag,
                                                                                                       CreatedDateTime = RateCollection.CreatedDateTime,
                                                                                                       CreatedMethod = RateCollection.CreatedMethod,
                                                                                                       CreatedUserID = RateCollection.CreatedUserID,
                                                                                                       LastAmendedDateTime = RateCollection.LastAmendedDateTime,
                                                                                                       LastAmendedMethod = RateCollection.LastAmendedMethod,
                                                                                                       LastAmendedUserID = RateCollection.LastAmendedUserID,

                                                                                                       RateCollectionDefn_PublicID = RCDPR.PublicID,
                                                                                                       RateFunction_PublicID = RFPR.PublicID,
                                                                                                       RateCode_RateCode = RCPR.RateCode,
                                                                                                       NewRateCode_RateCode = NRCPR.RateCode,
                                                                                                       Company_ShortName = CPR.ShortName
                                                                                                   }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>(RateCollections);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>> RateCollection_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("RateCollection_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.RateCollection.dbRowExtended> RateCollections = await (from RateCollection in _dbContext.RateCollection.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                       join RateCollectionDefn in _dbContext.RateCollectionDefn.AsNoTracking()
                                                                                                           on RateCollection.RateCollectionDefnID equals RateCollectionDefn.ID into RateCollectionDefnPR
                                                                                                       from RCDPR in RateCollectionDefnPR.DefaultIfEmpty()
                                                                                                       join RateFunction in _dbContext.RateFunction.AsNoTracking()
                                                                                                   on RateCollection.RateFunctionID equals RateFunction.ID into RateFunctionPR
                                                                                                       from RFPR in RateFunctionPR.DefaultIfEmpty()
                                                                                                       join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                   on RateCollection.RateCodeID equals RateCode.ID into RateCodePR
                                                                                                       from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                       join NewRateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                   on RateCollection.NewRateCodeID equals NewRateCode.ID into NewRateCodePR
                                                                                                       from NRCPR in NewRateCodePR.DefaultIfEmpty()
                                                                                                       join company in _dbContext.Company.AsNoTracking()
                                                                                                   on RateCollection.CompanyID equals company.ID into companyPR
                                                                                                       from CPR in companyPR.DefaultIfEmpty()

                                                                                                       select new wbm_common.DataObjects.RateCollection.dbRowExtended
                                                                                                       {
                                                                                                           ID = RateCollection.ID,
                                                                                                           CompanyID = RateCollection.CompanyID,
                                                                                                           RateCollectionDefnID = RateCollection.RateCollectionDefnID,
                                                                                                           RateChargeCode = RateCollection.RateChargeCode,
                                                                                                           RateFunctionID = RateCollection.RateFunctionID,
                                                                                                           RateCodeID = RateCollection.RateCodeID,
                                                                                                           Description = RateCollection.Description,
                                                                                                           SecondarySort = RateCollection.SecondarySort,
                                                                                                           DateOfChange = RateCollection.DateOfChange,
                                                                                                           NewRateCodeID = RateCollection.NewRateCodeID,
                                                                                                           DeletedFlag = RateCollection.DeletedFlag,
                                                                                                           CreatedDateTime = RateCollection.CreatedDateTime,
                                                                                                           CreatedMethod = RateCollection.CreatedMethod,
                                                                                                           CreatedUserID = RateCollection.CreatedUserID,
                                                                                                           LastAmendedDateTime = RateCollection.LastAmendedDateTime,
                                                                                                           LastAmendedMethod = RateCollection.LastAmendedMethod,
                                                                                                           LastAmendedUserID = RateCollection.LastAmendedUserID,

                                                                                                           RateCollectionDefn_PublicID = RCDPR.PublicID,
                                                                                                           RateFunction_PublicID = RFPR.PublicID,
                                                                                                           RateCode_RateCode = RCPR.RateCode,
                                                                                                           NewRateCode_RateCode = NRCPR.RateCode,
                                                                                                           Company_ShortName = CPR.ShortName
                                                                                                       }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>(RateCollections);
                }
                catch (Exception ex)
                {
                    logger.Error("RateCollection_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>> RateCollection_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.RateCollection.dbRowExtended> RateCollections = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.RateCollection.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>(RateCollections);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion

        #region RateCollectionDefn
        public async Task<Result<int>> RateCollectionDefn_Insert(wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefn)
        {
            try
            {
                await _dbContext.RateCollectionDefn.AddAsync(RateCollectionDefn);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCollectionDefn.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_Insert - " + RateCollectionDefn.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> RateCollectionDefn_Update(wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefn)
        {
            try
            {
                _dbContext.RateCollectionDefn.Attach(RateCollectionDefn);
                _dbContext.Entry(RateCollectionDefn).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCollectionDefn.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_Update - " + RateCollectionDefn.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateCollectionDefn.dbRow>> RateCollectionDefn_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefnDb = await _dbContext.RateCollectionDefn.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateCollectionDefnDb == null)
                    return Result.Failure<wbm_common.DataObjects.RateCollectionDefn.dbRow>("RateCollectionDefn not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCollectionDefn.dbRow>(RateCollectionDefnDb);

            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCollectionDefn.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateCollectionDefn.dbRow>> RateCollectionDefn_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefnDb = await _dbContext.RateCollectionDefn.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.PublicID == PublicID).FirstOrDefaultAsync();

                if (RateCollectionDefnDb == null)
                    return Result.Failure<wbm_common.DataObjects.RateCollectionDefn.dbRow>("RateCollectionDefn not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.RateCollectionDefn.dbRow>(RateCollectionDefnDb);

            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCollectionDefn.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>> RateCollectionDefn_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended> RateCollectionDefnDb = await (from RateCollectionDefn in _dbContext.RateCollectionDefn.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                        on RateCollectionDefn.CompanyID equals company.ID into companyPR
                                                                                                            from CPR in companyPR.DefaultIfEmpty()

                                                                                                            select new wbm_common.DataObjects.RateCollectionDefn.dbRowExtended
                                                                                                    {
                                                                                                        ID = RateCollectionDefn.ID,
                                                                                                        CompanyID = RateCollectionDefn.CompanyID,
                                                                                                        PublicID = RateCollectionDefn.PublicID,
                                                                                                        Description = RateCollectionDefn.Description,
                                                                                                        SortOrder = RateCollectionDefn.SortOrder,
                                                                                                        DeletedFlag = RateCollectionDefn.DeletedFlag,
                                                                                                        CreatedDateTime = RateCollectionDefn.CreatedDateTime,
                                                                                                        CreatedMethod = RateCollectionDefn.CreatedMethod,
                                                                                                        CreatedUserID = RateCollectionDefn.CreatedUserID,
                                                                                                        LastAmendedDateTime = RateCollectionDefn.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = RateCollectionDefn.LastAmendedMethod,
                                                                                                        LastAmendedUserID = RateCollectionDefn.LastAmendedUserID,

                                                                                                        Company_ShortName = CPR.ShortName
                                                                                                    }).ToListAsync();

                if (RateCollectionDefnDb == null)
                    return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>(new List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>(RateCollectionDefnDb );
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateCollectionDefn.dbRow>> RateCollectionDefn_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefnDb = await _dbContext.RateCollectionDefn.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (RateCollectionDefnDb == null)
                    return Result.Failure<wbm_common.DataObjects.RateCollectionDefn.dbRow>("RateCollectionDefn not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCollectionDefn.dbRow>(RateCollectionDefnDb);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCollectionDefn.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>> RateCollectionDefn_List()
        {
            try
            {
                List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended> RateCollectionDefns = await (from RateCollectionDefn in _dbContext.RateCollectionDefn.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                        on RateCollectionDefn.CompanyID equals company.ID into companyPR
                                                                                                            from CPR in companyPR.DefaultIfEmpty()

                                                                                                            select new wbm_common.DataObjects.RateCollectionDefn.dbRowExtended
                                                                                                    {
                                                                                                        ID = RateCollectionDefn.ID,
                                                                                                        CompanyID = RateCollectionDefn.CompanyID,
                                                                                                        PublicID = RateCollectionDefn.PublicID,
                                                                                                        Description = RateCollectionDefn.Description,
                                                                                                        SortOrder = RateCollectionDefn.SortOrder,
                                                                                                        DeletedFlag = RateCollectionDefn.DeletedFlag,
                                                                                                        CreatedDateTime = RateCollectionDefn.CreatedDateTime,
                                                                                                        CreatedMethod = RateCollectionDefn.CreatedMethod,
                                                                                                        CreatedUserID = RateCollectionDefn.CreatedUserID,
                                                                                                        LastAmendedDateTime = RateCollectionDefn.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = RateCollectionDefn.LastAmendedMethod,
                                                                                                        LastAmendedUserID = RateCollectionDefn.LastAmendedUserID,

                                                                                                        Company_ShortName = CPR.ShortName
                                                                                                    }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>(RateCollectionDefns);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>> RateCollectionDefn_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("RateCollectionDefn_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended> RateCollectionDefns = await (from RateCollectionDefn in _dbContext.RateCollectionDefn.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                               join company in _dbContext.Company.AsNoTracking()
                                                                                                                   on RateCollectionDefn.CompanyID equals company.ID into companyPR
                                                                                                               from CPR in companyPR.DefaultIfEmpty()

                                                                                                               select new wbm_common.DataObjects.RateCollectionDefn.dbRowExtended
                                                                                                               {
                                                                                                                   ID = RateCollectionDefn.ID,
                                                                                                                   CompanyID = RateCollectionDefn.CompanyID,
                                                                                                                   PublicID = RateCollectionDefn.PublicID,
                                                                                                                   Description = RateCollectionDefn.Description,
                                                                                                                   SortOrder = RateCollectionDefn.SortOrder,
                                                                                                                   DeletedFlag = RateCollectionDefn.DeletedFlag,
                                                                                                                   CreatedDateTime = RateCollectionDefn.CreatedDateTime,
                                                                                                                   CreatedMethod = RateCollectionDefn.CreatedMethod,
                                                                                                                   CreatedUserID = RateCollectionDefn.CreatedUserID,
                                                                                                                   LastAmendedDateTime = RateCollectionDefn.LastAmendedDateTime,
                                                                                                                   LastAmendedMethod = RateCollectionDefn.LastAmendedMethod,
                                                                                                                   LastAmendedUserID = RateCollectionDefn.LastAmendedUserID,

                                                                                                                   Company_ShortName = CPR.ShortName
                                                                                                               }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>(RateCollectionDefns);
                }
                catch (Exception ex)
                {
                    logger.Error("RateCollectionDefn_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>> RateCollectionDefn_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended> RateCollectionDefns = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>(RateCollectionDefns);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollectionDefn.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion

        #region RateFunction
        public async Task<Result<int>> RateFunction_Insert(wbm_common.DataObjects.RateFunction.dbRow RateFunction)
        {
            try
            {
                await _dbContext.RateFunction.AddAsync(RateFunction);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateFunction.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_Insert - " + RateFunction.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> RateFunction_Update(wbm_common.DataObjects.RateFunction.dbRow RateFunction)
        {
            try
            {
                _dbContext.RateFunction.Attach(RateFunction);
                _dbContext.Entry(RateFunction).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateFunction.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_Update - " + RateFunction.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateFunction.dbRow>> RateFunction_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateFunction.dbRow RateFunction = await _dbContext.RateFunction.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateFunction == null)
                    return Result.Failure<wbm_common.DataObjects.RateFunction.dbRow>("RateFunction not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateFunction.dbRow>(RateFunction);

            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateFunction.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateFunction.dbRow>> RateFunction_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.RateFunction.dbRow RateFunction = await _dbContext.RateFunction.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (RateFunction == null)
                    return Result.Failure<wbm_common.DataObjects.RateFunction.dbRow>("RateFunction not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.RateFunction.dbRow>(RateFunction);

            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateFunction.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.RateFunction.dbRow>>> RateFunction_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateFunction.dbRow RateFunction = await _dbContext.RateFunction.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateFunction == null)
                    return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(new List<wbm_common.DataObjects.RateFunction.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(new List<wbm_common.DataObjects.RateFunction.dbRow> { RateFunction });
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateFunction.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateFunction.dbRow>> RateFunction_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateFunction.dbRow RateFunction = await _dbContext.RateFunction.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (RateFunction == null)
                    return Result.Failure<wbm_common.DataObjects.RateFunction.dbRow>("RateFunction not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateFunction.dbRow>(RateFunction);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateFunction.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateFunction.dbRow>>> RateFunction_List()
        {
            try
            {
                List<wbm_common.DataObjects.RateFunction.dbRow> RateFunctions = await _dbContext.RateFunction.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(RateFunctions);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.RateFunction.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateFunction.dbRow>>> RateFunction_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("RateFunction_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.RateFunction.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.RateFunction.dbRow> RateFunctions = await _dbContext.RateFunction.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(RateFunctions);
                }
                catch (Exception ex)
                {
                    logger.Error("RateFunction_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.RateFunction.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateFunction.dbRow>>> RateFunction_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.RateFunction.dbRow> RateFunctions = await _dbContext.RateFunction.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(RateFunctions);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateFunction.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region TableRate
        public async Task<Result<int>> TableRate_Insert(wbm_common.DataObjects.TableRate.dbRow TableRate)
        {
            try
            {
                await _dbContext.TableRate.AddAsync(TableRate);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TableRate.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_Insert - " + TableRate.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TableRate_Update(wbm_common.DataObjects.TableRate.dbRow TableRate)
        {
            try
            {
                _dbContext.TableRate.Attach(TableRate);
                _dbContext.Entry(TableRate).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TableRate.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_Update - " + TableRate.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TableRate.dbRow>> TableRate_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TableRate.dbRow TableRate = await _dbContext.TableRate.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TableRate == null)
                    return Result.Failure<wbm_common.DataObjects.TableRate.dbRow>("TableRate not found : " + ID);
                else
                    return Result.Success<wbm_common.DataObjects.TableRate.dbRow>(TableRate);

            }
            catch (Exception ex)
            {
                logger.Error("TableRate_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TableRate.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRowExtended>>> TableRate_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TableRate.dbRowExtended TableRateDb = await (from TableRate in _dbContext.TableRate.AsNoTracking()
                                                                        .Where(x => x.ID == ID && !x.DeletedFlag)
                                                                                    join Rate in _dbContext.RateCode.AsNoTracking()
                                                                                        on TableRate.RateID equals Rate.ID into RatePR
                                                                                    from RPR in RatePR.DefaultIfEmpty()

                                                                                    select new wbm_common.DataObjects.TableRate.dbRowExtended
                                                                                  {
                                                                                      ID = TableRate.ID,
                                                                                      RateID = TableRate.RateID,
                                                                                      TableBreak = TableRate.TableBreak,
                                                                                      TableUnits = TableRate.TableUnits,
                                                                                      TableCharge = TableRate.TableCharge,
                                                                                      DateOfChange = TableRate.DateOfChange,
                                                                                      NewTableCharge = TableRate.NewTableCharge,
                                                                                      DeletedFlag = TableRate.DeletedFlag,
                                                                                      CreatedDateTime = TableRate.CreatedDateTime,
                                                                                      CreatedMethod = TableRate.CreatedMethod,
                                                                                      CreatedUserID = TableRate.CreatedUserID,
                                                                                      LastAmendedDateTime = TableRate.LastAmendedDateTime,
                                                                                      LastAmendedMethod = TableRate.LastAmendedMethod,
                                                                                      LastAmendedUserID = TableRate.LastAmendedUserID
                                                                                  }).FirstOrDefaultAsync();

                if (TableRateDb == null)
                    return Result.Success<List<wbm_common.DataObjects.TableRate.dbRowExtended>>(new List<wbm_common.DataObjects.TableRate.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TableRate.dbRowExtended>>(new List<wbm_common.DataObjects.TableRate.dbRowExtended> { TableRateDb });
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TableRate.dbRow>> TableRate_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TableRate.dbRow TableRateDb = await _dbContext.TableRate.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (TableRateDb == null)
                    return Result.Failure<wbm_common.DataObjects.TableRate.dbRow>("TableRate not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TableRate.dbRow>(TableRateDb);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TableRate.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRowExtended>>> TableRate_List()
        {
            try
            {
                List<wbm_common.DataObjects.TableRate.dbRowExtended> TableRates = await (from TableRate in _dbContext.TableRate.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                         join Rate in _dbContext.RateCode.AsNoTracking()
                                                                                             on TableRate.RateID equals Rate.ID into RatePR
                                                                                         from RPR in RatePR.DefaultIfEmpty()

                                                                                         select new wbm_common.DataObjects.TableRate.dbRowExtended
                                                                                         {
                                                                                             ID = TableRate.ID,
                                                                                             RateID = TableRate.RateID,
                                                                                             TableBreak = TableRate.TableBreak,
                                                                                             TableUnits = TableRate.TableUnits,
                                                                                             TableCharge = TableRate.TableCharge,
                                                                                             DateOfChange = TableRate.DateOfChange,
                                                                                             NewTableCharge = TableRate.NewTableCharge,
                                                                                             DeletedFlag = TableRate.DeletedFlag,
                                                                                             CreatedDateTime = TableRate.CreatedDateTime,
                                                                                             CreatedMethod = TableRate.CreatedMethod,
                                                                                             CreatedUserID = TableRate.CreatedUserID,
                                                                                             LastAmendedDateTime = TableRate.LastAmendedDateTime,
                                                                                             LastAmendedMethod = TableRate.LastAmendedMethod,
                                                                                             LastAmendedUserID = TableRate.LastAmendedUserID,

                                                                                             RateID_RateCode = RPR.RateCode
                                                                                         }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TableRate.dbRowExtended>>(TableRates);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRowExtended>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRowExtended>>> TableRate_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TableRate_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TableRate.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TableRate.dbRowExtended> TableRates = await (from TableRate in _dbContext.TableRate.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                             join Rate in _dbContext.RateCode.AsNoTracking()
                                                                                                 on TableRate.RateID equals Rate.ID into RatePR
                                                                                             from RPR in RatePR.DefaultIfEmpty()

                                                                                             select new wbm_common.DataObjects.TableRate.dbRowExtended
                                                                                             {
                                                                                                 ID = TableRate.ID,
                                                                                                 RateID = TableRate.RateID,
                                                                                                 TableBreak = TableRate.TableBreak,
                                                                                                 TableUnits = TableRate.TableUnits,
                                                                                                 TableCharge = TableRate.TableCharge,
                                                                                                 DateOfChange = TableRate.DateOfChange,
                                                                                                 NewTableCharge = TableRate.NewTableCharge,
                                                                                                 DeletedFlag = TableRate.DeletedFlag,
                                                                                                 CreatedDateTime = TableRate.CreatedDateTime,
                                                                                                 CreatedMethod = TableRate.CreatedMethod,
                                                                                                 CreatedUserID = TableRate.CreatedUserID,
                                                                                                 LastAmendedDateTime = TableRate.LastAmendedDateTime,
                                                                                                 LastAmendedMethod = TableRate.LastAmendedMethod,
                                                                                                 LastAmendedUserID = TableRate.LastAmendedUserID,

                                                                                                 RateID_RateCode = RPR.RateCode
                                                                                             }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TableRate.dbRowExtended>>(TableRates);
                }
                catch (Exception ex)
                {
                    logger.Error("TableRate_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRow>>> TableRate_ListByListRateCodeIDs(List<int> RCIDs)
        {
            if (RCIDs == null || RCIDs.Count == 0)
            {
                logger.Error("TableRate_ListByListRateCodeIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TableRate.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TableRate.dbRow> TableRates = await _dbContext.TableRate.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && RCIDs.Contains(x.RateID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TableRate.dbRow>>(TableRates);
                }
                catch (Exception ex)
                {
                    logger.Error("TableRate_ListByListRateCodeIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRow>>> TableRate_ListByListRateCodeIDsNoLookup(List<int> RCIDs)
        {
            if (RCIDs == null || RCIDs.Count == 0)
            {
                logger.Error("TableRate_ListByListRateCodeIDsNoLookup was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TableRate.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TableRate.dbRow> ListTableRate = await _dbContext.TableRate.AsNoTracking().Where(x => !x.DeletedFlag && RCIDs.Contains(x.RateID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TableRate.dbRow>>(ListTableRate);
                }
                catch (Exception ex)
                {
                    logger.Error("TableRate_ListByListRateCodeIDsNoLookup: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRow>>("Internal Exception occured");
                }
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRowExtended>>> TableRate_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TableRate.dbRowExtended> TableRates = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.TableRate.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TableRate.dbRowExtended>>(TableRates);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
