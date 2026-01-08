using AirlineBookingSystem.Fights.Application.Commands;
using AirlineBookingSystem.Fights.Application.Handlers;
using AirlineBookingSystem.Fights.Core.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Flights.Application.Tests.Handlers;

public class DeleteFlightHandlerTests
{
    private readonly Mock<IFlightRepository> _repositoryMock = new();
    private readonly DeleteFlightHandler _handler;

    public DeleteFlightHandlerTests()
    {
        _handler = new DeleteFlightHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldInvokeRepositoryDelete()
    {
        // Arrange
        var flightId = Guid.NewGuid();
        var command = new DeleteFlightCommand(flightId);

        _repositoryMock
            .Setup(r => r.DeleteFlightAsync(flightId))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.DeleteFlightAsync(flightId), Times.Once);
    }
}
