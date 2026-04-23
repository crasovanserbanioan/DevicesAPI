using DevicesAPI.Application.DTOs;
using DevicesAPI.Application.Services;
using DevicesAPI.Domain.Entities;
using DevicesAPI.Domain.Enums;
using DevicesAPI.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace DevicesAPI.UnitTests.Services;

public class DeviceServiceTests
{
	private readonly Mock<IDeviceRepository> _repositoryMock;
	private readonly DeviceService _sut;

	public DeviceServiceTests()
	{
		_repositoryMock = new Mock<IDeviceRepository>();
		_sut = new DeviceService(_repositoryMock.Object);
	}

	// ── Helpers ────────────────────────────────────────────────────────────────

	private static Device MakeDevice(DeviceState state = DeviceState.Available) => new()
	{
		Id = Guid.NewGuid(),
		Name = "Original Name",
		Brand = "Original Brand",
		State = state,
		CreationTime = DateTime.UtcNow.AddDays(-1)
	};

	// ── CreateAsync ────────────────────────────────────────────────────────────

	[Fact]
	public async Task CreateAsync_ShouldReturnDevice_WithGeneratedIdAndCreationTime()
	{
		var request = new CreateDeviceRequest { Name = "Phone", Brand = "Acme" };
		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Device>(), default))
			.ReturnsAsync((Device d, CancellationToken _) => d);

		var result = await _sut.CreateAsync(request);

		result.Id.Should().NotBeEmpty();
		result.Name.Should().Be("Phone");
		result.Brand.Should().Be("Acme");
		result.State.Should().Be(DeviceState.Available);
		result.CreationTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
	}

	// ── GetByIdAsync ───────────────────────────────────────────────────────────

	[Fact]
	public async Task GetByIdAsync_ShouldReturnDevice_WhenDeviceExists()
	{
		var device = MakeDevice();
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		var result = await _sut.GetByIdAsync(device.Id);

		result.Should().NotBeNull();
		result!.Id.Should().Be(device.Id);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldReturnNull_WhenDeviceDoesNotExist()
	{
		_repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Device?)null);

		var result = await _sut.GetByIdAsync(Guid.NewGuid());

		result.Should().BeNull();
	}

	// ── UpdateAsync (PUT) ──────────────────────────────────────────────────────

	[Fact]
	public async Task UpdateAsync_ShouldUpdateDevice_WhenDeviceIsAvailable()
	{
		var device = MakeDevice(DeviceState.Available);
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		var request = new UpdateDeviceRequest { Name = "New Name", Brand = "New Brand", State = DeviceState.Inactive };
		var result = await _sut.UpdateAsync(device.Id, request);

		result.Name.Should().Be("New Name");
		result.Brand.Should().Be("New Brand");
		result.State.Should().Be(DeviceState.Inactive);
		_repositoryMock.Verify(r => r.UpdateAsync(device, default), Times.Once);
	}

	[Fact]
	public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenDeviceDoesNotExist()
	{
		_repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Device?)null);

		var act = () => _sut.UpdateAsync(Guid.NewGuid(), new UpdateDeviceRequest { Name = "X", Brand = "Y", State = DeviceState.Available });

		await act.Should().ThrowAsync<KeyNotFoundException>();
	}

	[Fact]
	public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenDeviceIsInUseAndNameChanges()
	{
		var device = MakeDevice(DeviceState.InUse);
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		var request = new UpdateDeviceRequest { Name = "Different Name", Brand = device.Brand, State = DeviceState.InUse };
		var act = () => _sut.UpdateAsync(device.Id, request);

		await act.Should().ThrowAsync<InvalidOperationException>();
	}

	[Fact]
	public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenDeviceIsInUseAndBrandChanges()
	{
		var device = MakeDevice(DeviceState.InUse);
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		var request = new UpdateDeviceRequest { Name = device.Name, Brand = "Different Brand", State = DeviceState.InUse };
		var act = () => _sut.UpdateAsync(device.Id, request);

		await act.Should().ThrowAsync<InvalidOperationException>();
	}

	[Fact]
	public async Task UpdateAsync_ShouldSucceed_WhenDeviceIsInUseButNameAndBrandUnchanged()
	{
		var device = MakeDevice(DeviceState.InUse);
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		var request = new UpdateDeviceRequest { Name = device.Name, Brand = device.Brand, State = DeviceState.InUse };
		var act = () => _sut.UpdateAsync(device.Id, request);

		await act.Should().NotThrowAsync();
	}

	// ── PatchAsync (PATCH) ─────────────────────────────────────────────────────

	[Fact]
	public async Task PatchAsync_ShouldOnlyUpdateProvidedFields()
	{
		var device = MakeDevice(DeviceState.Available);
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		var request = new PatchDeviceRequest { Name = "Patched Name" }; // Brand and State are null
		var result = await _sut.PatchAsync(device.Id, request);

		result.Name.Should().Be("Patched Name");
		result.Brand.Should().Be("Original Brand"); // unchanged
		result.State.Should().Be(DeviceState.Available); // unchanged
	}

	[Fact]
	public async Task PatchAsync_ShouldThrowInvalidOperationException_WhenDeviceIsInUseAndNameChanges()
	{
		var device = MakeDevice(DeviceState.InUse);
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		var request = new PatchDeviceRequest { Name = "Different Name" };
		var act = () => _sut.PatchAsync(device.Id, request);

		await act.Should().ThrowAsync<InvalidOperationException>();
	}

	[Fact]
	public async Task PatchAsync_ShouldThrowInvalidOperationException_WhenDeviceIsInUseAndBrandChanges()
	{
		var device = MakeDevice(DeviceState.InUse);
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		var request = new PatchDeviceRequest { Brand = "Different Brand" };
		var act = () => _sut.PatchAsync(device.Id, request);

		await act.Should().ThrowAsync<InvalidOperationException>();
	}

	[Fact]
	public async Task PatchAsync_ShouldThrowKeyNotFoundException_WhenDeviceDoesNotExist()
	{
		_repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Device?)null);

		var act = () => _sut.PatchAsync(Guid.NewGuid(), new PatchDeviceRequest { Name = "X" });

		await act.Should().ThrowAsync<KeyNotFoundException>();
	}

	// ── DeleteAsync ────────────────────────────────────────────────────────────

	[Fact]
	public async Task DeleteAsync_ShouldDelete_WhenDeviceIsNotInUse()
	{
		var device = MakeDevice(DeviceState.Available);
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		await _sut.DeleteAsync(device.Id);

		_repositoryMock.Verify(r => r.DeleteAsync(device, default), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_ShouldThrowInvalidOperationException_WhenDeviceIsInUse()
	{
		var device = MakeDevice(DeviceState.InUse);
		_repositoryMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

		var act = () => _sut.DeleteAsync(device.Id);

		await act.Should().ThrowAsync<InvalidOperationException>();
	}

	[Fact]
	public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenDeviceDoesNotExist()
	{
		_repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Device?)null);

		var act = () => _sut.DeleteAsync(Guid.NewGuid());

		await act.Should().ThrowAsync<KeyNotFoundException>();
	}
}