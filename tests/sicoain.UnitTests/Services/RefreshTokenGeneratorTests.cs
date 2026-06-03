using FluentAssertions;
using sicoain.api.Services;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for RefreshTokenGeneratorTests.
    /// </summary>
    public class RefreshTokenGeneratorTests
    {
        private readonly RefreshTokenGenerator _sut = new();

        [Fact]
        public void GenerateToken_ShouldReturnNonEmptyString()
        {
            var token = _sut.GenerateToken();

            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateToken_ShouldReturnBase64With64Bytes()
        {
            var token = _sut.GenerateToken();

            var bytes = Convert.FromBase64String(token);
            bytes.Should().HaveCount(64);
        }

        [Fact]
        public void GenerateToken_ShouldReturnDifferentTokensEachCall()
        {
            var token1 = _sut.GenerateToken();
            var token2 = _sut.GenerateToken();

            token1.Should().NotBe(token2);
        }
    }
}
