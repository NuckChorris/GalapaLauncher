using System.Collections.Immutable;
using System.Threading.Tasks;
using DQXLauncher.Avalonia.Models;
using DQXLauncher.Core.Models;
using DQXLauncher.Core.Services;

namespace DQXLauncher.Avalonia.Services;

public class WindowsCredentialManagerFactory : IPlayerCredentialFactory
{
    public Task<IPlayerCredential> LoadAsync(string token)
    {
        return Task.FromResult<IPlayerCredential>(WindowsPlayerCredential.LoadInstance(token));
    }

    public Task<ImmutableList<string>> GetAllTokensAsync()
    {
        return Task.FromResult(WindowsPlayerCredential.GetAllTokensStatic());
    }

    public IPlayerCredential Create(string token)
    {
        return new WindowsPlayerCredential { Token = token };
    }
}