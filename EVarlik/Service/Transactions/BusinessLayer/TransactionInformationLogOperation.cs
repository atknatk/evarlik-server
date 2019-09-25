using System;
using System.Collections.Generic;
using System.Linq;
using EVarlik.Common.Model;
using EVarlik.Database.Context;
using EVarlik.Dto.Transactions;
using log4net;
using LinqKit;

namespace EVarlik.Service.Transactions.BusinessLayer
{
    public class TransactionInformationLogOperation
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public VarlikResult Save(TransactionInformationLogDto transactionInformationLogDto)
        {
            var result = new VarlikResult();

            using (var ctx = new VarlikContext())
            {
                var entity = transactionInformationLogDto.ToEntity(transactionInformationLogDto);

                var persistent = ctx.TransactionInformationLog.Add(entity);

                try
                {
                    ctx.SaveChanges();
                    result.ObjectId = persistent.Id;
                    result.Success();
                }
                catch (Exception e)
                {
                    Log.Error("Save", e);
                }
            }
            return result;
        }

        public VarlikResult<List<TransactionInformationLogDto>> GetAll(long idUser,int limit,int offset)
        {
            var result = new VarlikResult<List<TransactionInformationLogDto>>();
            using (var ctx = new VarlikContext())
            {
                var fromEntity = new TransactionInformationLogDto().FromEntity().Expand();

                result.Data = ctx.TransactionInformationLog
                    .AsExpandable()
                    .Where(l=>l.IdUser == idUser)
                    .OrderBy(l=>l.Id)
                    .Take(limit)
                    .Skip(offset)
                    .Select(fromEntity)
                    .ToList();

                result.Success();
            }
            return result;
        }

    }
}