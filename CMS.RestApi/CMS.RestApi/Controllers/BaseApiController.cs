using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace CMS.Api.Controllers
{
    [ApiController]
    [Route("cms/api/[controller]/[action]")]
    public class BaseApiController : ControllerBase
    {
        private readonly IServiceContainer _serviceContainer;

        public BaseApiController(IServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }

    }
}
