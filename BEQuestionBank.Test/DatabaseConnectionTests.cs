using BEQuestionBank.Core.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BEQuestionBank.Test;

public class DatabaseConnectionTests
{
    [Fact]
    public async Task CanConnectToDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=ep-cold-star-a1a1hz0u-pooler.ap-southeast-1.aws.neon.tech;Port=5432;Username=neondb_owner;Password=npg_jvgO2CLGT8Ww;Database=neondb;SSL Mode=Require")
            .Options;

        using var context = new AppDbContext(options);
        var canConnect = await context.Database.CanConnectAsync();

        Assert.True(canConnect);
    }
}