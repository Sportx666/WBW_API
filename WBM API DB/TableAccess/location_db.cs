using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> Location_Insert(wbm_common.DataObjects.Location.dbRow Location)
        {
            try
            {
                await _dbContext.Location.AddAsync(Location);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Location.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Location_Insert - " + Location.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Location_Update(wbm_common.DataObjects.Location.dbRow Location)
        {
            try
            {
                _dbContext.Location.Attach(Location);
                _dbContext.Entry(Location).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Location.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Location_Update - " + Location.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.Location.dbRow>>> Location_ListByPaperlessKeyID(string PaperlessKeyID)
        {
            try
            {
                wbm_common.DataObjects.Location.dbRow Location = await _dbContext.Location.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.Paperless_KeyID == PaperlessKeyID);

                if (Location == null)
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(new List<wbm_common.DataObjects.Location.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(new List<wbm_common.DataObjects.Location.dbRow> { Location });
            }
            catch (Exception ex)
            {
                logger.Error("Location_ListByPaperlessKeyID - " + PaperlessKeyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRow>>> Location_ListByCompanyID(int CompanyID)
        {
            try
            {
                wbm_common.DataObjects.Location.dbRow Location = await _dbContext.Location.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.CompanyID == CompanyID);

                if (Location == null)
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(new List<wbm_common.DataObjects.Location.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(new List<wbm_common.DataObjects.Location.dbRow> { Location });
            }
            catch (Exception ex)
            {
                logger.Error("Location_ListByCompanyID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRow>>> Location_ListBySiteID(int SiteID)
        {
            try
            {
                wbm_common.DataObjects.Location.dbRow Location = await _dbContext.Location.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.SiteID == SiteID);

                if (Location == null)
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(new List<wbm_common.DataObjects.Location.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(new List<wbm_common.DataObjects.Location.dbRow> { Location });
            }
            catch (Exception ex)
            {
                logger.Error("Location_ListBySiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.Location.dbRow>> Location_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Location.dbRow Location = await _dbContext.Location.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Location == null)
                    return Result.Failure<wbm_common.DataObjects.Location.dbRow>("Location not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Location.dbRow>(Location);

            }
            catch (Exception ex)
            {
                logger.Error("Location_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Location.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.Location.dbRow>>> LocationList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Location.dbRow Location = await _dbContext.Location.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Location == null)
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(new List<wbm_common.DataObjects.Location.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(new List<wbm_common.DataObjects.Location.dbRow> { Location });
            }
            catch (Exception ex)
            {
                logger.Error("Location_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Location.dbRow>> Location_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Location.dbRow Location = await _dbContext.Location.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (Location == null)
                    return Result.Failure<wbm_common.DataObjects.Location.dbRow>("Location not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Location.dbRow>(Location);
            }
            catch (Exception ex)
            {
                logger.Error("Location_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Location.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRow>>> Location_List()
        {
            try
            {
                List<wbm_common.DataObjects.Location.dbRow> Locations = await _dbContext.Location.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(Locations);
            }
            catch (Exception ex)
            {
                logger.Error("Location_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Location.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRow>>> Location_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Location_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Location.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Location.dbRow> Locations = await _dbContext.Location.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(Locations);
                }
                catch (Exception ex)
                {
                    logger.Error("Location_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Location.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Location.dbRow>>> Location_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Location.dbRow> Locations = await _dbContext.Location.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Location.dbRow>>(Locations);
            }
            catch (Exception ex)
            {
                logger.Error("Location_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Location.dbRow>>("Internal Exception occured");
            }
        }
    }
}
