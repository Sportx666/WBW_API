using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System.Runtime.ConstrainedExecution;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region CustomerAddress
        public async Task<Result<int>> CustomerAddress_Insert(wbm_common.DataObjects.CustomerAddress.dbRow CustomerAddress)
        {
            try
            {
                await _dbContext.CustomerAddress.AddAsync(CustomerAddress);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(CustomerAddress.ID);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_Insert - " + CustomerAddress.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> CustomerAddress_Update(wbm_common.DataObjects.CustomerAddress.dbRow CustomerAddress)
        {
            try
            {
                _dbContext.CustomerAddress.Attach(CustomerAddress);
                _dbContext.Entry(CustomerAddress).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(CustomerAddress.ID);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_Update - " + CustomerAddress.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>> CustomerAddress_ListByPaperlessKeyID(string PaperlessKeyID)
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddress.dbRowExtended> CustomerAddressDb = await (from CustomerAddress in _dbContext.CustomerAddress.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.Paperless_KeyID == PaperlessKeyID)
                                                                                              join site in _dbContext.Site.AsNoTracking()
                                                                                                  on CustomerAddress.SiteID equals site.ID into sitePR
                                                                                                      from SPR in sitePR.DefaultIfEmpty()
                                                                                                      join company in _dbContext.Company.AsNoTracking()
                                                                                                  on CustomerAddress.CompanyID equals company.ID into companyPR
                                                                                                      from CPR in companyPR.DefaultIfEmpty()
                                                                                                      join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                  on CustomerAddress.OwnerID equals Owner.ID into OwnerPR
                                                                                                      from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                      join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on CustomerAddress.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                      from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                      select new CustomerAddress.dbRowExtended
                                                                                              {
                                                                                                  ID = CustomerAddress.ID,
                                                                                                  CompanyID = CustomerAddress.CompanyID,
                                                                                                  OwnerID = CustomerAddress.OwnerID,
                                                                                                  SiteID = CustomerAddress.SiteID,
                                                                                                  CustomerID = CustomerAddress.CustomerID,
                                                                                                  Name = CustomerAddress.Name,
                                                                                                  AddressLine1 = CustomerAddress.AddressLine1,
                                                                                                  AddressLine2 = CustomerAddress.AddressLine2,
                                                                                                  AddressLine3 = CustomerAddress.AddressLine3,
                                                                                                  AddressLine4 = CustomerAddress.AddressLine4,
                                                                                                  Suburb = CustomerAddress.Suburb,
                                                                                                  State = CustomerAddress.State,
                                                                                                  PostCode = CustomerAddress.PostCode,
                                                                                                  PalletHireDelayDays = CustomerAddress.PalletHireDelayDays,
                                                                                                  InvoiceEnclosed = CustomerAddress.InvoiceEnclosed,
                                                                                                  Paperless_KeyID = CustomerAddress.Paperless_KeyID,
                                                                                                  DeletedFlag = CustomerAddress.DeletedFlag,
                                                                                                  CreatedDateTime = CustomerAddress.CreatedDateTime,
                                                                                                  CreatedMethod = CustomerAddress.CreatedMethod,
                                                                                                  CreatedUserID = CustomerAddress.CreatedUserID,
                                                                                                  LastAmendedDateTime = CustomerAddress.LastAmendedDateTime,
                                                                                                  LastAmendedMethod = CustomerAddress.LastAmendedMethod,
                                                                                                  LastAmendedUserID = CustomerAddress.LastAmendedUserID,
                                                                                                  Connote_RateCollectionRateChargeCode = CustomerAddress.Connote_RateCollectionRateChargeCode,

                                                                                                  Site_PublicID = SPR.PublicID,
                                                                                                  Company_ShortName = CPR.ShortName,
                                                                                                  Owner_Name = OPR.Name,
                                                                                                  Created_UserPublicID = SU.PublicID
                                                                                              }).ToListAsync();

                if (CustomerAddressDb == null)
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>(new List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>(CustomerAddressDb);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_ListByPaperlessKeyID - " + PaperlessKeyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>> CustomerAddress_SearchByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddress.dbRowExtended> CustomerAddressDb = await (from CustomerAddress in _dbContext.CustomerAddress.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                      join site in _dbContext.Site.AsNoTracking()
                                                                                                          on CustomerAddress.SiteID equals site.ID into sitePR
                                                                                                      from SPR in sitePR.DefaultIfEmpty()
                                                                                                      join company in _dbContext.Company.AsNoTracking()
                                                                                                  on CustomerAddress.CompanyID equals company.ID into companyPR
                                                                                                      from CPR in companyPR.DefaultIfEmpty()
                                                                                                      join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                  on CustomerAddress.OwnerID equals Owner.ID into OwnerPR
                                                                                                      from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                      join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on CustomerAddress.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                      from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                      select new CustomerAddress.dbRowExtended
                                                                                                      {
                                                                                                          ID = CustomerAddress.ID,
                                                                                                          CompanyID = CustomerAddress.CompanyID,
                                                                                                          OwnerID = CustomerAddress.OwnerID,
                                                                                                          SiteID = CustomerAddress.SiteID,
                                                                                                          CustomerID = CustomerAddress.CustomerID,
                                                                                                          Name = CustomerAddress.Name,
                                                                                                          AddressLine1 = CustomerAddress.AddressLine1,
                                                                                                          AddressLine2 = CustomerAddress.AddressLine2,
                                                                                                          AddressLine3 = CustomerAddress.AddressLine3,
                                                                                                          AddressLine4 = CustomerAddress.AddressLine4,
                                                                                                          Suburb = CustomerAddress.Suburb,
                                                                                                          State = CustomerAddress.State,
                                                                                                          PostCode = CustomerAddress.PostCode,
                                                                                                          PalletHireDelayDays = CustomerAddress.PalletHireDelayDays,
                                                                                                          InvoiceEnclosed = CustomerAddress.InvoiceEnclosed,
                                                                                                          Paperless_KeyID = CustomerAddress.Paperless_KeyID,
                                                                                                          DeletedFlag = CustomerAddress.DeletedFlag,
                                                                                                          CreatedDateTime = CustomerAddress.CreatedDateTime,
                                                                                                          CreatedMethod = CustomerAddress.CreatedMethod,
                                                                                                          CreatedUserID = CustomerAddress.CreatedUserID,
                                                                                                          LastAmendedDateTime = CustomerAddress.LastAmendedDateTime,
                                                                                                          LastAmendedMethod = CustomerAddress.LastAmendedMethod,
                                                                                                          LastAmendedUserID = CustomerAddress.LastAmendedUserID,
                                                                                                          Connote_RateCollectionRateChargeCode = CustomerAddress.Connote_RateCollectionRateChargeCode,

                                                                                                          Site_PublicID = SPR.PublicID,
                                                                                                          Company_ShortName = CPR.ShortName,
                                                                                                          Owner_Name = OPR.Name,
                                                                                                          Created_UserPublicID = SU.PublicID
                                                                                                      }).ToListAsync();

                if (CustomerAddressDb == null)
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>(new List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>(CustomerAddressDb);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>> CustomerAddress_ListByOwnerCustomerID(int OwnerID, string CustomerID)
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddress.dbRowExtended> CustomerAddressDb = await (from CustomerAddress in _dbContext.CustomerAddress.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.OwnerID == OwnerID || x.OwnerID == 0) && x.CustomerID == CustomerID)
                                                                                                      join site in _dbContext.Site.AsNoTracking()
                                                                                                          on CustomerAddress.SiteID equals site.ID into sitePR
                                                                                                      from SPR in sitePR.DefaultIfEmpty()
                                                                                                      join company in _dbContext.Company.AsNoTracking()
                                                                                                  on CustomerAddress.CompanyID equals company.ID into companyPR
                                                                                                      from CPR in companyPR.DefaultIfEmpty()
                                                                                                      join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                  on CustomerAddress.OwnerID equals Owner.ID into OwnerPR
                                                                                                      from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                      join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on CustomerAddress.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                      from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                      select new CustomerAddress.dbRowExtended
                                                                                                      {
                                                                                                          ID = CustomerAddress.ID,
                                                                                                          CompanyID = CustomerAddress.CompanyID,
                                                                                                          OwnerID = CustomerAddress.OwnerID,
                                                                                                          SiteID = CustomerAddress.SiteID,
                                                                                                          CustomerID = CustomerAddress.CustomerID,
                                                                                                          Name = CustomerAddress.Name,
                                                                                                          AddressLine1 = CustomerAddress.AddressLine1,
                                                                                                          AddressLine2 = CustomerAddress.AddressLine2,
                                                                                                          AddressLine3 = CustomerAddress.AddressLine3,
                                                                                                          AddressLine4 = CustomerAddress.AddressLine4,
                                                                                                          Suburb = CustomerAddress.Suburb,
                                                                                                          State = CustomerAddress.State,
                                                                                                          PostCode = CustomerAddress.PostCode,
                                                                                                          PalletHireDelayDays = CustomerAddress.PalletHireDelayDays,
                                                                                                          InvoiceEnclosed = CustomerAddress.InvoiceEnclosed,
                                                                                                          Paperless_KeyID = CustomerAddress.Paperless_KeyID,
                                                                                                          DeletedFlag = CustomerAddress.DeletedFlag,
                                                                                                          CreatedDateTime = CustomerAddress.CreatedDateTime,
                                                                                                          CreatedMethod = CustomerAddress.CreatedMethod,
                                                                                                          CreatedUserID = CustomerAddress.CreatedUserID,
                                                                                                          LastAmendedDateTime = CustomerAddress.LastAmendedDateTime,
                                                                                                          LastAmendedMethod = CustomerAddress.LastAmendedMethod,
                                                                                                          LastAmendedUserID = CustomerAddress.LastAmendedUserID,
                                                                                                          Connote_RateCollectionRateChargeCode = CustomerAddress.Connote_RateCollectionRateChargeCode,

                                                                                                          Site_PublicID = SPR.PublicID,
                                                                                                          Company_ShortName = CPR.ShortName,
                                                                                                          Owner_Name = OPR.Name,
                                                                                                          Created_UserPublicID = SU.PublicID
                                                                                                      }).ToListAsync();

                if (CustomerAddressDb == null)
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>(new List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>(CustomerAddressDb);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_ListByOwnerCustomerID - " + OwnerID.ToString() + ", " + CustomerID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.CustomerAddress.dbRow>> CustomerAddress_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddress.dbRow CustomerAddressDb = await _dbContext.CustomerAddress.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (CustomerAddressDb == null)
                    return Result.Failure<wbm_common.DataObjects.CustomerAddress.dbRow>("CustomerAddress not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.CustomerAddress.dbRow>(CustomerAddressDb);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.CustomerAddress.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>> CustomerAddress_List()
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddress.dbRowExtended> CustomerAddresssDb = await (from CustomerAddress in _dbContext.CustomerAddress.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                       join site in _dbContext.Site.AsNoTracking()
                                                                                                           on CustomerAddress.SiteID equals site.ID into sitePR
                                                                                                       from SPR in sitePR.DefaultIfEmpty()
                                                                                                       join company in _dbContext.Company.AsNoTracking()
                                                                                                   on CustomerAddress.CompanyID equals company.ID into companyPR
                                                                                                       from CPR in companyPR.DefaultIfEmpty()
                                                                                                       join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                   on CustomerAddress.OwnerID equals Owner.ID into OwnerPR
                                                                                                       from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                       join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                   on CustomerAddress.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                       from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                       select new CustomerAddress.dbRowExtended
                                                                                                       {
                                                                                                           ID = CustomerAddress.ID,
                                                                                                           CompanyID = CustomerAddress.CompanyID,
                                                                                                           OwnerID = CustomerAddress.OwnerID,
                                                                                                           SiteID = CustomerAddress.SiteID,
                                                                                                           CustomerID = CustomerAddress.CustomerID,
                                                                                                           Name = CustomerAddress.Name,
                                                                                                           AddressLine1 = CustomerAddress.AddressLine1,
                                                                                                           AddressLine2 = CustomerAddress.AddressLine2,
                                                                                                           AddressLine3 = CustomerAddress.AddressLine3,
                                                                                                           AddressLine4 = CustomerAddress.AddressLine4,
                                                                                                           Suburb = CustomerAddress.Suburb,
                                                                                                           State = CustomerAddress.State,
                                                                                                           PostCode = CustomerAddress.PostCode,
                                                                                                           PalletHireDelayDays = CustomerAddress.PalletHireDelayDays,
                                                                                                           InvoiceEnclosed = CustomerAddress.InvoiceEnclosed,
                                                                                                           Paperless_KeyID = CustomerAddress.Paperless_KeyID,
                                                                                                           DeletedFlag = CustomerAddress.DeletedFlag,
                                                                                                           CreatedDateTime = CustomerAddress.CreatedDateTime,
                                                                                                           CreatedMethod = CustomerAddress.CreatedMethod,
                                                                                                           CreatedUserID = CustomerAddress.CreatedUserID,
                                                                                                           LastAmendedDateTime = CustomerAddress.LastAmendedDateTime,
                                                                                                           LastAmendedMethod = CustomerAddress.LastAmendedMethod,
                                                                                                           LastAmendedUserID = CustomerAddress.LastAmendedUserID,
                                                                                                           Connote_RateCollectionRateChargeCode = CustomerAddress.Connote_RateCollectionRateChargeCode,

                                                                                                           Site_PublicID = SPR.PublicID,
                                                                                                           Company_ShortName = CPR.ShortName,
                                                                                                           Owner_Name = OPR.Name,
                                                                                                           Created_UserPublicID = SU.PublicID
                                                                                                       }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>(CustomerAddresssDb);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>> CustomerAddress_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddress.dbRowExtended> CustomerAddresss = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.CustomerAddress.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>(CustomerAddresss);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddress.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion

        #region CustomerAddressDefault
        public async Task<Result<int>> CustomerAddressDefault_Insert(wbm_common.DataObjects.CustomerAddressDefault.dbRow CustomerAddressDefault)
        {
            try
            {
                await _dbContext.CustomerAddressDefault.AddAsync(CustomerAddressDefault);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(CustomerAddressDefault.ID);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_Insert - " + CustomerAddressDefault.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> CustomerAddressDefault_Update(wbm_common.DataObjects.CustomerAddressDefault.dbRow CustomerAddressDefault)
        {
            try
            {
                _dbContext.CustomerAddressDefault.Attach(CustomerAddressDefault);
                _dbContext.Entry(CustomerAddressDefault).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(CustomerAddressDefault.ID);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_Update - " + CustomerAddressDefault.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.CustomerAddressDefault.dbRow>> CustomerAddressDefault_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended CustomerAddressDefaultdb = await (from CustomerAddressDefault in _dbContext.CustomerAddressDefault.AsNoTracking()
                                        .Where(x => !x.DeletedFlag && x.PublicID == PublicID)
                                                                                                              join site in _dbContext.Site.AsNoTracking()
                                                                                                              on CustomerAddressDefault.SiteID equals site.ID into sitePR
                                                                                                              from SPR in sitePR.DefaultIfEmpty()
                                                                                                              join company in _dbContext.Company.AsNoTracking()
                                                                                                              on CustomerAddressDefault.CompanyID equals company.ID into companyPR
                                                                                                              from CPR in companyPR.DefaultIfEmpty()
                                                                                                              join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                              on CustomerAddressDefault.OwnerID equals Owner.ID into OwnerPR
                                                                                                              from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                              join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                              on CustomerAddressDefault.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                              from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                              select new CustomerAddressDefault.dbRowExtended
                                                                                                              {
                                                                                                                  ID = CustomerAddressDefault.ID,
                                                                                                                  CompanyID = CustomerAddressDefault.CompanyID,
                                                                                                                  OwnerID = CustomerAddressDefault.OwnerID,
                                                                                                                  SiteID = CustomerAddressDefault.SiteID,
                                                                                                                  Description = CustomerAddressDefault.Description,
                                                                                                                  PublicID = CustomerAddressDefault.PublicID,
                                                                                                                  PalletHireDelayDays = CustomerAddressDefault.PalletHireDelayDays,
                                                                                                                  InvoiceEnclosed = CustomerAddressDefault.InvoiceEnclosed,
                                                                                                                  SortOrder = CustomerAddressDefault.SortOrder,
                                                                                                                  DeletedFlag = CustomerAddressDefault.DeletedFlag,
                                                                                                                  CreatedDateTime = CustomerAddressDefault.CreatedDateTime,
                                                                                                                  CreatedMethod = CustomerAddressDefault.CreatedMethod,
                                                                                                                  CreatedUserID = CustomerAddressDefault.CreatedUserID,
                                                                                                                  LastAmendedDateTime = CustomerAddressDefault.LastAmendedDateTime,
                                                                                                                  LastAmendedMethod = CustomerAddressDefault.LastAmendedMethod,
                                                                                                                  LastAmendedUserID = CustomerAddressDefault.LastAmendedUserID,

                                                                                                                  Site_PublicID = SPR.PublicID,
                                                                                                                  Company_ShortName = CPR.ShortName,
                                                                                                                  Owner_Name = OPR.Name,
                                                                                                                  Created_UserPublicID = SU.PublicID

                                                                                                              }).FirstOrDefaultAsync();


                if (CustomerAddressDefaultdb == null)
                    return Result.Failure<wbm_common.DataObjects.CustomerAddressDefault.dbRow>("CustomerAddressDefault not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.CustomerAddressDefault.dbRow>(CustomerAddressDefaultdb);

            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.CustomerAddressDefault.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>> CustomerAddressDefault_SearchByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended> CustomerAddressDefaultDb = await (from CustomerAddressDefault in _dbContext.CustomerAddressDefault.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).Take(1)
                                                                                                                join site in _dbContext.Site.AsNoTracking()
                                                                                                                    on CustomerAddressDefault.SiteID equals site.ID into sitePR
                                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                                    on CustomerAddressDefault.CompanyID equals company.ID into companyPR
                                                                                                                    from CPR in companyPR.DefaultIfEmpty()
                                                                                                                    join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                                    on CustomerAddressDefault.OwnerID equals Owner.ID into OwnerPR
                                                                                                                    from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                                    join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                                    on CustomerAddressDefault.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                                    from SU in SecurityUserPR.DefaultIfEmpty()
                                                                                                                    select new CustomerAddressDefault.dbRowExtended
                                                                                                                {
                                                                                                                    ID = CustomerAddressDefault.ID,
                                                                                                                    CompanyID = CustomerAddressDefault.CompanyID,
                                                                                                                    OwnerID = CustomerAddressDefault.OwnerID,
                                                                                                                    SiteID = CustomerAddressDefault.SiteID,
                                                                                                                    Description = CustomerAddressDefault.Description,
                                                                                                                    PublicID = CustomerAddressDefault.PublicID,
                                                                                                                    PalletHireDelayDays = CustomerAddressDefault.PalletHireDelayDays,
                                                                                                                    InvoiceEnclosed = CustomerAddressDefault.InvoiceEnclosed,
                                                                                                                    SortOrder = CustomerAddressDefault.SortOrder,
                                                                                                                    DeletedFlag = CustomerAddressDefault.DeletedFlag,
                                                                                                                    CreatedDateTime = CustomerAddressDefault.CreatedDateTime,
                                                                                                                    CreatedMethod = CustomerAddressDefault.CreatedMethod,
                                                                                                                    CreatedUserID = CustomerAddressDefault.CreatedUserID,
                                                                                                                    LastAmendedDateTime = CustomerAddressDefault.LastAmendedDateTime,
                                                                                                                    LastAmendedMethod = CustomerAddressDefault.LastAmendedMethod,
                                                                                                                    LastAmendedUserID = CustomerAddressDefault.LastAmendedUserID,

                                                                                                                    Site_PublicID = SPR.PublicID,
                                                                                                                    Company_ShortName = CPR.ShortName,
                                                                                                                    Owner_Name = OPR.Name,
                                                                                                                    Created_UserPublicID = SU.PublicID
                                                                                                                }).ToListAsync();

                if (CustomerAddressDefaultDb == null)
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>(new List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>(CustomerAddressDefaultDb);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.CustomerAddressDefault.dbRow>> CustomerAddressDefault_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddressDefault.dbRow CustomerAddressDefaultDb = await _dbContext.CustomerAddressDefault.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (CustomerAddressDefaultDb == null)
                    return Result.Failure<wbm_common.DataObjects.CustomerAddressDefault.dbRow>("CustomerAddressDefault not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.CustomerAddressDefault.dbRow>(CustomerAddressDefaultDb);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.CustomerAddressDefault.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>> CustomerAddressDefault_List()
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended> CustomerAddressDefaults = await (from CustomerAddressDefault in _dbContext.CustomerAddressDefault.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                                   join site in _dbContext.Site.AsNoTracking()
                                                                                                                       on CustomerAddressDefault.SiteID equals site.ID into sitePR
                                                                                                                   from SPR in sitePR.DefaultIfEmpty()
                                                                                                                   join company in _dbContext.Company.AsNoTracking()
                                                                                                                   on CustomerAddressDefault.CompanyID equals company.ID into companyPR
                                                                                                                   from CPR in companyPR.DefaultIfEmpty()
                                                                                                                   join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                                   on CustomerAddressDefault.OwnerID equals Owner.ID into OwnerPR
                                                                                                                   from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                                   join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on CustomerAddressDefault.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                                   from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                                   select new CustomerAddressDefault.dbRowExtended
                                                                                                                   {
                                                                                                                       ID = CustomerAddressDefault.ID,
                                                                                                                       CompanyID = CustomerAddressDefault.CompanyID,
                                                                                                                       OwnerID = CustomerAddressDefault.OwnerID,
                                                                                                                       SiteID = CustomerAddressDefault.SiteID,
                                                                                                                       Description = CustomerAddressDefault.Description,
                                                                                                                       PublicID = CustomerAddressDefault.PublicID,
                                                                                                                       PalletHireDelayDays = CustomerAddressDefault.PalletHireDelayDays,
                                                                                                                       InvoiceEnclosed = CustomerAddressDefault.InvoiceEnclosed,
                                                                                                                       SortOrder = CustomerAddressDefault.SortOrder,
                                                                                                                       DeletedFlag = CustomerAddressDefault.DeletedFlag,
                                                                                                                       CreatedDateTime = CustomerAddressDefault.CreatedDateTime,
                                                                                                                       CreatedMethod = CustomerAddressDefault.CreatedMethod,
                                                                                                                       CreatedUserID = CustomerAddressDefault.CreatedUserID,
                                                                                                                       LastAmendedDateTime = CustomerAddressDefault.LastAmendedDateTime,
                                                                                                                       LastAmendedMethod = CustomerAddressDefault.LastAmendedMethod,
                                                                                                                       LastAmendedUserID = CustomerAddressDefault.LastAmendedUserID,

                                                                                                                       Site_PublicID = SPR.PublicID,
                                                                                                                       Company_ShortName = CPR.ShortName,
                                                                                                                       Owner_Name = OPR.Name,
                                                                                                                       Created_UserPublicID = SU.PublicID
                                                                                                                   }).Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>(CustomerAddressDefaults);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>> CustomerAddressDefault_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("CustomerAddressDefault_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended> CustomerAddressDefaults = await (from CustomerAddressDefault in _dbContext.CustomerAddressDefault.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                                       join site in _dbContext.Site.AsNoTracking()
                                                                                                                           on CustomerAddressDefault.SiteID equals site.ID into sitePR
                                                                                                                       from SPR in sitePR.DefaultIfEmpty()
                                                                                                                       join company in _dbContext.Company.AsNoTracking()
                                                                                                                       on CustomerAddressDefault.CompanyID equals company.ID into companyPR
                                                                                                                       from CPR in companyPR.DefaultIfEmpty()
                                                                                                                       join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                                       on CustomerAddressDefault.OwnerID equals Owner.ID into OwnerPR
                                                                                                                       from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                                       join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                                       on CustomerAddressDefault.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                                       from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                                       select new CustomerAddressDefault.dbRowExtended
                                                                                                                       {
                                                                                                                           ID = CustomerAddressDefault.ID,
                                                                                                                           CompanyID = CustomerAddressDefault.CompanyID,
                                                                                                                           OwnerID = CustomerAddressDefault.OwnerID,
                                                                                                                           SiteID = CustomerAddressDefault.SiteID,
                                                                                                                           Description = CustomerAddressDefault.Description,
                                                                                                                           PublicID = CustomerAddressDefault.PublicID,
                                                                                                                           PalletHireDelayDays = CustomerAddressDefault.PalletHireDelayDays,
                                                                                                                           InvoiceEnclosed = CustomerAddressDefault.InvoiceEnclosed,
                                                                                                                           SortOrder = CustomerAddressDefault.SortOrder,
                                                                                                                           DeletedFlag = CustomerAddressDefault.DeletedFlag,
                                                                                                                           CreatedDateTime = CustomerAddressDefault.CreatedDateTime,
                                                                                                                           CreatedMethod = CustomerAddressDefault.CreatedMethod,
                                                                                                                           CreatedUserID = CustomerAddressDefault.CreatedUserID,
                                                                                                                           LastAmendedDateTime = CustomerAddressDefault.LastAmendedDateTime,
                                                                                                                           LastAmendedMethod = CustomerAddressDefault.LastAmendedMethod,
                                                                                                                           LastAmendedUserID = CustomerAddressDefault.LastAmendedUserID,

                                                                                                                           Site_PublicID = SPR.PublicID,
                                                                                                                           Company_ShortName = CPR.ShortName,
                                                                                                                           Owner_Name = OPR.Name,
                                                                                                                           Created_UserPublicID = SU.PublicID
                                                                                                                       }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>(CustomerAddressDefaults);
                }
                catch (Exception ex)
                {
                    logger.Error("CustomerAddressDefault_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>> CustomerAddressDefault_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended> CustomerAddressDefaults = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>(CustomerAddressDefaults);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddressDefault.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
