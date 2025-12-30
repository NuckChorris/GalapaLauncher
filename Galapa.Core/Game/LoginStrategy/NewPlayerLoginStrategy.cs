using System.Diagnostics.Contracts;
using Galapa.Core.Utils.WebClient;

namespace Galapa.Core.Game.LoginStrategy;

public class NewPlayerLoginStrategy : LoginStrategy, ILoginStepHandler<UsernamePasswordAction>
{
    private Type? _expectedActionType;
    private WebForm? _loginForm;

    public override async Task<LoginStep> Start()
    {
        try
        {
            this._loginForm = await this.GetLoginForm(new Dictionary<string, string>
            {
                { "dqxmode", "1" }
            });
        }
        catch (Exception)
        {
            // @TODO: Log the error
            return new DisplayError("Failed to load login form", new RestartStrategy());
        }

        this._expectedActionType = typeof(UsernamePasswordAction);
        return new AskUsernamePassword();
    }

    public async Task<LoginStep> Step(UsernamePasswordAction action)
    {
        Contract.Assert(this._expectedActionType == typeof(UsernamePasswordAction));
        Contract.Assert(this._loginForm is not null);

        var web = await this.GetWebClient();

        this._loginForm.Fields["sqexid"] = action.Username;
        this._loginForm.Fields["password"] = action.Password;

        var response = await LoginResponse.FromHttpResponse(await web.SendFormAsync(this._loginForm));

        if (response.ErrorMessage is not null)
        {
            this._loginForm = response.Form;
            return new DisplayError(response.ErrorMessage, new AskUsernamePassword(action.Username, action.Password));
        }

        if (response.SessionId is null)
        {
            this._loginForm = response.Form;
            return new DisplayError("Login failed", new AskUsernamePassword(action.Username, action.Password));
        }

        return new LoginCompleted(response.SessionId)
            { Token = response.Token, Username = action.Username, Password = action.Password };
    }
}