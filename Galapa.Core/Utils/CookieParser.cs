using System.Globalization;
using System.Net;

namespace Galapa.Core.Utils;

public static class CookieParser
{
    public static Cookie ParseCookie(string header, string domain)
    {
        // Example header: "DQXLogin=1234567890; Path=/; Secure; Expires=Wed, 21 Oct 2023 07:28:00 GMT"
        // Split by ; to get flag segments and the first one is the key=value pair
        var splitFlags = header.Split(';');
        // Split out the key=value pair, that part is done
        var keyValue = splitFlags[0].Split('=');
        // Flags are complicated. We parse them into a dictionary from key=value pairs, and keys without values are true
        var flags = splitFlags
            .Skip(1)
            .Select(part => part.Trim())
            .Where(part => !string.IsNullOrEmpty(part)).Select(part =>
            {
                var flagParts = part.Split('=', 2);
                if (flagParts.Length == 2) return new KeyValuePair<string, object>(flagParts[0], flagParts[1]);

                return new KeyValuePair<string, object>(flagParts[0], true);
            }).ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

        var expiry = ParseCookieExpiry(
            flags.TryGetValue("Max-Age", out var maxAgeObj) ? maxAgeObj as string : null,
            flags.TryGetValue("Expires", out var expiresObj) ? expiresObj as string : null
        );

        var cookie = new Cookie
        {
            Name = keyValue[0],
            Value = keyValue[1],
            Secure = flags.ContainsKey("Secure"),
            HttpOnly = flags.ContainsKey("HttpOnly"),
            Domain = flags.TryGetValue("Domain", out var domainObj) ? domainObj as string : domain,
            Path = flags.TryGetValue("Path", out var pathObj) ? pathObj as string : "/"
        };
        // Manually apply the expiry because... yeah
        if (expiry is not null) cookie.Expires = expiry.Value;

        return cookie;
    }

    private static DateTime? ParseCookieExpiry(string? maxAge, string? expires)
    {
        if (!string.IsNullOrEmpty(maxAge) && int.TryParse(maxAge, out var maxAgeSeconds))
            return DateTime.UtcNow.AddSeconds(maxAgeSeconds);

        return ParseDate(expires);
    }

    private static DateTime ParseDate(string? time)
    {
        try
        {
            if (string.IsNullOrEmpty(time)) return DateTime.MinValue;

            return DateTime.ParseExact(time, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", null, DateTimeStyles.AssumeUniversal);
        }
        catch (Exception)
        {
            return DateTime.MinValue;
        }
    }
}