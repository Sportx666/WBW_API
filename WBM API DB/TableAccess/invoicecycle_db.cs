using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region InvoiceCycle
        public async Task<Result<int>> InvoiceCycle_Insert(wbm_common.DataObjects.InvoiceCycle.dbRow InvoiceCycle)
        {
            try
            {
                await _dbContext.InvoiceCycle.AddAsync(InvoiceCycle);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceCycle.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycle_Insert - " + InvoiceCycle.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> InvoiceCycle_Update(wbm_common.DataObjects.InvoiceCycle.dbRow InvoiceCycle)
        {
            try
            {
                _dbContext.InvoiceCycle.Attach(InvoiceCycle);
                _dbContext.Entry(InvoiceCycle).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceCycle.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycle_Update - " + InvoiceCycle.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceCycle.dbRow>> InvoiceCycle_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycle.dbRow InvoiceCycle = await _dbContext.InvoiceCycle.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (InvoiceCycle == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceCycle.dbRow>("InvoiceCycle not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceCycle.dbRow>(InvoiceCycle);

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycle_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceCycle.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>> InvoiceCycleList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycle.dbRow InvoiceCycle = await _dbContext.InvoiceCycle.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (InvoiceCycle == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>(new List<wbm_common.DataObjects.InvoiceCycle.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>(new List<wbm_common.DataObjects.InvoiceCycle.dbRow> { InvoiceCycle });
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycle_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceCycle.dbRow>> InvoiceCycle_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycle.dbRow InvoiceCycle = await _dbContext.InvoiceCycle.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (InvoiceCycle == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceCycle.dbRow>("InvoiceCycle not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceCycle.dbRow>(InvoiceCycle);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycle_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceCycle.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>> InvoiceCycle_List()
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceCycle.dbRow> InvoiceCycles = await _dbContext.InvoiceCycle.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>(InvoiceCycles);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycle_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>> InvoiceCycle_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("InvoiceCycle_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.InvoiceCycle.dbRow> InvoiceCycles = await _dbContext.InvoiceCycle.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>(InvoiceCycles);
                }
                catch (Exception ex)
                {
                    logger.Error("InvoiceCycle_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>> InvoiceCycle_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceCycle.dbRow> InvoiceCycles = await _dbContext.InvoiceCycle.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>(InvoiceCycles);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycle_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycle.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region InvoiceCycleCurrent
        public async Task<Result<int>> InvoiceCycleCurrent_Insert(wbm_common.DataObjects.InvoiceCycleCurrent.dbRow InvoiceCycleCurrent)
        {
            try
            {
                await _dbContext.InvoiceCycleCurrent.AddAsync(InvoiceCycleCurrent);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceCycleCurrent.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleCurrent_Insert - " + InvoiceCycleCurrent.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> InvoiceCycleCurrent_Update(wbm_common.DataObjects.InvoiceCycleCurrent.dbRow InvoiceCycleCurrent)
        {
            try
            {
                _dbContext.InvoiceCycleCurrent.Attach(InvoiceCycleCurrent);
                _dbContext.Entry(InvoiceCycleCurrent).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceCycleCurrent.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleCurrent_Update - " + InvoiceCycleCurrent.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>> InvoiceCycleCurrent_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycleCurrent.dbRow InvoiceCycleCurrent = await _dbContext.InvoiceCycleCurrent.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (InvoiceCycleCurrent == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>("InvoiceCycleCurrent not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>(InvoiceCycleCurrent);

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleCurrent_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>> InvoiceCycleCurrentList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycleCurrent.dbRow InvoiceCycleCurrent = await _dbContext.InvoiceCycleCurrent.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (InvoiceCycleCurrent == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>(new List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>(new List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow> { InvoiceCycleCurrent });
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleCurrent_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>> InvoiceCycleCurrent_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycleCurrent.dbRow InvoiceCycleCurrent = await _dbContext.InvoiceCycleCurrent.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (InvoiceCycleCurrent == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>("InvoiceCycleCurrent not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>(InvoiceCycleCurrent);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleCurrent_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>> InvoiceCycleCurrent_List()
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow> InvoiceCycleCurrents = await _dbContext.InvoiceCycleCurrent.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>(InvoiceCycleCurrents);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleCurrent_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>> InvoiceCycleCurrent_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("InvoiceCycleCurrent_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow> InvoiceCycleCurrents = await _dbContext.InvoiceCycleCurrent.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>(InvoiceCycleCurrents);
                }
                catch (Exception ex)
                {
                    logger.Error("InvoiceCycleCurrent_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>> InvoiceCycleCurrent_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow> InvoiceCycleCurrents = await _dbContext.InvoiceCycleCurrent.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>(InvoiceCycleCurrents);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleCurrent_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region InvoiceCycleHistory
        public async Task<Result<int>> InvoiceCycleHistory_Insert(wbm_common.DataObjects.InvoiceCycleHistory.dbRow InvoiceCycleHistory)
        {
            try
            {
                await _dbContext.InvoiceCycleHistory.AddAsync(InvoiceCycleHistory);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceCycleHistory.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleHistory_Insert - " + InvoiceCycleHistory.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> InvoiceCycleHistory_Update(wbm_common.DataObjects.InvoiceCycleHistory.dbRow InvoiceCycleHistory)
        {
            try
            {
                _dbContext.InvoiceCycleHistory.Attach(InvoiceCycleHistory);
                _dbContext.Entry(InvoiceCycleHistory).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceCycleHistory.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleHistory_Update - " + InvoiceCycleHistory.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>> InvoiceCycleHistory_ListByCompanyID(int CompanyID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycleHistory.dbRow InvoiceCycleHistory = await _dbContext.InvoiceCycleHistory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.CompanyID == CompanyID);

                if (InvoiceCycleHistory == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>(new List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>(new List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> { InvoiceCycleHistory });
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleHistory_ListByCompanyID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>> InvoiceCycleHistory_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycleHistory.dbRow InvoiceCycleHistory = await _dbContext.InvoiceCycleHistory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (InvoiceCycleHistory == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>("InvoiceCycleHistory not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>(InvoiceCycleHistory);

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleHistory_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>> InvoiceCycleHistoryList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycleHistory.dbRow InvoiceCycleHistory = await _dbContext.InvoiceCycleHistory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (InvoiceCycleHistory == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>(new List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>(new List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> { InvoiceCycleHistory });
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleHistory_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>> InvoiceCycleHistory_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceCycleHistory.dbRow InvoiceCycleHistory = await _dbContext.InvoiceCycleHistory.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (InvoiceCycleHistory == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>("InvoiceCycleHistory not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>(InvoiceCycleHistory);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleHistory_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>> InvoiceCycleHistory_List()
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> InvoiceCycleHistorys = await _dbContext.InvoiceCycleHistory.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>(InvoiceCycleHistorys);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleHistory_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>> InvoiceCycleHistory_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("InvoiceCycleHistory_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> InvoiceCycleHistorys = await _dbContext.InvoiceCycleHistory.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>(InvoiceCycleHistorys);
                }
                catch (Exception ex)
                {
                    logger.Error("InvoiceCycleHistory_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>> InvoiceCycleHistory_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> InvoiceCycleHistorys = await _dbContext.InvoiceCycleHistory.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>(InvoiceCycleHistorys);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceCycleHistory_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>("Internal Exception occured");
            }
        }

        #endregion
    }
}
