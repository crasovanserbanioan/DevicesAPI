using DevicesAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevicesAPI.Infrastructure.Data.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
	public void Configure(EntityTypeBuilder<Device> builder)
	{
		builder.ToTable("devices");

		builder.HasKey(d => d.Id);

		builder.Property(d => d.Id)
			.HasColumnName("id");

		builder.Property(d => d.Name)
			.HasColumnName("name")
			.HasMaxLength(100)
			.IsRequired();

		builder.Property(d => d.Brand)
			.HasColumnName("brand")
			.HasMaxLength(100)
			.IsRequired();

		builder.Property(d => d.State)
			.HasColumnName("state")
			.IsRequired()
			.HasConversion<string>();

		builder.Property(d => d.CreationTime)
			.HasColumnName("creation_time")
			.IsRequired();

		builder.HasIndex(d => d.Brand)
			.HasDatabaseName("ix_devices_brand");

		builder.HasIndex(d => d.State)
			.HasDatabaseName("ix_devices_state");
	}
}