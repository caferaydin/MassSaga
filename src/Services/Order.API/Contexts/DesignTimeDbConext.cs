using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Order.API.Contexts
{
    public class DesignTimeDbConext : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
            //optionsBuilder.UseNpgsql(Configuration.ConnectionString);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=OrderAPI;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=True;Trust Server Certificate=True;Command Timeout=0");


            return new OrderDbContext(optionsBuilder.Options);
        }
    }
}
