using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Cache.DAL
{
    public class AppDbConfiguration : DbConfiguration
    {
        public AppDbConfiguration()
        {
            SetDefaultConnectionFactory(new SqlConnectionFactory());
            SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
        }
    }
}