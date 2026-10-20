using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        public async Task<Result<int>> ChangeHeader_Insert(wbm_common.DataObjects.ChangeHeader.dbRow changeHeader)
        {
            try
            {
                await _dbContext.ChangeHeader.AddAsync(changeHeader);
                await _dbContext.SaveChangesAsync();

                return Result.Success(changeHeader.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ChangeHeader_Insert - " + changeHeader.ChangeHeaderTableID.ToString() + " - "
                    + changeHeader.RecordID.ToString() + " : " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }
        public async Task<Result<List<wbm_common.DataObjects.ChangeHeader_TableName.dbRow>>> ChangeHeader_TableName_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("ChangeHeader_TableName_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.ChangeHeader_TableName.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.ChangeHeader_TableName.dbRow> ChangeHeader_TableNames = await _dbContext.ChangeHeader_TableName.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.ChangeHeader_TableName.dbRow>>(ChangeHeader_TableNames);
                }
                catch (Exception ex)
                {
                    logger.Error("ChangeHeader_TableName_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.ChangeHeader_TableName.dbRow>>("Internal Exception occured");
                }
            }
        }


        public async Task<Result<int>> ChangeDetail_Insert(wbm_common.DataObjects.ChangeDetail.dbRow changeDetail)
        {
            try
            {
                if (changeDetail.OldValue == null)
                    changeDetail.OldValue = "";
                await _dbContext.ChangeDetail.AddAsync(changeDetail);
                await _dbContext.SaveChangesAsync();
                return Result.Success(changeDetail.ID);
            }
            catch (Exception ex)
            {
                logger.Error("ChangeDetail_Insert - " + changeDetail.ChangeHeaderTableID.ToString() + " - "
                    + changeDetail.RecordID.ToString() + " : " + ex);
                return Result.Exception<int>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.ChangeHeader.dbRowExtended>>> ChangeHeader_ListForTableIDRecordID(int TableID, int RecordID)
        {
            try
            {
                List<wbm_common.DataObjects.ChangeHeader.dbRowExtended> ListCH = await (from ChangeHeader in _dbContext.ChangeHeader.AsNoTracking()
                                                                        .Where(x => x.ChangeHeaderTableID == TableID && x.RecordID == RecordID)
                                                                                  join SecurityUser in _dbContext.Security_User.AsNoTracking()
                                                                                      on ChangeHeader.CreatedUserID equals SecurityUser.ID

                                                                                  select new wbm_common.DataObjects.ChangeHeader.dbRowExtended
                                                                                  {
                                                                                      ID = ChangeHeader.ID,
                                                                                      ChangeHeaderTableID = ChangeHeader.ChangeHeaderTableID,
                                                                                      RecordID = ChangeHeader.RecordID,
                                                                                      ChangeTime = ChangeHeader.ChangeTime,
                                                                                      CreatedDateTime = ChangeHeader.CreatedDateTime,
                                                                                      CreatedMethod = ChangeHeader.CreatedMethod,
                                                                                      CreatedUserID = ChangeHeader.CreatedUserID,
                                                                                      LastAmendedDateTime = ChangeHeader.LastAmendedDateTime,
                                                                                      LastAmendedMethod = ChangeHeader.LastAmendedMethod,
                                                                                      LastAmendedUserID = ChangeHeader.LastAmendedUserID,

                                                                                      Created_UserPublicID = SecurityUser.PublicID
                                                                                  }).OrderByDescending(x => x.ChangeTime).ToListAsync();

                if (ListCH == null)
                    return Result.Success<List<wbm_common.DataObjects.ChangeHeader.dbRowExtended>>(new List<wbm_common.DataObjects.ChangeHeader.dbRowExtended>());
                else
                    return Result.Success<List<wbm_common.DataObjects.ChangeHeader.dbRowExtended>>(ListCH);
            }
            catch (Exception ex)
            {
                logger.Error("ChangeHeader_ListForTableIDRecordID: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.ChangeHeader.dbRowExtended>>("Internal Exception occured");
            }
        }


        public async Task<Result<List<wbm_common.DataObjects.ChangeDetail.dbRow>>> ChangeDetail_ListByListHeaderID(List<int> ListCHHeaderID)
        {
            try
            {
                List<wbm_common.DataObjects.ChangeDetail.dbRow> ListChangeDetail = await _dbContext.ChangeDetail.AsNoTracking().Where(x => ListCHHeaderID.Contains(x.ChangeHeaderID)).OrderBy(x => x.ChangeHeaderID).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.ChangeDetail.dbRow>>(ListChangeDetail);
            }
            catch (Exception ex)
            {
                logger.Error("ChangeDetail_ListByListHeaderID: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.ChangeDetail.dbRow>>("Internal Exception occured");
            }
        }

    }
}
