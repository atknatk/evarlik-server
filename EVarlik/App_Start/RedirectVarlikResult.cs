using System.Web;
using EVarlik.Common.Model;
using Newtonsoft.Json;

namespace EVarlik
{
    public class RedirectVarlikResult
    {
        public static void RedirectWithData(VarlikResult result)
        {
            var data = result != null ? JsonConvert.SerializeObject(result) : string.Empty;
            HttpContext.Current.Response.StatusCode = 401;
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", data.Length.ToString());
            HttpContext.Current.Response.Write(data);
            HttpContext.Current.Response.End();
        }
    }
}