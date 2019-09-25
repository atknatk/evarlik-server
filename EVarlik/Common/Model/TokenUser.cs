using System;

namespace EVarlik.Common.Model
{

    public class BaseTokenUser
    {
        public string Mail { get; set; }
        public string Password { get; set; }
    }

    public class TokenUser : BaseTokenUser
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}