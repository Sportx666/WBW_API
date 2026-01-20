using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WBM_API.Models;
using wbm_common;
using wbm_common.DataObjects;

namespace WBM_API.WBM_API_DB
{

    public class wbmdbcontext : DbContext
    {
        public wbmdbcontext()
        {
        }

        public wbmdbcontext(DbContextOptions<wbmdbcontext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<wbm_common.DataObjects.DateBasedCounters.dbRow>()
                .HasKey(d => new { d.CounterDefnID, d.Date }); // Defines composite key

            modelBuilder.Entity<wbm_common.DataObjects.APICallStats.dbRow>()
                .HasKey(a => new { a.APIEndPoint, a.Date }); // Defines composite key
        }
 
        public DbSet<wbm_common.DataObjects.APICallStats.dbRow> APICallStats { get; set; }

        public DbSet<wbm_common.DataObjects.ActionRateCode.dbRow> ActionRateCode { get; set; }

        public DbSet<wbm_common.DataObjects.ActionType.dbRow> ActionType { get; set; }

        public DbSet<wbm_common.DataObjects.Carrier.dbRow> Carrier { get; set; }

        public DbSet<wbm_common.DataObjects.ChangeDetail.dbRow> ChangeDetail { get; set; }

        public DbSet<wbm_common.DataObjects.ChangeHeader.dbRow> ChangeHeader { get; set; }

        public DbSet<wbm_common.DataObjects.ChangeHeader_TableName.dbRow> ChangeHeader_TableName { get; set; }

        public DbSet<wbm_common.DataObjects.Company.dbRow> Company { get; set; }

        public DbSet<wbm_common.DataObjects.ConnoteFee.dbRow> ConnoteFee { get; set; }

        public DbSet<wbm_common.DataObjects.ContainerHeavyLift.dbRow> ContainerHeavyLift { get; set; }

        public DbSet<wbm_common.DataObjects.ContainerLift.dbRow> ContainerLift { get; set; }

        public DbSet<wbm_common.DataObjects.ContainerSize.dbRow> ContainerSize { get; set; }

        public DbSet<wbm_common.DataObjects.ContainerUnLoadType.dbRow> ContainerUnLoadType { get; set; }

        public DbSet<wbm_common.DataObjects.CustomerAddress.dbRow> CustomerAddress { get; set; }

        public DbSet<wbm_common.DataObjects.CustomerAddressDefault.dbRow> CustomerAddressDefault { get; set; }

        public DbSet<wbm_common.DataObjects.DateBasedCounterDefn.dbRow> DateBasedCounterDefn { get; set; }

        public DbSet<wbm_common.DataObjects.DateBasedCounters.dbRow> DateBasedCounters { get; set; }

        public DbSet<wbm_common.DataObjects.DevanLookup.dbRow> DevanLookup { get; set; }

        public DbSet<wbm_common.DataObjects.InvoiceCycle.dbRow> InvoiceCycle { get; set; }

        public DbSet<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow> InvoiceCycleCurrent { get; set; }

        public DbSet<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> InvoiceCycleHistory { get; set; }

        public DbSet<wbm_common.DataObjects.Location.dbRow> Location { get; set; }

        public DbSet<wbm_common.DataObjects.Log.dbRow> Log { get; set; }

        public DbSet<wbm_common.DataObjects.LongRunProcessHeader.dbRow> LongRunProcessHeader { get; set; }

        public DbSet<wbm_common.DataObjects.LongRunProcessStatus.dbRow> LongRunProcessStatus { get; set; }

        public DbSet<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow> LongRunProcessStatusDescription { get; set; }

        public DbSet<wbm_common.DataObjects.ManhattenFiles.dbRow> ManhattenFiles { get; set; }

        public DbSet<wbm_common.DataObjects.ManhattenFileType.dbRow> ManhattenFileType { get; set; }

        public DbSet<wbm_common.DataObjects.NoteType.dbRow> NoteType { get; set; }

        public DbSet<wbm_common.DataObjects.OrderDetail.dbRow> OrderDetail { get; set; }

        public DbSet<wbm_common.DataObjects.OrderHeader.dbRow> OrderHeader { get; set; }

        public DbSet<wbm_common.DataObjects.Owner.dbRow> Owner { get; set; }

        public DbSet<wbm_common.DataObjects.PalletStorageType.dbRow> PalletStorageType { get; set; }

        public DbSet<wbm_common.DataObjects.PaperlessDataType.dbRow> PaperlessDataType { get; set; }

        public DbSet<wbm_common.DataObjects.PaperlessTransactionXref.dbRow> PaperlessTransactionXref { get; set; }

        public DbSet<wbm_common.DataObjects.PaperlessXrefTranslate.dbRow> PaperlessXrefTranslate { get; set; }

        public DbSet<wbm_common.DataObjects.PriorityOrder.dbRow> PriorityOrder { get; set; }

        public DbSet<wbm_common.DataObjects.ProcessingRule.dbRow> ProcessingRule { get; set; }

        public DbSet<wbm_common.DataObjects.ProcessingRuleDefn.dbRow> ProcessingRuleDefn { get; set; }

        public DbSet<wbm_common.DataObjects.ProcessingRuleProcess.dbRow> ProcessingRuleProcess { get; set; }

        public DbSet<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow> ProcessingRuleTriggerCondition { get; set; }

        public DbSet<wbm_common.DataObjects.PurchaseOrderDetail.dbRow> PurchaseOrderDetail { get; set; }

        public DbSet<wbm_common.DataObjects.PurchaseOrderHeader.dbRow> PurchaseOrderHeader { get; set; }

        public DbSet<wbm_common.DataObjects.RateCode.dbRow> RateCode { get; set; }

        public DbSet<wbm_common.DataObjects.RateCollection.dbRow> RateCollection { get; set; }

        public DbSet<wbm_common.DataObjects.RateCollectionDefn.dbRow> RateCollectionDefn { get; set; }

        public DbSet<wbm_common.DataObjects.RateFunction.dbRow> RateFunction { get; set; }

        public DbSet<wbm_common.DataObjects.Security_Item.dbRow> Security_Item { get; set; }

        public DbSet<wbm_common.DataObjects.Security_ItemCategory.dbRow> Security_ItemCategory { get; set; }

        public DbSet<wbm_common.DataObjects.Security_Role.dbRow> Security_Role { get; set; }

        public DbSet<wbm_common.DataObjects.Security_Role_Item.dbRow> Security_Role_Item { get; set; }

        public DbSet<wbm_common.DataObjects.Security_RoleGroup.dbRow> Security_RoleGroup { get; set; }

        public DbSet<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow> Security_RoleGroup_Role { get; set; }

        public DbSet<wbm_common.DataObjects.Security_User.dbRow> Security_User { get; set; }
        public DbSet<wbm_common.DataObjects.Security_User_Password.dbRow> Security_User_Password { get; set; }

        public DbSet<wbm_common.DataObjects.Security_User_Item.dbRow> Security_User_Item { get; set; }

        public DbSet<wbm_common.DataObjects.Security_User_Role.dbRow> Security_User_Role { get; set; }
        public DbSet<wbm_common.DataObjects.Security_User_RoleGroup.dbRow> Security_User_RoleGroup { get; set; }

        public DbSet<wbm_common.DataObjects.Site.dbRow> Site { get; set; }

        public DbSet<wbm_common.DataObjects.SiteSystem.dbRow> SiteSystem { get; set; }

        public DbSet<wbm_common.DataObjects.Stock.dbRow> Stock { get; set; }

        public DbSet<wbm_common.DataObjects.StockOnHand.dbRow> StockOnHand { get; set; }

        public DbSet<wbm_common.DataObjects.StockRateCategory.dbRow> StockRateCategory { get; set; }

        public DbSet<wbm_common.DataObjects.StorageRateCodeFrom.dbRow> StorageRateCodeFrom { get; set; }

        public DbSet<wbm_common.DataObjects.TableRate.dbRow> TableRate { get; set; }

        public DbSet<wbm_common.DataObjects.ToDo.dbRow> ToDo { get; set; }

        public DbSet<wbm_common.DataObjects.ToDo_Category.dbRow> ToDo_Category { get; set; }

        public DbSet<wbm_common.DataObjects.ToDoStatus_Type.dbRow> ToDoStatus_Type { get; set; }

        public DbSet<wbm_common.DataObjects.TransactionDetail.dbRow> TransactionDetail { get; set; }

        public DbSet<wbm_common.DataObjects.TransactionException.dbRow> TransactionException { get; set; }

        public DbSet<wbm_common.DataObjects.TransactionExceptionType.dbRow> TransactionExceptionType { get; set; }

        public DbSet<wbm_common.DataObjects.TransactionHeader.dbRow> TransactionHeader { get; set; }

        public DbSet<wbm_common.DataObjects.TransactionNotes.dbRow> TransactionNotes { get; set; }

        public DbSet<wbm_common.DataObjects.TransactionSource.dbRow> TransactionSource { get; set; }

        public DbSet<wbm_common.DataObjects.UnitType.dbRow> UnitType { get; set; }

        public DbSet<wbm_common.DataObjects.ZoneProcessing.dbRow> ZoneProcessing { get; set; }

    }
}
