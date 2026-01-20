using Microsoft.EntityFrameworkCore;
using WBM_API.Models;

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
    }
}
