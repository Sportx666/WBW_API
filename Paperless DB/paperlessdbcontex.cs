using Microsoft.EntityFrameworkCore;
using wbm_common.Paperless_DB_DataObjects;

namespace WBM_API.Paperless_DB
{
    public class paperlessdbcontext : DbContext
    {
        public paperlessdbcontext()
        {
        }

        public paperlessdbcontext(DbContextOptions<paperlessdbcontext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<PURCH_ORD_PROD_DW> PURCH_ORD_PROD_DW { get; set; }

        public DbSet<PICK_HEAD_DW> PICK_HEAD_DW { get; set; }

    }
}
