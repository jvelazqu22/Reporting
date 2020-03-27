namespace Domain.Constants
{
    public class BroadcastLoggingMessage
    {
        public const string DatabaseErrorRemoveFromQueue = "The broadcast encountered a database error and will be removed from queue to allow retry.";

        public const string NotTimeToRunBroadcast = "Not time to run broadcast";

        public const string BroadcastDidNotContainReports = "Broadcast did not contain any reports. Marked in error and removed from queue.";

        public const string UserDoesNotExist = "User does not exist";

        public const string GenericError = "The broacast contained an error.";
    }
}
