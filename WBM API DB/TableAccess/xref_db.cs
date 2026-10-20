using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> PaperlessTransactionXref_Insert(wbm_common.DataObjects.PaperlessTransactionXref.dbRow PaperlessTransactionXref)
        {
            try
            {
                await _dbContext.PaperlessTransactionXref.AddAsync(PaperlessTransactionXref);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PaperlessTransactionXref.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_Insert - " + PaperlessTransactionXref.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> PaperlessTransactionXref_Update(wbm_common.DataObjects.PaperlessTransactionXref.dbRow PaperlessTransactionXref)
        {
            try
            {
                _dbContext.PaperlessTransactionXref.Attach(PaperlessTransactionXref);
                _dbContext.Entry(PaperlessTransactionXref).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PaperlessTransactionXref.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_Update - " + PaperlessTransactionXref.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>> PaperlessTransactionXref_ListBySingleCompanyID(int CompanyID)
        {
            try
            {
                List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> PaperlessTransactionXrefDb = await _dbContext.PaperlessTransactionXref.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.CompanyID == CompanyID).ToListAsync();

                if (PaperlessTransactionXrefDb == null)
                    return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(new List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(PaperlessTransactionXrefDb);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_ListBySingleCompanyID - " + CompanyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>> PaperlessTransactionXref_ListByPaperlessKeyID(string PaperlessKeyID)
        {
            try
            {
                List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> PaperlessTransactionXrefDb = await _dbContext.PaperlessTransactionXref.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.PaperlessKeyID == PaperlessKeyID).ToListAsync();

                if (PaperlessTransactionXrefDb == null)
                    return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(new List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(PaperlessTransactionXrefDb);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_ListByPaperlessKEYID - " + PaperlessKeyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>> PaperlessTransactionXref_SearchByReferenceNumber(string ReferenceNumber)
        {
            try
            {
                wbm_common.DataObjects.PaperlessTransactionXref.dbRow PaperlessTransactionXrefDb = await _dbContext.PaperlessTransactionXref.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ReferenceNumber == ReferenceNumber).FirstOrDefaultAsync();

                if (PaperlessTransactionXrefDb == null)
                    return Result.Success<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>(new wbm_common.DataObjects.PaperlessTransactionXref.dbRow());
                else
                    return Result.Success<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>(PaperlessTransactionXrefDb);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_ListByReferenceNumber - " + ReferenceNumber + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>> PaperlessTransactionXref_ListByTransactionHeaderID(int TransactionHeaderID)
        {
            try
            {
                List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> PaperlessTransactionXrefDb = await _dbContext.PaperlessTransactionXref.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.TransactionHeaderID == TransactionHeaderID).ToListAsync();

                if (PaperlessTransactionXrefDb == null)
                    return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(new List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(PaperlessTransactionXrefDb);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_ListByTransactionHeaderID - " + TransactionHeaderID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>("Internal Exception occured");
            }
        }
        // read and search are different because of the deleted flag

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>> PaperlessTransactionXref_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> PaperlessTransactionXrefDb = await _dbContext.PaperlessTransactionXref.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).ToListAsync();

                if (PaperlessTransactionXrefDb == null)
                    return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(new List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(PaperlessTransactionXrefDb);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>> PaperlessTransactionXref_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PaperlessTransactionXref.dbRow PaperlessTransactionXrefDb = await _dbContext.PaperlessTransactionXref.AsNoTracking()
                                                                        .Where(x => x.ID == ID).Take(1).FirstOrDefaultAsync();

                if (PaperlessTransactionXrefDb == null)
                    return Result.Failure<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>("PaperlessTransactionXref not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>(PaperlessTransactionXrefDb);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>> PaperlessTransactionXref_List()
        {
            try
            {
                List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> PaperlessTransactionXrefs = await _dbContext.PaperlessTransactionXref.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(PaperlessTransactionXrefs);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>> PaperlessTransactionXref_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("PaperlessTransactionXref_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> PaperlessTransactionXrefs = await _dbContext.PaperlessTransactionXref.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(PaperlessTransactionXrefs);
                }
                catch (Exception ex)
                {
                    logger.Error("PaperlessTransactionXref_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>> PaperlessTransactionXref_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> PaperlessTransactionXrefs = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>(PaperlessTransactionXrefs);
            }
            catch (Exception ex)
            {
                logger.Error("PaperlessTransactionXref_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>("Internal Exception occured");
            }
        }
    }
}
