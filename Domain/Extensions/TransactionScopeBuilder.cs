using System.Transactions;

namespace Domain.Extensions
{
    public static class TransactionScopeBuilder
    {
        public static TransactionScope BuildNoLockScope()
        {
            return new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() {IsolationLevel = IsolationLevel.ReadUncommitted});
        }

        public static TransactionScope BuildScope()
        {
            return new TransactionScope(TransactionScopeOption.RequiresNew);
        }


    }
}
