using System.ComponentModel.DataAnnotations;
using DevicesAPI.Domain.Enums;

namespace DevicesAPI.Application.DTOs;

public class UpdateDeviceRequest
{
	[Required]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;

	[Required]
	[StringLength(100)]
	public string Brand { get; set; } = string.Empty;

	[Required]
	public DeviceState State { get; set; }
}