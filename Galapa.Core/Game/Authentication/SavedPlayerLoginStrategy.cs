using System.Diagnostics.Contracts;
using Galapa.Core.Web;

namespace Galapa.Core.Game.Authentication;

public class SavedPlayerLoginStrategy(string token) : LoginStrategy, ILoginStepHandler<PasswordAction>
{
    private Type? _expectedActionType;
    private WebForm? _loginForm;
    private string? _username;

    public override async Task<LoginStep> Start()
    {
        try
        {
            this._loginForm = await this.GetLoginForm(new Dictionary<string, string>
            {
                { "dqxmode", "2" },
                { "id", token }
            });
        }
        catch (Exception)
        {
            // @TODO: Log the error
            return new DisplayError("Failed to load login form", new RestartStrategy());
        }

        this._username = this._loginForm.Fields["sqexid"];

        this._expectedActionType = typeof(PasswordAction);
        return new AskPassword(this._username);
    }

    public virtual async Task<LoginStep> Step(PasswordAction action)
    {
        Contract.Assert(this._expectedActionType == typeof(PasswordAction));
        Contract.Assert(this._loginForm is not null);

        var web = await this.GetWebClient();

        this._loginForm.Fields["password"] = action.Password;

        var response = await LoginResponse.FromHttpResponse(await web.SendFormAsync(this._loginForm));

        if (response.ErrorMessage is not null && response.Form is not null)
        {
            this._loginForm = response.Form;
            return new DisplayError(response.ErrorMessage, new AskPassword(this._loginForm.Fields["sqexid"],
                action.Password));
        }

        if (response.SessionId is null && response.Form is not null)
        {
            this._loginForm = response.Form;
            return new DisplayError("Login failed", new AskPassword(this._loginForm.Fields["sqexid"],
                action.Password));
        }

        if (response.SessionId is null) return new DisplayError("Login failed", new RestartStrategy());

        return new LoginCompleted(response.SessionId)
        {
            Username = this._username,
            Password = action.Password
        };
    }
}