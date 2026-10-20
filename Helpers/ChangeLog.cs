using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using wbm_common.ComplexDataObjects;

namespace WBM_API.Helpers
{
    public class ChangeLogHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<ChangeHeaderAndDetailLog>> GetChangeLog(wbm_common.DataObjects.ChangeTracking.ChangeHeaderTableID TableID, int RecordID, WBMDatabase _WBMDB)
        {
            ChangeHeaderAndDetailLog CHDL = new ChangeHeaderAndDetailLog();
            CHDL.ListChangeEvent = new List<ChangeHeaderAndDetailLog.ChangeEvent>();

            Result<List<wbm_common.DataObjects.ChangeHeader.dbRowExtended>> CHr = await _WBMDB.ChangeHeader_ListForTableIDRecordID((int)TableID, RecordID);

            if (CHr.IsSuccess)
            {
                List<int> ListCHID = new List<int>();
                foreach (ChangeHeader.dbRowExtended ch in CHr.Value)
                {
                    ListCHID.Add(ch.ID);
                }

                if (ListCHID.Count > 0)
                {
                    Result<List<wbm_common.DataObjects.ChangeDetail.dbRow>> CDr = await _WBMDB.ChangeDetail_ListByListHeaderID(ListCHID);
                    if (CDr.IsSuccess)
                    {
                        // we've got a list of Change Header and a list of CHangeDetail; need to create a list of ChangeEvents and return that
                        wbm_common.ComplexDataObjects.ChangeHeaderAndDetailLog.ChangeEvent changeEvent = null;
                        foreach (ChangeHeader.dbRowExtended chRow in CHr.Value)
                        {
                            changeEvent = new ChangeHeaderAndDetailLog.ChangeEvent();
                            changeEvent.ChangeHeader = chRow;
                            changeEvent.ListChangeDetail = CDr.Value.FindAll(x => x.ChangeHeaderID == chRow.ID);

                            CHDL.ListChangeEvent.Add(changeEvent);
                        }
                    }
                    else
                    {
                        if (CDr.IsFailure)
                            return Result.Failure<ChangeHeaderAndDetailLog>(CDr.ErrorMessage);
                        else
                            return Result.Exception<ChangeHeaderAndDetailLog>("Internal Exception occured");
                    }
                }
                else
                                    {
                    // no change header records found, return empty list
                    return Result.Success<ChangeHeaderAndDetailLog>(CHDL);
                }
            }
            else
            {
                if (CHr.IsFailure)
                    return Result.Failure<ChangeHeaderAndDetailLog>(CHr.ErrorMessage);
                else
                    return Result.Exception<ChangeHeaderAndDetailLog>("Internal Exception occured");
            }

            return Result.Success<ChangeHeaderAndDetailLog>(CHDL);
        }

    }
}
