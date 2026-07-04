using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Business;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.UnitTests.Services;

public class BusinessServiceIsMainTests
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly BusinessService _service;

    public BusinessServiceIsMainTests()
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

    // Create with one email, no IsMain explicitly set → email created with IsMain = true
    [Fact]
    public async Task CreateAsync_WithOneEmailAndNoIsMainSet_EmailCreatedWithIsMainTrue()
    {
        var request = new CreateBusinessRequest
        {
            Name = "Test Business",
            Emails = new List<CreateEntityEmailRequest>
            {
                new() { Email = "test@test.com" }
            }
        };

        await _service.CreateAsync(request);

        var emails = _context.Set<BusinessEmail>().ToList();
        emails.Should().ContainSingle();
        emails[0].IsMain.Should().BeTrue();
    }

    // Create with two emails, first marked IsMain → first has IsMain=true, second has IsMain=false
    [Fact]
    public async Task CreateAsync_WithTwoEmailsFirstMarkedIsMain_KeepsFirstAsMain()
    {
        var request = new CreateBusinessRequest
        {
            Name = "Test Business",
            Emails = new List<CreateEntityEmailRequest>
            {
                new() { Email = "main@test.com", IsMain = true },
                new() { Email = "second@test.com", IsMain = false }
            }
        };

        await _service.CreateAsync(request);

        var emails = _context.Set<BusinessEmail>().ToList();
        emails.Should().HaveCount(2);
        emails.Should().ContainSingle(e => e.IsMain);
        emails.First(e => e.IsMain).Email.Should().Be("main@test.com");
    }

    // Create with two emails, none marked → first auto-assigned IsMain=true, second stays false
    [Fact]
    public async Task CreateAsync_WithTwoEmailsNoneMarked_FirstAutoAssignedIsMain()
    {
        var request = new CreateBusinessRequest
        {
            Name = "Test Business",
            Emails = new List<CreateEntityEmailRequest>
            {
                new() { Email = "first@test.com" },
                new() { Email = "second@test.com" }
            }
        };

        await _service.CreateAsync(request);

        var emails = _context.Set<BusinessEmail>().ToList();
        emails.Should().HaveCount(2);
        emails.Should().ContainSingle(e => e.IsMain);
        emails.First(e => e.IsMain).Email.Should().Be("first@test.com");
    }

    // Create with two phones, second marked IsMain, both with PhoneType → phoneType passed through, second has IsMain=true
    [Fact]
    public async Task CreateAsync_WithTwoPhonesSecondMarkedIsMain_PhoneTypePassedThrough()
    {
        var request = new CreateBusinessRequest
        {
            Name = "Test Business",
            Phones = new List<CreateEntityPhoneRequest>
            {
                new() { Phone = "3001112233", PhoneType = PhoneType.Mobile },
                new() { Phone = "1234567", IsMain = true, PhoneType = PhoneType.Work }
            }
        };

        await _service.CreateAsync(request);

        var phones = _context.Set<BusinessPhone>().ToList();
        phones.Should().HaveCount(2);
        phones.Should().ContainSingle(p => p.IsMain);
        var mainPhone = phones.First(p => p.IsMain);
        mainPhone.Phone.Should().Be("1234567");
        mainPhone.PhoneType.Should().Be(PhoneType.Work);
        mainPhone.IsMain.Should().BeTrue();

        var nonMainPhone = phones.First(p => !p.IsMain);
        nonMainPhone.Phone.Should().Be("3001112233");
        nonMainPhone.PhoneType.Should().Be(PhoneType.Mobile);
    }
}
