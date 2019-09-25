using System.Threading.Tasks;
using EVarlik.Common.Model;

namespace EVarlik.Service.Wallets.Manager
{
    public interface IWalletManager
    {
        Task<VarlikResult<string>>CreateNewAddress(string idCoinType);
        Task<VarlikResult<decimal>> GetAddressBalance(string idCoinType, string address);

        VarlikResult<string> Withdraw(string idCoinType,
            string fromAddress,
            string toAddress,
            decimal amount,
            string pin);
    }
}