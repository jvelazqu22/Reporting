using System.Reflection;
using System.Text.RegularExpressions;

using com.ciswired.libraries.CISLogger;

using Domain.Orm.iBankMastersQueries.BroadcastQueries;

using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.BroadcastReport
{
    public class RecoverableFoxProErrorHandler
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG =
            new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string _foxProErrorMessage = "AN UNEXPECTED ERROR HAS OCCURRED";
        private static readonly string _nolockMessage = "COULD NOT CONTINUE SCAN WITH NOLOCK";
        public static bool IsRecoverableError(string errorText, IMastersQueryable queryDb)
        {
            LOG.Debug($"Checking if FoxPro broadcast failed due to NOLOCK. Error text: [{errorText}]");
            if (!errorText.ToUpper().Contains(_foxProErrorMessage)) return false;

            var errorNumber = GetErrorNumber(errorText);

            if (!errorNumber.HasValue) return false;

            var errorMessage = GetErrorMessage(errorNumber.Value, queryDb);

            return IsRecoverableError(errorMessage);
        }

        private static int? GetErrorNumber(string errorText)
        {
            var tempNumber = Regex.Match(errorText, @"\d+").Value;
            var errorNumber = 0;

            if (int.TryParse(tempNumber, out errorNumber))
            {
                return errorNumber;
            }
            else
            {
                return null;
            }
        }

        private static string GetErrorMessage(int errorNumber, IMastersQueryable db)
        {
            var query = new GetErrorMessageQuery(db, errorNumber);
            return query.ExecuteQuery();
        }

        private static bool IsRecoverableError(string errorMessage)
        {
            if(string.IsNullOrEmpty(errorMessage)) return false;

            return errorMessage.ToUpper().Contains(_nolockMessage);
        }
    }
}
