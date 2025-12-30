using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Galapa.Core.Game;

public enum MaintenanceState
{
    Down,
    Up,
    Unknown
}

public class MaintenanceStatus
{
    public async Task<(MaintenanceState, string?)> IsInMaintenance()
    {
        // This doesn't use CookieJar because it doesn't use cookies.
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://launcher.dqx.jp/smgame/gameRequest/mainte/check");
        request.Headers.Add("User-Agent", "Server State Check");
        request.Headers.Add("Cache-Control", "no-cache");
        var response = await httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var res = await response.Content.ReadFromJsonAsync<MaintenanceResponse>();

            if (res?.Status == "0") return (MaintenanceState.Up, res.Message);
            return (MaintenanceState.Down, res?.Message);
        }

        return (MaintenanceState.Unknown, null);
    }

    // This is instantiated via ReadFromJsonAsync
    // ReSharper disable once ClassNeverInstantiated.Local
    private class MaintenanceResponse
    {
        [JsonPropertyName("status")] public required string Status { get; set; }

        [JsonPropertyName("text")] public required string Message { get; set; }
    }
}