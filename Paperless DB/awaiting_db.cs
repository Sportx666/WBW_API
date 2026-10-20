using Microsoft.EntityFrameworkCore;
using wbm_common.NonDatabaseObjects;
using wbm_common.Paperless_DB_DataObjects;

namespace WBM_API.Paperless_DB
{
    public partial class PaperlessDatabase
    {
        public async Task<Result<List<PURCH_ORD_PROD_DW>>> PURCH_ORD_PROD_DW_ListByKEYID(List<string> ListKeyID)
        {
            try
            {
                List<PURCH_ORD_PROD_DW> PURCH_ORD_PROD_DWDb = await _dbContext.PURCH_ORD_PROD_DW.AsNoTracking()
                                                                        .Where(x => ListKeyID.Any(y => x.KEYID.Contains(y))).ToListAsync();

                if (PURCH_ORD_PROD_DWDb == null)
                    return Result.Success<List<PURCH_ORD_PROD_DW>>(new List<PURCH_ORD_PROD_DW>());
                else
                    return Result.Success<List<PURCH_ORD_PROD_DW>>(PURCH_ORD_PROD_DWDb);
            }
            catch (Exception ex)
            {
                logger.Error("PURCH_ORD_PROD_DW_ListByKEYID - " + string.Join(",", ListKeyID) + ": " + ex);
                return Result.Exception<List<PURCH_ORD_PROD_DW>>("Internal Exception occured");
            }
        }
        public async Task<Result<List<PICK_HEAD_DW>>> PICK_HEAD_DW_ListByKEYID(List<string> ListKeyID)
        {
            try
            {
                List<PICK_HEAD_DW> PURCH_ORD_PROD_DWDb = await _dbContext.PICK_HEAD_DW.AsNoTracking()
                                                                        .Where(x => ListKeyID.Any(y => x.KEYID.Contains(y))).ToListAsync();

                if (PURCH_ORD_PROD_DWDb == null)
                    return Result.Success<List<PICK_HEAD_DW>>(new List<PICK_HEAD_DW>());
                else
                    return Result.Success<List<PICK_HEAD_DW>>(PURCH_ORD_PROD_DWDb);
            }
            catch (Exception ex)
            {
                logger.Error("PICK_HEAD_DW_ListByKEYID - " + string.Join(",", ListKeyID) + ": " + ex);
                return Result.Exception<List<PICK_HEAD_DW>>("Internal Exception occured");
            }
        }
    }
}
