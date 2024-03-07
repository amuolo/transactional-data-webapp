using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WebAppTests;

public class BasicUnitTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicUnitTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
}
