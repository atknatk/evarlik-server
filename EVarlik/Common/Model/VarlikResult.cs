using System.Collections.Generic;
using EVarlik.Common.Enum;

namespace EVarlik.Common.Model
{
    public class VarlikResult
    {
        public ResultStatus Status { get; set; }
        public string Message { get; set; }
        public long ObjectId { get; set; }
        public string Exception { get; set; }

        public VarlikResult()
        {
            Status = ResultStatus.UnknownError; 
        }

        public VarlikResult Success()
        {
            Status = ResultStatus.Success;
            return this;
        }

        public bool IsSuccess => Status == ResultStatus.Success;
    }

    public class VarlikResult<T> : VarlikResult
    {
        public T Data { get; set; }
        public VarlikResult<T> SetData(T data, ResultStatus status = ResultStatus.Success)
        {
            Data = data;
            Status = status;
            return this;
        }
    }
}