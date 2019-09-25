using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Lookup; 

namespace EVarlik.Dto.Lookup
{
    public class CoinTypeDto
    { 
        public string Name { get; set; }
        public string Code { get; set; }

        public  Expression<Func<CoinTypeEnum, CoinTypeDto>> FromEntity()
        {
            return l => new CoinTypeDto()
            { 
                Name = l.Name,
                Code = l.Code
            };
        }

    }
}