using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using WBM_API.Models;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class InvoiceCycleHistoryHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>> ExecuteSearch(wbm_common.DataObjects.InvoiceCycleHistory.Search _InvoiceCycleHistorySearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>> rc;
            List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> InvoiceCycleHistorys;
            switch (_InvoiceCycleHistorySearchData.SearchMode)
            {
                case wbm_common.DataObjects.InvoiceCycleHistory.Search.SearchModeType.Undefined:
                    {
                        _InvoiceCycleHistorySearchData.GenerateSqlQuery();
                        rc = await _WBMDB.InvoiceCycleHistory_ListBySql(_InvoiceCycleHistorySearchData.SqlQuery, _InvoiceCycleHistorySearchData.ListSqlParameter);
                        break;
                    }
                case wbm_common.DataObjects.InvoiceCycleHistory.Search.SearchModeType.SingleID:
                    {
                        rc = await _WBMDB.InvoiceCycleHistoryList_SearchByID(_InvoiceCycleHistorySearchData.ID);
                        break;
                    }
                case wbm_common.DataObjects.InvoiceCycleHistory.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.InvoiceCycleHistory_List();
                        break;
                    }
                case wbm_common.DataObjects.InvoiceCycleHistory.Search.SearchModeType.SingleCompany:
                    {
                        if (_InvoiceCycleHistorySearchData.ListCompanyID == null || _InvoiceCycleHistorySearchData.ListCompanyID.Count() != 1)
                        {
                            logger.Error("InvoiceCycleHistoryHelper.ExecuteSearch: Location SingleCompany doesn't have single company input");
                            rc = Result.Exception<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>("Internal Exception occured");
                            break;
                        }
                        rc = await _WBMDB.InvoiceCycleHistory_ListByCompanyID(_InvoiceCycleHistorySearchData.ListCompanyID[0]);
                        break;
                    }
                default:
                    {
                        logger.Error("InvoiceCycleHistoryHelper.ExecuteSearch: InvoiceCycleHistory Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }

        public class InvoiceCycleHistorySave
        {
            private wbm_common.DataObjects.InvoiceCycleHistory saveObject { get; set; }
            private wbm_common.DataObjects.InvoiceCycleHistory.dbRow saveRow { get; set; }

            private wbm_common.DataObjects.InvoiceCycleHistory.dbRow existRow { get; set; }

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

            public InvoiceCycleHistorySave(wbm_common.DataObjects.InvoiceCycleHistory saveObject, int UserID, WBMDatabase wBMDB)
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
                    Result<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> rc = _WBMDB.InvoiceCycleHistory_ReadByID(saveRow.ID).GetAwaiter().GetResult();

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

                #region Check that PublicID is unique
                if (Exists && saveRow.ID == existRow.ID)
                {
                    // Don't need to check as we are the same
                }
                else
                {
                    Result<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> rcexist = _WBMDB.InvoiceCycleHistory_SearchByID(saveRow.ID).GetAwaiter().GetResult();

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
                        ErrorMessage = "ID already in use, must be unique - " + saveRow.ID.ToString();
                        logger.Error("InvoiceCycleHistorySave ID " + saveRow.ID.ToString() + " already exists");
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
                return ct.CheckForChanges(existRow, saveRow, wbm_common.DataObjects.InvoiceCycleHistory.ListChangeTrackerData);
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
                    changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.InvoiceCycleHistory,
                        saveObject.dbRow_instance.ID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);
                }

                saveRow.LastAmendedDateTime = ChangeDate;
                saveRow.LastAmendedMethod = saveObject.CalledFrom;
                saveRow.LastAmendedUserID = UserID;
            }

            public Boolean Save_New()
            {
                Result<int> sr = _WBMDB.InvoiceCycleHistory_Insert(saveRow).GetAwaiter().GetResult();

                if (sr.IsSuccess)
                {
                    saveRow.ID = sr.Value;
                    return true;
                }
                else
                {
                    ErrorMessage = sr.ErrorMessage;
                    if (sr.IsException)
                        ReturnState = 500;
                    else
                        ReturnState = 400;
                    return false;
                }
            }

            public Boolean Save_Update_or_Delete()
            {
                Boolean ErrorOccured = false;

                // We have change log therefore need to do this in a transaction

                using var transaction = _WBMDB.dbContext.Database.BeginTransaction();

                transaction.CreateSavepoint("Start");

                // Deletes are just an update with DeletedFlag true
                Result<int> cuResult = _WBMDB.InvoiceCycleHistory_Update(saveRow).GetAwaiter().GetResult();

                #region ChangeHeader and detail
                if (cuResult.IsSuccess)
                {
                    // Insert ChangeHeader and detail entries
                    Result<int> chResult = _WBMDB.ChangeHeader_Insert(changeHeaderRow).GetAwaiter().GetResult();

                    if (chResult.IsSuccess)
                    {
                        changeHeaderRow.ID = chResult.Value;

                        int ChangeHeaderTableID = (int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.InvoiceCycleHistory;

                        foreach (wbm_common.DataObjects.ChangeDetail.dbRow cdRow in ct.ListChangeDetail)
                        {
                            cdRow.ChangeHeaderID = changeHeaderRow.ID;
                            cdRow.ChangeHeaderTableID = ChangeHeaderTableID;
                            cdRow.RecordID = changeHeaderRow.RecordID;

                            Result<int> cdResult = _WBMDB.ChangeDetail_Insert((wbm_common.DataObjects.ChangeDetail.dbRow)cdRow).GetAwaiter().GetResult();
                            if (cdResult.IsFailure)
                            {
                                // Error occured
                                ErrorOccured = true;
                                ErrorMessage = cdResult.ErrorMessage;
                                if (cdResult.IsException)
                                    ReturnState = 500;
                                else
                                    ReturnState = 400;
                                break;
                            }
                        }
                    }
                    else
                    {
                        ErrorOccured = true;
                        ErrorMessage = chResult.ErrorMessage;
                        if (chResult.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;
                    }
                }
                else
                {
                    ErrorOccured = true;
                    ErrorMessage = cuResult.ErrorMessage;
                    if (cuResult.IsException)
                        ReturnState = 500;
                    else
                        ReturnState = 400;
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
