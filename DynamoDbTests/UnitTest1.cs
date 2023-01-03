using System;
using System.Threading.Tasks;
using Alba;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;
using static API.Controllers.ValuesController;

namespace DynamoDbTests;

public class UnitTest1 : IAsyncLifetime
{
    private readonly TestcontainersContainer _localStackContainer;

    public UnitTest1()
    {
        var localStackBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("localstack/localstack")
                .WithCleanUp(true)
                .WithEnvironment("DEFAULT_REGION", "eu-central-1")
                .WithEnvironment("SERVICES", "dynamodb")
                .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                .WithEnvironment("DEBUG", "1")
                .WithPortBinding(4566, 4566);

        _localStackContainer = localStackBuilder.Build();
    }

    [Fact]
    public async Task Test1()
    {
        await using var host = await AlbaHost.For<Program>();
        await host.Scenario(_ =>
        {
            _.Get.Url("/api/Values/Init");
            _.StatusCodeShouldBeOk();
        });
    }

    [Fact]
    public async Task Test2()
    {
        await using var host = await AlbaHost.For<Program>();
        await host.Scenario(_ =>
       {
           _.Get.Url("/api/Values/Init");
           _.StatusCodeShouldBeOk();
       });

        await host.Scenario(_ =>
        {
            _.Post.Json(new PostInput { Id = 1, Title = "Test1" }).ToUrl("/api/Values");
            _.StatusCodeShouldBeOk();
        });
    }

    public async Task InitializeAsync()
    {
        await _localStackContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _localStackContainer.DisposeAsync();
    }
}