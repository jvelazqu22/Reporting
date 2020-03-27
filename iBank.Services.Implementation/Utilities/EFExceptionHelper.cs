using System.Data.Entity.Validation;
using CODE.Framework.Core.Utilities;

namespace iBank.Services.Implementation.Utilities
{
    public static class EFExceptionHelper
    {
        public static string ProcessDbEntityValidationErrors(string initialMessage, DbEntityValidationException e)
        {
            var result = string.Empty;
            LoggingMediator.Log(initialMessage, e);
            foreach (var eve in e.EntityValidationErrors)
            {
                LoggingMediator.Log(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                foreach (var ve in eve.ValidationErrors)
                {
                    LoggingMediator.Log(string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                        ve.PropertyName,
                        eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                        ve.ErrorMessage));
                    result = result + ", " + ve.ErrorMessage;
                }
            }
            return initialMessage + result;
        }

        public static string ProcessDbEntityValidationErrors(string initialMessage, DbEntityValidationException e, com.ciswired.libraries.CISLogger.ILogger log)
        {
            var result = string.Empty;
            log.Error(initialMessage, e);
            foreach (var eve in e.EntityValidationErrors)
            {
                log.Error(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                foreach (var ve in eve.ValidationErrors)
                {
                    log.Error(string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                        ve.PropertyName,
                        eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                        ve.ErrorMessage));
                    result = result + ", " + ve.ErrorMessage;
                }
            }
            return initialMessage + result;
        }
    }
}
