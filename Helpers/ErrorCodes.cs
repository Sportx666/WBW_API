using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public enum ErrorCodes
    {
        LogDB = 10000, // Failed to enter Log into db
        CreateLog = 10001, // Failed to create Log
        FirstStage = 10002, // Failed at first stage
        GeneralDB = 10003, // Database error
        LocalFileFolder = 10004, // Local computer file/folder error
        ExpectedDataMissing = 10005, // Expected data missing
    }

    public static class ErrorCodeHelper
    {
        public static string ErrorCodeAsString(ErrorCodes code)
        {
            return ((int)code).ToString();
        }

        public static void updateErrorLog(wbm_common.DataObjects.Log.dbRow apiTransactionLog, DateTime startOfCall, string error, WBMDatabase _WBMDB)
        {
            apiTransactionLog.ReturnData = error;
            apiTransactionLog.CallLength = (int)(DateTime.Now - startOfCall).TotalMilliseconds;
            _WBMDB.Log_APITransactionToDB(ref apiTransactionLog);
        }
    }
}
