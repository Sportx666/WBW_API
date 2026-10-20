using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> OrderHeader_Insert(wbm_common.DataObjects.OrderHeader.dbRow OrderHeader)
        {
            try
            {
                await _dbContext.OrderHeader.AddAsync(OrderHeader);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(OrderHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("OrderHeader_Insert - " + OrderHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> OrderHeader_Update(wbm_common.DataObjects.OrderHeader.dbRow OrderHeader)
        {
            try
            {
                _dbContext.OrderHeader.Attach(OrderHeader);
                _dbContext.Entry(OrderHeader).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(OrderHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("OrderHeader_Update - " + OrderHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.OrderHeader.dbRow>> OrderHeader_SearchByTransactionHeaderID(int TransactionHeaderID)
        {
            try
            {
                wbm_common.DataObjects.OrderHeader.dbRow OrderHeader = await _dbContext.OrderHeader.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.TransactionHeaderID == TransactionHeaderID);

                if (OrderHeader == null)
                    return Result.Failure<wbm_common.DataObjects.OrderHeader.dbRow>("OrderHeader not found : " + TransactionHeaderID);
                else
                    return Result.Success<wbm_common.DataObjects.OrderHeader.dbRow>(OrderHeader);

            }
            catch (Exception ex)
            {
                logger.Error("OrderHeader_SearchByTransactionHeaderID - " + TransactionHeaderID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.OrderHeader.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.OrderHeader.dbRow>>> OrderHeader_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.OrderHeader.dbRow> OrderHeaders = await _dbContext.OrderHeader.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.OrderHeader.dbRow>>(OrderHeaders);
            }
            catch (Exception ex)
            {
                logger.Error("OrderHeader_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.OrderHeader.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> PurchaseOrderHeader_Insert(wbm_common.DataObjects.PurchaseOrderHeader.dbRow PurchaseOrderHeader)
        {
            try
            {
                await _dbContext.PurchaseOrderHeader.AddAsync(PurchaseOrderHeader);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PurchaseOrderHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PurchaseOrderHeader_Insert - " + PurchaseOrderHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> PurchaseOrderHeader_Update(wbm_common.DataObjects.PurchaseOrderHeader.dbRow PurchaseOrderHeader)
        {
            try
            {
                _dbContext.PurchaseOrderHeader.Attach(PurchaseOrderHeader);
                _dbContext.Entry(PurchaseOrderHeader).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PurchaseOrderHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PurchaseOrderHeader_Update - " + PurchaseOrderHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.PurchaseOrderHeader.dbRow>> PurchaseOrderHeader_SearchByTransactionHeaderID(int TransactionHeaderID)
        {
            try
            {
                wbm_common.DataObjects.PurchaseOrderHeader.dbRow PurchaseOrderHeader = await _dbContext.PurchaseOrderHeader.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.TransactionHeaderID == TransactionHeaderID);

                if (PurchaseOrderHeader == null)
                    return Result.Failure<wbm_common.DataObjects.PurchaseOrderHeader.dbRow>("PurchaseOrderHeader not found : " + TransactionHeaderID);
                else
                    return Result.Success<wbm_common.DataObjects.PurchaseOrderHeader.dbRow>(PurchaseOrderHeader);

            }
            catch (Exception ex)
            {
                logger.Error("PurchaseOrderHeader_SearchByTransactionHeaderID - " + TransactionHeaderID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PurchaseOrderHeader.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PurchaseOrderHeader.dbRow>>> PurchaseOrderHeader_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.PurchaseOrderHeader.dbRow> PurchaseOrderHeaders = await _dbContext.PurchaseOrderHeader.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.PurchaseOrderHeader.dbRow>>(PurchaseOrderHeaders);
            }
            catch (Exception ex)
            {
                logger.Error("PurchaseOrderHeader_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PurchaseOrderHeader.dbRow>>("Internal Exception occured");
            }
        }
    }
}
