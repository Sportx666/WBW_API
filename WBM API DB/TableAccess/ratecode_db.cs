using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region RateCode
        public async Task<Result<int>> RateCode_Insert(wbm_common.DataObjects.RateCode.dbRow RateCode)
        {
            try
            {
                await _dbContext.RateCode.AddAsync(RateCode);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCode.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_Insert - " + RateCode.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> RateCode_Update(wbm_common.DataObjects.RateCode.dbRow RateCode)
        {
            try
            {
                _dbContext.RateCode.Attach(RateCode);
                _dbContext.Entry(RateCode).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCode.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_Update - " + RateCode.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.RateCode.dbRow>> RateCode_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCode.dbRow RateCode = await _dbContext.RateCode.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateCode == null)
                    return Result.Failure<wbm_common.DataObjects.RateCode.dbRow>("RateCode not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCode.dbRow>(RateCode);

            }
            catch (Exception ex)
            {
                logger.Error("RateCode_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCode.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.RateCode.dbRow>>> RateCodeList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCode.dbRow RateCode = await _dbContext.RateCode.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateCode == null)
                    return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(new List<wbm_common.DataObjects.RateCode.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(new List<wbm_common.DataObjects.RateCode.dbRow> { RateCode });
            }
            catch (Exception ex)
            {
                logger.Error("RateCodeList_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCode.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateCode.dbRow>> RateCode_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCode.dbRow RateCode = await _dbContext.RateCode.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (RateCode == null)
                    return Result.Failure<wbm_common.DataObjects.RateCode.dbRow>("RateCode not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCode.dbRow>(RateCode);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCode.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCode.dbRow>>> RateCode_List()
        {
            try
            {
                List<wbm_common.DataObjects.RateCode.dbRow> RateCodes = await _dbContext.RateCode.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(RateCodes);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.RateCode.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCode.dbRow>>> RateCode_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("RateCode_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.RateCode.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.RateCode.dbRow> RateCodes = await _dbContext.RateCode.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(RateCodes);
                }
                catch (Exception ex)
                {
                    logger.Error("RateCode_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.RateCode.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCode.dbRow>>> RateCode_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.RateCode.dbRow> RateCodes = await _dbContext.RateCode.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.RateCode.dbRow>>(RateCodes);
            }
            catch (Exception ex)
            {
                logger.Error("RateCode_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCode.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region RateCollection
        public async Task<Result<int>> RateCollection_Insert(wbm_common.DataObjects.RateCollection.dbRow RateCollection)
        {
            try
            {
                await _dbContext.RateCollection.AddAsync(RateCollection);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCollection.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_Insert - " + RateCollection.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> RateCollection_Update(wbm_common.DataObjects.RateCollection.dbRow RateCollection)
        {
            try
            {
                _dbContext.RateCollection.Attach(RateCollection);
                _dbContext.Entry(RateCollection).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCollection.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_Update - " + RateCollection.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRow>>> RateCollection_ListBySingleCompanyID(int CompanyID)
        {
            try
            {
                List<wbm_common.DataObjects.RateCollection.dbRow> RateCollection = await _dbContext.RateCollection.AsNoTracking().Where(x => !x.DeletedFlag && x.CompanyID == CompanyID).ToListAsync();

                if (RateCollection == null)
                    return Result.Failure<List<wbm_common.DataObjects.RateCollection.dbRow>>("RateCollection not found : " + CompanyID.ToString());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRow>>(RateCollection);

            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_ListBySingleCompanyID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.RateCollection.dbRow>> RateCollection_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCollection.dbRow RateCollection = await _dbContext.RateCollection.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateCollection == null)
                    return Result.Failure<wbm_common.DataObjects.RateCollection.dbRow>("RateCollection not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCollection.dbRow>(RateCollection);

            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCollection.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRow>>> RateCollectionList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCollection.dbRow RateCollection = await _dbContext.RateCollection.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateCollection == null)
                    return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRow>>(new List<wbm_common.DataObjects.RateCollection.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRow>>(new List<wbm_common.DataObjects.RateCollection.dbRow> { RateCollection });
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionList_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateCollection.dbRow>> RateCollection_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCollection.dbRow RateCollection = await _dbContext.RateCollection.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (RateCollection == null)
                    return Result.Failure<wbm_common.DataObjects.RateCollection.dbRow>("RateCollection not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCollection.dbRow>(RateCollection);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCollection.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRow>>> RateCollection_List()
        {
            try
            {
                List<wbm_common.DataObjects.RateCollection.dbRow> RateCollections = await _dbContext.RateCollection.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRow>>(RateCollections);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRow>>> RateCollection_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("RateCollection_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.RateCollection.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.RateCollection.dbRow> RateCollections = await _dbContext.RateCollection.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRow>>(RateCollections);
                }
                catch (Exception ex)
                {
                    logger.Error("RateCollection_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollection.dbRow>>> RateCollection_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.RateCollection.dbRow> RateCollections = await _dbContext.RateCollection.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.RateCollection.dbRow>>(RateCollections);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollection_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollection.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region RateCollectionDefn
        public async Task<Result<int>> RateCollectionDefn_Insert(wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefn)
        {
            try
            {
                await _dbContext.RateCollectionDefn.AddAsync(RateCollectionDefn);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCollectionDefn.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_Insert - " + RateCollectionDefn.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> RateCollectionDefn_Update(wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefn)
        {
            try
            {
                _dbContext.RateCollectionDefn.Attach(RateCollectionDefn);
                _dbContext.Entry(RateCollectionDefn).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateCollectionDefn.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_Update - " + RateCollectionDefn.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateCollectionDefn.dbRow>> RateCollectionDefn_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefn = await _dbContext.RateCollectionDefn.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateCollectionDefn == null)
                    return Result.Failure<wbm_common.DataObjects.RateCollectionDefn.dbRow>("RateCollectionDefn not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCollectionDefn.dbRow>(RateCollectionDefn);

            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCollectionDefn.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>> RateCollectionDefnList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefn = await _dbContext.RateCollectionDefn.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateCollectionDefn == null)
                    return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>(new List<wbm_common.DataObjects.RateCollectionDefn.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>(new List<wbm_common.DataObjects.RateCollectionDefn.dbRow> { RateCollectionDefn });
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateCollectionDefn.dbRow>> RateCollectionDefn_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateCollectionDefn.dbRow RateCollectionDefn = await _dbContext.RateCollectionDefn.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (RateCollectionDefn == null)
                    return Result.Failure<wbm_common.DataObjects.RateCollectionDefn.dbRow>("RateCollectionDefn not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateCollectionDefn.dbRow>(RateCollectionDefn);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateCollectionDefn.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>> RateCollectionDefn_List()
        {
            try
            {
                List<wbm_common.DataObjects.RateCollectionDefn.dbRow> RateCollectionDefns = await _dbContext.RateCollectionDefn.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>(RateCollectionDefns);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>> RateCollectionDefn_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("RateCollectionDefn_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.RateCollectionDefn.dbRow> RateCollectionDefns = await _dbContext.RateCollectionDefn.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>(RateCollectionDefns);
                }
                catch (Exception ex)
                {
                    logger.Error("RateCollectionDefn_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>> RateCollectionDefn_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.RateCollectionDefn.dbRow> RateCollectionDefns = await _dbContext.RateCollectionDefn.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>(RateCollectionDefns);
            }
            catch (Exception ex)
            {
                logger.Error("RateCollectionDefn_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region RateFunction
        public async Task<Result<int>> RateFunction_Insert(wbm_common.DataObjects.RateFunction.dbRow RateFunction)
        {
            try
            {
                await _dbContext.RateFunction.AddAsync(RateFunction);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateFunction.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_Insert - " + RateFunction.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> RateFunction_Update(wbm_common.DataObjects.RateFunction.dbRow RateFunction)
        {
            try
            {
                _dbContext.RateFunction.Attach(RateFunction);
                _dbContext.Entry(RateFunction).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(RateFunction.ID);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_Update - " + RateFunction.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateFunction.dbRow>> RateFunction_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateFunction.dbRow RateFunction = await _dbContext.RateFunction.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateFunction == null)
                    return Result.Failure<wbm_common.DataObjects.RateFunction.dbRow>("RateFunction not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateFunction.dbRow>(RateFunction);

            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateFunction.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.RateFunction.dbRow>>> RateFunctionList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateFunction.dbRow RateFunction = await _dbContext.RateFunction.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (RateFunction == null)
                    return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(new List<wbm_common.DataObjects.RateFunction.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(new List<wbm_common.DataObjects.RateFunction.dbRow> { RateFunction });
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateFunction.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.RateFunction.dbRow>> RateFunction_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.RateFunction.dbRow RateFunction = await _dbContext.RateFunction.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (RateFunction == null)
                    return Result.Failure<wbm_common.DataObjects.RateFunction.dbRow>("RateFunction not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.RateFunction.dbRow>(RateFunction);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.RateFunction.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateFunction.dbRow>>> RateFunction_List()
        {
            try
            {
                List<wbm_common.DataObjects.RateFunction.dbRow> RateFunctions = await _dbContext.RateFunction.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(RateFunctions);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.RateFunction.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateFunction.dbRow>>> RateFunction_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("RateFunction_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.RateFunction.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.RateFunction.dbRow> RateFunctions = await _dbContext.RateFunction.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(RateFunctions);
                }
                catch (Exception ex)
                {
                    logger.Error("RateFunction_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.RateFunction.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.RateFunction.dbRow>>> RateFunction_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.RateFunction.dbRow> RateFunctions = await _dbContext.RateFunction.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.RateFunction.dbRow>>(RateFunctions);
            }
            catch (Exception ex)
            {
                logger.Error("RateFunction_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.RateFunction.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region TableRate
        public async Task<Result<int>> TableRate_Insert(wbm_common.DataObjects.TableRate.dbRow TableRate)
        {
            try
            {
                await _dbContext.TableRate.AddAsync(TableRate);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TableRate.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_Insert - " + TableRate.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TableRate_Update(wbm_common.DataObjects.TableRate.dbRow TableRate)
        {
            try
            {
                _dbContext.TableRate.Attach(TableRate);
                _dbContext.Entry(TableRate).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TableRate.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_Update - " + TableRate.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TableRate.dbRow>> TableRate_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TableRate.dbRow TableRate = await _dbContext.TableRate.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TableRate == null)
                    return Result.Failure<wbm_common.DataObjects.TableRate.dbRow>("TableRate not found : " + ID);
                else
                    return Result.Success<wbm_common.DataObjects.TableRate.dbRow>(TableRate);

            }
            catch (Exception ex)
            {
                logger.Error("TableRate_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TableRate.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRow>>> TableRateList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TableRate.dbRow TableRate = await _dbContext.TableRate.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TableRate == null)
                    return Result.Success<List<wbm_common.DataObjects.TableRate.dbRow>>(new List<wbm_common.DataObjects.TableRate.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TableRate.dbRow>>(new List<wbm_common.DataObjects.TableRate.dbRow> { TableRate });
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TableRate.dbRow>> TableRate_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TableRate.dbRow TableRate = await _dbContext.TableRate.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (TableRate == null)
                    return Result.Failure<wbm_common.DataObjects.TableRate.dbRow>("TableRate not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TableRate.dbRow>(TableRate);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TableRate.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRow>>> TableRate_List()
        {
            try
            {
                List<wbm_common.DataObjects.TableRate.dbRow> TableRates = await _dbContext.TableRate.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TableRate.dbRow>>(TableRates);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRow>>> TableRate_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TableRate_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TableRate.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TableRate.dbRow> TableRates = await _dbContext.TableRate.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TableRate.dbRow>>(TableRates);
                }
                catch (Exception ex)
                {
                    logger.Error("TableRate_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TableRate.dbRow>>> TableRate_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TableRate.dbRow> TableRates = await _dbContext.TableRate.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TableRate.dbRow>>(TableRates);
            }
            catch (Exception ex)
            {
                logger.Error("TableRate_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TableRate.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
