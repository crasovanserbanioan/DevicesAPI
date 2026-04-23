using DevicesAPI.Domain.Entities;
using DevicesAPI.Domain.Enums;

namespace DevicesAPI.Domain.Interfaces;

public interface IDeviceRepository
{
	Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<IEnumerable<Device>> GetByBrandAsync(string brand, CancellationToken cancellationToken = default);
	Task<IEnumerable<Device>> GetByStateAsync(DeviceState state, CancellationToken cancellationToken = default);
	Task<Device> AddAsync(Device device, CancellationToken cancellationToken = default);
	Task UpdateAsync(Device device, CancellationToken cancellationToken = default);
	Task DeleteAsync(Device device, CancellationToken cancellationToken = default);
}