using System;
using DQXLauncher.Core.Game.LoginStrategy;

namespace DQXLauncher.Avalonia.Services;

public class LoginNavigationService
{
    public enum StepChangeDirection
    {
        // Progressing to the next step
        Forward,

        // Going back to the previous step
        Backward,

        // No transition
        None
    }

    public LoginStep? Step { get; private set; }

    public event EventHandler<StepChange>? StepChanged;

    public void Forward(LoginStep step)
    {
        this.ChangeStep(step);
    }

    public void Backward(LoginStep step)
    {
        this.ChangeStep(step, StepChangeDirection.Backward);
    }

    private void ChangeStep(LoginStep step, StepChangeDirection direction = StepChangeDirection.Forward)
    {
        this.Step = step;
        var change = new StepChange { Step = step, Direction = direction };
        this.StepChanged?.Invoke(this, change);
    }

    public class StepChange
    {
        public required LoginStep Step { get; init; }
        public StepChangeDirection Direction { get; init; } = StepChangeDirection.Forward;
    }
}