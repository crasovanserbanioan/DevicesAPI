using DevicesAPI.Application.DTOs;
using DevicesAPI.Application.Interfaces;
using DevicesAPI.Application.Mappings;
using DevicesAPI.Domain.Enums;
using DevicesAPI.Domain.Interfaces;

namespace DevicesAPI.Application.Services;

public class DeviceService : IDeviceService
{
	private readonly IDeviceRepository _repository;

	public DeviceService(IDeviceRepository repository)
	{
		_repository = repository;
	}

	public async Task<DeviceResponse> CreateAsync(CreateDeviceRequest request, CancellationToken cancellationToken = default)
	{
		var device = request.ToEntity();
		var created = await _repository.AddAsync(device, cancellationToken);
		return created.ToResponse();
	}

	public async Task<DeviceResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var device = await _repository.GetByIdAsync(id, cancellationToken);
		return device?.ToResponse();
	}

	public async Task<IEnumerable<DeviceResponse>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var devices = await _repository.GetAllAsync(cancellationToken);
		return devices.Select(d => d.ToResponse());
	}

	public async Task<IEnumerable<DeviceResponse>> GetByBrandAsync(string brand, CancellationToken cancellationToken = default)
	{
		var devices = await _repository.GetByBrandAsync(brand, cancellationToken);
		return devices.Select(d => d.ToResponse());
	}

	public async Task<IEnumerable<DeviceResponse>> GetByStateAsync(DeviceState state, CancellationToken cancellationToken = default)
	{
		var devices = await _repository.GetByStateAsync(state, cancellationToken);
		return devices.Select(d => d.ToResponse());
	}

	public async Task<DeviceResponse> UpdateAsync(Guid id, UpdateDeviceRequest request, CancellationToken cancellationToken = default)
	{
		var device = await _repository.GetByIdAsync(id, cancellationToken)
			?? throw new KeyNotFoundException($"Device with ID '{id}' was not found.");

		if (device.State == DeviceState.InUse &&
			(device.Name != request.Name || device.Brand != request.Brand))
		{
			throw new InvalidOperationException("Cannot update name or brand of a device that is currently in use.");
		}

		device.Name = request.Name;
		device.Brand = request.Brand;
		device.State = request.State;

		await _repository.UpdateAsync(device, cancellationToken);
		return device.ToResponse();
	}

	public async Task<DeviceResponse> PatchAsync(Guid id, PatchDeviceRequest request, CancellationToken cancellationToken = default)
	{
		var device = await _repository.GetByIdAsync(id, cancellationToken)
			?? throw new KeyNotFoundException($"Device with ID '{id}' was not found.");

		if (device.State == DeviceState.InUse)
		{
			if (request.Name is not null && request.Name != device.Name)
				throw new InvalidOperationException("Cannot update name of a device that is currently in use.");

			if (request.Brand is not null && request.Brand != device.Brand)
				throw new InvalidOperationException("Cannot update brand of a device that is currently in use.");
		}

		if (request.Name is not null)
			device.Name = request.Name;

		if (request.Brand is not null)
			device.Brand = request.Brand;

		if (request.State.HasValue)
			device.State = request.State.Value;

		await _repository.UpdateAsync(device, cancellationToken);
		return device.ToResponse();
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var device = await _repository.GetByIdAsync(id, cancellationToken)
			?? throw new KeyNotFoundException($"Device with ID '{id}' was not found.");

		if (device.State == DeviceState.InUse)
			throw new InvalidOperationException("Cannot delete a device that is currently in use.");

		await _repository.DeleteAsync(device, cancellationToken);
	}
}