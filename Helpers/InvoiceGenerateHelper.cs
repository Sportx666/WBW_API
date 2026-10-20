using NLog;
using NLog.Web;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;


namespace WBM_API.Helpers
{
    public class InvoiceGenerateHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public InvoiceGenerateHolder IGH { get; set; }

        public int UserID { get; set; }

        private int CurrentOwnerID { get; set; }

        private List<TransactionHeader.dbRow> CurrentListTransactionHeader { get; set; }

        private Owner.dbRow CurrentOwnerRow { get; set; }
        private InvoiceHeader.dbRow CurrentIHRow { get; set; }

        private int CurrentInvoiceNumber { get; set; }
        private int CurrentInvoiceHeaderID { get; set; }

        private WBMDatabase _WBMDB;

        public Result<InvoiceGenerateReturn> BatchInvoiceGenerate(wbm_common.NonDatabaseObjects.InvoiceGenerateHolder IGH, int UserID, WBMDatabase WBMDB)
        {
            this.IGH = IGH;
            this.UserID = UserID;
            this._WBMDB = WBMDB;

            Boolean ExceptionOccurred = false;

            InvoiceGenerateReturn IGR = new InvoiceGenerateReturn();
            IGR.ListOwnerResult = new List<InvoiceGenerateReturn.OwnerResult>();

            if (IGH.ListOwnerID != null && IGH.ListOwnerID.Count > 0)
            {
                foreach (int ownerID in IGH.ListOwnerID)
                {
                    CurrentOwnerID = ownerID;
                    Result<Boolean> rc = InvoiceGenerateForOwner();

                    if (rc.IsSuccess)
                    {
                        IGR.ListOwnerResult.Add(new InvoiceGenerateReturn.OwnerResult(CurrentOwnerID, CurrentIHRow.InvoicePrefix + CurrentInvoiceNumber.ToString(), "", true));
                    }
                    else
                    {
                        if (rc.IsFailure)
                        {
                            IGR.ListOwnerResult.Add(new InvoiceGenerateReturn.OwnerResult(CurrentOwnerID, "", rc.ErrorMessage, false));
                        }
                        else
                        {
                            // Must be exception
                            IGR.ListOwnerResult.Add(new InvoiceGenerateReturn.OwnerResult(CurrentOwnerID, "", "Internal Error", false));
                            ExceptionOccurred = true;
                            break;
                        }
                    }
                }
            }

            if (ExceptionOccurred)
            {
                return Result.Exception<InvoiceGenerateReturn>(IGR, "Internal Error");
            }
            else
                return Result.Success<InvoiceGenerateReturn>(IGR);
        }

        private Result<Boolean> InvoiceGenerateForOwner()
        {
            // Check customer to see if we have already invoiced

            Result<Boolean> customerR = CheckCustomer();

            if(customerR.IsSuccess)
            {
                Result<Boolean> getdataR = GetTransactionHeaders();

                if (getdataR.IsSuccess)
                {
                    if (CurrentListTransactionHeader != null && CurrentListTransactionHeader.Count > 0)
                    {
                        // We have transactions to process, so we will create the invoice

                        Result<int> getInvoiceNumber = GetInvoiceNumber();
                        if (getInvoiceNumber.IsSuccess)
                        {
                            CurrentInvoiceNumber = getInvoiceNumber.Value;

                            CreateInvoiceHeader();

                            return Save_Records();
                        }
                        else
                        {
                            if (getInvoiceNumber.IsException)
                                return Result.Exception<Boolean>(getInvoiceNumber.ErrorMessage);
                            else
                                return Result.Failure<Boolean>(getInvoiceNumber.ErrorMessage);
                        }
                    }
                    else
                    {
                        return Result.Failure<Boolean>("No transactions to invoice");
                    }
                }
                else
                {
                    if (getdataR.IsException)
                        return Result.Exception<Boolean>(getdataR.ErrorMessage);
                    else
                        return Result.Failure<Boolean>(getdataR.ErrorMessage);
                }
            }
            else
            {
                if (customerR.IsException)
                    return Result.Exception<Boolean>(customerR.ErrorMessage);
                else
                    return Result.Failure<Boolean>(customerR.ErrorMessage);
            }
        }

        private Result<Boolean> CheckCustomer()
        {
            Result<wbm_common.DataObjects.Owner.dbRow> OwnerRC = _WBMDB.Owner_SearchByID(CurrentOwnerID).GetAwaiter().GetResult();

            if (OwnerRC.IsSuccess)
            {
                CurrentOwnerRow = OwnerRC.Value;

                if (CurrentOwnerRow.LastInvoiceDate != null && CurrentOwnerRow.LastInvoiceDate >= IGH.InvoiceDate)
                {
                    return Result.Failure<Boolean>("Customer has already been invoiced");
                }
            }
            else
            {
                if (OwnerRC.IsException)
                    return Result.Exception<Boolean>(OwnerRC.ErrorMessage);
                else
                    return Result.Failure<Boolean>(OwnerRC.ErrorMessage);
            }

            return Result.Success<Boolean>(true);

        }

        private Result<int> GetInvoiceNumber()
        {
            return _WBMDB.Counters_GetCounter(1).GetAwaiter().GetResult();
        }

        private Result<Boolean> GetTransactionHeaders()
        {
            Result<Boolean> rtnResult = null;

            Result<List<TransactionHeader.dbRow>> rc = _WBMDB.TransactionHeader_ListByOwnerIDCanInvoiceCutDate(CurrentOwnerID, IGH.InvoiceDate).GetAwaiter().GetResult();

            if (rc.IsSuccess)
            {
                CurrentListTransactionHeader = rc.Value;
                rtnResult = Result.Success<Boolean>(true);
            }
            else
            {
                if (rc.IsFailure)
                    rtnResult = Result.Failure<Boolean>(rc.ErrorMessage);
                else
                    rtnResult = Result.Exception<Boolean>(rc.ErrorMessage);
            }

            return rtnResult;
        }

        private void CreateInvoiceHeader()
        {
            CurrentIHRow = new InvoiceHeader.dbRow();

            CurrentIHRow.InvoiceNumber = CurrentInvoiceNumber;
            CurrentIHRow.InvoiceDate = IGH.InvoiceDate;
            CurrentIHRow.InvoiceCycleID = IGH.InvoiceCycleID;
            CurrentIHRow.CompanyID = IGH.CompanyID;
            CurrentIHRow.SiteID = IGH.SiteID;
            CurrentIHRow.OwnerID = CurrentOwnerID;
            CurrentIHRow.InvoicePrefix = "";

            CurrentIHRow.InvoiceFormatID = 1;
            CurrentIHRow.FirstExportDateTime = null;

            CurrentIHRow.DeletedFlag = false;
            CurrentIHRow.CreatedDateTime = DateTime.Now;
            CurrentIHRow.CreatedMethod = "BatchInvoiceGenerate";
            CurrentIHRow.CreatedUserID = UserID;

            CurrentIHRow.LastAmendedDateTime = CurrentIHRow.CreatedDateTime;
            CurrentIHRow.LastAmendedMethod = CurrentIHRow.CreatedMethod;
            CurrentIHRow.LastAmendedUserID = CurrentIHRow.CreatedUserID;

            //Calculate totals
            foreach (TransactionHeader.dbRow thRow in CurrentListTransactionHeader)
            {
                CurrentIHRow.ClientExGSTAmount += thRow.InvoiceExGST;
            }

            CurrentIHRow.ClientGSTAmount = Math.Round(CurrentIHRow.ClientExGSTAmount * 0.1m, 2);
            CurrentIHRow.ClientTotalAmount = CurrentIHRow.ClientExGSTAmount + CurrentIHRow.ClientGSTAmount;

        }

        #region Save

        public Result<Boolean> Save_Records()
        {
            string ErrorMessage = null;

            using var transaction = _WBMDB.dbContext.Database.BeginTransaction();

            transaction.CreateSavepoint("Start");

            Result<int> ihR = Save_InvoiceHeader();

            if (ihR.IsSuccess)
            {

                CurrentInvoiceHeaderID = ihR.Value;

                Result<Boolean> thR = Update_TransactionHeader();

                if (thR.IsSuccess)
                {
                    Result<Boolean> oR = Update_Owner();

                    if (!oR.IsSuccess)
                        ErrorMessage = oR.ErrorMessage;
                }
                else
                {
                    ErrorMessage = thR.ErrorMessage;
                }
            }
            else
            {
                ErrorMessage = ihR.ErrorMessage;
            }

            if (ErrorMessage == null)
            {
                transaction.Commit();
                return Result.Success<Boolean>(true);
            }
            else
            {
                transaction.RollbackToSavepoint("Start");
                return Result.Exception<Boolean>(ErrorMessage);
            }
        }

        public Result<int> Save_InvoiceHeader()
        {
            return _WBMDB.InvoiceHeader_Insert(CurrentIHRow).GetAwaiter().GetResult();
        }

        public Result<Boolean> Update_TransactionHeader()
        {
            List<int> ListTHID = new List<int>();

            foreach (TransactionHeader.dbRow thRow in CurrentListTransactionHeader)
            {
                ListTHID.Add(thRow.ID);
            }

            if (ListTHID.Count > 0)
                return _WBMDB.TransactionHeader_UpdateInvoiced(CurrentInvoiceHeaderID, IGH.InvoiceDate, ListTHID).GetAwaiter().GetResult();
            else
                return Result.Failure<Boolean>("No transaction headers to update");
        }


        public Result<Boolean> Update_Owner()
        {
            return _WBMDB.Owner_UpdateInvoiced(CurrentOwnerID, IGH.InvoiceDate).GetAwaiter().GetResult();
        }
        #endregion

    }
}



