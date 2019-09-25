using System;
using System.Linq.Expressions;
using EVarlik.Database.Entity.Users;

namespace EVarlik.Dto.Users
{
    public class UserWalletDto
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public string IdCoinType { get; set; }
        public string Address { get; set; }
        public string Secret { get; set; }

        public Expression<Func<UserWallet, UserWalletDto>> FromEntity()
        {
            return l => new UserWalletDto()
            {
                Id = l.Id,
                IdUser = l.IdUser,
                IdCoinType = l.IdCoinType,
                Address = l.Address,
                Secret = l.Secret
            };
        }

        public UserWallet ToEntity(UserWalletDto userWalletDto)
        {
            return new UserWallet()
            {
                Id = userWalletDto.Id,
                IdUser = userWalletDto.IdUser,
                IdCoinType = userWalletDto.IdCoinType,
                Address = userWalletDto.Address,
                Secret = userWalletDto.Secret
            };
        }
    }
}