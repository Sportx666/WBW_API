using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> Company_Insert(wbm_common.DataObjects.Company.dbRow Company)
        {
            try
            {
                await _dbContext.Company.AddAsync(Company);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Company.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Company_Insert - " + Company.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Company_Update(wbm_common.DataObjects.Company.dbRow Company)
        {
            try
            {
                _dbContext.Company.Attach(Company);
                _dbContext.Entry(Company).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Company.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Company_Update - " + Company.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<wbm_common.DataObjects.Company.dbRow>> Company_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Company.dbRow Company = await _dbContext.Company.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Company == null)
                    return Result.Failure<wbm_common.DataObjects.Company.dbRow>("Company not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Company.dbRow>(Company);

            }
            catch (Exception ex)
            {
                logger.Error("Company_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Company.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.Company.dbRow>>> CompanyList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Company.dbRow Company = await _dbContext.Company.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Company == null)
                    return Result.Success<List<wbm_common.DataObjects.Company.dbRow>>(new List<wbm_common.DataObjects.Company.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Company.dbRow>>(new List<wbm_common.DataObjects.Company.dbRow> { Company });
            }
            catch (Exception ex)
            {
                logger.Error("Company_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Company.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Company.dbRow>> Company_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Company.dbRow Company = await _dbContext.Company.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (Company == null)
                    return Result.Failure<wbm_common.DataObjects.Company.dbRow>("Company not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Company.dbRow>(Company);
            }
            catch (Exception ex)
            {
                logger.Error("Company_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Company.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Company.dbRow>>> Company_List()
        {
            try
            {
                List<wbm_common.DataObjects.Company.dbRow> Companys = await _dbContext.Company.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Company.dbRow>>(Companys);
            }
            catch (Exception ex)
            {
                logger.Error("Company_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Company.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Company.dbRow>>> Company_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Company_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Company.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Company.dbRow> Companys = await _dbContext.Company.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Company.dbRow>>(Companys);
                }
                catch (Exception ex)
                {
                    logger.Error("Company_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Company.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Company.dbRow>>> Company_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Company.dbRow> Companys = await _dbContext.Company.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Company.dbRow>>(Companys);
            }
            catch (Exception ex)
            {
                logger.Error("Company_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Company.dbRow>>("Internal Exception occured");
            }
        }
    }
}
