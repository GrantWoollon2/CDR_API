using Microsoft.EntityFrameworkCore;
using CDR_API.Models;


namespace CDR_API.Data
{
    public class CallDbContext : DbContext
    {
        //The options parameter for this constructor allows setting database connection variables 
        public CallDbContext(DbContextOptions<CallDbContext> options)
            : base(options)
        { }

        public DbSet<Call> Calls { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 3);
        }


    }
}
