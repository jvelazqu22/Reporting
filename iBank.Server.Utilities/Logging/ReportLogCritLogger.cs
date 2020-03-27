using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Helper;
using Domain.Orm.iBankMastersCommands;

using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.Logging
{
    public class ReportLogCritLogger
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int ReportLogNumber { get; set; }

        public ReportLogCritLogger(int reportLogNumber)
        {
            ReportLogNumber = reportLogNumber;
        }

        public ibRptLogCrit CreateRecord(string name, string value, string varOperator, bool isAdvancedCriteria)
        {
            return new ibRptLogCrit
            {
                rptlogno = ReportLogNumber,
                varname = name,
                varvalue = value,
                varoper = varOperator,
                advoptn = isAdvancedCriteria
            };
        }

        public IList<ibRptLogCrit> GetReportCriteriaRecords(Dictionary<int, ReportCriteria> reportParameters, string suppressCriteria, DateTime? beginDate, DateTime? endDate)
        {
            var records = new List<ibRptLogCrit>();
            foreach (var reportParameter in reportParameters)
            {
                if (reportParameter.Value.VarName.EqualsIgnoreCase("UDRKEY") && reportParameter.Value.VarValue.EqualsIgnoreCase("0")) continue;

                records.Add(CreateRecord(reportParameter.Value.VarName, reportParameter.Value.VarValue, "", false));
            }

            if (suppressCriteria.EqualsIgnoreCase("ON"))
            {
                records.Add(CreateRecord("SUPPRESSCRIT", "ON", "", false));
            }

            if (beginDate.HasValue && !records.Any(x => x.varname.EqualsIgnoreCase("BEGDATE")))
            {
                records.Add(CreateRecord("BEGDATE", beginDate.Value.ToShortDateString(), "", false));
            }

            if (endDate.HasValue && !records.Any(x => x.varname.EqualsIgnoreCase("ENDDATE")))
            {
                records.Add(CreateRecord("ENDDATE", endDate.Value.ToShortDateString(), "", false));
            }

            return records;
        }

        public IList<ibRptLogCrit> GetAdvancedCriteriaRecords(AdvancedParameters advancedParameters)
        {
            var records = new List<ibRptLogCrit>();
            foreach (var parm in advancedParameters.Parameters)
            {
                var value = (parm.Operator == Operator.Between || parm.Operator == Operator.NotBetween)
                                ? string.Format("{0},{1}", parm.Value1, parm.Value2)
                                : parm.Value1;
                records.Add(CreateRecord(parm.FieldName, value, parm.Operator.ToFriendlyString(), true));
            }

            //AndOr option
            if (advancedParameters.Parameters.Count > 1)
            {
                records.Add(CreateRecord("AOCANDOR", ((int)advancedParameters.AndOr).ToString(CultureInfo.InvariantCulture),
                    "=", true));
            }

            return records;
        }

        public IList<ibRptLogCrit> GetSystemVariableRecords(int batchNumber, string versionNumber)
        {
            var records = new List<ibRptLogCrit>();

            if (batchNumber != 0)
            {
                records.Add(CreateRecord("BATCHNUM", batchNumber.ToString(CultureInfo.InvariantCulture), "", false));
            }

            records.Add(CreateRecord("EXE-VERSION", versionNumber, "", false));

            return records;
        }

        public void AddRecords(IList<ibRptLogCrit> recordsToAdd, ICommandDb mastersCommandDb)
        {
            try
            {
                var addRptLogCritCmd = new AddRptLogCritCommand(mastersCommandDb, recordsToAdd);
                addRptLogCritCmd.ExecuteCommand();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        var msg = string.Format("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        LOG.Error(msg, dbEx);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLogger.LogException(null, null, e, MethodBase.GetCurrentMethod(), ServerType.Unknown, LOG);
                Console.WriteLine(e);
            }
        }
    }
}
