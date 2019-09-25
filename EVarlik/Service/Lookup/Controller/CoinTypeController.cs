using System.Collections.Generic;
using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Dto.Lookup;
using EVarlik.Service.Lookup.Manager;

namespace EVarlik.Service.Lookup.Controller
{
    public class CoinTypeController : VarlikController
    {
        private readonly CoinTypeManager _coinTypeManager;

        public CoinTypeController()
        {
            _coinTypeManager = new CoinTypeManager();
        }

        [HttpGet]
        public VarlikResult<List<CoinTypeDto>> GetAll()
        {
            return _coinTypeManager.GetAll();
        }
    }
}