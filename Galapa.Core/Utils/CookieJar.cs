using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Galapa.Core.Configuration;

namespace Galapa.Core.Utils;

public class CookieJar : DelegatingHandler
{
    private readonly string _jarFile;
    public CookiesDictionary Cookies = new();

    private CookieJar(HttpMessageHandler innerHandler, string jarName) : base(innerHandler)
    {
        this._jarFile = Path.Combine(JarPath, $"{jarName}.cookies.json");
        Directory.CreateDirectory(JarPath);
    }

    private static string JarPath => Paths.Cache;

    public static async Task<CookieJar> HandlerForJar(HttpMessageHandler innerHandler, string jarName)
    {
        var jar = new CookieJar(innerHandler, jarName);
        await jar.Load();
        return jar;
    }

    private void Cleanup()
    {
        foreach (var key in this.Cookies.Keys.ToList()) this.Cookies[key].RemoveAll(c => c.Expires <= DateTime.UtcNow);
    }

    public async Task Save()
    {
        this.Cleanup();
        await using var jarStream = new FileStream(this._jarFile, FileMode.Create);
        await JsonSerializer.SerializeAsync(jarStream, this.Cookies);
    }

    public async Task Load()
    {
        try
        {
            await using var stream =
                new FileStream(this._jarFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            this.Cookies = await JsonSerializer.DeserializeAsync<CookiesDictionary>(stream) ?? new CookiesDictionary();
        }
        catch (Exception)
        {
            this.Cookies = new CookiesDictionary();
        }

        this.Cleanup();
    }

    public async Task Clear()
    {
        this.Cookies.Clear();
        await this.Save();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        this.Cleanup();
        Debug.Assert(request.RequestUri != null, "request.RequestUri != null");

        // Attach cookies for the current URI
        foreach (var cookie in
                 this.GetCookiesForDomainAndPath(request.RequestUri.Host, request.RequestUri.AbsolutePath))
            request.Headers.Add("Cookie", cookie.ToString());

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // Save any Set-Cookie headers
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookieHeader in cookies) this.SetCookie(cookieHeader, request.RequestUri.Host);

            await this.Save();
        }

        return response;
    }

    private void SetCookie(string header, string domain)
    {
        var cookie = CookieParser.ParseCookie(header, domain);
        var cookies = this.Cookies.GetOrCreate(domain);

        // Remove any existing cookie with the same name
        cookies.RemoveAll(c => c.Name == cookie.Name);
        cookies.Add(cookie);
    }

    private List<Cookie> GetCookiesForDomainAndPath(string domain, string path)
    {
        if (!this.Cookies.TryGetValue(domain, out var cookies)) return [];

        return cookies
            .Where(cookie => path.StartsWith(cookie.Path) && cookie.Expires > DateTime.UtcNow)
            .ToList();
    }

    public class CookiesDictionary : Dictionary<string, List<Cookie>>
    {
        public List<Cookie> GetOrCreate(string key)
        {
            if (!this.TryGetValue(key, out var list))
            {
                list = new List<Cookie>();
                this[key] = list;
            }

            return list;
        }
    }
}