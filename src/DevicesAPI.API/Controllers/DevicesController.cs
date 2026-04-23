using DevicesAPI.Application.DTOs;
using DevicesAPI.Application.Interfaces;
using DevicesAPI.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DevicesAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DevicesController : ControllerBase
{
	private readonly IDeviceService _service;

	public DevicesController(IDeviceService service)
	{
		_service = service;
	}

	/// <summary>Creates a new device.</summary>
	/// <response code="201">Device created successfully.</response>
	/// <response code="400">Validation error.</response>
	[HttpPost]
	[ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Create([FromBody] CreateDeviceRequest request, CancellationToken cancellationToken)
	{
		var result = await _service.CreateAsync(request, cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	/// <summary>Gets a device by ID.</summary>
	/// <response code="200">Device found.</response>
	/// <response code="404">Device not found.</response>
	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
	{
		var result = await _service.GetByIdAsync(id, cancellationToken);
		return result is null ? NotFound() : Ok(result);
	}

	/// <summary>Gets all devices, optionally filtered by brand or state.</summary>
	/// <response code="200">List of devices.</response>
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<DeviceResponse>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(
		[FromQuery] string? brand,
		[FromQuery] DeviceState? state,
		CancellationToken cancellationToken)
	{
		if (brand is not null)
			return Ok(await _service.GetByBrandAsync(brand, cancellationToken));

		if (state.HasValue)
			return Ok(await _service.GetByStateAsync(state.Value, cancellationToken));

		return Ok(await _service.GetAllAsync(cancellationToken));
	}

	/// <summary>Fully updates an existing device.</summary>
	/// <response code="200">Device updated successfully.</response>
	/// <response code="400">Validation error.</response>
	/// <response code="404">Device not found.</response>
	/// <response code="409">Device is in use — name or brand cannot be changed.</response>
	[HttpPut("{id:guid}")]
	[ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeviceRequest request, CancellationToken cancellationToken)
	{
		var result = await _service.UpdateAsync(id, request, cancellationToken);
		return Ok(result);
	}

	/// <summary>Partially updates an existing device.</summary>
	/// <response code="200">Device patched successfully.</response>
	/// <response code="404">Device not found.</response>
	/// <response code="409">Device is in use — name or brand cannot be changed.</response>
	[HttpPatch("{id:guid}")]
	[ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	public async Task<IActionResult> Patch(Guid id, [FromBody] PatchDeviceRequest request, CancellationToken cancellationToken)
	{
		var result = await _service.PatchAsync(id, request, cancellationToken);
		return Ok(result);
	}

	/// <summary>Deletes a device by ID.</summary>
	/// <response code="204">Device deleted.</response>
	/// <response code="404">Device not found.</response>
	/// <response code="409">Device is in use and cannot be deleted.</response>
	[HttpDelete("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
	{
		await _service.DeleteAsync(id, cancellationToken);
		return NoContent();
	}
}