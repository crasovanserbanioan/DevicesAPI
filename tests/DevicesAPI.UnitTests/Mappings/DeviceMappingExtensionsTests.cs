using DevicesAPI.Application.DTOs;
using DevicesAPI.Application.Mappings;
using DevicesAPI.Domain.Entities;
using DevicesAPI.Domain.Enums;
using FluentAssertions;

namespace DevicesAPI.UnitTests.Mappings;

public class DeviceMappingExtensionsTests
{
	[Fact]
	public void ToEntity_ShouldMapAllFields_AndGenerateIdAndCreationTime()
	{
		var request = new CreateDeviceRequest { Name = "Phone", Brand = "Acme" };

		var entity = request.ToEntity();

		entity.Id.Should().NotBeEmpty();
		entity.Name.Should().Be("Phone");
		entity.Brand.Should().Be("Acme");
		entity.State.Should().Be(DeviceState.Available);
		entity.CreationTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public void ToResponse_ShouldMapAllFields()
	{
		var device = new Device
		{
			Id = Guid.NewGuid(),
			Name = "Tablet",
			Brand = "BrandX",
			State = DeviceState.InUse,
			CreationTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
		};

		var response = device.ToResponse();

		response.Id.Should().Be(device.Id);
		response.Name.Should().Be("Tablet");
		response.Brand.Should().Be("BrandX");
		response.State.Should().Be(DeviceState.InUse);
		response.CreationTime.Should().Be(device.CreationTime);
	}
}