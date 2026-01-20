using NLog;
using NLog.Web;

namespace WBM_API.WBM_API_DB
{
    public partial class WBMDatabase
    {
        private readonly wbmdbcontext _dbContext;
        public wbmdbcontext dbContext
        {
            get { return _dbContext; }
        }

        public WBMDatabase(wbmdbcontext dbContext)
        {
            _dbContext = dbContext;
        }

        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();


    }
}
