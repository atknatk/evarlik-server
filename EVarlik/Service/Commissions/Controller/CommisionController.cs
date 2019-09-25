using System.Collections.Generic;
using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Dto.Commisions;
using EVarlik.Service.Commissions.Manager;

namespace EVarlik.Service.Commissions.Controller
{
    public class CommisionController : VarlikController
    {
        private readonly CommisionManager _commisionManager;

        public CommisionController()
        {
            _commisionManager = new CommisionManager();
        }

        [HttpGet]
        [Route("api/Commission/All")]
        public VarlikResult<List<CommisionDto>> GetAllCommission()
        {
            return _commisionManager.GetAllCommission();
        }
    }
}