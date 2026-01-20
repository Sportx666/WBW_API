using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region LongRunningProcess
        public async Task<Result<int>> LongRunningHeaderRow_Insert(wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader)
        {
            try
            {
                await _dbContext.LongRunProcessHeader.AddAsync(longRunProcessHeader);
                await _dbContext.SaveChangesAsync();
                return Result.Success<int>(longRunProcessHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("LongRunningHeaderRow_Insert: Process - " + longRunProcessHeader.ProcessName + ", " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> LongRunningRows_InsertAndUpdate(wbm_common.DataObjects.LongRunProcessStatus.dbRow longRunProcessStatus,
            wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader)
        {
            try
            {
                using (var scope = _dbContext.Database.BeginTransaction())
                {
                    _dbContext.LongRunProcessHeader.Update(longRunProcessHeader);
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.LongRunProcessStatus.AddAsync(longRunProcessStatus);
                    await _dbContext.SaveChangesAsync();
                    Result<int> res = Result.Success<int>(longRunProcessStatus.ID);
                    scope.Commit();
                    return res;
                }
            }
            catch (Exception ex)
            {
                logger.Error("LongRunningRows_InsertAndUpdate: HeaderID - " + longRunProcessHeader.ID.ToString() + ", Descr - " + longRunProcessStatus.StatusDescription + ", " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        #endregion

        #region LongRunProcessStatusDescription
        public async Task<Result<int>> LongRunProcessStatusDescription_Insert(wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow LongRunProcessStatusDescription)
        {
            try
            {
                await _dbContext.LongRunProcessStatusDescription.AddAsync(LongRunProcessStatusDescription);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(LongRunProcessStatusDescription.ID);
            }
            catch (Exception ex)
            {
                logger.Error("LongRunProcessStatusDescription_Insert - " + LongRunProcessStatusDescription.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> LongRunProcessStatusDescription_Update(wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow LongRunProcessStatusDescription)
        {
            try
            {
                _dbContext.LongRunProcessStatusDescription.Attach(LongRunProcessStatusDescription);
                _dbContext.Entry(LongRunProcessStatusDescription).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(LongRunProcessStatusDescription.ID);
            }
            catch (Exception ex)
            {
                logger.Error("LongRunProcessStatusDescription_Update - " + LongRunProcessStatusDescription.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>> LongRunProcessStatusDescription_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow LongRunProcessStatusDescription = await _dbContext.LongRunProcessStatusDescription.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (LongRunProcessStatusDescription == null)
                    return Result.Failure<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>("LongRunProcessStatusDescription not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>(LongRunProcessStatusDescription);

            }
            catch (Exception ex)
            {
                logger.Error("LongRunProcessStatusDescription_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>> LongRunProcessStatusDescriptionList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow LongRunProcessStatusDescription = await _dbContext.LongRunProcessStatusDescription.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (LongRunProcessStatusDescription == null)
                    return Result.Success<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>(new List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>(new List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow> { LongRunProcessStatusDescription });
            }
            catch (Exception ex)
            {
                logger.Error("LongRunProcessStatusDescription_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>> LongRunProcessStatusDescription_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow LongRunProcessStatusDescription = await _dbContext.LongRunProcessStatusDescription.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (LongRunProcessStatusDescription == null)
                    return Result.Failure<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>("LongRunProcessStatusDescription not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>(LongRunProcessStatusDescription);
            }
            catch (Exception ex)
            {
                logger.Error("LongRunProcessStatusDescription_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>> LongRunProcessStatusDescription_List()
        {
            try
            {
                List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow> LongRunProcessStatusDescriptions = await _dbContext.LongRunProcessStatusDescription.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>(LongRunProcessStatusDescriptions);
            }
            catch (Exception ex)
            {
                logger.Error("LongRunProcessStatusDescription_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>> LongRunProcessStatusDescription_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("LongRunProcessStatusDescription_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow> LongRunProcessStatusDescriptions = await _dbContext.LongRunProcessStatusDescription.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>(LongRunProcessStatusDescriptions);
                }
                catch (Exception ex)
                {
                    logger.Error("LongRunProcessStatusDescription_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>> LongRunProcessStatusDescription_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow> LongRunProcessStatusDescriptions = await _dbContext.LongRunProcessStatusDescription.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>(LongRunProcessStatusDescriptions);
            }
            catch (Exception ex)
            {
                logger.Error("LongRunProcessStatusDescription_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}