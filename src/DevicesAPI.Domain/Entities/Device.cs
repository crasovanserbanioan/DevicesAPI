using DevicesAPI.Domain.Enums;

namespace DevicesAPI.Domain.Entities;

public class Device
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Brand { get; set; } = string.Empty;
	public DeviceState State { get; set; } = DeviceState.Available;
	public DateTime CreationTime { get; set; }
}