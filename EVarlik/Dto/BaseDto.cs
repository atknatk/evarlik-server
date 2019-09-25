using System.ComponentModel.DataAnnotations;
using System.Linq;
using EVarlik.Common.Enum;
using EVarlik.Common.Model;

namespace EVarlik.Dto
{
    public abstract class BaseDto
    {
        public VarlikResult IsValid()
        {
            var result = new VarlikResult();
            var properties = GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name == "IsExist" || propertyInfo.Name == "Select") continue;
                var attrs = propertyInfo.GetCustomAttributes(true);
                var value = propertyInfo.GetValue(this);
                var validAttr = attrs.OfType<ValidationAttribute>().FirstOrDefault(valid => !valid.IsValid(value));
                if (validAttr == null) continue;
                result.Message = string.IsNullOrEmpty(validAttr.ErrorMessageResourceName) ?
                    (string.IsNullOrEmpty(validAttr.ErrorMessage) ?
                    validAttr.FormatErrorMessage(propertyInfo.Name) :
                    validAttr.ErrorMessage) :
                    validAttr.ErrorMessageResourceName;
                result.Status = ResultStatus.InValidParamater;
                return result;
            }
            return result.Success();
        }
    }
}