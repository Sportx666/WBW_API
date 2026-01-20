using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> NoteType_Insert(wbm_common.DataObjects.NoteType.dbRow NoteType)
        {
            try
            {
                await _dbContext.NoteType.AddAsync(NoteType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(NoteType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("NoteType_Insert - " + NoteType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> NoteType_Update(wbm_common.DataObjects.NoteType.dbRow NoteType)
        {
            try
            {
                _dbContext.NoteType.Attach(NoteType);
                _dbContext.Entry(NoteType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(NoteType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("NoteType_Update - " + NoteType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.NoteType.dbRow>> NoteType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.NoteType.dbRow NoteType = await _dbContext.NoteType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (NoteType == null)
                    return Result.Failure<wbm_common.DataObjects.NoteType.dbRow>("NoteType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.NoteType.dbRow>(NoteType);

            }
            catch (Exception ex)
            {
                logger.Error("NoteType_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.NoteType.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.NoteType.dbRow>>> NoteTypeList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.NoteType.dbRow NoteType = await _dbContext.NoteType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (NoteType == null)
                    return Result.Success<List<wbm_common.DataObjects.NoteType.dbRow>>(new List<wbm_common.DataObjects.NoteType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.NoteType.dbRow>>(new List<wbm_common.DataObjects.NoteType.dbRow> { NoteType });
            }
            catch (Exception ex)
            {
                logger.Error("NoteType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.NoteType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.NoteType.dbRow>> NoteType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.NoteType.dbRow NoteType = await _dbContext.NoteType.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (NoteType == null)
                    return Result.Failure<wbm_common.DataObjects.NoteType.dbRow>("NoteType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.NoteType.dbRow>(NoteType);
            }
            catch (Exception ex)
            {
                logger.Error("NoteType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.NoteType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.NoteType.dbRow>>> NoteType_List()
        {
            try
            {
                List<wbm_common.DataObjects.NoteType.dbRow> NoteTypes = await _dbContext.NoteType.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.NoteType.dbRow>>(NoteTypes);
            }
            catch (Exception ex)
            {
                logger.Error("NoteType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.NoteType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.NoteType.dbRow>>> NoteType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("NoteType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.NoteType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.NoteType.dbRow> NoteTypes = await _dbContext.NoteType.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.NoteType.dbRow>>(NoteTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("NoteType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.NoteType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.NoteType.dbRow>>> NoteType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.NoteType.dbRow> NoteTypes = await _dbContext.NoteType.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.NoteType.dbRow>>(NoteTypes);
            }
            catch (Exception ex)
            {
                logger.Error("NoteType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.NoteType.dbRow>>("Internal Exception occured");
            }
        }
    }
}
