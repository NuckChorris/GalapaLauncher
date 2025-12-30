using Galapa.Core.Models;

namespace Galapa.Core.Game.Authentication;

public class AutoLoginStrategy(SavedPlayerLoginStrategy strategy, IPlayerCredential credential)
    : LoginStrategy, ILoginStepHandler<PasswordAction>
{
    public override async Task<LoginStep> Start()
    {
        return await this.AutoStep(await strategy.Start());
    }

    public async Task<LoginStep> Step(PasswordAction action)
    {
        return await this.AutoStep(await strategy.Step(action));
    }

    private LoginStepAction? ActionForStep(LoginStep step)
    {
        if (step is AskPassword && credential.Password is not null) return new PasswordAction(credential.Password);

        if (step is AskOtp && credential.TotpKey is not null) return new OtpAction(credential.TotpKey);

        return null;
    }

    private async Task<LoginStep> AutoStep(LoginStep step)
    {
        // Loop auto-stepping until we hit null
        var action = this.ActionForStep(step);
        while (action is not null)
        {
            step = await base.Step(action);
            action = this.ActionForStep(step);
        }

        return step;
    }
}