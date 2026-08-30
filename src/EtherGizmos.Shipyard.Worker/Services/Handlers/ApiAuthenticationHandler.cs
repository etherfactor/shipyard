using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Services.Handlers;

internal class ApiAuthenticationHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<ApiOptions> _apiOptions;

    private static AccessToken? _accessToken;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public ApiAuthenticationHandler(
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<ApiOptions> apiOptions)
    {
        _httpClientFactory = httpClientFactory;
        _apiOptions = apiOptions;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri?.AbsoluteUri.StartsWith(_apiOptions.CurrentValue.BaseUrl, StringComparison.OrdinalIgnoreCase) == true
            && request.RequestUri?.AbsoluteUri.Contains("oauth/v2.0", StringComparison.OrdinalIgnoreCase) == false)
        {
            //Add a few minutes so we don't try to use a token right as it expires
            var now = DateTimeOffset.UtcNow.AddMinutes(5);
            if (_accessToken is null || _accessToken.Expires < now)
            {
                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    if (_accessToken is null || _accessToken.Expires < now)
                    {
                        var apiOptions = _apiOptions.CurrentValue;
                        _accessToken = await GenerateAccessTokenAsync(cancellationToken);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            request.Headers.Authorization = new("Bearer", _accessToken.Value);
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _accessToken = null;
        }

        return response;
    }

    private async Task<AccessToken> GenerateAccessTokenAsync(
        CancellationToken cancellationToken = default)
    {
        var apiOptions = _apiOptions.CurrentValue;

        using var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(apiOptions.BaseUrl);

        var clientId = apiOptions.OAuth2.ClientId;
        var clientSecret = apiOptions.OAuth2.ClientSecret;
        var content = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = apiOptions.OAuth2.ClientId,
            ["client_secret"] = apiOptions.OAuth2.ClientSecret,
            ["scope"] = string.Join(" ",
            [
                "carrier.read",
                "package.write",
                "tracking-update.read",
            ]),
        });

        var response = await client.PostAsync("/oauth/v2.0/token", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(
            JsonSerializerOptions.App,
            cancellationToken: cancellationToken);

        if (tokenResponse?.AccessToken is null) throw new InvalidOperationException();

        return new(
            tokenResponse.AccessToken,
            DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn));
    }

    private record AccessToken(
        string Value,
        DateTimeOffset Expires);

    private record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("token_type")] string TokenType,
        [property: JsonPropertyName("expires_in")] int ExpiresIn,
        [property: JsonPropertyName("refresh_token")] string? RefreshToken,
        [property: JsonPropertyName("scope")] string Scope);
}
