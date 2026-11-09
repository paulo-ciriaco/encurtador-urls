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

    [Fact]
    public async Task GetUrlByIdAsync_ComIdVazio_DeveLancarArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetUrlByIdAsync(Guid.Empty)
        );
    }

    [Fact]
    public async Task GetUrlByIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        var id = Guid.NewGuid();

        _mockContext.Setup(c => c.GetByIdAsync(id))
            .ReturnsAsync((UrlItem?)null);

        var resultado = await _service.GetUrlByIdAsync(id);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task GetUrlByIdAsync_ComIdExistente_DeveRetornarUrlItem()
    {
        var id = Guid.NewGuid();

        var urlEsperada = new UrlItem
        {
            Id = id,
            OriginalUrl = "https://www.google.com",
            ShortCode = "abc123",
            CreatedAt = DateTimeOffset.Now,
            ClickCount = 0
        };

        _mockContext.Setup(c => c.GetByIdAsync(id))
            .ReturnsAsync(urlEsperada);

        var resultado = await _service.GetUrlByIdAsync(id);

        Assert.Same(urlEsperada, resultado);
    }
}