using DevicesAPI.Domain.Entities;
using DevicesAPI.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DevicesAPI.Infrastructure.Data;

public class DevicesDbContext : DbContext
{
	public DevicesDbContext(DbContextOptions<DevicesDbContext> options) : base(options)
	{
	}

	public DbSet<Device> Devices => Set<Device>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new DeviceConfiguration());
	}
}