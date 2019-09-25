using System;
using EVarlik.Common.Model;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;

namespace EVarlik.Authorization
{
    public class JwtManager
    {
        private readonly string secret = "gAYTXDc98mD9U2gG";

        public VarlikResult<string> Encode(TokenUser tokenUser)
        {
            var result = new VarlikResult<string>();

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(tokenUser, secret);
            result.Data = token;


            result.Success();

            return result;
        }

        public VarlikResult<TokenUser> Decode(string token)
        {
            var result = new VarlikResult<TokenUser>();

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.Decode(token, secret, verify: true);
                var res = JsonConvert.DeserializeObject<TokenUser>(json);

                /*   if ((DateTime.Now -res.CreatedAt).Minutes >3 )
                   {
                       result.Status = ResultStatus.TokenTimedOut;
                       return result;
                   }*/

                result.Data = res;
                result.Success();
                return result;
            }
            catch (Exception e)
            {  
            }
            return result;
        }
    }
}