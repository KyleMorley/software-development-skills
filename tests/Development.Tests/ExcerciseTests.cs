using Modules.Creational;

namespace Development.Tests;

public class AlertDispatcherTests
{
    public AlertFactory AlertFactory = new();

    [Fact]
    public void Dispatch_SuccessEmailAlert_ShouldReturnString()
    {
        // Arrange
        var dispatcher = new AlertDispatcher(AlertFactory);
        var alert = new AlertRequest("Email", "ops@example.test", "Database backup completed", false);
        var expectedResult = "EMAIL from=alerts@example.test to=ops@example.test priority=normal: Database backup completed";

        // Act
        var actualResult = dispatcher.Dispatch(alert);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void Dispatch_NullAlert_ShouldThrowArgumentNullException()
    {
        // Arrange
        var dispatcher = new AlertDispatcher(AlertFactory);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => dispatcher.Dispatch(null!));
    }
}