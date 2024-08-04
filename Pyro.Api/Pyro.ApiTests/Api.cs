// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Pyro.ApiTests;

[SetUpFixture]
public class Api
{
    private static IContainer? container;
    private static Uri? baseAddress;

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

        baseAddress = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
            ? new Uri($"http://{container.IpAddress}:{containerPort}")
            : new Uri($"http://localhost:{hostPort}");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        if (container is not null)
        {
            await container.StopAsync();
            await container.DisposeAsync();
        }
    }

    public static Uri BaseAddress => baseAddress!;
}