using System; 
using EVarlik.Database.Context;
using EVarlik.Database.Entity.Commissions;
using EVarlik.Database.Entity.Lookup;
using EVarlik.Database.Entity.Transactions;
using EVarlik.Database.Entity.Users;

namespace EVarlik.ReadWrite
{
    public class GenerateDummyData
    {

        public static void WriteUser()
        {
            using (var ctx = new VarlikContext())
            {
                User user = new User();
                user.IsUserApproved = true;
                user.IsDeleted = false;
                user.Mail = "umurinan@gmail.com";
                user.Name = "Umur";
                user.Surname = "İnan";
                user.Adres = "West Gate";
                user.Iban = "TR45145715417541571";
                user.Password = "1";

                ctx.User.Add(user);
                ctx.SaveChanges();

                user = new User();
                user.IsUserApproved = true;
                user.IsDeleted = false;
                user.Mail = "aliaktas@gmail.com";
                user.Name = "Ali";
                user.Surname = "Aktaş";
                user.Adres = "Relax Plus";
                user.Iban = "DE251457154456475475";
                user.Password = "1";

                ctx.User.Add(user);
                ctx.SaveChanges();

                user = new User();
                user.IsUserApproved = true;
                user.IsDeleted = false;
                user.Mail = "atkatkn@gmail.com";
                user.Name = "Atakan";
                user.Surname = "Atik";
                user.Adres = "Yeşilsite Sitesi";
                user.Iban = "UK251457154175415854";
                user.Password = "1";

                ctx.User.Add(user);
                ctx.SaveChanges();
            }
        }

        public static void WriteTransactionType()
        {
            using (var ctx = new VarlikContext())
            {
                TransactionTypeEnum transactionTypeEnum = new TransactionTypeEnum();
                transactionTypeEnum.Code = "coin_sales";
                transactionTypeEnum.Name = "Coin Satışı";

                ctx.TransactionTypeEnum.Add(transactionTypeEnum);
                ctx.SaveChanges();

                transactionTypeEnum = new TransactionTypeEnum();
                transactionTypeEnum.Code = "coin_purchasing";
                transactionTypeEnum.Name = "Coin Alımı";

                ctx.TransactionTypeEnum.Add(transactionTypeEnum);
                ctx.SaveChanges();

                transactionTypeEnum = new TransactionTypeEnum();
                transactionTypeEnum.Code = "to_bank";
                transactionTypeEnum.Name = "Para Çekme";

                ctx.TransactionTypeEnum.Add(transactionTypeEnum);
                ctx.SaveChanges();

                transactionTypeEnum = new TransactionTypeEnum();
                transactionTypeEnum.Code = "from_bank";
                transactionTypeEnum.Name = "Para Yatırma";

                ctx.TransactionTypeEnum.Add(transactionTypeEnum);
                ctx.SaveChanges();

                transactionTypeEnum = new TransactionTypeEnum();
                transactionTypeEnum.Code = "to_wallet";
                transactionTypeEnum.Name = "Cüzdana Aktar";

                ctx.TransactionTypeEnum.Add(transactionTypeEnum);
                ctx.SaveChanges();

                transactionTypeEnum = new TransactionTypeEnum();
                transactionTypeEnum.Code = "from_wallet";
                transactionTypeEnum.Name = "Cüzdandan Aktar";

                ctx.TransactionTypeEnum.Add(transactionTypeEnum);
                ctx.SaveChanges();
            }
        }

        public static void WriteTransactionState()
        {
            using (var ctx = new VarlikContext())
            {
                TransactionStateEnum transactionStateEnum = new TransactionStateEnum();
                transactionStateEnum.Code = "processing";
                transactionStateEnum.Name = "İşlemde";

                ctx.TransactionStateEnum.Add(transactionStateEnum);
                ctx.SaveChanges();

                transactionStateEnum = new TransactionStateEnum();
                transactionStateEnum.Code = "fail";
                transactionStateEnum.Name = "Başarısız";

                ctx.TransactionStateEnum.Add(transactionStateEnum);
                ctx.SaveChanges();

                transactionStateEnum = new TransactionStateEnum();
                transactionStateEnum.Code = "partialy_completed";
                transactionStateEnum.Name = "Kısmen Tamamlandı";

                ctx.TransactionStateEnum.Add(transactionStateEnum);
                ctx.SaveChanges();

                transactionStateEnum = new TransactionStateEnum();
                transactionStateEnum.Code = "money_being_sent";
                transactionStateEnum.Name = "money_being_sent";

                ctx.TransactionStateEnum.Add(transactionStateEnum);
                ctx.SaveChanges();

                transactionStateEnum = new TransactionStateEnum();
                transactionStateEnum.Code = "pending_approval";
                transactionStateEnum.Name = "pending_approval";

                ctx.TransactionStateEnum.Add(transactionStateEnum);
                ctx.SaveChanges();

                transactionStateEnum = new TransactionStateEnum();
                transactionStateEnum.Code = "completed";
                transactionStateEnum.Name = "completed";

                ctx.TransactionStateEnum.Add(transactionStateEnum);
                ctx.SaveChanges();

                transactionStateEnum = new TransactionStateEnum();
                transactionStateEnum.Code = "cancelled_by_user";
                transactionStateEnum.Name = "cancelled_by_user";

                ctx.TransactionStateEnum.Add(transactionStateEnum);
                ctx.SaveChanges();

            }
        }

        public static void WriteCoinType()
        {
            using (var ctx = new VarlikContext())
            {
                CoinTypeEnum coinTypeEnum = new CoinTypeEnum();
                coinTypeEnum.Code = "BTC";
                coinTypeEnum.Name = "BitCoin";

                ctx.CoinTypeEnum.Add(coinTypeEnum);
                ctx.SaveChanges();

                coinTypeEnum = new CoinTypeEnum();
                coinTypeEnum.Code = "LTC";
                coinTypeEnum.Name = "LiteCoin";

                ctx.CoinTypeEnum.Add(coinTypeEnum);
                ctx.SaveChanges();

                coinTypeEnum = new CoinTypeEnum();
                coinTypeEnum.Code = "DOGE";
                coinTypeEnum.Name = "DogeCoin";

                ctx.CoinTypeEnum.Add(coinTypeEnum);
                ctx.SaveChanges();

                coinTypeEnum = new CoinTypeEnum();
                coinTypeEnum.Code = "IOTA";
                coinTypeEnum.Name = "IOTA";

                ctx.CoinTypeEnum.Add(coinTypeEnum);
                ctx.SaveChanges();

                coinTypeEnum = new CoinTypeEnum();
                coinTypeEnum.Code = "ETH";
                coinTypeEnum.Name = "ETH";

                ctx.CoinTypeEnum.Add(coinTypeEnum);
                ctx.SaveChanges();

                coinTypeEnum = new CoinTypeEnum();
                coinTypeEnum.Code = "XRP";
                coinTypeEnum.Name = "XRP";

                ctx.CoinTypeEnum.Add(coinTypeEnum);
                ctx.SaveChanges();
            }
        }

        public static void WriteCommission()
        {
            using (var ctx = new VarlikContext())
            {
                Commission commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Btc;
                commission.TransactionVolume = 0;
                commission.MakerPercatange = 0.0018M;
                commission.TakerPercatange= 0.0025M;
                commission.TransferFeeCoinCount = 0.0006M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Btc;
                commission.TransactionVolume = 250000;
                commission.MakerPercatange = 0.0015M;
                commission.TakerPercatange = 0.0024M;
                commission.TransferFeeCoinCount = 0.0006M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Btc;
                commission.TransactionVolume = 500000;
                commission.MakerPercatange = 0.0013M;
                commission.TakerPercatange = 0.0020M;
                commission.TransferFeeCoinCount = 0.0006M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                //--

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.DogeCoin;
                commission.TransactionVolume = 0;
                commission.MakerPercatange = 0.0020M;
                commission.TakerPercatange = 0.0030M;
                commission.TransferFeeCoinCount = 2;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.DogeCoin;
                commission.TransactionVolume = 250000;
                commission.MakerPercatange = 0.0015M;
                commission.TakerPercatange = 0.0025M;
                commission.TransferFeeCoinCount = 2;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.DogeCoin;
                commission.TransactionVolume = 500000;
                commission.MakerPercatange = 0.0013M;
                commission.TakerPercatange = 0.0015M;
                commission.TransferFeeCoinCount = 2;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                //--

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.LiteCoin;
                commission.TransactionVolume = 0;
                commission.MakerPercatange = 0.0020M;
                commission.TakerPercatange = 0.0030M;
                commission.TransferFeeCoinCount = 0.01M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.LiteCoin;
                commission.TransactionVolume = 250000;
                commission.MakerPercatange = 0.0015M;
                commission.TakerPercatange = 0.0025M;
                commission.TransferFeeCoinCount = 0.01M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.LiteCoin;
                commission.TransactionVolume = 500000;
                commission.MakerPercatange = 0.0013M;
                commission.TakerPercatange = 0.0015M;
                commission.TransferFeeCoinCount = 0.01M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                //--

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Eth;
                commission.TransactionVolume = 0;
                commission.MakerPercatange = 0.0020M;
                commission.TakerPercatange = 0.0030M;
                commission.TransferFeeCoinCount = 0.01M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Eth;
                commission.TransactionVolume = 250000;
                commission.MakerPercatange = 0.0015M;
                commission.TakerPercatange = 0.0025M;
                commission.TransferFeeCoinCount = 0.01M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Eth;
                commission.TransactionVolume = 500000;
                commission.MakerPercatange = 0.0013M;
                commission.TakerPercatange = 0.0015M;
                commission.TransferFeeCoinCount = 0.01M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                //--

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Ripple;
                commission.TransactionVolume = 0;
                commission.MakerPercatange = 0.0035M;
                commission.TakerPercatange = 0.0045M;
                commission.TransferFeeCoinCount = 4;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Ripple;
                commission.TransactionVolume = 250000;
                commission.MakerPercatange = 0.0030M;
                commission.TakerPercatange = 0.0040M;
                commission.TransferFeeCoinCount = 4;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Ripple;
                commission.TransactionVolume = 500000;
                commission.MakerPercatange = 0.0025M;
                commission.TakerPercatange = 0.0035M;
                commission.TransferFeeCoinCount = 4;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                //--
                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Iota;
                commission.TransactionVolume = 0;
                commission.MakerPercatange = 0.0035M;
                commission.TakerPercatange = 0.0045M;
                commission.TransferFeeCoinCount = 0.5M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Iota;
                commission.TransactionVolume = 250000;
                commission.MakerPercatange = 0.0030M;
                commission.TakerPercatange = 0.0040M;
                commission.TransferFeeCoinCount = 0.5M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

                commission = new Commission();
                commission.IdCoinType = Common.Enum.CoinTypeEnum.Iota;
                commission.TransactionVolume = 500000;
                commission.MakerPercatange = 0.0025M;
                commission.TakerPercatange = 0.0035M;
                commission.TransferFeeCoinCount = 0.5M;

                ctx.Commission.Add(commission);
                ctx.SaveChanges();

            }
        }

        private static void WriteTransactionOrder()
        {
            using (var ctx = new VarlikContext())
            {
                UserCoinTransactionOrder coinTransactionOrder = new UserCoinTransactionOrder();
                coinTransactionOrder.IdUser = 2;
                coinTransactionOrder.CoinAmount = 100;
                coinTransactionOrder.CoinUnitPrice = 1;
                coinTransactionOrder.IdCoinType = "dgc";
                coinTransactionOrder.IdTransactionType = "coin_sales";
                coinTransactionOrder.CreatedAt = DateTime.Now;

                ctx.UserCoinTransactionOrder.Add(coinTransactionOrder);
                ctx.SaveChanges();

                coinTransactionOrder = new UserCoinTransactionOrder();
                coinTransactionOrder.IdUser = 2;
                coinTransactionOrder.CoinAmount = 100;
                coinTransactionOrder.CoinUnitPrice = 100;
                coinTransactionOrder.IdCoinType = "btc";
                coinTransactionOrder.IdTransactionType = "coin_sales";
                coinTransactionOrder.CreatedAt = DateTime.Now;

                ctx.UserCoinTransactionOrder.Add(coinTransactionOrder);
                ctx.SaveChanges();

                coinTransactionOrder = new UserCoinTransactionOrder();
                coinTransactionOrder.IdUser = 3;
                coinTransactionOrder.CoinAmount = 100;
                coinTransactionOrder.CoinUnitPrice = 0.9M;
                coinTransactionOrder.IdCoinType = "dgc";
                coinTransactionOrder.IdTransactionType = "coin_sales";
                coinTransactionOrder.CreatedAt = DateTime.Now;

                ctx.UserCoinTransactionOrder.Add(coinTransactionOrder);
                ctx.SaveChanges();

                coinTransactionOrder = new UserCoinTransactionOrder();
                coinTransactionOrder.IdUser = 2;
                coinTransactionOrder.MoneyAmount = 150;
                coinTransactionOrder.IdTransactionType = "to_bank";
                coinTransactionOrder.CreatedAt = DateTime.Now;

                ctx.UserCoinTransactionOrder.Add(coinTransactionOrder);
                ctx.SaveChanges();

                coinTransactionOrder = new UserCoinTransactionOrder();
                coinTransactionOrder.IdUser = 3;
                coinTransactionOrder.MoneyAmount = 350;
                coinTransactionOrder.IdTransactionType = "from_bank";
                coinTransactionOrder.CreatedAt = DateTime.Now;

                ctx.UserCoinTransactionOrder.Add(coinTransactionOrder);
                ctx.SaveChanges();
            }
        }

    }
}