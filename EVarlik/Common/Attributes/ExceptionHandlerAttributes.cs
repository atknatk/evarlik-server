using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Filters;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using log4net;

namespace EVarlik.Common.Attributes
{
    public class ExceptionHandlerAttributes : ExceptionFilterAttribute
    {
        private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(HttpActionExecutedContext context)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            var exp = context.Exception;
            _log.Error(exp);
            response.Content = new ObjectContent<VarlikResult>(new VarlikResult()
            {
                Status = ResultStatus.UnknownError,
                Message = exp.Message
            }, new JsonMediaTypeFormatter(), "application/json");

            context.Response = response;
            throw new HttpResponseException(response);
        }
    }
}