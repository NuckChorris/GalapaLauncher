namespace Galapa.Core.Game.LoginStrategy;

public abstract record LoginStepAction;

public record UsernamePasswordAction(string Username, string Password) : LoginStepAction;

public record PasswordAction(string Password) : LoginStepAction;

public record OtpAction(string Otp) : LoginStepAction;

public record EasyPlayAction : LoginStepAction;