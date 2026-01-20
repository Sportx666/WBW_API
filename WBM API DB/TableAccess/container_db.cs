using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region ContainerSize
        public async Task<Result<int>> ContainerSize_Insert(wbm_common.DataObjects.ContainerSize.dbRow ContainerSize)
        {
            try
            {
                await _dbContext.ContainerSize.AddAsync(ContainerSize);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ContainerSize.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_Insert - " + ContainerSize.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ContainerSize_Update(wbm_common.DataObjects.ContainerSize.dbRow ContainerSize)
        {
            try
            {
                _dbContext.ContainerSize.Attach(ContainerSize);
                _dbContext.Entry(ContainerSize).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ContainerSize.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_Update - " + ContainerSize.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.ContainerSize.dbRow>> ContainerSize_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ContainerSize.dbRow ContainerSize = await _dbContext.ContainerSize.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (ContainerSize == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerSize.dbRow>("ContainerSize not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ContainerSize.dbRow>(ContainerSize);

            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerSize.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.ContainerSize.dbRow>>> ContainerSize_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerSize.dbRow ContainerSize = await _dbContext.ContainerSize.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ContainerSize == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRow>>(new List<wbm_common.DataObjects.ContainerSize.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRow>>(new List<wbm_common.DataObjects.ContainerSize.dbRow> { ContainerSize });
            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerSize.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ContainerSize.dbRow>> ContainerSize_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerSize.dbRow ContainerSize = await _dbContext.ContainerSize.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ContainerSize == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerSize.dbRow>("ContainerSize not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ContainerSize.dbRow>(ContainerSize);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerSize.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerSize.dbRow>>> ContainerSize_List()
        {
            try
            {
                List<wbm_common.DataObjects.ContainerSize.dbRow> ContainerSizes = await _dbContext.ContainerSize.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRow>>(ContainerSizes);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ContainerSize.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerSize.dbRow>>> ContainerSize_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ContainerSize_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ContainerSize.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ContainerSize.dbRow> ContainerSizes = await _dbContext.ContainerSize.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRow>>(ContainerSizes);
                }
                catch (Exception ex)
                {
                    logger.Error("ContainerSize_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ContainerSize.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerSize.dbRow>>> ContainerSize_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerSize.dbRow> ContainerSizes = await _dbContext.ContainerSize.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRow>>(ContainerSizes);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerSize.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region ContainerUnloadType
        public async Task<Result<int>> ContainerUnLoadType_Insert(wbm_common.DataObjects.ContainerUnLoadType.dbRow ContainerUnLoadType)
        {
            try
            {
                await _dbContext.ContainerUnLoadType.AddAsync(ContainerUnLoadType);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ContainerUnLoadType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_Insert - " + ContainerUnLoadType.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ContainerUnLoadType_Update(wbm_common.DataObjects.ContainerUnLoadType.dbRow ContainerUnLoadType)
        {
            try
            {
                _dbContext.ContainerUnLoadType.Attach(ContainerUnLoadType);
                _dbContext.Entry(ContainerUnLoadType).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ContainerUnLoadType.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_Update - " + ContainerUnLoadType.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.ContainerUnLoadType.dbRow>> ContainerUnLoadType_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ContainerUnLoadType.dbRow ContainerUnLoadType = await _dbContext.ContainerUnLoadType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (ContainerUnLoadType == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerUnLoadType.dbRow>("ContainerUnLoadType not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ContainerUnLoadType.dbRow>(ContainerUnLoadType);

            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerUnLoadType.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>> ContainerUnLoadType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerUnLoadType.dbRow ContainerUnLoadType = await _dbContext.ContainerUnLoadType.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ContainerUnLoadType == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>(new List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>(new List<wbm_common.DataObjects.ContainerUnLoadType.dbRow> { ContainerUnLoadType });
            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ContainerUnLoadType.dbRow>> ContainerUnLoadType_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerUnLoadType.dbRow ContainerUnLoadType = await _dbContext.ContainerUnLoadType.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ContainerUnLoadType == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerUnLoadType.dbRow>("ContainerUnLoadType not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ContainerUnLoadType.dbRow>(ContainerUnLoadType);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerUnLoadType.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>> ContainerUnLoadType_List()
        {
            try
            {
                List<wbm_common.DataObjects.ContainerUnLoadType.dbRow> ContainerUnLoadTypes = await _dbContext.ContainerUnLoadType.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>(ContainerUnLoadTypes);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>> ContainerUnLoadType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ContainerUnLoadType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ContainerUnLoadType.dbRow> ContainerUnLoadTypes = await _dbContext.ContainerUnLoadType.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>(ContainerUnLoadTypes);
                }
                catch (Exception ex)
                {
                    logger.Error("ContainerUnLoadType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>> ContainerUnLoadType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerUnLoadType.dbRow> ContainerUnLoadTypes = await _dbContext.ContainerUnLoadType.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>(ContainerUnLoadTypes);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region ContainerLift
        public async Task<Result<int>> ContainerLift_Insert(wbm_common.DataObjects.ContainerLift.dbRow ContainerLift)
        {
            try
            {
                await _dbContext.ContainerLift.AddAsync(ContainerLift);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ContainerLift.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_Insert - " + ContainerLift.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ContainerLift_Update(wbm_common.DataObjects.ContainerLift.dbRow ContainerLift)
        {
            try
            {
                _dbContext.ContainerLift.Attach(ContainerLift);
                _dbContext.Entry(ContainerLift).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ContainerLift.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_Update - " + ContainerLift.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRow>>> ContainerLift_ListByOwnerID(int OwnerID)
        {
            try
            {
                wbm_common.DataObjects.ContainerLift.dbRow ContainerLift = await _dbContext.ContainerLift.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.OwnerID == OwnerID);

                if (ContainerLift == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRow>>(new List<wbm_common.DataObjects.ContainerLift.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRow>>(new List<wbm_common.DataObjects.ContainerLift.dbRow> { ContainerLift });
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_ListByPaperlessKeyID - " + OwnerID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.ContainerLift.dbRow>> ContainerLift_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerLift.dbRow ContainerLift = await _dbContext.ContainerLift.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ContainerLift == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerLift.dbRow>("ContainerLift not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ContainerLift.dbRow>(ContainerLift);

            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerLift.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRow>>> ContainerLiftList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerLift.dbRow ContainerLift = await _dbContext.ContainerLift.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ContainerLift == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRow>>(new List<wbm_common.DataObjects.ContainerLift.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRow>>(new List<wbm_common.DataObjects.ContainerLift.dbRow> { ContainerLift });
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ContainerLift.dbRow>> ContainerLift_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerLift.dbRow ContainerLift = await _dbContext.ContainerLift.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ContainerLift == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerLift.dbRow>("ContainerLift not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ContainerLift.dbRow>(ContainerLift);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerLift.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRow>>> ContainerLift_List()
        {
            try
            {
                List<wbm_common.DataObjects.ContainerLift.dbRow> ContainerLifts = await _dbContext.ContainerLift.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRow>>(ContainerLifts);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRow>>> ContainerLift_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ContainerLift_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ContainerLift.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ContainerLift.dbRow> ContainerLifts = await _dbContext.ContainerLift.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRow>>(ContainerLifts);
                }
                catch (Exception ex)
                {
                    logger.Error("ContainerLift_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRow>>> ContainerLift_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerLift.dbRow> ContainerLifts = await _dbContext.ContainerLift.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRow>>(ContainerLifts);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region ContainerHeavyLift
        public async Task<Result<int>> ContainerHeavyLift_Insert(wbm_common.DataObjects.ContainerHeavyLift.dbRow ContainerHeavyLift)
        {
            try
            {
                await _dbContext.ContainerHeavyLift.AddAsync(ContainerHeavyLift);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ContainerHeavyLift.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_Insert - " + ContainerHeavyLift.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ContainerHeavyLift_Update(wbm_common.DataObjects.ContainerHeavyLift.dbRow ContainerHeavyLift)
        {
            try
            {
                _dbContext.ContainerHeavyLift.Attach(ContainerHeavyLift);
                _dbContext.Entry(ContainerHeavyLift).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ContainerHeavyLift.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_Update - " + ContainerHeavyLift.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>> ContainerHeavyLift_ListByOwnerID(int OwnerID)
        {
            try
            {
                wbm_common.DataObjects.ContainerHeavyLift.dbRow ContainerHeavyLift = await _dbContext.ContainerHeavyLift.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.OwnerID == OwnerID);

                if (ContainerHeavyLift == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>(new List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>(new List<wbm_common.DataObjects.ContainerHeavyLift.dbRow> { ContainerHeavyLift });
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_ListByPaperlessKeyID - " + OwnerID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.ContainerHeavyLift.dbRow>> ContainerHeavyLift_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerHeavyLift.dbRow ContainerHeavyLift = await _dbContext.ContainerHeavyLift.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ContainerHeavyLift == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerHeavyLift.dbRow>("ContainerHeavyLift not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ContainerHeavyLift.dbRow>(ContainerHeavyLift);

            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerHeavyLift.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>> ContainerHeavyLiftList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerHeavyLift.dbRow ContainerHeavyLift = await _dbContext.ContainerHeavyLift.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ContainerHeavyLift == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>(new List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>(new List<wbm_common.DataObjects.ContainerHeavyLift.dbRow> { ContainerHeavyLift });
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ContainerHeavyLift.dbRow>> ContainerHeavyLift_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerHeavyLift.dbRow ContainerHeavyLift = await _dbContext.ContainerHeavyLift.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ContainerHeavyLift == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerHeavyLift.dbRow>("ContainerHeavyLift not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ContainerHeavyLift.dbRow>(ContainerHeavyLift);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerHeavyLift.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>> ContainerHeavyLift_List()
        {
            try
            {
                List<wbm_common.DataObjects.ContainerHeavyLift.dbRow> ContainerHeavyLifts = await _dbContext.ContainerHeavyLift.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>(ContainerHeavyLifts);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>> ContainerHeavyLift_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ContainerHeavyLift_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ContainerHeavyLift.dbRow> ContainerHeavyLifts = await _dbContext.ContainerHeavyLift.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>(ContainerHeavyLifts);
                }
                catch (Exception ex)
                {
                    logger.Error("ContainerHeavyLift_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>> ContainerHeavyLift_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerHeavyLift.dbRow> ContainerHeavyLifts = await _dbContext.ContainerHeavyLift.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>(ContainerHeavyLifts);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
