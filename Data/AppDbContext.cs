using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;

namespace SocialNetwork.Data;

public class AppDbContext : DbContext 
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseMySql("Server=localhost;Database=SocialNetwork;User=root;Password=32105",
            new MySqlServerVersion(new Version(8, 0, 26)));
}