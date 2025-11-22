using Microsoft.EntityFrameworkCore;
public class ApplicationContext : DbContext
{   
    public DbSet<DateInformation> DateInformations => Set<DateInformation>();

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<DateInformation>(e => 
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Year, x.Month }).IsUnique();
            e.Property(x => x.Year).IsRequired();
            e.Property(x => x.Month).IsRequired();
            e.Property(x => x.Name).IsRequired();
        });
    }
    
}