using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public static class LookupColumnTableConductor
    {
        public static string LookupColumnValue(UserReportColumnInformation column, RawData row, int seqNo, UserReportInformation userReport,
                                         bool isPreview, UserDefinedLookupManager lookupHelper, ColumnValueRulesFactory factory)
        {
            switch (column.TableName)
            {
                case "TRIPS":
                case "IBTRIPS":
                case "HIBTRIPS":
                case "ACCTSPCL":
                case "TRIPTLS":
                case "ONDTRIPS":
                    return (column.Name == "PLUSMIN") && seqNo > 0
                               ? "0"
                               : lookupHelper.HandleLookupFieldTrip(column, row, userReport.TripSummaryLevel, factory);
                case "AUTO":
                    return lookupHelper.HandleLookupFieldCar(column, row, seqNo);
                case "HOTEL":
                    return lookupHelper.HandleLookupFieldHotel(isPreview, column, row, seqNo);
                case "TRAVAUTH":
                    return lookupHelper.HandleLookupFieldTravAuth(column, row);
                case "AUTHRZR":
                    return lookupHelper.HandleLookupFieldAuthorizer(column, row);
                case "CHGLOG":
                    return lookupHelper.HandleLookupFieldChangeLog(column, row, seqNo);
                case "SVCFEE":
                case "HIBSVCFEES":
                    return lookupHelper.HandleLookupFieldServiceFee(column, row, seqNo);
                case "LEGS":
                    return lookupHelper.HandleLookupFieldLeg(column, row, seqNo, factory);
                case "AIRLEG":
                    return lookupHelper.HandleLookupFieldLegAir(column, row, seqNo);
                case "RAIL":
                    return lookupHelper.HandleLookupFieldLegRail(column, row, seqNo);
                case "ONDMSEGS":
                    return lookupHelper.HandleLookupFieldMktSegs(column, row, seqNo, factory);
                case "MISCSEGS":
                    return lookupHelper.HandleLookupFieldMiscSegs(column, row);
                case "MSTUR":
                    return lookupHelper.HandleLookupFieldMiscSegsTour(column, row, seqNo);
                case "MSSEA":
                    return lookupHelper.HandleLookupFieldMiscSegsCruise(column, row, seqNo);
                case "MSLIM":
                    return lookupHelper.HandleLookupFieldMiscSegsLimo(column, row, seqNo);
                case "MSRAL":
                    return lookupHelper.HandleLookupFieldMiscSegsRailTicket(column, row, seqNo, factory);
                case "DUMMY":
                    return column.Name == "LEGPLUSMIN" ? row.PlusMin.ToString() : (seqNo + 1).ToString();
                default:
                    return "UNKNOWN TABLE: " + column.TableName;
            }
        }
    }
}
