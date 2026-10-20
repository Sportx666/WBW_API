using NLog;
using NLog.Web;
using System.Diagnostics;
using WBM_API.Helpers.Excel;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;


namespace WBM_API.Helpers
{
    public class InvoiceExportHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public InvoiceExportHolder IEH { get; set; }

        public int UserID { get; set; }

        private InvoiceHeader.dbRow CurrentInvoiceHeaderRow { get; set; }

        private List<TransactionHeader.dbRow> CurrentListTransactionHeader { get; set; }
        private List<TransactionDetail.dbRow> CurrentListTransactionDetail { get; set; }

        private int CurrentInvoiceReportGroupDefnID = 0;

        private List<InvoiceReportGroup.dbRow> CurrentListInvoiceReportGroup { get; set; }

        private List<InvoiceReportGroupMember.dbRow> CurrentListInvoiceReportGroupMember {  get; set; }

        private Owner.dbRow CurrentOwnerRow {  get; set; }

        private WBMDatabase _WBMDB;

        public Result<InvoiceGenerateReturn> BatchInvoiceExport(wbm_common.NonDatabaseObjects.InvoiceExportHolder IEH, int UserID, WBMDatabase WBMDB)
        {
            this.IEH = IEH;
            this.UserID = UserID;
            this._WBMDB = WBMDB;

            Boolean ExceptionOccurred = false;

            InvoiceGenerateReturn IGR = new InvoiceGenerateReturn();
            IGR.ListOwnerResult = new List<InvoiceGenerateReturn.OwnerResult>();

            if (IEH.ListInvoiceHeader != null && IEH.ListInvoiceHeader.Count > 0)
            {
                foreach (InvoiceHeader.dbRow ihRow in IEH.ListInvoiceHeader)
                {
                    CurrentInvoiceHeaderRow = ihRow;
                    Result<Boolean> rc = GetData();

                    if (rc.IsSuccess)
                    {
                        GenerateExcel();
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

        private Result<Boolean> GetData()
        {
            Result<Boolean> ownerR = GetOwner();

            if (ownerR.IsSuccess)
            {
                Result<Boolean> trans_headerR = GetTransactionHeaders();

                if (trans_headerR.IsSuccess)
                {
                    Result<Boolean> trans_detailR = GetTransactionDetails();

                    if (trans_detailR.IsSuccess)
                    {
                        Result<Boolean> invoicegroupR = GetInvoiceReportGroup();

                        if (invoicegroupR.IsSuccess)
                        {
                            Result<Boolean> invoicegroupmemberR = GetInvoiceReportGroupMember();

                            return invoicegroupmemberR;
                        }
                        else
                            return invoicegroupR;
                    }
                    else
                        return trans_detailR;
                }
                else
                    return trans_headerR;
            }
            else
                return ownerR;
        }

        private Result<Boolean> GetOwner()
        {
            Result<Boolean> rtnResult = null;

            Result<Owner.dbRow> rc = _WBMDB.Owner_ReadByID(CurrentInvoiceHeaderRow.OwnerID).GetAwaiter().GetResult();

            if (rc.IsSuccess)
            {
                CurrentOwnerRow = rc.Value;
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


        private Result<Boolean> GetTransactionHeaders()
        {
            Result<Boolean> rtnResult = null;

            Result<List<TransactionHeader.dbRow>> rc = _WBMDB.TransactionHeader_ListByInvoiceHeaderID(CurrentInvoiceHeaderRow.ID).GetAwaiter().GetResult();

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

        private Result<Boolean> GetTransactionDetails()
        {
            Result<Boolean> rtnResult = null;

            if (CurrentListTransactionHeader != null && CurrentListTransactionHeader.Count > 0)
            {
                List<int> ListTHID = new List<int>();

                foreach (TransactionHeader.dbRow thRow in CurrentListTransactionHeader)
                {
                    ListTHID.Add(thRow.ID);
                }

                Result<List<TransactionDetail.dbRow>> rc = _WBMDB.TransactionDetail_ListByListTransactionHeaderIDs(ListTHID).GetAwaiter().GetResult();

                if (rc.IsSuccess)
                {
                    CurrentListTransactionDetail = rc.Value;
                    rtnResult = Result.Success<Boolean>(true);
                }
                else
                {
                    if (rc.IsFailure)
                        rtnResult = Result.Failure<Boolean>(rc.ErrorMessage);
                    else
                        rtnResult = Result.Exception<Boolean>(rc.ErrorMessage);
                }
            }
            else
                rtnResult = Result.Failure<Boolean>("CurreentListTransactionheader null or empty");

            return rtnResult;
        }

        private Result<Boolean> GetInvoiceReportGroup()
        {
            Result<Boolean> rtnResult = null;

            if (CurrentInvoiceReportGroupDefnID != 1)
            {
                CurrentInvoiceReportGroupDefnID = 1;
                Result<List<InvoiceReportGroup.dbRow>> rc = _WBMDB.InvoiceReportGroup_ListByInvoiceReportGroupDefnID(1).GetAwaiter().GetResult();

                if (rc.IsSuccess)
                {
                    CurrentListInvoiceReportGroup = rc.Value;
                    rtnResult = Result.Success<Boolean>(true);
                }
                else
                {
                    if (rc.IsFailure)
                        rtnResult = Result.Failure<Boolean>(rc.ErrorMessage);
                    else
                        rtnResult = Result.Exception<Boolean>(rc.ErrorMessage);
                }
            }

            return rtnResult;
        }

        private Result<Boolean> GetInvoiceReportGroupMember()
        {
            Result<Boolean> rtnResult = null;

            Result<List<InvoiceReportGroupMember.dbRow>> rc = _WBMDB.InvoiceReportGroupMember_ListByInvoiceReportGroupDefnID(1).GetAwaiter().GetResult();

            if (rc.IsSuccess)
            {
                CurrentListInvoiceReportGroupMember = rc.Value;
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


        private Result<Boolean> GenerateExcel()
        {
            InvoiceReportExcel IRE = new InvoiceReportExcel();
            IRE.InvoiceHeaderRow = CurrentInvoiceHeaderRow;
            IRE.ListTransactionHeader = CurrentListTransactionHeader;
            IRE.ListTransactionDetail = CurrentListTransactionDetail;
            IRE.ListInvoiceGroup = CurrentListInvoiceReportGroup;
            IRE.ListInvoiceReportGroupMember = CurrentListInvoiceReportGroupMember;
            IRE.OwnerRow = CurrentOwnerRow;

            if (IRE.GenerateExcel())
            {
                string ExcelFile = ControllerHelper.GetExcelTempName();
                FileInfo exportFile = new FileInfo(ExcelFile);

                if (exportFile.Exists)
                {
                    try
                    {
                        exportFile.Delete();
                        exportFile = new FileInfo(ExcelFile);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                try
                {
                    using (FileStream file = new FileStream(ExcelFile, FileMode.Create, System.IO.FileAccess.Write))
                        IRE.ms.WriteTo(file);

                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo = new ProcessStartInfo(ExcelFile)
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                }
                catch (Exception e)
                {
                    int i = 1;
                }

                return Result.Success<Boolean>(true);
            }
            else
                return Result.Failure<Boolean>(IRE.ErrorMessage);
        }
    }
}



