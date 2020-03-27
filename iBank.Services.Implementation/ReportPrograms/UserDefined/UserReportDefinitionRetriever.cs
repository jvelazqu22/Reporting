using Domain.Helper;

using iBank.Server.Utilities.Classes;

using System.Collections.Generic;
using System.Linq;

using Domain.Exceptions;
using Domain.Orm.iBankClientQueries;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class UserReportDefinitionRetriever
    {
        private readonly ReportGlobals _globals;
        public readonly IList<collist2> collist2;

        private readonly IUserReportUpdater _updater;
        

        public UserReportDefinitionRetriever(ReportGlobals globals, IQuery<IList<collist2>> collist2Query)
        {
            _globals = globals;
            collist2 = collist2Query.ExecuteQuery();
            _updater = new UserReportUpdater();
        }

        public UserReportDefinitionRetriever(ReportGlobals globals, IQuery<IList<collist2>> collist2Query, IUserReportUpdater updater)
        {
            _globals = globals;
            collist2 = collist2Query.ExecuteQuery();
            _updater = updater;
        }

        public UserReportInformation LoadUserReportInformation(IClientDataStore clientStore, int reportKey)
        {
            var userReportQuery = new GetUserReportQuery(clientStore.ClientQueryDb, reportKey);
            var userReportRecord = userReportQuery.ExecuteQuery();

            if (userReportRecord == null) throw new ReportNotFoundException($"Unable to locate the user defined report via report key {reportKey}.");

            _updater.UpdateLastUsed(userReportRecord, clientStore);

            return new UserReportInformation
            {
                ReportKey = userReportRecord.reportkey,
                Agency = userReportRecord.agency.Trim(),
                ReportName = userReportRecord.crname.Trim(),
                UserId = userReportRecord.userid,
                ReportType = userReportRecord.crtype.Trim(),
                ReportTitle = userReportRecord.crtitle.Trim(),
                ReportSubtitle = userReportRecord.crsubtit.Trim(),
                PageFooterText = userReportRecord.pgfoottext.Trim(),
                Theme = userReportRecord.theme.Trim(),
                LastUsed = userReportRecord.lastused,
                // this is to refer to userrpts.segmentleg field: true/1 is segment, false/0 is leg
                // select segmentleg from userrpts where reportkey = 3429. This is very confusing, but it works. 
                SegmentOrLeg = userReportRecord.segmentleg == 0 ? SegmentOrLeg.Leg : SegmentOrLeg.Segment,
                UserNumber = userReportRecord.UserNumber,
                NoDetail = userReportRecord.nodetail,
                SuppressDetail = userReportRecord.nodetail,
                TripSummaryLevel = userReportRecord.tripsumlvl

            };
        }

        private static readonly List<string> _agencyColumns = new List<string> { "ACOMMISN", "CCOMMISN", "HCOMMISSN" };
        public List<UserReportColumnInformation> LoadUserReportColumnData(IQuery<IList<userrpt2>> getUserRpt2Query )
        {

            var columns = getUserRpt2Query.ExecuteQuery();

            if (!columns.Any()) throw new MissingUserDefinedColumnsException($"No fields stored for user defined report. Report Key [{_globals.GetParmValue(WhereCriteria.UDRKEY)}");

            if (_globals.User.AllowAgencyReports)
            {
                return columns.OrderBy(s => s.colorder)
                    .Select(s => new UserReportColumnInformation
                    {
                        Name = s.colname.Trim(),
                        Order = s.colorder,
                        Sort = s.sort ?? 0,
                        GroupBreak = s.grpbreak ?? 0,
                        SubTotal = s.subtotal,
                        PageBreak = s.pagebreak,
                        Header1 = s.udidhdg1.Trim(),
                        Header2 = s.udidhdg2.Trim(),
                        Width = s.udidwidth ?? 0,
                        UdidType = s.udidtype,
                        HorizontalAlignment = s.horAlign,
                        GoodField = s.goodfld.Trim(),
                        GoodOperator = s.goodoper.Trim(),
                        GoodHilite = s.goodhilite.Trim(),
                        GoodValue = s.goodvalue,
                        BadField = s.badfld.Trim(),
                        BadOperator = s.badoper.Trim(),
                        BadHilite = s.badhilite.Trim(),
                        BadValue = s.badvalue,
                        TlsOnly = IsTlsOnly(s.colname)
                    }).ToList();
            }
            else
            {
                return columns.Where(s => !_agencyColumns.Contains(s.colname.Trim()))
                     .OrderBy(s => s.colorder)
                     .Select(s => new UserReportColumnInformation
                     {
                         Name = s.colname.Trim(),
                         Order = s.colorder,
                         Sort = s.sort ?? 0,
                         GroupBreak = s.grpbreak ?? 0,
                         SubTotal = s.subtotal,
                         PageBreak = s.pagebreak,
                         Header1 = s.udidhdg1.Trim(),
                         Header2 = s.udidhdg2.Trim(),
                         Width = s.udidwidth ?? 0,
                         UdidType = s.udidtype,
                         HorizontalAlignment = s.horAlign,
                         GoodField = s.goodfld.Trim(),
                         GoodOperator = s.goodoper.Trim(),
                         GoodHilite = s.goodhilite.Trim(),
                         GoodValue = s.goodvalue,
                         BadField = s.badfld.Trim(),
                         BadOperator = s.badoper.Trim(),
                         BadHilite = s.badhilite.Trim(),
                         BadValue = s.badvalue,
                         TlsOnly = IsTlsOnly(s.colname)
                     }).ToList();
            }
        }

        public bool IsTlsOnly(string colname)
        {
            var col = collist2.FirstOrDefault(x => x.colname == colname);
            if (col == null) return false;
            else return col.tlsonly;
        }

        public bool SuppressDuplicates(IList<UserReportColumnInformation> columns)
        {
            return columns.Join(collist2.Where(s => UserReportCheckLists.TablesToCheck.Contains(s.coltable)), u => u.Name, c => c.colname, (u, c) => c.colname)
                .Count() == 2;
        }
        
    }
}
