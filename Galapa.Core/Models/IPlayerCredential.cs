using OtpNet;

namespace Galapa.Core.Models;

public interface IPlayerCredential
{
    public string Token { get; init; }
    public string? Password { get; set; }
    public string? TotpKey { get; set; }

    public void Save();
    public void Remove();

    public string ComputeTotp()
    {
        var totp = new Totp(Base32Encoding.ToBytes(this.TotpKey));
        return totp.ComputeTotp();
    }
}