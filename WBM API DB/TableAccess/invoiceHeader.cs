using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region InvoiceHeader
        public async Task<Result<int>> InvoiceHeader_Insert(wbm_common.DataObjects.InvoiceHeader.dbRow InvoiceHeader)
        {
            try
            {
                await _dbContext.InvoiceHeader.AddAsync(InvoiceHeader);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceHeader_Insert - " + InvoiceHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> InvoiceHeader_Update(wbm_common.DataObjects.InvoiceHeader.dbRow InvoiceHeader)
        {
            try
            {
                _dbContext.InvoiceHeader.Attach(InvoiceHeader);
                _dbContext.Entry(InvoiceHeader).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceHeader_Update - " + InvoiceHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>> InvoiceHeader_ListByCompanyID(int CompanyID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended> InvoiceHeaderDb = await (from InvoiceHeader in _dbContext.InvoiceHeader.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.CompanyID == CompanyID)
                                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                                      on InvoiceHeader.SiteID equals site.ID into sitePR
                                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                                      on InvoiceHeader.CompanyID equals company.ID into companyPR
                                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                      on InvoiceHeader.OwnerID equals Owner.ID into OwnerPR
                                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                  join InvoiceCycle in _dbContext.InvoiceCycle.AsNoTracking()
                                                                                                      on InvoiceHeader.InvoiceCycleID equals InvoiceCycle.ID into InvoiceCyclePR
                                                                                                  from ICPR in InvoiceCyclePR.DefaultIfEmpty()

                                                                                                  select new wbm_common.DataObjects.InvoiceHeader.dbRowExtended
                                                                                                  {
                                                                                                      ID = InvoiceHeader.ID,
                                                                                                      CompanyID = InvoiceHeader.CompanyID,
                                                                                                      SiteID = InvoiceHeader.SiteID,
                                                                                                      OwnerID = InvoiceHeader.OwnerID,
                                                                                                      InvoiceNumber = InvoiceHeader.InvoiceNumber,
                                                                                                      InvoicePrefix = InvoiceHeader.InvoicePrefix,
                                                                                                      InvoiceDate = InvoiceHeader.InvoiceDate,
                                                                                                      InvoiceCycleID = InvoiceHeader.InvoiceCycleID,
                                                                                                      ClientExGSTAmount = InvoiceHeader.ClientExGSTAmount,
                                                                                                      ClientGSTAmount = InvoiceHeader.ClientGSTAmount,
                                                                                                      ClientTotalAmount = InvoiceHeader.ClientTotalAmount,
                                                                                                      InvoiceFormatID = InvoiceHeader.InvoiceFormatID,
                                                                                                      FirstExportDateTime = InvoiceHeader.FirstExportDateTime,
                                                                                                      DeletedFlag = InvoiceHeader.DeletedFlag,
                                                                                                      CreatedDateTime = InvoiceHeader.CreatedDateTime,
                                                                                                      CreatedMethod = InvoiceHeader.CreatedMethod,
                                                                                                      CreatedUserID = InvoiceHeader.CreatedUserID,
                                                                                                      LastAmendedDateTime = InvoiceHeader.LastAmendedDateTime,
                                                                                                      LastAmendedMethod = InvoiceHeader.LastAmendedMethod,
                                                                                                      LastAmendedUserID = InvoiceHeader.LastAmendedUserID,

                                                                                                      InvoiceCycle_PublicID = ICPR.PublicID,
                                                                                                      Site_PublicID = SPR.PublicID,
                                                                                                      Owner_Name = OPR.Name,
                                                                                                      Company_ShortName = CPR.ShortName
                                                                                                  }).ToListAsync();

                if (InvoiceHeaderDb == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>(new List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>(InvoiceHeaderDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceHeader_ListByCompanyID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>> InvoiceHeader_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended> InvoiceHeaderDb = await (from InvoiceHeader in _dbContext.InvoiceHeader.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                                      on InvoiceHeader.SiteID equals site.ID into sitePR
                                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                                      on InvoiceHeader.CompanyID equals company.ID into companyPR
                                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                      on InvoiceHeader.OwnerID equals Owner.ID into OwnerPR
                                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                  join InvoiceCycle in _dbContext.InvoiceCycle.AsNoTracking()
                                                                                                      on InvoiceHeader.InvoiceCycleID equals InvoiceCycle.ID into InvoiceCyclePR
                                                                                                  from ICPR in InvoiceCyclePR.DefaultIfEmpty()

                                                                                                  select new wbm_common.DataObjects.InvoiceHeader.dbRowExtended
                                                                                                  {
                                                                                                      ID = InvoiceHeader.ID,
                                                                                                      CompanyID = InvoiceHeader.CompanyID,
                                                                                                      SiteID = InvoiceHeader.SiteID,
                                                                                                      OwnerID = InvoiceHeader.OwnerID,
                                                                                                      InvoiceNumber = InvoiceHeader.InvoiceNumber,
                                                                                                      InvoicePrefix = InvoiceHeader.InvoicePrefix,
                                                                                                      InvoiceDate = InvoiceHeader.InvoiceDate,
                                                                                                      InvoiceCycleID = InvoiceHeader.InvoiceCycleID,
                                                                                                      ClientExGSTAmount = InvoiceHeader.ClientExGSTAmount,
                                                                                                      ClientGSTAmount = InvoiceHeader.ClientGSTAmount,
                                                                                                      ClientTotalAmount = InvoiceHeader.ClientTotalAmount,
                                                                                                      InvoiceFormatID = InvoiceHeader.InvoiceFormatID,
                                                                                                      FirstExportDateTime = InvoiceHeader.FirstExportDateTime,
                                                                                                      DeletedFlag = InvoiceHeader.DeletedFlag,
                                                                                                      CreatedDateTime = InvoiceHeader.CreatedDateTime,
                                                                                                      CreatedMethod = InvoiceHeader.CreatedMethod,
                                                                                                      CreatedUserID = InvoiceHeader.CreatedUserID,
                                                                                                      LastAmendedDateTime = InvoiceHeader.LastAmendedDateTime,
                                                                                                      LastAmendedMethod = InvoiceHeader.LastAmendedMethod,
                                                                                                      LastAmendedUserID = InvoiceHeader.LastAmendedUserID,

                                                                                                      InvoiceCycle_PublicID = ICPR.PublicID,
                                                                                                      Site_PublicID = SPR.PublicID,
                                                                                                      Owner_Name = OPR.Name,
                                                                                                      Company_ShortName = CPR.ShortName
                                                                                                  }).ToListAsync();

                if (InvoiceHeaderDb == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>(new List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>(InvoiceHeaderDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceHeader_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceHeader.dbRow>> InvoiceHeader_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceHeader.dbRow InvoiceHeaderDb = await _dbContext.InvoiceHeader.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (InvoiceHeaderDb == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceHeader.dbRow>("InvoiceHeader not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceHeader.dbRow>(InvoiceHeaderDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceHeader_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceHeader.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>> InvoiceHeader_List()
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended> InvoiceHeaders = await (from InvoiceHeader in _dbContext.InvoiceHeader.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                 join site in _dbContext.Site.AsNoTracking()
                                                                                                     on InvoiceHeader.SiteID equals site.ID into sitePR
                                                                                                 from SPR in sitePR.DefaultIfEmpty()
                                                                                                 join company in _dbContext.Company.AsNoTracking()
                                                                                                     on InvoiceHeader.CompanyID equals company.ID into companyPR
                                                                                                 from CPR in companyPR.DefaultIfEmpty()
                                                                                                 join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                     on InvoiceHeader.OwnerID equals Owner.ID into OwnerPR
                                                                                                 from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                 join InvoiceCycle in _dbContext.InvoiceCycle.AsNoTracking()
                                                                                                     on InvoiceHeader.InvoiceCycleID equals InvoiceCycle.ID into InvoiceCyclePR
                                                                                                 from ICPR in InvoiceCyclePR.DefaultIfEmpty()

                                                                                                 select new wbm_common.DataObjects.InvoiceHeader.dbRowExtended
                                                                                                 {
                                                                                                     ID = InvoiceHeader.ID,
                                                                                                     CompanyID = InvoiceHeader.CompanyID,
                                                                                                     SiteID = InvoiceHeader.SiteID,
                                                                                                     OwnerID = InvoiceHeader.OwnerID,
                                                                                                     InvoiceNumber = InvoiceHeader.InvoiceNumber,
                                                                                                     InvoicePrefix = InvoiceHeader.InvoicePrefix,
                                                                                                     InvoiceDate = InvoiceHeader.InvoiceDate,
                                                                                                     InvoiceCycleID = InvoiceHeader.InvoiceCycleID,
                                                                                                     ClientExGSTAmount = InvoiceHeader.ClientExGSTAmount,
                                                                                                     ClientGSTAmount = InvoiceHeader.ClientGSTAmount,
                                                                                                     ClientTotalAmount = InvoiceHeader.ClientTotalAmount,
                                                                                                     InvoiceFormatID = InvoiceHeader.InvoiceFormatID,
                                                                                                     FirstExportDateTime = InvoiceHeader.FirstExportDateTime,
                                                                                                     DeletedFlag = InvoiceHeader.DeletedFlag,
                                                                                                     CreatedDateTime = InvoiceHeader.CreatedDateTime,
                                                                                                     CreatedMethod = InvoiceHeader.CreatedMethod,
                                                                                                     CreatedUserID = InvoiceHeader.CreatedUserID,
                                                                                                     LastAmendedDateTime = InvoiceHeader.LastAmendedDateTime,
                                                                                                     LastAmendedMethod = InvoiceHeader.LastAmendedMethod,
                                                                                                     LastAmendedUserID = InvoiceHeader.LastAmendedUserID,

                                                                                                     InvoiceCycle_PublicID = ICPR.PublicID,
                                                                                                     Site_PublicID = SPR.PublicID,
                                                                                                     Owner_Name = OPR.Name,
                                                                                                     Company_ShortName = CPR.ShortName
                                                                                                 }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>(InvoiceHeaders);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceHeader_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>> InvoiceHeader_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("InvoiceHeader_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended> InvoiceHeaders = await (from InvoiceHeader in _dbContext.InvoiceHeader.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                     join site in _dbContext.Site.AsNoTracking()
                                                                                                         on InvoiceHeader.SiteID equals site.ID into sitePR
                                                                                                     from SPR in sitePR.DefaultIfEmpty()
                                                                                                     join company in _dbContext.Company.AsNoTracking()
                                                                                                         on InvoiceHeader.CompanyID equals company.ID into companyPR
                                                                                                     from CPR in companyPR.DefaultIfEmpty()
                                                                                                     join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                         on InvoiceHeader.OwnerID equals Owner.ID into OwnerPR
                                                                                                     from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                     join InvoiceCycle in _dbContext.InvoiceCycle.AsNoTracking()
                                                                                                         on InvoiceHeader.InvoiceCycleID equals InvoiceCycle.ID into InvoiceCyclePR
                                                                                                     from ICPR in InvoiceCyclePR.DefaultIfEmpty()

                                                                                                     select new wbm_common.DataObjects.InvoiceHeader.dbRowExtended
                                                                                                     {
                                                                                                         ID = InvoiceHeader.ID,
                                                                                                         CompanyID = InvoiceHeader.CompanyID,
                                                                                                         SiteID = InvoiceHeader.SiteID,
                                                                                                         OwnerID = InvoiceHeader.OwnerID,
                                                                                                         InvoiceNumber = InvoiceHeader.InvoiceNumber,
                                                                                                         InvoicePrefix = InvoiceHeader.InvoicePrefix,
                                                                                                         InvoiceDate = InvoiceHeader.InvoiceDate,
                                                                                                         InvoiceCycleID = InvoiceHeader.InvoiceCycleID,
                                                                                                         ClientExGSTAmount = InvoiceHeader.ClientExGSTAmount,
                                                                                                         ClientGSTAmount = InvoiceHeader.ClientGSTAmount,
                                                                                                         ClientTotalAmount = InvoiceHeader.ClientTotalAmount,
                                                                                                         InvoiceFormatID = InvoiceHeader.InvoiceFormatID,
                                                                                                         FirstExportDateTime = InvoiceHeader.FirstExportDateTime,
                                                                                                         DeletedFlag = InvoiceHeader.DeletedFlag,
                                                                                                         CreatedDateTime = InvoiceHeader.CreatedDateTime,
                                                                                                         CreatedMethod = InvoiceHeader.CreatedMethod,
                                                                                                         CreatedUserID = InvoiceHeader.CreatedUserID,
                                                                                                         LastAmendedDateTime = InvoiceHeader.LastAmendedDateTime,
                                                                                                         LastAmendedMethod = InvoiceHeader.LastAmendedMethod,
                                                                                                         LastAmendedUserID = InvoiceHeader.LastAmendedUserID,

                                                                                                         InvoiceCycle_PublicID = ICPR.PublicID,
                                                                                                         Site_PublicID = SPR.PublicID,
                                                                                                         Owner_Name = OPR.Name,
                                                                                                         Company_ShortName = CPR.ShortName
                                                                                                     }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>(InvoiceHeaders);
                }
                catch (Exception ex)
                {
                    logger.Error("InvoiceHeader_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>> InvoiceHeader_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended> InvoiceHeaders = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>(InvoiceHeaders);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceHeader_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceHeader.dbRow>>> InvoiceHeader_ListAllUnExportedForCompany(int CompanyID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceHeader.dbRow> ListIH = await _dbContext.InvoiceHeader.AsNoTracking().Where(x => !x.DeletedFlag && x.FirstExportDateTime != null && x.CompanyID == CompanyID).ToListAsync();

                // no rows is valid
                return Result.Success<List<wbm_common.DataObjects.InvoiceHeader.dbRow>>(ListIH);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_ListAllUnInvoiced - : " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceHeader.dbRow>>("Internal Exception occured");
            }
        }


        #endregion
    }
}
