using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

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


        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRow>>> ConnoteFee_ListBySingleOwnerID(int OwnerID)
        {
            try
            {
                wbm_common.DataObjects.ConnoteFee.dbRow ConnoteFee = await _dbContext.ConnoteFee.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.OwnerID == OwnerID);

                if (ConnoteFee == null)
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRow>>(new List<wbm_common.DataObjects.ConnoteFee.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRow>>(new List<wbm_common.DataObjects.ConnoteFee.dbRow> { ConnoteFee });
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_ListBySingleOwnerID - " + OwnerID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ConnoteFee.dbRow>> ConnoteFee_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ConnoteFee.dbRow ConnoteFee = await _dbContext.ConnoteFee.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ConnoteFee == null)
                    return Result.Failure<wbm_common.DataObjects.ConnoteFee.dbRow>("ConnoteFee not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ConnoteFee.dbRow>(ConnoteFee);

            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ConnoteFee.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRow>>> ConnoteFeeList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ConnoteFee.dbRow ConnoteFee = await _dbContext.ConnoteFee.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ConnoteFee == null)
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRow>>(new List<wbm_common.DataObjects.ConnoteFee.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRow>>(new List<wbm_common.DataObjects.ConnoteFee.dbRow> { ConnoteFee });
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ConnoteFee.dbRow>> ConnoteFee_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ConnoteFee.dbRow ConnoteFee = await _dbContext.ConnoteFee.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ConnoteFee == null)
                    return Result.Failure<wbm_common.DataObjects.ConnoteFee.dbRow>("ConnoteFee not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ConnoteFee.dbRow>(ConnoteFee);
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ConnoteFee.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRow>>> ConnoteFee_List()
        {
            try
            {
                List<wbm_common.DataObjects.ConnoteFee.dbRow> ConnoteFees = await _dbContext.ConnoteFee.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRow>>(ConnoteFees);
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRow>>> ConnoteFee_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ConnoteFee_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ConnoteFee.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ConnoteFee.dbRow> ConnoteFees = await _dbContext.ConnoteFee.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRow>>(ConnoteFees);
                }
                catch (Exception ex)
                {
                    logger.Error("ConnoteFee_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ConnoteFee.dbRow>>> ConnoteFee_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ConnoteFee.dbRow> ConnoteFees = await _dbContext.ConnoteFee.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ConnoteFee.dbRow>>(ConnoteFees);
            }
            catch (Exception ex)
            {
                logger.Error("ConnoteFee_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ConnoteFee.dbRow>>("Internal Exception occured");
            }
        }
    }
}
