using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace EVarlik
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.RemoveAt(1);
            var formatter = config.Formatters[0] as JsonMediaTypeFormatter;
            if (formatter != null)
            {
                formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data"));
                formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain"));
            }
        }
    }
}
