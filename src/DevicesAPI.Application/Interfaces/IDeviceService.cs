using DevicesAPI.Application.DTOs;
using DevicesAPI.Domain.Enums;

namespace DevicesAPI.Application.Interfaces;

public interface IDeviceService
{
	Task<DeviceResponse> CreateAsync(CreateDeviceRequest request, CancellationToken cancellationToken = default);
	Task<DeviceResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IEnumerable<DeviceResponse>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<IEnumerable<DeviceResponse>> GetByBrandAsync(string brand, CancellationToken cancellationToken = default);
	Task<IEnumerable<DeviceResponse>> GetByStateAsync(DeviceState state, CancellationToken cancellationToken = default);
	Task<DeviceResponse> UpdateAsync(Guid id, UpdateDeviceRequest request, CancellationToken cancellationToken = default);
	Task<DeviceResponse> PatchAsync(Guid id, PatchDeviceRequest request, CancellationToken cancellationToken = default);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}