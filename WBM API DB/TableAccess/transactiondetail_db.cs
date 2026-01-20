using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region TransactionDetail
        public async Task<Result<int>> TransactionDetail_Insert(wbm_common.DataObjects.TransactionDetail.dbRow TransactionDetail)
        {
            try
            {
                await _dbContext.TransactionDetail.AddAsync(TransactionDetail);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionDetail.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_Insert - " + TransactionDetail.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionDetail_Update(wbm_common.DataObjects.TransactionDetail.dbRow TransactionDetail)
        {
            try
            {
                _dbContext.TransactionDetail.Attach(TransactionDetail);
                _dbContext.Entry(TransactionDetail).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionDetail.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_Update - " + TransactionDetail.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRow>>> TransactionDetail_ListByListTransactionHeaderIDs(List<int> TransactionHeaderIDs)
        {
            if (TransactionHeaderIDs == null || TransactionHeaderIDs.Count == 0)
            {
                logger.Error("TransactionDetail_ListByListTransactionHeaderIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionDetail.dbRow> TransactionDetails = await _dbContext.TransactionDetail.AsNoTracking().Where(x => !x.DeletedFlag && TransactionHeaderIDs.Contains(x.TransactionHeaderID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRow>>(TransactionDetails);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionDetail_ListByListTransactionHeaderIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("Internal Exception occured");
                }
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionDetail.dbRow>> TransactionDetail_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionDetail.dbRow TransactionDetail = await _dbContext.TransactionDetail.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionDetail == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionDetail.dbRow>("TransactionDetail not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionDetail.dbRow>(TransactionDetail);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionDetail.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRow>>> TransactionDetailList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionDetail.dbRow TransactionDetail = await _dbContext.TransactionDetail.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionDetail == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRow>>(new List<wbm_common.DataObjects.TransactionDetail.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRow>>(new List<wbm_common.DataObjects.TransactionDetail.dbRow> { TransactionDetail });
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionDetail.dbRow>> TransactionDetail_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionDetail.dbRow TransactionDetail = await _dbContext.TransactionDetail.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (TransactionDetail == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionDetail.dbRow>("TransactionDetail not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionDetail.dbRow>(TransactionDetail);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionDetail.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRow>>> TransactionDetail_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionDetail.dbRow> TransactionDetails = await _dbContext.TransactionDetail.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRow>>(TransactionDetails);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRow>>> TransactionDetail_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionDetail_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionDetail.dbRow> TransactionDetails = await _dbContext.TransactionDetail.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRow>>(TransactionDetails);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionDetail_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionDetail.dbRow>>> TransactionDetail_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionDetail.dbRow> TransactionDetails = await _dbContext.TransactionDetail.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionDetail.dbRow>>(TransactionDetails);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionDetail_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionDetail.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region TransactionException
        public async Task<Result<int>> TransactionException_Insert(wbm_common.DataObjects.TransactionException.dbRow TransactionException)
        {
            try
            {
                await _dbContext.TransactionException.AddAsync(TransactionException);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionException.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_Insert - " + TransactionException.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionException_Update(wbm_common.DataObjects.TransactionException.dbRow TransactionException)
        {
            try
            {
                _dbContext.TransactionException.Attach(TransactionException);
                _dbContext.Entry(TransactionException).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionException.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_Update - " + TransactionException.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRow>>> TransactionException_ListByListTransactionHeaderIDs(List<int> TransactionHeaderIDs)
        {
            if (TransactionHeaderIDs == null || TransactionHeaderIDs.Count == 0)
            {
                logger.Error("TransactionException_ListByListTransactionHeaderIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionException.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionException.dbRow> TransactionExceptions = await _dbContext.TransactionException.AsNoTracking().Where(x => !x.DeletedFlag && TransactionHeaderIDs.Contains(x.TransactionHeaderID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRow>>(TransactionExceptions);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionException_ListByListTransactionHeaderIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRow>>("Internal Exception occured");
                }
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionException.dbRow>> TransactionException_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionException.dbRow TransactionException = await _dbContext.TransactionException.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionException == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionException.dbRow>("TransactionException not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionException.dbRow>(TransactionException);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionException.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRow>>> TransactionExceptionList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionException.dbRow TransactionException = await _dbContext.TransactionException.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionException == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRow>>(new List<wbm_common.DataObjects.TransactionException.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRow>>(new List<wbm_common.DataObjects.TransactionException.dbRow> { TransactionException });
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionException.dbRow>> TransactionException_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionException.dbRow TransactionException = await _dbContext.TransactionException.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (TransactionException == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionException.dbRow>("TransactionException not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionException.dbRow>(TransactionException);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionException.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRow>>> TransactionException_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionException.dbRow> TransactionExceptions = await _dbContext.TransactionException.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRow>>(TransactionExceptions);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRow>>> TransactionException_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionException_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionException.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionException.dbRow> TransactionExceptions = await _dbContext.TransactionException.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRow>>(TransactionExceptions);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionException_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionException.dbRow>>> TransactionException_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionException.dbRow> TransactionExceptions = await _dbContext.TransactionException.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionException.dbRow>>(TransactionExceptions);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionException_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionException.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region TransactionExceptionType
        public async Task<Result<int>> TransactionExceptionType_Insert(wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType)
        {
            try
            {
                await _dbContext.TransactionExceptionType.AddAsync(TransactionExceptionType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionExceptionType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_Insert - " + TransactionExceptionType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionExceptionType_Update(wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType)
        {
            try
            {
                _dbContext.TransactionExceptionType.Attach(TransactionExceptionType);
                _dbContext.Entry(TransactionExceptionType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionExceptionType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_Update - " + TransactionExceptionType.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionExceptionType.dbRow>> TransactionExceptionType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType = await _dbContext.TransactionExceptionType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionExceptionType == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionExceptionType.dbRow>("TransactionExceptionType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionExceptionType.dbRow>(TransactionExceptionType);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionExceptionType.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>> TransactionExceptionTypeList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType = await _dbContext.TransactionExceptionType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionExceptionType == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(new List<wbm_common.DataObjects.TransactionExceptionType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(new List<wbm_common.DataObjects.TransactionExceptionType.dbRow> { TransactionExceptionType });
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionExceptionType.dbRow>> TransactionExceptionType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionExceptionType.dbRow TransactionExceptionType = await _dbContext.TransactionExceptionType.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (TransactionExceptionType == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionExceptionType.dbRow>("TransactionExceptionType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionExceptionType.dbRow>(TransactionExceptionType);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionExceptionType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>> TransactionExceptionType_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionExceptionType.dbRow> TransactionExceptionTypes = await _dbContext.TransactionExceptionType.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(TransactionExceptionTypes);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>> TransactionExceptionType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionExceptionType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionExceptionType.dbRow> TransactionExceptionTypes = await _dbContext.TransactionExceptionType.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(TransactionExceptionTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionExceptionType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>> TransactionExceptionType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionExceptionType.dbRow> TransactionExceptionTypes = await _dbContext.TransactionExceptionType.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>(TransactionExceptionTypes);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionExceptionType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region TransactionHeader
        public async Task<Result<int>> TransactionHeader_Insert(wbm_common.DataObjects.TransactionHeader.dbRow TransactionHeader)
        {
            try
            {
                await _dbContext.TransactionHeader.AddAsync(TransactionHeader);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_Insert - " + TransactionHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionHeader_Update(wbm_common.DataObjects.TransactionHeader.dbRow TransactionHeader)
        {
            try
            {
                _dbContext.TransactionHeader.Attach(TransactionHeader);
                _dbContext.Entry(TransactionHeader).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_Update - " + TransactionHeader.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionHeader.dbRow>> TransactionHeader_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionHeader.dbRow TransactionHeader = await _dbContext.TransactionHeader.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionHeader == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionHeader.dbRow>("TransactionHeader not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionHeader.dbRow>(TransactionHeader);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionHeader.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRow>>> TransactionHeaderList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionHeader.dbRow TransactionHeader = await _dbContext.TransactionHeader.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionHeader == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRow>>(new List<wbm_common.DataObjects.TransactionHeader.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRow>>(new List<wbm_common.DataObjects.TransactionHeader.dbRow> { TransactionHeader });
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionHeader.dbRow>> TransactionHeader_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionHeader.dbRow TransactionHeader = await _dbContext.TransactionHeader.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (TransactionHeader == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionHeader.dbRow>("TransactionHeader not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionHeader.dbRow>(TransactionHeader);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionHeader.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRow>>> TransactionHeader_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionHeader.dbRow> TransactionHeaders = await _dbContext.TransactionHeader.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRow>>(TransactionHeaders);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRow>>> TransactionHeader_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionHeader_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionHeader.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionHeader.dbRow> TransactionHeaders = await _dbContext.TransactionHeader.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRow>>(TransactionHeaders);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionHeader_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionHeader.dbRow>>> TransactionHeader_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionHeader.dbRow> TransactionHeaders = await _dbContext.TransactionHeader.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionHeader.dbRow>>(TransactionHeaders);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionHeader_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionHeader.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region TransactionSource
        public async Task<Result<int>> TransactionSource_Insert(wbm_common.DataObjects.TransactionSource.dbRow TransactionSource)
        {
            try
            {
                await _dbContext.TransactionSource.AddAsync(TransactionSource);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionSource.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_Insert - " + TransactionSource.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> TransactionSource_Update(wbm_common.DataObjects.TransactionSource.dbRow TransactionSource)
        {
            try
            {
                _dbContext.TransactionSource.Attach(TransactionSource);
                _dbContext.Entry(TransactionSource).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(TransactionSource.ID);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_Update - " + TransactionSource.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionSource.dbRow>> TransactionSource_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionSource.dbRow TransactionSource = await _dbContext.TransactionSource.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionSource == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionSource.dbRow>("TransactionSource not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionSource.dbRow>(TransactionSource);

            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionSource.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.TransactionSource.dbRow>>> TransactionSourceList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionSource.dbRow TransactionSource = await _dbContext.TransactionSource.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (TransactionSource == null)
                    return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(new List<wbm_common.DataObjects.TransactionSource.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(new List<wbm_common.DataObjects.TransactionSource.dbRow> { TransactionSource });
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionSource.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.TransactionSource.dbRow>> TransactionSource_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.TransactionSource.dbRow TransactionSource = await _dbContext.TransactionSource.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (TransactionSource == null)
                    return Result.Failure<wbm_common.DataObjects.TransactionSource.dbRow>("TransactionSource not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.TransactionSource.dbRow>(TransactionSource);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.TransactionSource.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionSource.dbRow>>> TransactionSource_List()
        {
            try
            {
                List<wbm_common.DataObjects.TransactionSource.dbRow> TransactionSources = await _dbContext.TransactionSource.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(TransactionSources);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.TransactionSource.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionSource.dbRow>>> TransactionSource_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("TransactionSource_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.TransactionSource.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.TransactionSource.dbRow> TransactionSources = await _dbContext.TransactionSource.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(TransactionSources);
                }
                catch (Exception ex)
                {
                    logger.Error("TransactionSource_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.TransactionSource.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.TransactionSource.dbRow>>> TransactionSource_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.TransactionSource.dbRow> TransactionSources = await _dbContext.TransactionSource.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.TransactionSource.dbRow>>(TransactionSources);
            }
            catch (Exception ex)
            {
                logger.Error("TransactionSource_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.TransactionSource.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
