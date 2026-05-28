using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using sicoain.api.Services;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class IpAddressProviderTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpRequest> _requestMock;
        private readonly IpAddressProvider _sut;

        public IpAddressProviderTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextMock = new Mock<HttpContext>();
            _requestMock = new Mock<HttpRequest>();

            _httpContextMock.Setup(x => x.Request).Returns(_requestMock.Object);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

            _sut = new IpAddressProvider(_httpContextAccessorMock.Object);
        }

        [Fact]
        public void GetCurrentIpAddress_WhenXForwardedForPresent_ReturnsFirstIp()
        {
            var headers = new HeaderDictionary
            {
                { "X-Forwarded-For", "192.168.1.1, 10.0.0.1" }
            };
            _requestMock.Setup(x => x.Headers).Returns(headers);

            var result = _sut.GetCurrentIpAddress();

            result.Should().Be("192.168.1.1");
        }

        [Fact]
        public void GetCurrentIpAddress_WhenNoForwardedFor_UsesRemoteIpAddress()
        {
            var headers = new HeaderDictionary();
            _requestMock.Setup(x => x.Headers).Returns(headers);

            var connectionMock = new Mock<ConnectionInfo>();
            connectionMock.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Parse("10.0.0.5"));
            _httpContextMock.Setup(x => x.Connection).Returns(connectionMock.Object);

            var result = _sut.GetCurrentIpAddress();

            result.Should().Be("10.0.0.5");
        }

        [Fact]
        public void GetCurrentIpAddress_WhenRemoteIpAddressNull_ReturnsUnknown()
        {
            var headers = new HeaderDictionary();
            _requestMock.Setup(x => x.Headers).Returns(headers);

            var connectionMock = new Mock<ConnectionInfo>();
            connectionMock.Setup(x => x.RemoteIpAddress).Returns((IPAddress?)null);
            _httpContextMock.Setup(x => x.Connection).Returns(connectionMock.Object);

            var result = _sut.GetCurrentIpAddress();

            result.Should().Be("unknown");
        }

        [Fact]
        public void GetCurrentIpAddress_WhenHttpContextNull_ReturnsUnknown()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            var result = _sut.GetCurrentIpAddress();

            result.Should().Be("unknown");
        }
    }
}
