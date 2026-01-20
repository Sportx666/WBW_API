using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WBM_API.Models;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;


namespace WBM_API.Helpers.Excel
{
    public class OwnerExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.InvoiceCycle.dbRow> invoicecycles = null;
        public List<wbm_common.DataObjects.RateCollectionDefn.dbRow> ratecollectiondefns = null;
        public List<wbm_common.DataObjects.Owner.dbRow> Owners = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Owner.Search OwnerSearchData;
        public OwnerExcel(wbm_common.DataObjects.Owner.Search OwnerSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.OwnerSearchData = OwnerSearchData;
            _WBMDB = wBMDB;
        }
        public async Task<bool> GenerateExcel(Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, IHubCallerClients inClients = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            CallBackStatusFunction = inCallBackStatusFunction;
            Clients = inClients;
            lrph = longRunProcessHeader;
            if (await GetData())
            {
                try
                {
                    using (ExcelPackage ep = new ExcelPackage(ms))
                    {
                        OwnerPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("OwnerExcelReport: " + excpt.Message);
                    return false;
                }
            }
            else
                HasErrors = true;

            return !HasErrors;
        }

        private async Task<bool> callCallBackFunctionFunction(string StatusDescription, int CurC, int maxc)
        {
            if (CallBackStatusFunction != null)
                await CallBackStatusFunction(Clients, _WBMDB, StatusDescription, CurC, maxc, lrph);
            return true;
        }

        private async Task<bool> GetData()
        {

            Result<List<wbm_common.DataObjects.Owner.dbRow>> rc = await OwnerHelper.ExecuteSearch(OwnerSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Owners = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListRateCollectionDefnID = Owners.Select(x => x.NewRateCollectionDefnID).Union(Owners.Select(x => x.CurrentRateCollectionDefnID)).Distinct().ToList();
            List<int> ListCompanyIDs = Owners.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListInvoiceCyclesIDs = Owners.Select(x => x.InvoiceCycleID).Distinct().ToList();
            List<int> ListUserID = Owners.Select(x => x.CreatedUserID).Union(Owners.Select(x => x.LastAmendedUserID)).Distinct().ToList();

            if (ListInvoiceCyclesIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.InvoiceCycle.dbRow>> invoicecyclesResults = _WBMDB.InvoiceCycle_ListByListIDs(ListInvoiceCyclesIDs).GetAwaiter().GetResult();
                if (invoicecyclesResults.IsFailure || invoicecyclesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                invoicecycles = invoicecyclesResults.Value;
            }

            if (ListRateCollectionDefnID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>> ratecollectiondefnsResults = _WBMDB.RateCollectionDefn_ListByListIDs(ListRateCollectionDefnID).GetAwaiter().GetResult();
                if (ratecollectiondefnsResults.IsFailure || ratecollectiondefnsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                ratecollectiondefns = ratecollectiondefnsResults.Value;
            }

            if (ListCompanyIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Company.dbRow>> companiesResults = _WBMDB.Company_ListByListIDs(ListCompanyIDs).GetAwaiter().GetResult();
                if (companiesResults.IsFailure || companiesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                companies = companiesResults.Value;
            }

            if (ListUserID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Security_User.dbRow>> userResults = _WBMDB.SecurityUser_ListByListID(ListUserID).GetAwaiter().GetResult();
                if (userResults.IsFailure || userResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                users = userResults.Value;
            }

            return true;
        }

        private void OwnerPage(ExcelPackage ep)
        {
            ExcelWorksheet OwnerWS = ep.Workbook.Worksheets.Add("Owner");

            #region header
            OwnerWS.View.FreezePanes(2, 1);
            OwnerWS.Cells[1, 1, 1, 24].Style.Font.Bold = true;
            OwnerWS.Cells[1, 1, 1, 24].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            OwnerWS.Column(1).Width = 12;
            OwnerWS.Column(2).Width = 25;
            OwnerWS.Column(3).Width = 15;
            OwnerWS.Column(4).Width = 15;
            OwnerWS.Column(5).Width = 15;
            OwnerWS.Column(6).Width = 15;
            OwnerWS.Column(7).Width = 15;
            OwnerWS.Column(8).Width = 15;
            OwnerWS.Column(9).Width = 15;
            OwnerWS.Column(10).Width = 10;
            OwnerWS.Column(11).Width = 15;
            OwnerWS.Column(12).Width = 30;
            OwnerWS.Column(13).Width = 15;
            OwnerWS.Column(13).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            OwnerWS.Column(14).Width = 30;
            OwnerWS.Column(15).Width = 20;
            OwnerWS.Column(16).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            OwnerWS.Column(16).Width = 25;
            OwnerWS.Column(17).Width = 25;
            OwnerWS.Column(18).Width = 25;
            OwnerWS.Column(19).Width = 25;
            OwnerWS.Column(20).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            OwnerWS.Column(20).Width = 25;
            OwnerWS.Column(21).Width = 25;
            OwnerWS.Column(22).Width = 25;
            OwnerWS.Column(23).Width = 25;
            OwnerWS.Column(24).Width = 25;

            OwnerWS.Cells[1, 1].Value = "Company";
            OwnerWS.Cells[1, 2].Value = "Company Name";
            OwnerWS.Cells[1, 3].Value = "Source Owner";
            OwnerWS.Cells[1, 4].Value = "Name";
            OwnerWS.Cells[1, 5].Value = "Address Line 1";
            OwnerWS.Cells[1, 6].Value = "Address Line 2";
            OwnerWS.Cells[1, 7].Value = "Address Line 3";
            OwnerWS.Cells[1, 8].Value = "Address Line 4";
            OwnerWS.Cells[1, 9].Value = "Suburb";
            OwnerWS.Cells[1, 10].Value = "State";
            OwnerWS.Cells[1, 11].Value = "Postcode";
            OwnerWS.Cells[1, 12].Value = "Current Rate Collection Defn";
            OwnerWS.Cells[1, 13].Value = "Date Of Change";
            OwnerWS.Cells[1, 14].Value = "New Rate Collection Defn";
            OwnerWS.Cells[1, 15].Value = "Invoice Cycle";
            OwnerWS.Cells[1, 16].Value = "Created DateTime";
            OwnerWS.Cells[1, 17].Value = "Created Method";
            OwnerWS.Cells[1, 18].Value = "Created User";
            OwnerWS.Cells[1, 19].Value = "Created User Name";
            OwnerWS.Cells[1, 20].Value = "Last Amended DateTime";
            OwnerWS.Cells[1, 21].Value = "Last Amended Method";
            OwnerWS.Cells[1, 22].Value = "Last Amended User";
            OwnerWS.Cells[1, 23].Value = "Last Amended Name";
            OwnerWS.Cells[1, 24].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Owners.Count(); i++)
            {
                if (companies.Any(x => x.ID == Owners[i].CompanyID))
                {
                    OwnerWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == Owners[i].CompanyID).ShortName;
                    OwnerWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == Owners[i].CompanyID).LongName;
                }
                else
                    OwnerWS.Cells[rc, 1].Value = "CompanyID: " + Owners[i].CompanyID.ToString();

                OwnerWS.Cells[rc, 3].Value = Owners[i].SourceOwnerID;
                OwnerWS.Cells[rc, 4].Value = Owners[i].Name;
                OwnerWS.Cells[rc, 5].Value = Owners[i].AddressLine1;
                OwnerWS.Cells[rc, 6].Value = Owners[i].AddressLine2;
                OwnerWS.Cells[rc, 7].Value = Owners[i].AddressLine3;
                OwnerWS.Cells[rc, 8].Value = Owners[i].AddressLine4;
                OwnerWS.Cells[rc, 9].Value = Owners[i].Suburb;
                OwnerWS.Cells[rc, 10].Value = Owners[i].State;
                OwnerWS.Cells[rc, 11].Value = Owners[i].PostCode;

                if (ratecollectiondefns.Any(x => x.ID == Owners[i].CurrentRateCollectionDefnID))
                    OwnerWS.Cells[rc, 12].Value = ratecollectiondefns.FirstOrDefault(x => x.ID == Owners[i].CurrentRateCollectionDefnID).PublicID;
                else
                    OwnerWS.Cells[rc, 12].Value = "CurrentRateCollectionDefnID: " + Owners[i].CurrentRateCollectionDefnID.ToString();

                OwnerWS.Cells[rc, 13].Value = Owners[i].DateOfChange;

                if (ratecollectiondefns.Any(x => x.ID == Owners[i].NewRateCollectionDefnID))
                    OwnerWS.Cells[rc, 14].Value = ratecollectiondefns.FirstOrDefault(x => x.ID == Owners[i].NewRateCollectionDefnID).PublicID;
                else
                    OwnerWS.Cells[rc, 14].Value = "NewRateCollectionDefnID: " + Owners[i].NewRateCollectionDefnID.ToString();

                if (invoicecycles.Any(x => x.ID == Owners[i].InvoiceCycleID))
                    OwnerWS.Cells[rc, 15].Value = invoicecycles.FirstOrDefault(x => x.ID == Owners[i].InvoiceCycleID).Description;
                else
                    OwnerWS.Cells[rc, 15].Value = "InvoiceCycleID: " + Owners[i].InvoiceCycleID.ToString();

                OwnerWS.Cells[rc, 16].Value = Owners[i].CreatedDateTime.ToOADate();
                OwnerWS.Cells[rc, 17].Value = Owners[i].CreatedMethod;
                if (users.Any(x => x.ID == Owners[i].CreatedUserID))
                {
                    OwnerWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == Owners[i].CreatedUserID).Firstname;
                    OwnerWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == Owners[i].CreatedUserID).PublicID;
                }
                else
                    OwnerWS.Cells[rc, 18].Value = "UserID: " + Owners[i].CreatedUserID.ToString();

                OwnerWS.Cells[rc, 20].Value = Owners[i].LastAmendedDateTime.ToOADate();
                OwnerWS.Cells[rc, 21].Value = Owners[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Owners[i].LastAmendedUserID))
                {
                    OwnerWS.Cells[rc, 22].Value = users.FirstOrDefault(x => x.ID == Owners[i].LastAmendedUserID).Firstname;
                    OwnerWS.Cells[rc, 23].Value = users.FirstOrDefault(x => x.ID == Owners[i].LastAmendedUserID).PublicID;
                }
                else
                    OwnerWS.Cells[rc, 22].Value = "UserID: " + Owners[i].LastAmendedUserID.ToString();

                OwnerWS.Cells[rc, 24].Value = Owners[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
