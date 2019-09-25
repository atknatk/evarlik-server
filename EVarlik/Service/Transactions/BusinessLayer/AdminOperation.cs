using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Database.Entity.Transactions;
using EVarlik.Service.Commissions.BusinessLayer;
using EVarlik.Service.Commissions.Manager;
using log4net;
using Microsoft.VisualBasic.Logging;

namespace EVarlik.Service.Transactions.BusinessLayer
{
    public class AdminOperation
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public VarlikResult ApproveAdmin(long idMainOrder,
            string idTransactionState,
            bool commissionable,
            decimal confirmableMoneyAmount = -1)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var mainOrder = ctx.MainOrderLog
                    .FirstOrDefault(l => l.Id == idMainOrder
                                && l.IdTransactionState != TransactionStateEnum.Completed);

                var order = ctx.UserCoinTransactionOrder
                    .FirstOrDefault(l => l.IdMainOrderLog == idMainOrder
                                         && l.IdTransactionState != TransactionStateEnum.Completed
                                         && l.IdTransactionType == mainOrder.IdTransactionType);

                if (mainOrder == null || order == null)
                {
                    return result;
                }

                if (idTransactionState == TransactionStateEnum.Completed)
                {
                    mainOrder.IdTransactionState = TransactionStateEnum.Completed;
                    mainOrder.TransactionDate = DateTime.Now;

                    ctx.UserCoinTransactionOrder.Remove(order);

                    //write tx log

                    var commissinMoney = 0M;
                    if (mainOrder.IdTransactionType == TransactionTypeEnum.ToBank)
                    {
                        commissinMoney = 2.5M;
                    }
                    var commissionCoinCount = 0M;
                    if (mainOrder.IdTransactionType == TransactionTypeEnum.ToWallet)
                    {
                        var commissionMAnager = new CommisionOperation();
                        var commissionR = commissionMAnager.GetTransferCommissionByIdCoinType(mainOrder.IdCoinType);
                        if (commissionR.IsSuccess)
                        {
                            commissionCoinCount = commissionR.Data;
                        }
                    }

                    var moneyAmount = order.MoneyAmount;
                    if (confirmableMoneyAmount > 0M)
                    {
                        moneyAmount = confirmableMoneyAmount;
                    }

                    var coinAmount = mainOrder.CoinAmount- commissionCoinCount;
                    if (!commissionable)
                    {
                        coinAmount = mainOrder.CoinAmount;
                        commissionCoinCount = 0;
                    }
                    var tx = new UserTransactionLog()
                    {
                        MoneyAmount = moneyAmount,
                        IdTransactionState = TransactionStateEnum.Completed,
                        IdTransactionType = mainOrder.IdTransactionType,
                        TransactionDate = mainOrder.TransactionDate.Value,
                        IsSucces = true,
                        IdUser = mainOrder.IdUser,
                        IdMainOrderLog = idMainOrder,
                        CoinAmount = coinAmount,
                        CoinUnitPrice = mainOrder.CoinUnitPrice,
                        IdCoinType = mainOrder.IdCoinType,
                        CommissionMoney = commissinMoney,
                        CommissionCoinCount = commissionCoinCount
                    };

                    ctx.UserCoinTransactionLog.Add(tx);
                    try
                    {
                        ctx.SaveChanges();
                        result.Success();
                    }
                    catch (Exception e)
                    {
                        Log.Error("ApproveFromBankAdmin", e);
                    }
                }
                else if (idTransactionState == TransactionStateEnum.CancelledByAdmin)
                {
                    ctx.UserCoinTransactionOrder.Remove(order);
                    mainOrder.IdTransactionState = TransactionStateEnum.CancelledByAdmin;
                    mainOrder.TransactionDate = DateTime.Now;
                    try
                    {
                        ctx.SaveChanges();
                        result.Success();
                    }
                    catch (Exception e)
                    {
                        Log.Error("ApproveFromBankAdmin", e);
                    }
                }
            }
            return result;
        }


    }
}