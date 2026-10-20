using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region Purch Ord Dw
        public async Task<Result<int>> AwaitingPurchOrdDW_Insert(wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow AwaitingPurchOrdDW)
        {
            try
            {
                await _dbContext.AwaitingPurchOrdDW.AddAsync(AwaitingPurchOrdDW);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(AwaitingPurchOrdDW.ID);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_Insert - " + AwaitingPurchOrdDW.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<int>> AwaitingPurchOrdDW_Update(wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow AwaitingPurchOrdDW)
        {
            try
            {
                _dbContext.AwaitingPurchOrdDW.Attach(AwaitingPurchOrdDW);
                _dbContext.Entry(AwaitingPurchOrdDW).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(AwaitingPurchOrdDW.ID);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_Update - " + AwaitingPurchOrdDW.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<int>> AwaitingPurchOrdDW_Delete(wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow AwaitingPurchOrdDW)
        {
            try
            {
                _dbContext.AwaitingPurchOrdDW.Remove(AwaitingPurchOrdDW);
                _dbContext.Entry(AwaitingPurchOrdDW).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(AwaitingPurchOrdDW.ID);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_Delete - " + AwaitingPurchOrdDW.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<int>> AwaitingPurchOrdDW_DeleteByKEYID(string keyID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow AwaitingPurchOrdDW = _dbContext.AwaitingPurchOrdDW.FirstOrDefault(x => x.KEYID == keyID);
                if (AwaitingPurchOrdDW != null)
                {
                    _dbContext.AwaitingPurchOrdDW.Where(x => x.KEYID == keyID).ExecuteDelete();
                    await _dbContext.SaveChangesAsync();
                }

                return Result.Success<int>(AwaitingPurchOrdDW == null ? 0 : AwaitingPurchOrdDW.ID);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_DeleteByKEYID - " + keyID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>> AwaitingPurchOrdDW_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow AwaitingPurchOrdDWDb = await _dbContext.AwaitingPurchOrdDW.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (AwaitingPurchOrdDWDb == null)
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>(new List<AwaitingPurchOrdDW.dbRow>());
                else
                    return Result.Success< List < wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow >> (new List<AwaitingPurchOrdDW.dbRow> { AwaitingPurchOrdDWDb });
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception< List < wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow >> ("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>> AwaitingPurchOrdDW_List()
        {
            try
            {
                List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow> AwaitingPurchOrdDWDb = await _dbContext.AwaitingPurchOrdDW.AsNoTracking().ToListAsync();

                if (AwaitingPurchOrdDWDb == null)
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>(new List<AwaitingPurchOrdDW.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>(AwaitingPurchOrdDWDb);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_List: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>> AwaitingPurchOrdDW_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow AwaitingPurchOrdDWDb = await _dbContext.AwaitingPurchOrdDW.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (AwaitingPurchOrdDWDb == null)
                    return Result.Failure<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>("AwaitingPurchOrdDW not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>(AwaitingPurchOrdDWDb );

            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>> AwaitingPurchOrdDW_ListByKEYID(string KEYID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow AwaitingPurchOrdDWDb = await _dbContext.AwaitingPurchOrdDW.AsNoTracking()
                                                                        .Where(x => x.KEYID.StartsWith(KEYID)).FirstOrDefaultAsync();

                if (AwaitingPurchOrdDWDb == null)
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>(new List<AwaitingPurchOrdDW.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>(new List<AwaitingPurchOrdDW.dbRow> { AwaitingPurchOrdDWDb });
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_ListByKEYID - " + KEYID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>> AwaitingPurchOrdDW_ListByInsertedDate(DateTime InsertedDate)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow AwaitingPurchOrdDWDb = await _dbContext.AwaitingPurchOrdDW.AsNoTracking()
                                                                        .Where(x => x.InsertedDate >= InsertedDate).FirstOrDefaultAsync();

                if (AwaitingPurchOrdDWDb == null)
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>(new List<AwaitingPurchOrdDW.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>(new List<AwaitingPurchOrdDW.dbRow> { AwaitingPurchOrdDWDb });
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_ListByInsertedDate - " + InsertedDate.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>> AwaitingPurchOrdDW_SearchByKEYID(string KEYID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow AwaitingPurchOrdDWDb = await _dbContext.AwaitingPurchOrdDW.AsNoTracking()
                                                                        .Where(x => x.KEYID.StartsWith(KEYID)).FirstOrDefaultAsync();

                if (AwaitingPurchOrdDWDb == null)
                    return Result.Failure<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>("AwaitingPurchOrdDW not found : " + KEYID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>(AwaitingPurchOrdDWDb );

            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_SearchByKEYID - " + KEYID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>> AwaitingPurchOrdDW_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow> AwaitingPurchOrdDWs = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>(AwaitingPurchOrdDWs);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPurchOrdDW_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region Pick Head DW
        public async Task<Result<int>> AwaitingPickHeadDW_Insert(wbm_common.DataObjects.AwaitingPickHeadDW.dbRow AwaitingPickHeadDW)
        {
            try
            {
                await _dbContext.AwaitingPickHeadDW.AddAsync(AwaitingPickHeadDW);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(AwaitingPickHeadDW.ID);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_Insert - " + AwaitingPickHeadDW.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<int>> AwaitingPickHeadDW_Update(wbm_common.DataObjects.AwaitingPickHeadDW.dbRow AwaitingPickHeadDW)
        {
            try
            {
                _dbContext.AwaitingPickHeadDW.Attach(AwaitingPickHeadDW);
                _dbContext.Entry(AwaitingPickHeadDW).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(AwaitingPickHeadDW.ID);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_Update - " + AwaitingPickHeadDW.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<int>> AwaitingPickHeadDW_Delete(wbm_common.DataObjects.AwaitingPickHeadDW.dbRow AwaitingPickHeadDW)
        {
            try
            {
                _dbContext.AwaitingPickHeadDW.Remove(AwaitingPickHeadDW);
                _dbContext.Entry(AwaitingPickHeadDW).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(AwaitingPickHeadDW.ID);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_Delete - " + AwaitingPickHeadDW.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<int>> AwaitingPickHeadDW_DeleteByKEYID(string keyID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPickHeadDW.dbRow AwaitingPickHeadDW = _dbContext.AwaitingPickHeadDW.FirstOrDefault(x => x.KEYID == keyID);
                if (AwaitingPickHeadDW != null)
                {
                    _dbContext.AwaitingPickHeadDW.Where(x => x.KEYID == keyID).ExecuteDelete();
                    await _dbContext.SaveChangesAsync();
                }

                return Result.Success<int>(AwaitingPickHeadDW == null ? 0 : AwaitingPickHeadDW.ID);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_DeleteByKEYID - " + keyID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>> AwaitingPickHeadDW_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPickHeadDW.dbRow AwaitingPickHeadDWDb = await _dbContext.AwaitingPickHeadDW.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (AwaitingPickHeadDWDb == null)
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>(new List<AwaitingPickHeadDW.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>(new List<AwaitingPickHeadDW.dbRow> { AwaitingPickHeadDWDb });
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>> AwaitingPickHeadDW_List()
        {
            try
            {
                List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow> AwaitingPickHeadDWDb = await _dbContext.AwaitingPickHeadDW.AsNoTracking().ToListAsync();

                if (AwaitingPickHeadDWDb == null)
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>(new List<AwaitingPickHeadDW.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>(AwaitingPickHeadDWDb);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_List: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>> AwaitingPickHeadDW_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPickHeadDW.dbRow AwaitingPickHeadDWDb = await _dbContext.AwaitingPickHeadDW.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (AwaitingPickHeadDWDb == null)
                    return Result.Failure<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>("AwaitingPickHeadDW not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>(AwaitingPickHeadDWDb);

            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>> AwaitingPickHeadDW_ListByKEYID(string KEYID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPickHeadDW.dbRow AwaitingPickHeadDWDb = await _dbContext.AwaitingPickHeadDW.AsNoTracking()
                                                                        .Where(x => x.KEYID == KEYID).FirstOrDefaultAsync();

                if (AwaitingPickHeadDWDb == null)
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>(new List<AwaitingPickHeadDW.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>(new List<AwaitingPickHeadDW.dbRow> { AwaitingPickHeadDWDb });
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_ListByKEYID - " + KEYID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>> AwaitingPickHeadDW_ListByInsertedDate(DateTime InsertedDate)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPickHeadDW.dbRow AwaitingPickHeadDWDb = await _dbContext.AwaitingPickHeadDW.AsNoTracking()
                                                                        .Where(x => x.InsertedDate >= InsertedDate).FirstOrDefaultAsync();

                if (AwaitingPickHeadDWDb == null)
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>(new List<AwaitingPickHeadDW.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>(new List<AwaitingPickHeadDW.dbRow> { AwaitingPickHeadDWDb });
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_ListByInsertedDate - " + InsertedDate.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>> AwaitingPickHeadDW_SearchByKEYID(string KEYID)
        {
            try
            {
                wbm_common.DataObjects.AwaitingPickHeadDW.dbRow AwaitingPickHeadDWDb = await _dbContext.AwaitingPickHeadDW.AsNoTracking()
                                                                        .Where(x => x.KEYID == KEYID).FirstOrDefaultAsync();

                if (AwaitingPickHeadDWDb == null)
                    return Result.Failure<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>("AwaitingPickHeadDW not found : " + KEYID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>(AwaitingPickHeadDWDb);

            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_SearchByKEYID - " + KEYID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>> AwaitingPickHeadDW_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow> AwaitingPickHeadDWs = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>(AwaitingPickHeadDWs);
            }
            catch (Exception ex)
            {
                logger.Error("AwaitingPickHeadDW_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
