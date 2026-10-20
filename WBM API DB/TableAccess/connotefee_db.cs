using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> ConnoteFee_Insert(wbm_common.DataObjects.ConnoteFee.dbRow ConnoteFee)
        {
            try
            {
                await _dbContext.ConnoteFee.AddAsync(ConnoteFee);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ConnoteFee.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_Insert - " + ConnoteFee.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ConnoteFee_Update(wbm_common.DataObjects.ConnoteFee.dbRow ConnoteFee)
        {
            try
            {
                _dbContext.ConnoteFee.Attach(ConnoteFee);
                _dbContext.Entry(ConnoteFee).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ConnoteFee.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_Update - " + ConnoteFee.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>> ConnoteFee_ListBySingleOwnerID(int OwnerID)
        {
            try
            {
                List<wbm_common.DataObjects.ConnoteFee.dbRowExtended> ConnoteFeeDb = await (from ConnoteFee in _dbContext.ConnoteFee.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.OwnerID == OwnerID || x.OwnerID == 0))
                                                                                            join site in _dbContext.Site.AsNoTracking()
                                                                                                on ConnoteFee.SiteID equals site.ID into sitePR
                                                                                            from SPR in sitePR.DefaultIfEmpty()
                                                                                            join company in _dbContext.Company.AsNoTracking()
                                                                                                on ConnoteFee.CompanyID equals company.ID into companyPR
                                                                                            from CPR in companyPR.DefaultIfEmpty()
                                                                                            join Owner in _dbContext.Owner.AsNoTracking()
                                                                                on ConnoteFee.OwnerID equals Owner.ID into OwnerPR
                                                                                            from OPR in OwnerPR.DefaultIfEmpty()
                                                                                            join Customer in _dbContext.CustomerAddress.AsNoTracking()
                                                                                on ConnoteFee.CustomerAddressID equals Customer.ID into CustomerPR
                                                                                            from CustPR in CustomerPR.DefaultIfEmpty()
                                                                                            join Carrier in _dbContext.Carrier.AsNoTracking()
                                                                                on ConnoteFee.CarrierID equals Carrier.ID into CarrierPR
                                                                                            from CarPR in CarrierPR.DefaultIfEmpty()
                                                                                            join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                on ConnoteFee.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                            from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                            join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                on ConnoteFee.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                            from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                            select new wbm_common.DataObjects.ConnoteFee.dbRowExtended
                                                                                            {
                                                                                                ID = ConnoteFee.ID,
                                                                                                CompanyID = ConnoteFee.CompanyID,
                                                                                                OwnerID = ConnoteFee.OwnerID,
                                                                                                SiteID = ConnoteFee.SiteID,
                                                                                                CustomerAddressID = ConnoteFee.CustomerAddressID,
                                                                                                CarrierID = ConnoteFee.CarrierID,
                                                                                                RateCollectionRateChargeCode = ConnoteFee.RateCollectionRateChargeCode,
                                                                                                DeletedFlag = ConnoteFee.DeletedFlag,
                                                                                                CreatedDateTime = ConnoteFee.CreatedDateTime,
                                                                                                CreatedMethod = ConnoteFee.CreatedMethod,
                                                                                                CreatedUserID = ConnoteFee.CreatedUserID,
                                                                                                LastAmendedDateTime = ConnoteFee.LastAmendedDateTime,
                                                                                                LastAmendedMethod = ConnoteFee.LastAmendedMethod,
                                                                                                LastAmendedUserID = ConnoteFee.LastAmendedUserID,

                                                                                                Site_PublicID = SPR.PublicID,
                                                                                                Company_ShortName = CPR.ShortName,
                                                                                                Owner_Name = OPR.Name,
                                                                                                Carrier_PublicID = CarPR.PublicID,
                                                                                                Customer_PublicID = CustPR.Name,
                                                                                                RateCode_Code = RCPR.Description,
                                                                                                Created_UserPublicID = SU.PublicID
                                                                                            }).ToListAsync();

                if (ConnoteFeeDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>(new List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>(ConnoteFeeDb);
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_ListBySingleOwnerID - " + OwnerID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>> ConnoteFee_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ConnoteFee.dbRowExtended ConnoteFeeDb = await (from ConnoteFee in _dbContext.ConnoteFee.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).Take(1)
                                                                                      join site in _dbContext.Site.AsNoTracking()
                                                                                          on ConnoteFee.SiteID equals site.ID into sitePR
                                                                                      from SPR in sitePR.DefaultIfEmpty()
                                                                                      join company in _dbContext.Company.AsNoTracking()
                                                                                          on ConnoteFee.CompanyID equals company.ID into companyPR
                                                                                      from CPR in companyPR.DefaultIfEmpty()
                                                                                      join Owner in _dbContext.Owner.AsNoTracking()
                                                                          on ConnoteFee.OwnerID equals Owner.ID into OwnerPR
                                                                                      from OPR in OwnerPR.DefaultIfEmpty()
                                                                                      join Customer in _dbContext.CustomerAddress.AsNoTracking()
                                                                          on ConnoteFee.CustomerAddressID equals Customer.ID into CustomerPR
                                                                                      from CustPR in CustomerPR.DefaultIfEmpty()
                                                                                      join Carrier in _dbContext.Carrier.AsNoTracking()
                                                                          on ConnoteFee.CarrierID equals Carrier.ID into CarrierPR
                                                                                      from CarPR in CarrierPR.DefaultIfEmpty()
                                                                                      join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                          on ConnoteFee.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                      from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                      join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                          on ConnoteFee.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                      from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                      select new wbm_common.DataObjects.ConnoteFee.dbRowExtended
                                                                                      {
                                                                                          ID = ConnoteFee.ID,
                                                                                          CompanyID = ConnoteFee.CompanyID,
                                                                                          OwnerID = ConnoteFee.OwnerID,
                                                                                          SiteID = ConnoteFee.SiteID,
                                                                                          CustomerAddressID = ConnoteFee.CustomerAddressID,
                                                                                          CarrierID = ConnoteFee.CarrierID,
                                                                                          RateCollectionRateChargeCode = ConnoteFee.RateCollectionRateChargeCode,
                                                                                          DeletedFlag = ConnoteFee.DeletedFlag,
                                                                                          CreatedDateTime = ConnoteFee.CreatedDateTime,
                                                                                          CreatedMethod = ConnoteFee.CreatedMethod,
                                                                                          CreatedUserID = ConnoteFee.CreatedUserID,
                                                                                          LastAmendedDateTime = ConnoteFee.LastAmendedDateTime,
                                                                                          LastAmendedMethod = ConnoteFee.LastAmendedMethod,
                                                                                          LastAmendedUserID = ConnoteFee.LastAmendedUserID,

                                                                                          Site_PublicID = SPR.PublicID,
                                                                                          Company_ShortName = CPR.ShortName,
                                                                                          Owner_Name = OPR.Name,
                                                                                          Carrier_PublicID = CarPR.PublicID,
                                                                                          Customer_PublicID = CustPR.Name,
                                                                                          RateCode_Code = RCPR.Description,
                                                                                          Created_UserPublicID = SU.PublicID
                                                                                      }).FirstOrDefaultAsync();

                if (ConnoteFeeDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>(new List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>(new List<wbm_common.DataObjects.ConnoteFee.dbRowExtended> { ConnoteFeeDb });
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ConnoteFee.dbRow>> ConnoteFee_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ConnoteFee.dbRow ConnoteFeeDb = await _dbContext.ConnoteFee.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (ConnoteFeeDb == null)
                    return Result.Failure<wbm_common.DataObjects.ConnoteFee.dbRow>("ConnoteFee not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ConnoteFee.dbRow>(ConnoteFeeDb);
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ConnoteFee.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>> ConnoteFee_List()
        {
            try
            {
                List<wbm_common.DataObjects.ConnoteFee.dbRowExtended> ConnoteFees = await (from ConnoteFee in _dbContext.ConnoteFee.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                           join site in _dbContext.Site.AsNoTracking()
                                                                                               on ConnoteFee.SiteID equals site.ID into sitePR
                                                                                           from SPR in sitePR.DefaultIfEmpty()
                                                                                           join company in _dbContext.Company.AsNoTracking()
                                                                                               on ConnoteFee.CompanyID equals company.ID into companyPR
                                                                                           from CPR in companyPR.DefaultIfEmpty()
                                                                                           join Owner in _dbContext.Owner.AsNoTracking()
                                                                               on ConnoteFee.OwnerID equals Owner.ID into OwnerPR
                                                                                           from OPR in OwnerPR.DefaultIfEmpty()
                                                                                           join Customer in _dbContext.CustomerAddress.AsNoTracking()
                                                                               on ConnoteFee.CustomerAddressID equals Customer.ID into CustomerPR
                                                                                           from CustPR in CustomerPR.DefaultIfEmpty()
                                                                                           join Carrier in _dbContext.Carrier.AsNoTracking()
                                                                               on ConnoteFee.CarrierID equals Carrier.ID into CarrierPR
                                                                                           from CarPR in CarrierPR.DefaultIfEmpty()
                                                                                           join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                               on ConnoteFee.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                           from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                           join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                               on ConnoteFee.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                           from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                           select new wbm_common.DataObjects.ConnoteFee.dbRowExtended
                                                                                           {
                                                                                               ID = ConnoteFee.ID,
                                                                                               CompanyID = ConnoteFee.CompanyID,
                                                                                               OwnerID = ConnoteFee.OwnerID,
                                                                                               SiteID = ConnoteFee.SiteID,
                                                                                               CustomerAddressID = ConnoteFee.CustomerAddressID,
                                                                                               CarrierID = ConnoteFee.CarrierID,
                                                                                               RateCollectionRateChargeCode = ConnoteFee.RateCollectionRateChargeCode,
                                                                                               DeletedFlag = ConnoteFee.DeletedFlag,
                                                                                               CreatedDateTime = ConnoteFee.CreatedDateTime,
                                                                                               CreatedMethod = ConnoteFee.CreatedMethod,
                                                                                               CreatedUserID = ConnoteFee.CreatedUserID,
                                                                                               LastAmendedDateTime = ConnoteFee.LastAmendedDateTime,
                                                                                               LastAmendedMethod = ConnoteFee.LastAmendedMethod,
                                                                                               LastAmendedUserID = ConnoteFee.LastAmendedUserID,

                                                                                               Site_PublicID = SPR.PublicID,
                                                                                               Company_ShortName = CPR.ShortName,
                                                                                               Owner_Name = OPR.Name,
                                                                                               Carrier_PublicID = CarPR.PublicID,
                                                                                               Customer_PublicID = CustPR.Name,
                                                                                               RateCode_Code = RCPR.Description,
                                                                                               Created_UserPublicID = SU.PublicID
                                                                                           }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>(ConnoteFees);
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>> ConnoteFee_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ConnoteFee_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ConnoteFee.dbRowExtended> ConnoteFees = await (from ConnoteFee in _dbContext.ConnoteFee.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                               join site in _dbContext.Site.AsNoTracking()
                                                                                                   on ConnoteFee.SiteID equals site.ID into sitePR
                                                                                               from SPR in sitePR.DefaultIfEmpty()
                                                                                               join company in _dbContext.Company.AsNoTracking()
                                                                                                   on ConnoteFee.CompanyID equals company.ID into companyPR
                                                                                               from CPR in companyPR.DefaultIfEmpty()
                                                                                               join Owner in _dbContext.Owner.AsNoTracking()
                                                                                   on ConnoteFee.OwnerID equals Owner.ID into OwnerPR
                                                                                               from OPR in OwnerPR.DefaultIfEmpty()
                                                                                               join Customer in _dbContext.CustomerAddress.AsNoTracking()
                                                                                   on ConnoteFee.CustomerAddressID equals Customer.ID into CustomerPR
                                                                                               from CustPR in CustomerPR.DefaultIfEmpty()
                                                                                               join Carrier in _dbContext.Carrier.AsNoTracking()
                                                                                   on ConnoteFee.CarrierID equals Carrier.ID into CarrierPR
                                                                                               from CarPR in CarrierPR.DefaultIfEmpty()
                                                                                               join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                   on ConnoteFee.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                               from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                               join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                   on ConnoteFee.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                               from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                               select new wbm_common.DataObjects.ConnoteFee.dbRowExtended
                                                                                               {
                                                                                                   ID = ConnoteFee.ID,
                                                                                                   CompanyID = ConnoteFee.CompanyID,
                                                                                                   OwnerID = ConnoteFee.OwnerID,
                                                                                                   SiteID = ConnoteFee.SiteID,
                                                                                                   CustomerAddressID = ConnoteFee.CustomerAddressID,
                                                                                                   CarrierID = ConnoteFee.CarrierID,
                                                                                                   RateCollectionRateChargeCode = ConnoteFee.RateCollectionRateChargeCode,
                                                                                                   DeletedFlag = ConnoteFee.DeletedFlag,
                                                                                                   CreatedDateTime = ConnoteFee.CreatedDateTime,
                                                                                                   CreatedMethod = ConnoteFee.CreatedMethod,
                                                                                                   CreatedUserID = ConnoteFee.CreatedUserID,
                                                                                                   LastAmendedDateTime = ConnoteFee.LastAmendedDateTime,
                                                                                                   LastAmendedMethod = ConnoteFee.LastAmendedMethod,
                                                                                                   LastAmendedUserID = ConnoteFee.LastAmendedUserID,

                                                                                                   Site_PublicID = SPR.PublicID,
                                                                                                   Company_ShortName = CPR.ShortName,
                                                                                                   Owner_Name = OPR.Name,
                                                                                                   Carrier_PublicID = CarPR.PublicID,
                                                                                                   Customer_PublicID = CustPR.Name,
                                                                                                   RateCode_Code = RCPR.Description,
                                                                                                   Created_UserPublicID = SU.PublicID
                                                                                               }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>(ConnoteFees);
                }
                catch (Exception ex)
                {
                    logger.Error("ConnoteFee_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>> ConnoteFee_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ConnoteFee.dbRowExtended> ConnoteFees = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.ConnoteFee.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>(ConnoteFees);
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRowExtended>>("Internal Exception occured");
            }
        }
    }
}
