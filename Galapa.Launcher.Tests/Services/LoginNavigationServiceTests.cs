using Galapa.Launcher.Services;

namespace Galapa.Launcher.Tests.Services;

public class LoginNavigationServiceTests
{
    [Fact]
    public void Forward_UpdatesStepAndFiresEvent()
    {
        // Arrange
        var service = new LoginNavigationService();
        var expectedStep = new AskUsernamePassword();
        LoginNavigationService.StepChange? receivedChange = null;

        service.StepChanged += (sender, change) => receivedChange = change;

        // Act
        service.Forward(expectedStep);

        // Assert
        Assert.NotNull(receivedChange);
        Assert.Equal(expectedStep, receivedChange.Step);
        Assert.Equal(LoginNavigationService.StepChangeDirection.Forward, receivedChange.Direction);
        Assert.Equal(expectedStep, service.Step);
    }

    [Fact]
    public void Backward_UpdatesStepAndFiresEvent_WithBackwardDirection()
    {
        // Arrange
        var service = new LoginNavigationService();
        var expectedStep = new AskPassword("testuser");
        LoginNavigationService.StepChange? receivedChange = null;

        service.StepChanged += (sender, change) => receivedChange = change;

        // Act
        service.Backward(expectedStep);

        // Assert
        Assert.NotNull(receivedChange);
        Assert.Equal(expectedStep, receivedChange.Step);
        Assert.Equal(LoginNavigationService.StepChangeDirection.Backward, receivedChange.Direction);
        Assert.Equal(expectedStep, service.Step);
    }

    [Fact]
    public void StepChanged_PassesCorrectStepData()
    {
        // Arrange
        var service = new LoginNavigationService();
        var step1 = new AskUsernamePassword("user1", "pass1");
        var step2 = new AskOtp("user2", "123456");
        var changes = new List<LoginNavigationService.StepChange>();

        service.StepChanged += (sender, change) => changes.Add(change);

        // Act
        service.Forward(step1);
        service.Forward(step2);

        // Assert
        Assert.Equal(2, changes.Count);

        Assert.Equal(step1, changes[0].Step);
        Assert.Equal(LoginNavigationService.StepChangeDirection.Forward, changes[0].Direction);

        Assert.Equal(step2, changes[1].Step);
        Assert.Equal(LoginNavigationService.StepChangeDirection.Forward, changes[1].Direction);
    }

    [Fact]
    public void MultipleSubscribers_AllReceiveEvents()
    {
        // Arrange
        var service = new LoginNavigationService();
        var expectedStep = new LoginCompleted("session123");

        LoginNavigationService.StepChange? subscriber1Change = null;
        LoginNavigationService.StepChange? subscriber2Change = null;
        LoginNavigationService.StepChange? subscriber3Change = null;

        service.StepChanged += (sender, change) => subscriber1Change = change;
        service.StepChanged += (sender, change) => subscriber2Change = change;
        service.StepChanged += (sender, change) => subscriber3Change = change;

        // Act
        service.Forward(expectedStep);

        // Assert
        Assert.NotNull(subscriber1Change);
        Assert.NotNull(subscriber2Change);
        Assert.NotNull(subscriber3Change);

        Assert.Equal(expectedStep, subscriber1Change.Step);
        Assert.Equal(expectedStep, subscriber2Change.Step);
        Assert.Equal(expectedStep, subscriber3Change.Step);
    }

    [Fact]
    public void Step_IsNullInitially()
    {
        // Arrange & Act
        var service = new LoginNavigationService();

        // Assert
        Assert.Null(service.Step);
    }

    [Fact]
    public void Forward_WithMultipleSteps_UpdatesStepCorrectly()
    {
        // Arrange
        var service = new LoginNavigationService();
        var step1 = new AskUsernamePassword();
        var step2 = new AskOtp("user");
        var step3 = new LoginCompleted("session");

        // Act & Assert
        service.Forward(step1);
        Assert.Equal(step1, service.Step);

        service.Forward(step2);
        Assert.Equal(step2, service.Step);

        service.Forward(step3);
        Assert.Equal(step3, service.Step);
    }

    [Fact]
    public void Backward_WithMultipleSteps_UpdatesStepCorrectly()
    {
        // Arrange
        var service = new LoginNavigationService();
        var step1 = new AskOtp("user");
        var step2 = new AskPassword("user");
        var step3 = new AskUsernamePassword();

        // Act & Assert
        service.Backward(step1);
        Assert.Equal(step1, service.Step);

        service.Backward(step2);
        Assert.Equal(step2, service.Step);

        service.Backward(step3);
        Assert.Equal(step3, service.Step);
    }

    [Fact]
    public void StepChanged_NotFiredWhenNoSubscribers()
    {
        // Arrange
        var service = new LoginNavigationService();
        var step = new AskUsernamePassword();

        // Act & Assert (should not throw)
        service.Forward(step);
        service.Backward(step);
    }

    [Fact]
    public void MixedForwardBackward_MaintainsCorrectDirection()
    {
        // Arrange
        var service = new LoginNavigationService();
        var changes = new List<LoginNavigationService.StepChange>();
        service.StepChanged += (sender, change) => changes.Add(change);

        var step1 = new AskUsernamePassword();
        var step2 = new AskOtp("user");
        var step3 = new AskPassword("user");

        // Act
        service.Forward(step1);
        service.Forward(step2);
        service.Backward(step3);

        // Assert
        Assert.Equal(3, changes.Count);
        Assert.Equal(LoginNavigationService.StepChangeDirection.Forward, changes[0].Direction);
        Assert.Equal(LoginNavigationService.StepChangeDirection.Forward, changes[1].Direction);
        Assert.Equal(LoginNavigationService.StepChangeDirection.Backward, changes[2].Direction);
    }
}