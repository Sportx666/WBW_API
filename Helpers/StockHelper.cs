using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;

namespace WBM_API.Helpers
{
    public class StockHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.Stock.dbRowExtended>>> ExecuteSearch(wbm_common.DataObjects.Stock.Search _StockSearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<List<wbm_common.DataObjects.Stock.dbRowExtended>> rc;
            List<wbm_common.DataObjects.Stock.dbRowExtended> Stocks;
            switch (_StockSearchData.SearchMode)
            {
                case wbm_common.DataObjects.Stock.Search.SearchModeType.Undefined:
                    {
                        _StockSearchData.GenerateSqlQuery();
                        rc = await _WBMDB.Stock_ListBySql(_StockSearchData.SqlQuery, _StockSearchData.ListSqlParameter);
                        break;
                    }
                case wbm_common.DataObjects.Stock.Search.SearchModeType.SingleID:
                    {
                        rc = await _WBMDB.Stock_ListByID(_StockSearchData.ID);
                        break;
                    }
                case wbm_common.DataObjects.Stock.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.Stock_List();
                        break;
                    }
                case wbm_common.DataObjects.Stock.Search.SearchModeType.SingleOwner:
                    {
                        if (_StockSearchData.ListOwnerID == null || _StockSearchData.ListOwnerID.Count() != 1)
                        {
                            logger.Error("StockHelper.ExecuteSearch: Stock SingleOwner doesn't have single owner input");
                            rc = Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
                            break;
                        }
                        rc = await _WBMDB.Stock_ListBySingleOwnerID(_StockSearchData.ListOwnerID[0]);
                        break;
                    }
                case wbm_common.DataObjects.Stock.Search.SearchModeType.SingleSite:
                    {
                        if (_StockSearchData.ListSiteID == null || _StockSearchData.ListSiteID.Count() != 1)
                        {
                            logger.Error("StockHelper.ExecuteSearch: Stock SingleSite doesn't have single site input");
                            rc = Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
                            break;
                        }
                        rc = await _WBMDB.Stock_ListBySingleSiteID(_StockSearchData.ListSiteID[0]);
                        break;
                    }
                case wbm_common.DataObjects.Stock.Search.SearchModeType.Paperless_KeyID:
                    {
                        rc = await _WBMDB.Stock_ListByPaperlessKeyID(_StockSearchData.Paperless_KeyID);
                        break;
                    }
                case wbm_common.DataObjects.Stock.Search.SearchModeType.Owner_ListProductCode:
                    {
                        rc = await _WBMDB.Stock_ListByOwnerIDListProductCode(_StockSearchData.ListProductCode, _StockSearchData.ListOwnerID[0]);
                        break;
                    }
                case wbm_common.DataObjects.Stock.Search.SearchModeType.ListID:
                    {
                        rc = await _WBMDB.Stock_ListID(_StockSearchData.ListID);
                        break;
                    }
                default:
                    {
                        logger.Error("StockHelper.ExecuteSearch: Stock Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.Stock.dbRowExtended>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }

        public class StockSave
        {
            private wbm_common.DataObjects.Stock saveObject { get; set; }
            private wbm_common.DataObjects.Stock.dbRow saveRow { get; set; }

            private wbm_common.DataObjects.Stock.dbRow existRow { get; set; }

            private ChangeTracking ct = null;


            private Boolean Exists { get; set; }
            private Boolean IsNew { get; set; }

            public string ErrorMessage { get; set; }

            public Boolean SavedSuccessfully { get; set; }

            public int ReturnState { get; set; }

            private Boolean ContinueProcessing = true;  // there are cases where we will not do the save but return successful - ie, if no changes

            private DateTime ChangeDate = new SqlDateTime(DateTime.Now).Value;

            private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            private readonly WBMDatabase _WBMDB;

            public int UserID { get; set; }

            private wbm_common.DataObjects.ChangeHeader.dbRow changeHeaderRow { get; set; }

            public StockSave(wbm_common.DataObjects.Stock saveObject, int UserID, WBMDatabase wBMDB)
            {
                this.saveObject = saveObject;
                saveRow = saveObject.dbRow_instance;
                _WBMDB = wBMDB;
                this.UserID = UserID;

                Exists = false;
                IsNew = false;
                SavedSuccessfully = false;
            }

            public Boolean Save()
            {
                List<string> fieldErrors = DbRowValidator.requiredFieldErrors(Stock.MyRequiredList, saveRow, _WBMDB);
                if (fieldErrors.Count() > 0)
                {
                    ErrorMessage = string.Join(", ", fieldErrors);
                    ReturnState = 400;
                    return false;
                }
                if (CheckExist())
                {
                    if (Exists)
                    {
                        if (CheckForChanges())
                        {
                            if (!ct.HasChanges)
                            {
                                // Nothing has changed, no need to save
                                ContinueProcessing = false;
                            }
                        }
                    }
                }
                else
                {
                    ContinueProcessing = false;
                }

                if (ContinueProcessing)
                {
                    // Havent failed yet, set the last changed fields
                    SetChanged();

                    // Insert, update or delete as case may be
                    if (saveRow.ID < 1)
                        SavedSuccessfully = Save_New();
                    else
                        SavedSuccessfully = Save_Update_or_Delete();
                }

                return SavedSuccessfully;
            }

            public Boolean CheckExist()
            {
                #region Do we have an existing rowe ?
                if (saveRow.ID > 0)
                {
                    Result<wbm_common.DataObjects.Stock.dbRow> rc = _WBMDB.Stock_ReadByID(saveRow.ID).GetAwaiter().GetResult();

                    if (rc.IsFailure)
                    {
                        ErrorMessage = rc.ErrorMessage;
                        if (rc.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;

                        return false;
                    }
                    else
                    {
                        existRow = rc.Value;

                        if (existRow.DeletedFlag == true)
                        {
                            if (saveRow.DeletedFlag == true)
                            {
                                // Already deleted, no issue
                                SavedSuccessfully = true;
                                return false;
                            }
                            else
                            {
                                // exist row is deleted, save row is not; this cannot be
                                ErrorMessage = "Record already deleted, cannot save changes";
                                ReturnState = 400;
                                return false;
                            }
                        }

                        // Check last amendedtime is the same
                        if (saveRow.LastAmendedDateTime != existRow.LastAmendedDateTime)
                        {
                            ErrorMessage = "Record has been amended since read, cannot save changes";
                            ReturnState = 400;
                            return false;
                        }

                        Exists = true;
                        IsNew = false;
                    }
                }
                else
                    IsNew = true;
                #endregion

                #region Check that ProductCode is unique
                if (Exists && saveRow.ID == existRow.ID)
                {
                    // Don't need to check as we are the same
                }
                else
                {
                    Result<wbm_common.DataObjects.Stock.dbRow> rcexist = _WBMDB.Stock_SearchByProductCode(saveRow.ProductCode).GetAwaiter().GetResult();

                    if (rcexist.IsException)
                    {
                        SavedSuccessfully = false;
                        ReturnState = 500;
                    }
                    else if (rcexist.IsFailure)
                    {
                        // Not found, so ok to continue
                    }
                    else
                    {
                        // We have at least one row... :(
                        ErrorMessage = "ProductCode already in use, must be unique - " + saveRow.ProductCode;
                        logger.Error("StockSave ProductCode " + saveRow.ProductCode + " already exists");
                        ReturnState = 400;
                        SavedSuccessfully = false;
                        return false;
                    }
                }

                #endregion

                return true;
            }

            public Boolean CheckForChanges()
            {
                ct = new ChangeTracking();
                return ct.CheckForChanges(existRow, saveRow, wbm_common.DataObjects.Stock.ListChangeTrackerData);
            }

            public void SetChanged()
            {
                if (IsNew)
                {
                    saveRow.CreatedDateTime = ChangeDate;
                    saveRow.CreatedMethod = saveObject.CalledFrom;
                    saveRow.CreatedUserID = UserID;
                }
                else
                {
                    // If we're not new, and we're here, there are changes
                    changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.Stock,
                        saveObject.dbRow_instance.ID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);
                }

                saveRow.LastAmendedDateTime = ChangeDate;
                saveRow.LastAmendedMethod = saveObject.CalledFrom;
                saveRow.LastAmendedUserID = UserID;
            }

            public Boolean Save_New()
            {
                SavedSuccessfully = true;
                using var transaction = _WBMDB.dbContext.Database.BeginTransaction();

                transaction.CreateSavepoint("Start");

                Result<int> sr = _WBMDB.Stock_Insert(saveRow).GetAwaiter().GetResult();

                if (sr.IsSuccess)
                {
                    saveRow.ID = sr.Value;

                    if (saveObject.ToDo != null)
                    {
                        saveObject.ToDo.dbRow_instance.SourceID = saveRow.ID;
                        ChangeTracking CTO = new ChangeTracking();
                        ToDoHelper TDH = new ToDoHelper();
                        Result<int> todo_result = TDH.ExternalAddInTodo(_WBMDB, saveObject.ToDo.dbRow_instance);
                        if (!todo_result.IsSuccess)
                        {
                            SavedSuccessfully = false;
                            ErrorMessage = todo_result.ErrorMessage;
                            ReturnState = 500;
                        }
                    }
                    if (SavedSuccessfully)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                else
                {
                    ErrorMessage = sr.ErrorMessage;
                    ReturnState = 500;
                }

                transaction.RollbackToSavepoint("Start");
                SavedSuccessfully = false;
                return false;
            }

            public Boolean Save_Update_or_Delete()
            {
                Boolean ErrorOccured = false;

                // We have change log therefore need to do this in a transaction

                using var transaction = _WBMDB.dbContext.Database.BeginTransaction();

                transaction.CreateSavepoint("Start");

                // Deletes are just an update with DeletedFlag true
                Result<int> cuResult = _WBMDB.Stock_Update(saveRow).GetAwaiter().GetResult();

                #region ChangeHeader, detail and ToDo
                if (cuResult.IsSuccess)
                {
                    if (saveObject.ToDo != null)
                        saveObject.ToDo.dbRow_instance.SourceID = cuResult.Value;
                    ChangeTracking CTO = new ChangeTracking();
                    string RtnError = CTO.addInChangeHeaderDetail(_WBMDB, changeHeaderRow, ct);
                    if (RtnError != "")
                    {
                        ErrorOccured = true;
                        ErrorMessage = RtnError;
                        ReturnState = 500;
                    }
                    else if (saveObject.ToDo != null)
                    {
                        ToDoHelper TDH = new ToDoHelper();
                        Result<int> todo_result = TDH.ExternalAddInTodo(_WBMDB, saveObject.ToDo.dbRow_instance);
                        if (!todo_result.IsSuccess)
                        {
                            SavedSuccessfully = false;
                            ErrorMessage = todo_result.ErrorMessage;
                            ReturnState = 500;
                        }
                    }
                }
                else
                {
                    ErrorOccured = true;
                    ErrorMessage = cuResult.ErrorMessage;
                    ReturnState = 500;
                }
                #endregion

                if (!ErrorOccured)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.RollbackToSavepoint("Start");
                    SavedSuccessfully = false;
                    return false;
                }
            }
        }
    }
}
