using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region ToDo
        public async Task<Result<int>> ToDo_Insert(wbm_common.DataObjects.ToDo.dbRow ToDo)
        {
            try
            {
                await _dbContext.ToDo.AddAsync(ToDo);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ToDo.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_Insert - " + ToDo.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ToDo_Update(wbm_common.DataObjects.ToDo.dbRow ToDo)
        {
            try
            {
                _dbContext.ToDo.Attach(ToDo);
                _dbContext.Entry(ToDo).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ToDo.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_Update - " + ToDo.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ToDo.dbRow>> ToDo_SearchByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ToDo.dbRow ToDoDb = await _dbContext.ToDo.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID).FirstOrDefaultAsync();

                if (ToDoDb == null)
                    return Result.Failure<wbm_common.DataObjects.ToDo.dbRow>("ToDo not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ToDo.dbRow>(ToDoDb);

            }
            catch (Exception ex)
            {
                logger.Error("ToDo_SearchByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ToDo.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ToDo.dbRowExtended>>> ToDo_ListByID(int ID)
        {
            try
            {
                List<wbm_common.DataObjects.ToDo.dbRowExtended> ToDoDb = await (from ToDo in _dbContext.ToDo.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag && x.ID == ID)
                                                                                join site in _dbContext.Site.AsNoTracking()
                                                                                    on ToDo.SiteID equals site.ID into sitePR
                                                                                from SPR in sitePR.DefaultIfEmpty()
                                                                                join company in _dbContext.Company.AsNoTracking()
                                                                                    on ToDo.CompanyID equals company.ID into companyPR
                                                                                from CPR in companyPR.DefaultIfEmpty()
                                                                                join Owner in _dbContext.Owner.AsNoTracking()
                                                                                    on ToDo.OwnerID equals Owner.ID into OwnerPR
                                                                                from OPR in OwnerPR.DefaultIfEmpty()
                                                                                join Category in _dbContext.ToDo_Category.AsNoTracking()
                                                                                    on ToDo.CategoryID equals Category.ID into CategoryPR
                                                                                from CatPR in CategoryPR.DefaultIfEmpty()
                                                                                join ChangeHeader_TableName in _dbContext.ChangeHeader_TableName.AsNoTracking()
                                                                                    on ToDo.ChangeHeaderTableID equals ChangeHeader_TableName.ID into ChangeHeader_TableNamePR
                                                                                from CHPR in ChangeHeader_TableNamePR.DefaultIfEmpty()
                                                                                join Status in _dbContext.ToDoStatus_Type.AsNoTracking()
                                                                                    on ToDo.StatusID equals Status.ID into StatusPR
                                                                                from StPR in StatusPR.DefaultIfEmpty()

                                                                                select new wbm_common.DataObjects.ToDo.dbRowExtended
                                                                                {
                                                                                    ID = ToDo.ID,
                                                                                    CategoryID = ToDo.CategoryID,
                                                                                    CompanyID = ToDo.CompanyID,
                                                                                    OwnerID = ToDo.OwnerID,
                                                                                    SiteID = ToDo.SiteID,
                                                                                    EventDateTime = ToDo.EventDateTime,
                                                                                    Description = ToDo.Description,
                                                                                    ChangeHeaderTableID = ToDo.ChangeHeaderTableID,
                                                                                    SourceID = ToDo.SourceID,
                                                                                    StatusID = ToDo.StatusID,
                                                                                    Note = ToDo.Note,
                                                                                    DeletedFlag = ToDo.DeletedFlag,
                                                                                    CreatedDateTime = ToDo.CreatedDateTime,
                                                                                    CreatedMethod = ToDo.CreatedMethod,
                                                                                    CreatedUserID = ToDo.CreatedUserID,
                                                                                    LastAmendedDateTime = ToDo.LastAmendedDateTime,
                                                                                    LastAmendedMethod = ToDo.LastAmendedMethod,
                                                                                    LastAmendedUserID = ToDo.LastAmendedUserID,

                                                                                    Site_PublicID = SPR.PublicID,
                                                                                    Company_ShortName = CPR.ShortName,
                                                                                    ToDoCategory_PublicID = CatPR.PublicID,
                                                                                    Owner_Name = OPR.Name,
                                                                                    ChangeHeaderTable_Name = CHPR.TableName,
                                                                                    ToDoStatusType_PublicID = StPR.PublicID
                                                                                }).ToListAsync();

                if (ToDoDb == null)
                    return Result.Success<List<wbm_common.DataObjects.ToDo.dbRowExtended>>(new List<wbm_common.DataObjects.ToDo.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ToDo.dbRowExtended>>(ToDoDb);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ToDo.dbRowExtended>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ToDo.dbRow>> ToDo_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ToDo.dbRow ToDoDb = await _dbContext.ToDo.AsNoTracking()
                                                                        .Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (ToDoDb == null)
                    return Result.Failure<wbm_common.DataObjects.ToDo.dbRow>("ToDo not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ToDo.dbRow>(ToDoDb);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ToDo.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ToDo.dbRowExtended>>> ToDo_List()
        {
            try
            {
                List<wbm_common.DataObjects.ToDo.dbRowExtended> ToDos = await (from ToDo in _dbContext.ToDo.AsNoTracking()
                                                                        .Where(x => !x.DeletedFlag)
                                                                               join site in _dbContext.Site.AsNoTracking()
                                                                                   on ToDo.SiteID equals site.ID into sitePR
                                                                               from SPR in sitePR.DefaultIfEmpty()
                                                                               join company in _dbContext.Company.AsNoTracking()
                                                                                   on ToDo.CompanyID equals company.ID into companyPR
                                                                               from CPR in companyPR.DefaultIfEmpty()
                                                                               join Owner in _dbContext.Owner.AsNoTracking()
                                                                                   on ToDo.OwnerID equals Owner.ID into OwnerPR
                                                                               from OPR in OwnerPR.DefaultIfEmpty()
                                                                               join Category in _dbContext.ToDo_Category.AsNoTracking()
                                                                                   on ToDo.CategoryID equals Category.ID into CategoryPR
                                                                               from CatPR in CategoryPR.DefaultIfEmpty()
                                                                               join ChangeHeader_TableName in _dbContext.ChangeHeader_TableName.AsNoTracking()
                                                                                   on ToDo.ChangeHeaderTableID equals ChangeHeader_TableName.ID into ChangeHeader_TableNamePR
                                                                               from CHPR in ChangeHeader_TableNamePR.DefaultIfEmpty()
                                                                               join Status in _dbContext.ToDoStatus_Type.AsNoTracking()
                                                                                   on ToDo.StatusID equals Status.ID into StatusPR
                                                                               from StPR in StatusPR.DefaultIfEmpty()

                                                                               select new wbm_common.DataObjects.ToDo.dbRowExtended
                                                                               {
                                                                                   ID = ToDo.ID,
                                                                                   CategoryID = ToDo.CategoryID,
                                                                                   CompanyID = ToDo.CompanyID,
                                                                                   OwnerID = ToDo.OwnerID,
                                                                                   SiteID = ToDo.SiteID,
                                                                                   EventDateTime = ToDo.EventDateTime,
                                                                                   Description = ToDo.Description,
                                                                                   ChangeHeaderTableID = ToDo.ChangeHeaderTableID,
                                                                                   SourceID = ToDo.SourceID,
                                                                                   StatusID = ToDo.StatusID,
                                                                                   Note = ToDo.Note,
                                                                                   DeletedFlag = ToDo.DeletedFlag,
                                                                                   CreatedDateTime = ToDo.CreatedDateTime,
                                                                                   CreatedMethod = ToDo.CreatedMethod,
                                                                                   CreatedUserID = ToDo.CreatedUserID,
                                                                                   LastAmendedDateTime = ToDo.LastAmendedDateTime,
                                                                                   LastAmendedMethod = ToDo.LastAmendedMethod,
                                                                                   LastAmendedUserID = ToDo.LastAmendedUserID,

                                                                                   Site_PublicID = SPR.PublicID,
                                                                                   Company_ShortName = CPR.ShortName,
                                                                                   ToDoCategory_PublicID = CatPR.PublicID,
                                                                                   Owner_Name = OPR.Name,
                                                                                   ChangeHeaderTable_Name = CHPR.TableName,
                                                                                   ToDoStatusType_PublicID = StPR.PublicID
                                                                               }).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ToDo.dbRowExtended>>(ToDos);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ToDo.dbRowExtended>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ToDo.dbRow>>> ToDo_ListWithDeletedByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ToDo_ListWithDeletedByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ToDo.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ToDo.dbRow> ToDos = await _dbContext.ToDo.AsNoTracking()
                                                                        .Where(x => IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ToDo.dbRow>>(ToDos);
                }
                catch (Exception ex)
                {
                    logger.Error("ToDo_ListWithDeletedByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ToDo.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ToDo.dbRowExtended>>> ToDo_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ToDo.dbRowExtended> ToDos = await _dbContext.Database.SqlQueryRaw<wbm_common.DataObjects.ToDo.dbRowExtended>(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ToDo.dbRowExtended>>(ToDos);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ToDo.dbRowExtended>>("Internal Exception occured");
            }
        }
        #endregion

        #region ToDoCategory
        public async Task<Result<int>> ToDo_Category_Insert(wbm_common.DataObjects.ToDo_Category.dbRow ToDo_Category)
        {
            try
            {
                await _dbContext.ToDo_Category.AddAsync(ToDo_Category);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ToDo_Category.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_Category_Insert - " + ToDo_Category.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ToDo_Category_Update(wbm_common.DataObjects.ToDo_Category.dbRow ToDo_Category)
        {
            try
            {
                _dbContext.ToDo_Category.Attach(ToDo_Category);
                _dbContext.Entry(ToDo_Category).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ToDo_Category.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_Category_Update - " + ToDo_Category.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ToDo_Category.dbRow>> ToDo_Category_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ToDo_Category.dbRow ToDo_Category = await _dbContext.ToDo_Category.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (ToDo_Category == null)
                    return Result.Failure<wbm_common.DataObjects.ToDo_Category.dbRow>("ToDo_Category not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ToDo_Category.dbRow>(ToDo_Category);

            }
            catch (Exception ex)
            {
                logger.Error("ToDo_Category_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ToDo_Category.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ToDo_Category.dbRow>>> ToDo_Category_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ToDo_Category.dbRow ToDo_Category = await _dbContext.ToDo_Category.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ToDo_Category == null)
                    return Result.Success<List<wbm_common.DataObjects.ToDo_Category.dbRow>>(new List<wbm_common.DataObjects.ToDo_Category.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ToDo_Category.dbRow>>(new List<wbm_common.DataObjects.ToDo_Category.dbRow> { ToDo_Category });
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_Category_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ToDo_Category.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ToDo_Category.dbRow>> ToDo_Category_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ToDo_Category.dbRow ToDo_Category = await _dbContext.ToDo_Category.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ToDo_Category == null)
                    return Result.Failure<wbm_common.DataObjects.ToDo_Category.dbRow>("ToDo_Category not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ToDo_Category.dbRow>(ToDo_Category);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_Category_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ToDo_Category.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ToDo_Category.dbRow>>> ToDo_Category_List()
        {
            try
            {
                List<wbm_common.DataObjects.ToDo_Category.dbRow> ToDo_Categorys = await _dbContext.ToDo_Category.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ToDo_Category.dbRow>>(ToDo_Categorys);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_Category_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ToDo_Category.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ToDo_Category.dbRow>>> ToDo_Category_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ToDo_Category_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ToDo_Category.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ToDo_Category.dbRow> ToDo_Categorys = await _dbContext.ToDo_Category.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ToDo_Category.dbRow>>(ToDo_Categorys);
                }
                catch (Exception ex)
                {
                    logger.Error("ToDo_Category_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ToDo_Category.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ToDo_Category.dbRow>>> ToDo_Category_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ToDo_Category.dbRow> ToDo_Categorys = await _dbContext.ToDo_Category.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ToDo_Category.dbRow>>(ToDo_Categorys);
            }
            catch (Exception ex)
            {
                logger.Error("ToDo_Category_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ToDo_Category.dbRow>>("Internal Exception occured");
            }
        }
        #endregion

        #region ToDoStatusType
        public async Task<Result<int>> ToDoStatus_Type_Insert(wbm_common.DataObjects.ToDoStatus_Type.dbRow ToDoStatus_Type)
        {
            try
            {
                await _dbContext.ToDoStatus_Type.AddAsync(ToDoStatus_Type);
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ToDoStatus_Type.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ToDoStatus_Type_Insert - " + ToDoStatus_Type.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<int>> ToDoStatus_Type_Update(wbm_common.DataObjects.ToDoStatus_Type.dbRow ToDoStatus_Type)
        {
            try
            {
                _dbContext.ToDoStatus_Type.Attach(ToDoStatus_Type);
                _dbContext.Entry(ToDoStatus_Type).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Success<int>(ToDoStatus_Type.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ToDoStatus_Type_Update - " + ToDoStatus_Type.ID.ToString() + ": " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ToDoStatus_Type.dbRow>> ToDoStatus_Type_SearchByPublicID(string PublicID)
        {
            try
            {
                wbm_common.DataObjects.ToDoStatus_Type.dbRow ToDoStatus_Type = await _dbContext.ToDoStatus_Type.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.PublicID == PublicID);

                if (ToDoStatus_Type == null)
                    return Result.Failure<wbm_common.DataObjects.ToDoStatus_Type.dbRow>("ToDoStatus_Type not found : " + PublicID);
                else
                    return Result.Success<wbm_common.DataObjects.ToDoStatus_Type.dbRow>(ToDoStatus_Type);

            }
            catch (Exception ex)
            {
                logger.Error("ToDoStatus_Type_SearchByPublicID - " + PublicID + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ToDoStatus_Type.dbRow>("Internal Exception occured");
            }
        }

        //because of the way the results return work, it makes more sense to keep the result format for all and just have this be a list instead of a single return
        public async Task<Result<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>> ToDoStatus_Type_ListByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ToDoStatus_Type.dbRow ToDoStatus_Type = await _dbContext.ToDoStatus_Type.AsNoTracking().FirstOrDefaultAsync(x => !x.DeletedFlag && x.ID == ID);

                if (ToDoStatus_Type == null)
                    return Result.Success<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>(new List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>(new List<wbm_common.DataObjects.ToDoStatus_Type.dbRow> { ToDoStatus_Type });
            }
            catch (Exception ex)
            {
                logger.Error("ToDoStatus_Type_ListByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>("Internal Exception occured");
            }
        }

        // read and search are different because of the deleted flag
        public async Task<Result<wbm_common.DataObjects.ToDoStatus_Type.dbRow>> ToDoStatus_Type_ReadByID(int ID)
        {
            try
            {
                wbm_common.DataObjects.ToDoStatus_Type.dbRow ToDoStatus_Type = await _dbContext.ToDoStatus_Type.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);

                if (ToDoStatus_Type == null)
                    return Result.Failure<wbm_common.DataObjects.ToDoStatus_Type.dbRow>("ToDoStatus_Type not found : " + ID.ToString());
                else
                    return Result.Success<wbm_common.DataObjects.ToDoStatus_Type.dbRow>(ToDoStatus_Type);
            }
            catch (Exception ex)
            {
                logger.Error("ToDoStatus_Type_ReadByID - " + ID.ToString() + ": " + ex);
                return Result.Exception<wbm_common.DataObjects.ToDoStatus_Type.dbRow>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>> ToDoStatus_Type_List()
        {
            try
            {
                List<wbm_common.DataObjects.ToDoStatus_Type.dbRow> ToDoStatus_Types = await _dbContext.ToDoStatus_Type.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>(ToDoStatus_Types);
            }
            catch (Exception ex)
            {
                logger.Error("ToDoStatus_Type_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>> ToDoStatus_Type_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ToDoStatus_Type_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ToDoStatus_Type.dbRow> ToDoStatus_Types = await _dbContext.ToDoStatus_Type.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>(ToDoStatus_Types);
                }
                catch (Exception ex)
                {
                    logger.Error("ToDoStatus_Type_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>("Internal Exception occured");
                }
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>> ToDoStatus_Type_ListBySql(string sqlString, List<SqlParameter> ListSqlParameter)
        {
            try
            {
                List<wbm_common.DataObjects.ToDoStatus_Type.dbRow> ToDoStatus_Types = await _dbContext.ToDoStatus_Type.FromSqlRaw(sqlString, ListSqlParameter.ToArray()).ToListAsync();
                return Result.Success<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>(ToDoStatus_Types);
            }
            catch (Exception ex)
            {
                logger.Error("ToDoStatus_Type_ListBySql -  " + sqlString + " : ex = " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>>("Internal Exception occured");
            }
        }
        #endregion
    }
}
