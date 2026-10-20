using System.ComponentModel.DataAnnotations;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class DbRowValidator
    {
        public static List<string> requiredFieldErrors<dynamic>(List<string> fields, dynamic obj, WBMDatabase WBMDB)
        {
            if (obj == null)
                return new List<string> { "The db row is null!" };
            List<string> output = new List<string>();
            foreach (string field in fields)
            {
                var value = obj.GetType().GetProperty(field).GetValue(obj);
                if (value == null)
                {
                    output.Add(field + " cannot be null");
                    continue;
                }
                var context = new ValidationContext(value, null, null);
                var results = new List<ValidationResult>();
                var attributes = obj.GetType().GetProperty(field).GetCustomAttributes(false).OfType<ValidationAttribute>().ToArray();
                if (!Validator.TryValidateValue(value, context, results, attributes))
                    foreach (var result in results)
                        output.Add(result.ErrorMessage);
                switch (field)
                {
                    case "SiteID":
                        Result<wbm_common.DataObjects.Site.dbRow> Siteexist = WBMDB.Site_SearchByID((int)value).GetAwaiter().GetResult();
                        if (Siteexist.IsException || Siteexist.IsFailure)
                            output.Add("Site not found in DB");
                        break;
                    case "CompanyID":
                        Result<wbm_common.DataObjects.Company.dbRowExtended> Companyexist = WBMDB.Company_SearchByID((int)value).GetAwaiter().GetResult();
                        if (Companyexist.IsException || Companyexist.IsFailure)
                            output.Add("Company not found in DB");
                        break;
                    case "OwnerID":
                        Result<wbm_common.DataObjects.Owner.dbRow> Ownerexist = WBMDB.Owner_SearchByID((int)value).GetAwaiter().GetResult();
                        if (Ownerexist.IsException || Ownerexist.IsFailure)
                            output.Add("Owner not found in DB");
                        break;
                    case "ActionTypeID":
                        Result<wbm_common.DataObjects.ActionType.dbRow> ActionTypeexist = WBMDB.ActionType_ReadByID((int)value).GetAwaiter().GetResult();
                        if (ActionTypeexist.IsException || ActionTypeexist.IsFailure)
                            output.Add("ActionType not found in DB");
                        break;
                    case "UnitTypeID":
                        Result<wbm_common.DataObjects.UnitType.dbRow> UnitTypeexist = WBMDB.UnitType_SearchByID((int)value).GetAwaiter().GetResult();
                        if (UnitTypeexist.IsException || UnitTypeexist.IsFailure)
                            output.Add("UnitType not found in DB");
                        break;
                    case "StockRateCategoryID":
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.StockRateCategory.dbRow> StockRateCategoryexist = WBMDB.StockRateCategory_ReadByID((int)value).GetAwaiter().GetResult();
                            if (StockRateCategoryexist.IsException || StockRateCategoryexist.IsFailure)
                                output.Add("StockRateCategory not found in DB");
                        }
                        break;
                    case "SiteSystemID":
                        Result<wbm_common.DataObjects.SiteSystem.dbRow> SiteSystemexist = WBMDB.SiteSystem_SearchByID((int)value).GetAwaiter().GetResult();
                        if (SiteSystemexist.IsException || SiteSystemexist.IsFailure)
                            output.Add("SiteSystem not found in DB");
                        break;
                    case "CarrierID":
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.Carrier.dbRow> Carrierexist = WBMDB.Carrier_ReadByID((int)value).GetAwaiter().GetResult();
                            if (Carrierexist.IsException || Carrierexist.IsFailure)
                                output.Add("Carrier not found in DB");
                        }
                        break;
                    case "ContainerSizeID":
                        Result<wbm_common.DataObjects.ContainerSize.dbRow> ContainerSizeexist = WBMDB.ContainerSize_ReadByID((int)value).GetAwaiter().GetResult();
                        if (ContainerSizeexist.IsException || ContainerSizeexist.IsFailure)
                            output.Add("ContainerSize not found in DB");
                        break;
                    //case "InvoiceHeaderID": //TDB
                    //    Result<wbm_common.DataObjects.InvoiceCycle.dbRow> InvoiceCycleexist = WBMDB.InvoiceCycle_SearchByID((int)value).GetAwaiter().GetResult();
                    //    if (InvoiceCycleexist.IsException || InvoiceCycleexist.IsFailure)
                    //        output.Add("InvoiceCycle not found in DB");
                    //    break;
                    case "InvoiceCycleID":
                        Result<wbm_common.DataObjects.InvoiceCycle.dbRow> InvoiceCycleexist = WBMDB.InvoiceCycle_SearchByID((int)value).GetAwaiter().GetResult();
                        if (InvoiceCycleexist.IsException || InvoiceCycleexist.IsFailure)
                            output.Add("InvoiceCycle not found in DB");
                        break;
                    case "PalletStorageTypeID":
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.PalletStorageType.dbRow> PalletStorageTypeexist = WBMDB.PalletStorageType_SearchByID((int)value).GetAwaiter().GetResult();
                            if (PalletStorageTypeexist.IsException || PalletStorageTypeexist.IsFailure)
                                output.Add("PalletStorageType not found in DB");
                        }
                        break;
                    case "FileTypeID":
                        Result<wbm_common.DataObjects.ManhattenFileType.dbRow> FileTypeexist = WBMDB.ManhattenFileType_SearchByID((int)value).GetAwaiter().GetResult();
                        if (FileTypeexist.IsException || FileTypeexist.IsFailure)
                            output.Add("ManhattenFileType not found in DB");
                        break;
                    case "ProcessCategoryID":
                        Result<wbm_common.DataObjects.ProcessCategory.dbRow> ProcessCategoryexist = WBMDB.ProcessCategory_SearchByID((int)value).GetAwaiter().GetResult();
                        if (ProcessCategoryexist.IsException || ProcessCategoryexist.IsFailure)
                            output.Add("ProcessCategory not found in DB");
                        break;
                    case "ProcessID":
                        Result<wbm_common.DataObjects.Process.dbRow> Processexist = WBMDB.Process_ReadByID((int)value).GetAwaiter().GetResult();
                        if (Processexist.IsException || Processexist.IsFailure)
                            output.Add("Process not found in DB");
                        break;
                    case "RateFunctionID":
                        Result<wbm_common.DataObjects.RateFunction.dbRow> RateFunctionexist = WBMDB.RateFunction_SearchByID((int)value).GetAwaiter().GetResult();
                        if (RateFunctionexist.IsException || RateFunctionexist.IsFailure)
                            output.Add("RateFunction not found in DB");
                        break;
                    case "RateCollectionDefnID":
                        Result<wbm_common.DataObjects.RateCollectionDefn.dbRow> RateCollectionDefnexist = WBMDB.RateCollectionDefn_SearchByID((int)value).GetAwaiter().GetResult();
                        if (RateCollectionDefnexist.IsException || RateCollectionDefnexist.IsFailure)
                            output.Add("RateCollectionDefn not found in DB");
                        break;
                    case "NewRateCollectionDefnID": // this one is exclusive to Owner, so it has one extra condition on the new rate. Anything new with it means this needs to be reexamined
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.RateCollectionDefn.dbRow> NewRateCollectionDefnexist = WBMDB.RateCollectionDefn_SearchByID((int)value).GetAwaiter().GetResult();
                            if (NewRateCollectionDefnexist.IsException || NewRateCollectionDefnexist.IsFailure)
                                output.Add("NewRateCollectionDefnID not found in DB");
                        }
                        var DateOfChange = obj.GetType().GetProperty("DateOfChange").GetValue(obj);
                        if (DateOfChange != null && (int)value <= 0)
                            output.Add("NewRateCollectionDefnID needs to have a positive value");
                        break;
                    case "RateCollectionRateChargeCode":
                        Result<wbm_common.DataObjects.RateCode.dbRow> RateChargeCodeexist = WBMDB.RateCode_SearchByRateCode((string)value).GetAwaiter().GetResult();
                        if (RateChargeCodeexist.IsException || RateChargeCodeexist.IsFailure)
                            output.Add(field + " not found in DB");
                        break;
                    case "NewRateCodeID": // this one is exclusive to Rate Collection, so it has one extra condition on the new rate. Anything new with it means this needs to be reexamined
                        // furthermore there is one extra condition, while a new rate code is needed for a non null date of change, if the charge is NOCHARGE it's ok
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.RateCode.dbRow> newRateCodeexist = WBMDB.RateCode_SearchByID((int)value).GetAwaiter().GetResult();
                            if (newRateCodeexist.IsException || newRateCodeexist.IsFailure)
                                output.Add("NewRateCodeID not found in DB");
                        }
                        var DateOfChangeRC = obj.GetType().GetProperty("DateOfChange").GetValue(obj);
                        if (DateOfChangeRC != null && (int)value <= 0)
                        {
                            var RateChargeCodeRC = obj.GetType().GetProperty("RateChargeCode").GetValue(obj);
                            if (RateChargeCodeRC == null || (string)RateChargeCodeRC != "NOCHARGE")
                                output.Add("NewRateCodeID needs to have a positive value");
                        }
                        break;
                    case "RateCodeID":
                        Result<wbm_common.DataObjects.RateCode.dbRow> RateCodeexist = WBMDB.RateCode_SearchByID((int)value).GetAwaiter().GetResult();
                        if (RateCodeexist.IsException || RateCodeexist.IsFailure)
                            output.Add(field + " not found in DB");
                        break;
                    case "CategoryID":
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.StockRateCategory.dbRow> Categoryexist = WBMDB.StockRateCategory_ReadByID((int)value).GetAwaiter().GetResult();
                            if (Categoryexist.IsException || Categoryexist.IsFailure)
                                output.Add("Category not found in DB");
                        }
                        break;
                    case "LocationID":
                        Result<wbm_common.DataObjects.Location.dbRow> Locationexist = WBMDB.Location_SearchByID((int)value).GetAwaiter().GetResult();
                        if (Locationexist.IsException || Locationexist.IsFailure)
                            output.Add("Site not found in DB");
                        break;
                    case "StorageRateCodeFromID":
                        Result<wbm_common.DataObjects.StorageRateCodeFrom.dbRow> StorageRateCodeFromexist = WBMDB.StorageRateCodeFrom_SearchByID((int)value).GetAwaiter().GetResult();
                        if (StorageRateCodeFromexist.IsException || StorageRateCodeFromexist.IsFailure)
                            output.Add("StorageRateCodeFrom not found in DB");
                        break;
                    case "ChangeHeaderTableID":
                        Result<List<wbm_common.DataObjects.ChangeHeader_TableName.dbRow>> ChangeHeaderTableexist = WBMDB.ChangeHeader_TableName_ListByListIDs(new List<int> { (int)value }).GetAwaiter().GetResult();
                        if (ChangeHeaderTableexist.IsException || ChangeHeaderTableexist.IsFailure || ChangeHeaderTableexist.Value == null || ChangeHeaderTableexist.Value.Count() == 0)
                            output.Add("ChangeHeaderTable not found in DB");
                        break;
                    case "StatusID":
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.ToDoStatus_Type.dbRow> Statusexist = WBMDB.ToDoStatus_Type_ReadByID((int)value).GetAwaiter().GetResult();
                            if (Statusexist.IsException || Statusexist.IsFailure)
                                output.Add("ToDoStatus_Type not found in DB");
                        }
                        break;
                    case "TransactionHeaderID":
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.TransactionHeader.dbRowExtended> TransactionHeaderexist = WBMDB.TransactionHeader_SearchByID((int)value).GetAwaiter().GetResult();
                            if (TransactionHeaderexist.IsException || TransactionHeaderexist.IsFailure)
                                output.Add("TransactionHeader not found in DB");
                        }
                        break;
                    case "TransactionDetailID":
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.TransactionDetail.dbRowExtended> TransactionDetailexist = WBMDB.TransactionDetail_SearchByID((int)value).GetAwaiter().GetResult();
                            if (TransactionDetailexist.IsException || TransactionDetailexist.IsFailure)
                                output.Add("TransactionDetail not found in DB");
                        }
                        break;
                    case "TransactionExceptionTypeID":
                        Result<wbm_common.DataObjects.TransactionExceptionType.dbRow> TransactionExceptionTypeexist = WBMDB.TransactionExceptionType_SearchByID((int)value).GetAwaiter().GetResult();
                        if (TransactionExceptionTypeexist.IsException || TransactionExceptionTypeexist.IsFailure)
                            output.Add("TransactionExceptionType not found in DB");
                        break;
                    case "TransactionNoteTypeID":
                        Result<wbm_common.DataObjects.NoteType.dbRow> NoteTypeexist = WBMDB.NoteType_SearchByID((int)value).GetAwaiter().GetResult();
                        if (NoteTypeexist.IsException || NoteTypeexist.IsFailure)
                            output.Add("NoteType not found in DB");
                        break;
                    case "TransactionSourceID":
                        Result<wbm_common.DataObjects.TransactionSource.dbRow> TransactionSourceexist = WBMDB.TransactionSource_SearchByID((int)value).GetAwaiter().GetResult();
                        if (TransactionSourceexist.IsException || TransactionSourceexist.IsFailure)
                            output.Add("TransactionSource not found in DB");
                        break;
                    case "ProcessingRuleDefnID":
                    case "CurrentProcessingRuleDefnID":
                        Result<wbm_common.DataObjects.ProcessingRuleDefn.dbRow> ProcessingRuleDefnexist = WBMDB.ProcessingRuleDefn_ReadByID((int)value).GetAwaiter().GetResult();
                        if (ProcessingRuleDefnexist.IsException || ProcessingRuleDefnexist.IsFailure)
                            output.Add(field + " not found in DB");
                        break;
                    case "NewProcessingRuleDefnID": // this one is exclusive to Owner, so it has one extra condition on the new rate. Anything new with it means this needs to be reexamined
                        if ((int)value > 0)
                        {
                            Result<wbm_common.DataObjects.ProcessingRuleDefn.dbRow> NewProcessingRuleDefnexist = WBMDB.ProcessingRuleDefn_ReadByID((int)value).GetAwaiter().GetResult();
                            if (NewProcessingRuleDefnexist.IsException || NewProcessingRuleDefnexist.IsFailure)
                                output.Add("NewProcessingRuleDefnID not found in DB");
                        }
                        var DateOfChangePR = obj.GetType().GetProperty("ProcessingRuleDateOfChange").GetValue(obj);
                        if (DateOfChangePR != null && (int)value <= 0)
                            output.Add("NewProcessingRuleDefnID needs to have a positive value");
                        break;
                }
            }
            return output.Distinct().ToList();
        }
    }
}
