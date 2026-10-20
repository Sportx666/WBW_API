using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class PaperlessTransactionXrefHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>> ExecuteSearch(wbm_common.DataObjects.PaperlessTransactionXref.Search _PaperlessTransactionXrefSearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.PaperlessTransactionXref.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.PaperlessTransactionXref.dbRow longRunPaperlessTransactionXrefHeader = null)
        {
            Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>> rc;
            List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> PaperlessTransactionXrefs;
            switch (_PaperlessTransactionXrefSearchData.SearchMode)
            {
                case wbm_common.DataObjects.PaperlessTransactionXref.Search.SearchModeType.Undefined:
                    {
                        _PaperlessTransactionXrefSearchData.GenerateSqlQuery();
                        rc = await _WBMDB.PaperlessTransactionXref_ListBySql(_PaperlessTransactionXrefSearchData.SqlQuery, _PaperlessTransactionXrefSearchData.ListSqlParameter);
                        break;
                    }
                case wbm_common.DataObjects.PaperlessTransactionXref.Search.SearchModeType.SingleID:
                    {
                        rc = await _WBMDB.PaperlessTransactionXref_ListByID(_PaperlessTransactionXrefSearchData.ID);
                        break;
                    }
                case wbm_common.DataObjects.PaperlessTransactionXref.Search.SearchModeType.PaperlessKeyID:
                    {
                        rc = await _WBMDB.PaperlessTransactionXref_ListByPaperlessKeyID(_PaperlessTransactionXrefSearchData.PaperlessKeyID);
                        break;
                    }
                case wbm_common.DataObjects.PaperlessTransactionXref.Search.SearchModeType.TransactionHeaderID:
                    {
                        rc = await _WBMDB.PaperlessTransactionXref_ListByTransactionHeaderID(_PaperlessTransactionXrefSearchData.ID);
                        break;
                    }
                case wbm_common.DataObjects.PaperlessTransactionXref.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.PaperlessTransactionXref_List();
                        break;
                    }
                default:
                    {
                        logger.Error("PaperlessTransactionXrefHelper.ExecuteSearch: PaperlessTransactionXref Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunPaperlessTransactionXrefHeader);

            return rc;
        }

        public class PaperlessTransactionXrefSave
        {
            private wbm_common.DataObjects.PaperlessTransactionXref saveObject { get; set; }
            private wbm_common.DataObjects.PaperlessTransactionXref.dbRow saveRow { get; set; }

            private wbm_common.DataObjects.PaperlessTransactionXref.dbRow existRow { get; set; }

            private ChangeTracking ct = null;


            private Boolean Exists { get; set; }
            private Boolean IsNew { get; set; }

            public string ErrorMessage { get; set; }

            public Boolean SavedSuccessfully { get; set; }

            public int ReturnState { get; set; }

            private Boolean ContinuePaperlessTransactionXrefing = true;  // there are cases where we will not do the save but return successful - ie, if no changes

            private DateTime ChangeDate = new SqlDateTime(DateTime.Now).Value;

            private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            private readonly WBMDatabase _WBMDB;

            public int UserID { get; set; }

            private wbm_common.DataObjects.ChangeHeader.dbRow changeHeaderRow { get; set; }

            public PaperlessTransactionXrefSave(wbm_common.DataObjects.PaperlessTransactionXref saveObject, int UserID, WBMDatabase wBMDB)
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
                List<string> fieldErrors = DbRowValidator.requiredFieldErrors(wbm_common.DataObjects.PaperlessTransactionXref.MyRequiredList, saveRow, _WBMDB);
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
                                ContinuePaperlessTransactionXrefing = false;
                            }
                        }
                    }
                }
                else
                {
                    ContinuePaperlessTransactionXrefing = false;
                }

                if (ContinuePaperlessTransactionXrefing)
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
                    Result<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> rc = _WBMDB.PaperlessTransactionXref_ReadByID(saveRow.ID).GetAwaiter().GetResult();

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

                #region Check that ReferenceNumber is unique
                if (Exists && saveRow.ReferenceNumber == existRow.ReferenceNumber)
                {
                    // Don't need to check as we are the same
                }
                else
                {
                    Result<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> rcexist = _WBMDB.PaperlessTransactionXref_SearchByReferenceNumber(saveRow.ReferenceNumber).GetAwaiter().GetResult();

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
                        ErrorMessage = "ReferenceNumber already in use, must be unique - " + saveRow.ReferenceNumber;
                        logger.Error("PaperlessTransactionXrefSave ID " + saveRow.ID.ToString() + " ReferenceNumber " + saveRow.ReferenceNumber + " already exists");
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
                return ct.CheckForChanges(existRow, saveRow, wbm_common.DataObjects.PaperlessTransactionXref.ListChangeTrackerData);
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
                    changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.PaperlessTransactionXref,
                        saveObject.dbRow_instance.ID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);
                }

                saveRow.LastAmendedDateTime = ChangeDate;
                saveRow.LastAmendedMethod = saveObject.CalledFrom;
                saveRow.LastAmendedUserID = UserID;
            }

            public Boolean Save_New()
            {
                Result<int> sr = _WBMDB.PaperlessTransactionXref_Insert(saveRow).GetAwaiter().GetResult();

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
                Result<int> cuResult = _WBMDB.PaperlessTransactionXref_Update(saveRow).GetAwaiter().GetResult();

                #region ChangeHeader and detail
                if (cuResult.IsSuccess)
                {
                    // Insert ChangeHeader and detail entries
                    Result<int> chResult = _WBMDB.ChangeHeader_Insert(changeHeaderRow).GetAwaiter().GetResult();

                    if (chResult.IsSuccess)
                    {
                        changeHeaderRow.ID = chResult.Value;

                        foreach (wbm_common.DataObjects.ChangeDetail.dbRow cdRow in ct.ListChangeDetail)
                        {
                            cdRow.ChangeHeaderID = changeHeaderRow.ID;
                            cdRow.ChangeHeaderTableID = changeHeaderRow.ChangeHeaderTableID;
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
