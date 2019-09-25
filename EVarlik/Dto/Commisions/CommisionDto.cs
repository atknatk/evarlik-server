using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Commissions;

namespace EVarlik.Dto.Commisions
{
    public class CommisionDto
    {
        public long Id { get; set; }
        public string IdCoinType { get; set; }
        public decimal TransactionVolume { get; set; }
        public decimal MakerPercatange { get; set; }
        public decimal TakerPercatange { get; set; }
        public decimal TransferFee { get; set; }
        public decimal TransferFeeCoinCount { get; set; }

        public Expression<Func<Commission, CommisionDto>> FromEntity()
        {
            return l => new CommisionDto()
            {
                Id = l.Id,
                IdCoinType = l.IdCoinType,
                TransactionVolume = l.TransactionVolume,
                MakerPercatange = l.MakerPercatange,
                TakerPercatange = l.TakerPercatange,
                TransferFee = l.TransferFee,
                TransferFeeCoinCount = l.TransferFeeCoinCount,
            };
        }


        public Commission ToEntity(CommisionDto commisionDto)
        {
            return new Commission()
            {
                Id = commisionDto.Id,
                IdCoinType = commisionDto.IdCoinType,
                TransactionVolume = commisionDto.TransactionVolume,
                MakerPercatange = commisionDto.MakerPercatange,
                TakerPercatange = commisionDto.TakerPercatange,
                TransferFee = commisionDto.TransferFee,
                TransferFeeCoinCount = commisionDto.TransferFeeCoinCount,
            };

        }
    }
}