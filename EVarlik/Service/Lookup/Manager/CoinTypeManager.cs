using System.Collections.Generic;
using EVarlik.Common.Model;
using EVarlik.Dto.Lookup;
using EVarlik.Service.Lookup.BusinessLayer;

namespace EVarlik.Service.Lookup.Manager
{
    public class CoinTypeManager
    {
        private readonly CoinTypeOperation _coinTypeOperation;

        public CoinTypeManager()
        {
            _coinTypeOperation = new CoinTypeOperation();
        }

        public VarlikResult<List<CoinTypeDto>> GetAll()
        {
            return _coinTypeOperation.GetAll();
        }
    }
}