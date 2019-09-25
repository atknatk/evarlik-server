using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Lookup;

namespace EVarlik.Dto.Lookup
{
    public class TransactionStateDto
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public Expression<Func<TransactionStateEnum, TransactionStateDto>> FromEntity()
        {
            return l => new TransactionStateDto()
            {
                Name = l.Name,
                Code = l.Code
            };
        }
    }
}