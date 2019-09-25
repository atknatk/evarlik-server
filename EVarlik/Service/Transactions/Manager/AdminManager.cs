using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Identity;
using EVarlik.Service.Transactions.BusinessLayer;

namespace EVarlik.Service.Transactions.Manager
{
    public class AdminManager
    {
        private readonly AdminOperation _adminOperation;

        public AdminManager()
        {
            _adminOperation = new AdminOperation();
        }

        public VarlikResult ApproveAdmin(long idMainOrder,
            string idTransactionState,
            bool commissionable,
            decimal confirmableMoneyAmount = -1)
        {
            var userId = IdentityHelper.Instance.CurrentUserId;
            if (userId > 0 && userId < 1001)
            {
                return _adminOperation.ApproveAdmin(idMainOrder, idTransactionState, commissionable,confirmableMoneyAmount);
            }
            var result = new VarlikResult();
            result.Status = ResultStatus.Unauthorized;
            return result;
        }
    }
}