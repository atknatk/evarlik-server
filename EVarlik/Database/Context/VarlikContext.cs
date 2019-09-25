using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EVarlik.Common.Attributes;
using EVarlik.Database.Entity.Commissions;
using EVarlik.Database.Entity.Lookup;
using EVarlik.Database.Entity.Transactions;
using EVarlik.Database.Entity.Users;
using EVarlik.Database.Entity.Wallets;

namespace EVarlik.Database.Context
{
    public class VarlikContext : DbContext
    {

        public DbSet<User> User { get; set; }
        public DbSet<VarlikLog> VarlikLog { get; set; }
        public DbSet<UserWallet> UserWallet { get; set; }
        public DbSet<Attachment> Attachment { get; set; }

        public DbSet<CoinTypeEnum> CoinTypeEnum { get; set; }
        public DbSet<TransactionStateEnum> TransactionStateEnum { get; set; }
        public DbSet<TransactionTypeEnum> TransactionTypeEnum { get; set; }

        public DbSet<TransactionInformationLog> TransactionInformationLog { get; set; }
        public DbSet<UserTransactionLog> UserCoinTransactionLog { get; set; }
        public DbSet<MainOrderLog> MainOrderLog { get; set; }
        public DbSet<UserCoinTransactionOrder> UserCoinTransactionOrder { get; set; }

        public DbSet<Commission> Commission { get; set; }

        public DbSet<ApiKey> ApiKey { get; set; }

        public VarlikContext() : base(nameOrConnectionString: "VarlikContext")
        {
            Database.CreateIfNotExists();

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");

            Database.Log = (query) => Debug.Write(query);
           
            foreach (Type classType in from t in Assembly.GetAssembly(typeof(DecimalPrecisionAttribute)).GetTypes()
                                       where t.Namespace != null && (t.IsClass && t.Namespace.StartsWith("EVarlik.Database.Entity"))
                                       select t)
            {
                foreach (var propAttr in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttribute<DecimalPrecisionAttribute>() != null).Select(
                       p => new { prop = p, attr = p.GetCustomAttribute<DecimalPrecisionAttribute>(true) }))
                {

                    var entityConfig = modelBuilder.GetType().GetMethod("Entity")?.MakeGenericMethod(classType).Invoke(modelBuilder, null);
                    ParameterExpression param = ParameterExpression.Parameter(classType, "c");
                    Expression property = Expression.Property(param, propAttr.prop.Name);
                    LambdaExpression lambdaExpression = Expression.Lambda(property, true,
                                                                             new ParameterExpression[]
                                                                                 {param});
                    DecimalPropertyConfiguration decimalConfig;
                    if (propAttr.prop.PropertyType.IsGenericType && propAttr.prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        MethodInfo methodInfo = entityConfig?.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[7];
                        decimalConfig = methodInfo?.Invoke(entityConfig, new[] { lambdaExpression }) as DecimalPropertyConfiguration;
                    }
                    else
                    {
                        MethodInfo methodInfo = entityConfig?.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[6];
                        decimalConfig = methodInfo?.Invoke(entityConfig, new[] { lambdaExpression }) as DecimalPropertyConfiguration;
                    }

                    decimalConfig?.HasPrecision(propAttr.attr.Precision, propAttr.attr.Scale);
                }
            }


        }
    }
}