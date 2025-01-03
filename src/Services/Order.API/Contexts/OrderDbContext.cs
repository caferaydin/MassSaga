using Microsoft.EntityFrameworkCore;

namespace Order.API.Contexts
{
    public class OrderDbContext : DbContext
    {

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }
        public DbSet<Models.Order> Orders { get; set; }
        public DbSet<Models.OrderItem> OrderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Order>().ToTable("Orders");
            modelBuilder.Entity<Models.OrderItem>().ToTable("OrderItems");
        }
    }
}
