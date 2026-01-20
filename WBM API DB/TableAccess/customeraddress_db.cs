using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region CustomerAddress
        public async Task<Result<int>> CustomerAddress_Insert(wbm_common.DataObjects.CustomerAddress.dbRow CustomerAddress)
        {
            try
            {
                await _dbContext.CustomerAddress.AddAsync(CustomerAddress);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(CustomerAddress.ID);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_Insert - " + CustomerAddress.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> CustomerAddress_Update(wbm_common.DataObjects.CustomerAddress.dbRow CustomerAddress)
        {
            try
            {
                _dbContext.CustomerAddress.Attach(CustomerAddress);
                _dbContext.Entry(CustomerAddress).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(CustomerAddress.ID);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_Update - " + CustomerAddress.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.CustomerAddress.dbRow>>> CustomerAddress_ListByPaperlessKeyID(string PaperlessKeyID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddress.dbRow CustomerAddress = await _dbContext.CustomerAddress.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.Paperless_KeyID == PaperlessKeyID);

                if (CustomerAddress == null)
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRow>>(new List<wbm_common.DataObjects.CustomerAddress.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRow>>(new List<wbm_common.DataObjects.CustomerAddress.dbRow> { CustomerAddress });
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_ListBySingleOwnerID - " + PaperlessKeyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddress.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.CustomerAddress.dbRow>> CustomerAddress_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddress.dbRow CustomerAddress = await _dbContext.CustomerAddress.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (CustomerAddress == null)
                    return Result.Failure<wbm_common.DataObjects.CustomerAddress.dbRow>("CustomerAddress not found : " + ID);
                else
                    return Result.Success<wbm_common.DataObjects.CustomerAddress.dbRow>(CustomerAddress);

            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.CustomerAddress.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.CustomerAddress.dbRow>>> CustomerAddressList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddress.dbRow CustomerAddress = await _dbContext.CustomerAddress.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (CustomerAddress == null)
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRow>>(new List<wbm_common.DataObjects.CustomerAddress.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRow>>(new List<wbm_common.DataObjects.CustomerAddress.dbRow> { CustomerAddress });
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddress.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.CustomerAddress.dbRow>> CustomerAddress_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddress.dbRow CustomerAddress = await _dbContext.CustomerAddress.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (CustomerAddress == null)
                    return Result.Failure<wbm_common.DataObjects.CustomerAddress.dbRow>("CustomerAddress not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.CustomerAddress.dbRow>(CustomerAddress);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.CustomerAddress.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddress.dbRow>>> CustomerAddress_List()
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddress.dbRow> CustomerAddresss = await _dbContext.CustomerAddress.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRow>>(CustomerAddresss);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.CustomerAddress.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddress.dbRow>>> CustomerAddress_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddress.dbRow> CustomerAddresss = await _dbContext.CustomerAddress.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.CustomerAddress.dbRow>>(CustomerAddresss);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddress_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddress.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region CustomerAddressDefault
        public async Task<Result<int>> CustomerAddressDefault_Insert(wbm_common.DataObjects.CustomerAddressDefault.dbRow CustomerAddressDefault)
        {
            try
            {
                await _dbContext.CustomerAddressDefault.AddAsync(CustomerAddressDefault);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(CustomerAddressDefault.ID);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_Insert - " + CustomerAddressDefault.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> CustomerAddressDefault_Update(wbm_common.DataObjects.CustomerAddressDefault.dbRow CustomerAddressDefault)
        {
            try
            {
                _dbContext.CustomerAddressDefault.Attach(CustomerAddressDefault);
                _dbContext.Entry(CustomerAddressDefault).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(CustomerAddressDefault.ID);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_Update - " + CustomerAddressDefault.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.CustomerAddressDefault.dbRow>> CustomerAddressDefault_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddressDefault.dbRow CustomerAddressDefault = await _dbContext.CustomerAddressDefault.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (CustomerAddressDefault == null)
                    return Result.Failure<wbm_common.DataObjects.CustomerAddressDefault.dbRow>("CustomerAddressDefault not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.CustomerAddressDefault.dbRow>(CustomerAddressDefault);

            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.CustomerAddressDefault.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>> CustomerAddressDefault_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddressDefault.dbRow CustomerAddressDefault = await _dbContext.CustomerAddressDefault.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (CustomerAddressDefault == null)
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>(new List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>(new List<wbm_common.DataObjects.CustomerAddressDefault.dbRow> { CustomerAddressDefault });
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.CustomerAddressDefault.dbRow>> CustomerAddressDefault_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.CustomerAddressDefault.dbRow CustomerAddressDefault = await _dbContext.CustomerAddressDefault.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (CustomerAddressDefault == null)
                    return Result.Failure<wbm_common.DataObjects.CustomerAddressDefault.dbRow>("CustomerAddressDefault not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.CustomerAddressDefault.dbRow>(CustomerAddressDefault);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.CustomerAddressDefault.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>> CustomerAddressDefault_List()
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddressDefault.dbRow> CustomerAddressDefaults = await _dbContext.CustomerAddressDefault.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>(CustomerAddressDefaults);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>> CustomerAddressDefault_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("CustomerAddressDefault_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.CustomerAddressDefault.dbRow> CustomerAddressDefaults = await _dbContext.CustomerAddressDefault.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>(CustomerAddressDefaults);
                }
                catch (Exception ex)
                {
                    logger.Error("CustomerAddressDefault_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>> CustomerAddressDefault_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.CustomerAddressDefault.dbRow> CustomerAddressDefaults = await _dbContext.CustomerAddressDefault.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>(CustomerAddressDefaults);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerAddressDefault_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
