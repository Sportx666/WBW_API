using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using System.Text;
using System.Threading.Tasks;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;
using wbm_common.Utility;


namespace WBM_API.Helpers
{
    public class InvoiceHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public class PreInvoiceGenerateStatistics
        {
            private WBMDatabase _WBMDB;

            private PreInvoiceGenerateStats rtnStat = new PreInvoiceGenerateStats();

            private List<TransactionHeader.dbRow> ListTHRow { get; set; }

            private int CurrentOwnerID = 0;
            private int CompanyID = 0;
            private DateTime CurrentCutDate = DateTime.MinValue;
            private Owner.dbRow CurrentOwnerdbRow = null;
            private List<int> ListOwnerIDAlreadyProcessed = new List<int>();

            private PreInvoiceGenerateStats.OwnerStat CurrentOS { get; set; }

            private Dictionary<int, DateTime> DictInvoiceCycleCutDate = new Dictionary<int, DateTime>();
            private List<Owner.dbRow> ListOwner { get; set; }

            public Result<PreInvoiceGenerateStats> Generate(PreInvoiceGenerateStats sendObject, WBMDatabase WBMDB)
            {
                this._WBMDB = WBMDB;
                this.CompanyID = sendObject.CompanyID;

                Boolean ExceptionOccurred = false;

                Result<Boolean> InvCRC = GetInvoiceCycleCurrent();

                if (InvCRC.IsSuccess)
                {
                    Result ownRC = GetOwners();

                    if (ownRC.IsSuccess)
                    {
                        Result<Boolean> thRC = GetTransactionHeaders();

                        if (thRC.IsSuccess)
                        {
                            Result<Boolean> procRC = ProcessInvoiceHeader();
                            
                            if (!procRC.IsSuccess)
                            {
                                rtnStat.ErrorMessage = procRC.ErrorMessage;
                                ExceptionOccurred = procRC.IsException;
                            }
                        }
                        else
                        {
                            rtnStat.ErrorMessage = thRC.ErrorMessage;
                            ExceptionOccurred = thRC.IsException;
                        }
                    }
                    else
                    {
                        rtnStat.ErrorMessage = ownRC.ErrorMessage;
                        ExceptionOccurred = ownRC.IsException;
                    }
                }
                else
                {
                    rtnStat.ErrorMessage = InvCRC.ErrorMessage;
                    ExceptionOccurred = InvCRC.IsException;
                }

                if (ExceptionOccurred)
                {
                    return Result.Exception<PreInvoiceGenerateStats>(rtnStat, "Internal Error");
                }
                else
                    return Result.Success<PreInvoiceGenerateStats>(rtnStat);
            }


            private Result<Boolean> GetInvoiceCycleCurrent()
            {
                Result<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>> ListInvCRC = _WBMDB.InvoiceCycleCurrent_ListForCompany(CompanyID).GetAwaiter().GetResult();

                if (ListInvCRC.IsSuccess)
                {
                    rtnStat.ListInvoiceCycleCurrent = ListInvCRC.Value;

                    foreach (InvoiceCycleCurrent.dbRow iccRow in ListInvCRC.Value)
                    {
                        DictInvoiceCycleCutDate.Add(iccRow.InvoiceCycleID, iccRow.CurrentPeriodEnding);
                    }

                }
                else
                {
                    if (ListInvCRC.IsException)
                        return Result.Exception<Boolean>(ListInvCRC.ErrorMessage);
                    else
                        return Result.Failure<Boolean>(ListInvCRC.ErrorMessage);
                }

                return Result.Success<Boolean>(true);
            }

            private Result<Boolean> GetOwners()
            {
                Result<List<wbm_common.DataObjects.Owner.dbRow>> ListOwnerCRC = _WBMDB.Owner_ListBySingleCompanyID(CompanyID).GetAwaiter().GetResult();

                if (ListOwnerCRC.IsSuccess)
                {
                    ListOwner = ListOwnerCRC.Value;
                }
                else
                {
                    if (ListOwnerCRC.IsException)
                        return Result.Exception<Boolean>(ListOwnerCRC.ErrorMessage);
                    else
                        return Result.Failure<Boolean>(ListOwnerCRC.ErrorMessage);
                }

                return Result.Success<Boolean>(true);
            }


            private Result<Boolean> GetTransactionHeaders()
            {
                Result<Boolean> rtnResult = null;

                Result<List<TransactionHeader.dbRow>> rc = _WBMDB.TransactionHeader_ListAllUnInvoiced().GetAwaiter().GetResult();

                if (rc.IsSuccess)
                {
                    ListTHRow = rc.Value;
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

            private Result<Boolean> ProcessInvoiceHeader()
            {
                string CurrentCategory = null;

                // this is sorted already as the dbquery gets it and sorts it. It will create weird duplicate customers without it.
                foreach (TransactionHeader.dbRow thRow in ListTHRow)
                {
                    #region Check for change of owner
                    // CHeck for change of owner; list ordered by owner ID
                    if (thRow.OwnerID != CurrentOwnerID)
                    {
                        Result<Boolean> soRC = SetOwner(thRow.OwnerID);

                        if (!soRC.IsSuccess)
                        {
                            return soRC;
                        }
                    }
                    #endregion

                    #region Get Category
                    // categories are in order...
                    // Dated past current cut date
                    // Held, not ready for invoicing
                    // transaction source
                    if (thRow.TransactionDateTime > CurrentCutDate)
                        CurrentCategory = "Future";
                    else
                    {
                        if (!thRow.CanInvoice)
                            CurrentCategory = "Held";
                        else
                        {
                            switch (thRow.TransactionTypeID)
                            {
                                case 1:
                                    // Order
                                    CurrentCategory = "Order";
                                    break;
                                case 2:
                                    CurrentCategory = "Receipt";
                                    break;
                                case 3:
                                    CurrentCategory = "Storage";
                                    break;
                                default:
                                    CurrentCategory = "Other";
                                    break;
                            }
                        }
                    }
                    #endregion

                    CurrentOS.AddStat(CurrentCategory, 1, thRow.InvoiceExGST);

                }

                return Result.Success<Boolean>(true);
            }

            private Result<Boolean> SetOwner(int newOwnerID)
            {
                if (ListOwnerIDAlreadyProcessed.Contains(newOwnerID))
                {
                    // Should not be here but just in case..
                    CurrentOS = rtnStat.ListOwnerStats.FirstOrDefault(x => x.OwnerID == newOwnerID);

                    if (CurrentOS == null)
                    {
                        CurrentOS = new PreInvoiceGenerateStats.OwnerStat();
                        CurrentOS.OwnerID = newOwnerID;
                        rtnStat.ListOwnerStats.Add(CurrentOS);
                    }
                    CurrentOwnerID = newOwnerID;
                }
                else
                {
                    CurrentOwnerID = newOwnerID;
                    CurrentOS = new PreInvoiceGenerateStats.OwnerStat();
                    CurrentOS.OwnerID = CurrentOwnerID;

                    rtnStat.ListOwnerStats.Add(CurrentOS);
                }

                CurrentOwnerdbRow = ListOwner.FirstOrDefault(x => x.ID == CurrentOwnerID);

                if (CurrentOwnerdbRow == null)
                {
                    // No owner.. fault...
                    return Result.Failure<Boolean>("Owner ID of " + CurrentOwnerID.ToString() + " not found in Owner Table for Company " + CompanyID.ToString());
                }

                if (DictInvoiceCycleCutDate.TryGetValue(CurrentOwnerdbRow.InvoiceCycleID, out DateTime iccDateTime))
                {
                    CurrentCutDate = iccDateTime;
                }
                else
                {
                    return Result.Failure<Boolean>("Owner ID of " + CurrentOwnerID.ToString() + " Invoice cycle ID of " + CurrentOwnerdbRow.InvoiceCycleID.ToString() + " not found in list of cycles");
                }

                CurrentOS.OwnerName = CurrentOwnerdbRow.Name;
                CurrentOS.LastInvoiceDate = CurrentOwnerdbRow.LastInvoiceDate;
                CurrentOS.InvoiceCycleID = CurrentOwnerdbRow.InvoiceCycleID;

                return Result.Success<Boolean>(true);
            }

        }


        public class PreInvoicePeriodClose
        {
            /* For customers on this cycle:
             * Not been invoiced, and have transactions - critical
             * Invoice not exported - critical
             * Check for customers with invalid invoice cycle - warning
             */

            private WBMDatabase _WBMDB;

            private PreInvoiceCloseCheck rtnCheck = new PreInvoiceCloseCheck();

            private int CompanyID = 0;
            private int InvoiceCycleID = 0;

            private DateTime CurrentCloseDate = DateTime.MinValue;
            private DateTime RequestedCloseDate = DateTime.MinValue;
            private DateTime? NextPeriodEnding = null;

            private List<Owner.dbRow> ListOwner { get; set; }
            public InvoiceCycleCurrent.dbRow CurrentInvoiceCycle { get; set; }

            private int CurrentOwnerID { get; set; }
            private Boolean ProcessOwner { get; set; }

            private List<TransactionHeader.dbRow> ListTHRow { get; set; }
            private List<InvoiceHeader.dbRow> ListIHRow { get; set; }

            private List<InvoiceCycleCurrent.dbRow> ListICCRow { get; set; }
            private List<int> ListInvoiceCycleCurrentID { get; set; }

            public Result<PreInvoiceCloseCheck> Generate(PreInvoiceCloseCheck sendObject, WBMDatabase WBMDB)
            {
                this._WBMDB = WBMDB;
                this.CompanyID = sendObject.CompanyID;
                this.InvoiceCycleID = sendObject.InvoiceCycleID;
                this.NextPeriodEnding = sendObject.NextPeriodEnding;

                Boolean ExceptionOccurred = false;

                Result<Boolean> InvCRC = GetInvoiceCycleCurrent();

                if (InvCRC.IsSuccess)
                {
                    Result ownRC = GetOwners();

                    if (ownRC.IsSuccess)
                    {
                        // Look for uninvoiced transactions
                        Result<Boolean> procRC = ProcessTransactionHeaders();

                        if (!procRC.IsSuccess)
                        {
                            // Look for invoices not yet exported
                            Result<Boolean> invexpRC = ProcessInvoiceHeaders();

                            if (invexpRC.IsSuccess)
                            {
                                rtnCheck.ErrorMessage = procRC.ErrorMessage;
                                ExceptionOccurred = procRC.IsException;
                            }
                        }
                        else
                        {
                            rtnCheck.ErrorMessage = procRC.ErrorMessage;
                            ExceptionOccurred = procRC.IsException;
                        }
                    }
                    else
                    {
                        rtnCheck.ErrorMessage = ownRC.ErrorMessage;
                        ExceptionOccurred = ownRC.IsException;
                    }
                }
                else
                {
                    rtnCheck.ErrorMessage = InvCRC.ErrorMessage;
                    ExceptionOccurred = InvCRC.IsException;
                }

                if (ExceptionOccurred)
                {
                    return Result.Exception<PreInvoiceCloseCheck>(rtnCheck, "Internal Error");
                }
                else
                    return Result.Success<PreInvoiceCloseCheck>(rtnCheck);
            }

            private Dictionary<int, PreInvoiceGenerateStats.DollarCount> DictUnInvoiced = new Dictionary<int, PreInvoiceGenerateStats.DollarCount>();
            private PreInvoiceGenerateStats.DollarCount CurrentDollarCount = null;


            private Result<Boolean> GetInvoiceCycleCurrent()
            {
                Result<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>> ListInvCRC = _WBMDB.InvoiceCycleCurrent_ListForCompany(CompanyID).GetAwaiter().GetResult();

                if (ListInvCRC.IsSuccess)
                {
                    ListICCRow = ListInvCRC.Value;

                    CurrentInvoiceCycle = ListInvCRC.Value.FirstOrDefault(x => x.InvoiceCycleID == InvoiceCycleID);

                    if (CurrentInvoiceCycle == null)
                    {
                        return Result.Failure<Boolean>("Company ID of " + CompanyID.ToString() + " no invoice cycle row found for cycle " + InvoiceCycleID);
                    }
                    else
                    {
                        CurrentCloseDate = CurrentInvoiceCycle.CurrentPeriodEnding;

                        if (CurrentCloseDate > RequestedCloseDate)
                        {
                            return Result.Failure<Boolean>("Requested close date of " + RequestedCloseDate.ToString("dd/MM/yyyy") + " is before the current invoice cycle close date of " + CurrentCloseDate.ToString("dd/MM/yyyy"));
                        }

                        // Check adhoc close date
                        if (InvoiceCycleID == 1)
                        {
                            if (NextPeriodEnding == null)
                            {
                                return Result.Failure<Boolean>("No next period ending stated for Adhoc cycle");
                            }
                            if (NextPeriodEnding.Value != NextPeriodEnding.Value.Date)
                            {
                                return Result.Failure<Boolean>("NextAdhocPeriodEnding must be date only no time");
                            }
                            if (CurrentCloseDate <= NextPeriodEnding)
                            {
                                return Result.Failure<Boolean>("Next Adhoc close date of " + NextPeriodEnding.Value.ToString("dd/MM/yyyy") + " is same or before the current invoice cycle close date of " + CurrentCloseDate.ToString("dd/MM/yyyy"));
                            }
                        }

                    }

                    // Populate list of current ID's for use in checking customers

                    ListInvoiceCycleCurrentID = new List<int>();
                    foreach (InvoiceCycleCurrent.dbRow iccrow in ListICCRow)
                    {
                        ListInvoiceCycleCurrentID.Add(iccrow.InvoiceCycleID);
                    }
                }
                else
                {
                    if (ListInvCRC.IsException)
                        return Result.Exception<Boolean>(ListInvCRC.ErrorMessage);
                    else
                        return Result.Failure<Boolean>(ListInvCRC.ErrorMessage);
                }

                return Result.Success<Boolean>(true);
            }

            private Result<Boolean> GetOwners()
            {
                Result<List<wbm_common.DataObjects.Owner.dbRow>> ListOwnerCRC = _WBMDB.Owner_List().GetAwaiter().GetResult();

                if (ListOwnerCRC.IsSuccess)
                {
                    ListOwner = ListOwnerCRC.Value;
                }
                else
                {
                    if (ListOwnerCRC.IsException)
                        return Result.Exception<Boolean>(ListOwnerCRC.ErrorMessage);
                    else
                        return Result.Failure<Boolean>(ListOwnerCRC.ErrorMessage);
                }

                return Result.Success<Boolean>(true);
            }

            private Result<Boolean> ProcessTransactionHeaders()
            {
                Result<Boolean> thRC = GetTransactionHeaders();

                if (thRC.IsSuccess)
                {
                    // this is sorted already as the dbquery gets it and sorts it. It will create weird duplicate customers without it.
                    foreach (TransactionHeader.dbRow thRow in ListTHRow)
                    {
                        #region Check for change of owner
                        // CHeck for change of owner; list ordered by owner ID
                        if (thRow.OwnerID != CurrentOwnerID)
                        {
                            Result<Boolean> soRC = SetOwner(thRow.OwnerID);

                            if (!soRC.IsSuccess)
                            {
                                return soRC;
                            }
                        }
                        #endregion

                        if (ProcessOwner)
                        {
                            if (thRow.TransactionDateTime <= CurrentCloseDate)
                            {
                                CurrentDollarCount.Count++;
                                CurrentDollarCount.Amount += thRow.InvoiceExGST;
                            }
                        }
                    }

                    // Transactions processed.. do we have any non-zero results that are uninvoiced ?
                    foreach (KeyValuePair<int, PreInvoiceGenerateStats.DollarCount> kvp in DictUnInvoiced)
                    {
                        if (kvp.Value.Amount != 0)
                        {
                            // If we are already invoiced that's a warning; if not invoiced, that's a critical
                            Owner.dbRow ownerRow = ListOwner.FirstOrDefault(x => x.ID == kvp.Key);

                            if (ownerRow != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append(ownerRow.OwnerNumberName + ", ");

                                PreInvoiceCloseCheck.CheckResult cr = new PreInvoiceCloseCheck.CheckResult();
                                cr.Category = "Owner";

                                if (ownerRow.LastInvoiceDate == CurrentCloseDate)
                                {
                                    cr.Status = "Warning";
                                    sb.Append("already invoiced; ");
                                }
                                else
                                {
                                    cr.Status = "Critical";
                                    sb.Append("not yet invoiced; ");
                                }

                                sb.Append(kvp.Value.Count.ToString() + " Transactions, $" + kvp.Value.Amount.ToString());

                                cr.Description = sb.ToString();

                                rtnCheck.ListResult.Add(cr);
                            }
                        }
                    }

                    return Result.Success<Boolean>(true);
                }
                else
                    return thRC;
            }

            private Result<Boolean> GetTransactionHeaders()
            {
                Result<Boolean> rtnResult = null;

                Result<List<TransactionHeader.dbRow>> rc = _WBMDB.TransactionHeader_ListAllUnInvoiced().GetAwaiter().GetResult();

                if (rc.IsSuccess)
                {
                    ListTHRow = rc.Value;
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

            private Result<Boolean> ProcessInvoiceHeaders()
            {
                Result<Boolean> ihRC = GetInvoiceHeaders();

                if (ihRC.IsSuccess)
                {
                    foreach (InvoiceHeader.dbRow ihRow in ListIHRow)
                    {
                        // If we are already invoiced that's a warning; if not invoiced, that's a critical
                        Owner.dbRow ownerRow = ListOwner.FirstOrDefault(x => x.ID == ihRow.OwnerID);

                        if (ownerRow != null)
                        {
                            if (ownerRow.InvoiceCycleID == InvoiceCycleID)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append(ownerRow.OwnerNumberName + ", ");

                                PreInvoiceCloseCheck.CheckResult cr = new PreInvoiceCloseCheck.CheckResult();
                                cr.Category = "Invoice";
                                cr.Category = "Critical";

                                sb.Append("Invoice " + ihRow.InvoicePrefix + ihRow.InvoiceNumber + " not yet exported");
                                cr.Description = sb.ToString();

                                rtnCheck.ListResult.Add(cr);
                            }
                        }
                    }
                    return Result.Success<Boolean>(true);
                }
                else
                    return ihRC;
            }

            private Result<Boolean> GetInvoiceHeaders()
            {
                Result<Boolean> rtnResult = null;

                Result<List<InvoiceHeader.dbRow>> rc = _WBMDB.InvoiceHeader_ListAllUnExportedForCompany(CompanyID).GetAwaiter().GetResult();

                if (rc.IsSuccess)
                {
                    ListIHRow = rc.Value;
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


            private Result<Boolean> SetOwner(int newOwnerID)
            {
                if (DictUnInvoiced.TryGetValue(newOwnerID, out CurrentDollarCount))
                {
                    // all ok
                }
                else
                {
                    CurrentDollarCount = new PreInvoiceGenerateStats.DollarCount();
                    DictUnInvoiced.Add(newOwnerID, CurrentDollarCount);
                }

                CurrentOwnerID = newOwnerID;

                Owner.dbRow ownerRow = ListOwner.FirstOrDefault(x => x.ID == CurrentOwnerID);

                if (ownerRow == null)
                {
                    // No owner.. fault...
                    return Result.Failure<Boolean>("Owner ID of " + CurrentOwnerID.ToString() + " not found in Owner Table for Company " + CompanyID.ToString());
                }

                if (ownerRow.InvoiceCycleID == InvoiceCycleID)
                {
                    ProcessOwner = true;
                }
                else
                {
                    // Not our invoice cycle..
                    ProcessOwner = false;

                    // Check to see that invoice cycle is valid
                    if (!ListInvoiceCycleCurrentID.Contains(ownerRow.InvoiceCycleID))
                    {
                        // Not found, lets raise an error
                        if (ownerRow.InvoiceCycleID == InvoiceCycleID)
                        {
                            PreInvoiceCloseCheck.CheckResult cr = new PreInvoiceCloseCheck.CheckResult();
                            cr.Category = "Invoice Cycle";
                            cr.Category = "Critical";

                            cr.Description = ownerRow.OwnerNumberName + " - invalid invoice cycle of " + ownerRow.InvoiceCycleID;

                            rtnCheck.ListResult.Add(cr);
                        }

                    }
                }

                return Result.Success<Boolean>(true);
            }

        }

        public class InvoicePeriodClose
        {
            /* For customers on this cycle:
             * Not been invoiced, and have transactions - critical
             * Invoice not exported - critical
             * Check for customers with invalid invoice cycle - warning
             */

            private WBMDatabase _WBMDB;

            private PreInvoiceCloseCheck rtnCheck = new PreInvoiceCloseCheck();

            private int CompanyID = 0;
            private int InvoiceCycleID = 0;

            private DateTime CurrentCloseDate = DateTime.MinValue;
            private DateTime RequestedCloseDate = DateTime.MinValue;
            private DateTime NextPeriodCloseDate = DateTime.MinValue;

            private InvoiceCycleCurrent.dbRow CurrentInvoiceCycle { get; set; }

            public async Task<Result<PreInvoiceCloseCheck>> Generate(PreInvoiceCloseCheck sendObject, WBMDatabase WBMDB, int UserID)
            {
                this._WBMDB = WBMDB;
                this.CompanyID = sendObject.CompanyID;
                this.InvoiceCycleID = sendObject.InvoiceCycleID;
                this.NextPeriodCloseDate = sendObject.NextPeriodEnding;

                Boolean ExceptionOccurred = false;

                // We will do a preinvoice close check to ensure all ok and if no criticals we can proceed.

                InvoiceHelper.PreInvoicePeriodClose IHPIC = new InvoiceHelper.PreInvoicePeriodClose();
                Result<PreInvoiceCloseCheck> PICCr = IHPIC.Generate(sendObject, _WBMDB);

                if (PICCr.IsSuccess && !PICCr.Value.HasCritical)
                {
                    // We are good to close..

                    DateTime ClosedDate = DateTime.Now;

                    Boolean savedOK = false;

                    #region update rows
                    InvoiceCycleCurrent.dbRow ICCRow = IHPIC.CurrentInvoiceCycle;

                    InvoiceCycleHistory.dbRow ICHRow = new InvoiceCycleHistory.dbRow(ICCRow, "Closed - " + ClosedDate.ToString("dd/MM/yy hh:mm:ss"));

                    ICHRow.CreatedDateTime = ClosedDate;
                    ICHRow.CreatedMethod = sendObject.CalledFrom;
                    ICHRow.CreatedUserID = UserID;
                    ICHRow.LastAmendedDateTime = ClosedDate;
                    ICHRow.CreatedMethod = sendObject.CalledFrom;
                    ICHRow.LastAmendedUserID = UserID;

                    ICCRow.LastAmendedDateTime = ClosedDate;
                    ICCRow.LastAmendedMethod = sendObject.CalledFrom;
                    ICCRow.LastAmendedUserID = UserID;
                    #endregion

                    #region save
                    using var transaction = _WBMDB.dbContext.Database.BeginTransaction();

                    transaction.CreateSavepoint("Start");

                    Result<int> HistR = await _WBMDB.InvoiceCycleHistory_Insert(ICHRow);

                    if (HistR.IsSuccess)
                    {
                        Result<int> CycR = await _WBMDB.InvoiceCycleCurrent_Update(ICCRow);

                        if (HistR.IsSuccess)
                        {
                            savedOK = true;
                        }
                    }

                    if (savedOK)
                    {
                        transaction.Commit();
                        return Result.Success<PreInvoiceCloseCheck>(rtnCheck);
                    }
                    else
                    {
                        transaction.RollbackToSavepoint("Start");
                        return Result.Exception<PreInvoiceCloseCheck>(rtnCheck, "Internal Error");
                    }

                    #endregion
                }
                else
                {
                    return PICCr;
                }
            }
        }
    }
}



