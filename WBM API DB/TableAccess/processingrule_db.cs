using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region Process
        public async Task<Result<int>> Process_Insert(wbm_common.DataObjects.Process.dbRow Process)
        {
            try
            {
                await _dbContext.Process.AddAsync(Process);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Process.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Process_Insert - " + Process.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> Process_Update(wbm_common.DataObjects.Process.dbRow Process)
        {
            try
            {
                _dbContext.Process.Attach(Process);
                _dbContext.Entry(Process).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(Process.ID);
            }
            catch (Exception ex)
            {
                logger.Error("Process_Update - " + Process.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.Process.dbRow>> Process_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.Process.dbRow Process = await _dbContext.Process.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (Process == null)
                    return Result.Failure<wbm_common.DataObjects.Process.dbRow>("Process not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.Process.dbRow>(Process);

            }
            catch (Exception ex)
            {
                logger.Error("Process_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Process.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.Process.dbRow>>> Process_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Process.dbRow Process = await _dbContext.Process.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (Process == null)
                    return Result.Success<List<wbm_common.DataObjects.Process.dbRow>>(new List<wbm_common.DataObjects.Process.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.Process.dbRow>>(new List<wbm_common.DataObjects.Process.dbRow> { Process });
            }
            catch (Exception ex)
            {
                logger.Error("Process_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Process.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.Process.dbRow>> Process_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.Process.dbRow Process = await _dbContext.Process.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (Process == null)
                    return Result.Failure<wbm_common.DataObjects.Process.dbRow>("Process not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.Process.dbRow>(Process);
            }
            catch (Exception ex)
            {
                logger.Error("Process_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.Process.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Process.dbRow>>> Process_List()
        {
            try
            {
                List<wbm_common.DataObjects.Process.dbRow> Processs = await _dbContext.Process.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Process.dbRow>>(Processs);
            }
            catch (Exception ex)
            {
                logger.Error("Process_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Process.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Process.dbRow>>> Process_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Process_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Process.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Process.dbRow> Processs = await _dbContext.Process.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Process.dbRow>>(Processs);
                }
                catch (Exception ex)
                {
                    logger.Error("Process_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Process.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Process.dbRow>>> Process_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.Process.dbRow> Processs = await _dbContext.Process.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.Process.dbRow>>(Processs);
            }
            catch (Exception ex)
            {
                logger.Error("Process_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Process.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region ProcessCategory
        public async Task<Result<int>> ProcessCategory_Insert(wbm_common.DataObjects.ProcessCategory.dbRow ProcessCategory)
        {
            try
            {
                await _dbContext.ProcessCategory.AddAsync(ProcessCategory);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessCategory.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessCategory_Insert - " + ProcessCategory.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ProcessCategory_Update(wbm_common.DataObjects.ProcessCategory.dbRow ProcessCategory)
        {
            try
            {
                _dbContext.ProcessCategory.Attach(ProcessCategory);
                _dbContext.Entry(ProcessCategory).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessCategory.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessCategory_Update - " + ProcessCategory.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ProcessCategory.dbRow>> ProcessCategory_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessCategory.dbRow ProcessCategory = await _dbContext.ProcessCategory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ProcessCategory == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessCategory.dbRow>("ProcessCategory not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ProcessCategory.dbRow>(ProcessCategory);

            }
            catch (Exception ex)
            {
                logger.Error("ProcessCategory_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessCategory.dbRow>("Internal Exception occured");
            }
        }
        public async Task<Result<wbm_common.DataObjects.ProcessCategory.dbRow>> ProcessCategory_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ProcessCategory.dbRow ProcessCategory = await _dbContext.ProcessCategory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (ProcessCategory == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessCategory.dbRow>("ProcessCategory not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ProcessCategory.dbRow>(ProcessCategory);

            }
            catch (Exception ex)
            {
                logger.Error("ProcessCategory_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessCategory.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ProcessCategory.dbRow>>> ProcessCategory_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessCategory.dbRow ProcessCategory = await _dbContext.ProcessCategory.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ProcessCategory == null)
                    return Result.Success<List<wbm_common.DataObjects.ProcessCategory.dbRow>>(new List<wbm_common.DataObjects.ProcessCategory.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ProcessCategory.dbRow>>(new List<wbm_common.DataObjects.ProcessCategory.dbRow> { ProcessCategory });
            }
            catch (Exception ex)
            {
                logger.Error("ProcessCategory_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessCategory.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ProcessCategory.dbRow>> ProcessCategory_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessCategory.dbRow ProcessCategory = await _dbContext.ProcessCategory.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ProcessCategory == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessCategory.dbRow>("ProcessCategory not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ProcessCategory.dbRow>(ProcessCategory);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessCategory_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessCategory.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessCategory.dbRow>>> ProcessCategory_List()
        {
            try
            {
                List<wbm_common.DataObjects.ProcessCategory.dbRow> ProcessCategorys = await _dbContext.ProcessCategory.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ProcessCategory.dbRow>>(ProcessCategorys);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessCategory_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ProcessCategory.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessCategory.dbRow>>> ProcessCategory_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ProcessCategory.dbRow> ProcessCategorys = await _dbContext.ProcessCategory.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ProcessCategory.dbRow>>(ProcessCategorys);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessCategory_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessCategory.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region ProcessingRule
        public async Task<Result<int>> ProcessingRule_Insert(wbm_common.DataObjects.ProcessingRule.dbRow ProcessingRule)
        {
            try
            {
                await _dbContext.ProcessingRule.AddAsync(ProcessingRule);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessingRule.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_Insert - " + ProcessingRule.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ProcessingRule_Update(wbm_common.DataObjects.ProcessingRule.dbRow ProcessingRule)
        {
            try
            {
                _dbContext.ProcessingRule.Attach(ProcessingRule);
                _dbContext.Entry(ProcessingRule).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessingRule.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_Update - " + ProcessingRule.ID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // TODO the process matches here seem off so I reused one
        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ProcessingRule.dbRow>> ProcessingRule_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRule.dbRow ProcessingRuleDb = await _dbContext.ProcessingRule.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).FirstOrDefaultAsync();

                if (ProcessingRuleDb == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRule.dbRow>("ProcessingRule not found : " + ID);
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRule.dbRow>(ProcessingRuleDb);

            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRule.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>> ProcessingRule_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRule.dbRowExtended> ProcessingRuleDb = await (from ProcessingRule in _dbContext.ProcessingRule.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                          join site in _dbContext.Site.AsNoTracking()
                                                                                              on ProcessingRule.SiteID equals site.ID into sitePR 
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                          join company in _dbContext.Company.AsNoTracking()
                                                                                              on ProcessingRule.CompanyID equals company.ID into companyPR 
                                                                                                    from CPR in companyPR.DefaultIfEmpty()
                                                                                          join processingRuleDefn in _dbContext.ProcessingRuleDefn.AsNoTracking()
                                                                                              on ProcessingRule.ProcessingRuleDefnID equals processingRuleDefn.ID into processingRuleDefnPR 
                                                                                                    from PRDPR in processingRuleDefnPR.DefaultIfEmpty()
                                                                                          join processCategory in _dbContext.ProcessCategory.AsNoTracking()
                                                                                              on ProcessingRule.ProcessCategoryID equals processCategory.ID into processCategoryPR 
                                                                                                    from PRCPR in processCategoryPR.DefaultIfEmpty()
                                                                                          join process in _dbContext.Process.AsNoTracking()
                                                                                              on ProcessingRule.ProcessID equals process.ID into processPR from PPR in processPR.DefaultIfEmpty()
                                                                                          join rateFunction in _dbContext.RateFunction.AsNoTracking()
                                                                                              on ProcessingRule.RateFunctionID equals rateFunction.ID into rateFunctionPR 
                                                                                                    from RFPR in rateFunctionPR.DefaultIfEmpty()
                                                                                          join ProcessingRuleProcess in _dbContext.ProcessingRuleProcess.AsNoTracking()
                                                                                              on ProcessingRule.ProcessID equals ProcessingRuleProcess.ID into ProcessingRuleProcessPR 
                                                                                                    from PRPPR in ProcessingRuleProcessPR.DefaultIfEmpty()
                                                                                          join ProcessingRuleTriggerCondition in _dbContext.ProcessingRuleTriggerCondition.AsNoTracking()
                                                                                              on ProcessingRule.ProcessID equals ProcessingRuleTriggerCondition.ID into ProcessingRuleTriggerConditionPR 
                                                                                                    from PRTCPR in ProcessingRuleTriggerConditionPR.DefaultIfEmpty()

                                                                                          select new ProcessingRule.dbRowExtended
                                                                                          {
                                                                                              ID = ProcessingRule.ID,
                                                                                              CompanyID = ProcessingRule.CompanyID,
                                                                                              SiteID = ProcessingRule.SiteID,
                                                                                              ProcessingRuleDefnID = ProcessingRule.ProcessingRuleDefnID,
                                                                                              ProcessCategoryID = ProcessingRule.ProcessCategoryID,
                                                                                              ProcessID = ProcessingRule.ProcessID,
                                                                                              SortOrder = ProcessingRule.SortOrder,
                                                                                              RateFunctionID = ProcessingRule.RateFunctionID,
                                                                                              DeletedFlag = ProcessingRule.DeletedFlag,
                                                                                              CreatedDateTime = ProcessingRule.CreatedDateTime,
                                                                                              CreatedMethod = ProcessingRule.CreatedMethod,
                                                                                              CreatedUserID = ProcessingRule.CreatedUserID,
                                                                                              LastAmendedDateTime = ProcessingRule.LastAmendedDateTime,
                                                                                              LastAmendedMethod = ProcessingRule.LastAmendedMethod,
                                                                                              LastAmendedUserID = ProcessingRule.LastAmendedUserID,

                                                                                              Site_PublicID = SPR.PublicID,
                                                                                              Company_ShortName = CPR.ShortName,
                                                                                              ProcessingRuleDefn_PublicID = PRDPR.PublicID,
                                                                                              ProcessCategory_PublicID = PRCPR.PublicID,
                                                                                              Process_PublicID = PPR.PublicID,
                                                                                              RateFunction_PublicID = RFPR.PublicID,
                                                                                              ProcessingRuleTriggerCondition_PublicID = PRPPR.PublicID,
                                                                                              ProcessingRuleProcess_PublicID = PRTCPR.PublicID
                                                                                          }).ToListAsync();

                if (ProcessingRuleDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>(new List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>(ProcessingRuleDb);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>> ProcessingRule_ListByProcessingRuleDefnID(int ProcessingRuleDefnID)
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRule.dbRowExtended> ProcessingRuleDb = await (from ProcessingRule in _dbContext.ProcessingRule.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ProcessingRuleDefnID == ProcessingRuleDefnID)
                                                                                                    join site in _dbContext.Site.AsNoTracking()
                                                                                                        on ProcessingRule.SiteID equals site.ID into sitePR
                                                                                                    from SPR in sitePR.DefaultIfEmpty()
                                                                                                    join company in _dbContext.Company.AsNoTracking()
                                                                                                        on ProcessingRule.CompanyID equals company.ID into companyPR
                                                                                                    from CPR in companyPR.DefaultIfEmpty()
                                                                                                    join processingRuleDefn in _dbContext.ProcessingRuleDefn.AsNoTracking()
                                                                                                        on ProcessingRule.ProcessingRuleDefnID equals processingRuleDefn.ID into processingRuleDefnPR
                                                                                                    from PRDPR in processingRuleDefnPR.DefaultIfEmpty()
                                                                                                    join processCategory in _dbContext.ProcessCategory.AsNoTracking()
                                                                                                        on ProcessingRule.ProcessCategoryID equals processCategory.ID into processCategoryPR
                                                                                                    from PRCPR in processCategoryPR.DefaultIfEmpty()
                                                                                                    join process in _dbContext.Process.AsNoTracking()
                                                                                                        on ProcessingRule.ProcessID equals process.ID into processPR
                                                                                                    from PPR in processPR.DefaultIfEmpty()
                                                                                                    join rateFunction in _dbContext.RateFunction.AsNoTracking()
                                                                                                        on ProcessingRule.RateFunctionID equals rateFunction.ID into rateFunctionPR
                                                                                                    from RFPR in rateFunctionPR.DefaultIfEmpty()
                                                                                                    join ProcessingRuleProcess in _dbContext.ProcessingRuleProcess.AsNoTracking()
                                                                                                        on ProcessingRule.ProcessID equals ProcessingRuleProcess.ID into ProcessingRuleProcessPR
                                                                                                    from PRPPR in ProcessingRuleProcessPR.DefaultIfEmpty()
                                                                                                    join ProcessingRuleTriggerCondition in _dbContext.ProcessingRuleTriggerCondition.AsNoTracking()
                                                                                                        on ProcessingRule.ProcessID equals ProcessingRuleTriggerCondition.ID into ProcessingRuleTriggerConditionPR
                                                                                                    from PRTCPR in ProcessingRuleTriggerConditionPR.DefaultIfEmpty()

                                                                                                    select new ProcessingRule.dbRowExtended
                                                                                                    {
                                                                                                        ID = ProcessingRule.ID,
                                                                                                        CompanyID = ProcessingRule.CompanyID,
                                                                                                        SiteID = ProcessingRule.SiteID,
                                                                                                        ProcessingRuleDefnID = ProcessingRule.ProcessingRuleDefnID,
                                                                                                        ProcessCategoryID = ProcessingRule.ProcessCategoryID,
                                                                                                        ProcessID = ProcessingRule.ProcessID,
                                                                                                        SortOrder = ProcessingRule.SortOrder,
                                                                                                        RateFunctionID = ProcessingRule.RateFunctionID,
                                                                                                        DeletedFlag = ProcessingRule.DeletedFlag,
                                                                                                        CreatedDateTime = ProcessingRule.CreatedDateTime,
                                                                                                        CreatedMethod = ProcessingRule.CreatedMethod,
                                                                                                        CreatedUserID = ProcessingRule.CreatedUserID,
                                                                                                        LastAmendedDateTime = ProcessingRule.LastAmendedDateTime,
                                                                                                        LastAmendedMethod = ProcessingRule.LastAmendedMethod,
                                                                                                        LastAmendedUserID = ProcessingRule.LastAmendedUserID,

                                                                                                        Site_PublicID = SPR.PublicID,
                                                                                                        Company_ShortName = CPR.ShortName,
                                                                                                        ProcessingRuleDefn_PublicID = PRDPR.PublicID,
                                                                                                        ProcessCategory_PublicID = PRCPR.PublicID,
                                                                                                        Process_PublicID = PPR.PublicID,
                                                                                                        RateFunction_PublicID = RFPR.PublicID,
                                                                                                        ProcessingRuleTriggerCondition_PublicID = PRPPR.PublicID,
                                                                                                        ProcessingRuleProcess_PublicID = PRTCPR.PublicID
                                                                                                    }).ToListAsync();

                if (ProcessingRuleDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>(new List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>(ProcessingRuleDb);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_ListByProcessingRuleDefnID - " + ProcessingRuleDefnID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ProcessingRule.dbRow>> ProcessingRule_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRule.dbRow ProcessingRuleDb = await _dbContext.ProcessingRule.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (ProcessingRuleDb == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRule.dbRow>("ProcessingRule not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRule.dbRow>(ProcessingRuleDb);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRule.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>> ProcessingRule_List()
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRule.dbRowExtended> ProcessingRules = await (from ProcessingRule in _dbContext.ProcessingRule.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                                                   join site in _dbContext.Site.AsNoTracking()
                                                                                                       on ProcessingRule.SiteID equals site.ID into sitePR
                                                                                                   from SPR in sitePR.DefaultIfEmpty()
                                                                                                   join company in _dbContext.Company.AsNoTracking()
                                                                                                       on ProcessingRule.CompanyID equals company.ID into companyPR
                                                                                                   from CPR in companyPR.DefaultIfEmpty()
                                                                                                   join processingRuleDefn in _dbContext.ProcessingRuleDefn.AsNoTracking()
                                                                                                       on ProcessingRule.ProcessingRuleDefnID equals processingRuleDefn.ID into processingRuleDefnPR
                                                                                                   from PRDPR in processingRuleDefnPR.DefaultIfEmpty()
                                                                                                   join processCategory in _dbContext.ProcessCategory.AsNoTracking()
                                                                                                       on ProcessingRule.ProcessCategoryID equals processCategory.ID into processCategoryPR
                                                                                                   from PRCPR in processCategoryPR.DefaultIfEmpty()
                                                                                                   join process in _dbContext.Process.AsNoTracking()
                                                                                                       on ProcessingRule.ProcessID equals process.ID into processPR
                                                                                                   from PPR in processPR.DefaultIfEmpty()
                                                                                                   join rateFunction in _dbContext.RateFunction.AsNoTracking()
                                                                                                       on ProcessingRule.RateFunctionID equals rateFunction.ID into rateFunctionPR
                                                                                                   from RFPR in rateFunctionPR.DefaultIfEmpty()
                                                                                                   join ProcessingRuleProcess in _dbContext.ProcessingRuleProcess.AsNoTracking()
                                                                                                       on ProcessingRule.ProcessID equals ProcessingRuleProcess.ID into ProcessingRuleProcessPR
                                                                                                   from PRPPR in ProcessingRuleProcessPR.DefaultIfEmpty()
                                                                                                   join ProcessingRuleTriggerCondition in _dbContext.ProcessingRuleTriggerCondition.AsNoTracking()
                                                                                                       on ProcessingRule.ProcessID equals ProcessingRuleTriggerCondition.ID into ProcessingRuleTriggerConditionPR
                                                                                                   from PRTCPR in ProcessingRuleTriggerConditionPR.DefaultIfEmpty()

                                                                                                   select new ProcessingRule.dbRowExtended
                                                                                                   {
                                                                                                       ID = ProcessingRule.ID,
                                                                                                       CompanyID = ProcessingRule.CompanyID,
                                                                                                       SiteID = ProcessingRule.SiteID,
                                                                                                       ProcessingRuleDefnID = ProcessingRule.ProcessingRuleDefnID,
                                                                                                       ProcessCategoryID = ProcessingRule.ProcessCategoryID,
                                                                                                       ProcessID = ProcessingRule.ProcessID,
                                                                                                       SortOrder = ProcessingRule.SortOrder,
                                                                                                       RateFunctionID = ProcessingRule.RateFunctionID,
                                                                                                       DeletedFlag = ProcessingRule.DeletedFlag,
                                                                                                       CreatedDateTime = ProcessingRule.CreatedDateTime,
                                                                                                       CreatedMethod = ProcessingRule.CreatedMethod,
                                                                                                       CreatedUserID = ProcessingRule.CreatedUserID,
                                                                                                       LastAmendedDateTime = ProcessingRule.LastAmendedDateTime,
                                                                                                       LastAmendedMethod = ProcessingRule.LastAmendedMethod,
                                                                                                       LastAmendedUserID = ProcessingRule.LastAmendedUserID,

                                                                                                       Site_PublicID = SPR.PublicID,
                                                                                                       Company_ShortName = CPR.ShortName,
                                                                                                       ProcessingRuleDefn_PublicID = PRDPR.PublicID,
                                                                                                       ProcessCategory_PublicID = PRCPR.PublicID,
                                                                                                       Process_PublicID = PPR.PublicID,
                                                                                                       RateFunction_PublicID = RFPR.PublicID,
                                                                                                       ProcessingRuleTriggerCondition_PublicID = PRPPR.PublicID,
                                                                                                       ProcessingRuleProcess_PublicID = PRTCPR.PublicID
                                                                                                   }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>(ProcessingRules);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>("Internal Exception occured");
            }
        }

        // TODO this currently fails since trigger id stuff is unclear, will do last/later
        public async Task<Result<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>> ProcessingRule_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRule.dbRowExtended> ProcessingRules = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.ProcessingRule.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>(ProcessingRules);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRule.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion

        #region ProcessingRuleDefn
        public async Task<Result<int>> ProcessingRuleDefn_Insert(wbm_common.DataObjects.ProcessingRuleDefn.dbRow ProcessingRuleDefn)
        {
            try
            {
                await _dbContext.ProcessingRuleDefn.AddAsync(ProcessingRuleDefn);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessingRuleDefn.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleDefn_Insert - " + ProcessingRuleDefn.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ProcessingRuleDefn_Update(wbm_common.DataObjects.ProcessingRuleDefn.dbRow ProcessingRuleDefn)
        {
            try
            {
                _dbContext.ProcessingRuleDefn.Attach(ProcessingRuleDefn);
                _dbContext.Entry(ProcessingRuleDefn).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessingRuleDefn.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleDefn_Update - " + ProcessingRuleDefn.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>> ProcessingRuleDefn_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleDefn.dbRow ProcessingRuleDefn = await _dbContext.ProcessingRuleDefn.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (ProcessingRuleDefn == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>("ProcessingRuleDefn not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>(ProcessingRuleDefn);

            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleDefn_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>> ProcessingRuleDefn_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleDefn.dbRow ProcessingRuleDefn = await _dbContext.ProcessingRuleDefn.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ProcessingRuleDefn == null)
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>(new List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>(new List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow> { ProcessingRuleDefn });
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleDefn_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>> ProcessingRuleDefn_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleDefn.dbRow ProcessingRuleDefn = await _dbContext.ProcessingRuleDefn.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ProcessingRuleDefn == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>("ProcessingRuleDefn not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>(ProcessingRuleDefn);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleDefn_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>> ProcessingRuleDefn_List()
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow> ProcessingRuleDefns = await _dbContext.ProcessingRuleDefn.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>(ProcessingRuleDefns);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleDefn_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>> ProcessingRuleDefn_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ProcessingRuleDefn_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow> ProcessingRuleDefns = await _dbContext.ProcessingRuleDefn.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>(ProcessingRuleDefns);
                }
                catch (Exception ex)
                {
                    logger.Error("ProcessingRuleDefn_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>> ProcessingRuleDefn_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow> ProcessingRuleDefns = await _dbContext.ProcessingRuleDefn.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>(ProcessingRuleDefns);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleDefn_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region ProcessingRuleProcess
        public async Task<Result<int>> ProcessingRuleProcess_Insert(wbm_common.DataObjects.ProcessingRuleProcess.dbRow ProcessingRuleProcess)
        {
            try
            {
                await _dbContext.ProcessingRuleProcess.AddAsync(ProcessingRuleProcess);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessingRuleProcess.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleProcess_Insert - " + ProcessingRuleProcess.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ProcessingRuleProcess_Update(wbm_common.DataObjects.ProcessingRuleProcess.dbRow ProcessingRuleProcess)
        {
            try
            {
                _dbContext.ProcessingRuleProcess.Attach(ProcessingRuleProcess);
                _dbContext.Entry(ProcessingRuleProcess).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessingRuleProcess.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleProcess_Update - " + ProcessingRuleProcess.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>> ProcessingRuleProcess_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleProcess.dbRow ProcessingRuleProcess = await _dbContext.ProcessingRuleProcess.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (ProcessingRuleProcess == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>("ProcessingRuleProcess not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>(ProcessingRuleProcess);

            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleProcess_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>> ProcessingRuleProcess_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleProcess.dbRow ProcessingRuleProcess = await _dbContext.ProcessingRuleProcess.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ProcessingRuleProcess == null)
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>(new List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>(new List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow> { ProcessingRuleProcess });
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleProcess_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>> ProcessingRuleProcess_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleProcess.dbRow ProcessingRuleProcess = await _dbContext.ProcessingRuleProcess.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ProcessingRuleProcess == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>("ProcessingRuleProcess not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>(ProcessingRuleProcess);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleProcess_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>> ProcessingRuleProcess_List()
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow> ProcessingRuleProcesss = await _dbContext.ProcessingRuleProcess.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>(ProcessingRuleProcesss);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleProcess_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>> ProcessingRuleProcess_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ProcessingRuleProcess_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow> ProcessingRuleProcesss = await _dbContext.ProcessingRuleProcess.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>(ProcessingRuleProcesss);
                }
                catch (Exception ex)
                {
                    logger.Error("ProcessingRuleProcess_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>> ProcessingRuleProcess_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow> ProcessingRuleProcesss = await _dbContext.ProcessingRuleProcess.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>(ProcessingRuleProcesss);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleProcess_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region ProcessingRuleTriggerCondition
        public async Task<Result<int>> ProcessingRuleTriggerCondition_Insert(wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow ProcessingRuleTriggerCondition)
        {
            try
            {
                await _dbContext.ProcessingRuleTriggerCondition.AddAsync(ProcessingRuleTriggerCondition);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessingRuleTriggerCondition.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleTriggerCondition_Insert - " + ProcessingRuleTriggerCondition.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ProcessingRuleTriggerCondition_Update(wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow ProcessingRuleTriggerCondition)
        {
            try
            {
                _dbContext.ProcessingRuleTriggerCondition.Attach(ProcessingRuleTriggerCondition);
                _dbContext.Entry(ProcessingRuleTriggerCondition).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ProcessingRuleTriggerCondition.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleTriggerCondition_Update - " + ProcessingRuleTriggerCondition.PublicID + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>> ProcessingRuleTriggerCondition_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow ProcessingRuleTriggerCondition = await _dbContext.ProcessingRuleTriggerCondition.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (ProcessingRuleTriggerCondition == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>("ProcessingRuleTriggerCondition not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>(ProcessingRuleTriggerCondition);

            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleTriggerCondition_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>> ProcessingRuleTriggerCondition_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow ProcessingRuleTriggerCondition = await _dbContext.ProcessingRuleTriggerCondition.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ProcessingRuleTriggerCondition == null)
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>(new List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>(new List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow> { ProcessingRuleTriggerCondition });
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleTriggerCondition_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>> ProcessingRuleTriggerCondition_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow ProcessingRuleTriggerCondition = await _dbContext.ProcessingRuleTriggerCondition.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ProcessingRuleTriggerCondition == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>("ProcessingRuleTriggerCondition not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>(ProcessingRuleTriggerCondition);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleTriggerCondition_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>> ProcessingRuleTriggerCondition_List()
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow> ProcessingRuleTriggerConditions = await _dbContext.ProcessingRuleTriggerCondition.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>(ProcessingRuleTriggerConditions);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleTriggerCondition_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>> ProcessingRuleTriggerCondition_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ProcessingRuleTriggerCondition_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow> ProcessingRuleTriggerConditions = await _dbContext.ProcessingRuleTriggerCondition.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>(ProcessingRuleTriggerConditions);
                }
                catch (Exception ex)
                {
                    logger.Error("ProcessingRuleTriggerCondition_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>> ProcessingRuleTriggerCondition_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow> ProcessingRuleTriggerConditions = await _dbContext.ProcessingRuleTriggerCondition.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>(ProcessingRuleTriggerConditions);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleTriggerCondition_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
