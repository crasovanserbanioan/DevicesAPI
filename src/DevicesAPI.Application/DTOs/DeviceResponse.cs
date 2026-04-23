using DevicesAPI.Domain.Enums;

namespace DevicesAPI.Application.DTOs;

public class DeviceResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Brand { get; set; } = string.Empty;
	public DeviceState State { get; set; }
	public DateTime CreationTime { get; set; }
}