using System.Collections.Immutable;
using Galapa.Core.Models;

namespace Galapa.Core.Game;

public interface IPlayerCredentialFactory
{
    /// <summary>
    ///     Load a credential from the credential store by token
    /// </summary>
    Task<IPlayerCredential> LoadAsync(string token);

    /// <summary>
    ///     Get all credential tokens currently stored in the credential store
    /// </summary>
    Task<ImmutableList<string>> GetAllTokensAsync();

    /// <summary>
    ///     Create a new empty in-memory credential for a token (does not persist to store)
    /// </summary>
    IPlayerCredential Create(string token);
}