// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Pyro.ApiTests.Clients;

namespace Pyro.ApiTests;

[SetUpFixture]
internal class Api
{
    private static Smtp? smtp;
    private static IContainer? container;
    private static Uri? baseAddress;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        smtp = new Smtp();
        smtp.Start();

        const int hostPort = 8080;
        const int containerPort = 80;
        var imageId = Environment.GetEnvironmentVariable("PYRO_IMAGE_ID") ??
                      "pyro";
        var isInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        var containerBuilder = new ContainerBuilder()
            .WithImage(imageId)
            .WithName("pyro")
            .WithPortBinding(hostPort, containerPort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
            .WithEnvironment(new Dictionary<string, string>
            {
                ["EmailService__Provider"] = "Smtp",
                ["EmailService__Login"] = "test",
                ["EmailService__Password"] = "1234",
            });

        if (isInContainer)
        {
            var addresses = (await Dns.GetHostAddressesAsync(Dns.GetHostName()))
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

            containerBuilder = containerBuilder
                .WithEnvironment("EmailService__Host", $"smtp://{addresses}:25");
        }
        else
        {
            containerBuilder = containerBuilder
                .WithExtraHost("host.docker.internal", "host-gateway")
                .WithEnvironment("EmailService__Host", "smtp://host.docker.internal:25");
        }

        container = containerBuilder.Build();

        await container.StartAsync();

        baseAddress = isInContainer
            ? new Uri($"http://{container.IpAddress}:{containerPort}")
            : new Uri($"http://localhost:{hostPort}");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        smtp?.Stop();

        if (container is not null)
        {
            await container.StopAsync();
            await container.DisposeAsync();
        }
    }

    public static Uri BaseAddress => baseAddress!;

    public static Smtp Smtp => smtp!;
}