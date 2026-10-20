using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

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

        public async Task<Result<wbm_common.DataObjects.ContainerSize.dbRowExtended>> ContainerSize_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ContainerSize.dbRowExtended ContainerSizedb = await (from ContainerSize in _dbContext.ContainerSize.AsNoTracking()
                                        .Where(x => !x.DeletedFlag && x.PublicID == PublicID)
                                                                                      join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                            on ContainerSize.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                      from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                      select new ContainerSize.dbRowExtended
                                                                                      {
                                                                                          ID = ContainerSize.ID,
                                                                                          PublicID = ContainerSize.PublicID,
                                                                                          Description = ContainerSize.Description,
                                                                                          SortOrder = ContainerSize.SortOrder,
                                                                                          CreatedDateTime = ContainerSize.CreatedDateTime,
                                                                                          CreatedMethod = ContainerSize.CreatedMethod,
                                                                                          CreatedUserID = ContainerSize.CreatedUserID,
                                                                                          LastAmendedDateTime = ContainerSize.LastAmendedDateTime,
                                                                                          LastAmendedMethod = ContainerSize.LastAmendedMethod,
                                                                                          LastAmendedUserID = ContainerSize.LastAmendedUserID,

                                                                                          Created_UserPublicID = SU.PublicID
                                                                                      }).FirstOrDefaultAsync();


                if (ContainerSizedb == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerSize.dbRowExtended>("ContainerSize not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ContainerSize.dbRowExtended>(ContainerSizedb);

            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerSize.dbRowExtended>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>> ContainerSize_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerSize.dbRowExtended ContainerSizedb = await (from ContainerSize in _dbContext.ContainerSize.AsNoTracking()
                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                            join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                  on ContainerSize.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                            from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                            select new ContainerSize.dbRowExtended
                                                                                            {
                                                                                                ID = ContainerSize.ID,
                                                                                                PublicID = ContainerSize.PublicID,
                                                                                                Description = ContainerSize.Description,
                                                                                                SortOrder = ContainerSize.SortOrder,
                                                                                                CreatedDateTime = ContainerSize.CreatedDateTime,
                                                                                                CreatedMethod = ContainerSize.CreatedMethod,
                                                                                                CreatedUserID = ContainerSize.CreatedUserID,
                                                                                                LastAmendedDateTime = ContainerSize.LastAmendedDateTime,
                                                                                                LastAmendedMethod = ContainerSize.LastAmendedMethod,
                                                                                                LastAmendedUserID = ContainerSize.LastAmendedUserID,

                                                                                                Created_UserPublicID = SU.PublicID
                                                                                            }).FirstOrDefaultAsync();


                if (ContainerSizedb == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>(new List<wbm_common.DataObjects.ContainerSize.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>(new List<wbm_common.DataObjects.ContainerSize.dbRowExtended> { ContainerSizedb });
            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>("Internal Exception occured");
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

        public async Task<Result<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>> ContainerSize_List()
        {
            try
            {
                List<wbm_common.DataObjects.ContainerSize.dbRowExtended> ContainerSizedb = await (from ContainerSize in _dbContext.ContainerSize.AsNoTracking()
                                        .Where(x => !x.DeletedFlag)
                                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                    on ContainerSize.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                  from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                  select new ContainerSize.dbRowExtended
                                                                                                  {
                                                                                                      ID = ContainerSize.ID,
                                                                                                      PublicID = ContainerSize.PublicID,
                                                                                                      Description = ContainerSize.Description,
                                                                                                      SortOrder = ContainerSize.SortOrder,
                                                                                                      CreatedDateTime = ContainerSize.CreatedDateTime,
                                                                                                      CreatedMethod = ContainerSize.CreatedMethod,
                                                                                                      CreatedUserID = ContainerSize.CreatedUserID,
                                                                                                      LastAmendedDateTime = ContainerSize.LastAmendedDateTime,
                                                                                                      LastAmendedMethod = ContainerSize.LastAmendedMethod,
                                                                                                      LastAmendedUserID = ContainerSize.LastAmendedUserID,

                                                                                                      Created_UserPublicID = SU.PublicID

                                                                                                  }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>(ContainerSizedb);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>> ContainerSize_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ContainerSize_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ContainerSize.dbRowExtended> ContainerSizedb = await (from ContainerSize in _dbContext.ContainerSize.AsNoTracking()
                                            .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                      join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                                        on ContainerSize.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                      from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                      select new ContainerSize.dbRowExtended
                                                                                                      {
                                                                                                          ID = ContainerSize.ID,
                                                                                                          PublicID = ContainerSize.PublicID,
                                                                                                          Description = ContainerSize.Description,
                                                                                                          SortOrder = ContainerSize.SortOrder,
                                                                                                          CreatedDateTime = ContainerSize.CreatedDateTime,
                                                                                                          CreatedMethod = ContainerSize.CreatedMethod,
                                                                                                          CreatedUserID = ContainerSize.CreatedUserID,
                                                                                                          LastAmendedDateTime = ContainerSize.LastAmendedDateTime,
                                                                                                          LastAmendedMethod = ContainerSize.LastAmendedMethod,
                                                                                                          LastAmendedUserID = ContainerSize.LastAmendedUserID,

                                                                                                          Created_UserPublicID = SU.PublicID

                                                                                                      }).ToListAsync();


                    return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>(ContainerSizedb);
                }
                catch (Exception ex)
                {
                    logger.Error("ContainerSize_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>> ContainerSize_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerSize.dbRowExtended> ContainerSizes = await _dbContext.Database.SqlQueryRaw<ContainerSize.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>(ContainerSizes);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerSize_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerSize.dbRowExtended>>("Internal Exception occured");
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

        public async Task<Result<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>> ContainerUnLoadType_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended ContainerUnLoadTypedb = await (from ContainerUnLoadType in _dbContext.ContainerUnLoadType.AsNoTracking()
                                        .Where(x => !x.DeletedFlag && x.PublicID == PublicID)
                                                                                      join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                            on ContainerUnLoadType.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                      from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                      select new ContainerUnLoadType.dbRowExtended
                                                                                      {
                                                                                          ID = ContainerUnLoadType.ID,
                                                                                          PublicID = ContainerUnLoadType.PublicID,
                                                                                          Description = ContainerUnLoadType.Description,
                                                                                          SortOrder = ContainerUnLoadType.SortOrder,
                                                                                          CreatedDateTime = ContainerUnLoadType.CreatedDateTime,
                                                                                          CreatedMethod = ContainerUnLoadType.CreatedMethod,
                                                                                          CreatedUserID = ContainerUnLoadType.CreatedUserID,
                                                                                          LastAmendedDateTime = ContainerUnLoadType.LastAmendedDateTime,
                                                                                          LastAmendedMethod = ContainerUnLoadType.LastAmendedMethod,
                                                                                          LastAmendedUserID = ContainerUnLoadType.LastAmendedUserID,

                                                                                          Created_UserPublicID = SU.PublicID
                                                                                      }).FirstOrDefaultAsync();


                if (ContainerUnLoadTypedb == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>("ContainerUnLoadType not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>(ContainerUnLoadTypedb);

            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>> ContainerUnLoadType_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended ContainerUnLoadTypedb = await (from ContainerUnLoadType in _dbContext.ContainerUnLoadType.AsNoTracking()
                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                        join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                              on ContainerUnLoadType.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                        from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                        select new ContainerUnLoadType.dbRowExtended
                                                                                                        {
                                                                                                            ID = ContainerUnLoadType.ID,
                                                                                                            PublicID = ContainerUnLoadType.PublicID,
                                                                                                            Description = ContainerUnLoadType.Description,
                                                                                                            SortOrder = ContainerUnLoadType.SortOrder,
                                                                                                            CreatedDateTime = ContainerUnLoadType.CreatedDateTime,
                                                                                                            CreatedMethod = ContainerUnLoadType.CreatedMethod,
                                                                                                            CreatedUserID = ContainerUnLoadType.CreatedUserID,
                                                                                                            LastAmendedDateTime = ContainerUnLoadType.LastAmendedDateTime,
                                                                                                            LastAmendedMethod = ContainerUnLoadType.LastAmendedMethod,
                                                                                                            LastAmendedUserID = ContainerUnLoadType.LastAmendedUserID,

                                                                                                            Created_UserPublicID = SU.PublicID
                                                                                                        }).FirstOrDefaultAsync();



                if (ContainerUnLoadTypedb == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>(new List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>(new List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended> { ContainerUnLoadTypedb });
            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>("Internal Exception occured");
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

        public async Task<Result<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>> ContainerUnLoadType_List()
        {
            try
            {
                List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended> ContainerUnLoadTypesdb = await (from ContainerUnLoadType in _dbContext.ContainerUnLoadType.AsNoTracking()
                        .Where(x => !x.DeletedFlag)
                                                                                                         join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                       on ContainerUnLoadType.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                         from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                         select new ContainerUnLoadType.dbRowExtended
                                                                                                         {
                                                                                                             ID = ContainerUnLoadType.ID,
                                                                                                             PublicID = ContainerUnLoadType.PublicID,
                                                                                                             Description = ContainerUnLoadType.Description,
                                                                                                             SortOrder = ContainerUnLoadType.SortOrder,
                                                                                                             CreatedDateTime = ContainerUnLoadType.CreatedDateTime,
                                                                                                             CreatedMethod = ContainerUnLoadType.CreatedMethod,
                                                                                                             CreatedUserID = ContainerUnLoadType.CreatedUserID,
                                                                                                             LastAmendedDateTime = ContainerUnLoadType.LastAmendedDateTime,
                                                                                                             LastAmendedMethod = ContainerUnLoadType.LastAmendedMethod,
                                                                                                             LastAmendedUserID = ContainerUnLoadType.LastAmendedUserID,

                                                                                                             Created_UserPublicID = SU.PublicID
                                                                                                  }).ToListAsync();


                return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>(ContainerUnLoadTypesdb);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>> ContainerUnLoadType_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ContainerUnLoadType_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended> ContainerUnLoadTypesdb = await (from ContainerUnLoadType in _dbContext.ContainerUnLoadType.AsNoTracking()
        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                                   join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                 on ContainerUnLoadType.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                                   from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                                   select new ContainerUnLoadType.dbRowExtended
                                                                                                                   {
                                                                                                                       ID = ContainerUnLoadType.ID,
                                                                                                                       PublicID = ContainerUnLoadType.PublicID,
                                                                                                                       Description = ContainerUnLoadType.Description,
                                                                                                                       SortOrder = ContainerUnLoadType.SortOrder,
                                                                                                                       CreatedDateTime = ContainerUnLoadType.CreatedDateTime,
                                                                                                                       CreatedMethod = ContainerUnLoadType.CreatedMethod,
                                                                                                                       CreatedUserID = ContainerUnLoadType.CreatedUserID,
                                                                                                                       LastAmendedDateTime = ContainerUnLoadType.LastAmendedDateTime,
                                                                                                                       LastAmendedMethod = ContainerUnLoadType.LastAmendedMethod,
                                                                                                                       LastAmendedUserID = ContainerUnLoadType.LastAmendedUserID,

                                                                                                                       Created_UserPublicID = SU.PublicID
                                                                                                                   }).ToListAsync();


                    return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>(ContainerUnLoadTypesdb);
                }
                catch (Exception ex)
                {
                    logger.Error("ContainerUnLoadType_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>> ContainerUnLoadType_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended> ContainerUnLoadTypes = await _dbContext.Database.SqlQueryRaw<ContainerUnLoadType.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>(ContainerUnLoadTypes);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerUnLoadType_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerUnLoadType.dbRowExtended>>("Internal Exception occured");
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

        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>> ContainerLift_ListByOwnerID(int OwnerID)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerLift.dbRowExtended> ContainerLiftDb = await (from ContainerLift in _dbContext.ContainerLift.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.OwnerID == OwnerID || x.OwnerID == 0))
                                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                                      on ContainerLift.SiteID equals site.ID into sitePR
                                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                        on ContainerLift.CompanyID equals company.ID into companyPR
                                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                        on ContainerLift.OwnerID equals Owner.ID into OwnerPR
                                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on ContainerLift.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                  from SU in SecurityUserPR.DefaultIfEmpty()


                                                                                                  select new ContainerLift.dbRowExtended
                                                                                                  {
                                                                                                      ID = ContainerLift.ID,
                                                                                                      CompanyID = ContainerLift.CompanyID,
                                                                                                      OwnerID = ContainerLift.OwnerID,
                                                                                                      SiteID = ContainerLift.SiteID,
                                                                                                      DefaultNumberOfLifts = ContainerLift.DefaultNumberOfLifts,
                                                                                                      DeletedFlag = ContainerLift.DeletedFlag,
                                                                                                      CreatedDateTime = ContainerLift.CreatedDateTime,
                                                                                                      CreatedMethod = ContainerLift.CreatedMethod,
                                                                                                      CreatedUserID = ContainerLift.CreatedUserID,
                                                                                                      LastAmendedDateTime = ContainerLift.LastAmendedDateTime,
                                                                                                      LastAmendedMethod = ContainerLift.LastAmendedMethod,
                                                                                                      LastAmendedUserID = ContainerLift.LastAmendedUserID,

                                                                                                      Site_PublicID = SPR.PublicID,
                                                                                                      Company_ShortName = CPR.ShortName,
                                                                                                      Owner_Name = OPR.Name,
                                                                                                      Created_UserPublicID = SU.PublicID
                                                                                                  }).ToListAsync();

                if (ContainerLiftDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>(new List<wbm_common.DataObjects.ContainerLift.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>(ContainerLiftDb);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_ListByOwnerID - " + OwnerID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>> ContainerLift_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerLift.dbRowExtended> ContainerLiftDb = await (from ContainerLift in _dbContext.ContainerLift.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                  join site in _dbContext.Site.AsNoTracking()
                                                                                                      on ContainerLift.SiteID equals site.ID into sitePR
                                                                                                  from SPR in sitePR.DefaultIfEmpty()
                                                                                                  join company in _dbContext.Company.AsNoTracking()
                                                                                        on ContainerLift.CompanyID equals company.ID into companyPR
                                                                                                  from CPR in companyPR.DefaultIfEmpty()
                                                                                                  join Owner in _dbContext.Owner.AsNoTracking()
                                                                                        on ContainerLift.OwnerID equals Owner.ID into OwnerPR
                                                                                                  from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on ContainerLift.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                    from SU in SecurityUserPR.DefaultIfEmpty()


                                                                                                  select new ContainerLift.dbRowExtended
                                                                                                  {
                                                                                                      ID = ContainerLift.ID,
                                                                                                      CompanyID = ContainerLift.CompanyID,
                                                                                                      OwnerID = ContainerLift.OwnerID,
                                                                                                      SiteID = ContainerLift.SiteID,
                                                                                                      DefaultNumberOfLifts = ContainerLift.DefaultNumberOfLifts,
                                                                                                      DeletedFlag = ContainerLift.DeletedFlag,
                                                                                                      CreatedDateTime = ContainerLift.CreatedDateTime,
                                                                                                      CreatedMethod = ContainerLift.CreatedMethod,
                                                                                                      CreatedUserID = ContainerLift.CreatedUserID,
                                                                                                      LastAmendedDateTime = ContainerLift.LastAmendedDateTime,
                                                                                                      LastAmendedMethod = ContainerLift.LastAmendedMethod,
                                                                                                      LastAmendedUserID = ContainerLift.LastAmendedUserID,

                                                                                                      Site_PublicID = SPR.PublicID,
                                                                                                      Company_ShortName = CPR.ShortName,
                                                                                                      Owner_Name = OPR.Name,
                                                                                                      Created_UserPublicID = SU.PublicID
                                                                                                  }).ToListAsync();

                if (ContainerLiftDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>(new List<wbm_common.DataObjects.ContainerLift.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>( ContainerLiftDb );
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ContainerLift.dbRow>> ContainerLift_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerLift.dbRow ContainerLiftDb = await _dbContext.ContainerLift.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (ContainerLiftDb == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerLift.dbRow>("ContainerLift not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ContainerLift.dbRow>(ContainerLiftDb);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerLift.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>> ContainerLift_List()
        {
            try
            {
                List<wbm_common.DataObjects.ContainerLift.dbRowExtended> ContainerLifts = await (from ContainerLift in _dbContext.ContainerLift.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                 join site in _dbContext.Site.AsNoTracking()
                                                                                                     on ContainerLift.SiteID equals site.ID into sitePR
                                                                                                 from SPR in sitePR.DefaultIfEmpty()
                                                                                                 join company in _dbContext.Company.AsNoTracking()
                                                                                       on ContainerLift.CompanyID equals company.ID into companyPR
                                                                                                 from CPR in companyPR.DefaultIfEmpty()
                                                                                                 join Owner in _dbContext.Owner.AsNoTracking()
                                                                                       on ContainerLift.OwnerID equals Owner.ID into OwnerPR
                                                                                                 from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                 join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on ContainerLift.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                 from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                 select new ContainerLift.dbRowExtended
                                                                                                 {
                                                                                                     ID = ContainerLift.ID,
                                                                                                     CompanyID = ContainerLift.CompanyID,
                                                                                                     OwnerID = ContainerLift.OwnerID,
                                                                                                     SiteID = ContainerLift.SiteID,
                                                                                                     DefaultNumberOfLifts = ContainerLift.DefaultNumberOfLifts,
                                                                                                     DeletedFlag = ContainerLift.DeletedFlag,
                                                                                                     CreatedDateTime = ContainerLift.CreatedDateTime,
                                                                                                     CreatedMethod = ContainerLift.CreatedMethod,
                                                                                                     CreatedUserID = ContainerLift.CreatedUserID,
                                                                                                     LastAmendedDateTime = ContainerLift.LastAmendedDateTime,
                                                                                                     LastAmendedMethod = ContainerLift.LastAmendedMethod,
                                                                                                     LastAmendedUserID = ContainerLift.LastAmendedUserID,

                                                                                                     Site_PublicID = SPR.PublicID,
                                                                                                     Company_ShortName = CPR.ShortName,
                                                                                                     Owner_Name = OPR.Name,
                                                                                                     Created_UserPublicID = SU.PublicID
                                                                                                 }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>(ContainerLifts);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>> ContainerLift_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ContainerLift_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ContainerLift.dbRowExtended> ContainerLifts = await (from ContainerLift in _dbContext.ContainerLift.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                     join site in _dbContext.Site.AsNoTracking()
                                                                                                         on ContainerLift.SiteID equals site.ID into sitePR
                                                                                                     from SPR in sitePR.DefaultIfEmpty()
                                                                                                     join company in _dbContext.Company.AsNoTracking()
                                                                                           on ContainerLift.CompanyID equals company.ID into companyPR
                                                                                                     from CPR in companyPR.DefaultIfEmpty()
                                                                                                     join Owner in _dbContext.Owner.AsNoTracking()
                                                                                           on ContainerLift.OwnerID equals Owner.ID into OwnerPR
                                                                                                     from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                     join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on ContainerLift.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                     from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                     select new ContainerLift.dbRowExtended
                                                                                                     {
                                                                                                         ID = ContainerLift.ID,
                                                                                                         CompanyID = ContainerLift.CompanyID,
                                                                                                         OwnerID = ContainerLift.OwnerID,
                                                                                                         SiteID = ContainerLift.SiteID,
                                                                                                         DefaultNumberOfLifts = ContainerLift.DefaultNumberOfLifts,
                                                                                                         DeletedFlag = ContainerLift.DeletedFlag,
                                                                                                         CreatedDateTime = ContainerLift.CreatedDateTime,
                                                                                                         CreatedMethod = ContainerLift.CreatedMethod,
                                                                                                         CreatedUserID = ContainerLift.CreatedUserID,
                                                                                                         LastAmendedDateTime = ContainerLift.LastAmendedDateTime,
                                                                                                         LastAmendedMethod = ContainerLift.LastAmendedMethod,
                                                                                                         LastAmendedUserID = ContainerLift.LastAmendedUserID,

                                                                                                         Site_PublicID = SPR.PublicID,
                                                                                                         Company_ShortName = CPR.ShortName,
                                                                                                         Owner_Name = OPR.Name,
                                                                                                         Created_UserPublicID = SU.PublicID
                                                                                                     }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>(ContainerLifts);
                }
                catch (Exception ex)
                {
                    logger.Error("ContainerLift_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>> ContainerLift_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerLift.dbRowExtended> ContainerLifts = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.ContainerLift.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>(ContainerLifts);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerLift_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerLift.dbRowExtended>>("Internal Exception occured");
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

        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>> ContainerHeavyLift_ListByOwnerID(int OwnerID)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended> ContainerHeavyLiftDb = await (from ContainerHeavyLift in _dbContext.ContainerHeavyLift.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.OwnerID == OwnerID || x.OwnerID == 0))
                                                                                                            join site in _dbContext.Site.AsNoTracking()
                                                                                                                on ContainerHeavyLift.SiteID equals site.ID into sitePR
                                                                                                            from SPR in sitePR.DefaultIfEmpty()
                                                                                                            join company in _dbContext.Company.AsNoTracking()
                                                                                                        on ContainerHeavyLift.CompanyID equals company.ID into companyPR
                                                                                                            from CPR in companyPR.DefaultIfEmpty()
                                                                                                            join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                        on ContainerHeavyLift.OwnerID equals Owner.ID into OwnerPR
                                                                                                            from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                            join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                        on ContainerHeavyLift.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                                            from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                            join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on ContainerHeavyLift.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                            from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                            select new ContainerHeavyLift.dbRowExtended
                                                                                                            {
                                                                                                                ID = ContainerHeavyLift.ID,
                                                                                                                CompanyID = ContainerHeavyLift.CompanyID,
                                                                                                                OwnerID = ContainerHeavyLift.OwnerID,
                                                                                                                SiteID = ContainerHeavyLift.SiteID,
                                                                                                                MinWeight = ContainerHeavyLift.MinWeight,
                                                                                                                MinLength = ContainerHeavyLift.MinLength,
                                                                                                                Percentage = ContainerHeavyLift.Percentage,
                                                                                                                RateCollectionRateChargeCode = ContainerHeavyLift.RateCollectionRateChargeCode,
                                                                                                                DeletedFlag = ContainerHeavyLift.DeletedFlag,
                                                                                                                CreatedDateTime = ContainerHeavyLift.CreatedDateTime,
                                                                                                                CreatedMethod = ContainerHeavyLift.CreatedMethod,
                                                                                                                CreatedUserID = ContainerHeavyLift.CreatedUserID,
                                                                                                                LastAmendedDateTime = ContainerHeavyLift.LastAmendedDateTime,
                                                                                                                LastAmendedMethod = ContainerHeavyLift.LastAmendedMethod,
                                                                                                                LastAmendedUserID = ContainerHeavyLift.LastAmendedUserID,

                                                                                                                Site_PublicID = SPR.PublicID,
                                                                                                                Company_ShortName = CPR.ShortName,
                                                                                                                Owner_Name = OPR.Name,
                                                                                                                RateCode_RateChargeCode = RCPR.Description,
                                                                                                                Created_UserPublicID = SU.PublicID
                                                                                                            }).ToListAsync();

                if (ContainerHeavyLiftDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>(new List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>(ContainerHeavyLiftDb);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_ListByOwnerID - " + OwnerID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>> ContainerHeavyLift_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended ContainerHeavyLiftDb = await (from ContainerHeavyLift in _dbContext.ContainerHeavyLift.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).Take(1)
                                                                                                      join site in _dbContext.Site.AsNoTracking()
                                                                                                          on ContainerHeavyLift.SiteID equals site.ID into sitePR
                                                                                                      from SPR in sitePR.DefaultIfEmpty()
                                                                                                      join company in _dbContext.Company.AsNoTracking()
                                                                                                  on ContainerHeavyLift.CompanyID equals company.ID into companyPR
                                                                                                      from CPR in companyPR.DefaultIfEmpty()
                                                                                                      join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                  on ContainerHeavyLift.OwnerID equals Owner.ID into OwnerPR
                                                                                                      from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                      join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                  on ContainerHeavyLift.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                                      from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                      join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on ContainerHeavyLift.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                      from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                      select new ContainerHeavyLift.dbRowExtended
                                                                                                      {
                                                                                                          ID = ContainerHeavyLift.ID,
                                                                                                          CompanyID = ContainerHeavyLift.CompanyID,
                                                                                                          OwnerID = ContainerHeavyLift.OwnerID,
                                                                                                          SiteID = ContainerHeavyLift.SiteID,
                                                                                                          MinWeight = ContainerHeavyLift.MinWeight,
                                                                                                          MinLength = ContainerHeavyLift.MinLength,
                                                                                                          Percentage = ContainerHeavyLift.Percentage,
                                                                                                          RateCollectionRateChargeCode = ContainerHeavyLift.RateCollectionRateChargeCode,
                                                                                                          DeletedFlag = ContainerHeavyLift.DeletedFlag,
                                                                                                          CreatedDateTime = ContainerHeavyLift.CreatedDateTime,
                                                                                                          CreatedMethod = ContainerHeavyLift.CreatedMethod,
                                                                                                          CreatedUserID = ContainerHeavyLift.CreatedUserID,
                                                                                                          LastAmendedDateTime = ContainerHeavyLift.LastAmendedDateTime,
                                                                                                          LastAmendedMethod = ContainerHeavyLift.LastAmendedMethod,
                                                                                                          LastAmendedUserID = ContainerHeavyLift.LastAmendedUserID,

                                                                                                          Site_PublicID = SPR.PublicID,
                                                                                                          Company_ShortName = CPR.ShortName,
                                                                                                          Owner_Name = OPR.Name,
                                                                                                          RateCode_RateChargeCode = RCPR.Description,
                                                                                                          Created_UserPublicID = SU.PublicID
                                                                                                      }).FirstOrDefaultAsync();

                if (ContainerHeavyLiftDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>(new List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>(new List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended> { ContainerHeavyLiftDb });
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ContainerHeavyLift.dbRow>> ContainerHeavyLift_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ContainerHeavyLift.dbRow ContainerHeavyLiftDb = await _dbContext.ContainerHeavyLift.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (ContainerHeavyLiftDb == null)
                    return Result.Failure<wbm_common.DataObjects.ContainerHeavyLift.dbRow>("ContainerHeavyLift not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ContainerHeavyLift.dbRow>(ContainerHeavyLiftDb);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ContainerHeavyLift.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>> ContainerHeavyLift_List()
        {
            try
            {
                List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended> ContainerHeavyLifts = await (from ContainerHeavyLift in _dbContext.ContainerHeavyLift.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                           join site in _dbContext.Site.AsNoTracking()
                                                                                                               on ContainerHeavyLift.SiteID equals site.ID into sitePR
                                                                                                           from SPR in sitePR.DefaultIfEmpty()
                                                                                                           join company in _dbContext.Company.AsNoTracking()
                                                                                                       on ContainerHeavyLift.CompanyID equals company.ID into companyPR
                                                                                                           from CPR in companyPR.DefaultIfEmpty()
                                                                                                           join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                       on ContainerHeavyLift.OwnerID equals Owner.ID into OwnerPR
                                                                                                           from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                           join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                       on ContainerHeavyLift.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                                           from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                           join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on ContainerHeavyLift.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                           from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                           select new ContainerHeavyLift.dbRowExtended
                                                                                                           {
                                                                                                               ID = ContainerHeavyLift.ID,
                                                                                                               CompanyID = ContainerHeavyLift.CompanyID,
                                                                                                               OwnerID = ContainerHeavyLift.OwnerID,
                                                                                                               SiteID = ContainerHeavyLift.SiteID,
                                                                                                               MinWeight = ContainerHeavyLift.MinWeight,
                                                                                                               MinLength = ContainerHeavyLift.MinLength,
                                                                                                               Percentage = ContainerHeavyLift.Percentage,
                                                                                                               RateCollectionRateChargeCode = ContainerHeavyLift.RateCollectionRateChargeCode,
                                                                                                               DeletedFlag = ContainerHeavyLift.DeletedFlag,
                                                                                                               CreatedDateTime = ContainerHeavyLift.CreatedDateTime,
                                                                                                               CreatedMethod = ContainerHeavyLift.CreatedMethod,
                                                                                                               CreatedUserID = ContainerHeavyLift.CreatedUserID,
                                                                                                               LastAmendedDateTime = ContainerHeavyLift.LastAmendedDateTime,
                                                                                                               LastAmendedMethod = ContainerHeavyLift.LastAmendedMethod,
                                                                                                               LastAmendedUserID = ContainerHeavyLift.LastAmendedUserID,

                                                                                                               Site_PublicID = SPR.PublicID,
                                                                                                               Company_ShortName = CPR.ShortName,
                                                                                                               Owner_Name = OPR.Name,
                                                                                                               RateCode_RateChargeCode = RCPR.Description,
                                                                                                               Created_UserPublicID = SU.PublicID
                                                                                                           }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>(ContainerHeavyLifts);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>> ContainerHeavyLift_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ContainerHeavyLift_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended> ContainerHeavyLifts = await (from ContainerHeavyLift in _dbContext.ContainerHeavyLift.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                               join site in _dbContext.Site.AsNoTracking()
                                                                                                                   on ContainerHeavyLift.SiteID equals site.ID into sitePR
                                                                                                               from SPR in sitePR.DefaultIfEmpty()
                                                                                                               join company in _dbContext.Company.AsNoTracking()
                                                                                                           on ContainerHeavyLift.CompanyID equals company.ID into companyPR
                                                                                                               from CPR in companyPR.DefaultIfEmpty()
                                                                                                               join Owner in _dbContext.Owner.AsNoTracking()
                                                                                                           on ContainerHeavyLift.OwnerID equals Owner.ID into OwnerPR
                                                                                                               from OPR in OwnerPR.DefaultIfEmpty()
                                                                                                               join RateCode in _dbContext.RateCode.AsNoTracking()
                                                                                                           on ContainerHeavyLift.RateCollectionRateChargeCode equals RateCode.RateCode into RateCodePR
                                                                                                               from RCPR in RateCodePR.DefaultIfEmpty()
                                                                                                               join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                          on ContainerHeavyLift.CreatedUserID equals SecurityUser.ID into SecurityUserPR
                                                                                                               from SU in SecurityUserPR.DefaultIfEmpty()

                                                                                                               select new ContainerHeavyLift.dbRowExtended
                                                                                                               {
                                                                                                                   ID = ContainerHeavyLift.ID,
                                                                                                                   CompanyID = ContainerHeavyLift.CompanyID,
                                                                                                                   OwnerID = ContainerHeavyLift.OwnerID,
                                                                                                                   SiteID = ContainerHeavyLift.SiteID,
                                                                                                                   MinWeight = ContainerHeavyLift.MinWeight,
                                                                                                                   MinLength = ContainerHeavyLift.MinLength,
                                                                                                                   Percentage = ContainerHeavyLift.Percentage,
                                                                                                                   RateCollectionRateChargeCode = ContainerHeavyLift.RateCollectionRateChargeCode,
                                                                                                                   DeletedFlag = ContainerHeavyLift.DeletedFlag,
                                                                                                                   CreatedDateTime = ContainerHeavyLift.CreatedDateTime,
                                                                                                                   CreatedMethod = ContainerHeavyLift.CreatedMethod,
                                                                                                                   CreatedUserID = ContainerHeavyLift.CreatedUserID,
                                                                                                                   LastAmendedDateTime = ContainerHeavyLift.LastAmendedDateTime,
                                                                                                                   LastAmendedMethod = ContainerHeavyLift.LastAmendedMethod,
                                                                                                                   LastAmendedUserID = ContainerHeavyLift.LastAmendedUserID,

                                                                                                                   Site_PublicID = SPR.PublicID,
                                                                                                                   Company_ShortName = CPR.ShortName,
                                                                                                                   Owner_Name = OPR.Name,
                                                                                                                   RateCode_RateChargeCode = RCPR.Description,
                                                                                                                   Created_UserPublicID = SU.PublicID
                                                                                                               }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>(ContainerHeavyLifts);
                }
                catch (Exception ex)
                {
                    logger.Error("ContainerHeavyLift_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>> ContainerHeavyLift_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended> ContainerHeavyLifts = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>(ContainerHeavyLifts);
            }
            catch (Exception ex)
            {
                logger.Error("ContainerHeavyLift_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ContainerHeavyLift.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
