using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;

namespace SocialNetwork.Data;

public class AppDbContext : DbContext 
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseMySql("Server=us-cdbr-east-06.cleardb.net;Database=heroku_16cc917e02fee03;UserId=bc300c9d2cbb35;Password=2596311e;ConvertZeroDatetime=True",
            new MySqlServerVersion(new Version(8, 0, 26)));
}