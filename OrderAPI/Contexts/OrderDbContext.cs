using Microsoft.EntityFrameworkCore;
using Shared;

namespace OrderAPI.Contexts
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderState> OrderStates { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // OrderState'in tablo adını belirtmek
            modelBuilder.Entity<OrderState>()
                .ToTable("OrderStates"); // Tablo adı "OrderStates" olarak ayarlanır
        }

    }



  
}
