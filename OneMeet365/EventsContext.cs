using Microsoft.EntityFrameworkCore;

namespace OneMeet365
{
    public class EventsContext : DbContext
    {
        public EventsContext(DbContextOptions<EventsContext> options) : base(options) { }
        public DbSet<EventCardData> Events { get; set; }
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventCardData>().HasIndex(e => e.ResourceResponseId).IsUnique();
        }
    }    
}
