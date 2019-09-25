using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Lookup;

namespace EVarlik.Dto.Lookup
{
    public class TransactionTypeDto
    { 
        public string Name { get; set; }
        public string Code { get; set; }

        public Expression<Func<TransactionTypeEnum, TransactionTypeDto>> FromEntity()
        {
            return l => new TransactionTypeDto()
            { 
                Name = l.Name,
                Code = l.Code
            };
        }
    }
}