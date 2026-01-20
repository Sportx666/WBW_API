using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region ManhattenFiles
        public async Task<Result<int>> ManhattenFiles_Insert(wbm_common.DataObjects.ManhattenFiles.dbRow ManhattenFiles)
        {
            try
            {
                await _dbContext.ManhattenFiles.AddAsync(ManhattenFiles);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ManhattenFiles.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFiles_Insert - " + ManhattenFiles.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ManhattenFiles_Update(wbm_common.DataObjects.ManhattenFiles.dbRow ManhattenFiles)
        {
            try
            {
                _dbContext.ManhattenFiles.Attach(ManhattenFiles);
                _dbContext.Entry(ManhattenFiles).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ManhattenFiles.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFiles_Update - " + ManhattenFiles.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>> ManhattenFiles_ListBySingleCompanyID(int CompanyID)
        {
            try
            {
                wbm_common.DataObjects.ManhattenFiles.dbRow ManhattenFiles = await _dbContext.ManhattenFiles.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.CompanyID == CompanyID);

                if (ManhattenFiles == null)
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>(new List<wbm_common.DataObjects.ManhattenFiles.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>(new List<wbm_common.DataObjects.ManhattenFiles.dbRow> { ManhattenFiles });
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFiles_ListBySingleCompanyID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>> ManhattenFiles_ListBySingleSiteID(int SiteID)
        {
            try
            {
                wbm_common.DataObjects.ManhattenFiles.dbRow ManhattenFiles = await _dbContext.ManhattenFiles.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.SiteID == SiteID);

                if (ManhattenFiles == null)
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>(new List<wbm_common.DataObjects.ManhattenFiles.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>(new List<wbm_common.DataObjects.ManhattenFiles.dbRow> { ManhattenFiles });
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFiles_ListBySingleSiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ManhattenFiles.dbRow>> ManhattenFiles_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ManhattenFiles.dbRow ManhattenFiles = await _dbContext.ManhattenFiles.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ManhattenFiles == null)
                    return Result.Failure<wbm_common.DataObjects.ManhattenFiles.dbRow>("ManhattenFiles not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ManhattenFiles.dbRow>(ManhattenFiles);

            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFiles_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ManhattenFiles.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>> ManhattenFilesList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ManhattenFiles.dbRow ManhattenFiles = await _dbContext.ManhattenFiles.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ManhattenFiles == null)
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>(new List<wbm_common.DataObjects.ManhattenFiles.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>(new List<wbm_common.DataObjects.ManhattenFiles.dbRow> { ManhattenFiles });
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFiles_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ManhattenFiles.dbRow>> ManhattenFiles_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ManhattenFiles.dbRow ManhattenFiles = await _dbContext.ManhattenFiles.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ManhattenFiles == null)
                    return Result.Failure<wbm_common.DataObjects.ManhattenFiles.dbRow>("ManhattenFiles not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ManhattenFiles.dbRow>(ManhattenFiles);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFiles_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ManhattenFiles.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>> ManhattenFiles_List()
        {
            try
            {
                List<wbm_common.DataObjects.ManhattenFiles.dbRow> ManhattenFiless = await _dbContext.ManhattenFiles.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>(ManhattenFiless);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFiles_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>> ManhattenFiles_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ManhattenFiles_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ManhattenFiles.dbRow> ManhattenFiless = await _dbContext.ManhattenFiles.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>(ManhattenFiless);
                }
                catch (Exception ex)
                {
                    logger.Error("ManhattenFiles_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>> ManhattenFiles_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ManhattenFiles.dbRow> ManhattenFiless = await _dbContext.ManhattenFiles.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>(ManhattenFiless);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFiles_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ManhattenFiles.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region ManhattenFileType
        public async Task<Result<int>> ManhattenFileType_Insert(wbm_common.DataObjects.ManhattenFileType.dbRow ManhattenFileType)
        {
            try
            {
                await _dbContext.ManhattenFileType.AddAsync(ManhattenFileType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ManhattenFileType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFileType_Insert - " + ManhattenFileType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ManhattenFileType_Update(wbm_common.DataObjects.ManhattenFileType.dbRow ManhattenFileType)
        {
            try
            {
                _dbContext.ManhattenFileType.Attach(ManhattenFileType);
                _dbContext.Entry(ManhattenFileType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ManhattenFileType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFileType_Update - " + ManhattenFileType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ManhattenFileType.dbRow>> ManhattenFileType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ManhattenFileType.dbRow ManhattenFileType = await _dbContext.ManhattenFileType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ManhattenFileType == null)
                    return Result.Failure<wbm_common.DataObjects.ManhattenFileType.dbRow>("ManhattenFileType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ManhattenFileType.dbRow>(ManhattenFileType);

            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFileType_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ManhattenFileType.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>> ManhattenFileTypeList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ManhattenFileType.dbRow ManhattenFileType = await _dbContext.ManhattenFileType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ManhattenFileType == null)
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>(new List<wbm_common.DataObjects.ManhattenFileType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>(new List<wbm_common.DataObjects.ManhattenFileType.dbRow> { ManhattenFileType });
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFileType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ManhattenFileType.dbRow>> ManhattenFileType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ManhattenFileType.dbRow ManhattenFileType = await _dbContext.ManhattenFileType.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ManhattenFileType == null)
                    return Result.Failure<wbm_common.DataObjects.ManhattenFileType.dbRow>("ManhattenFileType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ManhattenFileType.dbRow>(ManhattenFileType);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFileType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ManhattenFileType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>> ManhattenFileType_List()
        {
            try
            {
                List<wbm_common.DataObjects.ManhattenFileType.dbRow> ManhattenFileTypes = await _dbContext.ManhattenFileType.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>(ManhattenFileTypes);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFileType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>> ManhattenFileType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ManhattenFileType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ManhattenFileType.dbRow> ManhattenFileTypes = await _dbContext.ManhattenFileType.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>(ManhattenFileTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("ManhattenFileType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>> ManhattenFileType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ManhattenFileType.dbRow> ManhattenFileTypes = await _dbContext.ManhattenFileType.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>(ManhattenFileTypes);
            }
            catch (Exception ex)
            {
                logger.Error("ManhattenFileType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ManhattenFileType.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
