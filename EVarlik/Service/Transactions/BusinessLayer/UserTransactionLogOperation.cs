using System;
using System.Collections.Generic;
using System.Linq;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Dto.Transactions;
using EVarlik.Hubs;
using log4net;
using LinqKit;

namespace EVarlik.Service.Transactions.BusinessLayer
{
    public class UserTransactionLogOperation
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public VarlikResult Save(UserTransactionLogDto userCoinTransactionLogDto)
        {
            var result = new VarlikResult();
            using (var ctx = new VarlikContext())
            {
                var entity = userCoinTransactionLogDto.ToEntity(userCoinTransactionLogDto);
                var persistent = ctx.UserCoinTransactionLog.Add(entity);
                try
                {
                    ctx.SaveChanges();
                    result.Success();
                    result.ObjectId = persistent.Id;
                }
                catch (Exception e)
                {
                    Log.Error("Save", e);
                }
            }
            return result;
        }

        public VarlikResult<List<UserTransactionLogDto>> GetAll(long idUser, int limit, int offset)
        {
            var result = new VarlikResult<List<UserTransactionLogDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new UserTransactionLogDto().FromEntity().Expand();
                result.Data = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdUser == idUser)
                    .OrderBy(l => l.Id)
                    .Take(limit)
                    .Skip(offset)
                    .Select(fromEntity)
                    .ToList();

                result.Success();
            }
            return result;
        }

        public VarlikResult<List<UserTransactionLogDto>> GetLastTwoTransactionsByIdCoinType(string idCoinType)
        {
            var result = new VarlikResult<List<UserTransactionLogDto>>();

            using (var ctx = new VarlikContext())
            {
                var fromEntity = new UserTransactionLogDto().FromEntity().Expand();

                result.Data = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdCoinType == idCoinType)
                    .OrderByDescending(l => l.RequestedDate)
                    .Take(2)
                    .Skip(0)
                    .Select(fromEntity)
                    .ToList();

                result.Success();
            }

            return result;
        }

        public VarlikResult<List<PriceDto>> GetLastPrices()
        {
            var result = new VarlikResult<List<PriceDto>>();
            result.Data = new List<PriceDto>();

            using (var ctx = new VarlikContext())
            {
                //bitcoin 
                var bitcoinList = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdCoinType == CoinTypeEnum.Btc)
                    .OrderByDescending(l => l.RequestedDate)
                    .Take(2)
                    .Skip(0)
                    .ToList();

                if (bitcoinList.Count >= 2)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.Btc;
                    priceDto.CoinUnitPrice = bitcoinList[0].CoinUnitPrice;
                    if (bitcoinList[0].CoinUnitPrice > bitcoinList[1].CoinUnitPrice)
                    {
                        priceDto.IsIncreasing = true;
                    }
                    else
                    {
                        priceDto.IsIncreasing = false;
                    }
                    priceDto.Ratio = bitcoinList[0].CoinUnitPrice / bitcoinList[1].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }
                else if (bitcoinList.Count == 1)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.Btc;
                    priceDto.CoinUnitPrice = bitcoinList[0].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }

                //doge

                var dogeList = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdCoinType == CoinTypeEnum.DogeCoin)
                    .OrderByDescending(l => l.RequestedDate)
                    .Take(2)
                    .Skip(0)
                    .ToList();

                if (dogeList.Count >= 2)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.DogeCoin;
                    priceDto.CoinUnitPrice = dogeList[0].CoinUnitPrice;
                    if (dogeList[0].CoinUnitPrice > dogeList[1].CoinUnitPrice)
                    {
                        priceDto.IsIncreasing = true;
                    }
                    else
                    {
                        priceDto.IsIncreasing = false;
                    }
                    priceDto.Ratio = dogeList[0].CoinUnitPrice / dogeList[1].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }
                else if (dogeList.Count == 1)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.DogeCoin;
                    priceDto.CoinUnitPrice = dogeList[0].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }

                //iota
                var iotaList = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdCoinType == CoinTypeEnum.Iota)
                    .OrderByDescending(l => l.RequestedDate)
                    .Take(2)
                    .Skip(0)
                    .ToList();

                if (iotaList.Count >= 2)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.Iota;
                    priceDto.CoinUnitPrice = iotaList[0].CoinUnitPrice;
                    if (iotaList[0].CoinUnitPrice > iotaList[1].CoinUnitPrice)
                    {
                        priceDto.IsIncreasing = true;
                    }
                    else
                    {
                        priceDto.IsIncreasing = false;
                    }
                    priceDto.Ratio = iotaList[0].CoinUnitPrice / iotaList[1].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }
                else if (iotaList.Count == 1)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.Iota;
                    priceDto.CoinUnitPrice = iotaList[0].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }

                //eth
                var ethList = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdCoinType == CoinTypeEnum.Eth)
                    .OrderByDescending(l => l.RequestedDate)
                    .Take(2)
                    .Skip(0)
                    .ToList();

                if (ethList.Count >= 2)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.Eth;
                    priceDto.CoinUnitPrice = ethList[0].CoinUnitPrice;
                    if (ethList[0].CoinUnitPrice > ethList[1].CoinUnitPrice)
                    {
                        priceDto.IsIncreasing = true;
                    }
                    else
                    {
                        priceDto.IsIncreasing = false;
                    }
                    priceDto.Ratio = ethList[0].CoinUnitPrice / ethList[1].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }
                else if (ethList.Count == 1)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.Eth;
                    priceDto.CoinUnitPrice = ethList[0].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }

                //ripple
                var rippleList = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdCoinType == CoinTypeEnum.Ripple)
                    .OrderByDescending(l => l.RequestedDate)
                    .Take(2)
                    .Skip(0)
                    .ToList();

                if (rippleList.Count >= 2)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.Ripple;
                    priceDto.CoinUnitPrice = rippleList[0].CoinUnitPrice;
                    if (rippleList[0].CoinUnitPrice > rippleList[1].CoinUnitPrice)
                    {
                        priceDto.IsIncreasing = true;
                    }
                    else
                    {
                        priceDto.IsIncreasing = false;
                    }
                    priceDto.Ratio = rippleList[0].CoinUnitPrice / rippleList[1].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }
                else if (rippleList.Count == 1)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.Ripple;
                    priceDto.CoinUnitPrice = rippleList[0].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }

                //LTC

                var ltcList = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdCoinType == CoinTypeEnum.LiteCoin)
                    .OrderByDescending(l => l.RequestedDate)
                    .Take(2)
                    .Skip(0)
                    .ToList();

                if (ltcList.Count >= 2)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.LiteCoin;
                    priceDto.CoinUnitPrice = ltcList[0].CoinUnitPrice;
                    if (ltcList[0].CoinUnitPrice > ltcList[1].CoinUnitPrice)
                    {
                        priceDto.IsIncreasing = true;
                    }
                    else
                    {
                        priceDto.IsIncreasing = false;
                    }
                    priceDto.Ratio = ltcList[0].CoinUnitPrice / ltcList[1].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }
                else if (ltcList.Count == 1)
                {
                    PriceDto priceDto = new PriceDto();
                    priceDto.IdCoinType = CoinTypeEnum.LiteCoin;
                    priceDto.CoinUnitPrice = ltcList[0].CoinUnitPrice;
                    result.Data.Add(priceDto);
                }

            }
            result.Success();
            return result;
        }

        public VarlikResult<decimal> GetMaxPriceOfCoin(string idCoinType)
        {
            var result = new VarlikResult<decimal>();

            using (var ctx = new VarlikContext())
            {
                var coin = ctx.UserCoinTransactionLog
                    .AsExpandable()
                    .Where(l => l.IdCoinType == idCoinType)
                    .OrderByDescending(l => l.CoinUnitPrice)
                    .Take(1)
                    .Skip(0)
                    .Select(l => l.CoinUnitPrice)
                    .FirstOrDefault();

                result.Data = coin;
                result.Success();
            }

            return result;
        }

    }
}