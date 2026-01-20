using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> PriorityOrder_Insert(wbm_common.DataObjects.PriorityOrder.dbRow PriorityOrder)
        {
            try
            {
                await _dbContext.PriorityOrder.AddAsync(PriorityOrder);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PriorityOrder.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PriorityOrder_Insert - " + PriorityOrder.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> PriorityOrder_Update(wbm_common.DataObjects.PriorityOrder.dbRow PriorityOrder)
        {
            try
            {
                _dbContext.PriorityOrder.Attach(PriorityOrder);
                _dbContext.Entry(PriorityOrder).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(PriorityOrder.ID);
            }
            catch (Exception ex)
            {
                logger.Error("PriorityOrder_Update - " + PriorityOrder.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.PriorityOrder.dbRow>>> PriorityOrder_ListBySingleSiteID(int SiteID)
        {
            try
            {
                wbm_common.DataObjects.PriorityOrder.dbRow PriorityOrder = await _dbContext.PriorityOrder.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.SiteID == SiteID);

                if (PriorityOrder == null)
                    return Result.Success<List<wbm_common.DataObjects.PriorityOrder.dbRow>>(new List<wbm_common.DataObjects.PriorityOrder.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PriorityOrder.dbRow>>(new List<wbm_common.DataObjects.PriorityOrder.dbRow> { PriorityOrder });
            }
            catch (Exception ex)
            {
                logger.Error("PriorityOrder_ListBySingleSiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PriorityOrder.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.PriorityOrder.dbRow>> PriorityOrder_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PriorityOrder.dbRow PriorityOrder = await _dbContext.PriorityOrder.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (PriorityOrder == null)
                    return Result.Failure<wbm_common.DataObjects.PriorityOrder.dbRow>("PriorityOrder not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.PriorityOrder.dbRow>(PriorityOrder);

            }
            catch (Exception ex)
            {
                logger.Error("PriorityOrder_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PriorityOrder.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.PriorityOrder.dbRow>>> PriorityOrderList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PriorityOrder.dbRow PriorityOrder = await _dbContext.PriorityOrder.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (PriorityOrder == null)
                    return Result.Success<List<wbm_common.DataObjects.PriorityOrder.dbRow>>(new List<wbm_common.DataObjects.PriorityOrder.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.PriorityOrder.dbRow>>(new List<wbm_common.DataObjects.PriorityOrder.dbRow> { PriorityOrder });
            }
            catch (Exception ex)
            {
                logger.Error("PriorityOrder_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PriorityOrder.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.PriorityOrder.dbRow>> PriorityOrder_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.PriorityOrder.dbRow PriorityOrder = await _dbContext.PriorityOrder.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (PriorityOrder == null)
                    return Result.Failure<wbm_common.DataObjects.PriorityOrder.dbRow>("PriorityOrder not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.PriorityOrder.dbRow>(PriorityOrder);
            }
            catch (Exception ex)
            {
                logger.Error("PriorityOrder_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.PriorityOrder.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PriorityOrder.dbRow>>> PriorityOrder_List()
        {
            try
            {
                List<wbm_common.DataObjects.PriorityOrder.dbRow> PriorityOrders = await _dbContext.PriorityOrder.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.PriorityOrder.dbRow>>(PriorityOrders);
            }
            catch (Exception ex)
            {
                logger.Error("PriorityOrder_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.PriorityOrder.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.PriorityOrder.dbRow>>> PriorityOrder_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.PriorityOrder.dbRow> PriorityOrders = await _dbContext.PriorityOrder.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.PriorityOrder.dbRow>>(PriorityOrders);
            }
            catch (Exception ex)
            {
                logger.Error("PriorityOrder_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.PriorityOrder.dbRow>>("Internal Exception occured");
            }
        }
    }
}
