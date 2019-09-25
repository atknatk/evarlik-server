using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Transactions;

namespace EVarlik.Dto.Transactions
{
    public class TransactionInformationLogDto
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public string TxHash { get; set; }
        public string TxReceiptStatus { get; set; }
        public string BlockHeight { get; set; }
        public DateTime TimeStamp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public string CoinType { get; set; }

        public Expression<Func<TransactionInformationLog, TransactionInformationLogDto>> FromEntity()
        {
            return l => new TransactionInformationLogDto()
            {
                Id = l.Id,
                TxHash = l.TxHash,
                TxReceiptStatus = l.TxReceiptStatus,
                BlockHeight = l.BlockHeight,
                TimeStamp = l.TimeStamp,
                From = l.From,
                To = l.To,
                Amount = l.Amount,
                CoinType = l.CoinTypeEnum.Code
            };
        }

        public TransactionInformationLog ToEntity(TransactionInformationLogDto transactionInformationLogDto)
        {
            return new TransactionInformationLog()
            {
                Id = transactionInformationLogDto.Id,
                IdUser = transactionInformationLogDto.IdUser,
                TxHash = transactionInformationLogDto.TxHash,
                TxReceiptStatus = transactionInformationLogDto.TxReceiptStatus,
                BlockHeight = transactionInformationLogDto.BlockHeight,
                TimeStamp = transactionInformationLogDto.TimeStamp,
                From = transactionInformationLogDto.From,
                To = transactionInformationLogDto.To,
                Amount = transactionInformationLogDto.Amount,
                IdCoinType = transactionInformationLogDto.CoinType
            };
        }
    }
}