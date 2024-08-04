// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Pyro.Contracts.Requests.Identity;
using Pyro.Contracts.Responses.Identity;

namespace Pyro.ApiTests.Clients;

internal abstract class BaseClient : IDisposable
{
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    private HttpClient? client;
    private User? user;

    protected BaseClient()
    {
    }

    protected BaseClient(Uri baseAddress)
    {
        client = new HttpClient
        {
            BaseAddress = baseAddress,
        };
    }

    ~BaseClient()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            client?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private async Task<HttpResponseMessage?> SendRequest(
        HttpMethod method,
        [StringSyntax("Uri")] string url,
        object? data = null)
    {
        using var request = new HttpRequestMessage(method, url);
        if (data is not null)
            request.Content = JsonContent.Create(data);

        var response = await client!.SendAsync(request);

        return response;
    }

    private async Task<T?> SendRequest<T>(
        HttpMethod method,
        [StringSyntax("Uri")] string url,
        object? data = null)
    {
        using var response = await SendRequest(method, url, data) ??
                             throw new InvalidOperationException();

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<T>();

        return result;
    }

    private User? GetUser(string? accessToken)
    {
        if (accessToken is null)
            return null;

        var parts = accessToken.Split('.');
        if (parts.Length != 3)
            throw new InvalidOperationException("Invalid token");

        // pad a payload with '=' to make it a multiple of 4
        var payload = parts[1];
        var padding = payload.Length % 4;
        if (padding > 0)
            payload += new string('=', 4 - padding);
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        var user = JsonSerializer.Deserialize<User>(json, options) ??
                   throw new InvalidOperationException("Invalid token");

        return user;
    }

    public Task<HttpResponseMessage?> Get([StringSyntax("Uri")] string url)
        => SendRequest(HttpMethod.Get, url);

    public Task<T?> Get<T>([StringSyntax("Uri")] string url)
        => SendRequest<T>(HttpMethod.Get, url);

    public async Task<T?> GetUntil<T>(
        [StringSyntax("Uri")] string url,
        Func<T?, bool> predicate,
        TimeSpan? delay = null,
        TimeSpan? timeout = null)
    {
        delay ??= TimeSpan.FromMilliseconds(200);
        timeout ??= TimeSpan.FromSeconds(30);

        var result = default(T?);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            while (true)
            {
                result = await Get<T>(url);

                if (predicate(result))
                    break;

                if (stopwatch.Elapsed > timeout.Value)
                    throw new TimeoutException();

                await Task.Delay(delay.Value);
            }
        }
        finally
        {
            stopwatch.Stop();
        }

        return result;
    }

    public async Task<string> GetFile([StringSyntax("Uri")] string url)
    {
        var response = await SendRequest(HttpMethod.Get, url);
        if (response is null)
            throw new InvalidOperationException();

        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStringAsync();

        return stream;
    }

    public Task Post([StringSyntax("Uri")] string url, object? data = null)
        => SendRequest(HttpMethod.Post, url, data);

    public Task<T?> Post<T>([StringSyntax("Uri")] string url, object? data = null)
        => SendRequest<T>(HttpMethod.Post, url, data);

    public Task Put([StringSyntax("Uri")] string url, object? data = null)
        => SendRequest(HttpMethod.Put, url, data);

    public Task<T?> Put<T>([StringSyntax("Uri")] string url, object? data = null)
        => SendRequest<T>(HttpMethod.Put, url, data);

    public Task Delete([StringSyntax("Uri")] string url)
        => SendRequest(HttpMethod.Delete, url);

    public async Task Login(string username, string password)
    {
        var loginRequest = new LoginRequest(username, password);
        var tokenPairResponse = await Post<TokenPairResponse>("/api/identity/login", loginRequest);
        var token = tokenPairResponse?.AccessToken;
        client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        user = GetUser(token);
    }

    public Task Login()
        => Login("pyro", "pyro");

    public async Task Logout()
    {
        await Post("/api/identity/logout");
        client!.DefaultRequestHeaders.Authorization = null;
    }

    public T Share<T>()
        where T : BaseClient, new()
    {
        var obj = new T();

        obj.client = client;
        obj.user = user;

        return obj;
    }

    public User? User => user;
}