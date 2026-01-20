using Microsoft.AspNetCore.Http.HttpResults;
using WBM_API.Models;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class AccessHelper
    {
        public class SecurityItemHolder
        {
            public wbm_common.DataObjects.Security_Item.dbRow Security_Item { get; set; }
            public int AccessLevel { get; set; }
        }
        public class SecurityItemListHolder
        {
            public string error { get; set; }
            public List<SecurityItemHolder> SecurityItems { get; set; }
        }
        // the error messages currently don't get sent back to user, they are just logged
        public static async Task<SecurityItemListHolder> GetAccessLevels(wbm_common.DataObjects.Security_User.dbRow SecUser, WBMDatabase _WBMDB)
        {
            string errorMessage = "";
            List<SecurityItemHolder> SIsAL = new List<SecurityItemHolder>();

            Result<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>> SURGsResults = await _WBMDB.SecurityUserRoleGroup_ListByUserID(SecUser.ID);
            if (SURGsResults.IsFailure || SURGsResults.IsException)
            {
                //400 error
                return new SecurityItemListHolder
                {
                    error = SURGsResults.ErrorMessage,
                    SecurityItems = SIsAL
                };
            }
            if (SURGsResults.Value.Count() != 1)
            {
                //400 error
                return new SecurityItemListHolder
                {
                    error = "User does not have any permissions; Security_User_RoleGroup Error",
                    SecurityItems = SIsAL
                };
            }
            List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow> SURGs = SURGsResults.Value;

            Result<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>> SRGRsResults = await _WBMDB.SecurityRoleGroupRole_ListByRoleGroupID(SURGs[0].RoleGroupID);
            if (SRGRsResults.IsFailure || SRGRsResults.IsException)
            {
                //400 error
                return new SecurityItemListHolder
                {
                    error = SRGRsResults.ErrorMessage,
                    SecurityItems = SIsAL
                };
            }
            if (SRGRsResults.Value.Count() == 0)
            {
                //400 error
                return new SecurityItemListHolder
                {
                    error = "User does not have any permissions; Security_RoleGroup_Role Error",
                    SecurityItems = SIsAL
                };
            }
            List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow> SRGRs = SRGRsResults.Value;

            Result<List<wbm_common.DataObjects.Security_User_Role.dbRow>> SURsResults = await _WBMDB.SecurityUserRole_ListByUserID(SecUser.ID);
            if (SURsResults.IsException)
            {
                //400 error, could be 500 since it's more we didn't read anything
                return new SecurityItemListHolder
                {
                    error = SURsResults.ErrorMessage,
                    SecurityItems = SIsAL
                };
            }
            List<wbm_common.DataObjects.Security_User_Role.dbRow> SURs = new List<wbm_common.DataObjects.Security_User_Role.dbRow>();
            if (!SURsResults.IsFailure)
                SURs = SURsResults.Value;
            if (SURsResults.Value == null)
            {
                //400 error, could be 500 since it's more we didn't read anything
                return new SecurityItemListHolder
                {
                    error = "User does not have any permissions; Security_User_Role Error",
                    SecurityItems = SIsAL
                };
            }

            Result<List<wbm_common.DataObjects.Security_User_Item.dbRow>> SUIsResult = await _WBMDB.SecurityUserItem_ListByUserID(SecUser.ID);
            if (SUIsResult.IsException)
            {
                return new SecurityItemListHolder
                {
                    error = SUIsResult.ErrorMessage,
                    SecurityItems = SIsAL
                };
            }
            List<wbm_common.DataObjects.Security_User_Item.dbRow> SUIs = new List<wbm_common.DataObjects.Security_User_Item.dbRow>();
            if (!SUIsResult.IsFailure)
                SUIs = SUIsResult.Value;
            if (SUIs == null)
            {
                //400 error, could be 500 since it's more we didn't read anything
                return new SecurityItemListHolder
                {
                    error = "User does not have any permissions; Security_User_Item Error",
                    SecurityItems = SIsAL
                };
            }

            // combine two sources of roles
            Result<List<wbm_common.DataObjects.Security_Role.dbRow>> SRsResult = await _WBMDB.SecurityRole_ListByRoleIDs(SRGRs.Select(x => x.RoleID).Union(SURs.Select(x => x.RoleID))
                .Where(x => !SURs.Any(y => x == y.RoleID && y.AccessLevel == 0)) // remove access level 0
                .ToList());
            if (SRsResult.IsException)
            {
                return new SecurityItemListHolder
                {
                    error = SRsResult.ErrorMessage,
                    SecurityItems = SIsAL
                };
            }
            List<wbm_common.DataObjects.Security_Role.dbRow> SRs = new List<wbm_common.DataObjects.Security_Role.dbRow>();
            if (!SRsResult.IsFailure)
                SRs = SRsResult.Value;
            if (SRs == null)
            {
                //400 error, could be 500 since it's more we didn't read anything
                return new SecurityItemListHolder
                {
                    error = "User does not have any permissions; Security_Role Error",
                    SecurityItems = SIsAL
                };
            }
            List<(wbm_common.DataObjects.Security_Role.dbRow, int)> SRsAL = new List<(wbm_common.DataObjects.Security_Role.dbRow, int)>();
            foreach (wbm_common.DataObjects.Security_Role.dbRow SR in SRs)
            {
                if (SURs.Any(x => x.RoleID == SR.ID))
                    SRsAL.Add((SR, SURs.First(x => x.RoleID == SR.ID).AccessLevel)); // user access takes priority
                else
                    SRsAL.Add((SR, 2));
            }

            Result<List<wbm_common.DataObjects.Security_Role_Item.dbRow>> SRIsResult = await _WBMDB.SecurityRoleItem_ListByRoleID(SRs.Select(x => x.ID).ToList());
            if (SRIsResult.IsException)
            {
                //400 error, could be 500 since it's more we didn't read anything
                return new SecurityItemListHolder
                {
                    error = SRIsResult.ErrorMessage,
                    SecurityItems = SIsAL
                };
            }
            List<wbm_common.DataObjects.Security_Role_Item.dbRow> SRIs = new List<wbm_common.DataObjects.Security_Role_Item.dbRow>();
            if (!SRIsResult.IsFailure)
                SRIs = SRIsResult.Value;
            if (SRIsResult.Value == null)
            {
                //400 error, could be 500 since it's more we didn't read anything
                return new SecurityItemListHolder
                {
                    error = "User does not have any permissions; Security_Role_Item Error",
                    SecurityItems = SIsAL
                };
            }

            List<(wbm_common.DataObjects.Security_Role_Item.dbRow, int)> SRIsAL = new List<(wbm_common.DataObjects.Security_Role_Item.dbRow, int)>();
            foreach (wbm_common.DataObjects.Security_Role_Item.dbRow SRI in SRIs)
            {
                if (SRsAL.Any(x => x.Item1.ID == SRI.RoleID) && SURs.Any(x => x.RoleID == SRI.RoleID)) // keep user access as priority
                    SRIsAL.Add((SRI, SRsAL.First(x => x.Item1.ID == SRI.RoleID).Item2));
                else
                    SRIsAL.Add((SRI, SRI.AccessLevel));
            }

            Result<List<wbm_common.DataObjects.Security_Item.dbRow>> SIsResult = await _WBMDB.SecurityItem_ListByItemIDs(SUIs.Select(x => x.ItemID).Union(SRIs.Select(x => x.ItemID))
                .Where(x => !SUIs.Any(y => x == y.ItemID && y.AccessLevel == 0)) // remove access level 0
                .Where(x => !SRIs.Any(y => x == y.ItemID && y.AccessLevel == 0)) // remove access level 0
                .ToList());
            if (SIsResult.IsException)
            {
                return new SecurityItemListHolder
                {
                    error = SIsResult.ErrorMessage,
                    SecurityItems = SIsAL
                };
            }
            List<wbm_common.DataObjects.Security_Item.dbRow> SIs = new List<wbm_common.DataObjects.Security_Item.dbRow>();
            if (!SIsResult.IsFailure)
                SIs = SIsResult.Value;
            if (SIs == null)
            {
                //400 error, could be 500 since it's more we didn't read anything
                return new SecurityItemListHolder
                {
                    error = "User does not have any permissions; Security_Item Error",
                    SecurityItems = SIsAL
                };
            }
            foreach (wbm_common.DataObjects.Security_Item.dbRow SI in SIs)
            {
                if (SUIs.Any(x => x.ItemID == SI.ID))
                    SIsAL.Add(new SecurityItemHolder
                    {
                        Security_Item = SI,
                        AccessLevel = SUIs.First(x => x.ItemID == SI.ID).AccessLevel
                    }); // have user item has highest priority
                else if (SRIsAL.Any(x => x.Item1.ItemID == SI.ID))
                    SIsAL.Add(new SecurityItemHolder
                    {
                        Security_Item = SI,
                        AccessLevel = SRIsAL.First(x => x.Item1.ItemID == SI.ID).Item2
                    });
                else
                    SIsAL.Add(new SecurityItemHolder
                    {
                        Security_Item = SI,
                        AccessLevel = 2
                    });
            }
            return new SecurityItemListHolder
            {
                error = errorMessage,
                SecurityItems = SIsAL
            };
        }
    }
}
