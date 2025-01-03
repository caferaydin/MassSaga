using Microsoft.EntityFrameworkCore;

namespace Order.Worker.contexts
{
    public class WorkerDbContext : DbContext
    {
    }


    public class OrderModel
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
