using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using sicoain.api.Services;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class CookieManagerTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpResponse> _responseMock;
        private readonly Mock<HttpRequest> _requestMock;
        private readonly Mock<IResponseCookies> _responseCookiesMock;
        private readonly CookieManager _sut;

        public CookieManagerTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextMock = new Mock<HttpContext>();
            _responseMock = new Mock<HttpResponse>();
            _requestMock = new Mock<HttpRequest>();
            _responseCookiesMock = new Mock<IResponseCookies>();

            _httpContextMock.Setup(x => x.Response).Returns(_responseMock.Object);
            _httpContextMock.Setup(x => x.Request).Returns(_requestMock.Object);
            _responseMock.Setup(x => x.Cookies).Returns(_responseCookiesMock.Object);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

            _sut = new CookieManager(_httpContextAccessorMock.Object);
        }

        [Fact]
        public void SetTokenCookie_ShouldAppendCookieWithHttpOnly()
        {
            _sut.SetTokenCookie("refreshToken", "test-token", 60);

            _responseCookiesMock.Verify(x => x.Append(
                "refreshToken",
                "test-token",
                It.Is<CookieOptions>(o => o.HttpOnly == true)),
                Times.Once);
        }

        [Fact]
        public void SetTokenCookie_ShouldSetExpiration()
        {
            _sut.SetTokenCookie("key", "value", 30);

            _responseCookiesMock.Verify(x => x.Append(
                "key",
                "value",
                It.Is<CookieOptions>(o =>
                    o.Expires.HasValue &&
                    o.Expires.Value > DateTimeOffset.UtcNow)),
                Times.Once);
        }

        [Fact]
        public void GetCookieValue_WhenCookieExists_ReturnsValue()
        {
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(x => x.TryGetValue("my-key", out It.Ref<string?>.IsAny))
                .Returns((string key, out string? value) =>
                {
                    value = "my-value";
                    return true;
                });
            _requestMock.Setup(x => x.Cookies).Returns(cookiesMock.Object);

            var result = _sut.GetCookieValue("my-key");

            result.Should().Be("my-value");
        }

        [Fact]
        public void GetCookieValue_WhenCookieNotExists_ReturnsNull()
        {
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(x => x.TryGetValue("missing", out It.Ref<string?>.IsAny))
                .Returns((string key, out string? value) =>
                {
                    value = null;
                    return false;
                });
            _requestMock.Setup(x => x.Cookies).Returns(cookiesMock.Object);

            var result = _sut.GetCookieValue("missing");

            result.Should().BeNull();
        }

        [Fact]
        public void DeleteCookie_ShouldCallDeleteOnResponse()
        {
            _sut.DeleteCookie("my-key");

            _responseCookiesMock.Verify(x => x.Delete("my-key", It.IsAny<CookieOptions>()), Times.Once);
        }

        [Fact]
        public void GetHttpContext_ShouldReturnHttpContext()
        {
            var result = _sut.GetHttpContext();

            result.Should().Be(_httpContextMock.Object);
        }

        [Fact]
        public void SetTokenCookie_WhenHttpContextNull_DoesNotThrow()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            var act = () => _sut.SetTokenCookie("key", "value", 10);

            act.Should().NotThrow();
        }

        [Fact]
        public void DeleteCookie_WhenHttpContextNull_DoesNotThrow()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            var act = () => _sut.DeleteCookie("key");

            act.Should().NotThrow();
        }
    }
}
