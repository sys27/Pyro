// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Pyro.Contracts.Requests.Identity;
using Pyro.Contracts.Responses.Identity;

namespace Pyro.ApiTests;

[SetUpFixture]
public class Api
{
    private static IContainer? container;
    private static HttpClient? client;
    private static User? user;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        const int hostPort = 8080;
        const int containerPort = 80;
        var imageId = Environment.GetEnvironmentVariable("PYRO_IMAGE_ID") ??
                      "pyro";

        container = new ContainerBuilder()
            .WithImage(imageId)
            .WithName("pyro")
            .WithPortBinding(hostPort, containerPort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(containerPort))
            .Build();

        await container.StartAsync();

        var baseAddress = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
            ? new Uri($"http://{container.IpAddress}:{containerPort}")
            : new Uri($"http://localhost:{hostPort}");

        client = new HttpClient
        {
            BaseAddress = baseAddress,
        };

        var loginRequest = new LoginRequest("pyro", "pyro");
        var tokenPairResponse = await Post<TokenPairResponse>("/api/identity/login", loginRequest);
        var token = tokenPairResponse?.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        user = GetUser(token);
    }

    private static User? GetUser(string? accessToken)
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
        var user = JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ??
                   throw new InvalidOperationException("Invalid token");

        return user;
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await Post("/api/identity/logout");

        user = null;
        client?.Dispose();

        if (container is not null)
        {
            await container.StopAsync();
            await container.DisposeAsync();
        }
    }

    private static async Task<HttpResponseMessage?> SendRequest(
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

    private static async Task<T?> SendRequest<T>(
        HttpMethod method,
        [StringSyntax("Uri")] string url,
        object? data = null)
    {
        using var response = await SendRequest(method, url, data) ??
                             throw new InvalidOperationException();
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<T>();

        return result;
    }

    public static Task<HttpResponseMessage?> Get([StringSyntax("Uri")] string url)
        => SendRequest(HttpMethod.Get, url);

    public static Task<T?> Get<T>([StringSyntax("Uri")] string url)
        => SendRequest<T>(HttpMethod.Get, url);

    public static async Task<T?> GetUntil<T>(
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

    public static async Task<string> GetFile([StringSyntax("Uri")] string url)
    {
        var response = await SendRequest(HttpMethod.Get, url);
        if (response is null)
            throw new InvalidOperationException();

        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStringAsync();

        return stream;
    }

    public static Task Post([StringSyntax("Uri")] string url, object? data = null)
        => SendRequest(HttpMethod.Post, url, data);

    public static Task<T?> Post<T>([StringSyntax("Uri")] string url, object? data = null)
        => SendRequest<T>(HttpMethod.Post, url, data);

    public static Task Put([StringSyntax("Uri")] string url, object? data = null)
        => SendRequest(HttpMethod.Put, url, data);

    public static Task<T?> Put<T>([StringSyntax("Uri")] string url, object? data = null)
        => SendRequest<T>(HttpMethod.Put, url, data);

    public static Task Delete([StringSyntax("Uri")] string url)
        => SendRequest(HttpMethod.Delete, url);

    public static User? User => user;
}