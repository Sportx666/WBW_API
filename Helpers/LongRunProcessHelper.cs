using WBM_API.Models;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class LongRunProcessHelper
    {
        // Change the actual return to be Result<API.longRunProcessHeader>
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow CreateLongRunTaskHeader(int UserID, string ProcessName, int ProcessNameID, int CurrentCounter, WBMDatabase WBMDB)
        {
            wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = new wbm_common.DataObjects.LongRunProcessHeader.dbRow();
            longRunProcessHeader.UserID = UserID;
            longRunProcessHeader.ProcessName = ProcessName;
            longRunProcessHeader.ProcessNameID = ProcessNameID;
            longRunProcessHeader.StartedDateTime = DateTime.Now;
            longRunProcessHeader.FinishedDateTime = null;
            longRunProcessHeader.ProcessStatusID = 0;
            longRunProcessHeader.MaxCounter = 0;
            longRunProcessHeader.CurrentCounter = CurrentCounter;
            longRunProcessHeader.StatusDateTime = longRunProcessHeader.StartedDateTime;
            longRunProcessHeader.FilenamePath = "";
            longRunProcessHeader.DataToReturn = "";
            longRunProcessHeader.CanBeDeleted = false;

            Result<int> res = WBMDB.LongRunningHeaderRow_Insert(longRunProcessHeader).GetAwaiter().GetResult();
            if (res.IsSuccess)
            {
                longRunProcessHeader.ID = res.Value;
                return longRunProcessHeader;
            }
            else return null;
        }

        public wbm_common.DataObjects.LongRunProcessHeader.dbRow CreateStatus(wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph, int ProcessStatusID,
            int MaxCounter, int CurrentCounter, string StatusDescription, WBMDatabase WBMDB)
        {
            // change this to be the API version of dbRow..

            wbm_common.DataObjects.LongRunProcessStatus.dbRow longRunProcessStatus = new wbm_common.DataObjects.LongRunProcessStatus.dbRow();
            longRunProcessStatus.LongRunProcessHeaderID = lrph.ID;
            longRunProcessStatus.StatusDateTime = lrph.FinishedDateTime == null ? DateTime.Now : lrph.FinishedDateTime.Value;
            longRunProcessStatus.StatusDescription = StatusDescription;

            lrph.StatusDateTime = longRunProcessStatus.StatusDateTime;
            lrph.MaxCounter = MaxCounter;
            lrph.CurrentCounter = CurrentCounter;
            lrph.ProcessStatusID = ProcessStatusID;
            // Insert record
            Result<int> res = WBMDB.LongRunningRows_InsertAndUpdate(longRunProcessStatus, lrph).GetAwaiter().GetResult();
            if (res.IsSuccess)
                return lrph;
            return null;
        }

        public wbm_common.DataObjects.LongRunProcessHeader.dbRow FinishLongRunTask(wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph, string returnData)
        {
            lrph.DataToReturn = returnData;
            lrph.FinishedDateTime = DateTime.Now;
            return lrph;
        }

        public class LongRunProcessStatusUpdate
        {
            public string StatusDescription { get; set; }
            public int CurrentCounter { get; set; }
            public int MaxCounter { get; set; }
            public DateTime dateTime { get; set; }
        }
    }
}
