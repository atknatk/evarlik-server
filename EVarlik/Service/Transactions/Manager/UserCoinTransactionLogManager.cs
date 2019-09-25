using System.Collections.Generic;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Dto.Transactions;
using EVarlik.Hubs;
using EVarlik.Identity;
using EVarlik.Service.Transactions.BusinessLayer;

namespace EVarlik.Service.Transactions.Manager
{
    public class UserCoinTransactionLogManager
    {
        private readonly UserTransactionLogOperation _userCoinTransactionLogOperation;

        public UserCoinTransactionLogManager()
        {
            _userCoinTransactionLogOperation = new UserTransactionLogOperation();
        }

        public VarlikResult Save(UserTransactionLogDto userCoinTransactionLogDto)
        {
            if (userCoinTransactionLogDto == null)
            {
                var result = new VarlikResult();
                result.Status = ResultStatus.MissingRequiredParamater;
                return result;
            }

            var saveResult = _userCoinTransactionLogOperation.Save(userCoinTransactionLogDto);
            if (saveResult.IsSuccess)
            {
                var listR = GetLastTwoTransactionsByIdCoinType(userCoinTransactionLogDto.IdCoinType);
                if (listR.IsSuccess && listR.Data.Count >= 2)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = userCoinTransactionLogDto.IdCoinType;
                    priceDto.CoinUnitPrice = listR.Data[0].CoinUnitPrice;
                    if (listR.Data[0].CoinUnitPrice > listR.Data[1].CoinUnitPrice)
                    {
                        priceDto.IsIncreasing = true;
                    }
                    else
                    {
                        priceDto.IsIncreasing = false;
                    }

                    CoinPricePublisher coinPricePublisher = new CoinPricePublisher();
                    coinPricePublisher.PublishPrice(priceDto);
                }
            }
            return saveResult;
        }

        public void PushPrice(string idCoinType)
        {
            var listR = GetLastTwoTransactionsByIdCoinType(idCoinType);
            if (listR.IsSuccess && listR.Data.Count >= 2)
            {
                PriceDto priceDto = new PriceDto();
                priceDto.IdCoinType = idCoinType;
                priceDto.CoinUnitPrice = listR.Data[0].CoinUnitPrice;
                if (listR.Data[0].CoinUnitPrice > listR.Data[1].CoinUnitPrice)
                {
                    priceDto.IsIncreasing = true;
                }
                else
                {
                    priceDto.IsIncreasing = false;
                }

                CoinPricePublisher coinPricePublisher = new CoinPricePublisher();
                coinPricePublisher.PublishPrice(priceDto);
            }
        }

        public VarlikResult<List<UserTransactionLogDto>> GetAll(int limit, int offset)
        {
            var idUser = IdentityHelper.Instance.CurrentUserId;
            return _userCoinTransactionLogOperation.GetAll(idUser, limit, offset);
        }

        public VarlikResult<List<UserTransactionLogDto>> GetLastTwoTransactionsByIdCoinType(string idCoinType)
        {
            return _userCoinTransactionLogOperation.GetLastTwoTransactionsByIdCoinType(idCoinType);
        }

        public VarlikResult<List<PriceDto>> GetLastPrices()
        {
            return _userCoinTransactionLogOperation.GetLastPrices();
        }

        public VarlikResult<decimal> GetMaxPriceOfCoin(string idCoinType)
        {
            return _userCoinTransactionLogOperation.GetMaxPriceOfCoin(idCoinType);
        }
    }
}