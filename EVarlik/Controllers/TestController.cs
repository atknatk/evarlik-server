using System.Web.Http;

namespace EVarlik.Controllers
{
    public class TestController : VarlikController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok("Ok");
        }
    }
}