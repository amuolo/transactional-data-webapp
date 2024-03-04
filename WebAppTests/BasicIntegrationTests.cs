using Microsoft.AspNetCore.Mvc.Testing;
using Posting;
using Xunit;

namespace WebAppTests;

public class BasicIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/Home")]
    [InlineData("/Home/Privacy")]
    [InlineData("/Home/Audit")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Contract.Url + url);

        response.EnsureSuccessStatusCode(); 

        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }
}