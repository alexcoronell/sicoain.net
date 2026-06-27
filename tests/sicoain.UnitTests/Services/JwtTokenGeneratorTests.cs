using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using sicoain.api.Services;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for JwtTokenGeneratorTests.
    /// </summary>
    public class JwtTokenGeneratorTests
    {
        private readonly JwtTokenGenerator _sut;

        public JwtTokenGeneratorTests()
        {
            var jwtSettingsMock = new Mock<IConfigurationSection>();
            jwtSettingsMock.Setup(x => x["SecretKey"]).Returns("ThisIsASecretKeyForTestingPurposes!@#$%");
            jwtSettingsMock.Setup(x => x["ExpirationMinutes"]).Returns("15");
            jwtSettingsMock.Setup(x => x["Issuer"]).Returns("TestIssuer");
            jwtSettingsMock.Setup(x => x["Audience"]).Returns("TestAudience");

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSettingsMock.Object);

            _sut = new JwtTokenGenerator(configMock.Object);
        }

        [Fact]
        public void GenerateToken_ShouldReturnValidJwt()
        {
            var user = new User { Id = 1, Email = "test@test.com", FullName = "Test User" };

            var token = _sut.GenerateToken(user);

            token.Should().NotBeNullOrEmpty();
            token.Split('.').Should().HaveCount(3);
        }

        [Fact]
        public void GenerateToken_ShouldContainUserClaims()
        {
            var user = new User { Id = 42, Email = "user@example.com", FullName = "John Doe" };

            var token = _sut.GenerateToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Subject.Should().Be("42");
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "user@example.com");
            jwt.Claims.Should().Contain(c => c.Type == "fullName" && c.Value == "John Doe");
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        }

        [Fact]
        public void GenerateToken_ShouldSetExpiration()
        {
            var user = new User { Id = 1, Email = "a@b.com", FullName = "Test" };

            var token = _sut.GenerateToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void GenerateToken_ShouldIncludeAdditionalClaims()
        {
            var user = new User { Id = 1, Email = "a@b.com", FullName = "Test" };
            var additionalClaims = new List<Claim>
            {
                new("custom-claim", "custom-value")
            };

            var token = _sut.GenerateToken(user, additionalClaims);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == "custom-claim" && c.Value == "custom-value");
        }

        [Fact]
        public void GenerateToken_ShouldSignWithHmacSha256()
        {
            var user = new User { Id = 1, Email = "a@b.com", FullName = "Test" };

            var token = _sut.GenerateToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // Verify the signature algorithm
            var header = jwt.Header;
            header.Alg.Should().Be(SecurityAlgorithms.HmacSha256);
        }
    }
}
