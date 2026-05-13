using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Constants;

namespace sicoain.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion(ApiVersions.Current)]
    internal class BaseApiController : ControllerBase
    {

    }
}
