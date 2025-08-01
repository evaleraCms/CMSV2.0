using CMS.RestApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace CMS.Api.Controllers
{
    [ApiController]
    [Route("cms/api/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class BaseApiController<T> : ControllerBase where T : class
    {
        private readonly IServiceContainer _serviceContainer;
        private readonly ILogger<T> _logger;

        public BaseApiController(IServiceContainer serviceContainer, ILogger<T> logger)
        {
            _serviceContainer = serviceContainer;
            _logger = logger;
        }

    }
}
