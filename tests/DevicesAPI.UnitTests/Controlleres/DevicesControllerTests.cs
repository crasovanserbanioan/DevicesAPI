using DevicesAPI.API.Controllers;
using DevicesAPI.Application.DTOs;
using DevicesAPI.Application.Interfaces;
using DevicesAPI.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DevicesAPI.UnitTests.Controllers;

public class DevicesControllerTests
{
	private readonly Mock<IDeviceService> _serviceMock;
	private readonly DevicesController _sut;

	public DevicesControllerTests()
	{
		_serviceMock = new Mock<IDeviceService>();
		_sut = new DevicesController(_serviceMock.Object);
	}

	private static DeviceResponse MakeResponse(Guid? id = null) => new()
	{
		Id = id ?? Guid.NewGuid(),
		Name = "Test",
		Brand = "Acme",
		State = DeviceState.Available,
		CreationTime = DateTime.UtcNow
	};

	// ── Create ─────────────────────────────────────────────────────────────────

	[Fact]
	public async Task Create_ShouldReturn201_WithLocationHeader()
	{
		var response = MakeResponse();
		_serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateDeviceRequest>(), default)).ReturnsAsync(response);

		var result = await _sut.Create(new CreateDeviceRequest { Name = "Test", Brand = "Acme" }, default);

		var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
		created.StatusCode.Should().Be(201);
		created.Value.Should().Be(response);
		created.ActionName.Should().Be(nameof(DevicesController.GetById));
	}

	// ── GetById ────────────────────────────────────────────────────────────────

	[Fact]
	public async Task GetById_ShouldReturn200_WhenDeviceExists()
	{
		var response = MakeResponse();
		_serviceMock.Setup(s => s.GetByIdAsync(response.Id, default)).ReturnsAsync(response);

		var result = await _sut.GetById(response.Id, default);

		var ok = result.Should().BeOfType<OkObjectResult>().Subject;
		ok.Value.Should().Be(response);
	}

	[Fact]
	public async Task GetById_ShouldReturn404_WhenDeviceDoesNotExist()
	{
		_serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((DeviceResponse?)null);

		var result = await _sut.GetById(Guid.NewGuid(), default);

		result.Should().BeOfType<NotFoundResult>();
	}

	// ── GetAll ─────────────────────────────────────────────────────────────────

	[Fact]
	public async Task GetAll_ShouldReturnAllDevices_WhenNoFilterProvided()
	{
		var devices = new[] { MakeResponse(), MakeResponse() };
		_serviceMock.Setup(s => s.GetAllAsync(default)).ReturnsAsync(devices);

		var result = await _sut.GetAll(null, null, default);

		var ok = result.Should().BeOfType<OkObjectResult>().Subject;
		ok.Value.Should().Be(devices);
	}

	[Fact]
	public async Task GetAll_ShouldFilterByBrand_WhenBrandProvided()
	{
		var devices = new[] { MakeResponse() };
		_serviceMock.Setup(s => s.GetByBrandAsync("Acme", default)).ReturnsAsync(devices);

		var result = await _sut.GetAll("Acme", null, default);

		var ok = result.Should().BeOfType<OkObjectResult>().Subject;
		ok.Value.Should().Be(devices);
		_serviceMock.Verify(s => s.GetByBrandAsync("Acme", default), Times.Once);
		_serviceMock.Verify(s => s.GetAllAsync(default), Times.Never);
	}

	[Fact]
	public async Task GetAll_ShouldFilterByState_WhenStateProvided()
	{
		var devices = new[] { MakeResponse() };
		_serviceMock.Setup(s => s.GetByStateAsync(DeviceState.InUse, default)).ReturnsAsync(devices);

		var result = await _sut.GetAll(null, DeviceState.InUse, default);

		var ok = result.Should().BeOfType<OkObjectResult>().Subject;
		ok.Value.Should().Be(devices);
		_serviceMock.Verify(s => s.GetByStateAsync(DeviceState.InUse, default), Times.Once);
	}

	// ── Delete ─────────────────────────────────────────────────────────────────

	[Fact]
	public async Task Delete_ShouldReturn204_WhenSuccessful()
	{
		var id = Guid.NewGuid();
		_serviceMock.Setup(s => s.DeleteAsync(id, default)).Returns(Task.CompletedTask);

		var result = await _sut.Delete(id, default);

		result.Should().BeOfType<NoContentResult>();
	}
}