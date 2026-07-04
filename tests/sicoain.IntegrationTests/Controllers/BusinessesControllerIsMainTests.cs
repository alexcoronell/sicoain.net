using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Business;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Controllers;

public partial class BusinessesControllerTests
{
    // 99. CreateBusiness_WithEmailsAndPhones_ReturnsDtosWithIsMain
    [Fact]
    public async Task CreateBusiness_WithEmailsAndPhones_ReturnsDtosWithIsMain()
    {
        var request = new CreateBusinessRequest
        {
            Name = $"TestBusiness_{Guid.NewGuid():N}",
            Emails = new List<CreateEntityEmailRequest>
            {
                new() { Email = "main@business.com", IsMain = true },
                new() { Email = "secondary@business.com", IsMain = false }
            },
            Phones = new List<CreateEntityPhoneRequest>
            {
                new() { Phone = "3001112233", IsMain = true, PhoneType = PhoneType.Mobile },
                new() { Phone = "1234567", IsMain = false, PhoneType = PhoneType.Work }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Businesses", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var business = await response.Content.ReadFromJsonAsync<BusinessDto>();
        business.Should().NotBeNull();
        business!.Emails.Should().HaveCount(2);
        business.Emails.Should().ContainSingle(e => e.IsMain);
        business.Emails.First(e => e.IsMain).Email.Should().Be("main@business.com");
        business.Phones.Should().HaveCount(2);
        business.Phones.Should().ContainSingle(p => p.IsMain);
        business.Phones.First(p => p.IsMain).PhoneNumber.Should().Be("3001112233");
    }
}
