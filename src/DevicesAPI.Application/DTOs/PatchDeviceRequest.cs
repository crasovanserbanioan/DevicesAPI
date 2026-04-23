using DevicesAPI.Domain.Enums;

namespace DevicesAPI.Application.DTOs;

public class PatchDeviceRequest
{
	public string? Name { get; set; }
	public string? Brand { get; set; }
	public DeviceState? State { get; set; }
}