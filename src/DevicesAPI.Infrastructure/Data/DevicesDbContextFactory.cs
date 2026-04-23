using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DevicesAPI.Infrastructure.Data;

public class DevicesDbContextFactory : IDesignTimeDbContextFactory<DevicesDbContext>
{
	public DevicesDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<DevicesDbContext>();

		optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=devicesdb;Username=postgres;Password=postgres");

		return new DevicesDbContext(optionsBuilder.Options);
	}
}