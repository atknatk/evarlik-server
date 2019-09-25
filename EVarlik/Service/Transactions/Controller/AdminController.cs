using System.Web.Http;
using EVarlik.Common.Model;
using EVarlik.Controllers;
using EVarlik.Service.Transactions.Manager;

namespace EVarlik.Service.Transactions.Controller
{
    public class AdminController : VarlikController
    {
        private readonly AdminManager _adminManager;

        public AdminController()
        {
            _adminManager = new AdminManager();
        }

        [HttpPost]
        [Route("api/Admin/ApproveAdmin")]
        public VarlikResult ApproveAdmin(long idMainOrder,
            string idTransactionState,
            bool commissionable,
            decimal confirmableMoneyAmount)
        {
            return _adminManager.ApproveAdmin(idMainOrder, idTransactionState, commissionable, confirmableMoneyAmount);
        }
    }
}