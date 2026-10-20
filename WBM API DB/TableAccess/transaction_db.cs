using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region TransactionDetail
        public async Task<Result<int>> TransactionDetail_Insert(wbm_common.DataObjects.TransactionDetail.dbRow TransactionDetail)
        {
            try
            {
                TransactionDetail.ID = 0;
                await _dbContext.TransactionDetail.AddAsync(TransactionDetail);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionDetail.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_Insert - " + TransactionDetail.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionDetail_Update(wbm_common.DataObjects.TransactionDetail.dbRow TransactionDetail)
        {
            try
            {
                _dbContext.TransactionDetail.Attach(TransactionDetail);
                _dbContext.Entry(TransactionDetail).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionDetail.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_Update - " + TransactionDetail.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRow>>> TransactionDetail_ListByListTransactionHeaderIDs(List<int> TransactionHeaderIDs)
        {
            if (TransactionHeaderIDs == null || TransactionHeaderIDs.Count == 0)
            {
                logger.Error("TransactionDetail_ListByListTransactionHeaderIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionDetail.dbRow> TransactionDetails = await _dbContext.TransactionDetail.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && TransactionHeaderIDs.Contains(x.TransactionHeaderID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRow>>(TransactionDetails);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionDetail_ListByListTransactionHeaderIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("Internal Exception occured");
                }
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRow>>> TransactionDetail_ListIncludedDeletedByListTransactionHeaderIDs(List<int> TransactionHeaderIDs)
        {
            if (TransactionHeaderIDs == null || TransactionHeaderIDs.Count == 0)
            {
                logger.Error("TransactionDetail_ListIncludedDeletedByListTransactionHeaderIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionDetail.dbRow> TransactionDetails = await _dbContext.TransactionDetail.AsNoTracking()
                                                                        .Where(x => TransactionHeaderIDs.Contains(x.TransactionHeaderID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRow>>(TransactionDetails);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionDetail_ListIncludedDeletedByListTransactionHeaderIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("Internal Exception occured");
                }
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionDetail.dbRowExtended>> TransactionDetail_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionDetail.dbRowExtended TransactionDetailDb = await (from TransactionDetail in _dbContext.TransactionDetail.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                    join site in _dbContext.Site.AsNoTracking()
                                                                                                        on TransactionDetail.SiteID equals site.ID into sitePR
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                on TransactionDetail.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()
                                                                                                    join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                on TransactionDetail.OwnerID equals Owner.ID into OwnerPR
                                                                                                    from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                    join TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                                                on TransactionDetail.TransactionHeaderID equals TransactionHeader.ID into TransactionHeaderPR
                                                                                                    from TDPR in TransactionHeaderPR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.TransactionDetail.dbRowExtended
                                                                                                    {
                                                                                                        ID = TransactionDetail.ID,
                                                                                                        TransactionHeaderID = TransactionDetail.TransactionHeaderID,
                                                                                                        CompanyID = TransactionDetail.CompanyID,
                                                                                                        OwnerID = TransactionDetail.OwnerID,
                                                                                                        SiteID = TransactionDetail.SiteID,
                                                                                                        TransactionDateTime = TransactionDetail.TransactionDateTime,
                                                                                                        RateCodeID = TransactionDetail.RateCodeID,
                                                                                                        RateCode = TransactionDetail.RateCode,
                                                                                                        ChargeDescription = TransactionDetail.ChargeDescription,
                                                                                                        FullDescription = TransactionDetail.FullDescription,
                                                                                                        Quantity = TransactionDetail.Quantity,
                                                                                                        CalculatedRate = TransactionDetail.CalculatedRate,
                                                                                                        CalculatedExtended = TransactionDetail.CalculatedExtended,
                                                                                                        CalculatedFuel = TransactionDetail.CalculatedFuel,
                                                                                                        Override = TransactionDetail.Override,
                                                                                                        ActualCustomerRate = TransactionDetail.ActualCustomerRate,
                                                                                                        ActualCustomerExtended = TransactionDetail.ActualCustomerExtended,
                                                                                                        ActualCustomerFuel = TransactionDetail.ActualCustomerFuel,
                                                                                                        ExceptionFlag = TransactionDetail.ExceptionFlag,
                                                                                                        DeletedFlag = TransactionDetail.DeletedFlag,
                                                                                                        CreatedDateTime = TransactionDetail.CreatedDateTime,
                                                                                                        CreatedMethod = TransactionDetail.CreatedMethod,
                                                                                                        CreatedUserID = TransactionDetail.CreatedUserID,
                                                                                                        LastAmendedDateTime = TransactionDetail.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = TransactionDetail.LastAmendedMethod,
                                                                                                        LastAmendedUserID = TransactionDetail.LastAmendedUserID,

                                                                                                        Site_PublicID = SPR.PublicID,
                                                                                                        Company_ShortName = CPR.ShortName,
                                                                                                        TransactionHeader_Description = TDPR.Description,
                                                                                                        Owner_Name = OPR.Name
                                                                                                    }).FirstOrDefaultAsync();

                if (TransactionDetailDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionDetail.dbRowExtended>("TransactionDetail not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionDetail.dbRowExtended>(TransactionDetailDb);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionDetail.dbRowExtended>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>> TransactionDetail_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionDetail.dbRowExtended> TransactionDetailDb = await (from TransactionDetail in _dbContext.TransactionDetail.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                          join site in _dbContext.Site.AsNoTracking()
                                                                                                              on TransactionDetail.SiteID equals site.ID into sitePR
                                                                                                          from SPR in sitePR.DefaultIfEmpty()
                                                                                                          join company in _dbContext.Company.AsNoTracking()
                                                                                                      on TransactionDetail.CompanyID equals company.ID into companyPR
                                                                                                          from CPR in companyPR.DefaultIfEmpty()
                                                                                                          join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                      on TransactionDetail.OwnerID equals Owner.ID into OwnerPR
                                                                                                          from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                          join TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                                                      on TransactionDetail.TransactionHeaderID equals TransactionHeader.ID into TransactionHeaderPR
                                                                                                          from TDPR in TransactionHeaderPR.DefaultIfEmpty()

                                                                                                          select new wbm_common.DataObjects.TransactionDetail.dbRowExtended
                                                                                                          {
                                                                                                              ID = TransactionDetail.ID,
                                                                                                              TransactionHeaderID = TransactionDetail.TransactionHeaderID,
                                                                                                              CompanyID = TransactionDetail.CompanyID,
                                                                                                              OwnerID = TransactionDetail.OwnerID,
                                                                                                              SiteID = TransactionDetail.SiteID,
                                                                                                              TransactionDateTime = TransactionDetail.TransactionDateTime,
                                                                                                              RateCodeID = TransactionDetail.RateCodeID,
                                                                                                              RateCode = TransactionDetail.RateCode,
                                                                                                              ChargeDescription = TransactionDetail.ChargeDescription,
                                                                                                              FullDescription = TransactionDetail.FullDescription,
                                                                                                              Quantity = TransactionDetail.Quantity,
                                                                                                              CalculatedRate = TransactionDetail.CalculatedRate,
                                                                                                              CalculatedExtended = TransactionDetail.CalculatedExtended,
                                                                                                              CalculatedFuel = TransactionDetail.CalculatedFuel,
                                                                                                              Override = TransactionDetail.Override,
                                                                                                              ActualCustomerRate = TransactionDetail.ActualCustomerRate,
                                                                                                              ActualCustomerExtended = TransactionDetail.ActualCustomerExtended,
                                                                                                              ActualCustomerFuel = TransactionDetail.ActualCustomerFuel,
                                                                                                              ExceptionFlag = TransactionDetail.ExceptionFlag,
                                                                                                              DeletedFlag = TransactionDetail.DeletedFlag,
                                                                                                              CreatedDateTime = TransactionDetail.CreatedDateTime,
                                                                                                              CreatedMethod = TransactionDetail.CreatedMethod,
                                                                                                              CreatedUserID = TransactionDetail.CreatedUserID,
                                                                                                              LastAmendedDateTime = TransactionDetail.LastAmendedDateTime,
                                                                                                              LastAmendedMethod = TransactionDetail.LastAmendedMethod,
                                                                                                              LastAmendedUserID = TransactionDetail.LastAmendedUserID,

                                                                                                              Site_PublicID = SPR.PublicID,
                                                                                                              Company_ShortName = CPR.ShortName,
                                                                                                              TransactionHeader_Description = TDPR.Description,
                                                                                                              Owner_Name = OPR.Name
                                                                                                          }).ToListAsync();

                if (TransactionDetailDb == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>(new List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>(TransactionDetailDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionDetail.dbRowExtended>> TransactionDetail_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionDetail.dbRowExtended TransactionDetailDb = await (from TransactionDetail in _dbContext.TransactionDetail.AsNoTracking()
                                                                        .Where(x => x.ID == ID).Take(1)
                                                                                                    join site in _dbContext.Site.AsNoTracking()
                                                                                                        on TransactionDetail.SiteID equals site.ID into sitePR
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                on TransactionDetail.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()
                                                                                                    join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                on TransactionDetail.OwnerID equals Owner.ID into OwnerPR
                                                                                                    from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                    join TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                                                on TransactionDetail.TransactionHeaderID equals TransactionHeader.ID into TransactionHeaderPR
                                                                                                    from TDPR in TransactionHeaderPR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.TransactionDetail.dbRowExtended
                                                                                                    {
                                                                                                        ID = TransactionDetail.ID,
                                                                                                        TransactionHeaderID = TransactionDetail.TransactionHeaderID,
                                                                                                        CompanyID = TransactionDetail.CompanyID,
                                                                                                        OwnerID = TransactionDetail.OwnerID,
                                                                                                        SiteID = TransactionDetail.SiteID,
                                                                                                        TransactionDateTime = TransactionDetail.TransactionDateTime,
                                                                                                        RateCodeID = TransactionDetail.RateCodeID,
                                                                                                        RateCode = TransactionDetail.RateCode,
                                                                                                        ChargeDescription = TransactionDetail.ChargeDescription,
                                                                                                        FullDescription = TransactionDetail.FullDescription,
                                                                                                        Quantity = TransactionDetail.Quantity,
                                                                                                        CalculatedRate = TransactionDetail.CalculatedRate,
                                                                                                        CalculatedExtended = TransactionDetail.CalculatedExtended,
                                                                                                        CalculatedFuel = TransactionDetail.CalculatedFuel,
                                                                                                        Override = TransactionDetail.Override,
                                                                                                        ActualCustomerRate = TransactionDetail.ActualCustomerRate,
                                                                                                        ActualCustomerExtended = TransactionDetail.ActualCustomerExtended,
                                                                                                        ActualCustomerFuel = TransactionDetail.ActualCustomerFuel,
                                                                                                        ExceptionFlag = TransactionDetail.ExceptionFlag,
                                                                                                        DeletedFlag = TransactionDetail.DeletedFlag,
                                                                                                        CreatedDateTime = TransactionDetail.CreatedDateTime,
                                                                                                        CreatedMethod = TransactionDetail.CreatedMethod,
                                                                                                        CreatedUserID = TransactionDetail.CreatedUserID,
                                                                                                        LastAmendedDateTime = TransactionDetail.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = TransactionDetail.LastAmendedMethod,
                                                                                                        LastAmendedUserID = TransactionDetail.LastAmendedUserID,

                                                                                                        Site_PublicID = SPR.PublicID,
                                                                                                        Company_ShortName = CPR.ShortName,
                                                                                                        TransactionHeader_Description = TDPR.Description,
                                                                                                        Owner_Name = OPR.Name
                                                                                                    }).FirstOrDefaultAsync();

                if (TransactionDetailDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionDetail.dbRowExtended>("TransactionDetail not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionDetail.dbRowExtended>(TransactionDetailDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionDetail.dbRowExtended>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>> TransactionDetail_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionDetail.dbRowExtended> TransactionDetails = await (from TransactionDetail in _dbContext.TransactionDetail.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                         join site in _dbContext.Site.AsNoTracking()
                                                                                                             on TransactionDetail.SiteID equals site.ID into sitePR
                                                                                                         from SPR in sitePR.DefaultIfEmpty()
                                                                                                         join company in _dbContext.Company.AsNoTracking()
                                                                                                     on TransactionDetail.CompanyID equals company.ID into companyPR
                                                                                                         from CPR in companyPR.DefaultIfEmpty()
                                                                                                         join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                     on TransactionDetail.OwnerID equals Owner.ID into OwnerPR
                                                                                                         from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                         join TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                                                     on TransactionDetail.TransactionHeaderID equals TransactionHeader.ID into TransactionHeaderPR
                                                                                                         from TDPR in TransactionHeaderPR.DefaultIfEmpty()

                                                                                                         select new wbm_common.DataObjects.TransactionDetail.dbRowExtended
                                                                                                         {
                                                                                                             ID = TransactionDetail.ID,
                                                                                                             TransactionHeaderID = TransactionDetail.TransactionHeaderID,
                                                                                                             CompanyID = TransactionDetail.CompanyID,
                                                                                                             OwnerID = TransactionDetail.OwnerID,
                                                                                                             SiteID = TransactionDetail.SiteID,
                                                                                                             TransactionDateTime = TransactionDetail.TransactionDateTime,
                                                                                                             RateCodeID = TransactionDetail.RateCodeID,
                                                                                                             RateCode = TransactionDetail.RateCode,
                                                                                                             ChargeDescription = TransactionDetail.ChargeDescription,
                                                                                                             FullDescription = TransactionDetail.FullDescription,
                                                                                                             Quantity = TransactionDetail.Quantity,
                                                                                                             CalculatedRate = TransactionDetail.CalculatedRate,
                                                                                                             CalculatedExtended = TransactionDetail.CalculatedExtended,
                                                                                                             CalculatedFuel = TransactionDetail.CalculatedFuel,
                                                                                                             Override = TransactionDetail.Override,
                                                                                                             ActualCustomerRate = TransactionDetail.ActualCustomerRate,
                                                                                                             ActualCustomerExtended = TransactionDetail.ActualCustomerExtended,
                                                                                                             ActualCustomerFuel = TransactionDetail.ActualCustomerFuel,
                                                                                                             ExceptionFlag = TransactionDetail.ExceptionFlag,
                                                                                                             DeletedFlag = TransactionDetail.DeletedFlag,
                                                                                                             CreatedDateTime = TransactionDetail.CreatedDateTime,
                                                                                                             CreatedMethod = TransactionDetail.CreatedMethod,
                                                                                                             CreatedUserID = TransactionDetail.CreatedUserID,
                                                                                                             LastAmendedDateTime = TransactionDetail.LastAmendedDateTime,
                                                                                                             LastAmendedMethod = TransactionDetail.LastAmendedMethod,
                                                                                                             LastAmendedUserID = TransactionDetail.LastAmendedUserID,

                                                                                                             Site_PublicID = SPR.PublicID,
                                                                                                             Company_ShortName = CPR.ShortName,
                                                                                                             TransactionHeader_Description = TDPR.Description,
                                                                                                             Owner_Name = OPR.Name
                                                                                                         }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>(TransactionDetails);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>> TransactionDetail_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionDetail_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionDetail.dbRowExtended> TransactionDetails = await (from TransactionDetail in _dbContext.TransactionDetail.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                             join site in _dbContext.Site.AsNoTracking()
                                                                                                                 on TransactionDetail.SiteID equals site.ID into sitePR
                                                                                                             from SPR in sitePR.DefaultIfEmpty()
                                                                                                             join company in _dbContext.Company.AsNoTracking()
                                                                                                         on TransactionDetail.CompanyID equals company.ID into companyPR
                                                                                                             from CPR in companyPR.DefaultIfEmpty()
                                                                                                             join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                         on TransactionDetail.OwnerID equals Owner.ID into OwnerPR
                                                                                                             from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                             join TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                                                         on TransactionDetail.TransactionHeaderID equals TransactionHeader.ID into TransactionHeaderPR
                                                                                                             from TDPR in TransactionHeaderPR.DefaultIfEmpty()

                                                                                                             select new wbm_common.DataObjects.TransactionDetail.dbRowExtended
                                                                                                             {
                                                                                                                 ID = TransactionDetail.ID,
                                                                                                                 TransactionHeaderID = TransactionDetail.TransactionHeaderID,
                                                                                                                 CompanyID = TransactionDetail.CompanyID,
                                                                                                                 OwnerID = TransactionDetail.OwnerID,
                                                                                                                 SiteID = TransactionDetail.SiteID,
                                                                                                                 TransactionDateTime = TransactionDetail.TransactionDateTime,
                                                                                                                 RateCodeID = TransactionDetail.RateCodeID,
                                                                                                                 RateCode = TransactionDetail.RateCode,
                                                                                                                 ChargeDescription = TransactionDetail.ChargeDescription,
                                                                                                                 FullDescription = TransactionDetail.FullDescription,
                                                                                                                 Quantity = TransactionDetail.Quantity,
                                                                                                                 CalculatedRate = TransactionDetail.CalculatedRate,
                                                                                                                 CalculatedExtended = TransactionDetail.CalculatedExtended,
                                                                                                                 CalculatedFuel = TransactionDetail.CalculatedFuel,
                                                                                                                 Override = TransactionDetail.Override,
                                                                                                                 ActualCustomerRate = TransactionDetail.ActualCustomerRate,
                                                                                                                 ActualCustomerExtended = TransactionDetail.ActualCustomerExtended,
                                                                                                                 ActualCustomerFuel = TransactionDetail.ActualCustomerFuel,
                                                                                                                 ExceptionFlag = TransactionDetail.ExceptionFlag,
                                                                                                                 DeletedFlag = TransactionDetail.DeletedFlag,
                                                                                                                 CreatedDateTime = TransactionDetail.CreatedDateTime,
                                                                                                                 CreatedMethod = TransactionDetail.CreatedMethod,
                                                                                                                 CreatedUserID = TransactionDetail.CreatedUserID,
                                                                                                                 LastAmendedDateTime = TransactionDetail.LastAmendedDateTime,
                                                                                                                 LastAmendedMethod = TransactionDetail.LastAmendedMethod,
                                                                                                                 LastAmendedUserID = TransactionDetail.LastAmendedUserID,

                                                                                                                 Site_PublicID = SPR.PublicID,
                                                                                                                 Company_ShortName = CPR.ShortName,
                                                                                                                 TransactionHeader_Description = TDPR.Description,
                                                                                                                 Owner_Name = OPR.Name
                                                                                                             }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>(TransactionDetails);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionDetail_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>> TransactionDetail_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionDetail.dbRowExtended> TransactionDetails = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.TransactionDetail.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>(TransactionDetails);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion

        #region TransactionException
        public async Task<Result<int>> TransactionException_Insert(wbm_common.DataObjects.TransactionException.dbRow TransactionException)
        {
            try
            {
                await _dbContext.TransactionException.AddAsync(TransactionException);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionException.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_Insert - " + TransactionException.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionException_Update(wbm_common.DataObjects.TransactionException.dbRow TransactionException)
        {
            try
            {
                _dbContext.TransactionException.Attach(TransactionException);
                _dbContext.Entry(TransactionException).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionException.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_Update - " + TransactionException.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRow>>> TransactionException_ListByListTransactionHeaderIDs(List<int> TransactionHeaderIDs)
        {
            if (TransactionHeaderIDs == null || TransactionHeaderIDs.Count == 0)
            {
                logger.Error("TransactionException_ListByListTransactionHeaderIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionException.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionException.dbRow> TransactionExceptions = await _dbContext.TransactionException.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && TransactionHeaderIDs.Contains(x.TransactionHeaderID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRow>>(TransactionExceptions);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionException_ListByListTransactionHeaderIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRow>>> TransactionException_ListIncludedDeletedByListTransactionHeaderIDs(List<int> TransactionHeaderIDs)
        {
            if (TransactionHeaderIDs == null || TransactionHeaderIDs.Count == 0)
            {
                logger.Error("TransactionException_ListIncludedDeletedByListTransactionHeaderIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionException.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionException.dbRow> TransactionExceptions = await _dbContext.TransactionException.AsNoTracking()
                                                                        .Where(x => TransactionHeaderIDs.Contains(x.TransactionHeaderID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRow>>(TransactionExceptions);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionException_ListIncludedDeletedByListTransactionHeaderIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>> TransactionException_ListExtendedByListTransactionHeaderIDs(List<int> TransactionHeaderIDs)
        {
            if (TransactionHeaderIDs == null || TransactionHeaderIDs.Count == 0)
            {
                logger.Error("TransactionException_ListByListTransactionHeaderIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionException.dbRowExtended> TransactionExceptions = await (from TransactionException in _dbContext.TransactionException.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && TransactionHeaderIDs.Contains(x.TransactionHeaderID))
                                                                                                           join TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                                                               on TransactionException.TransactionHeaderID equals TransactionHeader.ID into TransactionHeaderPR
                                                                                                           from THPR in TransactionHeaderPR.DefaultIfEmpty()
                                                                                                           join TransactionDetail in _dbContext.TransactionDetail.AsNoTracking()
                                                                                                       on TransactionException.TransactionDetailID equals TransactionDetail.ID into TransactionDetailPR
                                                                                                           from TDPR in TransactionDetailPR.DefaultIfEmpty()
                                                                                                           join TransactionExceptionType in _dbContext.TransactionExceptionType.AsNoTracking()
                                                                                                       on TransactionException.TransactionExceptionTypeID equals TransactionExceptionType.ID into TransactionExceptionTypePR
                                                                                                           from TETPR in TransactionExceptionTypePR.DefaultIfEmpty()

                                                                                                           select new wbm_common.DataObjects.TransactionException.dbRowExtended
                                                                                                           {
                                                                                                               ID = TransactionException.ID,
                                                                                                               TransactionHeaderID = TransactionException.TransactionHeaderID,
                                                                                                               TransactionDetailID = TransactionException.TransactionDetailID,
                                                                                                               TransactionExceptionTypeID = TransactionException.TransactionExceptionTypeID,
                                                                                                               Description = TransactionException.Description,
                                                                                                               ClearedFlag = TransactionException.ClearedFlag,
                                                                                                               ClearedDateTime = TransactionException.ClearedDateTime,
                                                                                                               ClearedMethod = TransactionException.ClearedMethod,
                                                                                                               ClearedUserID = TransactionException.ClearedUserID,
                                                                                                               DeletedFlag = TransactionException.DeletedFlag,
                                                                                                               CreatedDateTime = TransactionException.CreatedDateTime,
                                                                                                               CreatedMethod = TransactionException.CreatedMethod,
                                                                                                               CreatedUserID = TransactionException.CreatedUserID,
                                                                                                               LastAmendedDateTime = TransactionException.LastAmendedDateTime,
                                                                                                               LastAmendedMethod = TransactionException.LastAmendedMethod,
                                                                                                               LastAmendedUserID = TransactionException.LastAmendedUserID,

                                                                                                               TransactionExceptionType_PublicID = TETPR.PublicID,
                                                                                                               TransactionHeader_Description = THPR.Description,
                                                                                                               TransactionDetail_FullDescription = TDPR.FullDescription
                                                                                                           }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>(TransactionExceptions);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionException_ListByListTransactionHeaderIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionException.dbRow>> TransactionException_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionException.dbRow TransactionExceptionDb = await _dbContext.TransactionException.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).FirstOrDefaultAsync();

                if (TransactionExceptionDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionException.dbRow>("TransactionException not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionException.dbRow>(TransactionExceptionDb);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionException.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>> TransactionException_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionException.dbRowExtended> TransactionExceptionDb = await (from TransactionException in _dbContext.TransactionException.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                                join TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                                                                    on TransactionException.TransactionHeaderID equals TransactionHeader.ID into TransactionHeaderPR
                                                                                                                from THPR in TransactionHeaderPR.DefaultIfEmpty()
                                                                                                                join TransactionDetail in _dbContext.TransactionDetail.AsNoTracking()
                                                                                                            on TransactionException.TransactionDetailID equals TransactionDetail.ID into TransactionDetailPR
                                                                                                                from TDPR in TransactionDetailPR.DefaultIfEmpty()
                                                                                                                join TransactionExceptionType in _dbContext.TransactionExceptionType.AsNoTracking()
                                                                                                            on TransactionException.TransactionExceptionTypeID equals TransactionExceptionType.ID into TransactionExceptionTypePR
                                                                                                                from TETPR in TransactionExceptionTypePR.DefaultIfEmpty()

                                                                                                                select new wbm_common.DataObjects.TransactionException.dbRowExtended
                                                                                                                {
                                                                                                                    ID = TransactionException.ID,
                                                                                                                    TransactionHeaderID = TransactionException.TransactionHeaderID,
                                                                                                                    TransactionDetailID = TransactionException.TransactionDetailID,
                                                                                                                    TransactionExceptionTypeID = TransactionException.TransactionExceptionTypeID,
                                                                                                                    Description = TransactionException.Description,
                                                                                                                    ClearedFlag = TransactionException.ClearedFlag,
                                                                                                                    ClearedDateTime = TransactionException.ClearedDateTime,
                                                                                                                    ClearedMethod = TransactionException.ClearedMethod,
                                                                                                                    ClearedUserID = TransactionException.ClearedUserID,
                                                                                                                    DeletedFlag = TransactionException.DeletedFlag,
                                                                                                                    CreatedDateTime = TransactionException.CreatedDateTime,
                                                                                                                    CreatedMethod = TransactionException.CreatedMethod,
                                                                                                                    CreatedUserID = TransactionException.CreatedUserID,
                                                                                                                    LastAmendedDateTime = TransactionException.LastAmendedDateTime,
                                                                                                                    LastAmendedMethod = TransactionException.LastAmendedMethod,
                                                                                                                    LastAmendedUserID = TransactionException.LastAmendedUserID,

                                                                                                                    TransactionExceptionType_PublicID = TETPR.PublicID,
                                                                                                                    TransactionHeader_Description = THPR.Description,
                                                                                                                    TransactionDetail_FullDescription = TDPR.FullDescription
                                                                                                                }).ToListAsync();

                if (TransactionExceptionDb == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>(new List<wbm_common.DataObjects.TransactionException.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>(TransactionExceptionDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionException.dbRow>> TransactionException_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionException.dbRow TransactionExceptionDb = await _dbContext.TransactionException.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (TransactionExceptionDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionException.dbRow>("TransactionException not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionException.dbRow>(TransactionExceptionDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionException.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>> TransactionException_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionException.dbRowExtended> TransactionExceptions = await (from TransactionException in _dbContext.TransactionException.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                               join TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                                                                   on TransactionException.TransactionHeaderID equals TransactionHeader.ID into TransactionHeaderPR
                                                                                                               from THPR in TransactionHeaderPR.DefaultIfEmpty()
                                                                                                               join TransactionDetail in _dbContext.TransactionDetail.AsNoTracking()
                                                                                                           on TransactionException.TransactionDetailID equals TransactionDetail.ID into TransactionDetailPR
                                                                                                               from TDPR in TransactionDetailPR.DefaultIfEmpty()
                                                                                                               join TransactionExceptionType in _dbContext.TransactionExceptionType.AsNoTracking()
                                                                                                           on TransactionException.TransactionExceptionTypeID equals TransactionExceptionType.ID into TransactionExceptionTypePR
                                                                                                               from TETPR in TransactionExceptionTypePR.DefaultIfEmpty()

                                                                                                               select new wbm_common.DataObjects.TransactionException.dbRowExtended
                                                                                                               {
                                                                                                                   ID = TransactionException.ID,
                                                                                                                   TransactionHeaderID = TransactionException.TransactionHeaderID,
                                                                                                                   TransactionDetailID = TransactionException.TransactionDetailID,
                                                                                                                   TransactionExceptionTypeID = TransactionException.TransactionExceptionTypeID,
                                                                                                                   Description = TransactionException.Description,
                                                                                                                   ClearedFlag = TransactionException.ClearedFlag,
                                                                                                                   ClearedDateTime = TransactionException.ClearedDateTime,
                                                                                                                   ClearedMethod = TransactionException.ClearedMethod,
                                                                                                                   ClearedUserID = TransactionException.ClearedUserID,
                                                                                                                   DeletedFlag = TransactionException.DeletedFlag,
                                                                                                                   CreatedDateTime = TransactionException.CreatedDateTime,
                                                                                                                   CreatedMethod = TransactionException.CreatedMethod,
                                                                                                                   CreatedUserID = TransactionException.CreatedUserID,
                                                                                                                   LastAmendedDateTime = TransactionException.LastAmendedDateTime,
                                                                                                                   LastAmendedMethod = TransactionException.LastAmendedMethod,
                                                                                                                   LastAmendedUserID = TransactionException.LastAmendedUserID,

                                                                                                                   TransactionExceptionType_PublicID = TETPR.PublicID,
                                                                                                                   TransactionHeader_Description = THPR.Description,
                                                                                                                   TransactionDetail_FullDescription = TDPR.FullDescription
                                                                                                               }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>(TransactionExceptions);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>> TransactionException_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionException_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionException.dbRowExtended> TransactionExceptions = await (from TransactionException in _dbContext.TransactionException.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                                   join TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                                                                       on TransactionException.TransactionHeaderID equals TransactionHeader.ID into TransactionHeaderPR
                                                                                                                   from THPR in TransactionHeaderPR.DefaultIfEmpty()
                                                                                                                   join TransactionDetail in _dbContext.TransactionDetail.AsNoTracking()
                                                                                                               on TransactionException.TransactionDetailID equals TransactionDetail.ID into TransactionDetailPR
                                                                                                                   from TDPR in TransactionDetailPR.DefaultIfEmpty()
                                                                                                                   join TransactionExceptionType in _dbContext.TransactionExceptionType.AsNoTracking()
                                                                                                               on TransactionException.TransactionExceptionTypeID equals TransactionExceptionType.ID into TransactionExceptionTypePR
                                                                                                                   from TETPR in TransactionExceptionTypePR.DefaultIfEmpty()

                                                                                                                   select new wbm_common.DataObjects.TransactionException.dbRowExtended
                                                                                                                   {
                                                                                                                       ID = TransactionException.ID,
                                                                                                                       TransactionHeaderID = TransactionException.TransactionHeaderID,
                                                                                                                       TransactionDetailID = TransactionException.TransactionDetailID,
                                                                                                                       TransactionExceptionTypeID = TransactionException.TransactionExceptionTypeID,
                                                                                                                       Description = TransactionException.Description,
                                                                                                                       ClearedFlag = TransactionException.ClearedFlag,
                                                                                                                       ClearedDateTime = TransactionException.ClearedDateTime,
                                                                                                                       ClearedMethod = TransactionException.ClearedMethod,
                                                                                                                       ClearedUserID = TransactionException.ClearedUserID,
                                                                                                                       DeletedFlag = TransactionException.DeletedFlag,
                                                                                                                       CreatedDateTime = TransactionException.CreatedDateTime,
                                                                                                                       CreatedMethod = TransactionException.CreatedMethod,
                                                                                                                       CreatedUserID = TransactionException.CreatedUserID,
                                                                                                                       LastAmendedDateTime = TransactionException.LastAmendedDateTime,
                                                                                                                       LastAmendedMethod = TransactionException.LastAmendedMethod,
                                                                                                                       LastAmendedUserID = TransactionException.LastAmendedUserID,

                                                                                                                       TransactionExceptionType_PublicID = TETPR.PublicID,
                                                                                                                       TransactionHeader_Description = THPR.Description,
                                                                                                                       TransactionDetail_FullDescription = TDPR.FullDescription
                                                                                                                   }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>(TransactionExceptions);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionException_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>> TransactionException_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionException.dbRowExtended> TransactionExceptions = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.TransactionException.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>(TransactionExceptions);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion

        #region TransactionExceptionType
        public async Task<Result<int>> TransactionExceptionType_Insert(wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType)
        {
            try
            {
                await _dbContext.TransactionExceptionType.AddAsync(TransactionExceptionType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionExceptionType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_Insert - " + TransactionExceptionType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionExceptionType_Update(wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType)
        {
            try
            {
                _dbContext.TransactionExceptionType.Attach(TransactionExceptionType);
                _dbContext.Entry(TransactionExceptionType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionExceptionType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_Update - " + TransactionExceptionType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionExceptionType.dbRow>> TransactionExceptionType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType = await _dbContext.TransactionExceptionType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionExceptionType == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionExceptionType.dbRow>("TransactionExceptionType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionExceptionType.dbRow>(TransactionExceptionType);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionExceptionType.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.TransactionExceptionType.dbRow>> TransactionExceptionType_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType = await _dbContext.TransactionExceptionType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (TransactionExceptionType == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionExceptionType.dbRow>("TransactionExceptionType not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.TransactionExceptionType.dbRow>(TransactionExceptionType);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionExceptionType.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>> TransactionExceptionType_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType = await _dbContext.TransactionExceptionType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionExceptionType == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(new List<wbm_common.DataObjects.TransactionExceptionType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(new List<wbm_common.DataObjects.TransactionExceptionType.dbRow> { TransactionExceptionType });
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionExceptionType.dbRow>> TransactionExceptionType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType = await _dbContext.TransactionExceptionType.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (TransactionExceptionType == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionExceptionType.dbRow>("TransactionExceptionType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionExceptionType.dbRow>(TransactionExceptionType);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionExceptionType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>> TransactionExceptionType_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionExceptionType.dbRow> TransactionExceptionTypes = await _dbContext.TransactionExceptionType.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(TransactionExceptionTypes);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>> TransactionExceptionType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionExceptionType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionExceptionType.dbRow> TransactionExceptionTypes = await _dbContext.TransactionExceptionType.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(TransactionExceptionTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionExceptionType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>> TransactionExceptionType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionExceptionType.dbRow> TransactionExceptionTypes = await _dbContext.TransactionExceptionType.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(TransactionExceptionTypes);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region TransactionHeader
        public async Task<Result<int>> TransactionHeader_Insert(wbm_common.DataObjects.TransactionHeader.dbRow TransactionHeader)
        {
            try
            {
                TransactionHeader.ID = 0;
                await _dbContext.TransactionHeader.AddAsync(TransactionHeader);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_Insert - " + TransactionHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionHeader_Update(wbm_common.DataObjects.TransactionHeader.dbRow TransactionHeader)
        {
            try
            {
                _dbContext.TransactionHeader.Attach(TransactionHeader);
                _dbContext.Entry(TransactionHeader).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_Update - " + TransactionHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionHeader.dbRowExtended>> TransactionHeader_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionHeader.dbRowExtended TransactionHeaderDb = await (from TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).Take(1)
                                                                                                    join site in _dbContext.Site.AsNoTracking()
                                                                                                        on TransactionHeader.SiteID equals site.ID into sitePR
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                on TransactionHeader.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()
                                                                                                    join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                on TransactionHeader.OwnerID equals Owner.ID into OwnerPR
                                                                                                    from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                    join TransactionSource in _dbContext.TransactionSource.AsNoTracking()
                                                                                                on TransactionHeader.TransactionSourceID equals TransactionSource.ID into TransactionSourcePR
                                                                                                    from TSPR in TransactionSourcePR.DefaultIfEmpty()
                                                                                                    join TransactionType in _dbContext.TransactionExceptionType.AsNoTracking()
                                                                                                on TransactionHeader.TransactionTypeID equals TransactionType.ID into TransactionTypePR
                                                                                                    from TTPR in TransactionTypePR.DefaultIfEmpty()
                                                                                                    join Invoice in _dbContext.InvoiceCycle.AsNoTracking()
                                                                                                on TransactionHeader.InvoiceHeaderID equals Invoice.ID into InvoicePR
                                                                                                    from IPR in InvoicePR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.TransactionHeader.dbRowExtended
                                                                                                    {
                                                                                                        ID = TransactionHeader.ID,
                                                                                                        CompanyID = TransactionHeader.CompanyID,
                                                                                                        OwnerID = TransactionHeader.OwnerID,
                                                                                                        SiteID = TransactionHeader.SiteID,
                                                                                                        TransactionSourceID = TransactionHeader.TransactionSourceID,
                                                                                                        TransactionDateTime = TransactionHeader.TransactionDateTime,
                                                                                                        TransactionTypeID = TransactionHeader.TransactionTypeID,
                                                                                                        TransactionReference = TransactionHeader.TransactionReference,
                                                                                                        Description = TransactionHeader.Description,
                                                                                                        InvoiceHeaderID = TransactionHeader.InvoiceHeaderID,
                                                                                                        InvoiceDateTime = TransactionHeader.InvoiceDateTime,
                                                                                                        InvoiceExFuel = TransactionHeader.InvoiceExFuel,
                                                                                                        InvoiceFuel = TransactionHeader.InvoiceFuel,
                                                                                                        InvoiceGST = TransactionHeader.InvoiceGST,
                                                                                                        InvoiceExGST = TransactionHeader.InvoiceExGST,
                                                                                                        ExceptionFlag = TransactionHeader.ExceptionFlag,
                                                                                                        CanInvoice = TransactionHeader.CanInvoice,
                                                                                                        DeletedFlag = TransactionHeader.DeletedFlag,
                                                                                                        CreatedDateTime = TransactionHeader.CreatedDateTime,
                                                                                                        CreatedMethod = TransactionHeader.CreatedMethod,
                                                                                                        CreatedUserID = TransactionHeader.CreatedUserID,
                                                                                                        LastAmendedDateTime = TransactionHeader.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = TransactionHeader.LastAmendedMethod,
                                                                                                        LastAmendedUserID = TransactionHeader.LastAmendedUserID,

                                                                                                        Site_PublicID = SPR.PublicID,
                                                                                                        Company_ShortName = CPR.ShortName,
                                                                                                        Owner_Name = OPR.Name,
                                                                                                        TransactionSource_PublicID = TSPR.PublicID,
                                                                                                        TransactionType_PublicID = TTPR.PublicID,
                                                                                                        InvoiceHeader_InvoiceNumber = IPR.Description
                                                                                                    }).FirstOrDefaultAsync();

                if (TransactionHeaderDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionHeader.dbRowExtended>("TransactionHeader not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionHeader.dbRowExtended>(TransactionHeaderDb);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionHeader.dbRowExtended>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.TransactionHeader.dbRow>> TransactionHeader_SearchByReferenceTransactionType(string reference, int type)
        {
            try
            {
                wbm_common.DataObjects.TransactionHeader.dbRow TransactionHeaderDb = await _dbContext.TransactionHeader.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.TransactionReference == reference && 
                                                                        (x.TransactionTypeID == type || type == 0)).FirstOrDefaultAsync();

                if (TransactionHeaderDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionHeader.dbRow>("TransactionHeader not found : " + reference + ", " + type.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionHeader.dbRow>(TransactionHeaderDb);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_SearchByReference - " + reference + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionHeader.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>> TransactionHeader_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionHeader.dbRowExtended> TransactionHeaderDb = await (from TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                          join site in _dbContext.Site.AsNoTracking()
                                                                                                              on TransactionHeader.SiteID equals site.ID into sitePR
                                                                                                          from SPR in sitePR.DefaultIfEmpty()
                                                                                                          join company in _dbContext.Company.AsNoTracking()
                                                                                                      on TransactionHeader.CompanyID equals company.ID into companyPR
                                                                                                          from CPR in companyPR.DefaultIfEmpty()
                                                                                                          join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                      on TransactionHeader.OwnerID equals Owner.ID into OwnerPR
                                                                                                          from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                          join TransactionSource in _dbContext.TransactionSource.AsNoTracking()
                                                                                                      on TransactionHeader.TransactionSourceID equals TransactionSource.ID into TransactionSourcePR
                                                                                                          from TSPR in TransactionSourcePR.DefaultIfEmpty()
                                                                                                          join TransactionType in _dbContext.TransactionExceptionType.AsNoTracking()
                                                                                                      on TransactionHeader.TransactionTypeID equals TransactionType.ID into TransactionTypePR
                                                                                                          from TTPR in TransactionTypePR.DefaultIfEmpty()
                                                                                                          join Invoice in _dbContext.InvoiceCycle.AsNoTracking()
                                                                                                      on TransactionHeader.InvoiceHeaderID equals Invoice.ID into InvoicePR
                                                                                                          from IPR in InvoicePR.DefaultIfEmpty()

                                                                                                          select new wbm_common.DataObjects.TransactionHeader.dbRowExtended
                                                                                                          {
                                                                                                              ID = TransactionHeader.ID,
                                                                                                              CompanyID = TransactionHeader.CompanyID,
                                                                                                              OwnerID = TransactionHeader.OwnerID,
                                                                                                              SiteID = TransactionHeader.SiteID,
                                                                                                              TransactionSourceID = TransactionHeader.TransactionSourceID,
                                                                                                              TransactionDateTime = TransactionHeader.TransactionDateTime,
                                                                                                              TransactionTypeID = TransactionHeader.TransactionTypeID,
                                                                                                              TransactionReference = TransactionHeader.TransactionReference,
                                                                                                              Description = TransactionHeader.Description,
                                                                                                              InvoiceHeaderID = TransactionHeader.InvoiceHeaderID,
                                                                                                              InvoiceDateTime = TransactionHeader.InvoiceDateTime,
                                                                                                              InvoiceExFuel = TransactionHeader.InvoiceExFuel,
                                                                                                              InvoiceFuel = TransactionHeader.InvoiceFuel,
                                                                                                              InvoiceGST = TransactionHeader.InvoiceGST,
                                                                                                              InvoiceExGST = TransactionHeader.InvoiceExGST,
                                                                                                              ExceptionFlag = TransactionHeader.ExceptionFlag,
                                                                                                              CanInvoice = TransactionHeader.CanInvoice,
                                                                                                              DeletedFlag = TransactionHeader.DeletedFlag,
                                                                                                              CreatedDateTime = TransactionHeader.CreatedDateTime,
                                                                                                              CreatedMethod = TransactionHeader.CreatedMethod,
                                                                                                              CreatedUserID = TransactionHeader.CreatedUserID,
                                                                                                              LastAmendedDateTime = TransactionHeader.LastAmendedDateTime,
                                                                                                              LastAmendedMethod = TransactionHeader.LastAmendedMethod,
                                                                                                              LastAmendedUserID = TransactionHeader.LastAmendedUserID,

                                                                                                              Site_PublicID = SPR.PublicID,
                                                                                                              Company_ShortName = CPR.ShortName,
                                                                                                              Owner_Name = OPR.Name,
                                                                                                              TransactionSource_PublicID = TSPR.PublicID,
                                                                                                              TransactionType_PublicID = TTPR.PublicID,
                                                                                                              InvoiceHeader_InvoiceNumber = IPR.Description
                                                                                                          }).ToListAsync();

                if (TransactionHeaderDb == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>(new List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>(TransactionHeaderDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionHeader.dbRowExtended>> TransactionHeader_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionHeader.dbRowExtended TransactionHeaderDb = await (from TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                        .Where(x => x.ID == ID).Take(1)
                                                                                                    join site in _dbContext.Site.AsNoTracking()
                                                                                                        on TransactionHeader.SiteID equals site.ID into sitePR
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                on TransactionHeader.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()
                                                                                                    join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                on TransactionHeader.OwnerID equals Owner.ID into OwnerPR
                                                                                                    from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                    join TransactionSource in _dbContext.TransactionSource.AsNoTracking()
                                                                                                on TransactionHeader.TransactionSourceID equals TransactionSource.ID into TransactionSourcePR
                                                                                                    from TSPR in TransactionSourcePR.DefaultIfEmpty()
                                                                                                    join TransactionType in _dbContext.TransactionExceptionType.AsNoTracking()
                                                                                                on TransactionHeader.TransactionTypeID equals TransactionType.ID into TransactionTypePR
                                                                                                    from TTPR in TransactionTypePR.DefaultIfEmpty()
                                                                                                    join Invoice in _dbContext.InvoiceCycle.AsNoTracking()
                                                                                                on TransactionHeader.InvoiceHeaderID equals Invoice.ID into InvoicePR
                                                                                                    from IPR in InvoicePR.DefaultIfEmpty()

                                                                                                    select new wbm_common.DataObjects.TransactionHeader.dbRowExtended
                                                                                                    {
                                                                                                        ID = TransactionHeader.ID,
                                                                                                        CompanyID = TransactionHeader.CompanyID,
                                                                                                        OwnerID = TransactionHeader.OwnerID,
                                                                                                        SiteID = TransactionHeader.SiteID,
                                                                                                        TransactionSourceID = TransactionHeader.TransactionSourceID,
                                                                                                        TransactionDateTime = TransactionHeader.TransactionDateTime,
                                                                                                        TransactionTypeID = TransactionHeader.TransactionTypeID,
                                                                                                        TransactionReference = TransactionHeader.TransactionReference,
                                                                                                        Description = TransactionHeader.Description,
                                                                                                        InvoiceHeaderID = TransactionHeader.InvoiceHeaderID,
                                                                                                        InvoiceDateTime = TransactionHeader.InvoiceDateTime,
                                                                                                        InvoiceExFuel = TransactionHeader.InvoiceExFuel,
                                                                                                        InvoiceFuel = TransactionHeader.InvoiceFuel,
                                                                                                        InvoiceGST = TransactionHeader.InvoiceGST,
                                                                                                        InvoiceExGST = TransactionHeader.InvoiceExGST,
                                                                                                        ExceptionFlag = TransactionHeader.ExceptionFlag,
                                                                                                        CanInvoice = TransactionHeader.CanInvoice,
                                                                                                        DeletedFlag = TransactionHeader.DeletedFlag,
                                                                                                        CreatedDateTime = TransactionHeader.CreatedDateTime,
                                                                                                        CreatedMethod = TransactionHeader.CreatedMethod,
                                                                                                        CreatedUserID = TransactionHeader.CreatedUserID,
                                                                                                        LastAmendedDateTime = TransactionHeader.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = TransactionHeader.LastAmendedMethod,
                                                                                                        LastAmendedUserID = TransactionHeader.LastAmendedUserID,

                                                                                                        Site_PublicID = SPR.PublicID,
                                                                                                        Company_ShortName = CPR.ShortName,
                                                                                                        Owner_Name = OPR.Name,
                                                                                                        TransactionSource_PublicID = TSPR.PublicID,
                                                                                                        TransactionType_PublicID = TTPR.PublicID,
                                                                                                        InvoiceHeader_InvoiceNumber = IPR.Description
                                                                                                    }).FirstOrDefaultAsync();

                if (TransactionHeaderDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionHeader.dbRowExtended>("TransactionHeader not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionHeader.dbRowExtended>(TransactionHeaderDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionHeader.dbRowExtended>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>> TransactionHeader_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionHeader.dbRowExtended> TransactionHeaders = await (from TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                         join site in _dbContext.Site.AsNoTracking()
                                                                                                             on TransactionHeader.SiteID equals site.ID into sitePR
                                                                                                         from SPR in sitePR.DefaultIfEmpty()
                                                                                                         join company in _dbContext.Company.AsNoTracking()
                                                                                                     on TransactionHeader.CompanyID equals company.ID into companyPR
                                                                                                         from CPR in companyPR.DefaultIfEmpty()
                                                                                                         join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                     on TransactionHeader.OwnerID equals Owner.ID into OwnerPR
                                                                                                         from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                         join TransactionSource in _dbContext.TransactionSource.AsNoTracking()
                                                                                                     on TransactionHeader.TransactionSourceID equals TransactionSource.ID into TransactionSourcePR
                                                                                                         from TSPR in TransactionSourcePR.DefaultIfEmpty()
                                                                                                         join TransactionType in _dbContext.TransactionExceptionType.AsNoTracking()
                                                                                                     on TransactionHeader.TransactionTypeID equals TransactionType.ID into TransactionTypePR
                                                                                                         from TTPR in TransactionTypePR.DefaultIfEmpty()
                                                                                                         join Invoice in _dbContext.InvoiceCycle.AsNoTracking()
                                                                                                     on TransactionHeader.InvoiceHeaderID equals Invoice.ID into InvoicePR
                                                                                                         from IPR in InvoicePR.DefaultIfEmpty()

                                                                                                         select new wbm_common.DataObjects.TransactionHeader.dbRowExtended
                                                                                                         {
                                                                                                             ID = TransactionHeader.ID,
                                                                                                             CompanyID = TransactionHeader.CompanyID,
                                                                                                             OwnerID = TransactionHeader.OwnerID,
                                                                                                             SiteID = TransactionHeader.SiteID,
                                                                                                             TransactionSourceID = TransactionHeader.TransactionSourceID,
                                                                                                             TransactionDateTime = TransactionHeader.TransactionDateTime,
                                                                                                             TransactionTypeID = TransactionHeader.TransactionTypeID,
                                                                                                             TransactionReference = TransactionHeader.TransactionReference,
                                                                                                             Description = TransactionHeader.Description,
                                                                                                             InvoiceHeaderID = TransactionHeader.InvoiceHeaderID,
                                                                                                             InvoiceDateTime = TransactionHeader.InvoiceDateTime,
                                                                                                             InvoiceExFuel = TransactionHeader.InvoiceExFuel,
                                                                                                             InvoiceFuel = TransactionHeader.InvoiceFuel,
                                                                                                             InvoiceGST = TransactionHeader.InvoiceGST,
                                                                                                             InvoiceExGST = TransactionHeader.InvoiceExGST,
                                                                                                             ExceptionFlag = TransactionHeader.ExceptionFlag,
                                                                                                             CanInvoice = TransactionHeader.CanInvoice,
                                                                                                             DeletedFlag = TransactionHeader.DeletedFlag,
                                                                                                             CreatedDateTime = TransactionHeader.CreatedDateTime,
                                                                                                             CreatedMethod = TransactionHeader.CreatedMethod,
                                                                                                             CreatedUserID = TransactionHeader.CreatedUserID,
                                                                                                             LastAmendedDateTime = TransactionHeader.LastAmendedDateTime,
                                                                                                             LastAmendedMethod = TransactionHeader.LastAmendedMethod,
                                                                                                             LastAmendedUserID = TransactionHeader.LastAmendedUserID,

                                                                                                             Site_PublicID = SPR.PublicID,
                                                                                                             Company_ShortName = CPR.ShortName,
                                                                                                             Owner_Name = OPR.Name,
                                                                                                             TransactionSource_PublicID = TSPR.PublicID,
                                                                                                             TransactionType_PublicID = TTPR.PublicID,
                                                                                                             InvoiceHeader_InvoiceNumber = IPR.Description
                                                                                                         }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>(TransactionHeaders);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>> TransactionHeader_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionHeader_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionHeader.dbRowExtended> TransactionHeaders = await (from TransactionHeader in _dbContext.TransactionHeader.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                             join site in _dbContext.Site.AsNoTracking()
                                                                                                                 on TransactionHeader.SiteID equals site.ID into sitePR
                                                                                                             from SPR in sitePR.DefaultIfEmpty()
                                                                                                             join company in _dbContext.Company.AsNoTracking()
                                                                                                         on TransactionHeader.CompanyID equals company.ID into companyPR
                                                                                                             from CPR in companyPR.DefaultIfEmpty()
                                                                                                             join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                         on TransactionHeader.OwnerID equals Owner.ID into OwnerPR
                                                                                                             from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                             join TransactionSource in _dbContext.TransactionSource.AsNoTracking()
                                                                                                         on TransactionHeader.TransactionSourceID equals TransactionSource.ID into TransactionSourcePR
                                                                                                             from TSPR in TransactionSourcePR.DefaultIfEmpty()
                                                                                                             join TransactionType in _dbContext.TransactionExceptionType.AsNoTracking()
                                                                                                         on TransactionHeader.TransactionTypeID equals TransactionType.ID into TransactionTypePR
                                                                                                             from TTPR in TransactionTypePR.DefaultIfEmpty()
                                                                                                             join Invoice in _dbContext.InvoiceCycle.AsNoTracking()
                                                                                                         on TransactionHeader.InvoiceHeaderID equals Invoice.ID into InvoicePR
                                                                                                             from IPR in InvoicePR.DefaultIfEmpty()

                                                                                                             select new wbm_common.DataObjects.TransactionHeader.dbRowExtended
                                                                                                             {
                                                                                                                 ID = TransactionHeader.ID,
                                                                                                                 CompanyID = TransactionHeader.CompanyID,
                                                                                                                 OwnerID = TransactionHeader.OwnerID,
                                                                                                                 SiteID = TransactionHeader.SiteID,
                                                                                                                 TransactionSourceID = TransactionHeader.TransactionSourceID,
                                                                                                                 TransactionDateTime = TransactionHeader.TransactionDateTime,
                                                                                                                 TransactionTypeID = TransactionHeader.TransactionTypeID,
                                                                                                                 TransactionReference = TransactionHeader.TransactionReference,
                                                                                                                 Description = TransactionHeader.Description,
                                                                                                                 InvoiceHeaderID = TransactionHeader.InvoiceHeaderID,
                                                                                                                 InvoiceDateTime = TransactionHeader.InvoiceDateTime,
                                                                                                                 InvoiceExFuel = TransactionHeader.InvoiceExFuel,
                                                                                                                 InvoiceFuel = TransactionHeader.InvoiceFuel,
                                                                                                                 InvoiceGST = TransactionHeader.InvoiceGST,
                                                                                                                 InvoiceExGST = TransactionHeader.InvoiceExGST,
                                                                                                                 ExceptionFlag = TransactionHeader.ExceptionFlag,
                                                                                                                 CanInvoice = TransactionHeader.CanInvoice,
                                                                                                                 DeletedFlag = TransactionHeader.DeletedFlag,
                                                                                                                 CreatedDateTime = TransactionHeader.CreatedDateTime,
                                                                                                                 CreatedMethod = TransactionHeader.CreatedMethod,
                                                                                                                 CreatedUserID = TransactionHeader.CreatedUserID,
                                                                                                                 LastAmendedDateTime = TransactionHeader.LastAmendedDateTime,
                                                                                                                 LastAmendedMethod = TransactionHeader.LastAmendedMethod,
                                                                                                                 LastAmendedUserID = TransactionHeader.LastAmendedUserID,

                                                                                                                 Site_PublicID = SPR.PublicID,
                                                                                                                 Company_ShortName = CPR.ShortName,
                                                                                                                 Owner_Name = OPR.Name,
                                                                                                                 TransactionSource_PublicID = TSPR.PublicID,
                                                                                                                 TransactionType_PublicID = TTPR.PublicID,
                                                                                                                 InvoiceHeader_InvoiceNumber = IPR.Description
                                                                                                             }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>(TransactionHeaders);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionHeader_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>> TransactionHeader_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionHeader.dbRowExtended> TransactionHeaders = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.TransactionHeader.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>(TransactionHeaders);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRow>>> TransactionHeader_ListByOwnerIDCanInvoiceCutDate(int OwnerID, DateTime CutDate)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionHeader.dbRow> ListTH = await _dbContext.TransactionHeader.AsNoTracking().Where(x => !x.DeletedFlag && x.OwnerID == OwnerID && x.CanInvoice && x.InvoiceHeaderID == 0 && x.TransactionDateTime <= CutDate).ToListAsync();

                // no rows is valid
                return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRow>>(ListTH);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_ListByOwnerIDCanInvoiceCutDate - " + OwnerID.ToString() + ", " + CutDate.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRow>>> TransactionHeader_ListByInvoiceHeaderID(int InvoiceHeaderID)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionHeader.dbRow> ListTH = await _dbContext.TransactionHeader.AsNoTracking().Where(x => !x.DeletedFlag && x.InvoiceHeaderID == InvoiceHeaderID).ToListAsync();

                // no rows is valid
                return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRow>>(ListTH);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_ListByInvoiceHeaderID - " + InvoiceHeaderID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRow>>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRow>>> TransactionHeader_ListAllUnInvoiced()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionHeader.dbRow> ListTH = await _dbContext.TransactionHeader.AsNoTracking().Where(x => !x.DeletedFlag && x.InvoiceHeaderID == 0).OrderBy(x => x.OwnerID).ThenBy(x => x.TransactionSourceID).ToListAsync();

                // no rows is valid
                return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRow>>(ListTH);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_ListAllUnInvoiced - : " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRow>>("Internal Exception occured");
            }
        }


        public async Task<Result<Boolean>> TransactionHeader_UpdateInvoiced(int InvoiceHeaderID, DateTime InvoiceDate, List<int> ListTHID)
        {
            if (ListTHID == null || ListTHID.Count == 0)
            {
                logger.Error("TransactionHeader_UpdateInvoiced was null or blank ");
                return Result.Failure<Boolean>("IDs list is null or empty");
            }

            try
            {
                await _dbContext.TransactionHeader.Where(x => ListTHID.Contains(x.ID)).ForEachAsync(x =>
                {
                    x.InvoiceHeaderID = InvoiceHeaderID;
                    x.InvoiceDateTime = InvoiceDate;
                });

                await _dbContext.SaveChangesAsync();

                return Result.Success<Boolean>(true);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_UpdateInvoiced - " + InvoiceHeaderID.ToString() + ", " + InvoiceDate.ToString() + ": " + ex);
                return Result.Exception<Boolean>("Internal Exception occured");
            }
        }

        public async Task<Result<Boolean>> TransactionHeader_RollBackInvoiced(int InvoiceHeaderID)
        {
            try
            {
                await _dbContext.TransactionHeader.Where(x => x.InvoiceHeaderID == InvoiceHeaderID).ForEachAsync(x =>
                {
                    x.InvoiceHeaderID = 0;
                    x.InvoiceDateTime = null;
                });

                await _dbContext.SaveChangesAsync();

                return Result.Success<Boolean>(true);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_RollBackInvoiced - " + InvoiceHeaderID.ToString() + ": " + ex);
                return Result.Exception<Boolean>("Internal Exception occured");
            }
        }


        #endregion

        #region TransactionSource
        public async Task<Result<int>> TransactionSource_Insert(wbm_common.DataObjects.TransactionSource.dbRow TransactionSource)
        {
            try
            {
                await _dbContext.TransactionSource.AddAsync(TransactionSource);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionSource.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_Insert - " + TransactionSource.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionSource_Update(wbm_common.DataObjects.TransactionSource.dbRow TransactionSource)
        {
            try
            {
                _dbContext.TransactionSource.Attach(TransactionSource);
                _dbContext.Entry(TransactionSource).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionSource.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_Update - " + TransactionSource.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.TransactionSource.dbRow>> TransactionSource_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionSource.dbRow TransactionSource = await _dbContext.TransactionSource.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionSource == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionSource.dbRow>("TransactionSource not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionSource.dbRow>(TransactionSource);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionSource.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionSource.dbRow>> TransactionSource_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.TransactionSource.dbRow TransactionSource = await _dbContext.TransactionSource.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (TransactionSource == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionSource.dbRow>("TransactionSource not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.TransactionSource.dbRow>(TransactionSource);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionSource.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionSource.dbRow>>> TransactionSource_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionSource.dbRow TransactionSource = await _dbContext.TransactionSource.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionSource == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(new List<wbm_common.DataObjects.TransactionSource.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(new List<wbm_common.DataObjects.TransactionSource.dbRow> { TransactionSource });
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionSource.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionSource.dbRow>> TransactionSource_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionSource.dbRow TransactionSource = await _dbContext.TransactionSource.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (TransactionSource == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionSource.dbRow>("TransactionSource not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionSource.dbRow>(TransactionSource);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionSource.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionSource.dbRow>>> TransactionSource_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionSource.dbRow> TransactionSources = await _dbContext.TransactionSource.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(TransactionSources);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionSource.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionSource.dbRow>>> TransactionSource_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionSource_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionSource.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionSource.dbRow> TransactionSources = await _dbContext.TransactionSource.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(TransactionSources);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionSource_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionSource.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionSource.dbRow>>> TransactionSource_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionSource.dbRow> TransactionSources = await _dbContext.TransactionSource.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(TransactionSources);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionSource.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region TransactionNotes
        public async Task<Result<int>> TransactionNotes_Insert(wbm_common.DataObjects.TransactionNotes.dbRow TransactionNotes)
        {
            try
            {
                await _dbContext.TransactionNotes.AddAsync(TransactionNotes);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionNotes.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionNotes_Insert - " + TransactionNotes.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionNotes_Update(wbm_common.DataObjects.TransactionNotes.dbRow TransactionNotes)
        {
            try
            {
                _dbContext.TransactionNotes.Attach(TransactionNotes);
                _dbContext.Entry(TransactionNotes).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionNotes.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionNotes_Update - " + TransactionNotes.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionNotes.dbRow>>> TransactionNotes_ListByListTransactionHeaderIDs(List<int> TransactionHeaderIDs)
        {
            if (TransactionHeaderIDs == null || TransactionHeaderIDs.Count == 0)
            {
                logger.Error("TransactionNotes_ListByListTransactionHeaderIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionNotes.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionNotes.dbRow> TransactionNotess = await _dbContext.TransactionNotes.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && TransactionHeaderIDs.Contains(x.TransactionHeaderID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionNotes.dbRow>>(TransactionNotess);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionNotes_ListByListTransactionHeaderIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionNotes.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionNotes.dbRow>>> TransactionNotes_ListIncludedDeletedByListTransactionHeaderIDs(List<int> TransactionHeaderIDs)
        {
            if (TransactionHeaderIDs == null || TransactionHeaderIDs.Count == 0)
            {
                logger.Error("TransactionNotes_ListIncludedDeletedByListTransactionHeaderIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionNotes.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionNotes.dbRow> TransactionNotess = await _dbContext.TransactionNotes.AsNoTracking()
                                                                        .Where(x => TransactionHeaderIDs.Contains(x.TransactionHeaderID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionNotes.dbRow>>(TransactionNotess);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionNotes_ListIncludedDeletedByListTransactionHeaderIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionNotes.dbRow>>("Internal Exception occured");
                }
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionNotes.dbRow>> TransactionNotes_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionNotes.dbRow TransactionNotesDb = await _dbContext.TransactionNotes.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).FirstOrDefaultAsync();

                if (TransactionNotesDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionNotes.dbRow>("TransactionNotes not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionNotes.dbRow>(TransactionNotesDb);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionNotes_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionNotes.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionNotes.dbRow>>> TransactionNotes_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionNotes.dbRow> TransactionNotesDb = await _dbContext.TransactionNotes.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).ToListAsync();

                if (TransactionNotesDb == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionNotes.dbRow>>(new List<wbm_common.DataObjects.TransactionNotes.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionNotes.dbRow>>(TransactionNotesDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionNotes_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionNotes.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionNotes.dbRow>> TransactionNotes_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionNotes.dbRow TransactionNotesDb = await _dbContext.TransactionNotes.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (TransactionNotesDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionNotes.dbRow>("TransactionNotes not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionNotes.dbRow>(TransactionNotesDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionNotes_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionNotes.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionNotes.dbRow>>> TransactionNotes_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionNotes.dbRow> TransactionNotess = await _dbContext.TransactionNotes.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionNotes.dbRow>>(TransactionNotess);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionNotes_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionNotes.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionNotes.dbRow>>> TransactionNotes_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionNotes_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionNotes.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionNotes.dbRow> TransactionNotess = await _dbContext.TransactionNotes.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionNotes.dbRow>>(TransactionNotess);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionNotes_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionNotes.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionNotes.dbRow>>> TransactionNotes_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionNotes.dbRow> TransactionNotess = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.TransactionNotes.dbRow>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionNotes.dbRow>>(TransactionNotess);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionNotes_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionNotes.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region TransactionType
        public async Task<Result<int>> TransactionType_Insert(wbm_common.DataObjects.TransactionType.dbRow TransactionType)
        {
            try
            {
                await _dbContext.TransactionType.AddAsync(TransactionType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionType_Insert - " + TransactionType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionType_Update(wbm_common.DataObjects.TransactionType.dbRow TransactionType)
        {
            try
            {
                _dbContext.TransactionType.Attach(TransactionType);
                _dbContext.Entry(TransactionType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionType_Update - " + TransactionType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.TransactionType.dbRow>>> TransactionType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionType.dbRow TransactionTypeDb = await _dbContext.TransactionType.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).FirstOrDefaultAsync();

                if (TransactionTypeDb == null)
                    return Result.Failure< List<wbm_common.DataObjects.TransactionType.dbRow>>("TransactionType not found : " + ID.ToString());
                else
                    return Result.Success< List<wbm_common.DataObjects.TransactionType.dbRow>>(new List<TransactionType.dbRow> { TransactionTypeDb });

            }
            catch (Exception ex)
            {
                logger.Error("TransactionType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception< List<wbm_common.DataObjects.TransactionType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.TransactionType.dbRow>> TransactionType_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.TransactionType.dbRow TransactionType = await _dbContext.TransactionType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (TransactionType == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionType.dbRow>("TransactionType not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.TransactionType.dbRow>(TransactionType);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionType_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionType.dbRow>("Internal Exception occured");
            }
        }        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return

        public async Task<Result<List<wbm_common.DataObjects.TransactionType.dbRow>>> TransactionType_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionType.dbRow> TransactionTypeDb = await _dbContext.TransactionType.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).ToListAsync();

                if (TransactionTypeDb == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionType.dbRow>>(new List<wbm_common.DataObjects.TransactionType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionType.dbRow>>(TransactionTypeDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionType_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionType.dbRow>> TransactionType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionType.dbRow TransactionTypeDb = await _dbContext.TransactionType.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (TransactionTypeDb == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionType.dbRow>("TransactionType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionType.dbRow>(TransactionTypeDb);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionType.dbRow>>> TransactionType_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionType.dbRow> TransactionTypes = await _dbContext.TransactionType.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionType.dbRow>>(TransactionTypes);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionType.dbRow>>> TransactionType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionType.dbRow> TransactionTypes = await _dbContext.TransactionType.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionType.dbRow>>(TransactionTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionType.dbRow>>> TransactionType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionType.dbRow> TransactionTypes = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.TransactionType.dbRow>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionType.dbRow>>(TransactionTypes);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionType.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

    }
}
