using Xunit;
using Moq;
using EncurtadorURLs.Services;
using EncurtadorURLs.Data;
using EncurtadorURLs.Models;

namespace EncurtadorURLs.Tests.Services;

public class UrlsServiceTests
{
    private readonly Mock<IUrlContext> _mockContext;
    private readonly UrlsService _service;

    public UrlsServiceTests()
    {
        _mockContext = new Mock<IUrlContext>();
        _service = new UrlsService(_mockContext.Object);
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComUrlValida_DeveRetornarUrlItemComShortCode()
    {
        var urlOriginal = "https://www.google.com";

        _mockContext.Setup(c => c.ShortCodeExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var resultado = await _service.CreateShortUrlAsync(urlOriginal);

        Assert.NotNull(resultado);
        Assert.Equal(urlOriginal, resultado.OriginalUrl);
        Assert.False(string.IsNullOrEmpty(resultado.ShortCode));

        _mockContext.Verify(
            c => c.AddAsync(It.IsAny<UrlItem>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComUrlVazia_DeveLancarArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateShortUrlAsync(string.Empty)
        );
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComEsquemaInvalido_DeveLancarArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateShortUrlAsync("ftp://arquivo-invalido.com")
        );
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComUrlNula_DeveLancarArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateShortUrlAsync(null!)
        );
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComUrlMuitoLonga_DeveLancarArgumentException()
    {
        var urlLonga = "https://exemplo.com/" + new string('a', 2100);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateShortUrlAsync(urlLonga)
        );
    }
}