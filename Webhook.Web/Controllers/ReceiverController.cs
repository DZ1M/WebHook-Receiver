
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Text.Json;

namespace Webhook.Web.Controllers
{
    public class ReceiverController : Controller
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _accessor;

        public ReceiverController(IHostEnvironment hostEnvironment, IHttpContextAccessor accessor)
        {
            _hostEnvironment = hostEnvironment;
            _accessor = accessor;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            return Process(null);
        }

        [HttpPost]
        [HttpPut]
        [HttpPatch]
        [HttpDelete]
        [Route("")]
        public IActionResult RequestObj(dynamic content)
        {
            return Process(content);
        }

        [HttpPost]
        [HttpPut]
        [HttpPatch]
        [HttpDelete]
        [Route("body")]
        public IActionResult RequestBody([FromBody] dynamic content)
        {
            return Process(content);
        }

        [HttpPost]
        [HttpPut]
        [HttpPatch]
        [HttpDelete]
        [Route("form")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult RequestForm([FromForm] IFormCollection formCollection)
        {
            dynamic content = new ExpandoObject();
            foreach (var item in formCollection)
            {
                ((IDictionary<string, object>)content)[item.Key] = item.Value.FirstOrDefault();
            }
            var jsonContent = JsonSerializer.Serialize(content);
            return Process(jsonContent);
        }


        private IActionResult Process(dynamic content)
        {
            try
            {
                var clientIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

                var message = new
                {
                    Headers = Request.Headers,
                    QueryString = Request.Query,
                    HttpMethod = Request.Method,
                    RawContent = content?.ToString(),
                    ClientIP = clientIp
                };

                return Ok(JsonSerializer.Serialize(message));
            }
            catch
            {
                return BadRequest(new { content });
            }
        }

    }
}
