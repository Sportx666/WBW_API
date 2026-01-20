using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
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

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ProcessingRule.dbRow>> ProcessingRule_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRule.dbRow ProcessingRule = await _dbContext.ProcessingRule.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ProcessingRule == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRule.dbRow>("ProcessingRule not found : " + ID);
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRule.dbRow>(ProcessingRule);

            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRule.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ProcessingRule.dbRow>>> ProcessingRuleList_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRule.dbRow ProcessingRule = await _dbContext.ProcessingRule.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ProcessingRule == null)
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRow>>(new List<wbm_common.DataObjects.ProcessingRule.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRow>>(new List<wbm_common.DataObjects.ProcessingRule.dbRow> { ProcessingRule });
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRule.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ProcessingRule.dbRow>> ProcessingRule_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRule.dbRow ProcessingRule = await _dbContext.ProcessingRule.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ProcessingRule == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRule.dbRow>("ProcessingRule not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRule.dbRow>(ProcessingRule);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRule.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRule.dbRow>>> ProcessingRule_List()
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRule.dbRow> ProcessingRules = await _dbContext.ProcessingRule.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRow>>(ProcessingRules);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ProcessingRule.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ProcessingRule.dbRow>>> ProcessingRule_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ProcessingRule.dbRow> ProcessingRules = await _dbContext.ProcessingRule.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ProcessingRule.dbRow>>(ProcessingRules);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRule_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ProcessingRule.dbRow>>("Internal Exception occured");
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

        public async Task<Result<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>> ProcessingRuleProcess_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleProcess.dbRow ProcessingRuleProcess = await _dbContext.ProcessingRuleProcess.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ProcessingRuleProcess == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>("ProcessingRuleProcess not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>(ProcessingRuleProcess);

            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleProcess_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>>> ProcessingRuleProcessList_SearchByID(int ID)
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
                logger.Error("ProcessingRuleProcess_SearchByID - " + ID.ToString() + ": " + ex);
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

        public async Task<Result<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>> ProcessingRuleTriggerCondition_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow ProcessingRuleTriggerCondition = await _dbContext.ProcessingRuleTriggerCondition.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ProcessingRuleTriggerCondition == null)
                    return Result.Failure<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>("ProcessingRuleTriggerCondition not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>(ProcessingRuleTriggerCondition);

            }
            catch (Exception ex)
            {
                logger.Error("ProcessingRuleTriggerCondition_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>>> ProcessingRuleTriggerConditionList_SearchByID(int ID)
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
                logger.Error("ProcessingRuleTriggerCondition_SearchByID - " + ID.ToString() + ": " + ex);
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
