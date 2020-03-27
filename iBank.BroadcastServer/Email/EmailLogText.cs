using System;

namespace iBank.BroadcastServer.Email
{
    public class EmailLogText
    {
        public static string GetEmailSendMessage(bool success, string emailAddress)
        {
            return success
                        ? Environment.NewLine + "Email sent to " + emailAddress
                        : Environment.NewLine + "Email send failed to " + emailAddress;
        }

        public static string GetNoDataMessage(string emailAddress)
        {
            return Environment.NewLine + "Email not sent to " + emailAddress + " because all reports have no data.";;
        }
    }
}
