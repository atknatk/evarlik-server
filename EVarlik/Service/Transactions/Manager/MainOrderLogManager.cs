using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;
using EVarlik.Database.Entity.Transactions;
using EVarlik.Dto.Transactions;
using EVarlik.Identity;
using EVarlik.Service.Transactions.BusinessLayer;
using LinqKit;

namespace EVarlik.Service.Transactions.Manager
{
    public class MainOrderLogManager
    {
        private readonly MainOrderLogOperation _mainOrderLogOperation;

        public MainOrderLogManager()
        {
            _mainOrderLogOperation = new MainOrderLogOperation();
        }

        public VarlikResult Save(MainOrderLogDto mainOrderLogDto)
        {
            return _mainOrderLogOperation.Save(mainOrderLogDto);
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllMainOrderByIdCoinType(string idCoinType)
        {
            long idUser = IdentityHelper.Instance.CurrentUserId;
            return _mainOrderLogOperation.GetAllMainOrderByIdCoinType(idUser, idCoinType);
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllBankOrder()
        {
            long idUser = IdentityHelper.Instance.CurrentUserId;
            return _mainOrderLogOperation.GetAllBankOrder(idUser);
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllWalletOrder(string idCoinType)
        {
            long idUser = IdentityHelper.Instance.CurrentUserId;
            return _mainOrderLogOperation.GetAllWalletOrder(idUser, idCoinType);
        }

        public VarlikResult CancelTheOrder(long idMainOrder)
        {
            long idUser = IdentityHelper.Instance.CurrentUserId;
            return _mainOrderLogOperation.CancelTheOrder(idMainOrder, idUser);
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllOrder()
        {
            long idUser = IdentityHelper.Instance.CurrentUserId;
            return _mainOrderLogOperation.GetAllOrder(idUser);
        }

        public VarlikResult<List<MainOrderLogAdminDto>> GetAllOrderAdmin(MainOrderLogDto dto, int limit, int offset)
        {
            if (limit > 1000 || limit == 0)
            {
                limit = 1000;
            }

            var userId = IdentityHelper.Instance.CurrentUserId;
            if (userId > 0 && userId < 501)
            {
                if (string.IsNullOrEmpty(dto?.IdTransactionType))
                {
                    Expression<Func<MainOrderLog, bool>> predicateNull = l => true;
                    return _mainOrderLogOperation.GetAllOrderAdmin(predicateNull, limit, offset);
                }
                var transactionTypeArr = dto.IdTransactionType.Split(',');
                Expression<Func<MainOrderLog, bool>> predicate = l => transactionTypeArr.Contains(l.IdTransactionType);

                if (!string.IsNullOrEmpty(dto.IdTransactionState))
                {
                    var transactionStateArr = dto.IdTransactionState.Split(',');
                    predicate = predicate.And(l => transactionStateArr.Contains(l.IdTransactionState));
                }

                if (!string.IsNullOrEmpty(dto.IdCoinType))
                {
                    var coinTypeArr = dto.IdCoinType.Split(',');
                    predicate = predicate.And(l => coinTypeArr.Contains(l.IdCoinType));
                }
                return _mainOrderLogOperation.GetAllOrderAdmin(predicate, limit, offset);
            }
            var result = new VarlikResult<List<MainOrderLogAdminDto>>();
            result.Status = ResultStatus.Unauthorized;
            return result;
        }

        public VarlikResult ApproveFromBankAdmin(long idMainOrder,string idTransactionState)
        {
            var userId = IdentityHelper.Instance.CurrentUserId;
            if (userId > 0 && userId < 1001)
            {
                return _mainOrderLogOperation.ApproveFromBankAdmin(idMainOrder, idTransactionState);
            }
            var result = new VarlikResult();
            result.Status = ResultStatus.Unauthorized;
            return result;
        }

        public VarlikResult ApproveToBankAdmin(long idMainOrder)
        {
            var userId = IdentityHelper.Instance.CurrentUserId;
            if (userId > 0 && userId < 1001)
            {
                return _mainOrderLogOperation.ApproveToBankAdmin(idMainOrder);
            }
            var result = new VarlikResult();
            result.Status = ResultStatus.Unauthorized;
            return result;
        }

        public VarlikResult<List<MainOrderLogDto>> GetAllRealCoinOrderAdmin(int limit, int offset)
        {
            if (limit > 1000 || limit == 0)
            {
                limit = 1000;
            }

            var userId = IdentityHelper.Instance.CurrentUserId;
            if (userId > 0 && userId < 1001)
            {
                return _mainOrderLogOperation.GetAllRealCoinOrderAdmin(limit, offset);
            }
            var result = new VarlikResult<List<MainOrderLogDto>>();
            result.Status = ResultStatus.Unauthorized;
            return result;
        }
    }
}