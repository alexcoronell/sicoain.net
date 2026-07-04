using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Business;
using sicoain.shared.Entities;

namespace sicoain.UnitTests.Services;

public class BusinessServiceUpdateIsMainTests
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly BusinessService _service;

    public BusinessServiceUpdateIsMainTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        var expression = new MapperConfigurationExpression();
        expression.CreateMap<Business, BusinessDto>()
            .ForMember(d => d.Emails, opt => opt.Ignore())
            .ForMember(d => d.Phones, opt => opt.Ignore());
        expression.CreateMap<CreateBusinessRequest, Business>()
            .ForMember(d => d.Emails, opt => opt.Ignore())
            .ForMember(d => d.Phones, opt => opt.Ignore());
        expression.CreateMap<UpdateBusinessRequest, Business>()
            .ForMember(d => d.Emails, opt => opt.Ignore())
            .ForMember(d => d.Phones, opt => opt.Ignore());
        var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
        _mapper = config.CreateMapper();

        _service = new BusinessService(_context, _mapper);
    }

    // Add a new email (no Id) → new email created, IsMain normalized
    [Fact]
    public async Task UpdateAsync_WithNewEmailNoId_AddsEmail()
    {
        var business = new Business { Name = "Test Business" };
        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateBusinessRequest
        {
            Emails = new List<UpdateEntityEmailRequest>
            {
                new() { Email = "new@test.com", IsMain = true }
            }
        };

        var result = await _service.UpdateAsync(business.Id, updateRequest);

        result.Should().NotBeNull();
        var emails = _context.Set<BusinessEmail>().ToList();
        emails.Should().ContainSingle();
        emails[0].Email.Should().Be("new@test.com");
        emails[0].IsMain.Should().BeTrue();
    }

    // Update existing email (match by Id) → IsMain updated on existing entity
    [Fact]
    public async Task UpdateAsync_WithExistingEmailId_UpdatesEmail()
    {
        var business = new Business { Name = "Test Business" };
        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        var email = new BusinessEmail
        {
            BusinessId = business.Id,
            Email = "old@test.com",
            IsMain = true,
            Business = null!
        };
        _context.Set<BusinessEmail>().Add(email);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateBusinessRequest
        {
            Emails = new List<UpdateEntityEmailRequest>
            {
                new() { Id = email.Id, Email = "updated@test.com", IsMain = true }
            }
        };

        await _service.UpdateAsync(business.Id, updateRequest);

        var dbEmail = _context.Set<BusinessEmail>().Single();
        dbEmail.Email.Should().Be("updated@test.com");
        dbEmail.IsMain.Should().BeTrue();
    }

    // Remove omitted email → email deleted from DB
    [Fact]
    public async Task UpdateAsync_WithOmittedEmail_RemovesIt()
    {
        var business = new Business { Name = "Test Business" };
        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        var email1 = new BusinessEmail { BusinessId = business.Id, Email = "keep@test.com", IsMain = true, Business = null! };
        var email2 = new BusinessEmail { BusinessId = business.Id, Email = "remove@test.com", IsMain = false, Business = null! };
        _context.Set<BusinessEmail>().AddRange(email1, email2);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateBusinessRequest
        {
            Emails = new List<UpdateEntityEmailRequest>
            {
                new() { Id = email1.Id, Email = "keep@test.com", IsMain = true }
            }
        };

        await _service.UpdateAsync(business.Id, updateRequest);

        var emails = _context.Set<BusinessEmail>().ToList();
        emails.Should().ContainSingle();
        emails[0].Email.Should().Be("keep@test.com");
    }

    // Mixed: update one, add one, remove one → correct sync
    [Fact]
    public async Task UpdateAsync_WithMixedChanges_SyncsCorrectly()
    {
        var business = new Business { Name = "Test Business" };
        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        var email1 = new BusinessEmail { BusinessId = business.Id, Email = "update@test.com", IsMain = true, Business = null! };
        var email2 = new BusinessEmail { BusinessId = business.Id, Email = "remove@test.com", IsMain = false, Business = null! };
        _context.Set<BusinessEmail>().AddRange(email1, email2);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateBusinessRequest
        {
            Emails = new List<UpdateEntityEmailRequest>
            {
                new() { Id = email1.Id, Email = "updated@test.com", IsMain = true },
                new() { Email = "added@test.com", IsMain = false }
            }
        };

        await _service.UpdateAsync(business.Id, updateRequest);

        var emails = _context.Set<BusinessEmail>().ToList();
        emails.Should().HaveCount(2);
        emails.Should().Contain(e => e.Email == "updated@test.com");
        emails.Should().Contain(e => e.Email == "added@test.com");
        emails.Should().NotContain(e => e.Email == "remove@test.com");
        emails.Should().ContainSingle(e => e.IsMain);
    }

    // Change IsMain from email#1 to email#2 → second becomes main, first becomes not
    [Fact]
    public async Task UpdateAsync_WhenIsMainChangesBetweenEmails_SyncsCorrectly()
    {
        var business = new Business { Name = "Test Business" };
        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        var email1 = new BusinessEmail { BusinessId = business.Id, Email = "first@test.com", IsMain = true, Business = null! };
        var email2 = new BusinessEmail { BusinessId = business.Id, Email = "second@test.com", IsMain = false, Business = null! };
        _context.Set<BusinessEmail>().AddRange(email1, email2);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateBusinessRequest
        {
            Emails = new List<UpdateEntityEmailRequest>
            {
                new() { Id = email1.Id, Email = "first@test.com", IsMain = false },
                new() { Id = email2.Id, Email = "second@test.com", IsMain = true }
            }
        };

        await _service.UpdateAsync(business.Id, updateRequest);

        var emails = _context.Set<BusinessEmail>().ToList();
        emails.Should().HaveCount(2);
        emails.Single(e => e.IsMain).Email.Should().Be("second@test.com");
        emails.Single(e => !e.IsMain).Email.Should().Be("first@test.com");
    }
}
