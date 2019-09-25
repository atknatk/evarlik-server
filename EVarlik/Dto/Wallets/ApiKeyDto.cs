using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Wallets;

namespace EVarlik.Dto.Wallets
{
    public class ApiKeyDto
    {
        public long Id { get; set; }
        public string IdCoinType { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }

        public Expression<Func<ApiKey, ApiKeyDto>> FromEntity()
        {
            return l => new ApiKeyDto()
            {
                Id = l.Id,
                IdCoinType = l.IdCoinType,
                Key = l.Key,
                Url = l.Url
            };
        }
    }
}