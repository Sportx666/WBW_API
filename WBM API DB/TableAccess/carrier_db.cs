using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> Carrier_Insert(wbm_common.DataObjects.Carrier.dbRow carrier)
        {
            try
            {
                await _dbContext.Carrier.AddAsync(carrier);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(carrier.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_Insert - " + carrier.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Carrier_Update(wbm_common.DataObjects.Carrier.dbRow carrier)
        {
            try
            {
                _dbContext.Carrier.Attach(carrier);
                _dbContext.Entry(carrier).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(carrier.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_Update - " + carrier.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRow>>> Carrier_ListByPaperlessKeyID(string PaperlessKeyID)
        {
            try
            {
                wbm_common.DataObjects.Carrier.dbRow carrier = await _dbContext.Carrier.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.Paperless_KeyID == PaperlessKeyID);

                if (carrier == null)
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRow>>(new List<wbm_common.DataObjects.Carrier.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRow>>(new List<wbm_common.DataObjects.Carrier.dbRow> { carrier });
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_ListByPaperlessKeyID - " + PaperlessKeyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.Carrier.dbRow>> Carrier_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.Carrier.dbRow carrier = await _dbContext.Carrier.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (carrier == null)
                    return Result.Failure<wbm_common.DataObjects.Carrier.dbRow>("Carrier not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.Carrier.dbRow>(carrier);

            }
            catch (Exception ex)
            {
                logger.Error("Carrier_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Carrier.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRow>>> Carrier_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Carrier.dbRow carrier = await _dbContext.Carrier.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (carrier == null)
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRow>>(new List<wbm_common.DataObjects.Carrier.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRow>>(new List<wbm_common.DataObjects.Carrier.dbRow> { carrier });
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Carrier.dbRow>> Carrier_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Carrier.dbRow carrier = await _dbContext.Carrier.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (carrier == null)
                    return Result.Failure<wbm_common.DataObjects.Carrier.dbRow>("Carrier not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Carrier.dbRow>(carrier);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Carrier.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRow>>> Carrier_List()
        {
            try
            {
                List<wbm_common.DataObjects.Carrier.dbRow> carriers = await _dbContext.Carrier.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Carrier.dbRow>>(carriers);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRow>>> Carrier_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Carrier_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Carrier.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Carrier.dbRow> carriers = await _dbContext.Carrier.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Carrier.dbRow>>(carriers);
                }
                catch (Exception ex)
                {
                    logger.Error("Carrier_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Carrier.dbRow>>> Carrier_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Carrier.dbRow> carriers = await _dbContext.Carrier.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Carrier.dbRow>>(carriers);
            }
            catch (Exception ex)
            {
                logger.Error("Carrier_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Carrier.dbRow>>("Internal Exception occured");
            }
        }
    }
}
