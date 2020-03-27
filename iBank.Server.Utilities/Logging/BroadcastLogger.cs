using System;

using com.ciswired.libraries.CISLogger;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace iBank.Server.Utilities.Logging
{
    public interface IBroadcastLogger
    {
        void DebugLogWithBatchInfo(bcstque4 broadcast, string msg);

        void DebugLogWithBatchInfo(ibbatch broadcast, string msg);

        void DebugLogWithBatchInfo(bcstque4 broadcast, string msg, Exception ex);

        void DebugLogWithBatchInfo(ibbatch broadcast, string msg, Exception ex);

        void InfoLogWithBatchInfo(bcstque4 broadcast, string msg);

        void InfoLogWithBatchInfo(ibbatch broadcast, string msg);

        void InfoLogWithBatchInfo(bcstque4 broadcast, string msg, Exception ex);

        void InfoLogWithBatchInfo(ibbatch broadcast, string msg, Exception ex);

        void WarnLogWithBatchInfo(bcstque4 broadcast, string msg);

        void WarnLogWithBatchInfo(ibbatch broadcast, string msg);

        void WarnLogWithBatchInfo(bcstque4 broadcast, string msg, Exception ex);

        void WarnLogWithBatchInfo(ibbatch broadcast, string msg, Exception ex);

        void ErrorLogWithBatchInfo(bcstque4 broadcast, string msg);

        void ErrorLogWithBatchInfo(ibbatch broadcast, string msg);

        void ErrorLogWithBatchInfo(bcstque4 broadcast, string msg, Exception ex);

        void ErrorLogWithBatchInfo(ibbatch broadcast, string msg, Exception ex);

        string FormatBroadcastInfo(bcstque4 broadcast);

        string FormatBroadcastInfo(ibbatch broadcast);

        string FormatMessageWithBroadcastInfo(bcstque4 broadcast, string msg);

        string FormatMessageWithBroadcastInfo(ibbatch broadcast, string msg);
    }

    public class BroadcastLogger : IBroadcastLogger
    {
        private readonly ILogger _log;
        public BroadcastLogger(ILogger log)
        {
            _log = log;
        }

        public void DebugLogWithBatchInfo(bcstque4 broadcast, string msg)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Debug(message);
        }

        public void DebugLogWithBatchInfo(ibbatch broadcast, string msg)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Debug(message);
        }

        public void DebugLogWithBatchInfo(bcstque4 broadcast, string msg, Exception ex)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Debug(message, ex);
        }

        public void DebugLogWithBatchInfo(ibbatch broadcast, string msg, Exception ex)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Debug(message, ex);
        }
        public void InfoLogWithBatchInfo(bcstque4 broadcast, string msg)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Info(message);
        }

        public void InfoLogWithBatchInfo(ibbatch broadcast, string msg)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Info(message);
        }

        public void InfoLogWithBatchInfo(bcstque4 broadcast, string msg, Exception ex)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Info(message, ex);
        }

        public void InfoLogWithBatchInfo(ibbatch broadcast, string msg, Exception ex)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Info(message, ex);
        }

        public void WarnLogWithBatchInfo(bcstque4 broadcast, string msg)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Warn(message);
        }

        public void WarnLogWithBatchInfo(ibbatch broadcast, string msg)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Warn(message);
        }

        public void WarnLogWithBatchInfo(bcstque4 broadcast, string msg, Exception ex)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Warn(message, ex);
        }

        public void WarnLogWithBatchInfo(ibbatch broadcast, string msg, Exception ex)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Warn(message, ex);
        }

        public void ErrorLogWithBatchInfo(bcstque4 broadcast, string msg)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Error(message);
        }

        public void ErrorLogWithBatchInfo(ibbatch broadcast, string msg)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Error(message);
        }

        public void ErrorLogWithBatchInfo(bcstque4 broadcast, string msg, Exception ex)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Error(message, ex);
        }

        public void ErrorLogWithBatchInfo(ibbatch broadcast, string msg, Exception ex)
        {
            var message = FormatMessageWithBroadcastInfo(broadcast, msg);
            _log.Error(message, ex);
        }

        public string FormatBroadcastInfo(bcstque4 broadcast)
        {
            return
                $"Batch:[{broadcast.batchnum} - {broadcast.batchname.Trim()}] | Agency:[{broadcast.agency.Trim()}] | User:[{broadcast.UserNumber}]";
        }

        public string FormatBroadcastInfo(ibbatch broadcast)
        {
            return
                $"Batch:[{broadcast.batchnum} - {broadcast.batchname.Trim()}] | Agency:[{broadcast.agency.Trim()}] | User:[{broadcast.UserNumber}]";
        }

        public string FormatMessageWithBroadcastInfo(bcstque4 broadcast, string msg)
        {
            return $"{FormatBroadcastInfo(broadcast)} | Message:[{msg}]";
        }

        public string FormatMessageWithBroadcastInfo(ibbatch broadcast, string msg)
        {
            return $"{FormatBroadcastInfo(broadcast)} | Message:[{msg}]";
        }
    }
}
