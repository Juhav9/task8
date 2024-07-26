using Microsoft.EntityFrameworkCore;
using BlazorSignalRApp.Shared;

namespace BlazorSignalRApp.Server.Data;

public class AppDataContext : DbContext
{
    public AppDataContext(DbContextOptions<AppDataContext> options)
           : base(options)
    {
    }

    public DbSet<ChatMessageNotification> Messages { get; set; } = null!;
    
    // TODO: implement the required DbSet
    public DbSet<Observation> Observations { get; set; } = null!;
    
}