using Newtonsoft.Json;
using NLog;
using NLog.Web;
using static wbm_common.DataObjects.ChangeTracking;

namespace WBM_API.Helpers
{
    public class ChangeTracking
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public static wbm_common.DataObjects.ChangeHeader.dbRow Create_ChangeHeader_dbRow(int ChangeHeaderTableID, int RecordID, DateTime ChangeTime, DateTime CreatedTime, string CreatedMethod, int userID)
        {
            wbm_common.DataObjects.ChangeHeader.dbRow chRow = new wbm_common.DataObjects.ChangeHeader.dbRow();

            chRow.RecordID = RecordID;
            chRow.ChangeTime = ChangeTime;
            chRow.ChangeHeaderTableID = ChangeHeaderTableID;
            chRow.CreatedDateTime = CreatedTime;
            chRow.CreatedMethod = CreatedMethod;
            chRow.CreatedUserID = userID;
            chRow.LastAmendedDateTime = CreatedTime;
            chRow.LastAmendedMethod = CreatedMethod;
            chRow.LastAmendedUserID = userID;
            chRow.DeletedFlag = false;

            return chRow;
        }

        public List<wbm_common.DataObjects.ChangeDetail.dbRow> ListChangeDetail { get; set; }

        public Boolean HasChanges = false;

        public Boolean CheckForChanges(object oldRow, Object newRow, List<ChangeTrackerData> listChangeTrackerData)
        {
            Boolean rtnVal = true;

            Boolean FieldHasChanges = false;

            // Test for deleted
            object deletedFlag = newRow.GetType().GetProperty("DeletedFlag")?.GetValue(newRow);
            if (deletedFlag != null)
            {
                if ((bool)deletedFlag != false)
                {
                    if (ListChangeDetail == null)
                    {
                        ListChangeDetail = new List<wbm_common.DataObjects.ChangeDetail.dbRow>();
                    }

                    ListChangeDetail.Add(new wbm_common.DataObjects.ChangeDetail.dbRow
                    {
                        FieldName = "Deleted",
                        OldValue = "False",
                        NewValue = "True"
                    });

                    HasChanges = true;
                    return true;
                }
            }

            foreach (var changeData in listChangeTrackerData)
            {
                FieldHasChanges = false;

                try
                {
                    object oldValue = oldRow.GetType().GetProperty(changeData.FieldName)?.GetValue(oldRow);
                    object newValue = newRow.GetType().GetProperty(changeData.FieldName)?.GetValue(newRow);

                    if (changeData.IsDecimal)
                    {
                        // decimals will cause false negatives as it will say 1.0 != 1.0000 so the flag check accounts for this
                        if (Math.Abs((decimal)oldValue - (decimal)newValue) > (decimal)0.005)
                            FieldHasChanges = true;
                    }
                    else
                    {
                        // because these are both objects the comparison will look at the reference so they will always be unequal
                        // unless we serialise them

                        if (JsonConvert.SerializeObject(oldValue) != JsonConvert.SerializeObject(newValue))
                            FieldHasChanges = true;
                    }

                    if (FieldHasChanges)
                    {
                        if (ListChangeDetail == null)
                        {
                            ListChangeDetail = new List<wbm_common.DataObjects.ChangeDetail.dbRow>();
                        }

                        string oldValueAsString = oldValue.ToString();
                        string newValueAsString = newValue.ToString();

                        if (changeData.ExtraProcessing != ProcessingType.None)
                        {
                            if (!ExtraProcessing(changeData.ExtraProcessing, oldValue, newValue, out oldValueAsString, out newValueAsString))
                                rtnVal = false;
                        }

                        if (rtnVal)
                        {
                            ListChangeDetail.Add(new wbm_common.DataObjects.ChangeDetail.dbRow
                            {
                                FieldName = changeData.PublicName,
                                OldValue = oldValueAsString,
                                NewValue = newValueAsString
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("ChangeTracking_CheckForChanges: " + ex);

                    rtnVal = false;
                }
            }

            if (ListChangeDetail != null && ListChangeDetail.Count > 0)
                HasChanges = true;

            return rtnVal;
        }

        private Boolean ExtraProcessing(ProcessingType pt, Object oldValue, Object newValue, out string oldValueAsString, out string newValueAsString)
        {
            Boolean rtnVal = true;

            oldValueAsString = "";
            newValueAsString = "";

            try
            {
                switch (pt)
                {
                    case ProcessingType.DateTime:
                        DateTime oldDate = (DateTime)oldValue;
                        oldValueAsString = oldDate.ToString("yyyy-MM-dd hh:mm:ss");

                        DateTime newDate = (DateTime)newValue;
                        newValueAsString = newDate.ToString("yyyy-MM-dd hh:mm:ss");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ChangeTracking_ExtraProcessing Type " + pt.ToString() + ": " + ex);
            }

            return rtnVal;

        }
    }
}
