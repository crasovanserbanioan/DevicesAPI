using DevicesAPI.Application.DTOs;
using DevicesAPI.Domain.Entities;
using DevicesAPI.Domain.Enums;

namespace DevicesAPI.Application.Mappings;

public static class DeviceMappingExtensions
{
	public static Device ToEntity(this CreateDeviceRequest request)
	{
		return new Device
		{
			Id = Guid.NewGuid(),
			Name = request.Name,
			Brand = request.Brand,
			State = DeviceState.Available,
			CreationTime = DateTime.UtcNow
		};
	}

	public static DeviceResponse ToResponse(this Device device)
	{
		return new DeviceResponse
		{
			Id = device.Id,
			Name = device.Name,
			Brand = device.Brand,
			State = device.State,
			CreationTime = device.CreationTime
		};
	}
}