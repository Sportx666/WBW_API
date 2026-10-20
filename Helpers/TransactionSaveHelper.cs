using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;


namespace WBM_API.Helpers
{
    public class TransactionSaveHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public static Result<TransactionHolder> getTransactionFromTransactionReferenceTransactionType(string TransactionReference, int type, WBMDatabase _WBMDB, out int ErrorCode, out List<string> ErrorStrings)
        {
            ErrorCode = 0;
            ErrorStrings = new List<string>();
            TransactionHolder TH = new TransactionHolder();

            Result<TransactionHeader.dbRow> rcTH = null;
            rcTH = _WBMDB.TransactionHeader_SearchByReferenceTransactionType(TransactionReference, type).GetAwaiter().GetResult();
            if (rcTH.IsFailure)
            {
                if (rcTH.IsException)
                {
                    ErrorStrings.Add(rcTH.ErrorMessage);
                    ErrorCode = 500;
                }
                else
                    ErrorCode = 400;

                return Result.Success<TransactionHolder>(TH);
            }
            else
                TH.TransactionHeader_Row = rcTH.Value;

            Result<List<TransactionDetail.dbRow>> rcTD = null;
            rcTD = _WBMDB.TransactionDetail_ListByListTransactionHeaderIDs(new List<int> { rcTH.Value.ID }).GetAwaiter().GetResult();
            if (rcTD.IsException)
            {
                ErrorStrings.Add(rcTD.ErrorMessage);
                ErrorCode = 500;
            }
            else if (rcTD.IsSuccess)
                TH.ListTransactionDetail = rcTD.Value;

            Result<List<TransactionException.dbRow>> rcTE = null;
            rcTE = _WBMDB.TransactionException_ListByListTransactionHeaderIDs(new List<int> { rcTH.Value.ID }).GetAwaiter().GetResult();
            if (rcTE.IsException)
            {
                ErrorStrings.Add(rcTE.ErrorMessage);
                ErrorCode = 500;
            }
            else if (rcTE.IsSuccess)
                TH.ListTransactionException = rcTE.Value;

            Result<List<TransactionNotes.dbRow>> rcTN = null;
            rcTN = _WBMDB.TransactionNotes_ListByListTransactionHeaderIDs(new List<int> { rcTH.Value.ID }).GetAwaiter().GetResult();
            if (rcTN.IsException)
            {
                ErrorStrings.Add(rcTN.ErrorMessage);
                ErrorCode = 500;
            }
            else if (rcTN.IsSuccess)
                TH.ListTransactionNotes = rcTN.Value;

            Result<OrderHeader.dbRow> rcOH = null;
            rcOH = _WBMDB.OrderHeader_SearchByTransactionHeaderID(rcTH.Value.ID).GetAwaiter().GetResult();
            if (rcOH.IsException)
            {
                ErrorStrings.Add(rcOH.ErrorMessage);
                ErrorCode = 500;
            }
            else if (rcOH.IsSuccess)
                TH.OrderHeader_Row = rcOH.Value;

            Result<PurchaseOrderHeader.dbRow> rcPOH = null;
            rcPOH = _WBMDB.PurchaseOrderHeader_SearchByTransactionHeaderID(rcTH.Value.ID).GetAwaiter().GetResult();
            if (rcOH.IsException)
            {
                ErrorStrings.Add(rcPOH.ErrorMessage);
                ErrorCode = 500;
            }
            else if (rcPOH.IsSuccess)
                TH.PurchaseOrderHeader_Row = rcPOH.Value;

            if (ErrorCode == 400)
            {
                return Result.Failure<TransactionHolder>(string.Join(", ", ErrorStrings));
            }
            else if (ErrorCode == 500)
            {
                return Result.Exception<TransactionHolder>(string.Join(", ", ErrorStrings));
            }
            else
                return Result.Success<TransactionHolder>(TH);
        }

        public static Result<TransactionHolder> getTransactionFromHeaderID(int TransactionHeaderID, WBMDatabase _WBMDB)
        {
            int ErrorCode = 0;
            List<string> ErrorStrings = new List<string>();
            TransactionHolder TH = new TransactionHolder();

            Result<TransactionHeader.dbRowExtended> rcTH = null;
            rcTH = _WBMDB.TransactionHeader_SearchByID(TransactionHeaderID).GetAwaiter().GetResult();
            if (rcTH.IsFailure)
            {
                ErrorStrings.Add(rcTH.ErrorMessage);
                if (rcTH.IsException)
                    ErrorCode = 500;
                else
                    ErrorCode = 400;
            }
            else
                TH.TransactionHeader_Row = rcTH.Value;

            if (ErrorCode == 0)
            {
                Result<List<TransactionDetail.dbRow>> rcTD = null;
                rcTD = _WBMDB.TransactionDetail_ListByListTransactionHeaderIDs(new List<int> { TransactionHeaderID }).GetAwaiter().GetResult();
                if (rcTD.IsException)
                {
                    ErrorStrings.Add(rcTD.ErrorMessage);
                    ErrorCode = 500;
                }
                else if (rcTD.IsSuccess)
                    TH.ListTransactionDetail = rcTD.Value;

                Result<List<TransactionException.dbRow>> rcTE = null;
                rcTE = _WBMDB.TransactionException_ListByListTransactionHeaderIDs(new List<int> { TransactionHeaderID }).GetAwaiter().GetResult();
                if (rcTE.IsException)
                {
                    ErrorStrings.Add(rcTE.ErrorMessage);
                    ErrorCode = 500;
                }
                else if (rcTE.IsSuccess)
                    TH.ListTransactionException = rcTE.Value;

                Result<List<TransactionNotes.dbRow>> rcTN = null;
                rcTN = _WBMDB.TransactionNotes_ListByListTransactionHeaderIDs(new List<int> { TransactionHeaderID }).GetAwaiter().GetResult();
                if (rcTN.IsException)
                {
                    ErrorStrings.Add(rcTN.ErrorMessage);
                    ErrorCode = 500;
                }
                else if (rcTN.IsSuccess)
                    TH.ListTransactionNotes = rcTN.Value;

                Result<OrderHeader.dbRow> rcOH = null;
                rcOH = _WBMDB.OrderHeader_SearchByTransactionHeaderID(TransactionHeaderID).GetAwaiter().GetResult();
                if (rcOH.IsException)
                {
                    ErrorStrings.Add(rcOH.ErrorMessage);
                    ErrorCode = 500;
                }
                else if (rcOH.IsSuccess)
                    TH.OrderHeader_Row = rcOH.Value;

                Result<PurchaseOrderHeader.dbRow> rcPOH = null;
                rcPOH = _WBMDB.PurchaseOrderHeader_SearchByTransactionHeaderID(TransactionHeaderID).GetAwaiter().GetResult();
                if (rcOH.IsException)
                {
                    ErrorStrings.Add(rcPOH.ErrorMessage);
                    ErrorCode = 500;
                }
                else if (rcPOH.IsSuccess)
                    TH.PurchaseOrderHeader_Row = rcPOH.Value;
            }

            if (ErrorCode == 400)
            {
                return Result.Failure<TransactionHolder>(string.Join(", ", ErrorStrings));
            }
            else if (ErrorCode == 500)
            {
                return Result.Exception<TransactionHolder>(string.Join(", ", ErrorStrings));
            }
            else
                return Result.Success<TransactionHolder>(TH);
        }


        public class TransactionHelperSave
        {
            private wbm_common.NonDatabaseObjects.TransactionHolder saveObject { get; set; }
            private wbm_common.NonDatabaseObjects.TransactionHolder existObject { get; set; }

            public wbm_common.NonDatabaseObjects.TransactionHolder updateObject { get; set; }

            private int TransactionHeaderID { get; set; }

            private class ChangeTrackingPair
            {
                public ChangeHeader.dbRow ChangeHeaderRow { get; set; }

                public ChangeTracking ct { get; set; }

                public ChangeTrackingPair(ChangeHeader.dbRow CHRow, ChangeTracking ct)
                {
                    this.ChangeHeaderRow = CHRow;
                    this.ct = ct;
                }
            }

            private List<ChangeTrackingPair> ListChangeTrackingPair = new List<ChangeTrackingPair>();

            private Boolean Exists { get; set; }
            private Boolean HasChanges = false;
            private Boolean IsValid = true;

            List<string> fieldErrors = new List<string>();

            public string ErrorMessage { get; set; }

            public Boolean SavedSuccessfully { get; set; }

            public int ReturnState { get; set; }

            private Boolean ContinueProcessing = true;  // there are cases where we will not do the save but return successful - ie, if no changes

            private DateTime ChangeDate = new SqlDateTime(DateTime.Now).Value;

            private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            private readonly WBMDatabase _WBMDB;

            public int UserID { get; set; }

            private wbm_common.DataObjects.ChangeHeader.dbRow changeHeaderRow { get; set; }

            public TransactionHelperSave(wbm_common.NonDatabaseObjects.TransactionHolder saveObject, int UserID, WBMDatabase wBMDB)
            {
                this.saveObject = saveObject;
                _WBMDB = wBMDB;
                this.UserID = UserID;

                Exists = false;
                SavedSuccessfully = false;
            }

            public Boolean Save()
            {
                if (CheckRequiredFields())
                {
                    if (CheckExist())
                    {
                        if (Exists)
                        {
                            if (!CheckForChanges())
                            {
                                // Nothing has changed, no need to save
                                ContinueProcessing = false;
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
                        SavedSuccessfully = Save_Object();
                    }
                }

                return SavedSuccessfully;
            }

            #region CheckRequired
            public Boolean CheckRequiredFields()
            {
                if (PreCheck())
                {
                    CheckRequired_TransactionHeader();

                    CheckRequired_TransactionDetail();

                    CheckRequired_TransactionException();

                    CheckRequired_TransactionNotes();

                    CheckRequired_ToDo();

                    if (saveObject.OrderHeader_Row != null)
                        CheckRequired_OrderHeader();

                    if (saveObject.PurchaseOrderHeader_Row != null)
                        CheckRequired_PurchaseOrderHeader();
                }

                return IsValid;
            }

            private Boolean PreCheck()
            {
                if (saveObject.TransactionHeader_Row == null)
                {
                    ErrorMessage = "Header row is null!";
                    ReturnState = 400;
                    return false;
                }
                if (saveObject.ListTransactionDetail == null)
                    saveObject.ListTransactionDetail = new List<TransactionDetail.dbRow>();
                if (saveObject.ListTransactionException == null)
                    saveObject.ListTransactionException = new List<TransactionException.dbRow>();
                if (saveObject.ListTransactionNotes == null)
                    saveObject.ListTransactionNotes = new List<TransactionNotes.dbRow>();
                if (saveObject.ListToDo == null)
                    saveObject.ListToDo = new List<ToDo.dbRow>();

                if (fieldErrors.Count() > 0)
                {
                    ErrorMessage = string.Join(", ", fieldErrors);
                    ReturnState = 400;
                    return false;
                }
                else
                    return true;

            }

            public void CheckRequired_TransactionHeader()
            {
                fieldErrors.AddRange(DbRowValidator.requiredFieldErrors(TransactionHeader.MyRequiredList, saveObject.TransactionHeader_Row, _WBMDB));
            }

            public void CheckRequired_TransactionDetail()
            {
                foreach (TransactionDetail.dbRow TRR in saveObject.ListTransactionDetail)
                    fieldErrors.AddRange(DbRowValidator.requiredFieldErrors(TransactionDetail.MyRequiredList, TRR, _WBMDB));
            }

            public void CheckRequired_TransactionException()
            {
                foreach (TransactionException.dbRow TER in saveObject.ListTransactionException)
                    fieldErrors.AddRange(DbRowValidator.requiredFieldErrors(TransactionException.MyRequiredList, TER, _WBMDB));
            }

            public void CheckRequired_TransactionNotes()
            {
                foreach (TransactionNotes.dbRow TNR in saveObject.ListTransactionNotes)
                    fieldErrors.AddRange(DbRowValidator.requiredFieldErrors(TransactionNotes.MyRequiredList, TNR, _WBMDB));
            }

            public void CheckRequired_ToDo()
            {
                foreach (ToDo.dbRow TDR in saveObject.ListToDo)
                    fieldErrors.AddRange(DbRowValidator.requiredFieldErrors(ToDo.MyRequiredList, TDR, _WBMDB));
            }

            public void CheckRequired_OrderHeader()
            {
                fieldErrors.AddRange(DbRowValidator.requiredFieldErrors(OrderHeader.MyRequiredList, saveObject.OrderHeader_Row, _WBMDB));
            }

            public void CheckRequired_PurchaseOrderHeader()
            {
                fieldErrors.AddRange(DbRowValidator.requiredFieldErrors(PurchaseOrderHeader.MyRequiredList, saveObject.PurchaseOrderHeader_Row, _WBMDB));
            }

            #endregion

            public Boolean CheckExist()
            {
                #region Do we have an existing row ?

                TransactionHeaderID = saveObject.TransactionHeader_Row.ID;

                if (TransactionHeaderID > 0)
                {
                    updateObject = new TransactionHolder();
                    updateObject.ListTransactionDetail = new List<TransactionDetail.dbRow>();
                    updateObject.ListTransactionException = new List<TransactionException.dbRow>();
                    updateObject.ListTransactionNotes = new List<TransactionNotes.dbRow>();
                    updateObject.ListToDo = new List<ToDo.dbRow>();

                    Result<TransactionHolder> rc = null;

                    rc = getTransactionFromHeaderID(TransactionHeaderID, _WBMDB);

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
                        existObject = rc.Value;

                        if (existObject.TransactionHeader_Row.DeletedFlag == true)
                        {
                            if (saveObject.TransactionHeader_Row.DeletedFlag == true)
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
                        if (saveObject.TransactionHeader_Row.LastAmendedDateTime != existObject.TransactionHeader_Row.LastAmendedDateTime)
                        {
                            ErrorMessage = "Record has been amended since read, cannot save changes";
                            ReturnState = 400;
                            return false;
                        }

                        Exists = true;
                    }
                }
                else
                    updateObject = saveObject;
                #endregion

                #region Check that TransactionReference is unique
                if (Exists && saveObject.TransactionHeader_Row.TransactionReference == existObject.TransactionHeader_Row.TransactionReference)
                {
                    // Don't need to check as we are the same
                }
                else
                {
                    Result<wbm_common.DataObjects.TransactionHeader.dbRow> rcexist = null;

                    rcexist = _WBMDB.TransactionHeader_SearchByReferenceTransactionType(saveObject.TransactionHeader_Row.TransactionReference, saveObject.TransactionHeader_Row.TransactionTypeID).GetAwaiter().GetResult();

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
                        ErrorMessage = "Reference already in use, must be unique - " + saveObject.TransactionHeader_Row.TransactionReference;
                        logger.Error("TransactionHeader " + saveObject.TransactionHeader_Row.TransactionReference + " already exists");
                        ReturnState = 400;
                        SavedSuccessfully = false;
                        return false;
                    }
                }
                #endregion
                if (InvoiceFailure())
                    return false;
                if (PaperlessTransactionXRefFailure())
                    return false;
                if (OrderHeadersFailure())
                    return false;
                if (HeaderIDFailure())
                    return false;

                return true;
            }

            #region Check that Invoice hasn't changed
            public bool InvoiceFailure()
            {
                if (Exists && saveObject.TransactionHeader_Row.InvoiceHeaderID > 0 && 
                    (Math.Abs(saveObject.TransactionHeader_Row.InvoiceExFuel - existObject.TransactionHeader_Row.InvoiceExFuel) > (decimal)0.01
                    || Math.Abs(saveObject.TransactionHeader_Row.InvoiceFuel - existObject.TransactionHeader_Row.InvoiceFuel) > (decimal)0.01
                    || Math.Abs(saveObject.TransactionHeader_Row.InvoiceGST - existObject.TransactionHeader_Row.InvoiceGST) > (decimal)0.01))
                {
                    ErrorMessage = "Invoice already applied - " + saveObject.TransactionHeader_Row.TransactionReference;
                    logger.Error("TransactionHeader Invoice already applied " + saveObject.TransactionHeader_Row.TransactionReference);
                    ReturnState = 400;
                    SavedSuccessfully = false;
                    return true;
                }
                return false;
            }
            #endregion

            #region XrefCheck
            public bool PaperlessTransactionXRefFailure()
            {
                // This routine returns false if everthing ok; and true if there is a fault.

                // For new records only; ones that we're supplied an Xref row; and in that xref row, the paperlesskeyID is populated.
                if (!Exists && saveObject.PaperlessTransactionXref_Row != null
                    && saveObject.PaperlessTransactionXref_Row.PaperlessKeyID != null
                    && saveObject.PaperlessTransactionXref_Row.PaperlessKeyID != "")
                {
                    Result<List<wbm_common.DataObjects.PaperlessTransactionXref.dbRow>> rc = _WBMDB.PaperlessTransactionXref_ListByPaperlessKeyID(saveObject.PaperlessTransactionXref_Row.PaperlessKeyID).Result;
                    if (rc.IsSuccess && rc.Value.Count() > 0)
                    {
                        ErrorMessage = "Transaction already applied on paperless key id - " + saveObject.PaperlessTransactionXref_Row.PaperlessKeyID;
                        logger.Error(ErrorMessage);
                        ReturnState = 400;
                        SavedSuccessfully = false;
                        return true;
                    }
                    else if (!rc.IsSuccess)
                    {
                        // Ie, failed or exception
                        ErrorMessage = rc.ErrorMessage + saveObject.PaperlessTransactionXref_Row.PaperlessKeyID;
                        logger.Error(ErrorMessage);

                        if (rc.IsFailure)
                            ReturnState = 400;
                        else
                            ReturnState = 500;

                        SavedSuccessfully = false;
                        return true;
                    }
                }
                return false;
            }
            #endregion

            #region OrderHeaders
            public bool OrderHeadersFailure()
            {
                if (Exists && saveObject.OrderHeader_Row != null
                    && existObject.OrderHeader_Row != null)
                {
                    if (saveObject.OrderHeader_Row.ID != existObject.OrderHeader_Row.ID)
                    {
                        ErrorMessage = "A difference Order Header is already attached to this transaction - " + saveObject.PaperlessTransactionXref_Row.PaperlessKeyID;
                        logger.Error("A difference Order Header is already attached to this transaction " + saveObject.PaperlessTransactionXref_Row.PaperlessKeyID);
                        ReturnState = 400;
                        SavedSuccessfully = false;
                        return true;
                    }
                }
                if (Exists && saveObject.PurchaseOrderHeader_Row != null
                    && existObject.PurchaseOrderHeader_Row != null)
                {
                    if (saveObject.PurchaseOrderHeader_Row.ID != existObject.PurchaseOrderHeader_Row.ID)
                    {
                        ErrorMessage = "A difference Purchase Order Header is already attached to this transaction - " + saveObject.PaperlessTransactionXref_Row.PaperlessKeyID;
                        logger.Error("A difference Purchase Order Header is already attached to this transaction " + saveObject.PaperlessTransactionXref_Row.PaperlessKeyID);
                        ReturnState = 400;
                        SavedSuccessfully = false;
                        return true;
                    }
                }
                return false;
            }
            #endregion

            #region HeaderID
            public bool HeaderIDFailure()
            {
                if ((saveObject.OrderHeader_Row != null && saveObject.OrderHeader_Row.TransactionHeaderID != saveObject.TransactionHeader_Row.ID)
                    || (saveObject.PurchaseOrderHeader_Row != null && saveObject.PurchaseOrderHeader_Row.TransactionHeaderID != saveObject.TransactionHeader_Row.ID))
                {
                    ErrorMessage = "A Transaction Header ID in the order(s) doesn't match - " + saveObject.TransactionHeader_Row.TransactionReference;
                    logger.Error("A Transaction Header ID in the order(s) doesn't match " + saveObject.TransactionHeader_Row.TransactionReference);
                    ReturnState = 400;
                    SavedSuccessfully = false;
                    return true;
                }
                if (saveObject.PaperlessTransactionXref_Row != null && saveObject.PaperlessTransactionXref_Row.TransactionHeaderID != saveObject.TransactionHeader_Row.ID)
                {
                    ErrorMessage = "A Transaction Header ID in the order(s) doesn't match - " + saveObject.TransactionHeader_Row.TransactionReference;
                    logger.Error("A Transaction Header ID in the order(s) doesn't match " + saveObject.TransactionHeader_Row.TransactionReference);
                    ReturnState = 400;
                    SavedSuccessfully = false;
                    return true;
                }
                if (saveObject.ListTransactionDetail.Any(x => x.TransactionHeaderID != saveObject.TransactionHeader_Row.ID))
                {
                    ErrorMessage = "A Transaction Header ID in the detail(s) doesn't match - " + saveObject.TransactionHeader_Row.TransactionReference;
                    logger.Error("A Transaction Header ID in the detail(s) doesn't match " + saveObject.TransactionHeader_Row.TransactionReference);
                    ReturnState = 400;
                    SavedSuccessfully = false;
                    return true;
                }
                if (saveObject.ListTransactionException.Any(x => x.TransactionHeaderID != saveObject.TransactionHeader_Row.ID))
                {
                    ErrorMessage = "A Transaction Header ID in the exception(s) doesn't match - " + saveObject.TransactionHeader_Row.TransactionReference;
                    logger.Error("A Transaction Header ID in the exception(s) doesn't match " + saveObject.TransactionHeader_Row.TransactionReference);
                    ReturnState = 400;
                    SavedSuccessfully = false;
                    return true;
                }
                if (saveObject.ListTransactionNotes.Any(x => x.TransactionHeaderID != saveObject.TransactionHeader_Row.ID))
                {
                    ErrorMessage = "A Transaction Header ID in the note(s) doesn't match - " + saveObject.TransactionHeader_Row.TransactionReference;
                    logger.Error("A Transaction Header ID in the note(s) doesn't match " + saveObject.TransactionHeader_Row.TransactionReference);
                    ReturnState = 400;
                    SavedSuccessfully = false;
                    return true;
                }
                return false;
            }
            #endregion

            #region CheckForChanges
            public Boolean CheckForChanges()
            {
                CheckForChangesChanges_TransactionHeader();

                CheckForChanges_TransactionDetail();

                CheckForChangesChanges_PaperlessTransactionXref();

                CheckForChanges_TransactionException();

                CheckForChanges_ToDo();

                CheckForChangesChanges_OrderHeader();

                CheckForChangesChanges_PurchaseOrderHeader();

                return HasChanges;
            }

            public void CheckForChangesChanges_TransactionHeader()
            {
                ChangeTracking ct = new ChangeTracking();

                // Transaction Header
                if (ct.CheckForChanges(existObject.TransactionHeader_Row, saveObject.TransactionHeader_Row, wbm_common.DataObjects.TransactionHeader.ListChangeTrackerData) && ct.ListChangeDetail != null)
                {
                    HasChanges = true;
                    updateObject.TransactionHeader_Row = saveObject.TransactionHeader_Row;

                    changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.TransactionHeader,
                        TransactionHeaderID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);

                    ListChangeTrackingPair.Add(new ChangeTrackingPair(changeHeaderRow, ct));
                }
            }

            public void CheckForChanges_TransactionDetail()
            {
                // Transaction Details
                foreach (TransactionDetail.dbRow saveDetailRow in saveObject.ListTransactionDetail)
                {
                    if (saveDetailRow.ID < 1)
                    {
                        // New record
                        updateObject.ListTransactionDetail.Add(saveDetailRow);
                    }
                    else
                    {
                        TransactionDetail.dbRow existDetailRow = existObject.ListTransactionDetail.Where(x => x.ID == saveDetailRow.ID).FirstOrDefault();
                        if (existDetailRow != null)
                        {
                            // We have an existing row, check for changes
                            ChangeTracking ct = new ChangeTracking();
                            if (ct.CheckForChanges(existDetailRow, saveDetailRow, wbm_common.DataObjects.TransactionDetail.ListChangeTrackerData) && ct.ListChangeDetail != null)
                            {
                                HasChanges = true;
                                updateObject.ListTransactionDetail.Add(saveDetailRow);

                                changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.TransactionDetail,
    TransactionHeaderID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);

                                ListChangeTrackingPair.Add(new ChangeTrackingPair(changeHeaderRow, ct));
                            }
                        }
                        else
                        {
                            ErrorMessage = "Cannot find TransactionDetail in database to update - " + saveObject.TransactionHeader_Row.TransactionReference;
                            logger.Error("Cannot find TransactionDetail in database to update " + saveObject.TransactionHeader_Row.TransactionReference);
                            ReturnState = 400;
                            ContinueProcessing = false;
                        }
                    }
                }
            }

            public void CheckForChangesChanges_PaperlessTransactionXref()
            {
                ChangeTracking ct = new ChangeTracking();

                // Transaction Header
                if (ct.CheckForChanges(existObject.PaperlessTransactionXref_Row, saveObject.PaperlessTransactionXref_Row, wbm_common.DataObjects.TransactionHeader.ListChangeTrackerData) && ct.ListChangeDetail != null)
                {
                    HasChanges = true;
                    updateObject.PaperlessTransactionXref_Row = saveObject.PaperlessTransactionXref_Row;

                    changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.PaperlessTransactionXref,
                        TransactionHeaderID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);

                    ListChangeTrackingPair.Add(new ChangeTrackingPair(changeHeaderRow, ct));
                }
            }

            public void CheckForChanges_TransactionException()
            {
                // Transaction Details
                foreach (TransactionException.dbRow saveExceptionRow in saveObject.ListTransactionException)
                {
                    if (saveExceptionRow.ID < 1)
                    {
                        // New record
                        updateObject.ListTransactionException.Add(saveExceptionRow);
                    }
                    else
                    {
                        TransactionException.dbRow existExceptionRow = existObject.ListTransactionException.Where(x => x.ID == saveExceptionRow.ID).FirstOrDefault();
                        if (existExceptionRow != null)
                        {
                            // We have an existing row, check for changes
                            ChangeTracking ct = new ChangeTracking();
                            if (ct.CheckForChanges(existExceptionRow, saveExceptionRow, wbm_common.DataObjects.TransactionException.ListChangeTrackerData) && ct.ListChangeDetail != null)
                            {
                                HasChanges = true;
                                updateObject.ListTransactionException.Add(saveExceptionRow);

                                changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.TransactionException,
                                    TransactionHeaderID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);

                                ListChangeTrackingPair.Add(new ChangeTrackingPair(changeHeaderRow, ct));
                            }
                        }
                        else
                        {
                            ErrorMessage = "Cannot find TransactionException in database to update - " + saveObject.TransactionHeader_Row.TransactionReference;
                            logger.Error("Cannot find TransactionException in database to update " + saveObject.TransactionHeader_Row.TransactionReference);
                            ReturnState = 400;
                            ContinueProcessing = false;
                        }
                    }
                }
            }

            public void CheckForChanges_TransactionNotes()
            {
                // Transaction Details
                foreach (TransactionNotes.dbRow saveNotesRow in saveObject.ListTransactionNotes)
                {
                    if (saveNotesRow.ID < 1)
                    {
                        // New record
                        updateObject.ListTransactionNotes.Add(saveNotesRow);
                    }
                    else
                    {
                        TransactionNotes.dbRow existNotesRow = existObject.ListTransactionNotes.Where(x => x.ID == saveNotesRow.ID).FirstOrDefault();
                        if (existNotesRow != null)
                        {
                            // We have an existing row, check for changes
                            ChangeTracking ct = new ChangeTracking();
                            if (ct.CheckForChanges(existNotesRow, saveNotesRow, wbm_common.DataObjects.TransactionNotes.ListChangeTrackerData) && ct.ListChangeDetail != null)
                            {
                                HasChanges = true;
                                updateObject.ListTransactionNotes.Add(saveNotesRow);

                                changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.TransactionNotes,
    TransactionHeaderID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);

                                ListChangeTrackingPair.Add(new ChangeTrackingPair(changeHeaderRow, ct));
                            }
                        }
                        else
                        {
                            ErrorMessage = "Cannot find TransactionNotes in database to update - " + saveObject.TransactionHeader_Row.TransactionReference;
                            logger.Error("Cannot find TransactionNotes in database to update " + saveObject.TransactionHeader_Row.TransactionReference);
                            ReturnState = 400;
                            ContinueProcessing = false;
                        }
                    }
                }
            }

            public void CheckForChanges_ToDo()
            {
                // Transaction Details
                foreach (ToDo.dbRow saveToDoRow in saveObject.ListToDo)
                {
                    if (saveToDoRow.ID < 1)
                    {
                        // New record
                        updateObject.ListToDo.Add(saveToDoRow);
                    }
                    else
                    {
                        ToDo.dbRow existToDoRow = existObject.ListToDo.Where(x => x.ID == saveToDoRow.ID).FirstOrDefault();
                        if (existToDoRow != null)
                        {
                            // We have an existing row, check for changes
                            ChangeTracking ct = new ChangeTracking();
                            if (ct.CheckForChanges(existToDoRow, saveToDoRow, wbm_common.DataObjects.ToDo.ListChangeTrackerData) && ct.ListChangeDetail != null)
                            {
                                HasChanges = true;
                                updateObject.ListToDo.Add(saveToDoRow);

                                changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.ToDo,
                                    TransactionHeaderID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);

                                ListChangeTrackingPair.Add(new ChangeTrackingPair(changeHeaderRow, ct));
                            }
                        }
                        else
                        {
                            ErrorMessage = "Cannot find ToDo in database to update - " + saveObject.TransactionHeader_Row.TransactionReference;
                            logger.Error("Cannot find ToDo in database to update " + saveObject.TransactionHeader_Row.TransactionReference);
                            ReturnState = 400;
                            ContinueProcessing = false;
                        }
                    }
                }
            }

            public void CheckForChangesChanges_OrderHeader()
            {
                ChangeTracking ct = new ChangeTracking();

                // Order Header
                if (saveObject.OrderHeader_Row != null)
                {
                    if (existObject.OrderHeader_Row != null && ct.CheckForChanges(existObject.OrderHeader_Row, saveObject.OrderHeader_Row, wbm_common.DataObjects.OrderHeader.ListChangeTrackerData) && ct.ListChangeDetail != null)
                    {
                        HasChanges = true;
                        updateObject.OrderHeader_Row = saveObject.OrderHeader_Row;

                        changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.OrderHeader,
                            TransactionHeaderID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);

                        ListChangeTrackingPair.Add(new ChangeTrackingPair(changeHeaderRow, ct));
                    }
                    else if (saveObject.OrderHeader_Row.ID > 0)
                    {
                        ErrorMessage = "Cannot find OrderHeader in database to update - " + saveObject.TransactionHeader_Row.TransactionReference;
                        logger.Error("Cannot find OrderHeader in database to update " + saveObject.TransactionHeader_Row.TransactionReference);
                        ReturnState = 400;
                        ContinueProcessing = false;
                    }
                }
            }

            public void CheckForChangesChanges_PurchaseOrderHeader()
            {
                ChangeTracking ct = new ChangeTracking();

                // Purchase Order Header
                if (saveObject.PurchaseOrderHeader_Row != null)
                {
                    if (existObject.PurchaseOrderHeader_Row != null && ct.CheckForChanges(existObject.PurchaseOrderHeader_Row, saveObject.PurchaseOrderHeader_Row, wbm_common.DataObjects.PurchaseOrderHeader.ListChangeTrackerData) && ct.ListChangeDetail != null)
                    {
                        HasChanges = true;
                        updateObject.PurchaseOrderHeader_Row = saveObject.PurchaseOrderHeader_Row;

                        changeHeaderRow = WBM_API.Helpers.ChangeTracking.Create_ChangeHeader_dbRow((int)wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID.PurchaseOrderHeader,
                            TransactionHeaderID, saveObject.ChangedTime, ChangeDate, saveObject.CalledFrom, UserID);

                        ListChangeTrackingPair.Add(new ChangeTrackingPair(changeHeaderRow, ct));
                    }
                    else if (saveObject.PurchaseOrderHeader_Row.ID > 0)
                    {
                        ErrorMessage = "Cannot find PurchaseOrderHeader in database to update - " + saveObject.TransactionHeader_Row.TransactionReference;
                        logger.Error("Cannot find PurchaseOrderHeader in database to update " + saveObject.TransactionHeader_Row.TransactionReference);
                        ReturnState = 400;
                        ContinueProcessing = false;
                    }
                }
            }
            #endregion

            #region SetChanged

            public void SetChanged()
            {
                SetChanged_TransactionHeader();

                SetChanged_TransactionDetail();

                SetChanged_XRef();

                SetChanged_TransactionException();

                SetChanged_TransactionNotes();

                SetChanged_ToDo();

                SetChanged_OrderHeader();

                SetChanged_PurchaseOrderHeader();
            }

            public void SetChanged_XRef()
            {
                if (updateObject.PaperlessTransactionXref_Row.ID < 1)
                {
                    // New
                    updateObject.PaperlessTransactionXref_Row.CreatedDateTime = ChangeDate;
                    updateObject.PaperlessTransactionXref_Row.CreatedMethod = saveObject.CalledFrom;
                    updateObject.PaperlessTransactionXref_Row.CreatedUserID = UserID;
                }

                updateObject.PaperlessTransactionXref_Row.LastAmendedDateTime = ChangeDate;
                updateObject.PaperlessTransactionXref_Row.LastAmendedMethod = saveObject.CalledFrom;
                updateObject.PaperlessTransactionXref_Row.LastAmendedUserID = UserID;
            }

            public void SetChanged_TransactionHeader()
            {
                if (updateObject.TransactionHeader_Row.ID < 1)
                {
                    // New
                    updateObject.TransactionHeader_Row.CreatedDateTime = ChangeDate;
                    updateObject.TransactionHeader_Row.CreatedMethod = saveObject.CalledFrom;
                    updateObject.TransactionHeader_Row.CreatedUserID = UserID;
                }

                updateObject.TransactionHeader_Row.LastAmendedDateTime = ChangeDate;
                updateObject.TransactionHeader_Row.LastAmendedMethod = saveObject.CalledFrom;
                updateObject.TransactionHeader_Row.LastAmendedUserID = UserID;
            }

            public void SetChanged_TransactionDetail()
            {
                // Transaction Details
                foreach (TransactionDetail.dbRow updateDetailRow in updateObject.ListTransactionDetail)
                {
                    if (updateDetailRow.ID < 1)
                    {
                        // New record
                        updateDetailRow.CreatedDateTime = ChangeDate;
                        updateDetailRow.CreatedMethod = saveObject.CalledFrom;
                        updateDetailRow.CreatedUserID = UserID;
                    }

                    updateDetailRow.LastAmendedDateTime = ChangeDate;
                    updateDetailRow.LastAmendedMethod = saveObject.CalledFrom;
                    updateDetailRow.LastAmendedUserID = UserID;
                }
            }

            public void SetChanged_TransactionException()
            {
                // Transaction Exceptions
                foreach (TransactionException.dbRow updateExceptionRow in updateObject.ListTransactionException)
                {
                    if (updateExceptionRow.ID < 1)
                    {
                        // New record
                        updateExceptionRow.CreatedDateTime = ChangeDate;
                        updateExceptionRow.CreatedMethod = saveObject.CalledFrom;
                        updateExceptionRow.CreatedUserID = UserID;
                    }

                    updateExceptionRow.LastAmendedDateTime = ChangeDate;
                    updateExceptionRow.LastAmendedMethod = saveObject.CalledFrom;
                    updateExceptionRow.LastAmendedUserID = UserID;
                }
            }

            public void SetChanged_TransactionNotes()
            {
                // Transaction Notess
                foreach (TransactionNotes.dbRow updateNotesRow in updateObject.ListTransactionNotes)
                {
                    if (updateNotesRow.ID < 1)
                    {
                        // New record
                        updateNotesRow.CreatedDateTime = ChangeDate;
                        updateNotesRow.CreatedMethod = saveObject.CalledFrom;
                        updateNotesRow.CreatedUserID = UserID;
                    }

                    updateNotesRow.LastAmendedDateTime = ChangeDate;
                    updateNotesRow.LastAmendedMethod = saveObject.CalledFrom;
                    updateNotesRow.LastAmendedUserID = UserID;
                }
            }

            public void SetChanged_ToDo()
            {
                //  ToDos
                foreach (ToDo.dbRow updateToDoRow in updateObject.ListToDo)
                {
                    if (updateToDoRow.ID < 1)
                    {
                        // New record
                        updateToDoRow.CreatedDateTime = ChangeDate;
                        updateToDoRow.CreatedMethod = saveObject.CalledFrom;
                        updateToDoRow.CreatedUserID = UserID;
                    }

                    updateToDoRow.LastAmendedDateTime = ChangeDate;
                    updateToDoRow.LastAmendedMethod = saveObject.CalledFrom;
                    updateToDoRow.LastAmendedUserID = UserID;
                }
            }

            public void SetChanged_OrderHeader()
            {
                if (saveObject.OrderHeader_Row != null)
                {
                    if (saveObject.OrderHeader_Row.ID < 1)
                    {
                        // New
                        saveObject.OrderHeader_Row.CreatedDateTime = ChangeDate;
                        saveObject.OrderHeader_Row.CreatedMethod = saveObject.CalledFrom;
                        saveObject.OrderHeader_Row.CreatedUserID = UserID;
                    }

                    saveObject.OrderHeader_Row.LastAmendedDateTime = ChangeDate;
                    saveObject.OrderHeader_Row.LastAmendedMethod = saveObject.CalledFrom;
                    saveObject.OrderHeader_Row.LastAmendedUserID = UserID;
                }
            }
            public void SetChanged_PurchaseOrderHeader()
            {
                if (saveObject.PurchaseOrderHeader_Row != null)
                {
                    if (saveObject.PurchaseOrderHeader_Row.ID < 1)
                    {
                        // New
                        saveObject.PurchaseOrderHeader_Row.CreatedDateTime = ChangeDate;
                        saveObject.PurchaseOrderHeader_Row.CreatedMethod = saveObject.CalledFrom;
                        saveObject.PurchaseOrderHeader_Row.CreatedUserID = UserID;
                    }

                    saveObject.PurchaseOrderHeader_Row.LastAmendedDateTime = ChangeDate;
                    saveObject.PurchaseOrderHeader_Row.LastAmendedMethod = saveObject.CalledFrom;
                    saveObject.PurchaseOrderHeader_Row.LastAmendedUserID = UserID;
                }
            }

            #endregion

            #region Save

            public Boolean Save_Object()
            {
                Boolean rtnVal = false;

                using var transaction = _WBMDB.dbContext.Database.BeginTransaction();

                transaction.CreateSavepoint("Start");


                if (Save_TransactionHeader())
                {
                    if (Save_TransactionDetail())
                    {
                        if (Save_TransactionException())
                        {
                            if (Save_TransactionNotes())
                            {
                                if (Save_ToDo())
                                {
                                    if (Save_ChangeTracking())
                                    {
                                        if (DeleteAwaitingRecord())
                                        {
                                            if (Save_Xref())
                                            {
                                                if (Save_OrderHeaders())
                                                    rtnVal = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (rtnVal)
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

            public Boolean Save_TransactionHeader()
            {
                Boolean rtnVal = true;

                Result<int> sr = null;
                // this might be the wrong check, but this should mean it's paperless
                if (saveObject.PaperlessTransactionXref_Row != null && saveObject.PaperlessTransactionXref_Row.PaperlessKeyID != null
                    && saveObject.PaperlessTransactionXref_Row.PaperlessKeyID != "")
                    updateObject.TransactionHeader_Row.TransactionReference = saveObject.PaperlessTransactionXref_Row.ReferenceNumber;

                if (updateObject.TransactionHeader_Row.ID < 1)
                    sr = _WBMDB.TransactionHeader_Insert(updateObject.TransactionHeader_Row).GetAwaiter().GetResult();
                else
                    sr = _WBMDB.TransactionHeader_Update(updateObject.TransactionHeader_Row).GetAwaiter().GetResult();

                if (sr.IsSuccess)
                {
                    if (TransactionHeaderID < 1)
                        TransactionHeaderID = sr.Value;
                }
                else
                {
                    ErrorMessage = sr.ErrorMessage;
                    if (sr.IsException)
                        ReturnState = 500;
                    else
                        ReturnState = 400;
                    rtnVal = false;
                }

                return rtnVal;
            }

            public Boolean Save_TransactionDetail()
            {
                Boolean rtnVal = true;
                Result<int> sr = null;

                int DetailID = 0;

                // Transaction Details
                foreach (TransactionDetail.dbRow updateDetailRow in updateObject.ListTransactionDetail)
                {
                    DetailID = updateDetailRow.ID;

                    if (updateDetailRow.ID < 1)
                    {
                        updateDetailRow.TransactionHeaderID = TransactionHeaderID;
                        sr = _WBMDB.TransactionDetail_Insert(updateDetailRow).GetAwaiter().GetResult();
                    }
                    else
                        sr = _WBMDB.TransactionDetail_Update(updateDetailRow).GetAwaiter().GetResult();

                    if (sr.IsSuccess)
                    {
                        if (DetailID < 1)
                        {
                            // Look for exceptions or notes that also reference this TransactionDetail
                            foreach (TransactionNotes.dbRow tn in updateObject.ListTransactionNotes)
                            {
                                if (tn.TransactionDetailID == updateDetailRow.TemporaryID)
                                    tn.TransactionDetailID = updateDetailRow.ID;
                            }

                            foreach (TransactionException.dbRow te in updateObject.ListTransactionException)
                            {
                                if (te.TransactionDetailID == updateDetailRow.TemporaryID)
                                    te.TransactionDetailID = updateDetailRow.ID;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = sr.ErrorMessage;
                        if (sr.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;
                        rtnVal = false;
                        break;
                    }
                }

                return rtnVal;
            }

            public Boolean Save_TransactionException()
            {
                Boolean rtnVal = true;
                Result<int> sr = null;

                // Transaction Exceptions
                foreach (TransactionException.dbRow updateExceptionRow in updateObject.ListTransactionException)
                {
                    if (updateExceptionRow.ID < 1)
                    {
                        updateExceptionRow.TransactionHeaderID = TransactionHeaderID;
                        sr = _WBMDB.TransactionException_Insert(updateExceptionRow).GetAwaiter().GetResult();
                    }
                    else
                        sr = _WBMDB.TransactionException_Update(updateExceptionRow).GetAwaiter().GetResult();


                    if (!sr.IsSuccess)
                    {
                        ErrorMessage = sr.ErrorMessage;
                        if (sr.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;
                        rtnVal = false;
                        break;
                    }

                }

                return rtnVal;

            }

            public Boolean Save_TransactionNotes()
            {
                Boolean rtnVal = true;
                Result<int> sr = null;

                // Transaction Notess
                foreach (TransactionNotes.dbRow updateNotesRow in updateObject.ListTransactionNotes)
                {
                    if (updateNotesRow.ID < 1)
                    {
                        updateNotesRow.TransactionHeaderID = TransactionHeaderID;
                        sr = _WBMDB.TransactionNotes_Insert(updateNotesRow).GetAwaiter().GetResult();
                    }
                    else
                        sr = _WBMDB.TransactionNotes_Update(updateNotesRow).GetAwaiter().GetResult();


                    if (!sr.IsSuccess)
                    {
                        ErrorMessage = sr.ErrorMessage;
                        if (sr.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;
                        rtnVal = false;
                        break;
                    }
                }

                return rtnVal;

            }

            public Boolean Save_ToDo()
            {
                Boolean rtnVal = true;
                Result<int> sr = null;

                // Transaction Details
                foreach (ToDo.dbRow todoRow in updateObject.ListToDo)
                {
                    if (todoRow.ID < 1)
                    {
                        todoRow.SourceID = TransactionHeaderID;
                        sr = _WBMDB.ToDo_Insert(todoRow).GetAwaiter().GetResult();
                    }
                    else
                        sr = _WBMDB.ToDo_Update(todoRow).GetAwaiter().GetResult();


                    if (!sr.IsSuccess)
                    {
                        ErrorMessage = sr.ErrorMessage;
                        if (sr.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;

                        rtnVal = false;
                        break;
                    }

                }
                return rtnVal;
            }

            public Boolean Save_ChangeTracking()
            {
                Boolean rtnVal = true;
                Result<int> sr = null;

                // Transaction Details
                foreach (ChangeTrackingPair ctp in ListChangeTrackingPair)
                {
                    Result<int> chResult = _WBMDB.ChangeHeader_Insert(ctp.ChangeHeaderRow).GetAwaiter().GetResult();
                    if (chResult.IsSuccess)
                    {
                        ctp.ChangeHeaderRow.ID = chResult.Value;

                        int ChangeHeaderTableID = ctp.ChangeHeaderRow.ChangeHeaderTableID;

                        foreach (wbm_common.DataObjects.ChangeDetail.dbRow cdRow in ctp.ct.ListChangeDetail)
                        {
                            cdRow.ChangeHeaderID = changeHeaderRow.ID;
                            cdRow.ChangeHeaderTableID = ChangeHeaderTableID;
                            cdRow.RecordID = changeHeaderRow.RecordID;

                            Result<int> cdResult = _WBMDB.ChangeDetail_Insert((wbm_common.DataObjects.ChangeDetail.dbRow)cdRow).GetAwaiter().GetResult();
                            if (cdResult.IsFailure)
                            {
                                // Error occured
                                rtnVal = false;
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
                        rtnVal = false;
                        ErrorMessage = chResult.ErrorMessage;
                        if (chResult.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;
                    }

                    if (!rtnVal)
                        break;
                }
                return rtnVal;
            }

            public Boolean DeleteAwaitingRecord()
            {
                Boolean rtnVal = true;

                // awaiting records are ones we're waiting for more database records to be supplied to process. These should get deleted on 
                // brand new transactions
                if (!Exists)
                {
                    Result<int> sr = null;

                    switch (updateObject.TransactionHeader_Row.TransactionTypeID)
                    {
                        case 1: // order
                            sr = _WBMDB.AwaitingPickHeadDW_DeleteByKEYID(updateObject.TransactionHeader_Row.TransactionReference).GetAwaiter().GetResult();
                            break;
                        case 2: //receipt
                            sr = _WBMDB.AwaitingPurchOrdDW_DeleteByKEYID(updateObject.TransactionHeader_Row.TransactionReference).GetAwaiter().GetResult();
                            break;
                        default:
                            sr = Result.Success<int>(0);
                            break;
                    }

                    if (!sr.IsSuccess)
                    {
                        ErrorMessage = sr.ErrorMessage;
                        if (sr.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;
                        rtnVal = false;
                    }
                }
                return rtnVal;
            }

            public Boolean Save_Xref()
            {
                Boolean rtnVal = true;

                // These should only be executed on paperless calls on a new transaction, not on updates
                if (!Exists && saveObject.PaperlessTransactionXref_Row != null && saveObject.PaperlessTransactionXref_Row.PaperlessKeyID != null
                    && saveObject.PaperlessTransactionXref_Row.PaperlessKeyID != "")
                {
                    switch (updateObject.TransactionHeader_Row.TransactionTypeID)
                    {
                        case 1: // order
                            saveObject.PaperlessTransactionXref_Row.PaperlessDataTypeID = 1;
                            break;
                        case 2: //receipt
                            saveObject.PaperlessTransactionXref_Row.PaperlessDataTypeID = 2;
                            break;
                        default:
                            return true;
                    }
                    saveObject.PaperlessTransactionXref_Row.TransactionHeaderID = TransactionHeaderID;
                    Result<int> sr = _WBMDB.PaperlessTransactionXref_Insert(saveObject.PaperlessTransactionXref_Row).GetAwaiter().GetResult();
                    if (!sr.IsSuccess)
                    {
                        ErrorMessage = sr.ErrorMessage;
                        if (sr.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;
                        rtnVal = false;
                    }
                }

                return rtnVal;
            }

            public Boolean Save_OrderHeaders()
            {
                Boolean rtnVal = true;
                Result<int> sr;
                // these should only be executing on manhatten files
                if (saveObject.OrderHeader_Row != null)
                {
                    saveObject.OrderHeader_Row.TransactionHeaderID = TransactionHeaderID;
                    if (saveObject.OrderHeader_Row.ID > 0)
                        sr = _WBMDB.OrderHeader_Update(saveObject.OrderHeader_Row).GetAwaiter().GetResult();
                    else
                        sr = _WBMDB.OrderHeader_Insert(saveObject.OrderHeader_Row).GetAwaiter().GetResult();
                    if (!sr.IsSuccess)
                    {
                        ErrorMessage = sr.ErrorMessage;
                        if (sr.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;
                        rtnVal = false;
                    }
                }

                if (saveObject.PurchaseOrderHeader_Row != null)
                {
                    saveObject.PurchaseOrderHeader_Row.TransactionHeaderID = TransactionHeaderID;
                    if (saveObject.PurchaseOrderHeader_Row.ID > 0)
                        sr = _WBMDB.PurchaseOrderHeader_Update(saveObject.PurchaseOrderHeader_Row).GetAwaiter().GetResult();
                    else
                        sr = _WBMDB.PurchaseOrderHeader_Insert(saveObject.PurchaseOrderHeader_Row).GetAwaiter().GetResult();
                    if (!sr.IsSuccess)
                    {
                        ErrorMessage = sr.ErrorMessage;
                        if (sr.IsException)
                            ReturnState = 500;
                        else
                            ReturnState = 400;
                        rtnVal = false;
                    }
                }

                return rtnVal;
            }

            #endregion
        }
    }
}
