namespace EVarlik.Common.Enum
{
    public class TransactionStateEnum
    {
        public const string Processing = "processing";
        public const string Completed = "completed";
        public const string PartialyCompleted = "partialy_completed";
        public const string MoneyBeingSent = "money_being_sent";
        public const string PendingApproval = "pending_approval";
        public const string Failed = "failed";
        public const string CancelledByUser = "cancelled_by_user";
        public const string CancelledByAdmin = "cancelled_by_admin";

    }
}