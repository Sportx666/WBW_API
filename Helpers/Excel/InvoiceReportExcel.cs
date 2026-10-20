using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Runtime.InteropServices.Marshalling;
using System.Xml.Linq;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;


namespace WBM_API.Helpers.Excel
{
    public class InvoiceReportExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public Owner.dbRow OwnerRow { get; set; }

        public InvoiceHeader.dbRow InvoiceHeaderRow = null;

        public List<TransactionHeader.dbRow> ListTransactionHeader = null;

        public List<TransactionDetail.dbRow> ListTransactionDetail = null;

        public List<InvoiceReportGroup.dbRow> ListInvoiceGroup = null;

        public List<InvoiceReportGroupMember.dbRow> ListInvoiceReportGroupMember = null;
       
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        #region Worksheets
        private ExcelWorksheet OverviewWS { get; set; }

        private ExcelWorksheet SummaryWS { get; set; }

        private ExcelWorksheet Sum_OrderWS { get; set; }

        private ExcelWorksheet Sum_ReceiptWS { get; set; }

        private ExcelWorksheet Sum_StorageWS { get; set; }

        private ExcelWorksheet Sum_OtherWS { get; set; }

        private ExcelWorksheet Trans_OrderWS { get; set; }

        private ExcelWorksheet Trans_ReceiptWS { get; set; }

        private ExcelWorksheet Trans_StorageWS { get; set; }

        private ExcelWorksheet Trans_OtherWS { get; set; }

        private ExcelWorksheet Trans_OrderTabWS { get; set; }

        private ExcelWorksheet Trans_ReceiptTabWS { get; set; }

        private ExcelWorksheet Trans_StorageTabWS { get; set; }

        private ExcelWorksheet Trans_OtherTabWS { get; set; }
        #endregion

        public ExcelPackage ep { get; set; }

        public InvoiceReportExcel()
        {
            ms = new MemoryStream();
        }
        public bool GenerateExcel()
        {
            try
            {
                using (ep = new ExcelPackage(ms))
                {
                    SetHeaders();
                    SetTransColumns(); // Put this after so that will appear after summary and overview
                    ProcessTransactions();
                    ep.Save();
                }
            }
            catch (Exception excpt)
            {
                logger.Error("InvoiceHeaderExcelReport: " + excpt.Message);
                ErrorMessage = excpt.Message;
                return false;
            }
            return true;
        }

        Dictionary<string, int> DictTransStartColumn = new Dictionary<string, int>();
        private void SetTransColumns()
        {
            Trans_OrderTabWS = ep.Workbook.Worksheets.Add("Tab_Order");
            Trans_ReceiptTabWS = ep.Workbook.Worksheets.Add("Tab_Receipt");
            Trans_StorageTabWS = ep.Workbook.Worksheets.Add("Tab_Storage");
            Trans_OtherTabWS = ep.Workbook.Worksheets.Add("Tab_Other");

            SetTransColumnsForSourceID(1, Trans_OrderTabWS);
            SetTransColumnsForSourceID(2, Trans_ReceiptTabWS);
            SetTransColumnsForSourceID(3, Trans_StorageTabWS);
            SetTransColumnsForSourceID(4, Trans_OtherTabWS);
        }

        int TabStartColumn = 7;
        int NumColumnsPerGroup = 2;
        int OtherStartCol = 5;

        private void SetTransColumnsForSourceID(int SourceID, ExcelWorksheet this_ws)
        {
            int StartColumn = TabStartColumn;

            string SourceIDAsString = SourceID.ToString() + "-";

            List<InvoiceReportGroupMember.dbRow> ListGroupMemberForGroup = null;
            List<InvoiceReportGroup.dbRow> ListForSource = ListInvoiceGroup.Where(r => r.TransactionSourceID == SourceID).OrderBy(r => r.SortOrder).ToList();
            foreach (InvoiceReportGroup.dbRow IRGrow in ListForSource)
            {
                ListGroupMemberForGroup = ListInvoiceReportGroupMember.Where(r => r.InvoiceReportGroupID == IRGrow.ID).ToList();
                foreach (InvoiceReportGroupMember.dbRow IRGMRow in ListGroupMemberForGroup)
                {
                    DictTransStartColumn.Add(SourceIDAsString + IRGMRow.RateCollectionChargeCode, StartColumn);
                }

                StartColumn += NumColumnsPerGroup;
            }

            SetHeaders_TabTrans(SourceID, this_ws, ListForSource);

        }

        private void SetHeaders_TabTrans(int TransctionSourceID, ExcelWorksheet this_WS, List<InvoiceReportGroup.dbRow> ListForSource)
        {
            // Fixed section

            this_WS.Column(1).Width = 12;
            this_WS.Column(2).Width = 12;
            this_WS.Column(3).Width = 20;
            this_WS.Column(4).Width = 15;
            this_WS.Column(5).Width = 12;
            this_WS.Column(6).Width = 15;


            this_WS.View.FreezePanes(2, 1);

            this_WS.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            this_WS.Column(2).Style.Numberformat.Format = "dd/mm/yy";
            this_WS.Column(4).Style.Numberformat.Format = "$#,##0.00";
            this_WS.Column(6).Style.Numberformat.Format = "$#,##0.00";

            this_WS.Cells[1, 1].Value = "Type";
            this_WS.Cells[1, 2].Value = "Date";
            this_WS.Cells[1, 3].Value = "Reference";
            this_WS.Cells[1, 4].Value = "Total Ex GST";
            this_WS.Cells[1, 5].Value = "Other Qty";
            this_WS.Cells[1, 6].Value = "Other Amount";

            int StartCol = TabStartColumn;
            // Variable
            foreach (InvoiceReportGroup.dbRow IRGRow in ListForSource)
            {
                this_WS.Cells[1, StartCol].Value = IRGRow.GroupName + " Qty";
                this_WS.Cells[1, StartCol+1].Value = IRGRow.GroupName + " Amount";
                this_WS.Column(StartCol+1).Style.Numberformat.Format = "$#,##0.00";

                this_WS.Column(StartCol).Width = 12;
                this_WS.Column(StartCol+1).Width = 15;

                StartCol += NumColumnsPerGroup;
            }

            this_WS.Cells[1, 1, 1, StartCol-1].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            this_WS.Cells[1, 1, 1, StartCol-1].Style.Font.Bold = true;

        }

        private void SetHeaders()
        {
            SetHeader_Overview();

            SummaryWS = ep.Workbook.Worksheets.Add("Summary_All");
            Sum_OrderWS = ep.Workbook.Worksheets.Add("Summary_Order");
            Sum_ReceiptWS = ep.Workbook.Worksheets.Add("Summary_Receipt");
            Sum_StorageWS = ep.Workbook.Worksheets.Add("Summary_Storage");
            Sum_OtherWS = ep.Workbook.Worksheets.Add("Summary_Other");

            Trans_OrderWS = ep.Workbook.Worksheets.Add("Transaction_Order");
            Trans_ReceiptWS = ep.Workbook.Worksheets.Add("Transaction_Receipt");
            Trans_StorageWS = ep.Workbook.Worksheets.Add("Transaction_Storage");
            Trans_OtherWS = ep.Workbook.Worksheets.Add("Transaction_Other");


            SetHeader_Summary(SummaryWS);
            SetHeader_Summary(Sum_OrderWS);
            SetHeader_Summary(Sum_ReceiptWS);
            SetHeader_Summary(Sum_StorageWS);
            SetHeader_Summary(Sum_OtherWS);


            SetHeader_Trans(Trans_OrderWS);
            SetHeader_Trans(Trans_ReceiptWS);
            SetHeader_Trans(Trans_StorageWS);
            SetHeader_Trans(Trans_OtherWS);
        }
        private void SetHeader_Overview()
        {
            OverviewWS = ep.Workbook.Worksheets.Add("Overview");

            OverviewWS.Cells[1, 1, 16, 1].Style.Font.Bold = true;
            OverviewWS.Cells[2, 2, 2, 3].Style.Font.Bold = true;

            OverviewWS.Cells[11, 1, 11, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

            OverviewWS.Column(1).Width = 25;
            OverviewWS.Column(2).Width = 15;

            OverviewWS.Cells[4,2].Style.Numberformat.Format = "dd/mm/yy";

            OverviewWS.Cells[7, 2, 16, 2].Style.Numberformat.Format = "$#,##0.00";


            OverviewWS.Cells[2, 1].Value = "Owner";
            
            OverviewWS.Cells[4, 1].Value = "Invoice Date";
            OverviewWS.Cells[5, 1].Value = "Invoice Number";
            
            OverviewWS.Cells[7, 1].Value = "Invoice Total Ex GST";
            OverviewWS.Cells[8, 1].Value = "GST";
            OverviewWS.Cells[9, 1].Value = "Grand Total";
            
            
            OverviewWS.Cells[13, 1].Value = "Total Order";
            OverviewWS.Cells[14, 1].Value = "Total Receipt";
            OverviewWS.Cells[15, 1].Value = "Total Storage";
            OverviewWS.Cells[16, 1].Value = "Total Other";

            OverviewWS.Cells[2, 2].Value = OwnerRow.SourceOwnerID;
            OverviewWS.Cells[2, 3].Value = OwnerRow.Name;

            OverviewWS.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            OverviewWS.Cells[4, 2].Value = InvoiceHeaderRow.InvoiceDate.ToOADate();
            OverviewWS.Cells[5, 2].Value = InvoiceHeaderRow.InvPrefixNumber;

            OverviewWS.Cells[4, 2, 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            OverviewWS.Cells[7, 2].Value = InvoiceHeaderRow.ClientExGSTAmount;
            OverviewWS.Cells[8, 2].Value = InvoiceHeaderRow.ClientGSTAmount;
            OverviewWS.Cells[9, 2].Value = InvoiceHeaderRow.ClientTotalAmount;

            OverviewWS.Cells[13, 2].Formula = "=SUM(Summary_Order!E:E)";
            OverviewWS.Cells[14, 2].Formula = "=SUM(Summary_Receipt!E:E)";
            OverviewWS.Cells[15, 2].Formula = "=SUM(Summary_Storage!E:E)";
            OverviewWS.Cells[16, 2].Formula = "=SUM(Summary_Other!E:E)";



        }

        private void SetHeader_Summary(ExcelWorksheet this_WS)
        {
            this_WS.Cells[1, 1, 1, 5].Style.Font.Bold = true;

            this_WS.Column(1).Width = 12;
            this_WS.Column(2).Width = 12;
            this_WS.Column(3).Width = 15;
            this_WS.Column(4).Width = 40;
            this_WS.Column(5).Width = 15;

            this_WS.Cells[1, 1, 1, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            this_WS.View.FreezePanes(2, 1);

            this_WS.Column(2).Style.Numberformat.Format = "dd/mm/yy";
            this_WS.Column(5).Style.Numberformat.Format = "$#,##0.00";

            this_WS.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            this_WS.Cells[1, 1].Value = "Type";
            this_WS.Cells[1, 2].Value = "Date";
            this_WS.Cells[1, 3].Value = "Reference";
            this_WS.Cells[1, 4].Value = "Description";
            this_WS.Cells[1, 5].Value = "Total Ex GST";
        }

        private void SetHeader_Trans(ExcelWorksheet this_WS)
        {
            this_WS.Cells[1, 1, 1, 11].Style.Font.Bold = true;

            this_WS.Column(1).Width = 12;
            this_WS.Column(2).Width = 12;
            this_WS.Column(3).Width = 20;
            this_WS.Column(4).Width = 15;
            this_WS.Column(5).Width = 25;
            this_WS.Column(6).Width = 40;
            this_WS.Column(7).Width = 12;
            this_WS.Column(8).Width = 12;
            this_WS.Column(9).Width = 15;
            this_WS.Column(10).Width = 15;
            this_WS.Column(11).Width = 15;


            this_WS.Cells[1, 1, 1, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            this_WS.View.FreezePanes(2, 1);

            this_WS.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            this_WS.Column(2).Style.Numberformat.Format = "dd/mm/yy";
            this_WS.Column(8).Style.Numberformat.Format = "$#,##0.00";
            this_WS.Column(9).Style.Numberformat.Format = "$#,##0.00";
            this_WS.Column(10).Style.Numberformat.Format = "$#,##0.00";
            this_WS.Column(11).Style.Numberformat.Format = "$#,##0.00";

            this_WS.Cells[1, 1].Value = "Type";
            this_WS.Cells[1, 2].Value = "Date";
            this_WS.Cells[1, 3].Value = "Reference";
            this_WS.Cells[1, 4].Value = "Category";
            this_WS.Cells[1, 5].Value = "Charge Description";
            this_WS.Cells[1, 6].Value = "Description";
            this_WS.Cells[1, 7].Value = "Quantity";
            this_WS.Cells[1, 8].Value = "Rate";
            this_WS.Cells[1, 9].Value = "Extended";
            this_WS.Cells[1, 10].Value = "Fuel";
            this_WS.Cells[1, 11].Value = "Total Ex GST";
        }

        TransactionHeader.dbRow Current_thRow = null;
        List<TransactionDetail.dbRow> Current_ListTD = null;

        private void ProcessTransactions()
        {
            foreach (TransactionHeader.dbRow thRow in ListTransactionHeader)
            {
                Current_thRow = thRow;
                
                Current_ListTD = ListTransactionDetail.Where(r => r.TransactionHeaderID == thRow.ID).ToList();

                ProcessTransaction();
            }
        }


        int RowCnt_Summary = 2;
        int RowCnt_Sum_Order = 2;
        int RowCnt_Sum_Receipt = 2;
        int RowCnt_Sum_Storage = 2;
        int RowCnt_Sum_Other = 2;

        int RowCnt_Trans_Order = 2;
        int RowCnt_Trans_Receipt = 2;
        int RowCnt_Trans_Storage = 2;
        int RowCnt_Trans_Other = 2;


        private void ProcessTransaction()
        {
            switch (Current_thRow.TransactionTypeID)
            {
                case 1:
                    AddTransToSummary(SummaryWS, RowCnt_Summary, RowCnt_Trans_Order, "Order", "Transaction_Order");
                    RowCnt_Summary++;

                    AddTransToSummary(Sum_OrderWS, RowCnt_Sum_Order, RowCnt_Trans_Order, "Order", "Transaction_Order");
                    AddTransToTabTrans(Trans_OrderTabWS, RowCnt_Sum_Order, "Order", "1-");
                    RowCnt_Sum_Order++;

                    AddTransToTrans(Trans_OrderWS, ref RowCnt_Trans_Order, "Order");

                    break;

                case 2:
                    AddTransToSummary(SummaryWS, RowCnt_Summary, RowCnt_Trans_Receipt, "Receipt", "Transaction_Receipt");
                    RowCnt_Summary++;

                    AddTransToSummary(Sum_ReceiptWS, RowCnt_Sum_Receipt, RowCnt_Trans_Receipt, "Receipt", "Transaction_Receipt");
                    AddTransToTabTrans(Trans_ReceiptTabWS, RowCnt_Sum_Receipt, "Receipt", "2-");
                    RowCnt_Sum_Receipt++;

                    AddTransToTrans(Trans_ReceiptWS, ref RowCnt_Trans_Receipt, "Receipt");
                    break;
                case 3:
                    AddTransToSummary(SummaryWS, RowCnt_Summary, RowCnt_Trans_Storage, "Storage", "Transaction_Storage");
                    RowCnt_Summary++;

                    AddTransToSummary(Sum_StorageWS, RowCnt_Sum_Storage, RowCnt_Trans_Storage, "Storage", "Transaction_Storage");
                    AddTransToTabTrans(Trans_StorageTabWS, RowCnt_Sum_Storage, "Storage", "3-");
                    RowCnt_Sum_Storage++;

                    AddTransToTrans(Trans_StorageWS, ref RowCnt_Trans_Storage, "Storage");
                    break;

                case 4:
                default:
                    AddTransToSummary(SummaryWS, RowCnt_Summary, RowCnt_Trans_Other, "Other", "Transaction_Other");
                    RowCnt_Summary++;

                    AddTransToSummary(Sum_OtherWS, RowCnt_Sum_Other, RowCnt_Trans_Other, "Other", "Transaction_Other");
                    AddTransToTabTrans(Trans_OtherTabWS, RowCnt_Sum_Other, "Other", "4-");
                    RowCnt_Sum_Other++;

                    AddTransToTrans(Trans_OtherWS, ref RowCnt_Trans_Other, "Other");
                    break;
            }
        }
        private void AddTransToSummary(ExcelWorksheet this_ws, int Rowcnt, int LinkRowcnt, string TransactionType, string LinkWSName)
        {
            this_ws.Cells[Rowcnt, 1].Value = TransactionType;
            this_ws.Cells[Rowcnt, 2].Value = Current_thRow.TransactionDateTime.ToOADate();
            //this_ws.Cells[Rowcnt, 3].Value = Current_thRow.TransactionReference;
            this_ws.Cells[Rowcnt, 4].Value = Current_thRow.Description;
            this_ws.Cells[Rowcnt, 5].Value = Current_thRow.InvoiceExGST;

            string hyperlink = LinkWSName + "!C" + LinkRowcnt.ToString();

            this_ws.Cells[Rowcnt, 3].Hyperlink = new ExcelHyperLink(hyperlink, Current_thRow.TransactionReference);


        }

        private void AddTransToTrans(ExcelWorksheet this_ws, ref int Rowcnt, string TransactionType)
        {
            foreach (TransactionDetail.dbRow tdRow in Current_ListTD)
            {
                this_ws.Cells[Rowcnt, 1].Value = TransactionType;
                this_ws.Cells[Rowcnt, 2].Value = Current_thRow.TransactionDateTime.ToOADate();
                this_ws.Cells[Rowcnt, 3].Value = Current_thRow.TransactionReference;

                //this_ws.Cells[Rowcnt, 4].Value = Current_thRow.Description; // Category
                this_ws.Cells[Rowcnt, 5].Value = tdRow.ChargeDescription;
                this_ws.Cells[Rowcnt, 6].Value = tdRow.FullDescription;
                this_ws.Cells[Rowcnt, 7].Value = tdRow.Quantity;
                this_ws.Cells[Rowcnt, 8].Value = tdRow.ActualCustomerRate;
                this_ws.Cells[Rowcnt, 9].Value = tdRow.ActualCustomerExtended;
                this_ws.Cells[Rowcnt, 10].Value = tdRow.ActualCustomerFuel;
                this_ws.Cells[Rowcnt, 11].Value = tdRow.ActualCustomerExGST;

                Rowcnt++;
            }
        }

        private void AddTransToTabTrans(ExcelWorksheet this_ws, int Rowcnt, string TransactionType, string LookupPrefix)
        {
            string ChargeCodeLookup = null;
            int StartCol = 0;
            int NewQty = 0;
            Decimal NewAmount = 0m;

            foreach (TransactionDetail.dbRow tdRow in Current_ListTD)
            {
                this_ws.Cells[Rowcnt, 1].Value = TransactionType;
                this_ws.Cells[Rowcnt, 2].Value = Current_thRow.TransactionDateTime.ToOADate();
                this_ws.Cells[Rowcnt, 3].Value = Current_thRow.TransactionReference;
                this_ws.Cells[Rowcnt, 4].Value = Current_thRow.InvoiceExGST;

                ChargeCodeLookup = LookupPrefix + tdRow.RateCode;
                if (!DictTransStartColumn.TryGetValue(ChargeCodeLookup, out StartCol))
                    StartCol = OtherStartCol;

                if (this_ws.Cells[Rowcnt, StartCol].Value != null)
                {
                    NewQty = (int)this_ws.Cells[Rowcnt, StartCol].Value + tdRow.Quantity;
                }
                else
                    NewQty = tdRow.Quantity;

                if (this_ws.Cells[Rowcnt, StartCol + 1].Value != null)
                {
                    NewAmount = (Decimal)this_ws.Cells[Rowcnt, StartCol + 1].Value + tdRow.ActualCustomerExGST;
                }
                else
                    NewAmount = tdRow.ActualCustomerExGST;

                this_ws.Cells[Rowcnt, StartCol].Value = NewQty;
                this_ws.Cells[Rowcnt, StartCol+1].Value = NewAmount;
            }
        }

    }
}
