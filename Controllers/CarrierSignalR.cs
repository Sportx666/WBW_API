using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using System.IdentityModel.Tokens.Jwt;
using WBM_API.Helpers;
using WBM_API.Helpers.Excel;
using WBM_API.Models;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;
using static WBM_API.Helpers.ControllerHelper;
using static WBM_API.Helpers.LongRunProcessHelper;

sealed class CarrierSignalR : Hub//<IChatClientSignalRInterface>
{
    private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    private readonly WBMDatabase _WBMDB;
    private LongRunProcessHelper LRPH = new LongRunProcessHelper();
    public IConfiguration _configuration;
    public CarrierSignalR(IConfiguration configuration, WBMDatabase inWBMDB)
    {
        _configuration = configuration;
        _WBMDB = inWBMDB;
    }
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", "Connected");
    }
    // send in {"protocol":"json","version":1}
    /*
     * token version {"arguments":["{\"token\":\"<token>\",\"search\":{\"PublicID\":\"TestCarrier\",\"ListSiteID\":[1],\"Description\":\"desc\",\"Paperless_KeyID\":\"keyid\",\"SearchMode\":4,\"ID\":3,\"ListID\":[3,4],\"CreatedMethod\":\"\",\"CreatedUserID\":1,\"LastAmendedMethod\":\"\",\"LastAmendedUserID\":1}}"],"target":"CarrierExcelReportSignalR","type":1} 
     */
    public class StringAndTokenInput
    {
        public string token { get; set; }
        public wbm_common.DataObjects.Carrier.Search search { get; set; }
    }
    public async Task CarrierExcelReportSignalR(string RequestStr)
    {
        string APIEndPoint = "CarrierExcelReportSignalR";
        string error;
        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
        DateTime startOfCall = DateTime.Now;
        try
        {
            UnpackReturn unpackReturn = await CommonUnpack(Clients, _WBMDB, APIEndPoint, RequestStr, startOfCall, apiTransactionLog, new StringAndTokenInput());
            if (!unpackReturn.success)
                return;
            apiTransactionLog = unpackReturn.log;
            wbm_common.DataObjects.Carrier.Search _carrierSearchData = (wbm_common.DataObjects.Carrier.Search)unpackReturn.genericObject;
            wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = unpackReturn.LRPH;
            CarrierExcel CE = new CarrierExcel(_carrierSearchData, _WBMDB);

            if (await CE.GenerateExcel(callBackToClient, Clients, longRunProcessHeader))
            {
                apiTransactionLog.CallSuccess = true;
                apiTransactionLog.CallLength = (int)(DateTime.Now - startOfCall).TotalMilliseconds;
                apiTransactionLog.ReturnData = JsonConvert.SerializeObject(CE.ms.ToArray());
                longRunProcessHeader.ProcessStatusID = 2;//finished
                longRunProcessHeader = LRPH.FinishLongRunTask(longRunProcessHeader, apiTransactionLog.ReturnData);
                longRunProcessHeader = LRPH.CreateStatus(longRunProcessHeader, longRunProcessHeader.ProcessStatusID, longRunProcessHeader.MaxCounter, longRunProcessHeader.MaxCounter, "CarrierListExcelReport Done", _WBMDB);
                _WBMDB.Log_APITransactionToDB(ref apiTransactionLog);
                _WBMDB.APICallStats_Update(APIEndPoint, startOfCall);
                await Clients.Caller.SendAsync("Finished", JsonConvert.SerializeObject(CE.ms.ToArray()));
                return;
            }
            else
            {
                longRunProcessHeader.ProcessStatusID = 3;//finished and errored
                longRunProcessHeader = LRPH.FinishLongRunTask(longRunProcessHeader, apiTransactionLog.ReturnData);
                longRunProcessHeader = LRPH.CreateStatus(longRunProcessHeader, longRunProcessHeader.ProcessStatusID, longRunProcessHeader.MaxCounter, longRunProcessHeader.MaxCounter, "CarrierListExcelReport Errored", _WBMDB);
                ErrorCodeHelper.updateErrorLog(apiTransactionLog, startOfCall, JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LocalFileFolder) } }), _WBMDB);
                await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LocalFileFolder) } }));
                return;
            }
        }
        catch (Exception ex)
        {
            logger.Error("CarrierExcelReportSignalR: " + ex);
            ErrorCodeHelper.updateErrorLog(apiTransactionLog, startOfCall, JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }), _WBMDB);
            await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }));
            return;
        }
    }

    public async Task CarrierSearchSignalR(string RequestStr)
    {
        string APIEndPoint = "CarrierSearchSignalR";
        string error;
        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
        DateTime startOfCall = DateTime.Now;
        try
        {
            UnpackReturn unpackReturn = await CommonUnpack(Clients, _WBMDB, APIEndPoint, RequestStr, startOfCall, apiTransactionLog, new StringAndTokenInput());
            if (!unpackReturn.success)
                return;
            apiTransactionLog = unpackReturn.log;
            wbm_common.DataObjects.Carrier.Search _carrierSearchData = (wbm_common.DataObjects.Carrier.Search)unpackReturn.genericObject;
            wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = unpackReturn.LRPH;

            Result<List<wbm_common.DataObjects.Carrier.dbRow>> carrierSearchResult = await CarrierHelper.ExecuteSearch(_carrierSearchData, _WBMDB, Clients, callBackToClient, longRunProcessHeader);

            if (carrierSearchResult.IsException || carrierSearchResult.IsFailure)
            {
                longRunProcessHeader.ProcessStatusID = 3;//finished and errored
                longRunProcessHeader = LRPH.FinishLongRunTask(longRunProcessHeader, apiTransactionLog.ReturnData);
                longRunProcessHeader = LRPH.CreateStatus(longRunProcessHeader, longRunProcessHeader.ProcessStatusID, longRunProcessHeader.MaxCounter, longRunProcessHeader.MaxCounter, carrierSearchResult.ErrorMessage, _WBMDB);
                ErrorCodeHelper.updateErrorLog(apiTransactionLog, startOfCall, JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = carrierSearchResult.ErrorMessage } }), _WBMDB);
                await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = carrierSearchResult.ErrorMessage } }));
                return;
            }
            else
            {
                apiTransactionLog.CallSuccess = true;
                apiTransactionLog.CallLength = (int)(DateTime.Now - startOfCall).TotalMilliseconds;
                //apiTransactionLog.ReturnData = JsonConvert.SerializeObject(carrierSearchResult.Value);
                longRunProcessHeader.ProcessStatusID = 2;//finished
                longRunProcessHeader = LRPH.FinishLongRunTask(longRunProcessHeader, apiTransactionLog.ReturnData);
                longRunProcessHeader = LRPH.CreateStatus(longRunProcessHeader, longRunProcessHeader.ProcessStatusID, longRunProcessHeader.MaxCounter, longRunProcessHeader.MaxCounter, "CarrierListExcelReport Done", _WBMDB);
                _WBMDB.Log_APITransactionToDB(ref apiTransactionLog);
                _WBMDB.APICallStats_Update(APIEndPoint, startOfCall);
                await Clients.Caller.SendAsync("Finished", JsonConvert.SerializeObject(carrierSearchResult.Value));
                return;
            }
        }
        catch (Exception ex)
        {
            logger.Error("CarrierSearchSignalR: " + ex);
            ErrorCodeHelper.updateErrorLog(apiTransactionLog, startOfCall, JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }), _WBMDB);
            await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }));
            return;
        }
    }
}