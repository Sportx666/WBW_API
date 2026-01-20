using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using WBM_API.Models;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        #region Securities to find out access level
        public async Task<Result<List<wbm_common.DataObjects.Security_User.dbRow>>> SecurityUser_ListByListID(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("SecurityUser_ListByListID: IDs was null or blank");
                return Result.Failure<List<wbm_common.DataObjects.Security_User.dbRow>>("SecurityUser_ListByListID: IDs was null or blank");
            }

            try
            {
                List<wbm_common.DataObjects.Security_User.dbRow> Users = await _dbContext.Security_User.Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                if (Users == null || Users.Count() == 0)
                    return Result.Failure<List<wbm_common.DataObjects.Security_User.dbRow>>("SecurityUser_ListByListID: no rows found");
                else
                    return Result.Success<List<wbm_common.DataObjects.Security_User.dbRow>>(Users);
            }
            catch (Exception ex)
            {
                logger.Error("SecurityUser_ListByListID: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Security_User.dbRow>>("SecurityUser_ListByListID: " + ex);
            }
        }

        public async Task<Result<wbm_common.DataObjects.Security_User.dbRow>> SecurityUser_ListByEmailAddress(string EmailAddress)
        {
            try
            {
                wbm_common.DataObjects.Security_User.dbRow user = await _dbContext.Security_User.FirstOrDefaultAsync(x => !x.DeletedFlag && x.Active && x.EmailAddress == EmailAddress);
                if (user == null)
                    return Result.Exception<wbm_common.DataObjects.Security_User.dbRow>("No user found");
                else
                    return Result.Success<wbm_common.DataObjects.Security_User.dbRow>(user);
            }
            catch (Exception ex)
            {
                logger.Error("SecurityUser_ListByEmailAddress: " + EmailAddress + " - " + ex);
                return Result.Exception<wbm_common.DataObjects.Security_User.dbRow>("SecurityUser_ListByEmailAddress: " + EmailAddress + " - " + ex);
            }
        }


        public async Task<Result<wbm_common.DataObjects.Security_User.dbRow>> SecurityUser_GetByUsername(string username, string Password)
        {
            try
            {
                wbm_common.DataObjects.Security_User.dbRow SecUser = await _dbContext.Security_User.FirstOrDefaultAsync(x => !x.DeletedFlag && x.Active && x.PublicID == username);

                if (SecUser != null)
                {
                    // We have a user, now check password
                    wbm_common.DataObjects.Security_User_Password.dbRow passwordCheck = await _dbContext.Security_User_Password.FirstOrDefaultAsync(x => x.UserID == SecUser.ID && x.Password == Password);
                
                    if (passwordCheck == null)
                    {
                        // Password incorrect
                        return Result.Failure<wbm_common.DataObjects.Security_User.dbRow>("Password incorrect, username:" + username);
                    }
                    return Result.Success<wbm_common.DataObjects.Security_User.dbRow>(SecUser);
                }

                return Result.Failure<wbm_common.DataObjects.Security_User.dbRow>("No user found: " + username);
            }
            catch (Exception ex)
            {
                logger.Error("SecurityUser_GetByUsername: " + username + " - " + ex);
                return Result.Exception<wbm_common.DataObjects.Security_User.dbRow>("SecurityUser_GetByUsername: " + username + " - " + ex);
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>> SecurityUserRoleGroup_ListByUserID(int UserID)
        {
            try
            {
                List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow> URGs = await _dbContext.Security_User_RoleGroup.Where(x => !x.DeletedFlag && x.UserID == UserID).ToListAsync();
                if (URGs == null || URGs.Count() == 0)
                    return Result.Failure<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>("No Security_User_RoleGroup rows found");
                else
                    return Result.Success<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>(URGs);
            }
            catch (Exception ex)
            {
                logger.Error("SecurityUserRoleGroup_ListByUserID: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>("SecurityUserRoleGroup_ListByUserID: " + ex);
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>> SecurityRoleGroupRole_ListByRoleGroupID(int RoleGroupID)
        {
            try
            {
                List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow> SRGRs = await _dbContext.Security_RoleGroup_Role.Where(x => !x.DeletedFlag && x.RoleGroupID == RoleGroupID).ToListAsync();
                if (SRGRs == null)
                    return Result.Failure<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>("No Security_RoleGroup_Role rows found");
                else
                    return Result.Success<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>(SRGRs);
            }
            catch (Exception ex)
            {
                logger.Error("SecurityRoleGroupRole_ListByRoleGroupID: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>("SecurityRoleGroupRole_ListByRoleGroupID: " + ex);
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_User_Role.dbRow>>> SecurityUserRole_ListByUserID(int UserID)
        {
            try
            {
                List<wbm_common.DataObjects.Security_User_Role.dbRow> SURs = await _dbContext.Security_User_Role.Where(x => !x.DeletedFlag && x.UserID == UserID).ToListAsync();
                if (SURs == null)
                    return Result.Failure<List<wbm_common.DataObjects.Security_User_Role.dbRow>>("No Security_User_Role rows found");
                else
                    return Result.Success<List<wbm_common.DataObjects.Security_User_Role.dbRow>>(SURs);
            }
            catch (Exception ex)
            {
                return Result.Exception<List<wbm_common.DataObjects.Security_User_Role.dbRow>>("SecurityUserRole_ListByUserID: " + ex);
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>> SecurityRoleItem_ListByRoleID(List<int> RoleIDs)
        {
            try
            {
                List < wbm_common.DataObjects.Security_Role_Item.dbRow > SRIs = await _dbContext.Security_Role_Item.Where(x => !x.DeletedFlag && RoleIDs.Contains(x.RoleID)).ToListAsync();
                if (SRIs == null)
                    return Result.Failure<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>("No Security_Role_Item rows");
                else
                    return Result.Success<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>(SRIs);
            }
            catch (Exception ex)
            {
                logger.Error("SecurityRoleItem_ListByRoleID: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>("SecurityRoleItem_ListByRoleID: " + ex);
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_User_Item.dbRow>>> SecurityUserItem_ListByUserID(int UserID)
        {
            try
            {
                List<wbm_common.DataObjects.Security_User_Item.dbRow> SUIs = await _dbContext.Security_User_Item.Where(x => !x.DeletedFlag && x.UserID == UserID).ToListAsync();
                if (SUIs == null)
                    return Result.Failure<List<wbm_common.DataObjects.Security_User_Item.dbRow>>("No Security_User_Item rows found");
                else
                    return Result.Success<List<wbm_common.DataObjects.Security_User_Item.dbRow>>(SUIs);
            }
            catch (Exception ex)
            {
                logger.Error("SecurityUserItem_ListByUserID: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Security_User_Item.dbRow>>("SecurityUserItem_ListByUserID: " + ex);
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_Item.dbRow>>> SecurityItem_ListByItemIDs(List<int> ItemIDs)
        {
            try
            {
                List<wbm_common.DataObjects.Security_Item.dbRow> SIs = await _dbContext.Security_Item.Where(x => !x.DeletedFlag && ItemIDs.Contains(x.ID)).ToListAsync();
                if (SIs == null)
                    return Result.Failure<List<wbm_common.DataObjects.Security_Item.dbRow>>("No Security_Item rows found");
                else
                    return Result.Success<List<wbm_common.DataObjects.Security_Item.dbRow>>(SIs);
            }
            catch (Exception ex)
            {
                logger.Error("SecurityItem_ListByItemIDs: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Security_Item.dbRow>>("SecurityItem_ListByItemIDs: " + ex);
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_Role.dbRow>>> SecurityRole_ListByRoleIDs(List<int> RoleIDs)
        {
            try
            {
                List < wbm_common.DataObjects.Security_Role.dbRow > SRs = await _dbContext.Security_Role.Where(x => !x.DeletedFlag && RoleIDs.Contains(x.ID)).ToListAsync();
                if (SRs == null)
                    return Result.Failure<List<wbm_common.DataObjects.Security_Role.dbRow>>("no Security_Role rows found");
                else
                    return Result.Success<List<wbm_common.DataObjects.Security_Role.dbRow>>(SRs);
            }
            catch (Exception ex)
            {
                logger.Error("SecurityRole_ListByRoleIDs: " + ex);
                return Result.Exception<List<wbm_common.DataObjects.Security_Role.dbRow>>("SecurityRole_ListByRoleIDs: " + ex);
            }
        }
        #endregion

        #region SecurityItem
        public async Task<Result<List<wbm_common.DataObjects.Security_Item.dbRow>>> Security_Item_List()
        {
            try
            {
                List<wbm_common.DataObjects.Security_Item.dbRow> Security_Items = await _dbContext.Security_Item.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Security_Item.dbRow>>(Security_Items);
            }
            catch (Exception ex)
            {
                logger.Error("Security_Item_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Security_Item.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_Item.dbRow>>> Security_Item_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Security_Item_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Security_Item.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Security_Item.dbRow> Security_Items = await _dbContext.Security_Item.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Security_Item.dbRow>>(Security_Items);
                }
                catch (Exception ex)
                {
                    logger.Error("Security_Item_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Security_Item.dbRow>>("Internal Exception occured");
                }
            }
        }
        #endregion

        #region SecurityItemCategory
        public async Task<Result<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>>> Security_ItemCategory_List()
        {
            try
            {
                List<wbm_common.DataObjects.Security_ItemCategory.dbRow> Security_ItemCategorys = await _dbContext.Security_ItemCategory.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>>(Security_ItemCategorys);
            }
            catch (Exception ex)
            {
                logger.Error("Security_ItemCategory_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>>> Security_ItemCategory_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Security_ItemCategory_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Security_ItemCategory.dbRow> Security_ItemCategorys = await _dbContext.Security_ItemCategory.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>>(Security_ItemCategorys);
                }
                catch (Exception ex)
                {
                    logger.Error("Security_ItemCategory_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>>("Internal Exception occured");
                }
            }
        }
        #endregion

        #region SecurityRole
        public async Task<Result<List<wbm_common.DataObjects.Security_Role.dbRow>>> Security_Role_List()
        {
            try
            {
                List<wbm_common.DataObjects.Security_Role.dbRow> Security_Roles = await _dbContext.Security_Role.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Security_Role.dbRow>>(Security_Roles);
            }
            catch (Exception ex)
            {
                logger.Error("Security_Role_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Security_Role.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_Role.dbRow>>> Security_Role_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Security_Role_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Security_Role.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Security_Role.dbRow> Security_Roles = await _dbContext.Security_Role.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Security_Role.dbRow>>(Security_Roles);
                }
                catch (Exception ex)
                {
                    logger.Error("Security_Role_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Security_Role.dbRow>>("Internal Exception occured");
                }
            }
        }
        #endregion

        #region SecurityRoleItem
        public async Task<Result<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>> Security_Role_Item_List()
        {
            try
            {
                List<wbm_common.DataObjects.Security_Role_Item.dbRow> Security_Role_Items = await _dbContext.Security_Role_Item.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>(Security_Role_Items);
            }
            catch (Exception ex)
            {
                logger.Error("Security_Role_Item_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>> Security_Role_Item_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Security_Role_Item_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Security_Role_Item.dbRow> Security_Role_Items = await _dbContext.Security_Role_Item.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>(Security_Role_Items);
                }
                catch (Exception ex)
                {
                    logger.Error("Security_Role_Item_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>("Internal Exception occured");
                }
            }
        }
        #endregion

        #region SecurityRoleGroup
        public async Task<Result<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>>> Security_RoleGroup_List()
        {
            try
            {
                List<wbm_common.DataObjects.Security_RoleGroup.dbRow> Security_RoleGroups = await _dbContext.Security_RoleGroup.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>>(Security_RoleGroups);
            }
            catch (Exception ex)
            {
                logger.Error("Security_RoleGroup_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>>> Security_RoleGroup_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Security_RoleGroup_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Security_RoleGroup.dbRow> Security_RoleGroups = await _dbContext.Security_RoleGroup.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>>(Security_RoleGroups);
                }
                catch (Exception ex)
                {
                    logger.Error("Security_RoleGroup_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>>("Internal Exception occured");
                }
            }
        }
        #endregion

        #region SecurityRoleGroupRole
        public async Task<Result<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>> Security_RoleGroup_Role_List()
        {
            try
            {
                List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow> Security_RoleGroup_Roles = await _dbContext.Security_RoleGroup_Role.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>(Security_RoleGroup_Roles);
            }
            catch (Exception ex)
            {
                logger.Error("Security_RoleGroup_Role_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>> Security_RoleGroup_Role_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Security_RoleGroup_Role_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow> Security_RoleGroup_Roles = await _dbContext.Security_RoleGroup_Role.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>(Security_RoleGroup_Roles);
                }
                catch (Exception ex)
                {
                    logger.Error("Security_RoleGroup_Role_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>>("Internal Exception occured");
                }
            }
        }
        #endregion

        #region SecurityUserItem
        public async Task<Result<List<wbm_common.DataObjects.Security_User_Item.dbRow>>> Security_User_Item_List()
        {
            try
            {
                List<wbm_common.DataObjects.Security_User_Item.dbRow> Security_User_Items = await _dbContext.Security_User_Item.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Security_User_Item.dbRow>>(Security_User_Items);
            }
            catch (Exception ex)
            {
                logger.Error("Security_User_Item_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Security_User_Item.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_User_Item.dbRow>>> Security_User_Item_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Security_User_Item_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Security_User_Item.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Security_User_Item.dbRow> Security_User_Items = await _dbContext.Security_User_Item.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Security_User_Item.dbRow>>(Security_User_Items);
                }
                catch (Exception ex)
                {
                    logger.Error("Security_User_Item_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Security_User_Item.dbRow>>("Internal Exception occured");
                }
            }
        }
        #endregion

        #region SecurityUserRole
        public async Task<Result<List<wbm_common.DataObjects.Security_User_Role.dbRow>>> Security_User_Role_List()
        {
            try
            {
                List<wbm_common.DataObjects.Security_User_Role.dbRow> Security_User_Roles = await _dbContext.Security_User_Role.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Security_User_Role.dbRow>>(Security_User_Roles);
            }
            catch (Exception ex)
            {
                logger.Error("Security_User_Role_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Security_User_Role.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_User_Role.dbRow>>> Security_User_Role_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Security_User_Role_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Security_User_Role.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Security_User_Role.dbRow> Security_User_Roles = await _dbContext.Security_User_Role.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Security_User_Role.dbRow>>(Security_User_Roles);
                }
                catch (Exception ex)
                {
                    logger.Error("Security_User_Role_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Security_User_Role.dbRow>>("Internal Exception occured");
                }
            }
        }
        #endregion

        #region SecurityUserRoleGroup
        public async Task<Result<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>> Security_User_RoleGroup_List()
        {
            try
            {
                List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow> Security_User_RoleGroups = await _dbContext.Security_User_RoleGroup.AsNoTracking().Where(x => !x.DeletedFlag).ToListAsync();

                return Result.Success<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>(Security_User_RoleGroups);
            }
            catch (Exception ex)
            {
                logger.Error("Security_User_RoleGroup_List: " + ex);

                return Result.Exception<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>("Internal Exception occured");
            }
        }

        public async Task<Result<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>> Security_User_RoleGroup_ListByListIDs(List<int> IDs)
        {
            if (IDs == null || IDs.Count == 0)
            {
                logger.Error("Security_User_RoleGroup_ListByListIDs was null or blank ");
                return Result.Failure<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>("IDs list is null or empty");
            }
            else
            {
                try
                {
                    List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow> Security_User_RoleGroups = await _dbContext.Security_User_RoleGroup.AsNoTracking().Where(x => !x.DeletedFlag && IDs.Contains(x.ID)).ToListAsync();
                    return Result.Success<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>(Security_User_RoleGroups);
                }
                catch (Exception ex)
                {
                    logger.Error("Security_User_RoleGroup_ListByListIDs: " + ex);
                    return Result.Exception<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>("Internal Exception occured");
                }
            }
        }
        #endregion
    }
}
