using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> Owner_Insert(wbm_common.DataObjects.Owner.dbRow Owner)
        {
            try
            {
                await _dbContext.Owner.AddAsync(Owner);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Owner.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Owner_Insert - " + Owner.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Owner_Update(wbm_common.DataObjects.Owner.dbRow Owner)
        {
            try
            {
                _dbContext.Owner.Attach(Owner);
                _dbContext.Entry(Owner).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Owner.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Owner_Update - " + Owner.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.Owner.dbRow>>> Owner_ListBySingleCompanyID(int CompanyID)
        {
            try
            {
                wbm_common.DataObjects.Owner.dbRow Owner = await _dbContext.Owner.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.CompanyID == CompanyID);

                if (Owner == null)
                    return Result.Success<List<wbm_common.DataObjects.Owner.dbRow>>(new List<wbm_common.DataObjects.Owner.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Owner.dbRow>>(new List<wbm_common.DataObjects.Owner.dbRow> { Owner });
            }
            catch (Exception ex)
            {
                logger.Error("Owner_SearchByID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Owner.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.Owner.dbRow>> Owner_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Owner.dbRow Owner = await _dbContext.Owner.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Owner == null)
                    return Result.Failure<wbm_common.DataObjects.Owner.dbRow>("Owner not found : " + ID);
                else
                    return Result.Success<wbm_common.DataObjects.Owner.dbRow>(Owner);

            }
            catch (Exception ex)
            {
                logger.Error("Owner_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Owner.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.Owner.dbRow>>> OwnerList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Owner.dbRow Owner = await _dbContext.Owner.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Owner == null)
                    return Result.Success<List<wbm_common.DataObjects.Owner.dbRow>>(new List<wbm_common.DataObjects.Owner.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Owner.dbRow>>(new List<wbm_common.DataObjects.Owner.dbRow> { Owner });
            }
            catch (Exception ex)
            {
                logger.Error("Owner_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Owner.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Owner.dbRow>> Owner_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Owner.dbRow Owner = await _dbContext.Owner.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (Owner == null)
                    return Result.Failure<wbm_common.DataObjects.Owner.dbRow>("Owner not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Owner.dbRow>(Owner);
            }
            catch (Exception ex)
            {
                logger.Error("Owner_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Owner.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Owner.dbRow>>> Owner_List()
        {
            try
            {
                List<wbm_common.DataObjects.Owner.dbRow> Owners = await _dbContext.Owner.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Owner.dbRow>>(Owners);
            }
            catch (Exception ex)
            {
                logger.Error("Owner_List: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Owner.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Owner.dbRow>>> Owner_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Owner_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Owner.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Owner.dbRow> Owners = await _dbContext.Owner.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Owner.dbRow>>(Owners);
                }
                catch (Exception ex)
                {
                    logger.Error("Owner_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Owner.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Owner.dbRow>>> Owner_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Owner.dbRow> Owners = await _dbContext.Owner.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Owner.dbRow>>(Owners);
            }
            catch (Exception ex)
            {
                logger.Error("Owner_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Owner.dbRow>>("Internal Exception occured");
            }
        }
    }
}
