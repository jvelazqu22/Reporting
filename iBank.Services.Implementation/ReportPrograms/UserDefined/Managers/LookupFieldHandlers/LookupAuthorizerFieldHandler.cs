using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupAuthorizerFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IClientDataStore _clientStore;
        public List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private readonly BuildWhere _buildWhere;

        public SegmentOrLeg _segmentOrLeg;

        public LookupAuthorizerFieldHandler(UserDefinedParameters userDefinedParams, IClientDataStore clientStore, SegmentOrLeg segmentOrLeg)
        {
            _userDefinedParams = userDefinedParams;
            _clientStore = clientStore;
            _segmentOrLeg = segmentOrLeg;
            TripSummaryLevel = new List<Tuple<string, string>>();
        }

        public string HandleLookupFieldAuthorizer(UserReportColumnInformation column, RawData mainRec)
        {
            var rec = _userDefinedParams.TravAuthorizerLookup[mainRec.RecKey].FirstOrDefault();

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "AAUTHSTAT":
                    switch (rec.Authstatus.Trim().ToUpper())
                    {
                        case "P":
                            return "'PENDING'";
                        case "N":
                            return "'NOTIFY'";
                        case "I":
                            return "'IN PROCESS'";
                        case "D":
                            return "'DECLINED'";
                        case "A":
                            return "'APPROVED'";
                        case "E":
                            return "'EXPIRED'";
                        case "C":
                            return "'CANCEL'";
                    }
                    break;
                case "AUTHEMAIL":
                    var authUserNumber = rec.Authrzrnbr.TryIntParse(-1);
                    if (authUserNumber > 0)
                    {
                        var authUserQuery = new GetUserByUserNumberQuery(_clientStore.ClientQueryDb, authUserNumber);
                        var authUser = authUserQuery.ExecuteQuery();
                        if (authUser != null)
                        {
                            return $"'{authUser.emailaddr.Trim()}'";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(rec.Auth1Email.Trim()))
                        {
                            return $"'{rec.Auth1Email.Trim()}'";
                        }

                        if (!string.IsNullOrEmpty(rec.Auth2Email.Trim()))
                        {
                            return $"'{rec.Auth2Email.Trim()}'";
                        }
                    }

                    break;
                case "AUTHNAME":

                    authUserNumber = rec.Authrzrnbr.TryIntParse(-1);
                    if (authUserNumber > 0)
                    {
                        var authUserQuery = new GetUserByUserNumberQuery(_clientStore.ClientQueryDb, authUserNumber);
                        var authUser = authUserQuery.ExecuteQuery();
                        if (authUser != null)
                        {
                            return $"'{authUser.firstname.Trim()} {authUser.lastname.Trim()}'";
                        }
                    }
                    else
                    {
                        //check via email address1 and agency
                        if (!string.IsNullOrEmpty(rec.Auth1Email.Trim()))
                        {
                            var authUserQuery = new GetUserByUserNumberQuery(_clientStore.ClientQueryDb, authUserNumber);
                            var authUser = authUserQuery.ExecuteQuery();
                            if (authUser != null)
                            {
                                return $"'{authUser.firstname.Trim()} {authUser.lastname.Trim()}'";
                            }
                        }
                        else
                        {
                            var authUserQuery = new GetUserByUserNumberQuery(_clientStore.ClientQueryDb, authUserNumber);
                            var authUser = authUserQuery.ExecuteQuery();
                            if (authUser != null)
                            {
                                return $"'{authUser.firstname.Trim()} {authUser.lastname.Trim()}'";
                            }
                        }
                    }
                    break;
                case "ASTATUSTIM":
                    return rec.Statustime;
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }

            return string.Empty;
        }
    }
}
