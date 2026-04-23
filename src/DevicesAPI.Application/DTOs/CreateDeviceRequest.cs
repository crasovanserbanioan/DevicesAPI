using System.ComponentModel.DataAnnotations;

namespace DevicesAPI.Application.DTOs;

public class CreateDeviceRequest
{
	[Required]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;

	[Required]
	[StringLength(100)]
	public string Brand { get; set; } = string.Empty;
}