using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region InvoiceReportGroupMember
        public async Task<Result<int>> InvoiceReportGroupMember_Insert(wbm_common.DataObjects.InvoiceReportGroupMember.dbRow InvoiceReportGroupMember)
        {
            try
            {
                await _dbContext.InvoiceReportGroupMember.AddAsync(InvoiceReportGroupMember);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceReportGroupMember.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupMember_Insert - " + InvoiceReportGroupMember.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> InvoiceReportGroupMember_Update(wbm_common.DataObjects.InvoiceReportGroupMember.dbRow InvoiceReportGroupMember)
        {
            try
            {
                _dbContext.InvoiceReportGroupMember.Attach(InvoiceReportGroupMember);
                _dbContext.Entry(InvoiceReportGroupMember).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceReportGroupMember.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupMember_Update - " + InvoiceReportGroupMember.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRow>>> InvoiceReportGroupMember_ListByInvoiceReportGroupDefnID(int DefnID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRow> Groups = await _dbContext.InvoiceReportGroupMember.AsNoTracking().Where(x => !x.DeletedFlag && x.InvoiceReportGroupDefnID == DefnID).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRow>>(Groups);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_ListByInvoiceReportGroupDefnID: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRow>>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>> InvoiceReportGroupMember_ListBySingleInvoiceReportGroupID(int InvoiceReportGroupID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended> InvoiceReportGroupMemberDb = await (from InvoiceReportGroupMember in _dbContext.InvoiceReportGroupMember.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.InvoiceReportGroupID == InvoiceReportGroupID || x.InvoiceReportGroupID == 0))
                                                                                                                        join InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                                                                            on InvoiceReportGroupMember.InvoiceReportGroupID equals InvoiceReportGroup.ID into InvoiceReportGroupM
                                                                                                                        from IRG in InvoiceReportGroupM.DefaultIfEmpty()
                                                                                                                        join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                            on InvoiceReportGroupMember.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                                        from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                                        select new wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended
                                                                                                                        {
                                                                                                                            ID = InvoiceReportGroupMember.ID,
                                                                                                                            CompanyID = InvoiceReportGroupMember.CompanyID,
                                                                                                                            InvoiceReportGroupDefnID = InvoiceReportGroupMember.InvoiceReportGroupDefnID,
                                                                                                                            InvoiceReportGroupID = InvoiceReportGroupMember.InvoiceReportGroupID,
                                                                                                                            RateCollectionChargeCode = InvoiceReportGroupMember.RateCollectionChargeCode,
                                                                                                                            DeletedFlag = InvoiceReportGroupMember.DeletedFlag,
                                                                                                                            CreatedDateTime = InvoiceReportGroupMember.CreatedDateTime,
                                                                                                                            CreatedMethod = InvoiceReportGroupMember.CreatedMethod,
                                                                                                                            CreatedUserID = InvoiceReportGroupMember.CreatedUserID,
                                                                                                                            LastAmendedDateTime = InvoiceReportGroupMember.LastAmendedDateTime,
                                                                                                                            LastAmendedMethod = InvoiceReportGroupMember.LastAmendedMethod,
                                                                                                                            LastAmendedUserID = InvoiceReportGroupMember.LastAmendedUserID,

                                                                                                                            InvoiceReportGroupDefn_PublicID = IRGD.PublicID,
                                                                                                                            InvoiceReportGroup_PublicID = IRG.PublicID
                                                                                                                        }).ToListAsync();

                if (InvoiceReportGroupMemberDb == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(new List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(InvoiceReportGroupMemberDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupMember_ListBySingleInvoiceReportGroupID - " + InvoiceReportGroupID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>> InvoiceReportGroupMember_ListBySingleInvoiceReportGroupDefnID(int InvoiceReportGroupDefnID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended> InvoiceReportGroupMemberDb = await (from InvoiceReportGroupMember in _dbContext.InvoiceReportGroupMember.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.InvoiceReportGroupDefnID == InvoiceReportGroupDefnID || x.InvoiceReportGroupDefnID == 0))
                                                                                                                        join InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                                                                            on InvoiceReportGroupMember.InvoiceReportGroupID equals InvoiceReportGroup.ID into InvoiceReportGroupM
                                                                                                                        from IRG in InvoiceReportGroupM.DefaultIfEmpty()
                                                                                                                        join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                            on InvoiceReportGroupMember.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                                        from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                                        select new wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended
                                                                                                                        {
                                                                                                                            ID = InvoiceReportGroupMember.ID,
                                                                                                                            CompanyID = InvoiceReportGroupMember.CompanyID,
                                                                                                                            InvoiceReportGroupDefnID = InvoiceReportGroupMember.InvoiceReportGroupDefnID,
                                                                                                                            InvoiceReportGroupID = InvoiceReportGroupMember.InvoiceReportGroupID,
                                                                                                                            RateCollectionChargeCode = InvoiceReportGroupMember.RateCollectionChargeCode,
                                                                                                                            DeletedFlag = InvoiceReportGroupMember.DeletedFlag,
                                                                                                                            CreatedDateTime = InvoiceReportGroupMember.CreatedDateTime,
                                                                                                                            CreatedMethod = InvoiceReportGroupMember.CreatedMethod,
                                                                                                                            CreatedUserID = InvoiceReportGroupMember.CreatedUserID,
                                                                                                                            LastAmendedDateTime = InvoiceReportGroupMember.LastAmendedDateTime,
                                                                                                                            LastAmendedMethod = InvoiceReportGroupMember.LastAmendedMethod,
                                                                                                                            LastAmendedUserID = InvoiceReportGroupMember.LastAmendedUserID,

                                                                                                                            InvoiceReportGroupDefn_PublicID = IRGD.PublicID,
                                                                                                                            InvoiceReportGroup_PublicID = IRG.PublicID
                                                                                                                        }).ToListAsync();

                if (InvoiceReportGroupMemberDb == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(new List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(InvoiceReportGroupMemberDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupMember_ListBySingleInvoiceReportGroupDefnID - " + InvoiceReportGroupDefnID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>> InvoiceReportGroupMember_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended> InvoiceReportGroupMemberDb = await (from InvoiceReportGroupMember in _dbContext.InvoiceReportGroupMember.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                                        join InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                                                                            on InvoiceReportGroupMember.InvoiceReportGroupID equals InvoiceReportGroup.ID into InvoiceReportGroupM
                                                                                                                        from IRG in InvoiceReportGroupM.DefaultIfEmpty()
                                                                                                                        join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                            on InvoiceReportGroupMember.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                                        from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                                        select new wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended
                                                                                                                        {
                                                                                                                            ID = InvoiceReportGroupMember.ID,
                                                                                                                            CompanyID = InvoiceReportGroupMember.CompanyID,
                                                                                                                            InvoiceReportGroupDefnID = InvoiceReportGroupMember.InvoiceReportGroupDefnID,
                                                                                                                            InvoiceReportGroupID = InvoiceReportGroupMember.InvoiceReportGroupID,
                                                                                                                            RateCollectionChargeCode = InvoiceReportGroupMember.RateCollectionChargeCode,
                                                                                                                            DeletedFlag = InvoiceReportGroupMember.DeletedFlag,
                                                                                                                            CreatedDateTime = InvoiceReportGroupMember.CreatedDateTime,
                                                                                                                            CreatedMethod = InvoiceReportGroupMember.CreatedMethod,
                                                                                                                            CreatedUserID = InvoiceReportGroupMember.CreatedUserID,
                                                                                                                            LastAmendedDateTime = InvoiceReportGroupMember.LastAmendedDateTime,
                                                                                                                            LastAmendedMethod = InvoiceReportGroupMember.LastAmendedMethod,
                                                                                                                            LastAmendedUserID = InvoiceReportGroupMember.LastAmendedUserID,

                                                                                                                            InvoiceReportGroupDefn_PublicID = IRGD.PublicID,
                                                                                                                            InvoiceReportGroup_PublicID = IRG.PublicID
                                                                                                                        }).ToListAsync();

                if (InvoiceReportGroupMemberDb == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(new List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(InvoiceReportGroupMemberDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupMember_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceReportGroupMember.dbRow>> InvoiceReportGroupMember_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceReportGroupMember.dbRow InvoiceReportGroupMemberDb = await _dbContext.InvoiceReportGroupMember.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (InvoiceReportGroupMemberDb == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceReportGroupMember.dbRow>("InvoiceReportGroupMember not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceReportGroupMember.dbRow>(InvoiceReportGroupMemberDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupMember_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceReportGroupMember.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>> InvoiceReportGroupMember_SearchByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended> InvoiceReportGroupMemberDb = await (from InvoiceReportGroupMember in _dbContext.InvoiceReportGroupMember.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                                  join InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                                                                      on InvoiceReportGroupMember.InvoiceReportGroupID equals InvoiceReportGroup.ID into InvoiceReportGroupM
                                                                                                                  from IRG in InvoiceReportGroupM.DefaultIfEmpty()
                                                                                                                  join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                      on InvoiceReportGroupMember.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                                  from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                                  select new wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended
                                                                                                                  {
                                                                                                                      ID = InvoiceReportGroupMember.ID,
                                                                                                                      CompanyID = InvoiceReportGroupMember.CompanyID,
                                                                                                                      InvoiceReportGroupDefnID = InvoiceReportGroupMember.InvoiceReportGroupDefnID,
                                                                                                                      InvoiceReportGroupID = InvoiceReportGroupMember.InvoiceReportGroupID,
                                                                                                                      RateCollectionChargeCode = InvoiceReportGroupMember.RateCollectionChargeCode,
                                                                                                                      DeletedFlag = InvoiceReportGroupMember.DeletedFlag,
                                                                                                                      CreatedDateTime = InvoiceReportGroupMember.CreatedDateTime,
                                                                                                                      CreatedMethod = InvoiceReportGroupMember.CreatedMethod,
                                                                                                                      CreatedUserID = InvoiceReportGroupMember.CreatedUserID,
                                                                                                                      LastAmendedDateTime = InvoiceReportGroupMember.LastAmendedDateTime,
                                                                                                                      LastAmendedMethod = InvoiceReportGroupMember.LastAmendedMethod,
                                                                                                                      LastAmendedUserID = InvoiceReportGroupMember.LastAmendedUserID,

                                                                                                                      InvoiceReportGroupDefn_PublicID = IRGD.PublicID,
                                                                                                                      InvoiceReportGroup_PublicID = IRG.PublicID
                                                                                                                  }).ToListAsync();

                if (InvoiceReportGroupMemberDb == null)
                    return Result.Failure<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>("InvoiceReportGroupMember not found : " + ID.ToString());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(InvoiceReportGroupMemberDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupMember_SearchByPublicID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>> InvoiceReportGroupMember_List()
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended> InvoiceReportGroupMembers = await (from InvoiceReportGroupMember in _dbContext.InvoiceReportGroupMember.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                                       join InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                                                                           on InvoiceReportGroupMember.InvoiceReportGroupID equals InvoiceReportGroup.ID into InvoiceReportGroupM
                                                                                                                       from IRG in InvoiceReportGroupM.DefaultIfEmpty()
                                                                                                                       join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                           on InvoiceReportGroupMember.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                                       from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                                       select new wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended
                                                                                                                       {
                                                                                                                           ID = InvoiceReportGroupMember.ID,
                                                                                                                           CompanyID = InvoiceReportGroupMember.CompanyID,
                                                                                                                           InvoiceReportGroupDefnID = InvoiceReportGroupMember.InvoiceReportGroupDefnID,
                                                                                                                           InvoiceReportGroupID = InvoiceReportGroupMember.InvoiceReportGroupID,
                                                                                                                           RateCollectionChargeCode = InvoiceReportGroupMember.RateCollectionChargeCode,
                                                                                                                           DeletedFlag = InvoiceReportGroupMember.DeletedFlag,
                                                                                                                           CreatedDateTime = InvoiceReportGroupMember.CreatedDateTime,
                                                                                                                           CreatedMethod = InvoiceReportGroupMember.CreatedMethod,
                                                                                                                           CreatedUserID = InvoiceReportGroupMember.CreatedUserID,
                                                                                                                           LastAmendedDateTime = InvoiceReportGroupMember.LastAmendedDateTime,
                                                                                                                           LastAmendedMethod = InvoiceReportGroupMember.LastAmendedMethod,
                                                                                                                           LastAmendedUserID = InvoiceReportGroupMember.LastAmendedUserID,

                                                                                                                           InvoiceReportGroupDefn_PublicID = IRGD.PublicID,
                                                                                                                           InvoiceReportGroup_PublicID = IRG.PublicID
                                                                                                                       }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(InvoiceReportGroupMembers);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupMember_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>> InvoiceReportGroupMember_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("InvoiceReportGroupMember_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended> InvoiceReportGroupMembers = await (from InvoiceReportGroupMember in _dbContext.InvoiceReportGroupMember.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                                           join InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                                                                               on InvoiceReportGroupMember.InvoiceReportGroupID equals InvoiceReportGroup.ID into InvoiceReportGroupM
                                                                                                                           from IRG in InvoiceReportGroupM.DefaultIfEmpty()
                                                                                                                           join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                               on InvoiceReportGroupMember.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                                           from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                                           select new wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended
                                                                                                                           {
                                                                                                                               ID = InvoiceReportGroupMember.ID,
                                                                                                                               CompanyID = InvoiceReportGroupMember.CompanyID,
                                                                                                                               InvoiceReportGroupDefnID = InvoiceReportGroupMember.InvoiceReportGroupDefnID,
                                                                                                                               InvoiceReportGroupID = InvoiceReportGroupMember.InvoiceReportGroupID,
                                                                                                                               RateCollectionChargeCode = InvoiceReportGroupMember.RateCollectionChargeCode,
                                                                                                                               DeletedFlag = InvoiceReportGroupMember.DeletedFlag,
                                                                                                                               CreatedDateTime = InvoiceReportGroupMember.CreatedDateTime,
                                                                                                                               CreatedMethod = InvoiceReportGroupMember.CreatedMethod,
                                                                                                                               CreatedUserID = InvoiceReportGroupMember.CreatedUserID,
                                                                                                                               LastAmendedDateTime = InvoiceReportGroupMember.LastAmendedDateTime,
                                                                                                                               LastAmendedMethod = InvoiceReportGroupMember.LastAmendedMethod,
                                                                                                                               LastAmendedUserID = InvoiceReportGroupMember.LastAmendedUserID,

                                                                                                                               InvoiceReportGroupDefn_PublicID = IRGD.PublicID,
                                                                                                                               InvoiceReportGroup_PublicID = IRG.PublicID
                                                                                                                           }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(InvoiceReportGroupMembers);
                }
                catch (Exception ex)
                {
                    logger.Error("InvoiceReportGroupMember_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>> InvoiceReportGroupMember_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended> InvoiceReportGroupMembers = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>(InvoiceReportGroupMembers);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupMember_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>>("Internal Exception occured");
            }
        }

        #endregion

        #region InvoiceReportGroup
        public async Task<Result<int>> InvoiceReportGroup_Insert(wbm_common.DataObjects.InvoiceReportGroup.dbRow InvoiceReportGroup)
        {
            try
            {
                await _dbContext.InvoiceReportGroup.AddAsync(InvoiceReportGroup);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceReportGroup.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_Insert - " + InvoiceReportGroup.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> InvoiceReportGroup_Update(wbm_common.DataObjects.InvoiceReportGroup.dbRow InvoiceReportGroup)
        {
            try
            {
                _dbContext.InvoiceReportGroup.Attach(InvoiceReportGroup);
                _dbContext.Entry(InvoiceReportGroup).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceReportGroup.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_Update - " + InvoiceReportGroup.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRow>>> InvoiceReportGroup_ListByInvoiceReportGroupDefnID(int DefnID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroup.dbRow> Groups = await _dbContext.InvoiceReportGroup.AsNoTracking().Where(x => !x.DeletedFlag && x.InvoiceReportGroupDefnID == DefnID).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRow>>(Groups);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_ListByInvoiceReportGroupDefnID: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroup.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>> InvoiceReportGroup_ListBySingleInvoiceReportGroupDefnID(int InvoiceReportGroupDefnID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended> InvoiceReportGroupDb = await (from InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && (x.InvoiceReportGroupDefnID == InvoiceReportGroupDefnID || x.InvoiceReportGroupDefnID == 0))
                                                                                                            join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                on InvoiceReportGroup.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                            from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                            select new wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended
                                                                                                            {
                                                                                                                ID = InvoiceReportGroup.ID,
                                                                                                                CompanyID = InvoiceReportGroup.CompanyID,
                                                                                                                InvoiceReportGroupDefnID = InvoiceReportGroup.InvoiceReportGroupDefnID,
                                                                                                                PublicID = InvoiceReportGroup.PublicID,
                                                                                                                TransactionSourceID = InvoiceReportGroup.TransactionSourceID,
                                                                                                                GroupName = InvoiceReportGroup.GroupName,
                                                                                                                SortOrder = InvoiceReportGroup.SortOrder,
                                                                                                                DeletedFlag = InvoiceReportGroup.DeletedFlag,
                                                                                                                CreatedDateTime = InvoiceReportGroup.CreatedDateTime,
                                                                                                                CreatedMethod = InvoiceReportGroup.CreatedMethod,
                                                                                                                CreatedUserID = InvoiceReportGroup.CreatedUserID,
                                                                                                                LastAmendedDateTime = InvoiceReportGroup.LastAmendedDateTime,
                                                                                                                LastAmendedMethod = InvoiceReportGroup.LastAmendedMethod,
                                                                                                                LastAmendedUserID = InvoiceReportGroup.LastAmendedUserID,

                                                                                                                InvoiceReportGroupDefn_PublicID = IRGD.PublicID
                                                                                                            }).ToListAsync();

                if (InvoiceReportGroupDb == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>(new List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>(InvoiceReportGroupDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_ListBySingleInvoiceReportGroupDefnID - " + InvoiceReportGroupDefnID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>> InvoiceReportGroup_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended> InvoiceReportGroupDb = await (from InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                            join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                on InvoiceReportGroup.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                            from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                            select new wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended
                                                                                                            {
                                                                                                                ID = InvoiceReportGroup.ID,
                                                                                                                CompanyID = InvoiceReportGroup.CompanyID,
                                                                                                                InvoiceReportGroupDefnID = InvoiceReportGroup.InvoiceReportGroupDefnID,
                                                                                                                PublicID = InvoiceReportGroup.PublicID,
                                                                                                                TransactionSourceID = InvoiceReportGroup.TransactionSourceID,
                                                                                                                GroupName = InvoiceReportGroup.GroupName,
                                                                                                                SortOrder = InvoiceReportGroup.SortOrder,
                                                                                                                DeletedFlag = InvoiceReportGroup.DeletedFlag,
                                                                                                                CreatedDateTime = InvoiceReportGroup.CreatedDateTime,
                                                                                                                CreatedMethod = InvoiceReportGroup.CreatedMethod,
                                                                                                                CreatedUserID = InvoiceReportGroup.CreatedUserID,
                                                                                                                LastAmendedDateTime = InvoiceReportGroup.LastAmendedDateTime,
                                                                                                                LastAmendedMethod = InvoiceReportGroup.LastAmendedMethod,
                                                                                                                LastAmendedUserID = InvoiceReportGroup.LastAmendedUserID,

                                                                                                                InvoiceReportGroupDefn_PublicID = IRGD.PublicID
                                                                                                            }).ToListAsync();

                if (InvoiceReportGroupDb == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>(new List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>(InvoiceReportGroupDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceReportGroup.dbRow>> InvoiceReportGroup_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceReportGroup.dbRow InvoiceReportGroupDb = await _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (InvoiceReportGroupDb == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceReportGroup.dbRow>("InvoiceReportGroup not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceReportGroup.dbRow>(InvoiceReportGroupDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceReportGroup.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>> InvoiceReportGroup_SearchByPublicID(string PublicID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended> InvoiceReportGroupDb = await (from InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.PublicID == PublicID)
                                                                                                            join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                on InvoiceReportGroup.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                            from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                            select new wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended
                                                                                                            {
                                                                                                                ID = InvoiceReportGroup.ID,
                                                                                                                CompanyID = InvoiceReportGroup.CompanyID,
                                                                                                                InvoiceReportGroupDefnID = InvoiceReportGroup.InvoiceReportGroupDefnID,
                                                                                                                PublicID = InvoiceReportGroup.PublicID,
                                                                                                                TransactionSourceID = InvoiceReportGroup.TransactionSourceID,
                                                                                                                GroupName = InvoiceReportGroup.GroupName,
                                                                                                                SortOrder = InvoiceReportGroup.SortOrder,
                                                                                                                DeletedFlag = InvoiceReportGroup.DeletedFlag,
                                                                                                                CreatedDateTime = InvoiceReportGroup.CreatedDateTime,
                                                                                                                CreatedMethod = InvoiceReportGroup.CreatedMethod,
                                                                                                                CreatedUserID = InvoiceReportGroup.CreatedUserID,
                                                                                                                LastAmendedDateTime = InvoiceReportGroup.LastAmendedDateTime,
                                                                                                                LastAmendedMethod = InvoiceReportGroup.LastAmendedMethod,
                                                                                                                LastAmendedUserID = InvoiceReportGroup.LastAmendedUserID,

                                                                                                                InvoiceReportGroupDefn_PublicID = IRGD.PublicID
                                                                                                            }).ToListAsync();

                if (InvoiceReportGroupDb == null)
                    return Result.Failure<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("InvoiceReportGroup not found : " + PublicID);
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>(InvoiceReportGroupDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>> InvoiceReportGroup_SearchByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended> InvoiceReportGroupDb = await (from InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                                            join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                on InvoiceReportGroup.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                            from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                            select new wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended
                                                                                                            {
                                                                                                                ID = InvoiceReportGroup.ID,
                                                                                                                CompanyID = InvoiceReportGroup.CompanyID,
                                                                                                                InvoiceReportGroupDefnID = InvoiceReportGroup.InvoiceReportGroupDefnID,
                                                                                                                PublicID = InvoiceReportGroup.PublicID,
                                                                                                                TransactionSourceID = InvoiceReportGroup.TransactionSourceID,
                                                                                                                GroupName = InvoiceReportGroup.GroupName,
                                                                                                                SortOrder = InvoiceReportGroup.SortOrder,
                                                                                                                DeletedFlag = InvoiceReportGroup.DeletedFlag,
                                                                                                                CreatedDateTime = InvoiceReportGroup.CreatedDateTime,
                                                                                                                CreatedMethod = InvoiceReportGroup.CreatedMethod,
                                                                                                                CreatedUserID = InvoiceReportGroup.CreatedUserID,
                                                                                                                LastAmendedDateTime = InvoiceReportGroup.LastAmendedDateTime,
                                                                                                                LastAmendedMethod = InvoiceReportGroup.LastAmendedMethod,
                                                                                                                LastAmendedUserID = InvoiceReportGroup.LastAmendedUserID,

                                                                                                                InvoiceReportGroupDefn_PublicID = IRGD.PublicID
                                                                                                            }).ToListAsync();

                if (InvoiceReportGroupDb == null)
                    return Result.Failure<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("InvoiceReportGroup not found : " + ID.ToString());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>(InvoiceReportGroupDb);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>> InvoiceReportGroup_List()
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended> InvoiceReportGroups = await (from InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                           join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                               on InvoiceReportGroup.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                           from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                           select new wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended
                                                                                                           {
                                                                                                               ID = InvoiceReportGroup.ID,
                                                                                                               CompanyID = InvoiceReportGroup.CompanyID,
                                                                                                               InvoiceReportGroupDefnID = InvoiceReportGroup.InvoiceReportGroupDefnID,
                                                                                                               PublicID = InvoiceReportGroup.PublicID,
                                                                                                               TransactionSourceID = InvoiceReportGroup.TransactionSourceID,
                                                                                                               GroupName = InvoiceReportGroup.GroupName,
                                                                                                               SortOrder = InvoiceReportGroup.SortOrder,
                                                                                                               DeletedFlag = InvoiceReportGroup.DeletedFlag,
                                                                                                               CreatedDateTime = InvoiceReportGroup.CreatedDateTime,
                                                                                                               CreatedMethod = InvoiceReportGroup.CreatedMethod,
                                                                                                               CreatedUserID = InvoiceReportGroup.CreatedUserID,
                                                                                                               LastAmendedDateTime = InvoiceReportGroup.LastAmendedDateTime,
                                                                                                               LastAmendedMethod = InvoiceReportGroup.LastAmendedMethod,
                                                                                                               LastAmendedUserID = InvoiceReportGroup.LastAmendedUserID,

                                                                                                               InvoiceReportGroupDefn_PublicID = IRGD.PublicID
                                                                                                           }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>(InvoiceReportGroups);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>> InvoiceReportGroup_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("InvoiceReportGroup_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended> InvoiceReportGroups = await (from InvoiceReportGroup in _dbContext.InvoiceReportGroup.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && IDs.Contains(x.ID))
                                                                                                               join InvoiceReportGroupDefn in _dbContext.InvoiceReportGroupDefn.AsNoTracking()
                                                                                                                   on InvoiceReportGroup.InvoiceReportGroupDefnID equals InvoiceReportGroupDefn.ID into InvoiceReportGroupDefnM
                                                                                                               from IRGD in InvoiceReportGroupDefnM.DefaultIfEmpty()
                                                                                                               select new wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended
                                                                                                               {
                                                                                                                   ID = InvoiceReportGroup.ID,
                                                                                                                   CompanyID = InvoiceReportGroup.CompanyID,
                                                                                                                   InvoiceReportGroupDefnID = InvoiceReportGroup.InvoiceReportGroupDefnID,
                                                                                                                   PublicID = InvoiceReportGroup.PublicID,
                                                                                                                   TransactionSourceID = InvoiceReportGroup.TransactionSourceID,
                                                                                                                   GroupName = InvoiceReportGroup.GroupName,
                                                                                                                   SortOrder = InvoiceReportGroup.SortOrder,
                                                                                                                   DeletedFlag = InvoiceReportGroup.DeletedFlag,
                                                                                                                   CreatedDateTime = InvoiceReportGroup.CreatedDateTime,
                                                                                                                   CreatedMethod = InvoiceReportGroup.CreatedMethod,
                                                                                                                   CreatedUserID = InvoiceReportGroup.CreatedUserID,
                                                                                                                   LastAmendedDateTime = InvoiceReportGroup.LastAmendedDateTime,
                                                                                                                   LastAmendedMethod = InvoiceReportGroup.LastAmendedMethod,
                                                                                                                   LastAmendedUserID = InvoiceReportGroup.LastAmendedUserID,

                                                                                                                   InvoiceReportGroupDefn_PublicID = IRGD.PublicID
                                                                                                               }).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>(InvoiceReportGroups);
                }
                catch (Exception ex)
                {
                    logger.Error("InvoiceReportGroup_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>> InvoiceReportGroup_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended> InvoiceReportGroups = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>(InvoiceReportGroups);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroup_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>>("Internal Exception occured");
            }
        }

        #endregion

        #region InvoiceReportGroupDefn
        public async Task<Result<int>> InvoiceReportGroupDefn_Insert(wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow InvoiceReportGroupDefn)
        {
            try
            {
                await _dbContext.InvoiceReportGroupDefn.AddAsync(InvoiceReportGroupDefn);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceReportGroupDefn.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupDefn_Insert - " + InvoiceReportGroupDefn.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> InvoiceReportGroupDefn_Update(wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow InvoiceReportGroupDefn)
        {
            try
            {
                _dbContext.InvoiceReportGroupDefn.Attach(InvoiceReportGroupDefn);
                _dbContext.Entry(InvoiceReportGroupDefn).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(InvoiceReportGroupDefn.ID);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupDefn_Update - " + InvoiceReportGroupDefn.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>> InvoiceReportGroupDefn_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow InvoiceReportGroupDefn = await _dbContext.InvoiceReportGroupDefn.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (InvoiceReportGroupDefn == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>("InvoiceReportGroupDefn not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>(InvoiceReportGroupDefn);

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupDefn_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>> InvoiceReportGroupDefn_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow InvoiceReportGroupDefn = await _dbContext.InvoiceReportGroupDefn.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (InvoiceReportGroupDefn == null)
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>(new List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>(new List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow> { InvoiceReportGroupDefn });
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupDefn_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>> InvoiceReportGroupDefn_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow InvoiceReportGroupDefn = await _dbContext.InvoiceReportGroupDefn.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (InvoiceReportGroupDefn == null)
                    return Result.Failure<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>("InvoiceReportGroupDefn not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>(InvoiceReportGroupDefn);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupDefn_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>> InvoiceReportGroupDefn_List()
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow> InvoiceReportGroupDefns = await _dbContext.InvoiceReportGroupDefn.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>(InvoiceReportGroupDefns);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupDefn_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>> InvoiceReportGroupDefn_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("InvoiceReportGroupDefn_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow> InvoiceReportGroupDefns = await _dbContext.InvoiceReportGroupDefn.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>(InvoiceReportGroupDefns);
                }
                catch (Exception ex)
                {
                    logger.Error("InvoiceReportGroupDefn_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>> InvoiceReportGroupDefn_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow> InvoiceReportGroupDefns = await _dbContext.InvoiceReportGroupDefn.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>(InvoiceReportGroupDefns);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportGroupDefn_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
