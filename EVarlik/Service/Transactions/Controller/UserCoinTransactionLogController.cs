using System.Collections.Generic;
using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Dto.Transactions;
using EVarlik.Service.Transactions.Manager;

namespace EVarlik.Service.Transactions.Controller
{
    public class UserCoinTransactionLogController : VarlikController
    {
        private readonly UserCoinTransactionLogManager _userCoinTransactionLogManager;

        public UserCoinTransactionLogController()
        {
            _userCoinTransactionLogManager = new UserCoinTransactionLogManager();
        }

        [HttpPost]
        public VarlikResult Save([FromBody]UserTransactionLogDto userCoinTransactionLogDto)
        {
            return _userCoinTransactionLogManager.Save(userCoinTransactionLogDto);
        }

        [HttpGet]
        public VarlikResult<List<UserTransactionLogDto>> GetAll(int limit, int offset)
        {
            return _userCoinTransactionLogManager.GetAll(limit, offset);
        }

        [HttpGet]
        [Route("api/Price/All")]
        public VarlikResult<List<PriceDto>> GetLastPrices()
        {
            return _userCoinTransactionLogManager.GetLastPrices();
        }
    }
}