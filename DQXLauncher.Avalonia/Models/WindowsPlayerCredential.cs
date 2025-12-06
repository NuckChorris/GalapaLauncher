using System;
using System.Collections.Immutable;
using System.Linq;
using Windows.Security.Credentials;
using DQXLauncher.Core.Models;

namespace DQXLauncher.Avalonia.Models;

/// <summary>
///     Windows-specific implementation of IPlayerCredential. Uses the Windows Credential Manager, which is not available
///     in Proton. For Proton, we will need an alternative which likely uses the FreeDesktop Secret Service.
/// </summary>
public class WindowsPlayerCredential : IPlayerCredential
{
    private static readonly string TotpResource = "DQXLauncher:TOTP";
    private static readonly string PasswordResource = "DQXLauncher:Password";
    private static readonly PasswordVault Vault = new();

    private PasswordCredential? PasswordCred
    {
        get
        {
            try
            {
                return Vault.Retrieve(PasswordResource, this.Token);
            }
            catch (Exception)
            {
                return null;
            }
        }
        set
        {
            if (this.PasswordCred is not null) Vault.Remove(this.PasswordCred);
            if (value is not null) Vault.Add(value);
        }
    }

    private PasswordCredential? TotpCred
    {
        get
        {
            try
            {
                return Vault.Retrieve(TotpResource, this.Token);
            }
            catch (Exception)
            {
                return null;
            }
        }
        set
        {
            if (this.TotpCred is not null) Vault.Remove(this.TotpCred);
            if (value is not null) Vault.Add(value);
        }
    }

    public string Token { get; init; }
    public string? Password { get; set; }
    public string? TotpKey { get; set; }

    internal static WindowsPlayerCredential LoadInstance(string token)
    {
        var instance = new WindowsPlayerCredential { Token = token };
        instance._Load();
        return instance;
    }

    public void Save()
    {
        this.PasswordCred = this.Password is null
            ? null
            : new PasswordCredential(PasswordResource, this.Token, this.Password);
        this.TotpCred = this.TotpKey is null ? null : new PasswordCredential(TotpResource, this.Token, this.TotpKey);
    }

    public void Remove()
    {
        if (this.PasswordCred is not null) Vault.Remove(this.PasswordCred);
        if (this.TotpCred is not null) Vault.Remove(this.TotpCred);
    }

    internal static ImmutableList<string> GetAllTokensStatic()
    {
        try
        {
            return Vault.FindAllByResource(PasswordResource)
                .Select(cred => cred.UserName).ToImmutableList();
        }
        catch (Exception)
        {
            return ImmutableList<string>.Empty;
        }
    }

    private void _Load()
    {
        this.PasswordCred?.RetrievePassword();
        this.TotpCred?.RetrievePassword();

        this.Password = this.PasswordCred?.Password;
        this.TotpKey = this.TotpCred?.Password;
    }
}