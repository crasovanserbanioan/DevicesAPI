using DevicesAPI.Domain.Entities;
using DevicesAPI.Domain.Enums;
using DevicesAPI.Domain.Interfaces;
using DevicesAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevicesAPI.Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
	private readonly DevicesDbContext _context;

	public DeviceRepository(DevicesDbContext context)
	{
		_context = context;
	}

	public async Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _context.Devices
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
	}

	public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Devices
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<Device>> GetByBrandAsync(string brand, CancellationToken cancellationToken = default)
	{
		return await _context.Devices
			.AsNoTracking()
			.Where(d => d.Brand.ToLower() == brand.ToLower())
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<Device>> GetByStateAsync(DeviceState state, CancellationToken cancellationToken = default)
	{
		return await _context.Devices
			.AsNoTracking()
			.Where(d => d.State == state)
			.ToListAsync(cancellationToken);
	}

	public async Task<Device> AddAsync(Device device, CancellationToken cancellationToken = default)
	{
		await _context.Devices.AddAsync(device, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return device;
	}

	public async Task UpdateAsync(Device device, CancellationToken cancellationToken = default)
	{
		_context.Devices.Update(device);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(Device device, CancellationToken cancellationToken = default)
	{
		_context.Devices.Remove(device);
		await _context.SaveChangesAsync(cancellationToken);
	}
}