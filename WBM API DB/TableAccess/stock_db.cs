using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region Stock
        public async Task<Result<int>> Stock_Insert(wbm_common.DataObjects.Stock.dbRow Stock)
        {
            try
            {
                await _dbContext.Stock.AddAsync(Stock);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Stock.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_Insert - " + Stock.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Stock_Update(wbm_common.DataObjects.Stock.dbRow Stock)
        {
            try
            {
                _dbContext.Stock.Attach(Stock);
                _dbContext.Entry(Stock).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Stock.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_Update - " + Stock.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRow>>> Stock_ListByPaperlessKeyID(string PaperlessKeyID)
        {
            try
            {
                wbm_common.DataObjects.Stock.dbRow Stock = await _dbContext.Stock.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.Paperless_KeyID == PaperlessKeyID);

                if (Stock == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(new List<wbm_common.DataObjects.Stock.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(new List<wbm_common.DataObjects.Stock.dbRow> { Stock });
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListByPaperlessKeyID - " + PaperlessKeyID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRow>>> Stock_ListBySingleSiteID(int SiteID)
        {
            try
            {
                wbm_common.DataObjects.Stock.dbRow Stock = await _dbContext.Stock.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.SiteID == SiteID);

                if (Stock == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(new List<wbm_common.DataObjects.Stock.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(new List<wbm_common.DataObjects.Stock.dbRow> { Stock });
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListBySingleSiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRow>>> Stock_ListBySingleOwnerID(int OwnerID)
        {
            try
            {
                wbm_common.DataObjects.Stock.dbRow Stock = await _dbContext.Stock.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.OwnerID == OwnerID);

                if (Stock == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(new List<wbm_common.DataObjects.Stock.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(new List<wbm_common.DataObjects.Stock.dbRow> { Stock });
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListBySingleOwnerID - " + OwnerID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Stock.dbRow>> Stock_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Stock.dbRow Stock = await _dbContext.Stock.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Stock == null)
                    return Result.Failure<wbm_common.DataObjects.Stock.dbRow>("Stock not found : " + ID);
                else
                    return Result.Success<wbm_common.DataObjects.Stock.dbRow>(Stock);

            }
            catch (Exception ex)
            {
                logger.Error("Stock_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Stock.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRow>>> StockList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Stock.dbRow Stock = await _dbContext.Stock.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Stock == null)
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(new List<wbm_common.DataObjects.Stock.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(new List<wbm_common.DataObjects.Stock.dbRow> { Stock });
            }
            catch (Exception ex)
            {
                logger.Error("Stock_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Stock.dbRow>> Stock_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Stock.dbRow Stock = await _dbContext.Stock.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (Stock == null)
                    return Result.Failure<wbm_common.DataObjects.Stock.dbRow>("Stock not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Stock.dbRow>(Stock);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Stock.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRow>>> Stock_List()
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRow> Stocks = await _dbContext.Stock.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(Stocks);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRow>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRow>>> Stock_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Stock_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Stock.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Stock.dbRow> Stocks = await _dbContext.Stock.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(Stocks);
                }
                catch (Exception ex)
                {
                    logger.Error("Stock_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Stock.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Stock.dbRow>>> Stock_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Stock.dbRow> Stocks = await _dbContext.Stock.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Stock.dbRow>>(Stocks);
            }
            catch (Exception ex)
            {
                logger.Error("Stock_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Stock.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region StockOnHand
        public async Task<Result<int>> StockOnHand_Insert(wbm_common.DataObjects.StockOnHand.dbRow StockOnHand)
        {
            try
            {
                await _dbContext.StockOnHand.AddAsync(StockOnHand);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StockOnHand.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_Insert - " + StockOnHand.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> StockOnHand_Update(wbm_common.DataObjects.StockOnHand.dbRow StockOnHand)
        {
            try
            {
                _dbContext.StockOnHand.Attach(StockOnHand);
                _dbContext.Entry(StockOnHand).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StockOnHand.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_Update - " + StockOnHand.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRow>>> StockOnHand_ListBySingleSiteID(int SiteID)
        {
            try
            {
                wbm_common.DataObjects.StockOnHand.dbRow StockOnHand = await _dbContext.StockOnHand.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.SiteID == SiteID);

                if (StockOnHand == null)
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRow>>(new List<wbm_common.DataObjects.StockOnHand.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRow>>(new List<wbm_common.DataObjects.StockOnHand.dbRow> { StockOnHand });
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_ListBySingleSiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRow>>> StockOnHand_ListBySingleOwnerID(int OwnerID)
        {
            try
            {
                wbm_common.DataObjects.StockOnHand.dbRow StockOnHand = await _dbContext.StockOnHand.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.OwnerID == OwnerID);

                if (StockOnHand == null)
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRow>>(new List<wbm_common.DataObjects.StockOnHand.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRow>>(new List<wbm_common.DataObjects.StockOnHand.dbRow> { StockOnHand });
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_ListBySingleOwnerID - " + OwnerID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.StockOnHand.dbRow>> StockOnHand_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StockOnHand.dbRow StockOnHand = await _dbContext.StockOnHand.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (StockOnHand == null)
                    return Result.Failure<wbm_common.DataObjects.StockOnHand.dbRow>("StockOnHand not found : " + ID);
                else
                    return Result.Success<wbm_common.DataObjects.StockOnHand.dbRow>(StockOnHand);

            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.StockOnHand.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRow>>> StockOnHandList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StockOnHand.dbRow StockOnHand = await _dbContext.StockOnHand.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (StockOnHand == null)
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRow>>(new List<wbm_common.DataObjects.StockOnHand.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRow>>(new List<wbm_common.DataObjects.StockOnHand.dbRow> { StockOnHand });
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.StockOnHand.dbRow>> StockOnHand_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StockOnHand.dbRow StockOnHand = await _dbContext.StockOnHand.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (StockOnHand == null)
                    return Result.Failure<wbm_common.DataObjects.StockOnHand.dbRow>("StockOnHand not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.StockOnHand.dbRow>(StockOnHand);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.StockOnHand.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRow>>> StockOnHand_List()
        {
            try
            {
                List<wbm_common.DataObjects.StockOnHand.dbRow> StockOnHands = await _dbContext.StockOnHand.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRow>>(StockOnHands);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockOnHand.dbRow>>> StockOnHand_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.StockOnHand.dbRow> StockOnHands = await _dbContext.StockOnHand.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.StockOnHand.dbRow>>(StockOnHands);
            }
            catch (Exception ex)
            {
                logger.Error("StockOnHand_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockOnHand.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region StockRateCategory
        public async Task<Result<int>> StockRateCategory_Insert(wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategory)
        {
            try
            {
                await _dbContext.StockRateCategory.AddAsync(StockRateCategory);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StockRateCategory.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_Insert - " + StockRateCategory.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> StockRateCategory_Update(wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategory)
        {
            try
            {
                _dbContext.StockRateCategory.Attach(StockRateCategory);
                _dbContext.Entry(StockRateCategory).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(StockRateCategory.ID);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_Update - " + StockRateCategory.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRow>>> StockRateCategory_ListBySingleSiteID(int SiteID)
        {
            try
            {
                wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategory = await _dbContext.StockRateCategory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.SiteID == SiteID);

                if (StockRateCategory == null)
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRow>>(new List<wbm_common.DataObjects.StockRateCategory.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRow>>(new List<wbm_common.DataObjects.StockRateCategory.dbRow> { StockRateCategory });
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_ListBySingleSiteID - " + SiteID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRow>>> StockRateCategory_ListBySingleCompanyID(int CompanyID)
        {
            try
            {
                wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategory = await _dbContext.StockRateCategory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.CompanyID == CompanyID);

                if (StockRateCategory == null)
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRow>>(new List<wbm_common.DataObjects.StockRateCategory.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRow>>(new List<wbm_common.DataObjects.StockRateCategory.dbRow> { StockRateCategory });
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_ListBySingleCompanyID - " + CompanyID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.StockRateCategory.dbRow>> StockRateCategory_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategory = await _dbContext.StockRateCategory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (StockRateCategory == null)
                    return Result.Failure<wbm_common.DataObjects.StockRateCategory.dbRow>("StockRateCategory not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.StockRateCategory.dbRow>(StockRateCategory);

            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.StockRateCategory.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRow>>> StockRateCategory_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategory = await _dbContext.StockRateCategory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (StockRateCategory == null)
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRow>>(new List<wbm_common.DataObjects.StockRateCategory.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRow>>(new List<wbm_common.DataObjects.StockRateCategory.dbRow> { StockRateCategory });
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.StockRateCategory.dbRow>> StockRateCategory_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.StockRateCategory.dbRow StockRateCategory = await _dbContext.StockRateCategory.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (StockRateCategory == null)
                    return Result.Failure<wbm_common.DataObjects.StockRateCategory.dbRow>("StockRateCategory not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.StockRateCategory.dbRow>(StockRateCategory);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.StockRateCategory.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRow>>> StockRateCategory_List()
        {
            try
            {
                List<wbm_common.DataObjects.StockRateCategory.dbRow> StockRateCategorys = await _dbContext.StockRateCategory.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRow>>(StockRateCategorys);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRow>>> StockRateCategory_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("StockRateCategory_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.StockRateCategory.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.StockRateCategory.dbRow> StockRateCategorys = await _dbContext.StockRateCategory.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRow>>(StockRateCategorys);
                }
                catch (Exception ex)
                {
                    logger.Error("StockRateCategory_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.StockRateCategory.dbRow>>> StockRateCategory_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.StockRateCategory.dbRow> StockRateCategorys = await _dbContext.StockRateCategory.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.StockRateCategory.dbRow>>(StockRateCategorys);
            }
            catch (Exception ex)
            {
                logger.Error("StockRateCategory_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.StockRateCategory.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
