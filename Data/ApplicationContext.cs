using Microsoft.EntityFrameworkCore;
using TestSwaggerAPI.Models;

namespace TestSwaggerAPI.Data;

public class ApplicationContext : DbContext
{   
    public DbSet<WorkCalendar> DateInformations => Set<WorkCalendar>();

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<WorkCalendar>(e => 
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Year, x.Month }).IsUnique();
            e.Property(x => x.Year).IsRequired();
            e.Property(x => x.Month).IsRequired();
            e.Property(x => x.Name).IsRequired();
        });
    }
    
}